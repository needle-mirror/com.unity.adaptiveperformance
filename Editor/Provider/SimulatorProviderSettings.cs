using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance.Simulator.Editor
{
    /// <summary>
    /// Provider Settings for Simulator Provider which controls the editor runtime asset instance which stores the Settings.
    /// </summary>
    [System.Serializable]
    [AdaptivePerformanceConfigurationData("Simulator", SimulatorProviderConstants.k_SettingsKey)]
    public class SimulatorProviderSettings : IAdaptivePerformanceSettings
    {
        /// <summary>
        /// Returns Samsung Provider Settings which are used by Adaptive Performance to apply Provider Settings.
        /// </summary>
        /// <returns>Samsung Provider Settings</returns>
        public static SimulatorProviderSettings GetSettings()
        {
            SimulatorProviderSettings settings = null;
            EditorBuildSettings.TryGetConfigObject<SimulatorProviderSettings>(SimulatorProviderConstants.k_SettingsKey, out settings);
            return settings;
        }
    }
}
