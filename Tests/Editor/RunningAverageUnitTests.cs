using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance.Editor.Tests
{
    public class RunningAverageUnitTests
    {
        RunningAverage runAverage;

        [SetUp]
        public void SetupTest()
        {
            runAverage = new RunningAverage();
        }

        [Test]
        public void CheckSampleSizeWindowWhenNoneSet()
        {
            Assert.AreEqual(100, runAverage.GetSampleWindowSize());
        }

        [Test]
        public void CheckSampleSizeWindowWhenSet()
        {
            Assert.AreEqual(4000, new RunningAverage(4000).GetSampleWindowSize());
        }

        [Test]
        public void CheckAverageWhenNoSampleWindowSizeSet_And_NoNewValueGiven()
        {
            runAverage.AddValue(0);
            Assert.AreEqual(1, runAverage.GetNumValues());
        }

        [Test]
        public void CheckAverage_NoSampleSize_And_NewValuesToAddGiven()
        {
            runAverage.AddValue(110);
            Assert.AreEqual(110, runAverage.GetAverageOr(1));
        }

        [Test]
        public void CheckAverage_SampleSizeGiven_And_NewValuesToAddGiven()
        {
            RunningAverage rv = new RunningAverage(230);
            rv.AddValue(156);
            Assert.AreEqual(156, rv.GetAverageOr(90));
        }

        [Test]
        public void CheckMostRecentVal_NoSampleSize_And_NewValuesToAddGiven()
        {
            runAverage.AddValue(249);
            Assert.AreEqual(249, runAverage.GetMostRecentValueOr(24));
        }

        [Test]
        public void CheckMostRecentVal_SampleSizeGiven_And_NewValuesToAddGiven()
        {
            RunningAverage rv = new RunningAverage(220);
            rv.AddValue(973);
            Assert.AreEqual(973, rv.GetMostRecentValueOr(23));
        }

        [Test]
        public void ResetValues_CheckNumberOfValues()
        {
            RunningAverage rv = new RunningAverage(7532);
            rv.Reset();
            Assert.AreEqual(0, rv.GetNumValues());
        }

        [Test]
        public void ResetValues_CheckSampleSizeRemainsSame()
        {
            RunningAverage rv = new RunningAverage(2452);
            rv.Reset();
            Assert.AreEqual(2452, rv.GetSampleWindowSize());
        }

    }

}
