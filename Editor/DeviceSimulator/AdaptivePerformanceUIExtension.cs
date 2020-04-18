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
    public class AdaptivePerformanceUIExtension : IDeviceSimulatorExtension, ISerializationCallbackReceiver
    {
        public string extensionTitle { get { return "Adaptive Performance"; } }

        public void OnExtendDeviceSimulator(VisualElement visualElement)
        {
            m_ExtensionFoldout = visualElement as Foldout;

            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.unity.adaptiveperformance/Editor/DeviceSimulator/AdaptivePerformanceExtension.uxml");
            m_ExtensionFoldout.Add(tree.CloneTree());

            //m_StatusFlag = m_ExtensionFoldout.Q<TextField>("flag-status");
            m_ThermalFoldout = m_ExtensionFoldout.Q<Foldout>("thermal");
            m_ThermalFoldout.value = m_SerializationStates.thermalFoldout;
            m_WarningLevel = m_ExtensionFoldout.Q<EnumField>("thermal-warning-level");
            m_TemperatureLevel = m_ExtensionFoldout.Q<Slider>("thermal-temperature-level");
            m_TemperatureLevelField = m_ExtensionFoldout.Q<FloatField>("thermal-temperature-level-field");
            m_TemperatureTrend = m_ExtensionFoldout.Q<Slider>("thermal-temperature-trend");
            m_TemperatureTrendField = m_ExtensionFoldout.Q<FloatField>("thermal-temperature-trend-field");
            m_PerformanceFoldout = m_ExtensionFoldout.Q<Foldout>("performance");
            m_PerformanceFoldout.value = m_SerializationStates.performanceFoldout;
            m_ControlAutoMode = m_ExtensionFoldout.Q<Toggle>("performance-control-auto-mode");
            m_CpuLevel = m_ExtensionFoldout.Q<SliderInt>("performance-cpu-level");
            m_CpuLevelField = m_ExtensionFoldout.Q<IntegerField>("performance-cpu-level-field");
            m_GpuLevel = m_ExtensionFoldout.Q<SliderInt>("performance-gpu-level");
            m_GpuLevelField = m_ExtensionFoldout.Q<IntegerField>("performance-gpu-level-field");
            m_Bottleneck = m_ExtensionFoldout.Q<EnumField>("performance-bottleneck");
            m_DevLogging = m_ExtensionFoldout.Q<Toggle>("developer-logging");
            m_DevLoggingFrequency = m_ExtensionFoldout.Q<IntegerField>("developer-logging-frequency");
            m_DeveloperFoldout = m_ExtensionFoldout.Q<Foldout>("developer-options");
            m_DeveloperFoldout.value = m_SerializationStates.developerFoldout;

            m_WarningLevel.RegisterCallback<ChangeEvent<Enum>>(evt =>
            {
                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.WarningLevel = (WarningLevel)evt.newValue;
            });
            m_TemperatureLevel.RegisterCallback<ChangeEvent<float>>(evt =>
            {
                m_TemperatureLevelField.value = evt.newValue;
                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.TemperatureLevel = evt.newValue;
            });
            m_TemperatureLevelField.RegisterCallback<ChangeEvent<float>>(evt =>
            {
                var newTemperatureLevel = evt.newValue;
                if (newTemperatureLevel < m_TemperatureLevel.lowValue)
                {
                    newTemperatureLevel = m_TemperatureLevel.lowValue;
                    m_TemperatureLevelField.value = newTemperatureLevel;
                }
                if (newTemperatureLevel > m_TemperatureLevel.highValue)
                {
                    newTemperatureLevel = m_TemperatureLevel.highValue;
                    m_TemperatureLevelField.value = newTemperatureLevel;
                }

                m_TemperatureLevel.value = newTemperatureLevel;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.TemperatureLevel = newTemperatureLevel;
            });
            m_TemperatureTrend.RegisterCallback<ChangeEvent<float>>(evt =>
            {
                m_TemperatureTrendField.value = evt.newValue;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.TemperatureTrend = evt.newValue;
            });
            m_TemperatureTrendField.RegisterCallback<ChangeEvent<float>>(evt =>
            {
                var newTemperatureTrend = evt.newValue;
                if (newTemperatureTrend < m_TemperatureTrend.lowValue)
                {
                    newTemperatureTrend = m_TemperatureTrend.lowValue;
                    m_TemperatureTrendField.value = newTemperatureTrend;
                }
                if (newTemperatureTrend > m_TemperatureTrend.highValue)
                {
                    newTemperatureTrend = m_TemperatureTrend.highValue;
                    m_TemperatureTrendField.value = newTemperatureTrend;
                }

                m_TemperatureTrend.value = newTemperatureTrend;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.TemperatureTrend = newTemperatureTrend;
            });
            m_ControlAutoMode.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                var ap = Holder.Instance;
                if (ap == null)
                    return;

                var ctrl = ap.DevicePerformanceControl;
                ctrl.AutomaticPerformanceControl = evt.newValue;

                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                subsystem.AcceptsPerformanceLevel = true;
            });
            m_CpuLevel.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                // sync value field
                m_CpuLevelField.value = evt.newValue;
             
                var ap = Holder.Instance;
                if (ap == null)
                    return;
                ap.DevicePerformanceControl.CpuLevel = evt.newValue;
            });
            m_CpuLevelField.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                var newCPULevel = evt.newValue;
                if (newCPULevel < m_CpuLevel.lowValue)
                {
                    newCPULevel = m_CpuLevel.lowValue;
                    m_CpuLevelField.SetValueWithoutNotify(newCPULevel);
                }
                if (newCPULevel > m_CpuLevel.highValue)
                {
                    newCPULevel = m_CpuLevel.highValue;
                    m_CpuLevelField.SetValueWithoutNotify(newCPULevel);
                }

                m_CpuLevel.value = newCPULevel;

                var ap = Holder.Instance;
                if (ap == null)
                    return;
                ap.DevicePerformanceControl.CpuLevel = newCPULevel;
            });
            m_GpuLevel.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                // sync value field
                m_GpuLevelField.value = evt.newValue;

                var ap = Holder.Instance;
                if (ap == null)
                    return;
                ap.DevicePerformanceControl.GpuLevel = evt.newValue;
            });
            m_GpuLevelField.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                var newGPULevel = evt.newValue;
                if (newGPULevel < m_GpuLevel.lowValue)
                {
                    newGPULevel = m_GpuLevel.lowValue;
                    m_GpuLevelField.SetValueWithoutNotify(newGPULevel);
                }
                if (newGPULevel > m_GpuLevel.highValue)
                {
                    newGPULevel = m_GpuLevel.highValue;
                    m_GpuLevelField.SetValueWithoutNotify(newGPULevel);
                }

                m_GpuLevel.value = newGPULevel;
           
                var ap = Holder.Instance;
                if (ap == null)
                    return;
                ap.DevicePerformanceControl.GpuLevel = newGPULevel;

            });
            m_Bottleneck.RegisterCallback<ChangeEvent<Enum>>(evt =>
            {
                SimulatorAdaptivePerformanceSubsystem subsystem = Subsystem();
                if (subsystem == null)
                    return;

                SetBottleneck((PerformanceBottleneck)evt.newValue, subsystem);
            });
            m_DevLogging.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                var ap = Holder.Instance;
                if (ap == null)
                    return;
                var devSettings = ap.DevelopmentSettings;

                devSettings.Logging = evt.newValue;
            });
            m_DevLoggingFrequency.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                var ap = Holder.Instance;
                if (ap == null)
                    return;
                var devSettings = ap.DevelopmentSettings;

                devSettings.LoggingFrequencyInFrames = evt.newValue;
            });

            EditorApplication.playModeStateChanged += LogPlayModeState;

            SyncAPSubsystemSettingsToEditor();
        }

        Foldout m_ExtensionFoldout;
        Foldout m_ThermalFoldout;
        EnumField m_WarningLevel;
        Slider m_TemperatureLevel;
        FloatField m_TemperatureLevelField;
        Slider m_TemperatureTrend;
        FloatField m_TemperatureTrendField;
        Foldout m_PerformanceFoldout;
        Toggle m_ControlAutoMode;
        SliderInt m_CpuLevel;
        IntegerField m_CpuLevelField;
        SliderInt m_GpuLevel;
        IntegerField m_GpuLevelField;
        EnumField m_Bottleneck;
        Foldout m_DeveloperFoldout;
        Toggle m_DevLogging;
        IntegerField m_DevLoggingFrequency;

        [SerializeField, HideInInspector]
        AdaptivePerformanceStates m_SerializationStates;

        [System.Serializable]
        internal struct AdaptivePerformanceStates
        {
            public bool thermalFoldout;
            public bool performanceFoldout;
            public bool developerFoldout;
        };

        public void OnBeforeSerialize()
        {
            m_SerializationStates.thermalFoldout = m_ThermalFoldout.value;
            m_SerializationStates.performanceFoldout = m_PerformanceFoldout.value;
            m_SerializationStates.developerFoldout = m_DeveloperFoldout.value;
        }

        public void OnAfterDeserialize() {}

        void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
                SyncAPSubsystemSettingsToEditor();
        }

        void SyncAPSubsystemSettingsToEditor()
        {
            var ap = Holder.Instance;
            if (ap == null)
                return;

            var ctrl = ap.DevicePerformanceControl;
            var devSettings = ap.DevelopmentSettings;
            var perfMetrics = ap.PerformanceStatus.PerformanceMetrics;
            var thermalMetrics = ap.ThermalStatus.ThermalMetrics;

            m_WarningLevel.value = thermalMetrics.WarningLevel;
            m_TemperatureLevel.value = thermalMetrics.TemperatureLevel;
            m_TemperatureLevelField.value = thermalMetrics.TemperatureLevel;
            m_TemperatureTrend.value = thermalMetrics.TemperatureTrend;
            m_TemperatureTrendField.value = thermalMetrics.TemperatureTrend;
            m_ControlAutoMode.value = ctrl.AutomaticPerformanceControl;
            m_CpuLevel.value = ctrl.CpuLevel;
            m_CpuLevel.highValue = ctrl.MaxCpuPerformanceLevel;
            m_CpuLevel.lowValue = 0;
            m_CpuLevelField.value = ctrl.CpuLevel;
            m_GpuLevel.value = ctrl.GpuLevel;
            m_GpuLevel.highValue = ctrl.MaxGpuPerformanceLevel;
            m_GpuLevel.lowValue = 0;
            m_GpuLevelField.value = ctrl.GpuLevel;
            m_Bottleneck.value = perfMetrics.PerformanceBottleneck;
            m_DevLogging.value = devSettings.Logging;
            m_DevLoggingFrequency.value = devSettings.LoggingFrequencyInFrames;

            // Set bottleneck so we get CPU/GPU frametimes and a valid bottleneck
            SetBottleneck((PerformanceBottleneck)m_Bottleneck.value, Subsystem());
        }

        void SetBottleneck(PerformanceBottleneck performanceBottleneck, SimulatorAdaptivePerformanceSubsystem subsystem)
        {
            if (subsystem == null)
                return;

            var targetFrameRate = Application.targetFrameRate;

            // default target framerate is -1 to use default platform framerate so we assume it's 60
            if(targetFrameRate == -1)
                targetFrameRate = 60;

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
                //PerformanceBottleneck.Unknowe - averageOverallFrametime > targetFramerate
                default:
                    subsystem.NextCpuFrameTime = currentTargetFramerateHalfMS;
                    subsystem.NextGpuFrameTime = currentTargetFramerateHalfMS;
                    subsystem.NextOverallFrameTime = currentTargetFramerateMS + 0.001f;
                    break;
            }
        }

        SimulatorAdaptivePerformanceSubsystem Subsystem()
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
