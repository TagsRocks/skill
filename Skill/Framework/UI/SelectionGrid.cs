using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;



namespace Skill.Framework.UI
{
    /// <summary>
    /// Make a grid of buttons.
    /// </summary>
    public class SelectionGrid : Control
    {
        /// <summary>
        /// Grid items
        /// </summary>
        public SelectionGridItemCollection Items { get; private set; }

        private int _SelectedIndex = -1;
        /// <summary>
        /// Gets or sets Selected grid item index
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
                    OnSelectedChanged();

                    if (_SelectedIndex >= 0 && _SelectedIndex < Items.Count)
                        Items[_SelectedIndex].IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets Selected grid item
        /// </summary>
        public SelectionGridItem SelectedOption
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

        /// <summary> How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</summary>
        public int XCount { get; set; }

        /// <summary>
        /// Occurs when selected item changed
        /// </summary>
        public event EventHandler SelectedChanged;
        /// <summary>
        /// when selected item changed
        /// </summary>
        protected virtual void OnSelectedChanged()
        {
            if (SelectedChanged != null)
                SelectedChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create an instance of SelectionGrid
        /// </summary>
        public SelectionGrid()
        {
            this.Items = new SelectionGridItemCollection();
            this.XCount = 2;
        }

        /// <summary>
        /// Render SelectionGrid
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                SelectedIndex = GUI.SelectionGrid(RenderArea, _SelectedIndex, Items.Contents, XCount, Style);
            }
            else
            {
                SelectedIndex = GUI.SelectionGrid(RenderArea, _SelectedIndex, Items.Contents, XCount);
            }
        }        
    }
}