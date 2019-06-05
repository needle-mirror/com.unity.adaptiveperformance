namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the number of shadow cascades to be used.
    /// </summary>
    public class AdaptiveShadowCascade : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Ensures settings are applied during startup.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (m_Settings == null)
                return;
            ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveShadowCascades);
        }

        /// <summary>
        /// Callback for any level change.
        /// </summary>
        protected override void OnLevel()
        {
            switch (CurrentLevel)
            {
                case 0:
                    AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias = 0;
                    break;
                case 1:
                    AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias = 1;
                    break;
                case 2:
                    AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias = 2;
                    break;
            }
        }
    }
}
