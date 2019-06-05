namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// Scaler impact on visuals.
    /// </summary>
    public enum ScalerVisualImpact
    {
        /// <summary>
        /// Low impact on visual quality. Changes may not be very noticeable to the user.
        /// </summary>
        Low,
        /// <summary>
        /// Medium impact on visual quality. Mildly affects the visuals in the scene and may be noticeable to the user.
        /// </summary>
        Medium,
        /// <summary>
        /// High impact on visual quality. The scaler will have an easily visible effect on quality.
        /// </summary>
        High
    }

    /// <summary>
    /// Scaler targeted bottleneck flags.
    /// </summary>
    [System.Flags]
    public enum ScalerTarget
    {
        /// <summary>
        /// The scaler targets the CPU and attempts to reduce the CPU load.
        /// </summary>
        CPU = 0x1,
        /// <summary>
        /// The scaler targets the GPU and attempts to reduce the GPU load.
        /// </summary>
        GPU = 0x2,
        /// <summary>
        /// The scaler targets fillrate. Often at the expense of visual quality.
        /// </summary>
        FillRate = 0x4
    }

    /// <summary>
    /// Abstract class representing single feature that is controlled by <see cref="AdaptivePerformanceIndexer"/>.
    /// You control the quality through changing the levels, where 0 represents the controller not being applied and 1,2... as applied.
    /// As a result, a higher level represents lower visuals, but better performance.
    /// </summary>
    public abstract class AdaptivePerformanceScaler : MonoBehaviour
    {
        private AdaptivePerformanceIndexer m_Indexer;
        /// <summary>
        /// Returns visual impact of scaler when applied.
        /// </summary>
        public abstract ScalerVisualImpact VisualImpact { get; }
        /// <summary>
        /// Returns what bottlenecks this scaler targets.
        /// </summary>
        public abstract ScalerTarget Target { get; }
        /// <summary>
        /// Returns current level of scale.
        /// </summary>
        public int CurrentLevel { get; private set; }
        /// <summary>
        /// Returns max level of scaler.
        /// </summary>
        public abstract int MaxLevel { get; }
        /// <summary>
        /// Returns `true` if scaler can no longer be applied, otherwise returns `false`.
        /// </summary>
        public bool IsMaxLevel { get => CurrentLevel == MaxLevel; }
        /// <summary>
        /// Returns `true` if scaler is not applied otherwise `false`.
        /// </summary>
        public bool NotLeveled { get => CurrentLevel == 0; }
        /// <summary>
        /// Scaler impact on GPU so far in milliseconds.
        /// </summary>
        public int GpuImpact { get; internal set; }
        /// <summary>
        /// Scaler impact on CPU so far in milliseconds.
        /// </summary>
        public int CpuImpact { get; internal set; }

        private int m_OverrideLevel = -1;

        /// <summary>
        /// Allows manually overriding level.
        /// If value -1 <see cref="AdaptivePerformanceIndexer"/> will handle levels, otherwise scaler will be forced to specified value.
        /// </summary>
        public int OverrideLevel
        {
            get => m_OverrideLevel;
            set
            {
                m_OverrideLevel = value;
                m_Indexer.UpdateOverrideLevel(this);
            }
        }

        /// <summary>
        /// Calculate the cost of applying this particular scaler.
        /// </summary>
        /// <returns>Cost of scaler ranges from 0 to infinity.</returns>
        public int CalculateCost()
        {
            var bottleneck = Holder.Instance.PerformanceStatus.PerformanceMetrics.PerformanceBottleneck;

            var cost = 0;

            switch (VisualImpact)
            {
                case ScalerVisualImpact.Low:
                    cost += CurrentLevel * 1;
                    break;
                case ScalerVisualImpact.Medium:
                    cost += CurrentLevel * 2;
                    break;
                case ScalerVisualImpact.High:
                    cost += CurrentLevel * 3;
                    break;
            }

            // Bottlneck should always be best priority
            if (bottleneck == PerformanceBottleneck.CPU && (Target & ScalerTarget.CPU) == 0)
                cost = 6;
            if (bottleneck == PerformanceBottleneck.GPU && (Target & ScalerTarget.GPU) == 0)
                cost = 6;
            if (bottleneck == PerformanceBottleneck.TargetFrameRate && (Target & ScalerTarget.FillRate) == 0)
                cost = 6;

            return cost;
        }

        private void OnEnable()
        {
            m_Indexer = Holder.Instance.Indexer;
            m_Indexer.AddScaler(this);
        }

        private void OnDisable()
        {
            m_Indexer.RemoveScaler(this);
        }

        internal void IncreaseLevel()
        {
            if (IsMaxLevel)
            {
                Debug.LogError("Can not increase scaler level as it is already max.");
                return;
            }
            CurrentLevel++;
            OnLevelIncrease();
            OnLevel();
        }

        internal void DecreaseLevel()
        {
            if (NotLeveled)
            {
                Debug.LogError("Can not decrease scaler level as it is already 0.");
                return;
            }
            CurrentLevel--;
            OnLevelDecrease();
            OnLevel();
        }

        /// <summary>
        /// Callback for when the performance level is increased.
        /// </summary>
        protected virtual void OnLevelIncrease() {}
        /// <summary>
        /// Callback for when the performance level is decreased.
        /// </summary>
        protected virtual void OnLevelDecrease() {}
        /// <summary>
        /// Callback for any level change
        /// </summary>
        protected virtual void OnLevel() {}
    }
}
