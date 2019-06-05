namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> for adjusting at what distance LODs are switched.
    /// </summary>
    public class AdaptiveLOD : AdaptivePerformanceScaler
    {
        private float m_LodBias;

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

        private void Start()
        {
            m_LodBias = QualitySettings.lodBias;
        }

        /// <summary>
        /// Callback for any level change.
        /// </summary>
        protected override void OnLevel()
        {
            switch (CurrentLevel)
            {
                case 0:
                    QualitySettings.lodBias = m_LodBias;
                    break;
                case 1:
                    QualitySettings.lodBias = m_LodBias * 0.8f;
                    break;
                case 2:
                    QualitySettings.lodBias = m_LodBias * 0.6f;
                    break;
                case 3:
                    QualitySettings.lodBias = m_LodBias * 0.4f;
                    break;
            }
        }
    }
}
