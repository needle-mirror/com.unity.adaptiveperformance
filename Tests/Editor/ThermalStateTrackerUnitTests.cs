using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEditor.AdaptivePerformance.Editor.Tests
{
    public class ThermalStateTrackerUnitTests
    {
        [Test]
        public void CheckStateAction_Throttling_NotImminent_And_WithoutWarningLevel()
        {
            Holder.Instance = new MyZeroTempLevelAdaptPerfInst();
            Assert.AreEqual(StateAction.Increase, new ThermalStateTracker().Update());
        }

        [Test]
        public void CheckStateAction_With_WarningLevel_Throttling()
        {
            Holder.Instance = new MyThrottlingWarningAdaptPerfInst();
            Assert.AreEqual(StateAction.FastDecrease, new ThermalStateTracker().Update());
        }

        [Test]
        public void CheckStateAction_With_WarningLevel_ImminentThrottling()
        {
            Holder.Instance = new MyImminentThrottlingAdaptPerfInst();
            Assert.AreEqual(StateAction.Stale, new ThermalStateTracker().Update());
        }
    }

    public class MyImminentThrottlingAdaptPerfInst : IAdaptivePerformance
    {
        public bool Initialized { get; set; }
        public bool Active { get; }

        public IThermalStatus ThermalStatus
        {
            get { return new MyImminentThrottlingThermalStatus(); }
        }

        public IPerformanceStatus PerformanceStatus { get; }

        public IDevicePerformanceControl DevicePerformanceControl { get; }

        public IPerformanceModeStatus PerformanceModeStatus { get; }

        public IDevelopmentSettings DevelopmentSettings { get; }

        public AdaptivePerformanceIndexer Indexer { get; }

        public IAdaptivePerformanceSettings Settings { get; }

        public bool SupportedFeature(Feature feature)
        {
            return true;
        }

        public void InitializeAdaptivePerformance() { }

        public void StartAdaptivePerformance() { }

        public void StopAdaptivePerformance() { }

        public void DeinitializeAdaptivePerformance() { }
    }

    public class MyZeroTempLevelAdaptPerfInst : IAdaptivePerformance
    {
        public bool Initialized { get; set; }
        public bool Active { get; }

        public IThermalStatus ThermalStatus
        {
            get { return new MyZeroTempLevelThermalStatus(); }
        }

        public IPerformanceStatus PerformanceStatus { get; }

        public IDevicePerformanceControl DevicePerformanceControl { get; }

        public IPerformanceModeStatus PerformanceModeStatus { get; }

        public IDevelopmentSettings DevelopmentSettings { get; }

        public AdaptivePerformanceIndexer Indexer { get; }

        public IAdaptivePerformanceSettings Settings { get; }

        public bool SupportedFeature(Feature feature)
        {
            return true;
        }

        public void InitializeAdaptivePerformance() { }

        public void StartAdaptivePerformance() { }

        public void StopAdaptivePerformance() { }

        public void DeinitializeAdaptivePerformance() { }
    }

    public class MyThrottlingWarningAdaptPerfInst : IAdaptivePerformance
    {
        public bool Initialized { get; set; }
        public bool Active { get; }

        public IThermalStatus ThermalStatus
        {
            get { return new MyThrottlingWarningThermalStatus(); }
        }

        public IPerformanceStatus PerformanceStatus { get; }

        public IDevicePerformanceControl DevicePerformanceControl { get; }

        public IPerformanceModeStatus PerformanceModeStatus { get; }

        public IDevelopmentSettings DevelopmentSettings { get; }

        public AdaptivePerformanceIndexer Indexer { get; }

        public IAdaptivePerformanceSettings Settings { get; }

        public bool SupportedFeature(Feature feature)
        {
            return true;
        }

        public void InitializeAdaptivePerformance() { }

        public void StartAdaptivePerformance() { }

        public void StopAdaptivePerformance() { }

        public void DeinitializeAdaptivePerformance() { }
    }

    public class MyZeroTempLevelThermalStatus : IThermalStatus
    {
        public ThermalMetrics ThermalMetrics
        {
            get { return new ThermalMetrics(); }
        }

        public event ThermalEventHandler ThermalEvent;
    }

    public class MyThrottlingWarningThermalStatus : IThermalStatus
    {
        public ThermalMetrics ThermalMetrics
        {
            get
            {
                ThermalMetrics tm = new ThermalMetrics();
                tm.WarningLevel = WarningLevel.Throttling;
                return tm;
            }
        }

        public event ThermalEventHandler ThermalEvent;
    }

    public class MyImminentThrottlingThermalStatus : IThermalStatus
    {
        public ThermalMetrics ThermalMetrics
        {
            get
            {
                ThermalMetrics itm = new ThermalMetrics();
                itm.WarningLevel = WarningLevel.ThrottlingImminent;
                return itm;
            }
        }

        public event ThermalEventHandler ThermalEvent;
    }
}
