using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Skill.Framework.UI;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Check if any control was changed inside a block of code.
    /// </summary>
    public class ChangeCheck : Canvas
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
        /// <summary>
        /// when any control was changed inside a block of code.
        /// </summary>
        protected virtual void OnChanged()
        {
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create new instance of ChangeCheck
        /// </summary>
        public ChangeCheck()            
        {

        }

        /// <summary> Begin Render control's content </summary>
        protected override void BeginRender()
        {
            base.BeginRender();
            EditorGUI.BeginChangeCheck();
        }
        /// <summary> End Render control's content </summary>
        protected override void EndRender()
        {
            IsChanged = EditorGUI.EndChangeCheck();
            base.EndRender();            
        }
    }
}
