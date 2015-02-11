using System;
using System.Collections.Generic;
using UnityEditor;
using Skill.Framework.UI;
using Skill.Editor.UI;
using UnityEngine;
using Skill.Editor;
using Skill.Framework;


namespace Skill.Editor
{
    

    [UnityEditor.CustomEditor(typeof(Skill.Framework.SpawnAsset))]
    class SpawnAssetEditor : UnityEditor.Editor, IEditor
    {
        class SpawnAssetListItem : EditorListItem
        {
            public Skill.Framework.SpawnObject Object { get; private set; }

            public SpawnAssetListItem(SpawnAssetEditor editor, Skill.Framework.SpawnObject obj)
                : base(editor)
            {
                this.Object = obj;
            }
            public override void UpdateName()
            {
                ObjectName = (Object.Prefab != null) ? Object.Prefab.name : "(Null)";
            }
        }


        #region CreateUI

        const float ButtonRowHeight = 26;
        private const float FrameHeight = 290;

        private Skill.Framework.UI.ListBox _ItemsList;
        private Skill.Framework.UI.Button _BtnAdd;
        private Skill.Editor.UI.ChangeCheck _MainPanel;
        private Skill.Framework.UI.Frame _Frame;
        private SpawnObjectField _ObjectField;
        private bool _IsCollectionChanged;

        private void CreateUI()
        {
            _ObjectField = new SpawnObjectField(this) { Row = 2, Column = 1 };

            _MainPanel = new ChangeCheck();
            _MainPanel.RowDefinitions.Add(ButtonRowHeight, GridUnitType.Pixel);
            _MainPanel.RowDefinitions.Add(1, GridUnitType.Star);
            _MainPanel.RowDefinitions.Add(_ObjectField.LayoutHeight, GridUnitType.Pixel);

            _ItemsList = new ListBox() { Row = 1, Column = 0, Margin = new Thickness(2) };
            _ItemsList.BackgroundVisible = true;            
            _ItemsList.DisableFocusable();

            _BtnAdd = new Skill.Framework.UI.Button() { Row = 0, Column = 0, Margin = new Thickness(2) };
            _BtnAdd.Content.text = "Add";
            _BtnAdd.Click += new System.EventHandler(_BtnAdd_Click);

            _MainPanel.Controls.Add(new Box() { Row = 2, Column = 1 });
            _MainPanel.Controls.Add(_BtnAdd);
            _MainPanel.Controls.Add(_ItemsList);
            _MainPanel.Controls.Add(_ObjectField);

            _Frame = new Frame("MainFrame");
            _Frame.Grid.Controls.Add(_MainPanel);

            _MainPanel.Changed += new EventHandler(_ChangeCheck_Changed);
            _ItemsList.SelectionChanged += _ItemsList_SelectionChanged;
        }

        void _ItemsList_SelectionChanged(object sender, EventArgs e)
        {
            _ObjectField.Object = (_ItemsList.SelectedItem != null) ? ((SpawnAssetListItem)_ItemsList.SelectedItem).Object : null;
            Repaint();
        }

        void _ChangeCheck_Changed(object sender, EventArgs e)
        {
            SpawnAsset data = target as Skill.Framework.SpawnAsset;
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
                SpawnAssetListItem newItem = new SpawnAssetListItem(this, new SpawnObject());
                _ItemsList.Controls.Insert(index + 1, newItem);
                _IsCollectionChanged = true;
            }
        }

        void _BtnAdd_Click(object sender, System.EventArgs e)
        {
            SpawnAssetListItem newItem = new SpawnAssetListItem(this, new SpawnObject());
            _ItemsList.Controls.Add(newItem);
            _ItemsList.SelectedItem = newItem;
            _IsCollectionChanged = true;
        }

        private void ApplyChanges()
        {
            if (_IsCollectionChanged)
            {
                SpawnAsset data = target as SpawnAsset;
                if (data != null)
                {
                    data.Objects = new SpawnObject[_ItemsList.Controls.Count];
                    for (int i = 0; i < _ItemsList.Controls.Count; i++)
                    {
                        SpawnAssetListItem field = (SpawnAssetListItem)_ItemsList.Controls[i];
                        data.Objects[i] = field.Object;
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
            Skill.Framework.SpawnAsset data = target as Skill.Framework.SpawnAsset;
            if (data != null)
            {
                if (data.Objects != null)
                {
                    foreach (var item in data.Objects)
                    {
                        if (item != null)
                        {
                            SpawnAssetListItem field = new SpawnAssetListItem(this, item);
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
            if (_ItemsList.SelectedStyle == null)
                _ItemsList.SelectedStyle = Resources.Styles.SelectedItem;
            ApplyChanges();
            _Frame.Update();
            _Frame.OnInspectorGUI(FrameHeight);
        }

        public void UpdateNames()
        {
            foreach (SpawnAssetListItem item in _ItemsList.Controls)
                item.UpdateName();
        }
    }
}
