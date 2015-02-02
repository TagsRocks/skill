using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor.UI
{
    /// <summary>
    /// helper methods to use when draw ui in inspector for custom object editors
    /// </summary>
    public static class InspectorHelper
    {
        private static GUIStyle _BoxStyle;
        private static GUIContent _BoxContent;
        private const float FrameMargin = 2;

        /// <summary>
        /// Draw frame in Inspector
        /// </summary>
        /// <param name="frame">Frame to draw</param>
        /// <param name="heigth">Height of frame</param>
        public static void OnInspectorGUI(this Skill.Framework.UI.Frame frame, float heigth)
        {
            if (_BoxStyle == null)
            {
                _BoxStyle = new GUIStyle();                
                _BoxContent = new GUIContent();
            }

            // make space for frame
            Rect framePosition = EditorGUILayout.BeginVertical();
            framePosition.x += FrameMargin;
            framePosition.width -= FrameMargin * 2;
            GUILayout.Box(_BoxContent, _BoxStyle, GUILayout.Height(heigth));
            EditorGUILayout.EndVertical();

            frame.Position = framePosition;
            frame.OnGUI();
        }        
    }
}
