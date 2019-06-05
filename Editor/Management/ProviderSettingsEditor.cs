using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance.Editor
{
    /// <summary>
    /// This is a custom Editor base for Provider Settings. It displays provider general settings and you can use it to extend provider settings editors to display custom provider settings.
    /// </summary>
    public class ProviderSettingsEditor : UnityEditor.Editor
    {
        const string k_Logging = "m_Logging";
        const string k_AutoPerformanceModeEnabled = "m_AutomaticPerformanceModeEnabled";
        const string k_StatsLoggingFrequencyInFrames = "m_StatsLoggingFrequencyInFrames";
        const string k_IndexerSettings = "m_IndexerSettings";
        const string k_IndexerActive = "m_Active";
        const string k_IndexerThermalActionDelay = "m_ThermalActionDelay";
        const string k_IndexerPerformanceActionDelay = "m_PerformanceActionDelay";
        const string k_IndexerThermalStateMode = "m_ThermalStateMode";
        const string k_IndexerThermalSafeRange = "m_ThermalSafeRange";

        static GUIContent s_LoggingLabel = EditorGUIUtility.TrTextContent("Logging", "Only active in development mode.");
        static GUIContent s_AutomaticPerformanceModeEnabledLabel = EditorGUIUtility.TrTextContent("Auto Performance Mode", "Auto Performance Mode controls performance by changing CPU and GPU levels.");
        static GUIContent s_StatsLoggingFrequencyInFramesLabel = EditorGUIUtility.TrTextContent("Logging Frequency", "Changes the logging frequency.");
        static GUIContent s_IndexerActiveLabel = EditorGUIUtility.TrTextContent("Active", "Is indexer enabled.");
        static GUIContent s_IndexerThermalActionDelayLabel = EditorGUIUtility.TrTextContent("Thermal Action Delay", "Delay after any scaler is applied or unapplied, because of thermal state.");
        static GUIContent s_IndexerPerformanceActionDelayLabel = EditorGUIUtility.TrTextContent("Performance Action Delay", "Delay after any scaler is applied or unapplied, because of performance state.");
        static GUIContent s_IndexerThermalStateModeLabel = EditorGUIUtility.TrTextContent("Thermal State Mode", "Thermal state mode used by indexer.");
        static GUIContent s_IndexerThermalSafeRangeLabel = EditorGUIUtility.TrTextContent("Thermal Safe Range", "Thermal level range that indexer will target.");

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

        static GUIContent k_ShowRuntimeSettings = new GUIContent("Runtime Settings");
        static GUIContent k_ShowDevelopmentSettings = new GUIContent("Development Settings");
        static GUIContent k_ShowIndexerSettings = new GUIContent("Indexer Settings");

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
            if (m_IndexerThermalStateModeProperty == null)
                m_IndexerThermalStateModeProperty = indexerSettings.FindPropertyRelative(k_IndexerThermalStateMode);
            if (m_IndexerThermalSafeRangeProperty == null)
                m_IndexerThermalSafeRangeProperty = indexerSettings.FindPropertyRelative(k_IndexerThermalSafeRange);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("Adaptive Performance settings cannot be changed when the editor is in play mode.", MessageType.Info);
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
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndBuildTargetSelectionGrouping();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Displays the base runtime settings. Requires DisplayBaseSettingsBegin() to be called before and DisplayBaseSettingsEnd() after as serialization is not taken care of.
        /// </summary>
        public void DisplayBaseRuntimeSettings()
        {
            m_ShowRuntimeSettings = EditorGUILayout.Foldout(m_ShowRuntimeSettings, k_ShowRuntimeSettings);
            if (m_ShowRuntimeSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_AutoPerformanceModeEnabledProperty, s_AutomaticPerformanceModeEnabledLabel);
                DisplayBaseIndexerSettings();
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Displays the base indexer settings. Requires the serializedObject to be updated before and applied after as serialization is not taken care of.
        /// </summary>
        public void DisplayBaseIndexerSettings()
        {
            m_ShowIndexerSettings = EditorGUILayout.Foldout(m_ShowIndexerSettings, k_ShowIndexerSettings);
            if (m_ShowIndexerSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_IndexerActiveProperty, s_IndexerActiveLabel);
                EditorGUILayout.PropertyField(m_IndexerThermalActionDelayProperty, s_IndexerThermalActionDelayLabel);
                EditorGUILayout.PropertyField(m_IndexerPerformanceActionDelayProperty, s_IndexerPerformanceActionDelayLabel);
                EditorGUILayout.PropertyField(m_IndexerThermalStateModeProperty, s_IndexerThermalStateModeLabel);
                if ((ThermalStateMode)m_IndexerThermalStateModeProperty.enumValueIndex == ThermalStateMode.TemperatureLevelBased)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_IndexerThermalSafeRangeProperty, s_IndexerThermalSafeRangeLabel);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Displays the base developer settings. Requires DisplayBaseSettingsBegin() to be called before and DisplayBaseSettingsEnd() after as serialization is not taken care of.
        /// </summary>
        public void DisplayBaseDeveloperSettings()
        {
            m_ShowDevelopmentSettings = EditorGUILayout.Foldout(m_ShowDevelopmentSettings, k_ShowDevelopmentSettings);
            if (m_ShowDevelopmentSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_LoggingProperty, s_LoggingLabel);
                EditorGUILayout.PropertyField(m_StatsLoggingFrequencyInFramesProperty, s_StatsLoggingFrequencyInFramesLabel);
                EditorGUI.indentLevel--;
            }
        }
    }
}
