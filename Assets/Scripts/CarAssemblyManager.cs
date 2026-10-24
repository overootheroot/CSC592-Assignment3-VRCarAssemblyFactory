
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CarAssemblyManager : MonoBehaviour
{
    [Header("Scene References")]
    public Transform assemblyCar;
    public Transform assemblyParts;
    public Transform snapTargets;
    public GameObject completedCar;

    [Header("Parts Tables")]
    public Transform table1;
    public Transform table2;

    [Header("Table Layout")]
    public int columnsPerTable = 4;
    public float partClearance = 0.01f;
    public float tableEdgePadding = 0.35f;

    [Header("Windows Display Fix")]
    [Range(0.1f, 1f)]
    public float windshieldDisplayScale = 0.55f;
    public Vector3 windshieldOffset =
        new Vector3(-0.15f, 0f, -0.15f);

    [Header("Small Light Parts")]
    [Range(0.1f, 1f)]
    public float lightDisplayScale = 0.6f;
    public Vector3 lightDisplayRotation = Vector3.zero;
    public float lightHeightAdjustment = 0f;

    [Header("VR Snapping")]
    public float snapDistance = 0.65f;
    public float turnLightSnapDistance = 1.0f;
    public Color ghostColor =
        new Color(0f, 1f, 0.3f, 0.25f);

    [Header("Visual Guidance")]
    public Color highlightColor = Color.green;
    public Color labelColor = Color.white;
    public float labelHeight = 0.45f;
    public float labelFontSize = 5f;
    public float labelScale = 0.14f;

    [Header("Conveyor Animation")]
    public Vector3 conveyorDirection = Vector3.right;
    public float conveyorSpeed = 0.8f;
    public float conveyorTravelDistance = 4f;

    [Header("Completion Effect")]
    public float popHeight = 0.8f;
    public float popDuration = 0.7f;

    private readonly string[] partNames =
    {
        "Interior",
        "Steeringwheel",
        "Windows",
        "FL_Door",
        "FR_Door",
        "RL_Door",
        "RR_Door",
        "FL_Wheel",
        "FR_Wheel",
        "RL_Wheel",
        "RR_Wheel",
        "Front_Lights",
        "Brake_Lights",
        "Turn_Lights_l",
        "Turn_Lights_r"
    };

    private class PartData
    {
        public string name;
        public Transform part;
        public Transform target;
        public Vector3 originalWorldScale;
        public Quaternion originalRotation;
        public Renderer[] renderers;
        public Material[][] originalMaterials;
        public Rigidbody body;
        public XRGrabInteractable grab;
        public GameObject ghost;
        public TextMeshPro label;
        public bool installed;
        public bool isHeld;
    }

    private readonly List<PartData> parts =
        new List<PartData>();

    private int currentStep;
    private bool completed;
    private Camera playerCamera;
    private TextMeshPro progressLabel;

    private void Start()
    {
        if (!assemblyCar || !assemblyParts ||
            !table1 || !table2)
        {
            Debug.LogError(
                "Assign AssemblyCar, AssemblyParts, " +
                "Table1 and Table2 on GameManager."
            );

            enabled = false;
            return;
        }

        if (!snapTargets)
        {
            snapTargets =
                FindDeepChild(assemblyCar, "SnapTargets");

            if (!snapTargets)
            {
                GameObject root =
                    new GameObject("SnapTargets");

                snapTargets = root.transform;
                snapTargets.SetParent(assemblyCar, false);
            }
        }

        playerCamera = Camera.main;

        if (!PrepareParts())
        {
            enabled = false;
            return;
        }

        ArrangePartsOnTables();

        if (completedCar)
            completedCar.SetActive(false);

        CreateProgressLabel();

        currentStep = 0;
        completed = false;

        ShowCurrentStep();

        Debug.Log(
            "VR assembly ready: " +
            parts.Count + " parts."
        );
    }

    private bool PrepareParts()
    {
        parts.Clear();

        List<Transform> foundParts =
            new List<Transform>();

        // Find all original parts first.
        foreach (string name in partNames)
        {
            Transform part =
                FindDeepChild(assemblyParts, name);

            if (!part)
                part = FindDeepChild(assemblyCar, name);

            if (!part)
            {
                Debug.LogError(
                    "Missing actual car part: " + name
                );

                return false;
            }

            foundParts.Add(part);
        }

        // Save snap positions before moving parts.
        for (int i = 0; i < partNames.Length; i++)
        {
            string name = partNames[i];
            Transform part = foundParts[i];

            Transform target =
                FindDeepChild(snapTargets, name);

            if (!target)
            {
                GameObject targetObject =
                    new GameObject(name);

                target = targetObject.transform;

                target.SetParent(snapTargets, true);
                target.position = part.position;
                target.rotation = part.rotation;

                Debug.Log(
                    "Created SnapTarget: " + name
                );
            }

            PartData data = new PartData();

            data.name = name;
            data.part = part;
            data.target = target;
            data.originalWorldScale = part.lossyScale;
            data.originalRotation = part.rotation;

            data.renderers =
                part.GetComponentsInChildren<Renderer>(true);

            data.originalMaterials =
                new Material[data.renderers.Length][];

            for (int r = 0; r < data.renderers.Length; r++)
            {
                data.originalMaterials[r] =
                    data.renderers[r].sharedMaterials;
            }

            data.body = part.GetComponent<Rigidbody>();

            if (!data.body)
            {
                data.body =
                    part.gameObject.AddComponent<Rigidbody>();
            }

            data.body.useGravity = false;
            data.body.isKinematic = true;

            Collider[] colliders =
                part.GetComponentsInChildren<Collider>(true);

            bool hasCollider = false;

            foreach (Collider c in colliders)
            {
                if (c.enabled && !c.isTrigger)
                {
                    hasCollider = true;
                    break;
                }
            }

            if (!hasCollider)
            {
                BoxCollider box =
                    part.gameObject.AddComponent<BoxCollider>();

                Bounds bounds = GetBounds(part);

                box.center =
                    part.InverseTransformPoint(bounds.center);

                Vector3 scale = part.lossyScale;

                box.size = new Vector3(
                    bounds.size.x /
                    Mathf.Max(Mathf.Abs(scale.x), 0.001f),

                    bounds.size.y /
                    Mathf.Max(Mathf.Abs(scale.y), 0.001f),

                    bounds.size.z /
                    Mathf.Max(Mathf.Abs(scale.z), 0.001f)
                );
            }

            data.grab =
                part.GetComponent<XRGrabInteractable>();

            if (!data.grab)
            {
                data.grab =
                    part.gameObject.AddComponent<XRGrabInteractable>();
            }

            data.grab.movementType =
                XRBaseInteractable.MovementType.Kinematic;

            data.grab.throwOnDetach = false;
            data.grab.enabled = false;

            PartData captured = data;

            data.grab.selectEntered.AddListener(
                args => OnPartGrabbed(captured)
            );

            data.grab.selectExited.AddListener(
                args => OnPartReleased(captured)
            );

            parts.Add(data);
        }

        foreach (PartData data in parts)
        {
            data.part.SetParent(assemblyParts, true);
        }

        return true;
    }

    private void ArrangePartsOnTables()
    {
        const int firstTableCount = 7;

        int columns =
            Mathf.Max(1, columnsPerTable);

        for (int i = 0; i < parts.Count; i++)
        {
            PartData data = parts[i];

            bool firstTable = i < firstTableCount;

            Transform table =
                firstTable ? table1 : table2;

            int index =
                firstTable ? i : i - firstTableCount;

            int count = firstTable
                ? firstTableCount
                : parts.Count - firstTableCount;

            int rows = Mathf.Max(
                1,
                Mathf.CeilToInt((float)count / columns)
            );

            int column = index % columns;
            int row = index / columns;

            Bounds tableBounds = GetBounds(table);

            float margin = Mathf.Min(
                tableEdgePadding,
                tableBounds.size.x * 0.1f,
                tableBounds.size.z * 0.1f
            );

            float cellWidth = Mathf.Max(
                0.05f,
                (tableBounds.size.x - margin * 2f)
                / columns
            );

            float cellDepth = Mathf.Max(
                0.05f,
                (tableBounds.size.z - margin * 2f)
                / rows
            );

            float x =
                tableBounds.min.x +
                margin +
                (column + 0.5f) * cellWidth;

            float z =
                tableBounds.min.z +
                margin +
                (row + 0.5f) * cellDepth;

            Transform part = data.part;

            part.rotation = data.originalRotation;

            SetWorldScale(
                part,
                data.originalWorldScale
            );

            if (data.name == "Windows")
            {
                SetWorldScale(
                    part,
                    data.originalWorldScale *
                    windshieldDisplayScale
                );
            }

            if (IsSmallLight(data.name))
            {
                SetWorldScale(
                    part,
                    data.originalWorldScale *
                    lightDisplayScale
                );

                part.rotation =
                    Quaternion.Euler(lightDisplayRotation);
            }

            // NEW: Fit the whole part inside its cell.
            Bounds bounds = GetBounds(part);

            float fit = Mathf.Min(
                1f,
                (cellWidth * 0.78f) /
                Mathf.Max(bounds.size.x, 0.001f),

                (cellDepth * 0.78f) /
                Mathf.Max(bounds.size.z, 0.001f)
            );

            if (fit < 1f)
            {
                SetWorldScale(
                    part,
                    part.lossyScale * fit
                );
            }

            bounds = GetBounds(part);

            part.position += new Vector3(
                x - bounds.center.x,
                tableBounds.max.y +
                    partClearance - bounds.min.y,
                z - bounds.center.z
            );

            if (data.name == "Windows")
            {
                part.position += windshieldOffset;
                AlignToTable(part, tableBounds);
            }

            if (IsSmallLight(data.name))
            {
                part.position +=
                    Vector3.up * lightHeightAdjustment;
            }

            // NEW: Prevent parts sticking off edges.
            bounds = GetBounds(part);

            float minX = tableBounds.min.x + margin;
            float maxX = tableBounds.max.x - margin;
            float minZ = tableBounds.min.z + margin;
            float maxZ = tableBounds.max.z - margin;

            float dx = 0f;
            float dz = 0f;

            if (bounds.min.x < minX)
                dx = minX - bounds.min.x;
            else if (bounds.max.x > maxX)
                dx = maxX - bounds.max.x;

            if (bounds.min.z < minZ)
                dz = minZ - bounds.min.z;
            else if (bounds.max.z > maxZ)
                dz = maxZ - bounds.max.z;

            part.position += new Vector3(dx, 0f, dz);
        }
    }

    private void OnPartGrabbed(PartData data)
    {
        if (completed || data.installed)
            return;

        data.isHeld = true;

        SetPartLabel(
            data,
            DisplayName(data.name) +
            "\nMOVE TO GREEN OUTLINE"
        );

        Debug.Log("Grabbed: " + data.name);
    }

    private void OnPartReleased(PartData data)
    {
        if (completed || data.installed)
            return;

        data.isHeld = false;

        if (currentStep >= parts.Count)
            return;

        if (parts[currentStep] != data)
            return;

        float distance = Vector3.Distance(
            GetBounds(data.part).center,
            GetTargetVisualCenter(data)
        );

        float allowedDistance =
            IsTurnLight(data.name)
            ? turnLightSnapDistance
            : snapDistance;

        if (distance <= allowedDistance)
        {
            StartCoroutine(InstallAfterRelease(data));
        }
        else
        {
            SetPartLabel(
                data,
                DisplayName(data.name) + "\nGRAB ME"
            );
        }
    }

    private IEnumerator InstallAfterRelease(PartData data)
    {
        yield return null;

        if (completed || data.installed)
            yield break;

        if (currentStep >= parts.Count)
            yield break;

        if (parts[currentStep] != data)
            yield break;

        if (data.grab.isSelected)
            yield break;

        InstallPart(data);
    }

    private void InstallPart(PartData data)
    {
        data.installed = true;
        data.grab.enabled = false;

        if (data.body)
        {
            data.body.linearVelocity = Vector3.zero;
            data.body.angularVelocity = Vector3.zero;
            data.body.isKinematic = true;
        }

        RestoreMaterials(data);

        if (data.ghost)
            Destroy(data.ghost);

        if (data.label)
            Destroy(data.label.gameObject);

        data.part.SetParent(assemblyCar, true);

        data.part.position = data.target.position;
        data.part.rotation = data.target.rotation;

        SetWorldScale(
            data.part,
            data.originalWorldScale
        );

        currentStep++;

        Debug.Log(
            "Installed: " + data.name
        );

        if (currentStep >= parts.Count)
        {
            StartCoroutine(FinishAssembly());
        }
        else
        {
            ShowCurrentStep();
        }
    }

    private void ShowCurrentStep()
    {
        if (currentStep >= parts.Count)
            return;

        PartData data = parts[currentStep];

        data.grab.enabled = true;

        HighlightPart(data);
        CreateGhost(data);
        CreatePartLabel(data);
        UpdateProgressLabel();

        Debug.Log(
            "STEP " + (currentStep + 1) +
            " / " + parts.Count +
            ": Install " + data.name
        );
    }

    private void HighlightPart(PartData data)
    {
        foreach (Renderer renderer in data.renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor(
                        "_BaseColor",
                        highlightColor
                    );
                }
                else if (material.HasProperty("_Color"))
                {
                    material.color = highlightColor;
                }
            }
        }
    }

    private void CreateGhost(PartData data)
    {
        if (data.ghost)
            Destroy(data.ghost);

        data.ghost = new GameObject(
            data.name + "_PlacementGhost"
        );

        data.ghost.transform.SetPositionAndRotation(
            data.target.position,
            data.target.rotation
        );

        foreach (Renderer source in data.renderers)
        {
            MeshFilter sourceFilter =
                source.GetComponent<MeshFilter>();

            if (!sourceFilter || !sourceFilter.sharedMesh)
                continue;

            GameObject meshObject = new GameObject(
                source.gameObject.name + "_GhostMesh"
            );

            meshObject.transform.SetParent(
                data.ghost.transform,
                false
            );

            Matrix4x4 relative =
                data.part.worldToLocalMatrix *
                source.transform.localToWorldMatrix;

            meshObject.transform.localPosition =
                relative.GetColumn(3);

            meshObject.transform.localRotation =
                Quaternion.Inverse(data.part.rotation) *
                source.transform.rotation;

            Vector3 sourceScale =
                source.transform.lossyScale;

            Vector3 partScale =
                data.part.lossyScale;

            meshObject.transform.localScale =
                new Vector3(
                    sourceScale.x /
                    Mathf.Max(Mathf.Abs(partScale.x), 0.001f),

                    sourceScale.y /
                    Mathf.Max(Mathf.Abs(partScale.y), 0.001f),

                    sourceScale.z /
                    Mathf.Max(Mathf.Abs(partScale.z), 0.001f)
                );

            MeshFilter filter =
                meshObject.AddComponent<MeshFilter>();

            filter.sharedMesh =
                sourceFilter.sharedMesh;

            MeshRenderer ghostRenderer =
                meshObject.AddComponent<MeshRenderer>();

            Material[] sourceMaterials =
                source.sharedMaterials;

            Material[] ghostMaterials =
                new Material[sourceMaterials.Length];

            for (int i = 0; i < sourceMaterials.Length; i++)
            {
                if (!sourceMaterials[i])
                    continue;

                Material mat =
                    new Material(sourceMaterials[i]);

                if (mat.HasProperty("_Surface"))
                {
                    mat.SetFloat("_Surface", 1f);

                    if (mat.HasProperty("_Blend"))
                        mat.SetFloat("_Blend", 0f);

                    if (mat.HasProperty("_ZWrite"))
                        mat.SetFloat("_ZWrite", 0f);

                    mat.EnableKeyword(
                        "_SURFACE_TYPE_TRANSPARENT"
                    );

                    mat.renderQueue = 3000;
                }

                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", ghostColor);
                else if (mat.HasProperty("_Color"))
                    mat.color = ghostColor;

                ghostMaterials[i] = mat;
            }

            ghostRenderer.materials = ghostMaterials;
        }

        SetWorldScale(
            data.ghost.transform,
            data.originalWorldScale
        );
    }

    private Vector3 GetTargetVisualCenter(PartData data)
    {
        if (data.ghost)
            return GetBounds(data.ghost.transform).center;

        return data.target.position;
    }

    private void CreatePartLabel(PartData data)
    {
        GameObject labelObject = new GameObject(
            data.name + "_FloatingLabel"
        );

        data.label =
            labelObject.AddComponent<TextMeshPro>();

        data.label.fontSize = labelFontSize;
        data.label.color = labelColor;

        data.label.alignment =
            TextAlignmentOptions.Center;

        data.label.textWrappingMode =
            TextWrappingModes.NoWrap;

        data.label.rectTransform.sizeDelta =
            new Vector2(10f, 3f);

        labelObject.transform.localScale =
            Vector3.one * labelScale;

        SetPartLabel(
            data,
            DisplayName(data.name) + "\nGRAB ME"
        );
    }

    private void SetPartLabel(
        PartData data,
        string message
    )
    {
        if (data.label)
            data.label.text = message;
    }

    private void CreateProgressLabel()
    {
        GameObject labelObject =
            new GameObject("AssemblyProgressLabel");

        progressLabel =
            labelObject.AddComponent<TextMeshPro>();

        progressLabel.fontSize = 6f;
        progressLabel.color = Color.white;

        progressLabel.alignment =
            TextAlignmentOptions.Center;

        progressLabel.rectTransform.sizeDelta =
            new Vector2(12f, 3f);

        progressLabel.transform.localScale =
            Vector3.one * 0.16f;
    }

    private void UpdateProgressLabel()
    {
        if (!progressLabel)
            return;

        if (currentStep < parts.Count)
        {
            progressLabel.text =
                "STEP " + (currentStep + 1) +
                " / " + parts.Count +
                "\nINSTALL " +
                DisplayName(parts[currentStep].name);
        }
        else
        {
            progressLabel.text =
                "ASSEMBLY FINISHED!";
        }
    }

    private void LateUpdate()
    {
        if (!playerCamera)
            playerCamera = Camera.main;

        if (!completed && currentStep < parts.Count)
        {
            PartData data = parts[currentStep];

            if (data.label)
            {
                Bounds bounds = GetBounds(data.part);

                data.label.transform.position =
                    new Vector3(
                        bounds.center.x,
                        bounds.max.y + labelHeight,
                        bounds.center.z
                    );

                FaceCamera(data.label.transform);

                if (data.isHeld)
                {
                    float distance = Vector3.Distance(
                        bounds.center,
                        GetTargetVisualCenter(data)
                    );

                    float allowedDistance =
                        IsTurnLight(data.name)
                        ? turnLightSnapDistance
                        : snapDistance;

                    string instruction =
                        distance <= allowedDistance
                        ? "RELEASE TO INSTALL"
                        : "MOVE TO GREEN OUTLINE";

                    SetPartLabel(
                        data,
                        DisplayName(data.name) +
                        "\n" + instruction
                    );
                }
            }
        }

        if (progressLabel)
        {
            Vector3 position =
                completed && completedCar
                ? completedCar.transform.position
                : assemblyCar.position;

            progressLabel.transform.position =
                position + Vector3.up * 2.8f;

            FaceCamera(progressLabel.transform);
        }
    }

    private void FaceCamera(Transform label)
    {
        if (!playerCamera)
            return;

        Vector3 direction =
            label.position -
            playerCamera.transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            label.rotation =
                Quaternion.LookRotation(
                    direction.normalized,
                    Vector3.up
                );
        }
    }

    private string DisplayName(string name)
    {
        return name.Replace(
            "_", " "
        ).ToUpperInvariant();
    }

    private void RestoreMaterials(PartData data)
    {
        for (int i = 0; i < data.renderers.Length; i++)
        {
            data.renderers[i].sharedMaterials =
                data.originalMaterials[i];
        }
    }

    private bool IsSmallLight(string name)
    {
        return name == "Front_Lights" ||
               name == "Brake_Lights" ||
               name == "Turn_Lights_l" ||
               name == "Turn_Lights_r";
    }

    private bool IsTurnLight(string name)
    {
        return name == "Turn_Lights_l" ||
               name == "Turn_Lights_r";
    }

    private void AlignToTable(
        Transform part,
        Bounds tableBounds
    )
    {
        Bounds bounds = GetBounds(part);

        float correction =
            tableBounds.max.y +
            partClearance -
            bounds.min.y;

        part.position +=
            Vector3.up * correction;
    }

    private void SetWorldScale(
        Transform part,
        Vector3 desiredWorldScale
    )
    {
        Vector3 parentScale =
            part.parent
            ? part.parent.lossyScale
            : Vector3.one;

        part.localScale = new Vector3(
            desiredWorldScale.x /
            Mathf.Max(Mathf.Abs(parentScale.x), 0.0001f),

            desiredWorldScale.y /
            Mathf.Max(Mathf.Abs(parentScale.y), 0.0001f),

            desiredWorldScale.z /
            Mathf.Max(Mathf.Abs(parentScale.z), 0.0001f)
        );
    }

    private Bounds GetBounds(Transform root)
    {
        Renderer[] renderers =
            root.GetComponentsInChildren<Renderer>(true);

        bool found = false;

        Bounds bounds =
            new Bounds(root.position, Vector3.zero);

        foreach (Renderer renderer in renderers)
        {
            if (!renderer.enabled ||
                !renderer.gameObject.activeInHierarchy)
                continue;

            if (!found)
            {
                bounds = renderer.bounds;
                found = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        if (!found)
        {
            bounds = new Bounds(
                root.position,
                Vector3.one * 0.1f
            );
        }

        return bounds;
    }

    private Transform FindDeepChild(
        Transform parent,
        string childName
    )
    {
        if (!parent)
            return null;

        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform found =
                FindDeepChild(child, childName);

            if (found)
                return found;
        }

        return null;
    }

    private IEnumerator FinishAssembly()
    {
        completed = true;
        UpdateProgressLabel();

        Debug.Log("All 15 parts assembled!");

        if (!completedCar)
            yield break;

        completedCar.SetActive(true);
        assemblyCar.gameObject.SetActive(false);

        Transform car = completedCar.transform;

        Vector3 restingPosition = car.position;
        Vector3 originalScale = car.localScale;

        car.position =
            restingPosition - Vector3.up * popHeight;

        car.localScale =
            originalScale * 0.8f;

        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(
                0f,
                1f,
                Mathf.Clamp01(
                    elapsed /
                    Mathf.Max(0.01f, popDuration)
                )
            );

            car.position = Vector3.Lerp(
                restingPosition - Vector3.up * popHeight,
                restingPosition,
                t
            );

            car.localScale = Vector3.Lerp(
                originalScale * 0.8f,
                originalScale,
                t
            );

            yield return null;
        }

        car.position = restingPosition;
        car.localScale = originalScale;

        yield return new WaitForSeconds(0.3f);

        Vector3 start = car.position;

        Vector3 direction =
            conveyorDirection.sqrMagnitude > 0.001f
            ? conveyorDirection.normalized
            : Vector3.right;

        Vector3 end =
            start + direction * conveyorTravelDistance;

        float duration =
            conveyorTravelDistance /
            Mathf.Max(0.01f, conveyorSpeed);

        elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / duration
            );

            car.position =
                Vector3.Lerp(start, end, t);

            yield return null;
        }

        car.position = end;

        Debug.Log("ASSEMBLY FINISHED!");
    }
}
