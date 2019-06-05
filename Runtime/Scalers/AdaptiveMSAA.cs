namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the level of antialiasing.
    /// </summary>
    public class AdaptiveMSAA : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Ensures settings are applied during startup.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (m_Settings == null)
                return;
            ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveMSAA);
        }

        /// <summary>
        /// Callback for any level change.
        /// </summary>
        protected override void OnLevel()
        {
            switch (CurrentLevel)
            {
                case 0:
                    AdaptivePerformanceRenderSettings.AntiAliasingQualityBias = 0;
                    break;
                case 1:
                    AdaptivePerformanceRenderSettings.AntiAliasingQualityBias = 1;
                    break;
                case 2:
                    AdaptivePerformanceRenderSettings.AntiAliasingQualityBias = 2;
                    break;
            }
        }
    }
}
