#if UNITY_EDITOR

using System;
using UnityEngine.Scripting;
using UnityEngine.AdaptivePerformance.Provider;

[assembly: AlwaysLinkAssembly]
namespace UnityEngine.AdaptivePerformance.Simulator
{
    /// <summary>
    /// The subsystem is used for simulating Adaptive Performance in the Editor with the Device Simulator. It is also used for Adaptive Performance tests and can be used to simulate Adaptive Performance when it is not available on the hardware you work with. 
    /// </summary>
    [Preserve]
    public class SimulatorAdaptivePerformanceSubsystem : AdaptivePerformanceSubsystem, IApplicationLifecycle, IDevicePerformanceLevelControl
    {
        /// <summary>
        /// Only for internal use.
        /// </summary>
        public Feature ChangeFlags
        {
            get { return updateResult.ChangeFlags; }
            set { updateResult.ChangeFlags = value; }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public float TemperatureLevel
        {
            get { return updateResult.TemperatureLevel; }
            set
            {
                updateResult.TemperatureLevel = value;
                updateResult.ChangeFlags |= Provider.Feature.TemperatureLevel;
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public float TemperatureTrend
        {
            get { return updateResult.TemperatureTrend; }
            set
            {
                updateResult.TemperatureTrend = value;
                updateResult.ChangeFlags |= Provider.Feature.TemperatureTrend;
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public WarningLevel WarningLevel
        {
            get { return updateResult.WarningLevel; }
            set
            {
                updateResult.WarningLevel = value;
                updateResult.ChangeFlags |= Provider.Feature.WarningLevel;
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public int CpuPerformanceLevel
        {
            get { return updateResult.CpuPerformanceLevel; }
            set
            {
                updateResult.CpuPerformanceLevel = value;
                updateResult.ChangeFlags |= Provider.Feature.CpuPerformanceLevel;
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public int GpuPerformanceLevel
        {
            get { return updateResult.GpuPerformanceLevel; }
            set
            {
                updateResult.GpuPerformanceLevel = value;
                updateResult.ChangeFlags |= Provider.Feature.GpuPerformanceLevel;
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public float NextGpuFrameTime
        {
            get { return updateResult.GpuFrameTime; }
            set
            {
                updateResult.GpuFrameTime = value;
                updateResult.ChangeFlags |= Provider.Feature.GpuFrameTime;
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public float NextCpuFrameTime
        {
            get { return updateResult.CpuFrameTime; }
            set
            {
                updateResult.CpuFrameTime = value;
                updateResult.ChangeFlags |= Provider.Feature.CpuFrameTime;
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public float NextOverallFrameTime
        {
            get { return updateResult.OverallFrameTime; }
            set
            {
                updateResult.OverallFrameTime = value;
                updateResult.ChangeFlags |= Provider.Feature.OverallFrameTime;
            }
        }

        /// <summary>
        /// Is needed when you want to simulate performance changes. To change AutomaticPerformanceControl you have to set AcceptsPerformanceLevel to `true`.
        /// </summary>
        public bool AcceptsPerformanceLevel
        {
            get { return updateResult.PerformanceLevelControlAvailable; }
            set
            {
                updateResult.PerformanceLevelControlAvailable = value;
                updateResult.ChangeFlags |= Provider.Feature.PerformanceLevelControl;
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public override Version Version
        {
            get
            {
                return new Version(1, 0, 0);
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public int MaxCpuPerformanceLevel
        {
            get
            {
                return 6;
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public int MaxGpuPerformanceLevel
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public Feature InitCapabilities
        {
            set { Capabilities = value; }
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public SimulatorAdaptivePerformanceSubsystem()
        {
            Capabilities = Feature.CpuPerformanceLevel | Feature.GpuPerformanceLevel | Feature.PerformanceLevelControl |
                Feature.TemperatureLevel | Feature.WarningLevel | Feature.TemperatureTrend | Feature.CpuFrameTime | Feature.GpuFrameTime | Feature.OverallFrameTime;
            updateResult.PerformanceLevelControlAvailable = true;
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        override public void Start()
        {
            initialized = true;
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        override public void Stop()
        {
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        protected override void OnDestroy() { }

        private PerformanceDataRecord updateResult = new PerformanceDataRecord();

        /// <summary>
        /// Only for internal use.
        /// </summary>
        /// <returns></returns>
        override public PerformanceDataRecord Update()
        {
            updateResult.ChangeFlags &= Capabilities;
            var result = updateResult;
            updateResult.ChangeFlags = Feature.None;
            return result;
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public void ApplicationPause()
        {
        }
    
        /// <summary>
        /// Only for internal use.
        /// </summary>
        public void ApplicationResume()
        {
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public override IApplicationLifecycle ApplicationLifecycle { get { return null; } }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public override IDevicePerformanceLevelControl PerformanceLevelControl { get { return this; } }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        public int LastRequestedCpuLevel { get; set; }
        /// <summary>
        /// Only for internal use.
        /// </summary>
        public int LastRequestedGpuLevel { get; set; }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        /// <param name="cpuLevel"></param>
        /// <param name="gpuLevel"></param>
        /// <returns></returns>
        public bool SetPerformanceLevel(int cpuLevel, int gpuLevel)
        {
            LastRequestedCpuLevel = cpuLevel;
            LastRequestedGpuLevel = gpuLevel;

            if (!AcceptsPerformanceLevel)
            {
                CpuPerformanceLevel = AdaptivePerformance.Constants.UnknownPerformanceLevel;
                GpuPerformanceLevel = AdaptivePerformance.Constants.UnknownPerformanceLevel;
                return false;
            }

            return cpuLevel >= 0 && gpuLevel >= 0 && cpuLevel <= MaxCpuPerformanceLevel && gpuLevel <= MaxGpuPerformanceLevel;
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        /// <returns></returns>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static AdaptivePerformanceSubsystemDescriptor RegisterDescriptor()
        {
            return AdaptivePerformanceSubsystemDescriptor.RegisterDescriptor(new AdaptivePerformanceSubsystemDescriptor.Cinfo
            {
                id = "SimulatorAdaptivePerformanceSubsystem",
                subsystemImplementationType = typeof(SimulatorAdaptivePerformanceSubsystem)
            });
        }

        /// <summary>
        /// Only for internal use.
        /// </summary>
        /// <returns></returns>
        public static SimulatorAdaptivePerformanceSubsystem Initialize()
        {
            var desc = RegisterDescriptor();
            if (desc == null)
                return null;
            var subsystem = desc.Create();
            return subsystem as SimulatorAdaptivePerformanceSubsystem;
        }
    }
}
#endif
