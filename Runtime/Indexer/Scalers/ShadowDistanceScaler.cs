namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to change the distance at which shadows are rendered.
    /// </summary>
    public class ShadowDistanceScaler : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Returns visual impact of scaler when applied.
        /// </summary>
        public override ScalerVisualImpact VisualImpact => ScalerVisualImpact.Low;
        /// <summary>
        /// Returns what bottlenecks this scaler targets.
        /// </summary>
        public override ScalerTarget Target => ScalerTarget.GPU;
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