using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using UnityEditor;
using Skill.Framework;

namespace Skill.Editor.Tools
{
    [CustomEditor(typeof(DictionaryData))]
    public class DictionaryDataEditor : UnityEditor.Editor
    {
        private DictionaryData _Data;
        void OnEnable()
        {
            _Data = (DictionaryData)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Edit"))
            {
                DictionaryDataEditorWindow.Instance.Show();
                DictionaryDataEditorWindow.Instance.Data = _Data;
            }
            if (GUILayout.Button("Translate"))
            {
                DictionaryDataTranslateWindow.Instance.Show();
                DictionaryDataTranslateWindow.Instance.Translate = _Data;
            }
            EditorGUILayout.EndVertical();
        }
    }
}