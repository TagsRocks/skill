using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Check if any control was changed inside a block of code.
    /// </summary>
    public class ChangeCheck : Skill.UI.Canvas
    {
        private bool _IsChanged;
        /// <summary>
        /// true if GUI.changed was set to true inside the block, otherwise false
        /// </summary>
        public bool IsChanged
        {
            get
            {
                return _IsChanged;
            }
            set
            {
                _IsChanged = value;
                if (_IsChanged)
                    OnChanged();
            }
        }

        /// <summary>
        /// occurs when any control was changed inside a block of code.
        /// </summary>
        public event EventHandler Changed;
        protected virtual void OnChanged()
        {
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        protected override void PaintControls()
        {
            EditorGUI.BeginChangeCheck();
            base.PaintControls();
            IsChanged = EditorGUI.EndChangeCheck();
        }        
    }
}
