using System;
using System.Collections.Generic;
using UnityEditor;
using Skill.Framework.UI;
using Skill.Editor.UI;
using UnityEngine;
using Skill.Editor;
using Skill.Framework;
using Skill.Framework.UI.Extended;


namespace Skill.Editor.Tools
{
    class CacheGroupListItem : EditorListItem
    {
        public Skill.Framework.Managers.CacheObject Object { get; private set; }

        public CacheGroupListItem(CacheGroupEditor editor, Skill.Framework.Managers.CacheObject obj)
            : base(editor)
        {
            this.Object = obj;
        }
        public void UpdateName()
        {
            ObjectName = (Object.Prefab != null) ? Object.Prefab.name : "(Null)";
        }
    }

    [UnityEditor.CustomEditor(typeof(Framework.Managers.CacheGroup))]
    class CacheGroupEditor : UnityEditor.Editor , IEditor
    {
        #region CreateUI
        private const float FrameHeight = 234;
        private const float ButtonRowHeight = 26;

        private Skill.Editor.UI.ChangeCheck _MainPanel;
        private Skill.Framework.UI.Extended.ListBox _ItemsList;
        private Skill.Editor.UI.Button _BtnAdd;
        private Skill.Framework.UI.Frame _Frame;
        private CacheObjectField _ObjectField;
        private bool _IsCollectionChanged;

        private void CreateUI()
        {
            _ObjectField = new CacheObjectField(this) { Row = 2, Column = 1 };            

            _MainPanel = new ChangeCheck();
            _MainPanel.RowDefinitions.Add(ButtonRowHeight, GridUnitType.Pixel);
            _MainPanel.RowDefinitions.Add(1, GridUnitType.Star);            
            _MainPanel.RowDefinitions.Add(_ObjectField.LayoutHeight, GridUnitType.Pixel);

            _ItemsList = new ListBox() { Row = 1, Column = 0, Margin = new Thickness(2) };
            _ItemsList.BackgroundVisible = true;
            _ItemsList.SelectedStyle = Resources.Styles.SelectedItem;
            _ItemsList.DisableFocusable();

            _BtnAdd = new Skill.Editor.UI.Button() { Row = 0, Column = 0, Margin = new Thickness(2) };
            _BtnAdd.Content.text = "Add";
            _BtnAdd.Click += new System.EventHandler(_BtnAdd_Click);

            _MainPanel.Controls.Add(new Box() { Row = 2, Column = 1 });
            _MainPanel.Controls.Add(_ItemsList);
            _MainPanel.Controls.Add(_ObjectField);
            _MainPanel.Controls.Add(_BtnAdd);

            _Frame = new Frame("MainFrame");
            _Frame.Grid.Controls.Add(_MainPanel);

            _MainPanel.Changed += new EventHandler(_ChangeCheck_Changed);
            _ItemsList.SelectionChanged += _ItemsPanel_SelectionChanged;
        }

        void _ItemsPanel_SelectionChanged(object sender, EventArgs e)
        {
            _ObjectField.Object = (_ItemsList.SelectedItem != null) ? ((CacheGroupListItem)_ItemsList.SelectedItem).Object : null;
            Repaint();
        }

        void _BtnAdd_Click(object sender, System.EventArgs e)
        {
            CacheGroupListItem newField = new CacheGroupListItem(this, new Framework.Managers.CacheObject());
            _ItemsList.Controls.Add(newField);
            _ItemsList.SelectedItem = newField;
            _IsCollectionChanged = true;
        }

        void _ChangeCheck_Changed(object sender, EventArgs e)
        {
            Skill.Framework.Managers.CacheGroup data = target as Skill.Framework.Managers.CacheGroup;
            if (data != null)
            {
                EditorUtility.SetDirty(data);
            }
        }

        public void Remove(EditorListItem item)
        {
            if (_ItemsList.Controls.Remove(item))
                _IsCollectionChanged = true;
        }
        public void NewAfter(EditorListItem item)
        {
            int index = _ItemsList.Controls.IndexOf(item);
            if (index >= 0)
            {
                CacheGroupListItem newField = new CacheGroupListItem(this, new Framework.Managers.CacheObject());
                _ItemsList.Controls.Insert(index + 1, newField);
                _IsCollectionChanged = true;
            }
        }

        private void ApplyChanges()
        {
            if (_IsCollectionChanged)
            {
                Skill.Framework.Managers.CacheGroup data = target as Skill.Framework.Managers.CacheGroup;
                if (data != null)
                {
                    data.Caches = new Framework.Managers.CacheObject[_ItemsList.Controls.Count];
                    for (int i = 0; i < _ItemsList.Controls.Count; i++)
                    {
                        CacheGroupListItem field = (CacheGroupListItem)_ItemsList.Controls[i];
                        data.Caches[i] = field.Object;
                    }
                    EditorUtility.SetDirty(data);
                }
                _IsCollectionChanged = false;

            }
        }
        #endregion

        void OnEnable()
        {
            CreateUI();
            Skill.Framework.Managers.CacheGroup data = target as Skill.Framework.Managers.CacheGroup;
            if (data != null)
            {
                if (data.Caches != null)
                {
                    foreach (var item in data.Caches)
                    {
                        if (item != null)
                        {
                            CacheGroupListItem field = new CacheGroupListItem(this, item);
                            _ItemsList.Controls.Add(field);
                        }
                    }
                }
            }
            else
            {
                _ItemsList.Controls.Clear();
            }
            if (_ItemsList.Controls.Count > 0)
                _ItemsList.SelectedIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            ApplyChanges();
            _Frame.Update();
            _Frame.OnInspectorGUI(FrameHeight);
        }

        public void UpdateNames()
        {
            foreach (CacheGroupListItem item in _ItemsList.Controls)
                item.UpdateName();
        }
    }

    

}
