using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance.Editor.Tests
{
    public class GpuTimeProviderUnitTests
    {
        [Test]
        public void VerifyFrameTime_WhenlatestTiming_IsZero()
        {
            Assert.AreEqual(-1.0f,new GpuTimeProvider().GpuFrameTime);
        }
    }
}
