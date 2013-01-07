using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make an inspector-window-like titlebar.
    /// </summary>
    public class InspectorTitlebar : EditorControl
    {
        /// <summary>
        /// Occurs when state of Foldout of InspectorTitlebar changed
        /// </summary>
        public event EventHandler FoldoutStateChanged;
        /// <summary>
        /// when state of Foldout of InspectorTitlebar changed
        /// </summary>
        protected virtual void OnFoldoutStateChanged()
        {
            if (FoldoutStateChanged != null) FoldoutStateChanged(this, EventArgs.Empty);
        }

        private bool _FoldoutState;
        /// <summary>
        /// boolean - The foldout state selected by the user. If true, you should render sub-objects.
        /// </summary>
        public bool FoldoutState
        {
            get { return _FoldoutState; }
            set
            {
                if (_FoldoutState != value)
                {
                    _FoldoutState = value;
                    OnFoldoutStateChanged();
                }
            }
        }

        /// <summary>
        /// The object (for example a component) or objects that the titlebar is for.
        /// </summary>
        public UnityEngine.Object Object { get; set; }

        /// <summary>
        /// The object (for example a component) or objects that the titlebar is for.
        /// </summary>
        public UnityEngine.Object[] Objects { get; set; }

        /// <summary>
        /// Create an InspectorTitlebar
        /// </summary>
        public InspectorTitlebar()
        {
            this.Height = 16;
        }

        /// <summary>
        /// Render InspectorTitlebar
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);

            if (Objects != null && Objects.Length > 0)
                FoldoutState = EditorGUI.InspectorTitlebar(RenderArea, _FoldoutState, Objects);
            else //if (Object != null)
                FoldoutState = EditorGUI.InspectorTitlebar(RenderArea, _FoldoutState, Object);
            //else
            //    Debug.LogError("Invalid object for InspectorTitlebar");
        }
    }
}
