using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEditor.AdaptivePerformance.Editor.Tests
{
    public class PerformanceStateTrackerUnitTests
    {
        MyAdaptivePerformanceInst m_ap;

        [SetUp]
        public void InitializeTest()
        {
            m_ap = new MyAdaptivePerformanceInst();
            Holder.Instance = m_ap;
        }

        [Test]
        public void StateAction_Stale_When_SampleCapacityZero()
        {
            m_ap.PerformanceStatus = new MyPerformanceStatusInst { FrameTiming = new UnityEngine.AdaptivePerformance.FrameTiming() };
            Assert.AreEqual(StateAction.Stale, new MyPerformanceStateTracker(0).Update());
        }

        [TestCase((1f / 120), StateAction.Stale)]
        [TestCase((1f / 60), StateAction.Stale)]
        [TestCase((1f / 50), StateAction.Decrease)]
        [TestCase((1f / 45), StateAction.FastDecrease)]
        [TestCase((1f / 30), StateAction.FastDecrease)]
        [TestCase((1f / 15), StateAction.FastDecrease)]
        public void StateAction_When_AverageFrameTime(float averageFrameTime, StateAction stateAction)
        {
            m_ap.PerformanceStatus = new MyPerformanceStatusInst { FrameTiming = new UnityEngine.AdaptivePerformance.FrameTiming { AverageFrameTime = averageFrameTime } };
            Assert.AreEqual(stateAction, new MyPerformanceStateTracker(100).Update());
        }

        [Test]
        public void StateAction_Decrease_When_SampleCapacity_WierdValue()
        {
            m_ap.PerformanceStatus = new MyPerformanceStatusInst { FrameTiming = new UnityEngine.AdaptivePerformance.FrameTiming { AverageFrameTime = 1f } };
            Assert.AreEqual(StateAction.Decrease, new SmallTFRPerformanceStateTracker(10).Update());
        }
    }

    class MyPerformanceStateTracker : PerformanceStateTracker
    {
        public MyPerformanceStateTracker(int sampleCapacity)
            : base(sampleCapacity) { }

        protected override float GetEffectiveTargetFrameRate()
        {
            return 60f;
        }
    }

    class SmallTFRPerformanceStateTracker : PerformanceStateTracker
    {
        public SmallTFRPerformanceStateTracker(int sampleCapacity)
            : base(sampleCapacity) { }

        protected override float GetEffectiveTargetFrameRate()
        {
            return 1.2f;
        }
    }

    public class MyAdaptivePerformanceInst : IAdaptivePerformance
    {
        public bool Initialized { get; set; }

        public bool Active { get; }

        public IThermalStatus ThermalStatus { get; }

        IPerformanceStatus m_PerformanceStatus;

        public IPerformanceStatus PerformanceStatus
        {
            get => m_PerformanceStatus;
            set => m_PerformanceStatus = value;
        }

        public IDevicePerformanceControl DevicePerformanceControl { get; }

        public IPerformanceModeStatus PerformanceModeStatus { get; }

        public IDevelopmentSettings DevelopmentSettings { get; }

        public AdaptivePerformanceIndexer Indexer { get; }

        public IAdaptivePerformanceSettings Settings { get; }

        public bool SupportedFeature(Feature feature)
        {
            return false;
        }

        public void InitializeAdaptivePerformance() { }

        public void StartAdaptivePerformance() { }

        public void StopAdaptivePerformance() { }

        public void DeinitializeAdaptivePerformance() { }
    }

    public class MyPerformanceStatusInst : IPerformanceStatus
    {
        public PerformanceMetrics PerformanceMetrics { get; }

        UnityEngine.AdaptivePerformance.FrameTiming m_FrameTiming;

        public UnityEngine.AdaptivePerformance.FrameTiming FrameTiming
        {
            get => m_FrameTiming;
            set => m_FrameTiming = value;
        }

        public event PerformanceBottleneckChangeHandler PerformanceBottleneckChangeEvent;
        public event PerformanceLevelChangeHandler PerformanceLevelChangeEvent;
        public event PerformanceBoostChangeHandler PerformanceBoostChangeEvent;

        public PerformanceMode PerformanceMode { get; set; }
    }
}
