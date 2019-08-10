namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to change the distance at which shadows are rendered.
    /// </summary>
    public class AdaptiveShadowDistance : AdaptivePerformanceScaler
    {
        float m_DefaultShadowDistance;
        /// <summary>
        /// Ensures settings are applied during startup.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (m_Settings == null)
                return;
            ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveShadowDistance);
        }

        /// <summary>
        /// Callback when scaler gets disabled and removed from indexer
        /// </summary>
        protected override void OnDisabled()
        {
            AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier = m_DefaultShadowDistance;
        }

        /// <summary>
        /// Callback when scaler gets enabled and added to the indexer
        /// </summary>
        protected override void OnEnabled()
        {
            m_DefaultShadowDistance = AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier;
        }

        /// <summary>
        /// Callback for any level change.
        /// </summary>
        protected override void OnLevel()
        {
            switch (CurrentLevel)
            {
                case 0:
                    AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier = 1;
                    break;
                case 1:
                    AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier = 0.75f;
                    break;
                case 2:
                    AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier = 0.5f;
                    break;
                case 3:
                    AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier = 0.15f;
                    break;
            }
        }
    }
}
