using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a field for editing an AnimationCurve.
    /// </summary>
    public class CurveField : EditorControl
    {
        /// <summary>
        /// Optional label to display in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        private AnimationCurve _Curve;
        /// <summary>
        /// AnimationCurve - The curve edited by the user.
        /// </summary>
        public AnimationCurve Curve
        {
            get { return _Curve; }
            set { _Curve = value; }
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
            : this(new AnimationCurve())
        {
        }

        /// <summary>
        /// Create a CurveField
        /// </summary>
        /// <param name="curve">AnimationCurve to edit</param>
        public CurveField(AnimationCurve curve)
        {
            _Curve = curve;
            Label = new GUIContent();
            this.Height = 18;

        }

        /// <summary>
        /// Render CurveField
        /// </summary>
        protected override void Render()
        {
            if (_Curve != null)
            {
                if (UseColor)
                    _Curve = EditorGUI.CurveField(RenderArea, Label, _Curve, Color, Ranges);
                else
                    _Curve = EditorGUI.CurveField(RenderArea, Label, _Curve);
            }
        }
    }
}
