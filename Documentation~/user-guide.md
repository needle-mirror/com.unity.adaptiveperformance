**_Adaptive Performance User Guide_**

# Using Adaptive Performance

When you install the Adaptive Performance package, Unity automatically creates a GameObject that implements `IAdaptivePerformance` in your Project at run time. To access the instance, use `UnityEngine.AdaptivePerformance.Holder.Instance`.

To check if your device supports Adaptive Performance, use the `Instance.Active` property. To get detailed information during runtime, enable debug logging in the provider settings or via the `Instance.DevelopmentSettings.Logging` during runtime or via boot time flags from the settings API:

```
static class AdaptivePerformanceConfig
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Setup()
    {
        IAdaptivePerformanceSettings settings = AdaptivePerformanceGeneralSettings.Instance.Manager.activeLoader.GetSettings();
        settings.logging = true;
    }
}
```

Unity enables AdaptivePerformance by default once you install the package and if it finds a suitable subsystem. A subsystem needs to be installed and checked in the settings to be added at built time and findable during runtime. To disable Adaptive Performance, disable the *Initialize Adaptive Performance* checkbox in the provider tab for the target platform.

![Samsung Android provider in the provider list is installed and ready to use. Initialize Adaptive Performance checkbox is installed](Images/installation-provider.png)

For a description of the detailed startup behavior of a subsystem please read the subsystem documentation.

## Performance Status

Adaptive Performance tracks several performance metrics and updates them every frame. To access these metrics, use the `Instance.PerformanceStatus`property.

### Frame timing

Adaptive Performance always tracks the average GPU, CPU, and overall frame times, and updates them every frame. You can access the latest timing data using the `PerformanceStatus.FrameTiming` property.

The overall frame time is the time difference between frames. You can use it to calculate the current framerate of the application.
The CPU time only includes the time the CPU is actually executing Unity's main thread and the render thread. It doesn’t include the times when Unity might be blocked by the operating system, or when Unity needs to wait for the GPU to catch up with rendering.
The GPU time is the time the GPU is actively processing data to render a frame. It doesn’t include the time when the GPU has to wait for Unity to provide data to render.

### Performance bottleneck

Adaptive Performance uses the currently configured target frame rate (see `UnityEngine.Application.targetFrameRate` and `QualitySettings`) and the information that `FrameTiming`provides to calculate what is limiting the frame rate of the application. If the application isn’t performing at the desired target framerate, it might be bound by either CPU or GPU processing. You can subscribe with a delegate function to the `PerformanceStatus.PerformanceBottleneckChangeEvent` event to get a notification whenever the current performance bottleneck of the application changes.

You can use the information about the current performance bottleneck to make targeted adjustments to the game content at run time. For example, in a GPU-bound application, lowering the rendering resolution often improves the frame rate significantly, but the same change might not make a big difference for a CPU-bound application.

## Device thermal state feedback

The Adaptive Performance API gives you access to the current thermal warning level of the device (`Instance.ThermalStatus.ThermalMetrics.WarningLevel`) and a more detailed temperature level (`Instance.ThermalStatus.ThermalMetrics.TemperatureLevel`). The application can make modifications based on these values to avoid the operating system throttling it.
The following example shows the implementation of a Unity component that uses Adaptive Performance feedback to adjust the global LOD bias:

```
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class AdaptiveLOD : MonoBehaviour
{
    private IAdaptivePerformance ap = null;

    void Start() {
        ap = Holder.Instance;
        if (!ap.Active)
            return;

        QualitySettings.lodBias = 1.0f;
        ap.ThermalStatus.ThermalEvent += OnThermalEvent;
    }

    void OnThermalEvent(ThermalMetrics ev) {
        switch (ev.WarningLevel) {
            case WarningLevel.NoWarning:
                QualitySettings.lodBias = 1;
                break;
            case WarningLevel.ThrottlingImminent:
                if (ev.temperatureLevel > 0.8f)
                    QualitySettings.lodBias = 0.75f;
                else
                    QualitySettings.lodBias = 1.0f;
                break;
            case WarningLevel.Throttling:
                QualitySettings.lodBias = 0.5f;
                break;
        }
    }
}
```

## Configuring CPU and GPU performance levels

The CPU and GPU consume the most power on a mobile device, especially when running a game. Typically, the operating system decides which clock speeds to use for the CPU and GPU. CPU cores and GPUs are less efficient when running at their maximum clock speed. When they run at high clock speeds, the mobile device overheats, and the operating system throttles CPU and GPU frequency to cool down the device.
By default, Adaptive Performance automatically configures CPU and GPU performance levels based on the current performance bottleneck.
Alternatively, you can switch to `Manual` mode; to do this, set  `Instance.DevicePerformanceControl.AutomaticPerformanceControl` to `false`. In `Manual` mode, you can use the `Instance.DevicePerformanceControl.CpuLevel` and `Instance.DevicePerformanceControl.GpuLevel` properties to optimize CPU and GPU performance. To check which mode you are currently in, use `Instance.DevicePerformanceControl.PerformanceControlMode`.

The application can configure these properties based on the thermal feedback and the frame time data that the Adaptive Performance API provides. It also uses these questions about its current performance requirements:
- Did the application reach the target frame rate in the previous frames?
- Is the application in an in-game scene, a loading screen, or a menu?
- Are device temperatures rising?
- Is the device close to thermal throttling?
- Is the device GPU or CPU bound?

*Note:* Changing GPU and GPU levels only has an effect as long as the device is not in thermal throttling state (`Instance.WarningLevel` equals `PerformanceWarningLevel.Throttling`).
In some situations, the device might take control over the CPU and GPU levels. This changes the value of `Instance.DevicePerformanceControl.PerformanceControlMode`
to `PerformanceControlMode.System`.

The following example shows how to reduce thermal pressure and power consumption by using the Adaptive Performance Automatic Performance Control. It adjusts the CPU and GPU levels based on your `targetFrameRate` and helps you to reduce power consumption, heat and current efficiently. Setting CPU and GPU level manually is not recommended instead you should use the Automatic Performance Control to achieve the best performance by setting the `targetFrameRate` only:

```
public void EnterMenu()
{
    if (!ap.Active)
        return;

    Application.targetFrameRate = 30;
    // Enable automatic regulation of CPU and GPU level by Adaptive Performance
    var ctrl = ap.DevicePerformanceControl;
    ctrl.AutomaticPerformanceControl = true;
}

public void EnterBenchmark()
{
    var ctrl = ap.DevicePerformanceControl;
    // Set higher CPU and GPU level when benchmarking a level
    ctrl.cpuLevel = ctrl.MaxCpuPerformanceLevel;
    ctrl.gpuLevel = ctrl.MaxGpuPerformanceLevel;
}
```

## Indexer

The Indexer is a Adaptive Performance system that tracks thermal and performance state and offers a quantified quality index. This index is used by Scalers to adjust quality. The quality control is achieved by changing different scaler levels.

Scaler can make a decisions by priorities supplied from the indexer using the following targets:
- Targets current bottleneck.
- Lowest level.
- Lowest visual impact.

### Using Indexer and Scaler

Scaler only work when Indexer is active. To activate the scaler check the 'Active' checkbox in Adaptive Performance settings __Edit > Project Settings > Adaptive Performance > {Provider} > Runtime Settings > Indexer Settings > Active__.

![Samsung Android provider in the provider list is installed and ready to use. Initialize Adaptive Performance checkbox is installed](Images/settings-provider-logging.png)

Add any Scaler into the scene which you want to control using the Indexer quality quantification.

### Scaler

A component that represents single feature that can be anything (etc. Graphics, Physics...).
Scaler controls its feature quality with levels and by default it starts with a zero. As the level increases, the feature quality decreases in LOD fashion..

### Standard Scalers

Adaptive Performance provides a few common Scaler.

General render Scaler:
- TextureQualityScaler
- LODScaler
- AdaptiveResolutionScaler

Universal Render Pipeline Scaler (These Scaler only work with `com.unity.render-pipelines.universal`, `7.5`, `8.2`, `9.0`):
- RenderScaleScaler
- DynamicBatchingScaler
- LutScaler
- MSAAScaler
- ShadowCascadeScaler
- ShadowDistanceScaler
- ShadowQualityScaler
- ShadowmapResolutionScaler
- SortingScaler

### Custom Scaler

In order to create custom scaler you need to create a new class that inherits `AdaptivePerformanceScaler`.

The following example shows scaler for controlling texture quality:
```
public class TextureQualityScaler : AdaptivePerformanceScaler
{
   public override ScalerVisualImpact VisualImpact => ScalerVisualImpact.High;
   public override ScalerTarget Target => ScalerTarget.GPU;
   public override int MaxLevel => 2;

   protected override void OnLevel()
   {
      switch (CurrentLevel)
      {
         case 0:
            QualitySettings.masterTextureLimit = 0;
            break;
         case 1:
            QualitySettings.masterTextureLimit = 1;
            break;
         case 2:
            QualitySettings.masterTextureLimit = 2;
            break;
       }
    }
}
```

# Technical details
## Requirements

This version of Adaptive Performance is compatible with Unity Editor versions 2019 LTS and later (2020.2 and later recommended).

To use Adaptive Performance, you must have at least one subsystem installed. See the [Installing Adaptive Performance](installing-and-configuring.md#installation) section in this documentation for more details.
