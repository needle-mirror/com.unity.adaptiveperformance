namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> for adjusting at what distance LODs are switched.
    /// </summary>
    public class LODScaler : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Returns visual impact of scaler when applied.
        /// </summary>
        public override ScalerVisualImpact VisualImpact => ScalerVisualImpact.High;
        /// <summary>
        /// Returns what bottlenecks this scaler targets.
        /// </summary>
        public override ScalerTarget Target => ScalerTarget.GPU;
        /// <summary>
        /// Returns max level of scaler.
        /// </summary>
        public override int MaxLevel => 3;
        private float m_LodBias;

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
