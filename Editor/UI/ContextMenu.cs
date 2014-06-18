using System.Collections;
using System;
using System.Collections.Generic;
using Skill.Editor.UI;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary> MenuItemBase </summary>
    public abstract class MenuItemBase
    {
        private List<MenuItem> _Items;
        internal MenuItemBase Parent { get; set; }
        internal int Count { get { return _Items.Count; } }
        internal MenuItem this[int index] { get { return _Items[index]; } }

        /// <summary> IsChanged </summary>
        protected bool IsChanged { get; set; }

        /// <summary>
        /// Height of ContextMenu to draw DropDown
        /// </summary>
        public float Height { get; private set; }

        /// <summary> Create a MenuItemBase </summary>
        protected MenuItemBase()
        {
            _Items = new List<MenuItem>();
            Height = 16;
            IsChanged = true;
        }

        internal void CalcHeight(float itemH, float sepratorH)
        {
            if (_Items.Count == 0)
                Height = itemH;
            else
            {
                Height = 0;
                for (int i = 0; i < _Items.Count; i++)
                {
                    if (_Items[i] == null)
                        Height += sepratorH;
                    else
                        Height += itemH;
                }
            }
        }

        /// <summary>
        /// When any properties of item is changed
        /// </summary>
        protected virtual void OnChanged()
        {
            if (Parent != null)
                Parent.OnChanged();
        }

        /// <summary>
        /// Adds a MenuItem element to a ContextMenu
        /// </summary>
        /// <param name="item">Identifies the MenuItem to add to the collection.</param>
        public void Add(MenuItem item)
        {
            if (item == null)
                throw new ArgumentNullException("MenuItem is null");

            ValidateNameForDuplicate(item);

            item.Parent = this;
            _Items.Add(item);

            IsChanged = true;
        }

        private void ValidateNameForDuplicate(MenuItem item)
        {
            int i = 1;
            string name = item.Name;
            do
            {
                MenuItem sameNameItem = null;
                foreach (var myItem in _Items)
                {
                    if (myItem == null) continue;
                    if (myItem.Name == name)
                    {
                        sameNameItem = myItem;
                        break;
                    }
                }
                if (sameNameItem != null)
                {
                    name = item.Name + i++;
                    continue;
                }
            } while (false);
            item.Name = name;
        }

        /// <summary>
        /// Adds a Separator to MenuItem
        /// </summary>    
        public void AddSeparator()
        {
            _Items.Add(null);
            IsChanged = true;
        }

        /// <summary>
        /// Remove all MenuItem and seperators
        /// </summary>        
        public void Clear()
        {
            foreach (var item in _Items)
            {
                if (item != null)
                {
                    item.Parent = null;
                }
            }
            _Items.Clear();
            IsChanged = true;
        }
    }

    /// <summary>
    /// ContextMenu 
    /// </summary>
    public class ContextMenu : MenuItemBase, Skill.Framework.UI.IContextMenu
    {
        private GenericMenu _GenericMenu;
        private GenericMenu.MenuFunction2 _MenuFunction;


        /// <summary> Height of an item </summary>
        public float ItemHeight { get; set; }
        /// <summary> Height of n separator </summary>
        public float SeparatorHeight { get; set; }

        /// <summary> Owner ui object that showthis contenxt menu  </summary>
        public Framework.UI.BaseControl Owner { get; private set; }

        /// <summary>
        /// position of ContextMenu
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// Create a ContextMenu
        /// </summary>
        public ContextMenu()
        {
            _MenuFunction = Item_Click;
            ItemHeight = 16;
            SeparatorHeight = 8;
        }

        /// <summary>
        /// Apply changes and generate GenericMenu
        /// </summary>
        protected virtual void ApplyChanges()
        {
            if (IsChanged)
            {
                _GenericMenu = new GenericMenu();
                GenerateMenu(this, _GenericMenu, string.Empty);
                IsChanged = false;
            }
        }

        private void GenerateMenu(MenuItemBase contextMenu, GenericMenu genericMenu, string path)
        {
            if (contextMenu.Count > 0)
            {
                for (int i = 0; i < contextMenu.Count; i++)
                {
                    MenuItem item = contextMenu[i];
                    if (item == null)
                    {
                        if (string.IsNullOrEmpty(path))
                            genericMenu.AddSeparator(string.Empty);
                        else
                            genericMenu.AddSeparator(path + "/");
                    }
                    else
                    {
                        item.Path = item.Name;
                        if (!string.IsNullOrEmpty(path))
                            item.Path = string.Format("{0}/{1}", path, item.Name);

                        if (item.Count > 0)
                        {
                            GenerateMenu(item, genericMenu, item.Path);
                        }
                        else
                        {
                            GUIContent content = new GUIContent(item.Content);
                            content.text = item.Path;

                            if (item.IsEnabled)
                                _GenericMenu.AddItem(content, item.IsChecked, _MenuFunction, item);
                            else
                                _GenericMenu.AddDisabledItem(content);
                        }

                    }
                }
            }
            contextMenu.CalcHeight(ItemHeight, SeparatorHeight);
        }

        /// <summary> Show the menu at the given screen rect. </summary>
        /// <param name="position"> position of ContextMenu</param>
        public void DropDown(Rect position)
        {
            ApplyChanges();
            this.Position = new Vector2(position.x, position.y);
            _GenericMenu.DropDown(position);
        }

        /// <summary> Show the menu under the mouse. </summary>
        /// <param name="owner"> Owner ui object that showthis contenxt menu  </param>
        /// <param name="position">Mouse position</param>
        public void Show(Framework.UI.BaseControl owner, Vector2 position)
        {            
            this.Owner = owner;
            this.Position = position;
            BeginShow();
            ApplyChanges();
            _GenericMenu.ShowAsContext();
        }

        /// <summary>
        /// Prepare right before show
        /// </summary>
        /// <param name="owner"> Owner ui object that showthis contenxt menu  </param>
        protected virtual void BeginShow() { }


        private void Item_Click(object userData)
        {
            ((MenuItem)userData).RaiseClick();
        }

        protected override void OnChanged()
        {
            IsChanged = true;
        }


    }


    /// <summary>
    /// Menu Item
    /// </summary>
    public class MenuItem : MenuItemBase
    {
        /// <summary>
        /// text,Image or tootip of item
        /// </summary>
        public GUIContent Content { get; private set; }

        /// <summary> Name </summary>
        public string Name { get { return Content.text; } set { Content.text = value; } }

        /// <summary> Path of item </summary>
        public string Path { get; internal set; }


        private bool _IsChecked;
        /// <summary> Is checked </summary>
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {

                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    OnCheckedChanged();
                }
            }
        }

        /// <summary> User data </summary>
        public object UserData { get; set; }

        /// <summary>
        /// Occurs When item clicked
        /// </summary>
        public event EventHandler Click;
        /// <summary>
        /// Occurs When item clicked
        /// </summary>
        protected virtual void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        internal void RaiseClick()
        {
            OnClick();
        }

        private bool _IsEnabled;
        /// <summary> Is enabled </summary>
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {

                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    OnEnableChanged();
                }
            }
        }

        /// <summary>
        /// Occurs When item enabled or disabled
        /// </summary>
        public event EventHandler EnableChanged;
        /// <summary>
        /// Occurs When item enabled or disabled
        /// </summary>
        protected virtual void OnEnableChanged()
        {
            if (EnableChanged != null)
                EnableChanged(this, EventArgs.Empty);
            OnChanged();
        }


        /// <summary>
        /// Occurs When item checked is changed
        /// </summary>
        public event EventHandler CheckedChanged;
        /// <summary>
        /// Occurs When item checked is changed
        /// </summary>
        protected virtual void OnCheckedChanged()
        {
            if (CheckedChanged != null)
                CheckedChanged(this, EventArgs.Empty);
            OnChanged();
        }


        /// <summary>
        /// Create an instance of MenuItem
        /// </summary>    

        public MenuItem(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid name");
            this.Content = new GUIContent() { text = name };
            this._IsEnabled = true;
            this.IsChecked = false;
        }
    }
}
