﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Create a group of controls that can be disabled.
    /// </summary>
    /// <remarks>
    /// The group cannot be used to enable controls that would otherwise be disabled to begin with.
    /// The groups can be nested and the controls within a child group will be disabled both if that child group is itself disabled or if a parent group is.
    /// </remarks>
    public class DisabledGroup : Skill.UI.Canvas
    {
        /// <summary>
        /// Boolean specifying if the controls inside the group should be disabled.
        /// </summary>
        public bool Disabled { get; set; }
        
        protected override void PaintControls()
        {
            EditorGUI.BeginDisabledGroup(Disabled);
            base.PaintControls();
            EditorGUI.EndDisabledGroup();
        }        
    }
}
