namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the number of shadow cascades to be used.
    /// </summary>
    public class AdaptiveShadowCascade : AdaptivePerformanceScaler
    {
        int m_DefaultCascadeCount;

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
        /// Callback when scaler gets disabled and removed from indexer
        /// </summary>
        protected override void OnDisabled()
        {
            AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias = m_DefaultCascadeCount;
        }

        /// <summary>
        /// Callback when scaler gets enabled and added to the indexer
        /// </summary>
        protected override void OnEnabled()
        {
            m_DefaultCascadeCount = AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias;
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
