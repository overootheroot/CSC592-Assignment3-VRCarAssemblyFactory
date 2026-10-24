# CSC592-Assignment3-VRCarAssemblyFactory
Unity  VR Car Assembly Factory with 15 interactive parts, guided assembly, VR grabbing, snapping, and conveyor animation
# VR Car Assembly Factory

**CSC 592 – Assignment 3 | Virtual Reality Assembly Project**

An interactive VR automotive assembly simulation developed using Unity 6.3 LTS, C#, and the XR Interaction Toolkit.

---

## 1. Team Members

1. **Jatin Repalli**
2. **Danny Sooza**

## 2. Project Overview

VR Car Assembly Factory is an interactive virtual reality application that simulates assembling a vehicle inside a virtual automotive manufacturing facility.

The application allows users to select, grab, move, and install individual vehicle components using VR controller interactions. Users follow a guided sequence supported by floating instructions, green component highlighting, transparent placement outlines, automatic snapping, and progress indicators.

The factory includes an assembly platform, conveyor belt, two parts-storage tables, factory flooring, industrial textures, and a vehicle separated into 15 interactive components.

After successfully installing all 15 components, the system displays a completed vehicle and triggers an animation that moves the car along the conveyor belt.

The project was developed and tested in Unity using the XR Interaction Simulator, allowing simulated VR controller interaction without a physical VR headset.

## 3. Project Objectives

The project aims to:

- Create a VR-ready automotive assembly environment.
- Implement controller-based grabbing and manipulation.
- Divide a vehicle into at least eight individually interactive parts.
- Extend the assembly to 15 parts for the master's-level bonus.
- Guide users through a predefined assembly sequence.
- Provide clear visual instructions throughout the process.
- Highlight the currently active component.
- Display transparent placement guides for accurate installation.
- Automatically snap parts into their designated positions.
- Track assembly progress.
- Display a completed-vehicle animation.
- Document the implementation, screenshots, and asset sources in GitHub.

## 4. Factory Environment

### Factory Floor

The factory floor provides an industrial setting for the assembly simulation.

Concrete-style textures are used to establish a recognizable manufacturing environment.

### Assembly Platform

The assembly platform represents the central location where the vehicle is constructed.

Each interactive component has a corresponding installation target that identifies its final position on the vehicle.

### Parts Storage Tables

Two storage tables hold the separated vehicle components.

The application distributes parts across the tables and adjusts their display sizes to keep them visible and accessible.

### Conveyor Belt

The conveyor belt represents the final stage of the assembly workstation.

After all components are installed, the completed vehicle is displayed and moves along the conveyor, providing visual confirmation that the task has been completed.

### Lighting and Industrial Materials

The scene uses Unity's Universal Render Pipeline and industrial-style materials to represent concrete, steel, rubber, and other factory surfaces.

## 5. Fifteen Vehicle Components

The assembly workflow contains the following 15 components:

| No. | Component | Description |
|---|---|---|
| 1 | Chassis | Main structural vehicle body |
| 2 | Front-left door | Front-left vehicle door |
| 3 | Front-right door | Front-right vehicle door |
| 4 | Rear-left door | Rear-left vehicle door |
| 5 | Rear-right door | Rear-right vehicle door |
| 6 | Front-left wheel | Front-left wheel assembly |
| 7 | Front-right wheel | Front-right wheel assembly |
| 8 | Rear-left wheel | Rear-left wheel assembly |
| 9 | Rear-right wheel | Rear-right wheel assembly |
| 10 | Front lights | Front lighting components |
| 11 | Brake lights | Rear brake lighting components |
| 12 | Left turn light | Left turn signal component |
| 13 | Right turn light | Right turn signal component |
| 14 | Steering wheel | Steering component |
| 15 | Windows | Vehicle window components |

The components are separated from the assembled model and distributed across the storage tables at runtime.

Each part has a designated installation position and participates in the guided assembly workflow.

## 6. Implemented Features

### 6.1 VR Controller Grabbing

The application uses Unity's XR Interaction Toolkit to enable users to interact with vehicle components.

The grabbing system uses:

- XR Grab Interactable components.
- Rigidbody components.
- Colliders.
- VR controller-based selection and release.

Users can grab the active component, move it through the virtual environment, and release it near its correct installation target.

### 6.2 Sequential Assembly

The application uses a predefined sequence to guide users through installing the components.

Only the currently required component is activated for the assembly step.

After successful installation, the system advances to the next component.

### 6.3 Component Highlighting

The active component is highlighted in green to help users identify the next part to assemble.

The highlight changes automatically as the assembly progresses.

### 6.4 Green Placement Outlines

Green outline indicates the correct installation position for the active component.

This visual guide helps users understand where the component should be placed.

### 6.5 Automatic Snapping

When a component is released sufficiently close to its installation target, the system automatically installs it in the correct position.

The snapping system:

- Checks the distance between the component and its target.
- Places the component at the designated location.
- Restores its installed size and appearance.
- Prevents further grabbing after installation.
- Advances the assembly sequence.

### 6.6 Automatic Parts Organization

The application arranges components across two storage tables.

The display size and position of each component are adjusted to keep the storage layout organized.

### 6.7 Assembly Progress Tracking

The application tracks the current assembly step out of 15 total steps.

The progress indicator updates automatically after each successful installation.

### 6.8 Completed Vehicle Animation

After all 15 components are installed:

1. The assembly manager detects completion.
2. The completed vehicle is displayed.
3. The vehicle performs a finishing animation.
4. The vehicle moves along the conveyor belt.
5. The completion message is displayed.

## 7. Floating Instructions and Guided Assembly

The application uses TextMeshPro to display floating instructions that guide users through each stage of the assembly process.

These instructions update automatically based on the current component and the user's interaction.

### Step 1 — Assembly Progress

**Displayed text: `STEP 1 / 15`**

The progress indicator shows the current assembly step and total number of components.

It updates after each successful installation:

`STEP 1 / 15` → `STEP 2 / 15` → ... → `STEP 15 / 15`

### Step 2 — Identify and Grab the Component

**Displayed text: `[PART NAME] / GRAB ME`**

The system identifies the currently active vehicle component and highlights it in green.

The user locates the highlighted part on the storage table and grabs it using the VR controller.

### Step 3 — Move Toward the Placement Guide

**Displayed text: `MOVE TO GREEN OUTLINE`**

After the user grabs the component, the instruction directs them toward the green transparent outline.

The outline represents the correct installation position.

The user moves the component toward this target while holding it.

### Step 4 — Release the Component

**Displayed text: `RELEASE TO INSTALL`**

When the component reaches the allowed installation range, the system instructs the user to release it.

Releasing the component near the target triggers automatic snapping.

The component is installed at its designated position.

### Step 5 — Continue to the Next Component

After successful installation:

1. The installed component remains in its correct position.
2. The progress indicator advances.
3. The next required component becomes active.
4. The new component is highlighted.
5. The floating instructions update.
6. The placement outline moves to the next installation target.

The process repeats until all 15 components are installed.

### Step 6 — Assembly Completion

**Displayed text: `ASSEMBLY FINISHED!`**

After the final component is installed, the application displays the completion message.

The completed vehicle appears and moves along the conveyor belt.

### Floating Instruction Summary

| Floating Instruction | Meaning | User Action |
|---|---|---|
| `STEP X / 15` | Displays current assembly progress | Follow the current step |
| `[PART NAME] / GRAB ME` | Identifies the active component | Grab the highlighted part |
| `MOVE TO GREEN OUTLINE` | Identifies the correct installation position | Move the component toward the green outline |
| `RELEASE TO INSTALL` | Indicates the component is close enough to install | Release the component |
| `ASSEMBLY FINISHED!` | Confirms successful completion | Observe the completed vehicle animation |

### Benefits of the Floating Instruction System

The floating instructions improve usability by:

- Providing clear, step-by-step guidance.
- Helping users identify the correct component.
- Reducing confusion when multiple parts are visible.
- Indicating where components should be installed.
- Providing feedback during grabbing and placement.
- Tracking progress through all 15 assembly steps.
- Confirming completion of the assembly.

## 8. Master's-Level Bonus

### BONUS 1 — Fifteen-Part Assembly

**Status: IMPLEMENTED**

The project implements 15 individually assembled vehicle components, exceeding the minimum requirement of eight parts.

Each component is included in the guided assembly workflow and has an installation target.

The user must complete all 15 steps before the final vehicle animation is triggered.

This satisfies the master's-level requirement to implement at least one bonus feature.

## 9. Project Architecture

The application uses a central C# script, `CarAssemblyManager.cs`, to coordinate the assembly workflow.

### CarAssemblyManager.cs Responsibilities

- Identifying vehicle components.
- Preparing installation targets.
- Separating parts from the original vehicle.
- Distributing parts across storage tables.
- Configuring grabbing and physics components.
- Managing the current active part.
- Displaying floating instructions.
- Updating component highlights.
- Displaying placement guides.
- Checking installation proximity.
- Snapping components into position.
- Tracking assembly progress.
- Triggering the final animation.

### Main Scene Objects

| Scene Object | Purpose |
|---|---|
| FactoryFloor | Industrial factory environment |
| AssemblyPlatform | Central vehicle assembly location |
| ConveyorBelt | Completed vehicle movement |
| AssemblyCar | Vehicle components and assembly structure |
| AssemblyParts | Container for interactive parts |
| SnapTargets | Installation positions |
| CompletedCar | Finished vehicle animation |
| PartsStorage | Storage tables for separated components |
| GameManager | Controls the assembly sequence |
| XR Interaction Manager | Manages XR interactions |
| XR Interaction Simulator | Simulated VR controller testing |
| XR Origin | XR camera and controller setup |

## 10. Technologies Used

| Technology | Purpose |
|---|---|
| Unity 6.3 LTS (6000.3.22f1) | Scene development and game engine |
| C# | Assembly and interaction logic |
| XR Interaction Toolkit | VR grabbing and manipulation |
| XR Interaction Simulator | Controller simulation |
| Universal Render Pipeline | Rendering and materials |
| TextMeshPro | Floating instructions and progress indicators |
| GitHub | Project repository |
| GitHub Desktop | Version control management |

## 11. Asset Credits and Attribution

The project uses third-party vehicle geometry and industrial PBR textures.

### 11.1 Vehicle Model — Low Poly Small Car

**Asset:** Low Poly Small Car  
**Platform:** Sketchfab  
**Source:** https://sketchfab.com/3d-models/low-poly-small-car-ebe7c5e98a7448b5abb2eaf0cb22b766

The model provides the vehicle geometry used for the interactive assembly workflow.

The car was configured in Unity to support separated components, grabbing, placement guides, automatic snapping, and assembly completion.

**Original creator and license:** Refer to the linked Sketchfab model page. Verify the creator's name and applicable license before final submission.

### 11.2 Concrete Texture — Concrete028

**Asset:** Concrete028  
**Provider:** ambientCG  
**License:** Creative Commons CC0 1.0  
**Source:** https://ambientcg.com/view?id=Concrete028

Used to provide concrete-style industrial surfaces in the factory environment.

**Credit:** ambientCG — Concrete028.

### 11.3 Steel/Metal Texture — Metal046A

**Asset:** Metal046A  
**Provider:** ambientCG  
**License:** Creative Commons CC0 1.0  
**Source:** https://ambientcg.com/view?id=Metal046A

Used to create metallic and steel-like industrial surfaces.

**Credit:** ambientCG — Metal046A.

### 11.4 Rubber Texture — Rubber003

**Asset:** Rubber003  
**Provider:** ambientCG  
**License:** Creative Commons CC0 1.0  
**Source:** https://ambientcg.com/view?id=Rubber003

Used for rubber-style surfaces and vehicle components.

**Credit:** ambientCG — Rubber003.

### 11.5 Additional Materials

The project also uses Unity materials and visual effects for vehicle surfaces, windows, lights, component highlighting, and transparent placement guides.

### 11.6 Unity Frameworks

The project uses Unity Engine, XR Interaction Toolkit, XR Interaction Simulator, Universal Render Pipeline, and TextMeshPro.

**Provider:** Unity Technologies  
**Documentation:** https://docs.unity3d.com/

### 11.7 Licensing Statement

The three ambientCG textures are available under the Creative Commons CC0 1.0 license.

Although CC0 does not require attribution, their sources are documented for academic acknowledgment.

The Sketchfab vehicle model remains subject to the license specified by its original creator.

## 12. Screenshots and Assembly Demonstration

The following screenshots demonstrate the major features of the VR Car Assembly Factory.

### Screenshot 1 — Factory Overview

**Filename:** `01_Factory_Overview.png`

Shows the overall factory environment, assembly platform, conveyor belt, and two storage tables containing the separated car components.

![Factory Overview]([Screenshots/01_Factory_Overview.png](https://github.com/overootheroot/CSC592-Assignment3-VRCarAssemblyFactory/blob/main/Screenshots/Screenshot%202026-10-23%20at%2010.13.12%E2%80%AFPM.png))

### Screenshot 2 — Guided Assembly Instructions

**Filename:** `02_Assembly_Instructions.png`

Shows the active highlighted component, floating component-name label, and current assembly progress.

The screenshot demonstrates how users are guided through the assembly workflow.

![Assembly Instructions](Screenshots/02_Assembly_Instructions.png)

### Screenshot 3 — VR Controller Grabbing

**Filename:** `03_VR_Grabbing.png`

Shows a simulated VR controller grabbing and manipulating an interactive vehicle component.

This demonstrates the XR Interaction Toolkit grabbing functionality.

![VR Grabbing](Screenshots/03_VR_Grabbing.png)

### Screenshot 4 — Snap Placement

**Filename:** `04_Snap_Placement.png`

Shows a component being positioned near its green placement outline.

The screenshot demonstrates the visual placement guide and installation workflow.

![Snap Placement](Screenshots/04_Snap_Placement.png)

### Screenshot 5 — Completed Assembly

**Filename:** `05_Completed_Assembly.png`

Shows the completed vehicle after the installation of all 15 components, including the final completion stage.

![Completed Assembly](Screenshots/05_Completed_Assembly.png)

## 13. Running the Project

### Requirements

- Unity Hub.
- Unity Editor 6000.3.22f1.
- Required Unity packages referenced by the project.
- A computer capable of running the Unity Editor.

### Setup Instructions

1. Clone or download the GitHub repository.
2. Open Unity Hub.
3. Select **Add project from disk**.
4. Choose the repository folder.
5. Open the project using Unity 6000.3.22f1.
6. Allow Unity to import assets and packages.
7. Open the `CarAssemblyFactory` scene.
8. Enable the intended XR Origin and XR Interaction Simulator.
9. Enter Play Mode.

### Assembly Instructions

1. Locate the highlighted component.
2. Grab it using the simulated VR controller.
3. Follow the floating instructions.
4. Move the component toward its green placement outline.
5. Release it near the correct installation position.
6. Allow the system to snap the component into place.
7. Continue to the next highlighted component.
8. Repeat until all 15 components are installed.
9. Observe the final completed-vehicle animation.

**Testing note:** The complete assembly workflow was tested using Unity's XR Interaction Simulator. Physical VR headset compatibility has not been independently verified.

## 14. Challenges and Solutions

### Organizing 15 Components

**Challenge:** Managing multiple components and installation targets.

**Solution:** A central assembly manager organizes parts, prepares targets, and controls the installation sequence.

### Providing Clear Visual Guidance

**Challenge:** Helping users identify the correct component and installation position.

**Solution:** Green highlighting, transparent placement outlines, floating instructions, and progress indicators guide the assembly process.

### Accurate Component Placement

**Challenge:** Positioning components accurately using VR controllers.

**Solution:** Proximity-based snapping installs components at their correct locations when released sufficiently close to their targets.

### Storage Table Organization

**Challenge:** Keeping differently sized vehicle components visible and accessible.

**Solution:** The application distributes components across two storage tables and adjusts their displayed sizes.

### Testing Without a Physical VR Headset

**Challenge:** Testing controller-based interactions without dedicated VR hardware.

**Solution:** The XR Interaction Simulator was used to test grabbing, placement, snapping, and the completed assembly workflow.

## 15. Assignment Rubric Coverage

| Rubric Category | Maximum Points | Implementation |
|---|---:|---|
| VR-Ready Assembly Scene | 25 | Factory environment, lighting, assembly platform, conveyor, and XR setup |
| Visual Cues | 25 | Green placement guides, highlighted components, floating instructions, and progress indicators |
| VR Controller Grabbing | 20 | XR Grab Interactable and simulated controller interactions |
| Eight or More Parts | 15 | Fifteen interactive components |
| README and Repository | 15 | Project description, documented features, screenshots, setup instructions, and asset credits |
| Master's Bonus | Additional points | Bonus 1: Fifteen-part assembly |

## 16. Conclusion

VR Car Assembly Factory demonstrates a guided automotive assembly experience using Unity and VR interaction technologies.

The project combines an industrial factory environment, 15 interactive vehicle components, VR controller-based manipulation, visual placement guides, floating instructions, automatic snapping, progress tracking, and a completed-vehicle conveyor animation.

The 15-part assembly workflow satisfies the master's-level bonus requirement while extending the core assignment requirements.

---

**Project:** VR Car Assembly Factory  
**Assignment:** CSC 592 – Assignment 3

**Team Members:**
1. **Jatin Repalli**
2. **Danny Sooza**

**External Asset Providers:** Sketchfab and ambientCG.
