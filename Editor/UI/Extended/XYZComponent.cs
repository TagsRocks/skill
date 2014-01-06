﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// Make three ToggleButtons for X,Y and Z
    /// </summary>
    public class XYZComponent : Skill.Editor.UI.EditorControl
    {

        private Skill.Framework.UI.StackPanel _Panel;
        private Skill.Framework.UI.ToggleButton _TbX, _TbY, _TbZ;

        /// <summary> Retrieves ToggleButton used for X component </summary>
        public Skill.Framework.UI.ToggleButton XComponent { get { return _TbX; } }
        /// <summary> Retrieves ToggleButton used for Y component </summary>
        public Skill.Framework.UI.ToggleButton YComponent { get { return _TbY; } }
        /// <summary> Retrieves ToggleButton used for Z component </summary>
        public Skill.Framework.UI.ToggleButton ZComponent { get { return _TbZ; } }

        /// <summary> Is X component checked </summary>
        public bool IsXChecked { get { return _TbX.IsChecked; } }
        /// <summary> Is Y component checked </summary>
        public bool IsYChecked { get { return _TbY.IsChecked; } }
        /// <summary> Is Z component checked </summary>
        public bool IszChecked { get { return _TbZ.IsChecked; } }

        /// <summary>
        /// Create a ToggleButton
        /// </summary>
        public XYZComponent()
        {
            this.Height = 16;
            this._Panel = new Skill.Framework.UI.StackPanel() { Orientation = Skill.Framework.UI.Orientation.Horizontal, Parent = this };

            this._TbX = new Skill.Framework.UI.ToggleButton() { Margin = new Skill.Framework.UI.Thickness(2, 0, 0, 0), Width = 30 };
            this._TbX.Content.text = "X";

            this._TbY = new Skill.Framework.UI.ToggleButton() { Margin = new Skill.Framework.UI.Thickness(2, 0, 0, 0), Width = 30 };
            this._TbY.Content.text = "Y";

            this._TbZ = new Skill.Framework.UI.ToggleButton() { Margin = new Skill.Framework.UI.Thickness(2, 0, 0, 0), Width = 30 };
            this._TbZ.Content.text = "Z";

            this._Panel.Controls.Add(this._TbX);
            this._Panel.Controls.Add(this._TbY);
            this._Panel.Controls.Add(this._TbZ);
        }

        /// <summary>
        /// when RenderArea of control changed
        /// </summary>
        protected override void OnRenderAreaChanged()
        {
            _Panel.RenderArea = RenderArea;
            base.OnRenderAreaChanged();
        }

        /// <summary>
        /// Render XYZComponent
        /// </summary>
        protected override void Render()
        {
            this._Panel.OnGUI();
        }

    }
}
