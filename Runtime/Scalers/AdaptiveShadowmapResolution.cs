namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the resolution of shadow maps.
    /// </summary>
    public class AdaptiveShadowmapResolution : AdaptivePerformanceScaler
    {
        float m_DefaultShadowmapResolution;

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
        /// Callback when scaler gets disabled and removed from indexer
        /// </summary>
        protected override void OnDisabled()
        {
            AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier = m_DefaultShadowmapResolution;
        }

        /// <summary>
        /// Callback when scaler gets enabled and added to the indexer
        /// </summary>
        protected override void OnEnabled()
        {
            m_DefaultShadowmapResolution = 1;
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
                AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier = m_DefaultShadowmapResolution * Scale;
        }
    }
}
