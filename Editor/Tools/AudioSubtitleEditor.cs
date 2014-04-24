using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Skill.Editor.UI.Extended;
using Skill.Framework.Audio;

namespace Skill.Editor.Tools
{
    [CustomEditor(typeof(AudioSubtitle))]
    public class AudioSubtitleEditor : UnityEditor.Editor
    {
        private AudioSubtitle _AudioSubtitle;
        void OnEnable()
        {
            _AudioSubtitle = (AudioSubtitle)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Edit"))
            {
                AudioSubtitleEditorWindow.Instance.Show();
                AudioSubtitleEditorWindow.Instance.Subtitle = _AudioSubtitle;
            }
            EditorGUILayout.EndVertical();
        }
    }
}