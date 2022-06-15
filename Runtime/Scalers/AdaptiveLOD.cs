using UnityEngine.Scripting;

namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> for adjusting at what distance LODs are switched.
    /// </summary>
#if !UNITY_2021_2_OR_NEWER
    [Preserve]
#endif
    public class AdaptiveLOD : AdaptivePerformanceScaler
    {
        float m_DefaultLodBias;

        /// <summary>
        /// Ensures settings are applied during startup.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (m_Settings == null)
                return;
            ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveLOD);
        }

        /// <summary>
        /// Callback when scaler gets disabled and removed from indexer
        /// </summary>
        protected override void OnDisabled()
        {
            QualitySettings.lodBias = m_DefaultLodBias;
        }

        /// <summary>
        /// Callback when scaler gets enabled and added to the indexer
        /// </summary>
        protected override void OnEnabled()
        {
            m_DefaultLodBias = QualitySettings.lodBias;
        }

        /// <summary>
        /// Callback for any level change.
        /// </summary>
        protected override void OnLevel()
        {
            if (ScaleChanged())
                QualitySettings.lodBias = m_DefaultLodBias * Scale;
        }
    }
}
