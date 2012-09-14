﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.UI;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Defines base class for all editor control
    /// </summary>
    public abstract class EditorControl : Control
    {
        /// <summary>
        /// Gets or sets a value indicating whether this element is enabled in the user interface (UI).
        /// </summary>
        /// <returns>  true if the element is enabled; otherwise, false. The default value is true. </returns>
        public bool IsEnabled { get; set; }

        public EditorControl()
        {
            this.IsEnabled = true;
            this.Position = new UnityEngine.Rect(0, 0, 300, 16);
        }

        protected override void BeginPaint(PaintParameters paintParams)
        {
            base.BeginPaint(paintParams);
            if (!IsEnabled)
                EditorGUI.BeginDisabledGroup(true);
        }

        protected override void EndPaint(PaintParameters paintParams)
        {
            base.EndPaint(paintParams);
            if (!IsEnabled)
                EditorGUI.EndDisabledGroup();
        }
    }
}
