using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a layer selection field.
    /// </summary>
    public class LayerField : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }      

        /// <summary>
        /// Occurs when Layer of LayerField changed
        /// </summary>
        public event EventHandler LayerChanged;
        protected virtual void OnLayerChanged()
        {
            if (LayerChanged != null) LayerChanged(this, EventArgs.Empty);
        }

        private int _Layer;
        /// <summary>
        /// int - The layer selected by the user.
        /// </summary>
        public int Layer
        {
            get { return _Layer; }
            set
            {
                if (_Layer != value)
                {
                    _Layer = value;
                    OnLayerChanged();
                }
            }
        }

        /// <summary>
        /// Create an instance of LayerField
        /// </summary>
        public LayerField()
        {
            Label = new GUIContent();
            this.Height = 16;
        }

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                Layer = EditorGUI.LayerField(PaintArea, Label, _Layer, Style);
            }
            else
            {
                Layer = EditorGUI.LayerField(PaintArea, Label, _Layer);
            }
        }        
    }
}
