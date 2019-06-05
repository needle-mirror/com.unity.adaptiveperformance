using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AdaptivePerformance;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEditor.AdaptivePerformance.Simulator.Editor
{
    /// <summary>
    /// SimulatorProviderLoader implents the loader for Adaptive Performance device Simulator Extension.
    /// </summary>
    [AdaptivePerformanceSupportedBuildTargetAttribute(BuildTargetGroup.Standalone)]
    public class SimulatorProviderLoader : AdaptivePerformanceLoaderHelper
    {
        static List<AdaptivePerformanceSubsystemDescriptor> s_SimulatorSubsystemDescriptors =
            new List<AdaptivePerformanceSubsystemDescriptor>();

        /// <summary>Return the currently active Simulator Subsystem intance, if any.</summary>
        public SimulatorAdaptivePerformanceSubsystem simulatorSubsystem
        {
            get { return GetLoadedSubsystem<SimulatorAdaptivePerformanceSubsystem>(); }
        }

        /// <summary>
        /// Implementation of <see cref="AdaptivePerformanceLoader.GetDefaultSubsystem"/>
        /// </summary>
        /// <returns>The Simulator as currently loaded default subststem. Adaptive Performance always initilizes the first subsystem and use it as a default as always only one subsystem can be present. The order can be changed in the Adaptive Performance Provider Settings.</returns>
        public override ISubsystem GetDefaultSubsystem()
        {
            return simulatorSubsystem;
        }

        /// <summary>
        /// Implementation of <see cref="AdaptivePerformanceLoader.GetSettings"/>
        /// </summary>
        /// <returns>Returns the Simulator settings.</returns>
        public override IAdaptivePerformanceSettings GetSettings()
        {
            return SimulatorProviderSettings.GetSettings();
        }

        /// <summary>Implementaion of <see cref="AdaptivePerformanceLoader.Initialize"/></summary>
        /// <returns>True if successfully initialized the Simulator subsystem, false otherwise</returns>
        public override bool Initialize()
        {
            CreateSubsystem<AdaptivePerformanceSubsystemDescriptor, SimulatorAdaptivePerformanceSubsystem>(s_SimulatorSubsystemDescriptors, "SimulatorAdaptivePerformanceSubsystem");
            if (simulatorSubsystem == null)
            {
                Debug.LogError("Unable to start the Simulator subsystem.");
            }

            return simulatorSubsystem != null;
        }

        /// <summary>Implementaion of <see cref="AdaptivePerformanceLoader.Start"/></summary>
        /// <returns>True if successfully started the Simulator subsystem, false otherwise</returns>
        public override bool Start()
        {
            StartSubsystem<SimulatorAdaptivePerformanceSubsystem>();
            return true;
        }

        /// <summary>Implementaion of <see cref="AdaptivePerformanceLoader.Stop"/></summary>
        /// <returns>True if successfully stopped the Simulator subsystem, false otherwise</returns>
        public override bool Stop()
        {
            StopSubsystem<SimulatorAdaptivePerformanceSubsystem>();
            return true;
        }

        /// <summary>Implementaion of <see cref="AdaptivePerformanceLoader.Deinitialize"/></summary>
        /// <returns>True if successfully deinitialized the Simulator subsystem, false otherwise</returns>
        public override bool Deinitialize()
        {
            DestroySubsystem<SimulatorAdaptivePerformanceSubsystem>();
            return true;
        }
    }
}
