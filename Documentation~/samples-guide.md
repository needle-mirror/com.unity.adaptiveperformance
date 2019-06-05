**_Adaptive Performance Samples Guide_**

Adaptive Performance ships with samples to help you integrate all the techniques that comprise Adaptive Performance into your project.

# Using Adaptive Performance Samples

When you install the Adaptive Performance package, Unity automatically downloads Adaptive Performance samples. You can import them into your project to see how Adaptive Performance APIs can be used and to have a great starting point, or to test and verify results on your device.

As they run each sample will log information about the current state and any decisions being made in the system. The [Android Logcat package](https://docs.unity3d.com/Packages/com.unity.mobile.android-logcat@latest) is an excellent way to monitor the current activity when using Adaptive Performance on Android.

Each sample also defines conditions for the sample to finish. When those conditions are met the application will exit. All messages, state changes, temperatures, etc. can be found in the log.

## Installation

You can install the samples via the Package Manager by using the Adaptive Performance menu. You should install the sample environment before installing any other samples to have the necessary base structure available. Following samples are available and show specific use-cases of Adpative Performance.

- __[Sample Environment](#sample-environment)__
- __[Thermal Sample](#thermal-sample)__
- __[Bottleneck Sample](#bottleneck-sample)__
- __[VRR Sample](#vrr-sample)__
- __[Automatic Performance Control Sample](#automatic-performance-control-sample)__

## Sample Environment
The sample environment is required for most of the tests as the test share assets. You should install the sample environment before installing any other samples to have the necessary base structure available.

## Thermal Sample
This sample demonstrates how to register and respond to thermal state change events from the Adaptive Performance API. The thermal states can be difficult to trigger when a device is just sitting on a desk due to their design to prevent overheating. You may need to set the device on something warm, keep them in your hands or find another way to prevent heat loss on the device to see the effects.

This sample is heavy on GPU use to produce enough heat and then be able to cool down quickly afterward to activate the different warning levels.

Those levels are:

- Nominal - The device is cool enough to operate at full CPU speed.
- Throttling Imminent - The device is heating up and trending toward the need to throttle the CPU and GPU soon to prevent overheating.
- Throttling - The device has overheated and the CPU and GPU speeds have been throttled until the temperate drops down to a safe level.

The sample also has sliders to indicate the current temperature trend and level. When the device reaches a throttling state the sample will stop drawing most objects to begin a fast cool off period until it is trending back toward a nominal state.

## Bottleneck Sample
This sample demonstrates the use of the bottleneck detection API. There are 3 bottleneck types that can be detected: CPU, GPU, and target framerate.

The sample runs in a rotation of the 3 possible states:

- Targeting CPU Bottleneck - Disable GPU heavy tasks, enable CPU heavy tasks.
- Targeting GPU Bottleneck - Disable CPU heavy tasks, enable GPU heavy tasks.
- Targeting Framerate - Disable CPU and GPU heavy tasks.

Along the way information about the bottleneck state is stored in a list of Marker structs. Each Marker contains the time, a label and how many objects were active at that time. When the sample has finished all of the Markers are written to the log.

If the currently targeted bottleneck has been stable for at least 5 seconds a Marker is saved with the status information. After waiting 3 seconds to let the device settle, more information is saved and then test switches to the next bottleneck.

### Options

The BottleneckControl prefab in the Bottleneck scene enables you to configure several attributes about how the test operates.

- The CPU and GPU loaders: These are prefabs that will be spawned until the target bottleneck is reached.
- Time Out: How long (in seconds) to wait for each bottleneck to be reached before switching to the next one.
- Wait Time Before Switch: How many seconds before starting the next part of the test.
- Wait Time After Target Reached: How long to wait (in seconds) after meeting the target bottleneck.
- State Change Iterations: How many times the sample iterates through each bottleneck type.

## VRR Sample
Some devices are capable of changing the screens refresh rate through device settings or at runtime with Adaptive Performance. This sample shows some moving objects with a dropdown menu to change the refresh rate so you can see how the refresh rate affects how smooth motion is on the screen. The slider lets you adjust the target framerate.

At lower refresh rates the motion of the objects may appear a little choppy and will get more smooth as you increase the screens refresh rate.

At the time of writing the only device to support Variable Refresh Rate is the Samsung S20+ with GameSDK 3.2 or later.

## Automatic Performance Control Sample
Automatic Performance Control samples shows how the Automatic Performance Control features of Adaptive Performance works. We use two different prefabs that have a medium and high CPU load respectively. In both scenes we are targeting 60 FPS and attempting to keep it stable. If the device reports that throttling is imminent the sample drops the framerate to 45 and lets the automatic mode work do magic to reduce the load on the device until it cools down and stops throttling.

### Options

The AutomaticPerformanceControl scene has an AutoPerfControl prefab that allows you to modify some parameters of the test.

- High Level, Mid Level and No Load: References to prefabs which are used for each test stage.
- Test Timeout: Time in seconds for the test to run.
- Auto Control Mode: You may disable the Adaptive Performance Auto mode to see what effect the system has on thermal and bottleneck performance.
- State Text: Reference to the UI Text element to display status.
- Logging Active: A toggle to enable or disable logging.
- Logging Frequency: The interval between writing messages to the log, in seconds.

The TestSequence Component has some additional options for configuring how the tests run.

- Auto Mode: The test will loop through the defined sequence until the device reaches the ImminentThrottling thermal state  at which point target FPS is set to 45. The timeout is defined in the Auto Perf Control Component.
- Test Sequence: The order test levels should be loaded and how long they should run in seconds.

# Technical details
## Requirements

Most samples are designed for Unity built-in renderpipeline. If you use another render pipeline you need to convert the Assets. For instance, if you use Universal Render Pipeline you can simply convert the sample assets with __Edit > Render Pipeline > Universal Render Pipeline > Upgrade Project Materials to UniversalRP Materials__.

## Project Settings

You need to enable Frame Timing Stats under __Project Settings > Player > Other Settings > Enable Frame Timing Stats__ to enable proper timing for Adaptive Performance.

You need to use `Don't Sync` in the Quality Settings under __Project Settings > Quality > Other > Don't Sync__ if you want to limit the target framerate by using Application.targetFrameRate.
