using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    public class MultiFloatField : EditorControl
    {
        private GUIContent[] _SubLabels;
        private float[] _Values;
        private char[] _Chars;

        public GUIContent Label { get; private set; }
        public bool UseLable { get; set; }

        /// <summary>
        /// Create an instance of MultiFloatField
        /// </summary>
        /// <param name="floatCount">NUmber of floats</param>
        public MultiFloatField(int floatCount)
        {
            this.Label = new GUIContent();
            this._SubLabels = new GUIContent[floatCount];
            for (int i = 0; i < floatCount; i++)
                this._SubLabels[i] = new GUIContent();
            this._Values = new float[floatCount];
            this._Chars = new char[floatCount];
        }

        public float GetValue(int index) { return _Values[index]; }
        public void SetValue(int index, float value) { _Values[index] = value; }

        public char GetLabel(int index) { return _Chars[index]; }
        public void SetLabel(int index, char label) { _Chars[index] = label; _SubLabels[index].text = new string(label, 1); }

        /// <summary>
        /// Render MultiFloatField
        /// </summary>
        protected override void Render()
        {
            if (UseLable)
                EditorGUI.MultiFloatField(RenderArea, Label, _SubLabels, _Values);
            else
                EditorGUI.MultiFloatField(RenderArea, _SubLabels, _Values);
        }
    }
}
