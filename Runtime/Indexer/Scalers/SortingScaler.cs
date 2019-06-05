namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to change if objects in the scene are sorted by depth before rendering to reduce overdraw.
    /// </summary>
    public class SortingScaler : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Returns visual impact of scaler when applied.
        /// </summary>
        public override ScalerVisualImpact VisualImpact => ScalerVisualImpact.Medium;
        /// <summary>
        /// Returns what bottlenecks this scaler targets.
        /// </summary>
        public override ScalerTarget Target => ScalerTarget.CPU;
        /// <summary>
        /// Returns max level of scaler.
        /// </summary>
        public override int MaxLevel => 1;

        /// <summary>
        /// Callback for any level change.
        /// </summary>
        protected override void OnLevel()
        {
            switch (CurrentLevel)
            {
                case 0:
                    AdaptivePerformanceRenderSettings.SkipFrontToBackSorting = false;
                    break;
                case 1:
                    AdaptivePerformanceRenderSettings.SkipFrontToBackSorting = true;
                    break;
            }
        }
    }
}
