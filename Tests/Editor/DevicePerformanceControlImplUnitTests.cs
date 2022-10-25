using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEditor.AdaptivePerformance.Editor.Tests
{

    public class DevicePerformanceControlImplUnitTests
    {
        [Test]
        public void UpdateDoesNotOccur_When_ControlModeIsNotSystem_And_CPULevelUnknown()
        {
            DevicePerformanceControlImpl dp = new DevicePerformanceControlImpl(new MyDevicePerformanceLevelControl());
            PerformanceLevelChangeEventArgs pce = new PerformanceLevelChangeEventArgs();
            Assert.AreEqual(false, dp.Update(out pce));
        }

        [Test]
        public void UpdateDoesNotOccur_When_ControlModeIsSystem_And_CPULevelUnknown()
        {
            DevicePerformanceControlImpl dp = new DevicePerformanceControlImpl(new MyDevicePerformanceLevelControl());
            dp.PerformanceControlMode = PerformanceControlMode.System;
            PerformanceLevelChangeEventArgs pce = new PerformanceLevelChangeEventArgs();
            Assert.AreEqual(false, dp.Update(out pce));
        }

        [Test]
        public void UpdateDoesNotOccur_When_ControlModeIsSystem_And_CPULevelSet_DefaultAverage()
        {
            DevicePerformanceControlImpl dp = new DevicePerformanceControlImpl(new MyDevicePerformanceLevelControl());
            dp.CurrentCpuLevel = Constants.DefaultAverageFrameCount;
            dp.PerformanceControlMode = PerformanceControlMode.System;
            PerformanceLevelChangeEventArgs pce = new PerformanceLevelChangeEventArgs();
            dp.Update(out pce);
            Assert.AreEqual(Constants.UnknownPerformanceLevel, pce.CpuLevel);
        }

        [Test]
        public void UpdateDoesNotOccur_When_ControlModeIsNotSystem_And_CPULevelIsAverageFramerate()
        {
            DevicePerformanceControlImpl dp = new DevicePerformanceControlImpl(new MyDevicePerformanceLevelControl());
            dp.CpuLevel = Constants.DefaultAverageFrameCount;
            dp.PerformanceControlMode = PerformanceControlMode.Manual;
            PerformanceLevelChangeEventArgs pce = new PerformanceLevelChangeEventArgs();
            dp.Update(out pce);
            Assert.AreEqual(false, dp.Update(out pce));
        }

        [Test]
        public void UpdateDoesOccur_When_ControlModeIsmanual_And_CPULevelIsAverageFramerate()
        {
            DevicePerformanceControlImpl dp = new DevicePerformanceControlImpl(new MyDevicePerformanceLevelControl());
            dp.CpuLevel = Constants.DefaultAverageFrameCount;
            dp.PerformanceControlMode = PerformanceControlMode.Manual;
            PerformanceLevelChangeEventArgs pce = new PerformanceLevelChangeEventArgs();
            Assert.AreEqual(true, dp.Update(out pce));
        }
    }

    public class MyDevicePerformanceLevelControl : IDevicePerformanceLevelControl
    {
        public int MaxCpuPerformanceLevel { get; }

        public int MaxGpuPerformanceLevel { get; }

        public bool SetPerformanceLevel(ref int cpu, ref int gpu)
        {
            return true;
        }

        public bool EnableCpuBoost()
        {
            return false;
        }

        public bool EnableGpuBoost()
        {
            return false;
        }
    }

}
