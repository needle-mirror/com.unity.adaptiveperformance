using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.AdaptivePerformance
{
    // TODO: Enum and values is very vague, we should come up something better
    /// <summary>
    /// Describes what action is needed to stabilize.
    /// </summary>
    public enum StateAction
    {
        /// <summary>
        /// No action is required.
        /// </summary>
        Stale,
        /// <summary>
        /// Recommended to increase quality.
        /// </summary>
        Increase,
        /// <summary>
        /// Quality must be decreased.
        /// </summary>
        Decrease,
        /// <summary>
        /// Quality must be decreased as soon as possible.
        /// </summary>
        FastDecrease,
    }

    /// <summary>
    /// Describes what thermal method is used for figuring out thermal state of device.
    /// </summary>
    public enum ThermalStateMode
    {
        /// <summary>
        /// Temperature state is calculated from thermal level and thermal trend.
        /// Has better control over thermal state, but not very accurate between device versions.
        /// </summary>
        TemperatureLevelBased,
        /// <summary>
        /// Temperature state is calculated from thermal warning and thermal trend.
        /// </summary>
        WarningBased,
    }

    /// <summary>
    /// System used for tracking device thermal state.
    /// </summary>
    internal class ThermalStateTracker
    {
        private Vector2 m_SafeRange = new Vector2(0.6f, 0.7f);
        private Queue<float> m_Samples;
        private int m_SampleCapacity;
        private float m_LastThermalTrend;

        public Vector2 SafeRange
        {
            get { return m_SafeRange; }
            set { m_SafeRange = value; }
        }

        public float Trend { get; private set; }
        public ThermalStateMode Mode { get; set; }

        public ThermalStateTracker(int sampleCapacity)
        {
            m_Samples = new Queue<float>(sampleCapacity);
            m_SampleCapacity = sampleCapacity;
        }

        public StateAction Update()
        {
            var thermalTrend = Holder.Instance.ThermalStatus.ThermalMetrics.TemperatureTrend;
            var diff = thermalTrend - m_LastThermalTrend;
            m_LastThermalTrend = thermalTrend;

            m_Samples.Enqueue(diff);
            if (m_Samples.Count > m_SampleCapacity)
                m_Samples.Dequeue();

            // Calculate from samples if trend is increasing or decreasing
            var trend = 0.0f;
            foreach (var sample in m_Samples)
                trend += sample;
            Trend = trend;

            if (Mode == ThermalStateMode.TemperatureLevelBased)
            {
                var thermalLevel = Holder.Instance.ThermalStatus.ThermalMetrics.TemperatureLevel;
                var min = m_SafeRange.x <= thermalLevel;
                var max = m_SafeRange.y >= thermalLevel;

                // This is thermal safe zone and this is where we want to be ideally, no action needed
                if (min && max)
                    return StateAction.Stale;

                // This is thermal cold zone, it means we can push device and it is recommended to get away from it
                if (thermalLevel <= 0.20f)
                    return StateAction.Increase;

                // This is thermal hot zone and it is very close to throttling, we should do everything to get away from it
                if (thermalLevel >= 0.8f)
                    return StateAction.FastDecrease;

                // This is thermal cool zone, we still have space to increase thermal levels to get into safe zone
                if (!min && trend <= 0)
                    return StateAction.Increase;

                // This is thermal mild zone, we have to decrease thermal levels to get back into safe zone
                if (!max && trend >= 0)
                    return StateAction.Decrease;
            }

            if (Mode == ThermalStateMode.WarningBased)
            {
                var warning = Holder.Instance.ThermalStatus.ThermalMetrics.WarningLevel;

                if (warning == WarningLevel.NoWarning && trend <= 0)
                    return StateAction.Increase;

                if (warning == WarningLevel.ThrottlingImminent && trend >= 0)
                    return StateAction.Decrease;

                if (warning == WarningLevel.Throttling)
                    return StateAction.FastDecrease;
            }

            return StateAction.Stale;
        }
    }

    /// <summary>
    /// System used for tracking device performance state.
    /// </summary>
    internal class PerformanceStateTracker
    {
        private Queue<float> m_Samples;
        private int m_SampleCapacity;

        public float Trend { get; set; }

        public PerformanceStateTracker(int sampleCapacity)
        {
            m_Samples = new Queue<float>(sampleCapacity);
            m_SampleCapacity = sampleCapacity;
        }

        public StateAction Update()
        {
            var frameMs = Holder.Instance.PerformanceStatus.FrameTiming.AverageFrameTime;
            if (frameMs > 0)
            {
                var targetMs = 1f / AdaptivePerformanceManager.EffectiveTargetFrameRate();
                var diffMs = (frameMs / targetMs) - 1;

                m_Samples.Enqueue(diffMs);
                if (m_Samples.Count > m_SampleCapacity)
                    m_Samples.Dequeue();
            }

            var trend = 0.0f;
            foreach (var sample in m_Samples)
                trend += sample;
            trend /= m_Samples.Count;
            Trend = trend;

            // It is underperforming heavily, we need to increase performance
            if (trend >= 0.30)
                return StateAction.FastDecrease;

            // It is underperforming, we need to increase performance
            if (trend >= 0.15)
                return StateAction.Decrease;

            // TODO: we need to way identify overperforming as currently AverageFrameTime is returned with vsync
            // return StaterAction.Increase;

            return StateAction.Stale;
        }
    }

    /// <summary>
    /// System used for tracking impact of scaler on cpu and gpu counters.
    /// </summary>
    internal class AdaptivePerformanceScalerEfficiencyTracker
    {
        private AdaptivePerformanceScaler m_Scaler;
        private float m_LastAverageGpuFrameTime;
        private float m_LastAverageCpuFrameTime;
        private bool m_IsApplied;

        public bool IsRunning { get => m_Scaler != null; }

        public void Start(AdaptivePerformanceScaler scaler, bool isApply)
        {
            Debug.Assert(!IsRunning, "AdaptivePerformanceScalerEfficiencyTracker is already running");
            m_Scaler = scaler;
            m_LastAverageGpuFrameTime = Holder.Instance.PerformanceStatus.FrameTiming.AverageGpuFrameTime;
            m_LastAverageCpuFrameTime = Holder.Instance.PerformanceStatus.FrameTiming.AverageCpuFrameTime;
            m_IsApplied = true;
        }

        public void Stop()
        {
            var gpu = Holder.Instance.PerformanceStatus.FrameTiming.AverageGpuFrameTime - m_LastAverageGpuFrameTime;
            var cpu = Holder.Instance.PerformanceStatus.FrameTiming.AverageCpuFrameTime - m_LastAverageCpuFrameTime;
            var sign = m_IsApplied ? 1 : -1;
            m_Scaler.GpuImpact = sign * (int)(gpu * 1000);
            m_Scaler.CpuImpact = sign * (int)(cpu * 1000);
            m_Scaler = null;
        }
    }

    /// <summary>
    /// Higher level implementation of Adaptive performance that tracks performance and thermal state of device and applies <see cref="AdaptivePerformanceScaler"/> according it.
    /// System acts <see cref="AdaptivePerformanceScaler"/> manager and handles all lifetime of it in scene.
    /// </summary>
    public class AdaptivePerformanceIndexer
    {
        private List<AdaptivePerformanceScaler> m_UnappliedScalers;
        private List<AdaptivePerformanceScaler> m_AppliedScalers;
        private ThermalStateTracker m_ThermalStateTracker;
        private PerformanceStateTracker m_PerformanceStateTracker;
        private AdaptivePerformanceScalerEfficiencyTracker m_ScalerEfficiencyTracker;
        private IAdaptivePerformanceSettings m_Settings;
        const string m_FeatureName = "Indexer";

        /// <summary>
        /// Time left until next action.
        /// </summary>
        public float TimeUntilNextAction { get; private set; }

        /// <summary>
        /// Current determined action needed from thermal state.
        /// Action <see cref="StateAction.Increase"/> will be ignored if <see cref="PerformanceAction"/> is in decrease.
        /// </summary>
        public StateAction ThermalAction { get; private set; }

        /// <summary>
        /// Current determined action needed from performance state.
        /// Action <see cref="StateAction.Increase"/> will be ignored if <see cref="ThermalAction"/> is in decrease.
        /// </summary>
        public StateAction PerformanceAction { get; private set; }

        /// <summary>
        /// Returns all currently applied scalers.
        /// </summary>
        /// <param name="scalers">Output where scalers will be written.</param>
        public void GetAppliedScalers(ref List<AdaptivePerformanceScaler> scalers)
        {
            scalers.Clear();
            scalers.AddRange(m_AppliedScalers);
        }

        /// <summary>
        /// Returns all currently unapplied scalers.
        /// </summary>
        /// <param name="scalers">Output where scalers will be written.</param>
        public void GetUnappliedScalers(ref List<AdaptivePerformanceScaler> scalers)
        {
            scalers.Clear();
            scalers.AddRange(m_UnappliedScalers);
        }

        /// <summary>
        /// Unapply all currently active scalers.
        /// </summary>
        public void UnapplyAllScalers()
        {
            TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay;
            while (m_AppliedScalers.Count != 0)
            {
                var scaler = m_AppliedScalers[0];
                UnapplyScaler(scaler);
            }
        }

        internal void UpdateOverrideLevel(AdaptivePerformanceScaler scaler)
        {
            if (scaler.OverrideLevel == -1)
                return;
            while (scaler.OverrideLevel > scaler.CurrentLevel)
                ApplyScaler(scaler);
            while (scaler.OverrideLevel < scaler.CurrentLevel)
                UnapplyScaler(scaler);
        }

        internal void AddScaler(AdaptivePerformanceScaler scaler)
        {
            if (m_UnappliedScalers.Contains(scaler) || m_AppliedScalers.Contains(scaler))
                return;

            m_UnappliedScalers.Add(scaler);
        }

        internal void RemoveScaler(AdaptivePerformanceScaler scaler)
        {
            if (m_UnappliedScalers.Contains(scaler))
            {
                m_UnappliedScalers.Remove(scaler);
                return;
            }

            if (m_AppliedScalers.Contains(scaler))
            {
                while (!scaler.NotLeveled)
                    scaler.DecreaseLevel();
                m_AppliedScalers.Remove(scaler);
            }
        }

        internal AdaptivePerformanceIndexer(ref IAdaptivePerformanceSettings settings)
        {
            m_Settings = settings;
            TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay;
            m_ThermalStateTracker = new ThermalStateTracker(120);
            m_PerformanceStateTracker = new PerformanceStateTracker(120);
            m_UnappliedScalers = new List<AdaptivePerformanceScaler>();
            m_AppliedScalers = new List<AdaptivePerformanceScaler>();
            m_ScalerEfficiencyTracker = new AdaptivePerformanceScalerEfficiencyTracker();

            AdaptivePerformanceAnalytics.RegisterFeature(m_FeatureName, m_Settings.indexerSettings.active);
        }

        internal void Update()
        {
            if (Holder.Instance == null || !m_Settings.indexerSettings.active)
                return;

            // Update mode in case it was changed
            m_ThermalStateTracker.Mode = m_Settings.indexerSettings.thermalStateMode;

            var thermalAction = m_ThermalStateTracker.Update();
            var performanceAction = m_PerformanceStateTracker.Update();

            ThermalAction = thermalAction;
            PerformanceAction = performanceAction;

            // Enforce minimum wait time between any scaler changes
            TimeUntilNextAction = Mathf.Max(TimeUntilNextAction - Time.deltaTime, 0);
            if (TimeUntilNextAction != 0)
                return;

            if (m_ScalerEfficiencyTracker.IsRunning)
                m_ScalerEfficiencyTracker.Stop();

            if (thermalAction == StateAction.Increase && performanceAction == StateAction.Stale)
            {
                UnapplyHighestCostScaler();
                TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay;
                return;
            }
            if (thermalAction == StateAction.Decrease)
            {
                ApplyLowestCostScaler();
                TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay;
                return;
            }
            if (performanceAction == StateAction.Decrease)
            {
                ApplyLowestCostScaler();
                TimeUntilNextAction = m_Settings.indexerSettings.performanceActionDelay;
                return;
            }
            if (thermalAction == StateAction.FastDecrease)
            {
                ApplyLowestCostScaler();
                TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay / 2;
                return;
            }
            if (performanceAction == StateAction.FastDecrease)
            {
                ApplyLowestCostScaler();
                TimeUntilNextAction = m_Settings.indexerSettings.performanceActionDelay / 2;
                return;
            }
        }

        private bool ApplyLowestCostScaler()
        {
            AdaptivePerformanceScaler result = null;
            var lowestCost = float.PositiveInfinity;

            foreach (var scaler in m_UnappliedScalers)
            {
                if (scaler.OverrideLevel != -1)
                    continue;

                var cost = scaler.CalculateCost();

                if (lowestCost > cost)
                {
                    result = scaler;
                    lowestCost = cost;
                }
            }

            foreach (var scaler in m_AppliedScalers)
            {
                if (scaler.OverrideLevel != -1)
                    continue;

                if (scaler.IsMaxLevel)
                    continue;

                var cost = scaler.CalculateCost();

                if (lowestCost > cost)
                {
                    result = scaler;
                    lowestCost = cost;
                }
            }

            if (result != null)
            {
                m_ScalerEfficiencyTracker.Start(result, true);

                ApplyScaler(result);
                return true;
            }

            return false;
        }

        private void ApplyScaler(AdaptivePerformanceScaler scaler)
        {
            if (scaler.NotLeveled)
            {
                m_UnappliedScalers.Remove(scaler);
                m_AppliedScalers.Add(scaler);
            }
            scaler.IncreaseLevel();
        }

        private bool UnapplyHighestCostScaler()
        {
            AdaptivePerformanceScaler result = null;
            var highestCost = float.NegativeInfinity;

            foreach (var scaler in m_AppliedScalers)
            {
                if (scaler.OverrideLevel != -1)
                    continue;

                var cost = scaler.CalculateCost();

                if (highestCost < cost)
                {
                    result = scaler;
                    highestCost = cost;
                }
            }

            if (result != null)
            {
                m_ScalerEfficiencyTracker.Start(result, false);

                UnapplyScaler(result);
                return true;
            }

            return false;
        }

        private void UnapplyScaler(AdaptivePerformanceScaler scaler)
        {
            scaler.DecreaseLevel();
            if (scaler.NotLeveled)
            {
                m_AppliedScalers.Remove(scaler);
                m_UnappliedScalers.Add(scaler);
            }
        }
    }
}
