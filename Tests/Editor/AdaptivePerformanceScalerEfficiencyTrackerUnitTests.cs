using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEditor.AdaptivePerformance.Editor.Tests
{
    public class AdaptivePerformanceScalerEfficiencyTrackerUnitTests
    {
        AdaptivePerformanceScalerEfficiencyTracker testSubject;

        [SetUp]
        public void startFixture()
        {
            Holder.Instance = new MyAPPerformance();
            testSubject = new AdaptivePerformanceScalerEfficiencyTracker();
        }

        [Test]
        public void ScalarNotRunning_WhenTracker_NeitherStarted_OrStopped()
        {
            Assert.AreEqual(false, testSubject.IsRunning);
        }

        [Test]
        public void ScalarNotRunning_WhenStarted_ButProvidedScalerNotInitialized()
        {
            testSubject.Start(null, false);
            Assert.AreEqual(false, testSubject.IsRunning);
        }

        [Test]
        public void ScalarRunning_WhenStarted_ProvidedScalerInitialized()
        {
            testSubject.Start(new DummyScaler(), false);
            Assert.AreEqual(true, testSubject.IsRunning);
        }

        [Test]
        public void ScalarNotRunning_WhenStartedThenStopped_ProvidedScalerInitialized()
        {
            testSubject.Start(new DummyScaler(), false);
            testSubject.Stop();
            Assert.AreEqual(false, testSubject.IsRunning);
        }

        [Test]
        public void ScalarNotRunning_WhenonlyStopped()
        {
            try
            {
                testSubject.Stop();
                Assert.Fail(("One cannot just stop the scalar without having started / initialized it"));
            }
            catch (NullReferenceException)
            {
                Assert.AreEqual(true, true);
            }
        }
    }

    public class MyAPPerformance : IAdaptivePerformance
    {
        public bool Initialized { get; set; }
        public bool Active { get; }

        public IThermalStatus ThermalStatus { get; }

        public IPerformanceStatus PerformanceStatus
        {
            get { return new MyPerfStatus(); }
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

    public class MyPerfStatus : IPerformanceStatus
    {
        public PerformanceMetrics PerformanceMetrics { get; }

        public UnityEngine.AdaptivePerformance.FrameTiming FrameTiming { get; }

        public event PerformanceBottleneckChangeHandler PerformanceBottleneckChangeEvent;

        public event PerformanceLevelChangeHandler PerformanceLevelChangeEvent;

        public event PerformanceBoostChangeHandler PerformanceBoostChangeEvent;
        public PerformanceMode PerformanceMode { get; set; }
    }

    public class DummyScaler : AdaptivePerformanceScaler { }
}
