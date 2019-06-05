namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the resolution of shadow maps.
    /// </summary>
    public class AdaptiveShadowmapResolution : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Ensures settings are applied during startup.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (m_Settings == null)
                return;
            ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveShadowmapResolution);
        }

        /// <summary>
        /// Callback for any level change.
        /// </summary>
        protected override void OnLevel()
        {
            switch (CurrentLevel)
            {
                case 0:
                    AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier = 1;
                    break;
                case 1:
                    AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier = 0.75f;
                    break;
                case 2:
                    AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier = 0.5f;
                    break;
                case 3:
                    AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier = 0.15f;
                    break;
            }
        }
    }
}
