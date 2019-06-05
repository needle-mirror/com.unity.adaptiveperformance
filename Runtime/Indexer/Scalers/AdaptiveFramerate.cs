namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the application update rate using <see cref="Application.TargetFramerate"/>.
    /// </summary>
    public class AdaptiveFramerate : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Returns visual impact of scaler when applied.
        /// </summary>
        public override ScalerVisualImpact VisualImpact => ScalerVisualImpact.High;

        /// <summary>
        /// Returns what bottlenecks this scaler targets.
        /// </summary>
        public override ScalerTarget Target => ScalerTarget.CPU | ScalerTarget.GPU | ScalerTarget.FillRate;

        /// <summary>
        /// Returns max level of scaler.
        /// </summary>
        public override int MaxLevel => maximumFPS - minimumFPS;

        /// <summary>
        /// Minimum framerate that the scaler is allowed to scale to.
        /// </summary>
        [SerializeField, Tooltip("Minimum allowed FPS to scale to.")]
        protected int minimumFPS = 15;

        /// <summary>
        /// Maximum framerate that the scaler is allowed to scale to.
        /// </summary>
        [SerializeField, Tooltip("Maximum allowed FPS to scale to.")]
        protected int maximumFPS = 60;

        /// <summary>
        /// Callback for when the quality level is decreased/scaler level increased.
        /// </summary>
        protected virtual void Awake()
        {
            Application.targetFrameRate = maximumFPS;
        }

        /// <summary>
        /// Callback for when the quality level is decreased/scaler level increased.
        /// </summary>
        protected override void OnLevelIncrease()
        {
            base.OnLevelIncrease();

            var framerateDecrease = 1;

            if (Holder.Instance.Indexer.PerformanceAction == StateAction.FastDecrease)
                framerateDecrease = 5;

            var fps = Application.targetFrameRate - framerateDecrease;

            if (fps >= minimumFPS && fps <= maximumFPS)
                Application.targetFrameRate = fps;
        }

        /// <summary>
        /// Callback for when the quality level is increased/scaler level decreased.
        /// </summary>
        protected override void OnLevelDecrease()
        {
            base.OnLevelDecrease();

            var fps = Application.targetFrameRate + 5;
            if (fps >= minimumFPS && fps <= maximumFPS)
                Application.targetFrameRate = fps;
        }
    }
}
