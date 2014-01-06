using System;
using System.Collections.Generic;
using UnityEditor;
using Skill.Framework.UI;
using Skill.Framework.UI.Extended;
using Skill.Editor.UI;
using UnityEngine;
using Skill.Editor;


namespace Skill.Editor.Tools
{

       
    class ImplantObjectListItem : EditorListItem
    {
        public ImplantObject Object { get; private set; }

        public ImplantObjectListItem(ImplantAssetEditor editor, ImplantObject obj)
            : base(editor)
        {
            this.Object = obj;
        }
        public void UpdateName()
        {
            ObjectName = (Object.Prefab != null) ? Object.Prefab.name : "(Null)";
        }
    }


    [UnityEditor.CustomEditor(typeof(ImplantAsset))]
    class ImplantAssetEditor : UnityEditor.Editor, IEditor
    {
        #region CreateUI

        const float ButtonRowHeight = 26;
        private const float FrameHeight = 290;

        private Skill.Framework.UI.Extended.ListBox _ItemsList;
        private Skill.Editor.UI.Button _BtnAdd;
        private Skill.Editor.UI.ChangeCheck _MainPanel;
        private Skill.Framework.UI.Frame _Frame;
        private ImplantObjectField _ObjectField;
        private bool _IsCollectionChanged;

        private void CreateUI()
        {
            _ObjectField = new ImplantObjectField(this) { Row = 2, Column = 1 };

            _MainPanel = new ChangeCheck();
            _MainPanel.RowDefinitions.Add(ButtonRowHeight, GridUnitType.Pixel);
            _MainPanel.RowDefinitions.Add(1, GridUnitType.Star);
            _MainPanel.RowDefinitions.Add(_ObjectField.LayoutHeight, GridUnitType.Pixel);

            _ItemsList = new ListBox() { Row = 1, Column = 0, Margin = new Thickness(2) };
            _ItemsList.BackgroundVisible = true;
            _ItemsList.SelectedStyle = new GUIStyle();
            _ItemsList.SelectedStyle.normal.background = Resources.Textures.SelectedItemBackground;
            _ItemsList.DisableFocusable();

            _BtnAdd = new Skill.Editor.UI.Button() { Row = 0, Column = 0, Margin = new Thickness(2) };
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
            _ObjectField.Object = (_ItemsList.SelectedItem != null) ? ((ImplantObjectListItem)_ItemsList.SelectedItem).Object : null;
            Repaint();
        }

        void _ChangeCheck_Changed(object sender, EventArgs e)
        {
            ImplantAsset data = target as ImplantAsset;
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
                ImplantObjectListItem newItem = new ImplantObjectListItem(this, CreateNew());
                _ItemsList.Controls.Insert(index + 1, newItem);
                _IsCollectionChanged = true;
            }
        }

        private ImplantObject CreateNew()
        {
            return new ImplantObject() { Prefab = null, MinScalePercent = 0.8f, MaxScalePercent = 1.0f, Weight = 1.0f, Rotation = ImplantObjectRotation.SurfaceNormal };
        }

        void _BtnAdd_Click(object sender, System.EventArgs e)
        {
            ImplantObjectListItem newItem = new ImplantObjectListItem(this, CreateNew());
            _ItemsList.Controls.Add(newItem);
            _ItemsList.SelectedItem = newItem;
            _IsCollectionChanged = true;
        }

        private void ApplyChanges()
        {
            if (_IsCollectionChanged)
            {
                ImplantAsset data = target as ImplantAsset;
                if (data != null)
                {
                    data.Objects = new ImplantObject[_ItemsList.Controls.Count];
                    for (int i = 0; i < _ItemsList.Controls.Count; i++)
                    {
                        ImplantObjectListItem field = (ImplantObjectListItem)_ItemsList.Controls[i];
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
            ImplantAsset data = target as ImplantAsset;
            if (data != null)
            {
                if (data.Objects != null)
                {
                    foreach (var item in data.Objects)
                    {
                        if (item != null)
                        {
                            ImplantObjectListItem field = new ImplantObjectListItem(this, item);
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
            foreach (ImplantObjectListItem item in _ItemsList.Controls)
                item.UpdateName();
        }
    }
}
