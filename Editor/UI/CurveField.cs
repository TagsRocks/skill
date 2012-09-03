using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a field for editing an AnimationCurve.
    /// </summary>
    public class CurveField : Control
    {
        /// <summary>
        /// Optional label to display in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when Curve of CurveField changed
        /// </summary>
        public event EventHandler CurveChanged;
        protected virtual void OnCurveChanged()
        {
            if (CurveChanged != null) CurveChanged(this, EventArgs.Empty);
        }

        private AnimationCurve _Curve;
        /// <summary>
        /// AnimationCurve - The curve edited by the user.
        /// </summary>
        public AnimationCurve Curve
        {
            get { return _Curve; }
            set
            {
                if (_Curve != value)
                {
                    _Curve = value;
                    OnCurveChanged();
                }
            }
        }

        /// <summary>
        /// The color to show the curve with.
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// Optional rectangle that the curve is restrained within.
        /// </summary>
        public Rect Ranges { get; set; }

        /// <summary>
        /// Use color?
        /// </summary>
        public bool UseColor { get; set; }

        /// <summary>
        /// Create a CurveField
        /// </summary>
        public CurveField()
        {
            Label = new GUIContent();
        }

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (UseColor)
                Curve = EditorGUI.CurveField(PaintArea, Label, _Curve, Color, Ranges);
            else
                Curve = EditorGUI.CurveField(PaintArea, Label, _Curve);
        }
    }
}
