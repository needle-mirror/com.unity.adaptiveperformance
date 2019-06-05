namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the quality of shadows.
    /// </summary>
    public class ShadowQualityScaler : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Returns visual impact of scaler when applied.
        /// </summary>
        public override ScalerVisualImpact VisualImpact => ScalerVisualImpact.High;
        /// <summary>
        /// Returns what bottlenecks this scaler targets.
        /// </summary>
        public override ScalerTarget Target => ScalerTarget.GPU | ScalerTarget.CPU;
        /// <summary>
        /// Returns max level of scaler.
        /// </summary>
        public override int MaxLevel => 3;

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
