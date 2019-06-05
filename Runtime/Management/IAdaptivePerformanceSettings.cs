using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// Settings of indexer system.
    /// </summary>
    [System.Serializable]
    public class AdaptivePerformanceIndexerSettings
    {
        [SerializeField, Tooltip("Active")]
        bool m_Active = true;

        /// <summary>
        /// Returns `true` if Indexer was active, `false` otherwise.
        /// </summary>
        public bool active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        [SerializeField, Tooltip("Thermal Action Delay")]
        float m_ThermalActionDelay = 10;

        /// <summary>
        /// Delay after any scaler is applied or unapplied, because of thermal state.
        /// </summary>
        public float thermalActionDelay
        {
            get { return m_ThermalActionDelay; }
            set { m_ThermalActionDelay = value; }
        }

        [SerializeField, Tooltip("Performance Action Delay")]
        float m_PerformanceActionDelay = 4;

        /// <summary>
        /// Delay after any scaler is applied or unapplied, because of performance state.
        /// </summary>
        public float performanceActionDelay
        {
            get { return m_PerformanceActionDelay; }
            set { m_PerformanceActionDelay = value; }
        }

        [SerializeField, Tooltip("Thermal State Mode")]
        ThermalStateMode m_ThermalStateMode = ThermalStateMode.TemperatureLevelBased;

        /// <summary>
        /// Thermal state mode used by indexer.
        /// </summary>
        public ThermalStateMode thermalStateMode
        {
            get { return m_ThermalStateMode; }
            set { m_ThermalStateMode = value; }
        }

        [SerializeField, Tooltip("Thermal Safe Range")]
        Vector2 m_ThermalSafeRange = new Vector2(0.6f, 0.7f);

        /// <summary>
        /// Thermal level range that indexer will target.
        /// </summary>
        public Vector2 thermalSafeRange
        {
            get { return m_ThermalSafeRange; }
            set { m_ThermalSafeRange = value; }
        }
    }

    /// <summary>
    /// Provider Settings Interface as base class of provider for control of the editor runtime asset instance which stores the Settings.
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
        /// <value>`true` to enable debug logging, `false` to disable it (default: `false`)</value>
        public bool logging
        {
            get { return m_Logging; }
            set { m_Logging = value; }
        }

        [SerializeField, Tooltip("Automatic Performance Mode")]
        bool m_AutomaticPerformanceModeEnabled = true;

        /// <summary>
        /// The Initial value of <see cref="IDevicePerformanceControl.AutomaticPerformanceControl"/>.
        /// </summary>
        /// <value>`true` to enable Automatic Performance Mode, `false` to disable it (default: `true`)</value>
        public bool automaticPerformanceMode
        {
            get { return m_AutomaticPerformanceModeEnabled; }
            set { m_AutomaticPerformanceModeEnabled = value; }
        }

        [SerializeField, Tooltip("Logging Frequency (Development mode only)")]
        int m_StatsLoggingFrequencyInFrames = 50;

        /// <summary>
        /// Adjust the frequency in frames at which the application logs frame statistics to the console.
        /// This is only relevant when logging is enabled. See <see cref="Logging"/>.
        /// This setting can also be controlled after startup using <see cref="IDevelopmentSettings.StatsLoggingFrequencyInFrames"/>.
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
    }
}
