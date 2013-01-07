﻿using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a label with a foldout arrow to the left of it.
    /// </summary>
    public class Foldout : EditorControl
    {
        /// <summary>
        /// The label to show.
        /// </summary>
        public GUIContent Content { get; private set; }

        /// <summary>
        /// Toggle on label click
        /// </summary>
        public bool ToggleOnLabelClick { get; set; }

        /// <summary>
        /// Occurs when state of Foldout changed
        /// </summary>
        public event EventHandler StateChanged;
        /// <summary>
        /// when state of Foldout changed
        /// </summary>
        protected virtual void OnStateChanged()
        {
            if (StateChanged != null) StateChanged(this, EventArgs.Empty);
        }

        private bool _FoldoutState;
        /// <summary>
        /// boolean - The foldout state selected by the user.
        /// </summary>
        public bool FoldoutState
        {
            get { return _FoldoutState; }
            set
            {
                if (_FoldoutState != value)
                {
                    _FoldoutState = value;
                    OnStateChanged();
                }
            }
        }

        /// <summary>
        /// Create a Foldout
        /// </summary>
        public Foldout()
        {
            Content = new GUIContent();
            this.Height = 16;
            this.ToggleOnLabelClick = true;
        }

        /// <summary>
        /// Render Foldout
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                FoldoutState = EditorGUI.Foldout(RenderArea, _FoldoutState, Content, ToggleOnLabelClick, Style);
            }
            else
            {
                FoldoutState = EditorGUI.Foldout(RenderArea, _FoldoutState, Content, ToggleOnLabelClick);
            }
        }
    }
}
