using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// Show child controls in tree view. use FolderView class to arrange controls
    /// </summary>
    public class TreeView : StackPanel
    {

        private BaseControl _SelectedItem;

        /// <summary> Gets or sets the selected item. it must be in hierarchy of TreeView </summary>    
        public BaseControl SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (value != null && _SelectedItem != value)
                {
                    if (!IsInHierarchy(value))
                        value = null;
                }

                if (_SelectedItem != value)
                {
                    _SelectedItem = value;
                    OnSelectedItemChanged();
                }
            }
        }

        /// <summary>
        /// Style to use for Box used as background of selected items
        /// </summary>
        public GUIStyle SelectedStyle { get; set; }

        /// <summary>
        /// Occurs when the SelectedItem of TreeView changes.
        /// </summary>
        public event System.EventHandler SelectedItemChanged;
        /// <summary>
        /// when the SelectedItem of TreeView changes.
        /// </summary>
        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null) SelectedItemChanged(this, System.EventArgs.Empty);
            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
        }

        /// <summary>
        /// Create a TreeView
        /// </summary>
        public TreeView()
        {
            Orientation = Skill.Framework.UI.Orientation.Vertical;
            WantsMouseEvents = true;
            SelectedStyle = Resources.Styles.SelectedItem;
        }

        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="args">args</param>
        protected override void OnMouseDown(MouseClickEventArgs args)
        {
            if (args.Button == MouseButton.Left)
            {
                Vector2 mousePos = args.MousePosition;
                if (_SelectedItem != null && _SelectedItem.Containes(mousePos)) // maybe selected item is a folder and user clicked on child of it
                {
                    BaseControl select = _SelectedItem.GetControlAtPoint(mousePos);
                    if (select != null)
                        SelectedItem = select;
                }
                else
                {
                    BaseControl select = GetControlAtPoint(mousePos);
                    if (select != null)
                        SelectedItem = select;
                }
            }
            base.OnMouseDown(args);
        }

        /// <summary>
        /// Render
        /// </summary>
        protected override void Render()
        {
            if (_SelectedItem != null) // render background of selected item
            {
                Rect ra = this.RenderArea;
                Rect cRa = _SelectedItem.RenderArea;
                cRa.xMin = ra.xMin;                 
                cRa.xMax = ra.xMax;

                if (SelectedStyle != null)
                    GUI.Box(cRa, string.Empty, SelectedStyle);
                else
                    GUI.Box(cRa, string.Empty);
            }

            base.Render(); // render rest of controls
        }

    }
}
