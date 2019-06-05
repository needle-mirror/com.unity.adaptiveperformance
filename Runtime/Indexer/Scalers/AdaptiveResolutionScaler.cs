namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the resolution of the main camera.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class AdaptiveResolutionScaler : AdaptivePerformanceScaler
    {
        /// <summary>
        /// Returns visual impact of scaler when applied.
        /// </summary>
        public override ScalerVisualImpact VisualImpact => ScalerVisualImpact.Low;
        /// <summary>
        /// Returns what bottlenecks this scaler targets.
        /// </summary>
        public override ScalerTarget Target => ScalerTarget.FillRate | ScalerTarget.GPU;
        /// <summary>
        /// Returns max level of scaler.
        /// </summary>
        public override int MaxLevel => scaleLevels - 1;

        Camera currentCamera;

        /// <summary>
        /// The maximum amount the resolutions width can be scaled by. Must be less than or equal to 1.0.
        /// </summary>
        public float maxResolutionWidthScale = 1.0f;
        /// <summary>
        /// The maximum amount the resolutions height can be scaled by. Must be less than or equal to 1.0.
        /// </summary>
        public float maxResolutionHeightScale = 1.0f;
        /// <summary>
        /// The minimum amount the resolutions width can be scaled by. Must be greater than 0.0.
        /// </summary>
        public float minResolutionWidthScale = 0.4f;
        /// <summary>
        /// The minimum amount the resolutions height can be scaled by. Must be greater than 0.0.
        /// </summary>
        public float minResolutionHeightScale = 0.4f;
        float scaleWidthIncrement = 1.0f;
        float scaleHeightIncrement = 1.0f;
        /// <summary>
        /// Defines how many steps will be used when scaling between the minimum and maximum.
        /// </summary>
        public int scaleLevels = 4;

        float widthScale = 1.0f;
        float heightScale = 1.0f;

        private void Start()
        {
            scaleWidthIncrement = (maxResolutionWidthScale - minResolutionWidthScale) / scaleLevels;
            scaleHeightIncrement = (maxResolutionHeightScale - minResolutionHeightScale) / scaleLevels;
            currentCamera = GetComponent<Camera>();
            APLog.Debug(string.Format("Allow Dynamic Resolution for Adaptive Resolution: {0}", currentCamera.allowDynamicResolution));
            if (currentCamera.allowDynamicResolution)
            {
                int rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
                int rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);
                APLog.Debug(string.Format("Adaptive Resolution Scale: {0:F3}x{1:F3}\nResolution: {2}x{3}\n",
                    widthScale,
                    heightScale,
                    rezWidth,
                    rezHeight));
            }
            else
            {
                // TODO: Renderscale
            }
        }

        /// <summary>
        /// Callback for any level change
        /// </summary>
        protected override void OnLevel()
        {
            float oldWidthScale = widthScale;
            float oldHeightScale = heightScale;

            widthScale = scaleWidthIncrement * (scaleLevels - CurrentLevel) + minResolutionWidthScale;
            heightScale = scaleHeightIncrement * (scaleLevels - CurrentLevel) + minResolutionHeightScale;

            if (currentCamera.allowDynamicResolution)
            {
                if (widthScale != oldWidthScale || heightScale != oldHeightScale)
                {
                    ScalableBufferManager.ResizeBuffers(widthScale, heightScale);
                }
                // TODO: DetermineResolution();
                int rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
                int rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);
                APLog.Debug(string.Format("Scale: {0:F3}x{1:F3}\nResolution: {2}x{3}\nScaleFactor: {4:F3}x{5:F3}",
                    widthScale,
                    heightScale,
                    rezWidth,
                    rezHeight,
                    ScalableBufferManager.widthScaleFactor,
                    ScalableBufferManager.heightScaleFactor));
            }
            else
            {
                APLog.Debug("Ensure 'Allow Dynamic Resolution' is enabled on the camera.");
#if UNIVERSAL_RENDER_PIPELINE
                AdaptivePerformanceRenderSettings.RenderScaleMultiplier = widthScale;
                APLog.Debug(string.Format("Render Scale Multiplier : {0:F3}", widthScale));
#else
                APLog.Debug("You might not use a supported Render Pipeline. Currently only Universal Render Pipeline and Built-in are supported by Adaptive Resolution.");
#endif
            }
        }
    }
}
