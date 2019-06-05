using UnityEngine;
using UnityEngine.AdaptivePerformance;

static class AdaptivePerformanceConfig
{
    /// <summary>
    /// In case you want to manually override settings during startup, this can be done with the IAdaptivePerformanceSettings.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Setup()
    {
        if (!AdaptivePerformanceGeneralSettings.Instance || !AdaptivePerformanceGeneralSettings.Instance.Manager || !AdaptivePerformanceGeneralSettings.Instance.Manager.isInitializationComplete)
            return;

        IAdaptivePerformanceSettings settings = AdaptivePerformanceGeneralSettings.Instance.Manager.activeLoader.GetSettings();
        if (settings == null)
            return;

        settings.automaticPerformanceMode = false;
        settings.logging = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
    }
}


public class AdaptivePerformanceSettings : MonoBehaviour {}
