using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text;

namespace Skill.Editor
{
    public class AboutSkill : UnityEditor.EditorWindow
    {
        private static Vector2 Size = new Vector2(500,400);        

        private static AboutSkill _Instance;

        private GUIStyle _TextLableStyle;
        private GUIStyle _CloseButonStyle;

        public void OnGUI()
        {
            if (_TextLableStyle == null)
                CreateStyles();

            Rect r = EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(Skill.Editor.Properties.Resources.AppDescription, _TextLableStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();

            
            if (GUILayout.Button("Close", _CloseButonStyle, GUILayout.ExpandWidth(false)))
                this.Close();
        }

        private void CreateStyles()
        {
            _TextLableStyle = new GUIStyle()
            {                
                margin= new RectOffset(5,5,5,5),
                 stretchHeight = true,
                  wordWrap = true
            };

            _CloseButonStyle = new GUIStyle(GUI.skin.button)
            {
                margin = new RectOffset(5, 5, 100, 5)
            };
        }

        public AboutSkill()
        {
            hideFlags = HideFlags.DontSave;

            if (_Instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton. Self destruction in 3...");
                Destroy(this);
                return;
            }

            _Instance = this;

            title = "About Skill";
            position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);            
        }

        public void OnDestroy()
        {
            _Instance = null;
        }


        public static AboutSkill Instance
        {
            get
            {
                if (_Instance == null)
                {
                    EditorWindow.GetWindow<AboutSkill>();
                }
                return _Instance;
            }
        }
    }
}
