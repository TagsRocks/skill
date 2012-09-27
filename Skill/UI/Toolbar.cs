using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.UI
{
    /// <summary>
    /// Make a toolbar
    /// </summary>
    public class Toolbar : Control
    {
        /// <summary>
        /// Toolbar buttons
        /// </summary>
        public ToolbarButtonCollection Items { get; private set; }

        private int _SelectedIndex = -1;
        /// <summary>
        /// gets or sets selected button index
        /// </summary>
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (_SelectedIndex != value)
                {
                    if (_SelectedIndex >= 0 && _SelectedIndex < Items.Count)
                        Items[_SelectedIndex].IsSelected = false;

                    _SelectedIndex = value;

                    if (_SelectedIndex >= 0 && _SelectedIndex < Items.Count)
                        Items[_SelectedIndex].IsSelected = true;
                }
            }
        }

        /// <summary>
        /// gets or sets selected button
        /// </summary>
        public ToolbarButton SelectedOption
        {
            get
            {
                if (_SelectedIndex >= 0 && _SelectedIndex < Items.Count)
                    return Items[_SelectedIndex];
                return null;
            }
            set
            {
                if (value == null)
                {
                    SelectedIndex = 0;
                }
                else
                {
                    SelectedIndex = Items.IndexOf(value);
                }
            }
        }

        /// <summary>
        /// Create a Toolbar
        /// </summary>
        public Toolbar()
        {
            this.Items = new ToolbarButtonCollection();
        }

        /// <summary>
        /// Render Toolbar
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                SelectedIndex = GUI.Toolbar(RenderArea, _SelectedIndex, Items.Contents, Style);
            }
            else
            {
                SelectedIndex = GUI.Toolbar(RenderArea, _SelectedIndex, Items.Contents);
            }
        }
    }

}