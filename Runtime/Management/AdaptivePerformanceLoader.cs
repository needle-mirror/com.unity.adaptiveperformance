using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// Adaptive Performance Loader abstract class used as a base class for specific provider implementations. Providers should implement
    /// subclasses of this to provide specific initialization and management implementations that make sense for their supported
    /// scenarios and needs.
    /// </summary>
    public abstract class AdaptivePerformanceLoader : ScriptableObject
    {
        /// <summary>
        /// Initialize the loader. This should initialize all subsystems to support the desired runtime setup this
        /// loader represents.
        /// </summary>
        ///
        /// <returns>True if initialization succeeded, false otherwise.</returns>
        public virtual bool Initialize() { return false; }

        /// <summary>
        /// Ask loader to start all initialized subsystems.
        /// </summary>
        ///
        /// <returns>True if all subsystems were successfully started, false otherwise.</returns>
        public virtual bool Start() { return false; }

        /// <summary>
        /// Ask loader to stop all initialized subsystems.
        /// </summary>
        ///
        /// <returns>True if all subsystems were successfully stopped, false otherwise.</returns>
        public virtual bool Stop() { return false; }

        /// <summary>
        /// Ask loader to deinitialize all initialized subsystems.
        /// </summary>
        ///
        /// <returns>True if deinitialization succeeded, false otherwise.</returns>
        public virtual bool Deinitialize() { return false; }

        /// <summary>
        /// Gets the loaded subsystem of the specified type. This is implementation-specific, because implementations contain data on
        /// what they have loaded and how best to get it.
        /// </summary>
        ///
        /// <typeparam name="T">Type of the subsystem to get.</typeparam>
        ///
        /// <returns>The loaded subsystem, or null if no subsystem found.</returns>
        public abstract T GetLoadedSubsystem<T>() where T : class, ISubsystem;

        /// <summary>
        /// Gets the loaded default subsystem.
        /// </summary>
        /// <returns>The loaded subsystem, or null if no default subsystem is loaded.</returns>
        public abstract ISubsystem GetDefaultSubsystem();

        /// <summary>
        /// Gets the Settings of the loader used to descibe the loader and subsystems.
        /// </summary>
        /// <returns>The settings of the loader.</returns>
        public abstract IAdaptivePerformanceSettings GetSettings();
    }
}
