using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance.Editor.Tests
{
    public class AdaptivePerformanceLoaderHelperUnitTests : AdaptivePerformanceLoaderHelper
    {
        [Test]
        public void CreateSubsystemWithNullDescriptersProvided_ExceptionThrown()
        {
            try
            {
                CreateSubsystem<ISubsystemDescriptor, ISubsystem>(null, "something");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(ArgumentNullException), ex.GetType());
            }
        }

        [Test]
        public void CreateSubsystemWithEmptyDescriptersProvided_NoExceptionThrown()
        {
            try
            {
                CreateSubsystem<ISubsystemDescriptor, ISubsystem>(new List<ISubsystemDescriptor>(), "something");
            }
            catch (Exception ex)
            {
                Assert.Fail("Not meant to throw an Exception : " + ex.GetType());
            }
        }

        [Test]
        public void CreateSubsystemWithAtLeastOneDescripter_NoExceptionThrown()
        {
            try
            {
                List<ISubsystemDescriptor> sdList = new List<ISubsystemDescriptor>();
                sdList.Add(new MyOwnSubsystemDescriptor());
                CreateSubsystem<ISubsystemDescriptor, ISubsystem>(sdList, "something");
            }
            catch (Exception ex)
            {
                Assert.Fail("Not meant to throw an Exception : " + ex.GetType());
            }
        }

        public override bool Initialized { get; }
        public override bool Running { get; }

        public override ISubsystem GetDefaultSubsystem()
        {
            return null;
        }

        public override IAdaptivePerformanceSettings GetSettings()
        {
            return null;
        }
    }

    public class MyOwnSubsystemDescriptor : ISubsystemDescriptor
    {
        public string id { get; }

        public ISubsystem Create()
        {
            return null;
        }
    }
}
