using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Skill.Editor.UI.Extended
{

    /// <summary>
    /// MenuBar at top of EditorWindow.
    /// Only works in EditorWindow and EditorWindow.wantsMouseMove = true
    /// </summary>
    /// <remarks>
    /// Just Add MenuBarItem unless you get error
    /// </remarks>
    public class MenuBar : Skill.Framework.UI.StackPanel
    {
        /// <summary>
        ///  Background of MenuBar ( it is possible to customize it's style)
        /// </summary>
        public Skill.Framework.UI.Box Background { get; private set; }

        private MenuBarItem _ActiveItem;
        internal MenuBarItem ActiveItem
        {
            get { return _ActiveItem; }
            set
            {
                if (_ActiveItem != null) _ActiveItem.DeActive();
                _ActiveItem = value;
                if (_ActiveItem != null) _ActiveItem.Active();
            }
        }


        /// <summary>
        /// Create a MenuBar
        /// </summary>
        public MenuBar()
        {
            WantsMouseEvents = true;
            Background = new Skill.Framework.UI.Box(); Background.Parent = this;
            Background.Style = new GUIStyle(EditorStyles.toolbar);
            Orientation = Skill.Framework.UI.Orientation.Horizontal;
        }

        /// <summary>
        /// When RenderArea changed
        /// </summary> 
        protected override void OnRenderAreaChanged()
        {
            Background.RenderArea = RenderArea;
            base.OnRenderAreaChanged();
        }

        /// <summary>
        /// Render
        /// </summary>
        protected override void Render()
        {
            Background.OnGUI();
            base.Render();
        }
    }

    /// <summary>
    /// MenuBarItem for add to MenuBar
    /// </summary>
    public class MenuBarItem : Skill.Framework.UI.Box
    {
        private Skill.Editor.UI.ContextMenu _ContextMenu;

        public GUIStyle DefaultStyle { get; set; }
        public GUIStyle HoverStyle { get; set; }
        public GUIStyle PressedStyle { get; set; }

        /// <summary> Title (Content.text) </summary>
        public string Title { get { return Content.text; } set { Content.text = value; } }

        /// <summary>
        /// Parent MenuBar
        /// </summary>
        public override Skill.Framework.UI.IControl Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                if (value != null && !(value is MenuBar))
                    throw new System.InvalidOperationException("Can not add 'MenuBarItem' to something other than 'MenuBar'");
                base.Parent = value;
                if (value != null)
                {
                    _MenuBar = (MenuBar)value;
                    if (_MenuBar.OwnerFrame != null)
                        _EditorFrame = _MenuBar.OwnerFrame as Skill.Editor.UI.EditorFrame;
                }
                else
                {
                    _MenuBar = null;
                    _EditorFrame = null;
                }
            }
        }

        private MenuBar _MenuBar;
        private Skill.Editor.UI.EditorFrame _EditorFrame;
        private void RepaintEditorWindow()
        {
            if (_EditorFrame == null)
            {
                if (_MenuBar != null && _MenuBar.OwnerFrame != null)
                    _EditorFrame = _MenuBar.OwnerFrame as Skill.Editor.UI.EditorFrame;
            }
            if (_EditorFrame != null)
                _EditorFrame.Owner.Repaint();
        }

        /// <summary>
        /// Create a MenuBarItem
        /// </summary>
        public MenuBarItem()
        {
            WantsMouseEvents = true;
            Width = 35;
            _ContextMenu = new Skill.Editor.UI.ContextMenu();


            this.DefaultStyle = new GUIStyle(EditorStyles.toolbarButton);

            this.HoverStyle = new GUIStyle(EditorStyles.toolbarButton);
            this.HoverStyle.normal = this.HoverStyle.hover;

            this.PressedStyle = new GUIStyle(EditorStyles.toolbarButton);
            this.PressedStyle.normal = this.PressedStyle.active;

            this.Style = DefaultStyle;
        }

        protected override void OnMouseDown(Skill.Framework.UI.MouseClickEventArgs args)
        {
            if (args.Button == Skill.Framework.UI.MouseButton.Left)
            {
                if (_MenuBar != null)
                    _MenuBar.ActiveItem = this;
            }
            base.OnMouseDown(args);
        }

        protected override void OnMouseEnter(Skill.Framework.UI.MouseEventArgs args)
        {
            this.Style = HoverStyle;
            RepaintEditorWindow();
            base.OnMouseEnter(args);
        }

        protected override void OnMouseLeave(Skill.Framework.UI.MouseEventArgs args)
        {
            this.Style = DefaultStyle;
            RepaintEditorWindow();
            base.OnMouseLeave(args);
        }

        private Rect _DropDownRect;
        private bool _IsActive;
        private int _PaintCounter;
        internal void Active()
        {
            if (!_IsActive)
            {
                _IsActive = true;
                _PaintCounter = 1;
            }
            _DropDownRect = RenderArea;
            _DropDownRect.y = _DropDownRect.yMax - 14;
            this.Style = PressedStyle;
            RepaintEditorWindow();
        }

        internal void DeActive()
        {
            this.Style = DefaultStyle;
            _IsActive = false;
        }

        /// <summary> Render </summary>
        protected override void Render()
        {
            base.Render();
            if (_IsActive)
            {
                if (_PaintCounter > 0)
                {
                    _PaintCounter--;
                    RepaintEditorWindow();
                }
                else
                {
                    _ContextMenu.DropDown(_DropDownRect);
                    _IsActive = false;
                }
            }
        }

        /// <summary>
        /// Adds a MenuItem element to a ContextMenu
        /// </summary>
        /// <param name="item">Identifies the MenuItem to add to the collection.</param>
        public void Add(Skill.Editor.UI.MenuItem item)
        {
            _ContextMenu.Add(item);
        }

        /// <summary>
        /// Adds a Separator to ContextMenuItem
        /// </summary>    
        public void AddSeparator()
        {
            _ContextMenu.AddSeparator();
        }
    }

}
