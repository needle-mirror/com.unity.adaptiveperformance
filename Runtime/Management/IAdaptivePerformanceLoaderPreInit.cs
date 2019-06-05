#if UNITY_EDITOR
using UnityEditor;

namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// AdaptivePerformanceLoader interface for retrieving the Adaptive Performance PreInit library name from an AdaptivePerformanceLoader instance
    /// </summary>
    public interface IAdaptivePerformanceLoaderPreInit
    {
        /// <summary>
        /// Get the library name, if any, to use for Adaptive Performance PreInit.
        /// </summary>
        ///
        /// <param name="buildTarget">An enum specifying which platform this build is for.</param>
        /// <param name="buildTargetGroup">An enum specifying which platform group this build is for.</param>
        /// <returns>A string specifying the library name used for Adaptive Performance PreInit.</returns>
        string GetPreInitLibraryName(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup);
    }
}
#endif
