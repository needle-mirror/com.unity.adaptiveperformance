using UnityEngine.Rendering;

namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the resolution of all render targets that allow dynamic resolution.
    /// If dynamic resolution is not supported by the device or graphics API, rendering pipeline's render scale multiplier will be used as a fallback.
    /// </summary>
    public class AdaptiveResolution : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Returns max level of scaler.
        /// </summary>
        public override int MaxLevel => scaleLevels - 1;

        float scaleIncrement = 1.0f;
        /// <summary>
        /// Defines how many steps will be used when scaling between the minimum and maximum.
        /// </summary>
        public int scaleLevels = 10;

        private static int instanceCount = 0;

        /// <summary>
        /// Ensures settings are applied during startup.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (m_Settings == null)
                return;
            ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveResolution);
        }

        void OnValidate()
        {
            if (scaleLevels < 2)
                scaleLevels = 2;
            MaxBound = Mathf.Clamp(MaxBound, 0.25f, 1.0f);
            MinBound = Mathf.Clamp(MinBound, 0.25f, MaxBound);
        }

        // TODO: expose dynamicResolution capability through SystemInfo
        private bool IsDynamicResolutionSupported()
        {
#if UNITY_XBOXONE || UNITY_PS4 || UNITY_SWITCH || UNITY_IOS || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return true;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_TVOS // metal only
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal;
#elif UNITY_ANDROID
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan;
#elif UNITY_WSA
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12;
#else
            return false;
#endif
        }

        private void Start()
        {
            ++instanceCount;
            if (instanceCount > 1)
                Debug.LogWarning("Multiple Adaptive Resolution scalers created, they will interfere with each other.");
            if (!IsDynamicResolutionSupported())
                Debug.Log(string.Format("Dynamic resolution is not supported. Will be using fallback to Render Scale Multiplier"));
            scaleIncrement = (MaxBound - MinBound) / MaxLevel;
        }

        private void OnDestroy()
        {
            --instanceCount;
            if (Scale == 1.0f)
                return;

            APLog.Debug("Restoring dynamic resolution scale factor to 1.0");
            if (IsDynamicResolutionSupported())
                ScalableBufferManager.ResizeBuffers(1.0f, 1.0f);
            else
                AdaptivePerformanceRenderSettings.RenderScaleMultiplier = 1.0f;
        }

        /// <summary>
        /// Callback for any level change
        /// </summary>
        protected override void OnLevel()
        {
            float oldScaleFactor = Scale;
            Scale = scaleIncrement * (MaxLevel - CurrentLevel) + MinBound;

            // if Gfx API does not support Dynamic resolution, currentCamera.allowDynamicResolution will be false
            if (IsDynamicResolutionSupported())
            {
                if (Scale != oldScaleFactor)
                    ScalableBufferManager.ResizeBuffers(Scale, Scale);
                int rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
                int rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);
                APLog.Debug(string.Format("Adaptive Resolution Scale: {0:F3} Resolution: {1}x{2} ScaleFactor: {3:F3}x{4:F3} Level:{5}/{6}",
                    Scale,
                    rezWidth,
                    rezHeight,
                    ScalableBufferManager.widthScaleFactor,
                    ScalableBufferManager.heightScaleFactor,
                    CurrentLevel,
                    MaxLevel));
            }
            else
            {
                AdaptivePerformanceRenderSettings.RenderScaleMultiplier = Scale;
                APLog.Debug(string.Format("Dynamic resolution is not supported. Using fallback to Render Scale Multiplier : {0:F3}", Scale));
                // TODO: warn if unsupported render pipeline is used
                //Debug.Log("You might not use a supported Render Pipeline. Currently only Universal Render Pipeline and Built-in are supported by Adaptive Resolution.");
            }
        }
    }
}
