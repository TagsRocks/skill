using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using UnityEditor;
using Skill.Framework;

namespace Skill.Editor.Tools
{
    [CustomEditor(typeof(Dictionary))]
    public class DictionaryEditor : UnityEditor.Editor
    {
        private Dictionary _Data;
        void OnEnable()
        {
            _Data = (Dictionary)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Edit"))
            {
                DictionaryEditorWindow.Instance.Show();
                DictionaryEditorWindow.Instance.Dictionary = _Data;
            }
            if (GUILayout.Button("Translate"))
            {
                DictionaryTranslateWindow.Instance.Show();
                DictionaryTranslateWindow.Instance.Source = _Data;
            }
            EditorGUILayout.EndVertical();
        }
    }
}