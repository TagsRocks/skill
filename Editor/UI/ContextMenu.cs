﻿using System.Collections;
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

        /// <summary>
        /// Height of ContextMenu to draw DropDown
        /// </summary>
        public float Height { get; private set; }

        /// <summary> Create a MenuItemBase </summary>
        protected MenuItemBase()
        {
            _Items = new List<MenuItem>();
            Height = 16;
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
        /// IsEnable of one sub items changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Item_EnableChanged(object sender, EventArgs e)
        {
            if (Parent != null)
                Parent.Item_EnableChanged(sender, e);
        }

        /// <summary>
        /// Adds a MenuItem element to a ContextMenu
        /// </summary>
        /// <param name="item">Identifies the MenuItem to add to the collection.</param>
        public void Add(MenuItem item)
        {
            if (item == null)
                throw new ArgumentNullException("MenuItem is null");

            item.Parent = this;
            item.EnableChanged += Item_EnableChanged;
            _Items.Add(item);
        }

        /// <summary>
        /// Adds a Separator to MenuItem
        /// </summary>    
        public void AddSeparator()
        {
            _Items.Add(null);
        }
    }

    /// <summary>
    /// ContextMenu 
    /// </summary>
    public class ContextMenu : MenuItemBase, Skill.Framework.UI.IContextMenu
    {
        private GenericMenu _GenericMenu;
        private GenericMenu.MenuFunction2 _MenuFunction;
        private bool _Changed;


        /// <summary> Height of an item </summary>
        public float ItemHeight { get; set; }
        /// <summary> Height of n separator </summary>
        public float SeparatorHeight { get; set; }



        /// <summary>
        /// Create a ContextMenu
        /// </summary>
        public ContextMenu()
        {
            _Changed = true;
            _MenuFunction = Item_Click;
            ItemHeight = 16;
            SeparatorHeight = 8;
        }

        private void ApplyChanges()
        {
            if (_Changed)
            {
                _GenericMenu = new GenericMenu();
                GenerateMenu(this, _GenericMenu, string.Empty);
                _Changed = false;
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
            _GenericMenu.DropDown(position);
        }

        /// <summary> Show the menu under the mouse. </summary>
        public void Show()
        {
            ApplyChanges();
            _GenericMenu.ShowAsContext();
        }


        private void Item_Click(object userData)
        {
            ((MenuItem)userData).RaiseClick();
        }

        protected override void Item_EnableChanged(object sender, EventArgs e)
        {
            _Changed = true;
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
        public string Name { get { return Content.text; } }

        /// <summary> Path of item </summary>
        public string Path { get; internal set; }

        /// <summary> Is checked </summary>
        public bool IsChecked { get; set; }

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
