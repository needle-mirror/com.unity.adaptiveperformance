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
        /// <returns>Whether or not initialization succeeded.</returns>
        public virtual bool Initialize() { return false; }

        /// <summary>
        /// Ask loader to start all initialized subsystems.
        /// </summary>
        ///
        /// <returns>Whether or not all subsystems were successfully started.</returns>
        public virtual bool Start() { return false; }

        /// <summary>
        /// Ask loader to stop all initialized subsystems.
        /// </summary>
        ///
        /// <returns>Whether or not all subsystems were successfully stopped.</returns>
        public virtual bool Stop() { return false; }

        /// <summary>
        /// Ask loader to deinitialize all initialized subsystems.
        /// </summary>
        ///
        /// <returns>Whether or not deinitialization succeeded.</returns>
        public virtual bool Deinitialize() { return false; }

        /// <summary>
        /// Gets the loaded subsystem of the specified type. Implementation dependent as only implemetnations
        /// know what they have loaded and how best to get it..
        /// </summary>
        ///
        /// <typeparam name="T">Type of the subsystem to get</typeparam>
        ///
        /// <returns>The loaded subsystem or null if not found.</returns>
        public abstract T GetLoadedSubsystem<T>() where T : class, ISubsystem;

        /// <summary>
        /// Gets the loader's supported graphics device types. If the list is empty, it is assumed that it supports all graphics device types.
        /// </summary>
        ///
        /// <param name="buildingPlayer">True if the player is being built. You may want to include or exclude graphics apis if the player is being built or not.</param>
        /// <returns>Returns the loader's supported graphics device types.</returns>
        public virtual List<GraphicsDeviceType> GetSupportedGraphicsDeviceTypes(bool buildingPlayer)
        {
            return new List<GraphicsDeviceType>();
        }

        /// <summary>
        /// Gets the loaded default subsystem.
        /// </summary>
        /// <returns>The loaded subsystem or null if none loaded.</returns>
        public abstract ISubsystem GetDefaultSubsystem();

        /// <summary>
        /// Gets the Settings of the loader used to descibe the loader and subsystems.
        /// </summary>
        /// <returns>The settings of the loader.</returns>
        public abstract IAdaptivePerformanceSettings GetSettings();
    }
}