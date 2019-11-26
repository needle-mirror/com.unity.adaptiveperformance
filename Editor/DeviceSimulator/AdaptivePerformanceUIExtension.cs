#if DEVICE_SIMULATOR_ENABLED
using System;
using Unity.DeviceSimulator;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.AdaptivePerformance.Simulator;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance
{
    public class AdaptivePerformanceUIExtension : IDeviceSimulatorExtension
    {
        public string extensionTitle { get { return "Adaptive Performance"; } }

        public void OnExtendDeviceSimulator(VisualElement visualElement)
        {
            var container = new VisualElement();
            visualElement.Add(container);

            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.unity.adaptiveperformance/Editor/DeviceSimulator/AdaptivePerformanceExtension.uxml");
            visualElement.Add(tree.CloneTree());

            var foldout = visualElement as Foldout;
            foldout.value = true;

            statusFlag = visualElement.Q<TextField>("flag-status");
            thermalFoldout = visualElement.Q<Foldout>("thermal");
            warningLevel = visualElement.Q<EnumField>("thermal-warning-level");
            temperatureLevel = visualElement.Q<Slider>("thermal-temperature-level");
            temperatureLevelField = visualElement.Q<FloatField>("thermal-temperature-level-field");
            temperatureTrend = visualElement.Q<Slider>("thermal-temperature-trend");
            temperatureTrendField = visualElement.Q<FloatField>("thermal-temperature-trend-field");
            performanceFoldout = visualElement.Q<Foldout>("performance");
            controlAutoMode = visualElement.Q<Toggle>("performance-control-auto-mode");
            cpuLevel = visualElement.Q<SliderInt>("performance-cpu-level");
            cpuLevelField = visualElement.Q<IntegerField>("performance-cpu-level-field");
            gpuLevel = visualElement.Q<SliderInt>("performance-gpu-level");
            gpuLevelField = visualElement.Q<IntegerField>("performance-gpu-level-field");
            bottleneck = visualElement.Q<EnumField>("performance-bottleneck");
            devLogging = visualElement.Q<Toggle>("dev-logging");
            devLoggingFrequency = visualElement.Q<IntegerField>("dev-log-freq");
            developerFoldout = visualElement.Q<Foldout>("developer-options");
            

            warningLevel.RegisterCallback<ChangeEvent<Enum>>(evt =>
            {
                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.WarningLevel = (WarningLevel)evt.newValue;
            });
            temperatureLevel.RegisterCallback<ChangeEvent<float>>(evt =>
            {
                temperatureLevelField.value = evt.newValue;
                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.TemperatureLevel = evt.newValue;
            });
            temperatureLevelField.RegisterCallback<ChangeEvent<float>>(evt =>
            {
                var newTemperatureLevel = evt.newValue;
                if (newTemperatureLevel < temperatureLevel.lowValue)
                {
                    newTemperatureLevel = temperatureLevel.lowValue;
                    temperatureLevelField.value = newTemperatureLevel;
                }
                if (newTemperatureLevel > temperatureLevel.highValue)
                {
                    newTemperatureLevel = temperatureLevel.highValue;
                    temperatureLevelField.value = newTemperatureLevel;
                }

                temperatureLevel.value = newTemperatureLevel;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.TemperatureLevel = newTemperatureLevel;
            });
            temperatureTrend.RegisterCallback<ChangeEvent<float>>(evt =>
            {
                temperatureTrendField.value = evt.newValue;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.TemperatureTrend = evt.newValue;
            });
            temperatureTrendField.RegisterCallback<ChangeEvent<float>>(evt =>
            {
                var newTemperatureTrend = evt.newValue;
                if (newTemperatureTrend < temperatureTrend.lowValue)
                {
                    newTemperatureTrend = temperatureTrend.lowValue;
                    temperatureTrendField.value = newTemperatureTrend;
                }
                if (newTemperatureTrend > temperatureTrend.highValue)
                {
                    newTemperatureTrend = temperatureTrend.highValue;
                    temperatureTrendField.value = newTemperatureTrend;
                }

                temperatureTrend.value = newTemperatureTrend;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.TemperatureTrend = newTemperatureTrend;
            });
            controlAutoMode.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                var ap = Holder.Instance;
                if (ap == null)
                    return;

                var ctrl = ap.DevicePerformanceControl;
                ctrl.AutomaticPerformanceControl = evt.newValue;
            });
            cpuLevel.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                // sync value field
                cpuLevelField.value = evt.newValue;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.CpuPerformanceLevel = evt.newValue;
            });
            cpuLevelField.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                var newCPULevel = evt.newValue;
                if (newCPULevel < cpuLevel.lowValue)
                {
                    newCPULevel = cpuLevel.lowValue;
                    cpuLevelField.SetValueWithoutNotify(newCPULevel);
                }
                if (newCPULevel > cpuLevel.highValue)
                {
                    newCPULevel = cpuLevel.highValue;
                    cpuLevelField.SetValueWithoutNotify(newCPULevel);
                }

                cpuLevel.value = newCPULevel;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.CpuPerformanceLevel = newCPULevel;
            });
            gpuLevel.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                // sync value field
                gpuLevelField.value = evt.newValue;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.GpuPerformanceLevel = evt.newValue;
            });
            gpuLevelField.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                var newGPULevel = evt.newValue;
                if (newGPULevel < gpuLevel.lowValue)
                {
                    newGPULevel = gpuLevel.lowValue;
                    gpuLevelField.SetValueWithoutNotify(newGPULevel);
                }
                if (newGPULevel > gpuLevel.highValue)
                {
                    newGPULevel = gpuLevel.highValue;
                    gpuLevelField.SetValueWithoutNotify(newGPULevel);
                }

                gpuLevel.value = newGPULevel;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.GpuPerformanceLevel = newGPULevel;
            });
            bottleneck.RegisterCallback<ChangeEvent<Enum>>(evt =>
            {
                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                SetBottleneck((PerformanceBottleneck)evt.newValue, subsystem);
            });
            devLogging.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                var ap = Holder.Instance;
                if (ap == null)
                    return;
                var devSettings = ap.DevelopmentSettings;

                devSettings.Logging = evt.newValue;
            });
            devLoggingFrequency.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                var ap = Holder.Instance;
                if (ap == null)
                    return;
                var devSettings = ap.DevelopmentSettings;

                devSettings.LoggingFrequencyInFrames = evt.newValue;
            });

            EditorApplication.playModeStateChanged += LogPlayModeState;
            thermalFoldout.SetEnabled(false);
            performanceFoldout.SetEnabled(false);
            developerFoldout.SetEnabled(false);
            SyncAPSubsystemSettingsToEditor();
        }

        private TextField statusFlag;
        private Foldout thermalFoldout;
        private EnumField warningLevel;
        private Slider temperatureLevel;
        private FloatField temperatureLevelField;
        private Slider temperatureTrend;
        private FloatField temperatureTrendField;
        private Foldout performanceFoldout;
        private Toggle controlAutoMode;
        private SliderInt cpuLevel;
        private IntegerField cpuLevelField;
        private SliderInt gpuLevel;
        private IntegerField gpuLevelField;
        private EnumField bottleneck;
        private Foldout developerFoldout;
        private Toggle devLogging;
        private IntegerField devLoggingFrequency;

        private void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                statusFlag.style.visibility = Visibility.Hidden;
                statusFlag.style.position = Position.Absolute;
                thermalFoldout.SetEnabled(true);
                performanceFoldout.SetEnabled(true);
                developerFoldout.SetEnabled(true);
                SyncAPSubsystemSettingsToEditor();
            }
            else
            {
                statusFlag.style.visibility = Visibility.Visible;
                statusFlag.style.position = Position.Relative;
                thermalFoldout.SetEnabled(false);
                performanceFoldout.SetEnabled(false);
                developerFoldout.SetEnabled(false);
            }
        }

        private void SyncAPSubsystemSettingsToEditor()
        {
            var ap = Holder.Instance;
            if (ap == null)
                return;

            var ctrl = ap.DevicePerformanceControl;
            var devSettings = ap.DevelopmentSettings;
            var perfMetrics = ap.PerformanceStatus.PerformanceMetrics;
            var thermalMetrics = ap.ThermalStatus.ThermalMetrics;

            warningLevel.value = thermalMetrics.WarningLevel;
            temperatureLevel.value = thermalMetrics.TemperatureLevel;
            temperatureLevelField.value = thermalMetrics.TemperatureLevel;
            temperatureTrend.value = thermalMetrics.TemperatureTrend;
            temperatureTrendField.value = thermalMetrics.TemperatureTrend;
            controlAutoMode.value = ctrl.AutomaticPerformanceControl;
            cpuLevel.value = ctrl.CpuLevel;
            cpuLevel.highValue = ctrl.MaxCpuPerformanceLevel;
            cpuLevel.lowValue = 0;
            cpuLevelField.value = ctrl.CpuLevel;
            gpuLevel.value = ctrl.GpuLevel;
            gpuLevel.highValue = ctrl.MaxGpuPerformanceLevel;
            gpuLevel.lowValue = 0;
            gpuLevelField.value = ctrl.GpuLevel;
            bottleneck.value = perfMetrics.PerformanceBottleneck;
            devLogging.value = devSettings.Logging;
            devLoggingFrequency.value = devSettings.LoggingFrequencyInFrames;
            // Set bottleneck so we get CPU/GPU frametimes and a valid bottleneck
            SetBottleneck((PerformanceBottleneck)bottleneck.value, Subsystem());
        }

        private void SetBottleneck(PerformanceBottleneck performanceBottleneck, SimulatorAdaptivePerformanceSubsystem subsystem)
        {
            if (subsystem == null)
                return;

            var targetFrameRate = Application.targetFrameRate;

            // default target framerate is -1 to use default platform framerate so we assume it's 60
            if(targetFrameRate == -1)
            {
                targetFrameRate = 60;
            }

            var currentTargetFramerateHalfMS = 1.0f / targetFrameRate / 2.0f;
            var currentTargetFramerateMS = 1.0f / targetFrameRate;
            switch (performanceBottleneck)
            {
                case PerformanceBottleneck.CPU: // averageOverallFrametime > targetFramerate && averageCpuFrametime >= averageOverallFrametime
                    subsystem.NextCpuFrameTime = currentTargetFramerateMS;
                    subsystem.NextGpuFrameTime = currentTargetFramerateHalfMS;
                    subsystem.NextOverallFrameTime = currentTargetFramerateMS + 0.001f;
                    break;
                case PerformanceBottleneck.GPU: // averageOverallFrametime > targetFramerate && averageGpuFrametime >= averageOverallFrametime
                    subsystem.NextCpuFrameTime = currentTargetFramerateHalfMS;
                    subsystem.NextGpuFrameTime = currentTargetFramerateMS;
                    subsystem.NextOverallFrameTime = currentTargetFramerateMS + 0.001f;
                    break;
                case PerformanceBottleneck.TargetFrameRate: // averageOverallFrametime == targetFramerate
                    subsystem.NextCpuFrameTime = currentTargetFramerateHalfMS;
                    subsystem.NextGpuFrameTime = currentTargetFramerateHalfMS;
                    subsystem.NextOverallFrameTime = currentTargetFramerateMS;
                    break;
                case PerformanceBottleneck.Unknown: // averageOverallFrametime > targetFramerate
                default:
                    subsystem.NextCpuFrameTime = currentTargetFramerateHalfMS;
                    subsystem.NextGpuFrameTime = currentTargetFramerateHalfMS;
                    subsystem.NextOverallFrameTime = currentTargetFramerateMS + 0.001f;
                    break;
            }
        }

        private SimulatorAdaptivePerformanceSubsystem Subsystem()
        {
            if (!Application.isPlaying)
                return null;

            return StartupSettings.PreferredSubsystem as SimulatorAdaptivePerformanceSubsystem;
        }
    }

    public static class AdaptivePerformanceSimulatorSetup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            StartupSettings.PreferredSubsystem = SimulatorAdaptivePerformanceSubsystem.Initialize();
        }
    }
}
#endif
