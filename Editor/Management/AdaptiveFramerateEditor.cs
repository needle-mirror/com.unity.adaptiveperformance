using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance.Editor
{
    /// <summary>
    /// This is custom Editor for Adaptive Framerate Settings.
    /// </summary>
    [CustomEditor(typeof(AdaptiveFramerate), true)]
    public class AdaptiveFramerateEditor : UnityEditor.Editor
    {
        SerializedProperty m_Minimum;
        SerializedProperty m_Maximum;
        float minVal = 15;
        float maxVal = 240;

        private void OnEnable()
        {
            m_Minimum = serializedObject.FindProperty("minimumFPS");
            minVal = m_Minimum.intValue;
            m_Maximum = serializedObject.FindProperty("maximumFPS");
            maxVal = m_Maximum.intValue;
        }

        /// <summary>
        /// Override of Editor callback to display custom settings.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField($"Minimum Framerate: {Mathf.RoundToInt(minVal)}");
            EditorGUILayout.LabelField($"Maximum Framerate: {Mathf.RoundToInt(maxVal)}");
            EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, 15, 240);

            m_Minimum.intValue = Mathf.RoundToInt(minVal);
            m_Maximum.intValue = Mathf.RoundToInt(maxVal);

            serializedObject.ApplyModifiedProperties();

            if (QualitySettings.vSyncCount > 0)
            {
                EditorGUILayout.HelpBox(
                    "Adaptive Framerate is only supported without VSync. Set VSync Count to \"Don't Sync\" in Quality settings.",
                    MessageType.Warning, true);
            }
        }
    }
}
