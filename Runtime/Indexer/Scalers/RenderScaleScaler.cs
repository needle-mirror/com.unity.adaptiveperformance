namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust resolution via RenderScale in URP.
    /// </summary>
    public class RenderScaleScaler : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Returns visual impact of scaler when applied.
        /// </summary>
        public override ScalerVisualImpact VisualImpact => ScalerVisualImpact.High;
        /// <summary>
        /// Returns what bottlenecks this scaler targets.
        /// </summary>
        public override ScalerTarget Target => ScalerTarget.FillRate | ScalerTarget.GPU;
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
                    AdaptivePerformanceRenderSettings.RenderScaleMultiplier = 1;
                    break;
                case 1:
                    AdaptivePerformanceRenderSettings.RenderScaleMultiplier = 0.8f;
                    break;
                case 2:
                    AdaptivePerformanceRenderSettings.RenderScaleMultiplier = 0.7f;
                    break;
                case 3:
                    AdaptivePerformanceRenderSettings.RenderScaleMultiplier = 0.5f;
                    break;
            }
        }
    }
}
