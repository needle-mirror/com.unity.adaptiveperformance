using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.AdaptivePerformance
{
    // Changes to tooltips in this file should be reflected in ProviderSettingsEditor as well.

    /// <summary>
    /// Settings of indexer system.
    /// </summary>
    [System.Serializable]
    public class AdaptivePerformanceIndexerSettings
    {
        const string m_FeatureName = "Indexer";

        [SerializeField, Tooltip("Active")]
        bool m_Active = true;

        /// <summary>
        /// Returns true if Indexer was active, false otherwise.
        /// </summary>
        public bool active
        {
            get { return m_Active; }
            set
            {
                if (m_Active == value)
                    return;

                m_Active = value;
                AdaptivePerformanceAnalytics.SendAdaptiveFeatureUpdateEvent(m_FeatureName, m_Active);
            }
        }

        [SerializeField, Tooltip("Thermal Action Delay")]
        float m_ThermalActionDelay = 10;

        /// <summary>
        /// Delay after any scaler is applied or unapplied because of thermal state.
        /// </summary>
        public float thermalActionDelay
        {
            get { return m_ThermalActionDelay; }
            set { m_ThermalActionDelay = value; }
        }

        [SerializeField, Tooltip("Performance Action Delay")]
        float m_PerformanceActionDelay = 4;

        /// <summary>
        /// Delay after any scaler is applied or unapplied because of performance state.
        /// </summary>
        public float performanceActionDelay
        {
            get { return m_PerformanceActionDelay; }
            set { m_PerformanceActionDelay = value; }
        }
    }

    /// <summary>
    /// Settings of indexer system.
    /// </summary>
    [System.Serializable]
    public class AdaptivePerformanceScalerSettings
    {
        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to adjust the application update rate using Application.TargetFramerate")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveFramerate = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive Framerate",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.High,
            target =  ScalerTarget.CPU | ScalerTarget.GPU | ScalerTarget.FillRate,
            minBound = 15,
            maxBound = 60,
            maxLevel = 60 - 15
        };

        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to adjust the application update rate using <see cref="Application.targetFrameRate"/>.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveFramerate
        {
            get { return m_AdaptiveFramerate; }
            set { m_AdaptiveFramerate = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to adjust the resolution of all render targets that allow dynamic resolution.")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveResolution = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive Resolution",
            enabled = false,
            scale = -1.0f,
            visualImpact = ScalerVisualImpact.Low,
            target =  ScalerTarget.FillRate | ScalerTarget.GPU,
            maxLevel = 9,
            minBound = 0.5f,
            maxBound = 1,
        };

        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to adjust the resolution of all render targets that allow dynamic resolution.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveResolution
        {
            get { return m_AdaptiveResolution; }
            set { m_AdaptiveResolution = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to control if dynamic batching is enabled.")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveBatching = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive Batching",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.Medium,
            target =  ScalerTarget.CPU,
            maxLevel = 1,
            minBound = -1,
            maxBound = -1,
        };
        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to control if dynamic batching is enabled.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveBatching
        {
            get { return m_AdaptiveBatching; }
            set { m_AdaptiveBatching = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer for adjusting at what distance LODs are switched.")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveLOD = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive LOD",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.High,
            target =  ScalerTarget.GPU,
            maxLevel = 3,
            minBound = -1,
            maxBound = -1,
        };


        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> for adjusting at what distance LODs are switched.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveLOD
        {
            get { return m_AdaptiveLOD; }
            set { m_AdaptiveLOD = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to adjust the size of the palette used for color grading in URP.")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveLut = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive Lut",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.Medium,
            target =  ScalerTarget.GPU | ScalerTarget.CPU,
            maxLevel = 1,
            minBound = -1,
            maxBound = -1,
        };


        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to adjust the size of the palette used for color grading in URP.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveLut
        {
            get { return m_AdaptiveLut; }
            set { m_AdaptiveLut = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to adjust the level of antialiasing.")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveMSAA = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive MSAA",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.Medium,
            target =  ScalerTarget.GPU | ScalerTarget.FillRate,
            maxLevel = 2,
            minBound = -1,
            maxBound = -1,
        };


        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to adjust the level of antialiasing.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveMSAA
        {
            get { return m_AdaptiveMSAA; }
            set { m_AdaptiveMSAA = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to adjust the number of shadow cascades to be used.")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowCascades = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive Shadow Cascade",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.Medium,
            target =  ScalerTarget.GPU | ScalerTarget.CPU,
            maxLevel = 2,
            minBound = -1,
            maxBound = -1,
        };

        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to adjust the number of shadow cascades to be used.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveShadowCascades
        {
            get { return m_AdaptiveShadowCascades; }
            set { m_AdaptiveShadowCascades = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to change the distance at which shadows are rendered.")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowDistance = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive Shadow Distance",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.Low,
            target =  ScalerTarget.GPU,
            maxLevel = 3,
            minBound = -1,
            maxBound = -1,
        };

        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to change the distance at which shadows are rendered.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveShadowDistance
        {
            get { return m_AdaptiveShadowDistance; }
            set { m_AdaptiveShadowDistance = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to adjust the resolution of shadow maps.")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowmapResolution = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive Shadowmap Resolution",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.Low,
            target =  ScalerTarget.GPU,
            maxLevel = 3,
            minBound = -1,
            maxBound = -1,
        };

        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to adjust the resolution of shadow maps.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveShadowmapResolution
        {
            get { return m_AdaptiveShadowmapResolution; }
            set { m_AdaptiveShadowmapResolution = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to adjust the quality of shadows.")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowQuality = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive Shadow Quality",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.High,
            target =  ScalerTarget.GPU | ScalerTarget.CPU,
            maxLevel = 3,
            minBound = -1,
            maxBound = -1,
        };

        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to adjust the quality of shadows.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveShadowQuality
        {
            get { return m_AdaptiveShadowQuality; }
            set { m_AdaptiveShadowQuality = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to change if objects in the scene are sorted by depth before rendering to reduce overdraw.")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveSorting = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive Sorting",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.Medium,
            target =  ScalerTarget.CPU,
            maxLevel = 1,
            minBound = -1,
            maxBound = -1,
        };

        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to change if objects in the scene are sorted by depth before rendering to reduce overdraw.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveSorting
        {
            get { return m_AdaptiveSorting; }
            set { m_AdaptiveSorting = value; }
        }

        [SerializeField, Tooltip("Settings for a scaler used by the Indexer to disable transparent objects rendering")]
        AdaptivePerformanceScalerSettingsBase m_AdaptiveTransparency = new AdaptivePerformanceScalerSettingsBase
        {
            name = "Adaptive Transparency",
            enabled = false,
            scale = -1,
            visualImpact = ScalerVisualImpact.High,
            target =  ScalerTarget.GPU,
            maxLevel = 1,
            minBound = -1,
            maxBound = -1,
        };

        /// <summary>
        /// A scaler setting used by <see cref="AdaptivePerformanceIndexer"/> to disable transparent objects rendering.
        /// </summary>
        public AdaptivePerformanceScalerSettingsBase AdaptiveTransparency
        {
            get { return m_AdaptiveTransparency; }
            set { m_AdaptiveTransparency = value; }
        }
    }
    /// <summary>
    /// Settings of indexer system.
    /// </summary>
    [System.Serializable]
    public class AdaptivePerformanceScalerSettingsBase
    {
        [SerializeField, Tooltip("Name of the scaler.")]
        string m_Name = "Base Scaler";

        /// <summary>
        /// Returns the name of the scaler.
        /// </summary>
        public string name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
            }
        }

        [SerializeField, Tooltip("Active")]
        bool m_Enabled = false;

        /// <summary>
        /// Returns true if Indexer was active, false otherwise.
        /// </summary>
        public bool enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        [SerializeField, Tooltip("Scale to control the quality impact for the scaler. No quality change when 1, improved quality when >1, and lowered quality when <1.")]
        float m_Scale = -1.0f;

        /// <summary>
        /// Scale to control the quality impact for the scaler. No quality change when 1, improved quality when bigger 1, and lowered quality when smaller 1.
        /// </summary>
        public float scale
        {
            get { return m_Scale; }
            set { m_Scale = value; }
        }

        [SerializeField, Tooltip("Visual impact the scaler has on the application. The higher the value, the more impact the scaler has on the visuals.")]
        ScalerVisualImpact m_VisualImpact = ScalerVisualImpact.Low;

        /// <summary>
        /// Visual impact the scaler has on the application. The higher the value, the more impact the scaler has on the visuals.
        /// </summary>
        public ScalerVisualImpact visualImpact
        {
            get { return m_VisualImpact; }
            set { m_VisualImpact = value; }
        }

        [SerializeField, Tooltip("Application bottleneck that the scaler targets. The target selected has the most impact on the quality control of this scaler.")]
        ScalerTarget m_Target = ScalerTarget.CPU;

        /// <summary>
        /// Application bottleneck that the scaler targets. The target selected has the most impact on the quality control of this scaler.
        /// </summary>
        public ScalerTarget target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }

        [SerializeField, Tooltip("Maximum level for the scaler. This is tied to the implementation of the scaler to divide the levels into concrete steps.")]
        int m_MaxLevel = 1;

        /// <summary>
        /// Maximum level for the scaler. This is tied to the implementation of the scaler to divide the levels into concrete steps.
        /// </summary>
        public int maxLevel
        {
            get { return m_MaxLevel; }
            set { m_MaxLevel = value; }
        }

        [SerializeField, Tooltip("Minimum value for the scale boundary.")]
        float m_MinBound = -1.0f;

        /// <summary>
        /// Minimum value for the scale boundary.
        /// </summary>
        public float minBound
        {
            get { return m_MinBound; }
            set { m_MinBound = value; }
        }

        [SerializeField, Tooltip("Maximum value for the scale boundary.")]
        float m_MaxBound = -1.0f;

        /// <summary>
        /// Maximum value for the scale boundary.
        /// </summary>
        public float maxBound
        {
            get { return m_MaxBound; }
            set { m_MaxBound = value; }
        }
    }

    /// <summary>
    /// Provider Settings Interface as base class of the provider. Used to control the Editor runtime asset instance which stores the Settings.
    /// </summary>
    public class IAdaptivePerformanceSettings : ScriptableObject
    {
        [SerializeField, Tooltip("Enable Logging in Devmode")]
        bool m_Logging = true;

        /// <summary>
        ///  Control debug logging.
        ///  This setting only affects development builds. All logging is disabled in release builds.
        ///  This setting can also be controlled after startup using <see cref="IDevelopmentSettings.Logging"/>.
        ///  Logging is disabled by default.
        /// </summary>
        /// <value>Set this to true to enable debug logging, or false to disable it. It is false by default.</value>
        public bool logging
        {
            get { return m_Logging; }
            set { m_Logging = value; }
        }

        [SerializeField, Tooltip("Automatic Performance Mode")]
        bool m_AutomaticPerformanceModeEnabled = true;

        /// <summary>
        /// The initial value of <see cref="IDevicePerformanceControl.AutomaticPerformanceControl"/>.
        /// </summary>
        /// <value>Set this to true to enable Automatic Performance Mode, or false to disable it. It is true by default.</value>
        public bool automaticPerformanceMode
        {
            get { return m_AutomaticPerformanceModeEnabled; }
            set { m_AutomaticPerformanceModeEnabled = value; }
        }

        [SerializeField, Tooltip("Logging Frequency (Development mode only)")]
        int m_StatsLoggingFrequencyInFrames = 50;

        /// <summary>
        /// Adjust the frequency in frames at which the application logs frame statistics to the console.
        /// This is only relevant when logging is enabled. See <see cref="IDevelopmentSettings.Logging"/>.
        /// This setting can also be controlled after startup using <see cref="IDevelopmentSettings.LoggingFrequencyInFrames"/>.
        /// </summary>
        /// <value>Logging frequency in frames (default: 50)</value>
        public int statsLoggingFrequencyInFrames
        {
            get { return m_StatsLoggingFrequencyInFrames; }
            set { m_StatsLoggingFrequencyInFrames = value; }
        }

        [SerializeField, Tooltip("Indexer Settings")]
        AdaptivePerformanceIndexerSettings m_IndexerSettings;

        /// <summary>
        /// Settings of indexer system.
        /// </summary>
        public AdaptivePerformanceIndexerSettings indexerSettings
        {
            get { return m_IndexerSettings; }
            set { m_IndexerSettings = value; }
        }


        [SerializeField, Tooltip("Scaler Settings")]
        AdaptivePerformanceScalerSettings m_ScalerSettings;

        /// <summary>
        /// Settings of scaler system.
        /// </summary>
        public AdaptivePerformanceScalerSettings scalerSettings
        {
            get { return m_ScalerSettings; }
            set { m_ScalerSettings = value; }
        }
    }
}
