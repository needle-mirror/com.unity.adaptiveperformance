namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the quality of shadows.
    /// </summary>
    public class AdaptiveShadowQuality : AdaptivePerformanceScaler
    {
        int m_DefaultShadowQualityBias;

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
        /// Callback when scaler gets disabled and removed from indexer
        /// </summary>
        protected override void OnDisabled()
        {
            AdaptivePerformanceRenderSettings.ShadowQualityBias = m_DefaultShadowQualityBias;
        }

        /// <summary>
        /// Callback when scaler gets enabled and added to the indexer
        /// </summary>
        protected override void OnEnabled()
        {
            m_DefaultShadowQualityBias = MaxLevel;
        }

        /// <summary>
        /// Callback for any level change.
        /// </summary>
        protected override void OnLevel()
        {
            float oldScaleFactor = Scale;
            float scaleIncrement = (MaxBound - MinBound) / MaxLevel;

            Scale = scaleIncrement * (MaxLevel - CurrentLevel) + MinBound;

            if (Scale != oldScaleFactor)
                AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias = m_DefaultShadowQualityBias - (int)(((float)m_DefaultShadowQualityBias) * Scale);
        }
    }
}