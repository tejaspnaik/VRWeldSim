# VR Welding Simulator Demo

This project is a functional prototype of a VR welding simulator built in Unity. It demonstrates the ability to interact with a virtual environment, use a welding tool to create dynamic 3D weld seams, and receive realistic visual and audio feedback.

## Features
* **VR Interaction:** Grab and manipulate the welding tool using standard VR controllers.
* **Dynamic Weld Creation:** Create a 3D weld seam by moving the active welder along a surface. The system supports both flat planes and corner/T-joints.
* **Visual Effects:** Includes a particle system for sparks, and a "hot-to-cold" material effect where new welds glow and then cool down over time.
* **Audio Feedback:** A looping sizzle sound plays while the welder is active, enhancing immersion.
* **Mesh Merging:** After a weld pass is complete, all individual weld blobs can be merged into a single, optimized mesh.

## How to Use
1.  Launch the simulation in a VR headset or using the Unity Device Simulator.
2.  Grab the welding tool with your controller.
3.  Bring the tip of the welder into contact with a weldable surface.
4.  Pull and hold the trigger to start welding. Move the tool along the surface to create a seam.
5.  After completing a seam, press the **A Button** (Primary Button) on your controller to finalize the weld and merge the blobs.

## Setup Instructions

To run this project, you will need the following setup:

### 1. Unity Version
* **Unity 6000.1.7f1** or newer.

### 2. Required Packages
Install these packages from the Unity **Package Manager** (`Window > Package Manager`):

* **XR Interaction Toolkit:** The main package for all VR interactions. It provides the components for grabbing the welder (`XR Grab Interactable`) and handling controller inputs.
* **XR Plug-in Management:** This package acts as the bridge between the Unity engine and your VR hardware (like the Meta Quest).
* **Input System:** The modern system for managing all inputs from your VR controllers, keyboard, and mouse. It's a required dependency for the XR Interaction Toolkit.
* **ProBuilder:** The tool used to create and edit the 3D models in your scene, such as the welding surfaces.
* **Universal Render Pipeline (URP):** This is the rendering backbone of the project, required for modern visual effects.

### 3. Project Configuration
* In **Project Settings > XR Plug-in Management**, ensure the **Oculus** provider is enabled under the **Android** tab for Meta Quest compatibility.

## Credits & Assets
* 3D models for the environment and welder tool were sourced from **SketchFab** and the **Unity Asset Store**.
* The weld blob asset and initial script inspiration are from **wbigoljr's** Unity Welding Simulator project, which can be found here: [https://github.com/wbigoljr/unity_welding_simulator/tree/main](https://github.com/wbigoljr/unity_welding_simulator/tree/main)
