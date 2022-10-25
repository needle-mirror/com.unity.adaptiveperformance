using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance.Editor.Tests
{

    [TestFixture]
    [Ignore("Re-establish this test when sufficient static stubbing framework in place")]
    public class CpuTimeProviderUnitTests
    {
        CpuTimeProvider testProvider;

        [SetUp]
        public void SetupTheTest()
        {
            testProvider = new CpuTimeProvider();
        }

        [Test]
        public void CheckCPUFrameTime_AfterInitialisation_WithoutReset()
        {
            Assert.AreEqual(-1, new CpuTimeProvider().CpuFrameTime);
        }

        [Test]
        public void CheckCPUFrameTime_ImmediatelyAfterReset()
        {
            testProvider.Reset();
            Assert.AreEqual(-1, testProvider.CpuFrameTime);
        }

        [Test]
        public void CheckCPUFrameTime_PerformLatestUpdate()
        {
            testProvider.LateUpdate();
            Assert.AreEqual(-1, testProvider.CpuFrameTime);
        }

        [Test]
        public void CheckCPUFrameTime_EndOfFrameDetected_ThenPerformLatestUpdate()
        {
            testProvider.EndOfFrame();
            testProvider.LateUpdate();
            Assert.AreEqual(-1, testProvider.CpuFrameTime);
        }

        [Test]
        public void CheckCPUFrameTime_EndOfFrameDetected_NoUpdateInstruction()
        {
            testProvider.EndOfFrame();
            Assert.AreEqual(-1, testProvider.CpuFrameTime);
        }
    }
}
