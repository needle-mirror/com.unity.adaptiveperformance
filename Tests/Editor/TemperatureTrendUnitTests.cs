using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance.Editor.Tests
{
    public class TemperatureTrendUnitTests
    {
        [Test]
        public void CheckThermalTrend_WhenProviderTrendUsed()
        {
            var tt = new TemperatureTrend(true);
            tt.Update(55,0,false,0);
            Assert.AreEqual(55, tt.ThermalTrend);
        }

        [Test]
        public void CheckThermalTrend_WhenProviderTrendNotUsed_AndNumValues_Zero()
        {
            var tt = new TemperatureTrend(false);
            tt.Update(3,45,false,0);
            Assert.AreEqual(0, tt.ThermalTrend);
        }

        [Test]
        public void CheckThermalTrend_TimestampGreaterThanMeasurementTimeframe()
        {
            var tt = new TemperatureTrend(false);
            tt.NumValues = 3;
            tt.Update(3,45,false,100);
            Assert.AreEqual(1, tt.ThermalTrend);
        }
    }
}
