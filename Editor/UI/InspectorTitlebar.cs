using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make an inspector-window-like titlebar.
    /// </summary>
    public class InspectorTitlebar : Control
    {
        /// <summary>
        /// Occurs when state of Foldout of InspectorTitlebar changed
        /// </summary>
        public event EventHandler FoldoutStateChanged;
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

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);

            if (Objects != null && Objects.Length > 0)
                FoldoutState = EditorGUI.InspectorTitlebar(PaintArea, _FoldoutState, Objects);
            else if (Objects != null)
                FoldoutState = EditorGUI.InspectorTitlebar(PaintArea, _FoldoutState, Object);
            else
                Debug.LogError("Invalid object for InspectorTitlebar");
        }
    }
}
