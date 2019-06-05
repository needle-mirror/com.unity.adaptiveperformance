# Using Adaptive Performance samples

Adaptive Performance ships with samples to help you integrate all package functionality into your Project.

When you install the Adaptive Performance package, Unity automatically downloads its associated samples. Import these samples into your Project to see example uses of the Adaptive Performance APIs you can use as a starting point, or to test and verify results on your device.

While running, each sample will log information about the current state and any decisions being made in the system. The [Android Logcat package](https://docs.unity3d.com/Packages/com.unity.mobile.android-logcat@latest) is an excellent way to monitor current activity when using Adaptive Performance on Android. Each sample also defines its own end conditions. When those conditions are met, the application will exit. You can find all messages, state changes, temperatures, etc. in the log.

## Installation

Install Adaptive Performance samples from the **Package Manager** window. The following samples are available:

- [Sample Environment](#sample-environment)
- [Thermal Sample](#thermal-sample)
- [Bottleneck Sample](#bottleneck-sample)
- [VRR Sample](#vrr-sample)
- [Automatic Performance Control Sample](#automatic-performance-control-sample)

## Sample Environment
The sample environment is required for most of the samples, because the samples share the same assets. You should install the sample environment before installing any other samples to have the necessary base structure available.

## Thermal Sample
This sample demonstrates how to register and respond to thermal state change events from the Adaptive Performance API. The thermal states can be difficult to trigger when a device is just sitting on a desk, because devices are designed to prevent overheating. You might need to set the device on something warm, keep it in your hands, or find another way to prevent heat loss on the device to see the effects.

This sample is heavy on GPU usage to produce enough heat and then be able to cool down quickly afterward to activate the different warning levels. These levels are:

- Nominal - The device is cool enough to operate at full CPU speed.
- Throttling Imminent - The device is heating up and trending toward the need to throttle the CPU and GPU soon to prevent overheating.
- Throttling - The device has overheated and the CPU and GPU speeds have been throttled until the temperate drops down to a safe level.

The sample also has sliders to indicate the current temperature trend and level. When the device reaches a throttling state, the sample will stop drawing most objects to begin a fast cool off period until it is trending back toward a nominal state.

## Bottleneck Sample
This sample demonstrates the use of the bottleneck detection API. Adaptive Performance can detect three bottleneck types: CPU, GPU, and target framerate.

The sample runs in a rotation of the three possible states:

- Targeting CPU Bottleneck - Disable GPU heavy tasks, enable CPU heavy tasks.
- Targeting GPU Bottleneck - Disable CPU heavy tasks, enable GPU heavy tasks.
- Targeting Framerate - Disable CPU and GPU heavy tasks.

While the sample runs, information about the bottleneck state is stored in a list of Marker structs. Each Marker contains the time, a label, and the number of objects that were active at that time. When the sample has finished running, all of the Markers are written to the log.

If the currently targeted bottleneck has been stable for at least 5 seconds, the system saves a Marker with the status information. After waiting 3 seconds to let the device settle, more information is saved, then test switches to the next bottleneck.

### Options

The BottleneckControl Prefab in the Bottleneck scene enables you to configure several attributes of how the test operates:

|**Option**|**Description**|
|:---|:---|
|CPU and GPU loaders|These are Prefabs that will be spawned until the target bottleneck is reached.|
|Time Out|How long (in seconds) to wait for each bottleneck to be reached before switching to the next one.|
|Wait Time Before Switch|How many seconds before starting the next part of the test.|
|Wait Time After Target Reached|How long to wait (in seconds) after meeting the target bottleneck.|
|State Change Iterations|How many times the sample iterates through each bottleneck type.|

## VRR Sample
Some devices are capable of changing the screen's refresh rate through device settings or at runtime with Adaptive Performance. This sample shows some moving objects with a dropdown menu to change the refresh rate so you can see how the refresh rate affects the smoothness of motion on the screen. The slider lets you adjust the target framerate.

At lower refresh rates, the motion of the objects might appear a little choppy. It will get smoother as you increase the screen's refresh rate.

At the time of writing, the only device to support Variable Refresh Rate is the Samsung S20+ with GameSDK 3.2 or later.

## Automatic Performance Control Sample
The Automatic Performance Control sample shows how the Automatic Performance Control features of Adaptive Performance works, using two different prefabs that have a medium and high CPU load respectively. Both scenes target 60 FPS and attempt to keep the frame rate stable. If the device reports that throttling is imminent, the sample drops the framerate to 45 and Adaptive Performance automatically reduces the load on the device until it cools down and stops throttling.

The AutomaticPerformanceControl scene has an AutoPerfControl Prefab that allows you to modify some parameters of the test:

|**Option**|**Description**|
|:---|:---|
|High Level, Mid Level and No Load|References to Prefabs which are used for each test stage.|
|Test Timeout|Time in seconds for the test to run.|
|Auto Control Mode|You can disable the Adaptive Performance auto mode to see what effect the system has on thermal and bottleneck performance.|
|State Text|Reference to the UI Text element to display status.|
|Logging Active|A toggle to enable or disable logging.|
|Logging Frequency|The interval between writing messages to the log, in seconds.|

The TestSequence component has some additional options for configuring how the tests run:

|**Option**|**Description**|
|:---|:---|
|Auto Mode|The test will loop through the defined sequence until the device reaches the Throttling Imminent thermal state  at which point target FPS is set to 45. The timeout is defined in the Auto Perf Control component.|
|Test Sequence|The order in which test levels should load, and how long they should run, in seconds.|

## Adaptive Framerate
The Adaptive Framerate sample shows how the rendering rate can be adjusted dynamically at runtime. This feature uses the Indexer system to make decisions on when and how much to increase or decrease the rendering rate to maintain performance and thermal stability. The sample uses the same content as the Auto Performance Control sample. It switches between high and medium CPU load while using no load for 15 seconds in between each load set to allow the framerate to come back up.

### Options
The Adaptive Framerate sample scene has two GameObjects named **AdaptiveFramerate** and **VariableRefreshRateScaler**. By default the **AdaptiveFramerate** GameObject is enabled to use Application.TargetFramerate for managing the framerate. Alternatively you may use Variable Refresh Rate on supported devices. To do so disable the **AdaptiveFramerate** GameObject and enable the **VariableRefreshRateScaler** GameObject.

Both scalers allow setting a minimum and maximum framerate for the scene via a double ended slider.

# Technical details
## Requirements

Most samples are designed for Unity built-in render pipeline. If you use another render pipeline, you need to convert the Assets. For example, if you use the Universal Render Pipeline, go to **Edit &gt; Render Pipeline &gt; Universal Render Pipeline &gt; Upgrade Project Materials to UniversalRP Materials** from Unity's main menu.

## Project Settings

To enable proper timing for Adaptive Performance, you need to enable the **Frame Timing Stats** option (menu: **Edit &gt; Project Settings &gt; Player &gt; Other Settings**).

If you want to use `Application.targetFrameRate` to limit the target frame rate, set the **VSync Count** option under **Edit &gt; Project Settings &gt; Quality &gt; Other** to **Don't Sync**.
