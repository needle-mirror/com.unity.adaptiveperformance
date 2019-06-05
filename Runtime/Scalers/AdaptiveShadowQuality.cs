namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the quality of shadows.
    /// </summary>
    public class AdaptiveShadowQuality : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Ensures settings are applied during startup.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (m_Settings == null)
                return;
            ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveShadowQuality);
        }

        /// <summary>
        /// Callback for any level change.
        /// </summary>
        protected override void OnLevel()
        {
            switch (CurrentLevel)
            {
                case 0:
                    AdaptivePerformanceRenderSettings.ShadowQualityBias = 0;
                    break;
                case 1:
                    AdaptivePerformanceRenderSettings.ShadowQualityBias = 1;
                    break;
                case 2:
                    AdaptivePerformanceRenderSettings.ShadowQualityBias = 2;
                    break;
                case 3:
                    AdaptivePerformanceRenderSettings.ShadowQualityBias = 3;
                    break;
            }
        }
    }
}
