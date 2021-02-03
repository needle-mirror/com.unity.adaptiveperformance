using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance.Editor
{
    /// <summary>
    /// This is a custom Editor base for Provider Settings. It displays provider general settings and you can use it to extend provider settings editors to display custom provider settings.
    /// </summary>
    public class ProviderSettingsEditor : UnityEditor.Editor
    {
        struct ScalerSettingInformation
        {
            public bool showScalerSettings;
        }

        const string k_Logging = "m_Logging";
        const string k_AutoPerformanceModeEnabled = "m_AutomaticPerformanceModeEnabled";
        const string k_StatsLoggingFrequencyInFrames = "m_StatsLoggingFrequencyInFrames";
        const string k_IndexerSettings = "m_IndexerSettings";
        const string k_IndexerActive = "m_Active";
        const string k_IndexerThermalActionDelay = "m_ThermalActionDelay";
        const string k_IndexerPerformanceActionDelay = "m_PerformanceActionDelay";
        const string k_ScalerSettings = "m_ScalerSettings";
        const string k_ScalerName = "m_Name";
        const string k_ScalerEnabled = "m_Enabled";
        const string k_ScalerScale = "m_Scale";
        const string k_ScalerVisualImpact = "m_VisualImpact";
        const string k_ScalerTarget = "m_Target";
        const string k_ScalerMaxLevel = "m_MaxLevel";
        const string k_ScalerMinBound = "m_MinBound";
        const string k_ScalerMaxBound = "m_MaxBound";

        static GUIContent s_LoggingLabel = EditorGUIUtility.TrTextContent(L10n.Tr("Logging"), L10n.Tr("Only active in development mode."));
        static GUIContent s_AutomaticPerformanceModeEnabledLabel = EditorGUIUtility.TrTextContent(L10n.Tr("Auto Performance Mode"), L10n.Tr("Auto Performance Mode controls performance by changing CPU and GPU levels."));
        static GUIContent s_StatsLoggingFrequencyInFramesLabel = EditorGUIUtility.TrTextContent(L10n.Tr("Logging Frequency"), L10n.Tr("Changes the logging frequency."));
        static GUIContent s_IndexerActiveLabel = EditorGUIUtility.TrTextContent(L10n.Tr("Active"), L10n.Tr("Is indexer enabled."));
        static GUIContent s_IndexerThermalActionDelayLabel = EditorGUIUtility.TrTextContent(L10n.Tr("Thermal Action Delay"), L10n.Tr("Delay after any scaler is applied or unapplied because of thermal state."));
        static GUIContent s_IndexerPerformanceActionDelayLabel = EditorGUIUtility.TrTextContent(L10n.Tr("Performance Action Delay"), L10n.Tr("Delay after any scaler is applied or unapplied because of performance state."));

        static GUIContent s_ScalerActive = EditorGUIUtility.TrTextContent(L10n.Tr("Enabled"), L10n.Tr("Is the Scaler enabled."));
        static GUIContent s_ScalerScale = EditorGUIUtility.TrTextContent(L10n.Tr("Scale"), L10n.Tr("Scale to control the quality impact for the scaler. No quality change when 1, improved quality when >1, and lowered quality when <1"));
        static GUIContent s_ScalerVisualImpact = EditorGUIUtility.TrTextContent(L10n.Tr("Visual Impact"), L10n.Tr("Visual impact the scaler has on the application. The higher the more impact the scaler has on the visuals."));
        static GUIContent s_ScalerTarget = EditorGUIUtility.TrTextContent(L10n.Tr("Target"), L10n.Tr("Target for the scaler of the application bottleneck. The target selected has the most impact on the quality control of this scaler."));
        static GUIContent s_ScalerMaxLevel = EditorGUIUtility.TrTextContent(L10n.Tr("Max Level"), L10n.Tr("Maximum level for the scaler. This is tied to the implementation of the scaler to divide the levels into concrete steps."));
        static GUIContent s_ScalerMinBound = EditorGUIUtility.TrTextContent(L10n.Tr("Min"), L10n.Tr("Minimum value for the scale boundary."));
        static GUIContent s_ScalerMaxBound = EditorGUIUtility.TrTextContent(L10n.Tr("Max"), L10n.Tr("Maximum value for the scale boundary."));
        static GUIContent s_FrameRateLimits = EditorGUIUtility.TrTextContent(L10n.Tr("Framerate"), L10n.Tr("Set the minimum and maximum framerate for the scaler to operate in."));
        static GUIContent s_BoundryLimits = EditorGUIUtility.TrTextContent(L10n.Tr("Boundary"), L10n.Tr("Set the minimum and maximum boundary for the scaler to operate in."));

        static GUIContent s_AdaptiveFramerate = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive Framerate"), L10n.Tr("Adaptive Framerate enables you to automatically control the application's framerate by the defined minimum and maximum framerate. It uses Application.targetFramerate to control the framerate for your application."));
        static GUIContent s_AdaptiveResolution = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive Resolution"), L10n.Tr("Adaptive Resolution enables you to automatically control the screen resolution of the application by the defined scale. It uses Dynamic Resolution (Vulkan only) and uses Resolution Scale of the Universal Render Pipeline as fallback if the project uses Universal Render Pipeline."));
        static GUIContent s_AdaptiveBatching = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive Batching"), L10n.Tr("Adaptive Batching toggles dynamic batching based on the thermal and performance load."));
        static GUIContent s_AdaptiveLOD = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive LOD"), L10n.Tr("Adaptive LOD changes the LOD bias based on the thermal and performance load."));
        static GUIContent s_AdaptiveLut = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive LUT"), L10n.Tr("Requires Universal Render Pipeline. Adaptive LUT changes the LUT Bias of the Universal Render Pipeline based on the thermal and performance load."));
        static GUIContent s_AdaptiveMSAA = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive MSAA"), L10n.Tr("Requires Universal Render Pipeline. Adaptive MSAA changes the Anti Aliasing Quality Bias of the Universal Render Pipeline base on the thermal and performance load."));
        static GUIContent s_AdaptiveShadowCascade = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive Shadow Cascade"), L10n.Tr("Requires Universal Render Pipeline. Adaptive Shadow Cascade changes the Main Light Shadow Cascades Count Bias of the Universal Render Pipeline base on the thermal and performance load."));
        static GUIContent s_AdaptiveShadowDistance = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive Shadow Distance"), L10n.Tr("Requires Universal Render Pipeline. Adaptive Shadow Distance changes the Max Shadow Distance Multiplier of the Universal Render Pipeline base on the thermal and performance load."));
        static GUIContent s_AdaptiveShadowmapResolution = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive Shadowmap Resolution"), L10n.Tr("Requires Universal Render Pipeline. Adaptive Shadowmap Resolution changes the  Main Light Shadowmap Resolution Multiplier of the Universal Render Pipeline base on the thermal and performance load."));
        static GUIContent s_AdaptiveShadowQuality = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive Shadow Quality"), L10n.Tr("Requires Universal Render Pipeline. Adaptive Shadow Quality changes the Shadow Quality Bias of the Universal Render Pipeline base on the thermal and performance load."));
        static GUIContent s_AdaptiveSorting = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive Sorting"), L10n.Tr("Requires Universal Render Pipeline. Adaptive Sorting skips the front-to-back sorting of the Universal Render Pipeline based on the thermal and performance load."));
        static GUIContent s_AdaptiveTransparency = EditorGUIUtility.TrTextContent(L10n.Tr("Adaptive Transparency"), L10n.Tr("Requires Universal Render Pipeline. Adaptive Transparency skips transparent objects render pass."));

        static string s_FrameRateWarning = L10n.Tr("Adaptive Framerate is only supported without VSync. Set VSync Count to \"Don't Sync\" in Quality settings.");

        SerializedProperty m_LoggingProperty;
        SerializedProperty m_AutoPerformanceModeEnabledProperty;
        SerializedProperty m_StatsLoggingFrequencyInFramesProperty;
        SerializedProperty m_IndexerActiveProperty;
        SerializedProperty m_IndexerThermalActionDelayProperty;
        SerializedProperty m_IndexerPerformanceActionDelayProperty;
        SerializedProperty m_IndexerThermalStateModeProperty;
        SerializedProperty m_IndexerThermalSafeRangeProperty;

        /// <summary>
        /// Whether the runtime settings are collapsed or not.
        /// </summary>
        public bool m_ShowRuntimeSettings = true;
        /// <summary>
        /// Whether the development settings are collapsed or not.
        /// </summary>
        public bool m_ShowDevelopmentSettings = true;
        /// <summary>
        /// Whether the indexer settings are collapsed or not.
        /// </summary>
        public bool m_ShowIndexerSettings = true;
        /// <summary>
        /// Whether the scaler settings are collapsed or not.
        /// </summary>
        public bool m_ShowScalerSettings = true;

        static GUIContent k_ShowRuntimeSettings = EditorGUIUtility.TrTextContent(L10n.Tr("Runtime Settings"));
        static GUIContent k_ShowDevelopmentSettings = EditorGUIUtility.TrTextContent(L10n.Tr("Development Settings"));
        static GUIContent k_ShowIndexerSettings = EditorGUIUtility.TrTextContent(L10n.Tr("Indexer Settings"));
        static GUIContent k_ShowScalerSettings = EditorGUIUtility.TrTextContent(L10n.Tr("Scaler Settings"));

        Dictionary<string, ScalerSettingInformation> m_Scalers = new Dictionary<string, ScalerSettingInformation>();

        bool m_PreviousHierarchyMode;

        /// <summary>
        /// Starts the display block of the base settings. Needs to be called if DisplayBaseRuntimeSettings() or DisplayBaseDeveloperSettings() gets called. Needs to be concluded by a call to DisplayBaseSettingsEnd().
        /// </summary>
        /// <returns>
        /// False if the settings cannot be loaded. Otherwise true.
        /// </returns>
        public bool DisplayBaseSettingsBegin()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return false;

            serializedObject.Update();

            m_PreviousHierarchyMode = EditorGUIUtility.hierarchyMode;
            EditorGUIUtility.hierarchyMode = false;

            if (m_LoggingProperty == null)
                m_LoggingProperty = serializedObject.FindProperty(k_Logging);
            if (m_AutoPerformanceModeEnabledProperty == null)
                m_AutoPerformanceModeEnabledProperty = serializedObject.FindProperty(k_AutoPerformanceModeEnabled);
            if (m_StatsLoggingFrequencyInFramesProperty == null)
                m_StatsLoggingFrequencyInFramesProperty = serializedObject.FindProperty(k_StatsLoggingFrequencyInFrames);
            var indexerSettings = serializedObject.FindProperty(k_IndexerSettings);
            Debug.Assert(indexerSettings != null);
            if (m_IndexerActiveProperty == null)
                m_IndexerActiveProperty = indexerSettings.FindPropertyRelative(k_IndexerActive);
            if (m_IndexerThermalActionDelayProperty == null)
                m_IndexerThermalActionDelayProperty = indexerSettings.FindPropertyRelative(k_IndexerThermalActionDelay);
            if (m_IndexerPerformanceActionDelayProperty == null)
                m_IndexerPerformanceActionDelayProperty = indexerSettings.FindPropertyRelative(k_IndexerPerformanceActionDelay);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("Adaptive Performance settings cannot be changed when the Editor is in Play mode.", MessageType.Info);
                EditorGUILayout.Space();
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            return true;
        }

        /// <summary>
        /// Ends the display block of the base settings. Needs to be called if DisplayBaseSettingsBegin() is called.
        /// </summary>
        public void DisplayBaseSettingsEnd()
        {
            EditorGUILayout.EndBuildTargetSelectionGrouping(); // Start happens in provider Editor
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            EditorGUIUtility.hierarchyMode = m_PreviousHierarchyMode;

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Displays the base runtime settings. Requires DisplayBaseSettingsBegin() to be called before and DisplayBaseSettingsEnd() after as serialization is not taken care of.
        /// </summary>
        public void DisplayBaseRuntimeSettings()
        {
            m_ShowRuntimeSettings = EditorGUILayout.Foldout(m_ShowRuntimeSettings, k_ShowRuntimeSettings, true);
            if (m_ShowRuntimeSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_AutoPerformanceModeEnabledProperty, s_AutomaticPerformanceModeEnabledLabel);
                DisplayBaseIndexerSettings();
                DisplayScalerSettings();
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Displays the base indexer settings. Requires the serializedObject to be updated before and applied after as serialization is not taken care of.
        /// </summary>
        public void DisplayBaseIndexerSettings()
        {
            m_ShowIndexerSettings = EditorGUILayout.Foldout(m_ShowIndexerSettings, k_ShowIndexerSettings, true);
            if (m_ShowIndexerSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_IndexerActiveProperty, s_IndexerActiveLabel);
                GUI.enabled = m_IndexerActiveProperty.boolValue && !EditorApplication.isPlayingOrWillChangePlaymode;
                EditorGUILayout.PropertyField(m_IndexerThermalActionDelayProperty, s_IndexerThermalActionDelayLabel, GUILayout.MaxWidth(EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth));
                EditorGUILayout.PropertyField(m_IndexerPerformanceActionDelayProperty, s_IndexerPerformanceActionDelayLabel, GUILayout.MaxWidth(EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth));
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Displays the base scaler settings. Requires the serializedObject to be updated before and applied after as serialization is not taken care of.
        /// </summary>
        public void DisplayScalerSettings()
        {
            GUI.enabled = m_IndexerActiveProperty.boolValue && !EditorApplication.isPlayingOrWillChangePlaymode;
            EditorGUILayout.BeginHorizontal();
            m_ShowScalerSettings = EditorGUILayout.Foldout(m_ShowScalerSettings, k_ShowScalerSettings, true);
            if (m_ShowScalerSettings)
            {
                EditorGUILayout.LabelField(s_ScalerActive, EditorStyles.boldLabel, GUILayout.MaxWidth(70));
                EditorGUILayout.LabelField(s_ScalerVisualImpact, EditorStyles.boldLabel, GUILayout.MaxWidth(100));
                EditorGUILayout.LabelField(s_ScalerTarget, EditorStyles.boldLabel, GUILayout.MaxWidth(200));
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                if (!m_IndexerActiveProperty.boolValue)
                {
                    EditorGUILayout.HelpBox("You have to enable Adaptive Performance Indexer to use Scaler.", MessageType.Info);
                    EditorGUILayout.Space();
                }
                var scalerSettings = serializedObject.FindProperty(k_ScalerSettings);
                Debug.Assert(scalerSettings != null);

                AdaptivePerformanceScalerSettings settingsObject = new AdaptivePerformanceScalerSettings();
                MemberInfo[] memberInfo;
                Type settingsType = settingsObject.GetType();
                memberInfo = settingsType.GetProperties();

                for (int i = 0; i < memberInfo.Length; i++)
                {
                    var scalerSetting = scalerSettings.FindPropertyRelative($"m_{memberInfo[i].Name}");
                    DrawScalerSetting(scalerSetting, m_IndexerActiveProperty.boolValue && !EditorApplication.isPlayingOrWillChangePlaymode);
                }

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.EndHorizontal();
            }
            GUI.enabled = true;
        }

        void DrawScalerSetting(SerializedProperty scalerSetting, bool renderNotDisabled)
        {
            string name = scalerSetting.FindPropertyRelative(k_ScalerName).stringValue;
            var isEnabled = renderNotDisabled && !EditorApplication.isPlayingOrWillChangePlaymode;
            var isFrameRateScaler = name == "Adaptive Framerate";
            var isResolutionScaler = name == "Adaptive Resolution";
            var minBound = scalerSetting.FindPropertyRelative(k_ScalerMinBound).floatValue;
            var maxBound = scalerSetting.FindPropertyRelative(k_ScalerMaxBound).floatValue;
            var needsFoldout = minBound != -1 || maxBound != -1 || scalerSetting.FindPropertyRelative(k_ScalerScale).floatValue != -1;
            if (isFrameRateScaler && QualitySettings.vSyncCount > 0)
            {
                isEnabled = false;
            }

            GUI.enabled = isEnabled;

            ScalerSettingInformation scalerSettingInfo;
            if (!m_Scalers.TryGetValue(name, out scalerSettingInfo))
            {
                scalerSettingInfo = new ScalerSettingInformation()
                {
                    showScalerSettings = false
                };
            }

            EditorGUILayout.BeginHorizontal();
            var scalerName = ReturnScalerGUIContent(name);
            if (needsFoldout)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(200));
                scalerSettingInfo.showScalerSettings = EditorGUILayout.Foldout(scalerSettingInfo.showScalerSettings, scalerName, true);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.LabelField(scalerName);


            scalerSetting.FindPropertyRelative(k_ScalerEnabled).boolValue = EditorGUILayout.Toggle("", scalerSetting.FindPropertyRelative(k_ScalerEnabled).boolValue, GUILayout.MaxWidth(70));
            GUI.enabled = scalerSetting.FindPropertyRelative(k_ScalerEnabled).boolValue && isEnabled;
            EditorGUILayout.PropertyField(scalerSetting.FindPropertyRelative(k_ScalerVisualImpact), GUIContent.none, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));

            ScalerTarget staticFlagMask = (ScalerTarget)scalerSetting.FindPropertyRelative(k_ScalerTarget).intValue;
            var propDisplayNames = "";
            foreach (var enumValue in System.Enum.GetValues(typeof(ScalerTarget)))
            {
                int checkBit = (int)staticFlagMask & (int)enumValue;
                if (checkBit != 0)
                {
                    propDisplayNames += propDisplayNames.Length != 0 ? " | " : "";
                    propDisplayNames += enumValue.ToString();
                }
            }
            EditorGUILayout.LabelField(propDisplayNames, GUILayout.MaxWidth(200));
            if (scalerSettingInfo.showScalerSettings && needsFoldout)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel++;

                if (isFrameRateScaler && QualitySettings.vSyncCount > 0)
                {
                    EditorGUILayout.HelpBox(s_FrameRateWarning, MessageType.Warning, true);
                }

                if (scalerSetting.FindPropertyRelative(k_ScalerScale).floatValue != -1)
                    EditorGUILayout.PropertyField(scalerSetting.FindPropertyRelative(k_ScalerScale), s_ScalerScale, GUILayout.MaxWidth(EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth));

                if (isResolutionScaler)
                    EditorGUILayout.PropertyField(scalerSetting.FindPropertyRelative(k_ScalerMaxLevel), s_ScalerMaxLevel, GUILayout.MaxWidth(EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth));

                if (minBound != -1 && maxBound != -1)
                {
                    EditorGUILayout.LabelField(isFrameRateScaler ? s_FrameRateLimits : s_BoundryLimits, EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField(s_ScalerMinBound, GUILayout.MaxWidth(100));
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(scalerSetting.FindPropertyRelative(k_ScalerMinBound), GUIContent.none, GUILayout.MaxWidth(100));
                        if (EditorGUI.EndChangeCheck())
                        {
                            RangeCheckProperty(k_ScalerMinBound, scalerSetting.FindPropertyRelative(k_ScalerMinBound).floatValue, isFrameRateScaler, scalerSetting, true, maxBound);
                            minBound = scalerSetting.FindPropertyRelative(k_ScalerMinBound).floatValue;
                        }

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.MinMaxSlider(ref minBound, ref maxBound, isFrameRateScaler ? 15 : 0f, isFrameRateScaler ? 140 : 1f);
                        if (EditorGUI.EndChangeCheck())
                        {
                            RangeCheckProperty(k_ScalerMinBound, minBound, isFrameRateScaler, scalerSetting, true, maxBound);
                            RangeCheckProperty(k_ScalerMaxBound, maxBound, isFrameRateScaler, scalerSetting, false, minBound);
                        }

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(scalerSetting.FindPropertyRelative(k_ScalerMaxBound), GUIContent.none, GUILayout.MaxWidth(100));
                        if (EditorGUI.EndChangeCheck())
                        {
                            RangeCheckProperty(k_ScalerMaxBound, scalerSetting.FindPropertyRelative(k_ScalerMaxBound).floatValue, isFrameRateScaler, scalerSetting, false, minBound);
                            maxBound = scalerSetting.FindPropertyRelative(k_ScalerMaxBound).floatValue;
                        }
                        EditorGUILayout.LabelField(s_ScalerMaxBound, GUILayout.MaxWidth(100));
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.EndHorizontal();
            }

            GUI.enabled = true;
            m_Scalers[name] = scalerSettingInfo;
        }

        void RangeCheckProperty(string property, float value, bool frameRateScaler, SerializedProperty sproperty, bool isMin, float referenceValue)
        {
            if (isMin && value > referenceValue || !isMin && value < referenceValue)
                value = referenceValue;

            sproperty.FindPropertyRelative(property).floatValue = frameRateScaler ? Mathf.Clamp(Mathf.RoundToInt(value), 15, 140) : Mathf.Clamp((float)Math.Round(value, 2), 0, 1);
        }

        GUIContent ReturnScalerGUIContent(string scalerName)
        {
            switch (scalerName)
            {
                case "Adaptive Framerate":
                    return s_AdaptiveFramerate;
                case "Adaptive Resolution":
                    return s_AdaptiveResolution;
                case "Adaptive Batching":
                    return s_AdaptiveBatching;
                case "Adaptive LOD":
                    return s_AdaptiveLOD;
                case "Adaptive Lut":
                    return s_AdaptiveLut;
                case "Adaptive MSAA":
                    return s_AdaptiveMSAA;
                case "Adaptive Shadow Cascade":
                    return s_AdaptiveShadowCascade;
                case "Adaptive Shadow Distance":
                    return s_AdaptiveShadowDistance;
                case "Adaptive Shadowmap Resolution":
                    return s_AdaptiveShadowmapResolution;
                case "Adaptive Shadow Quality":
                    return s_AdaptiveShadowQuality;
                case "Adaptive Sorting":
                    return s_AdaptiveSorting;
                case "Adaptive Transparency":
                    return s_AdaptiveTransparency;
                default:
                    return new GUIContent("");
            }
        }

        /// <summary>
        /// Displays the base developer settings. Requires DisplayBaseSettingsBegin() to be called before and DisplayBaseSettingsEnd() after as serialization is not taken care of.
        /// </summary>
        public void DisplayBaseDeveloperSettings()
        {
            GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;
            m_ShowDevelopmentSettings = EditorGUILayout.Foldout(m_ShowDevelopmentSettings, k_ShowDevelopmentSettings, true);
            if (m_ShowDevelopmentSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_LoggingProperty, s_LoggingLabel);
                EditorGUILayout.PropertyField(m_StatsLoggingFrequencyInFramesProperty, s_StatsLoggingFrequencyInFramesLabel, GUILayout.MaxWidth(EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth));
                EditorGUI.indentLevel--;
            }
            GUI.enabled = true;
        }
    }
}
