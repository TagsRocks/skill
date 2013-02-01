using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Framework.UI;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Defines base class for all editor control
    /// </summary>
    public abstract class EditorControl : Control
    {

        /// <summary>
        /// Create an EditorControl
        /// </summary>
        public EditorControl()
        {
            this.Position = new UnityEngine.Rect(0, 0, 300, 16);
        }

        /// <summary> Make control enabled or disabled</summary>
        /// <param name="enable">Enabled value</param>        
        protected override void ApplyGUIEnable(bool enable)
        {
            EditorGUI.BeginDisabledGroup(enable);
        }

        /// <summary>
        /// Restore previous value of GUI enable
        /// </summary>        
        protected override void RestoreGUIEnable()
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}
