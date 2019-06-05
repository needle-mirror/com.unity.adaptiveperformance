namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the level of antialiasing.
    /// </summary>
    public class MSAAScaler : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Returns visual impact of scaler when applied.
        /// </summary>
        public override ScalerVisualImpact VisualImpact => ScalerVisualImpact.Medium;
        /// <summary>
        /// Returns what bottlenecks this scaler targets.
        /// </summary>
        public override ScalerTarget Target => ScalerTarget.GPU | ScalerTarget.FillRate;
        /// <summary>
        /// Returns max level of scaler.
        /// </summary>
        public override int MaxLevel => 2;

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
