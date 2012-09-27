using System;
using System.Collections.Generic;
using UnityEditor;
using Skill.UI;
using Skill.Editor.UI;
using UnityEngine;
using Skill.Editor;


namespace Skill.Editor.Tools
{
    
    [UnityEditor.CustomEditor(typeof(ImplantAsset))]
    class ImplantAssetEditor : UnityEditor.Editor
    {
        #region CreateUI
        private Skill.UI.Grid _MainGrid;
        private Skill.UI.ListBox _PrefabsLB;
        private Skill.UI.StackPanel _ButtonsPanel;
        private Skill.Editor.UI.Button _BtnAdd;
        private Skill.Editor.UI.Button _BtnRemove;
        private Skill.Editor.UI.Button _BtnClear;
        private Skill.Editor.UI.ChangeCheck _ChangeCheck;        

        private void CreateUI()
        {
            _MainGrid = new Skill.UI.Grid();
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            _PrefabsLB = new ListBox() { Row = 1, Column = 0, Margin = new Thickness(2), SelectionMode = Skill.UI.SelectionMode.Single };
            _PrefabsLB.Background.Visibility = Visibility.Visible;
            _PrefabsLB.SelectionChanged += new System.EventHandler(_PrefabsLB_SelectionChanged);

            _ButtonsPanel = new StackPanel() { Row = 0, Column = 0, HorizontalAlignment = HorizontalAlignment.Left, Orientation = Orientation.Horizontal };

            _BtnAdd = new Skill.Editor.UI.Button() { Margin = new Thickness(2, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center, Width = 80 };
            _BtnAdd.Content.text = "Add";
            _BtnAdd.Click += new System.EventHandler(_BtnAdd_Click);

            _BtnRemove = new Skill.Editor.UI.Button() { Margin = new Thickness(2, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center, Width = 80, IsEnabled = false };
            _BtnRemove.Content.text = "Remove";
            _BtnRemove.Click += new System.EventHandler(_BtnRemove_Click);

            _BtnClear = new Skill.Editor.UI.Button() { Margin = new Thickness(2, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center, Width = 80 };
            _BtnClear.Content.text = "Clear";
            _BtnClear.Click += new System.EventHandler(_BtnClear_Click);

            _ButtonsPanel.Controls.Add(_BtnAdd);
            _ButtonsPanel.Controls.Add(_BtnRemove);
            _ButtonsPanel.Controls.Add(_BtnClear);

            _MainGrid.Controls.Add(_ButtonsPanel);
            _MainGrid.Controls.Add(_PrefabsLB);

            _ChangeCheck = new ChangeCheck();
            _ChangeCheck.Controls.Add(_MainGrid);
            _ChangeCheck.Changed += new EventHandler(_ChangeCheck_Changed);
        }

        void _ChangeCheck_Changed(object sender, EventArgs e)
        {
            ImplantAsset asset = target as ImplantAsset;
            if (asset != null)
            {
                EditorUtility.SetDirty(asset);
            }
        }



        void _BtnRemove_Click(object sender, System.EventArgs e)
        {
            if (_PrefabsLB.SelectedItem != null)
            {
                if (_PrefabsLB.Controls.Remove(_PrefabsLB.SelectedItem))
                {
                    SetChanges();
                }
            }
        }

        void _PrefabsLB_SelectionChanged(object sender, System.EventArgs e)
        {
            _BtnRemove.IsEnabled = _PrefabsLB.SelectedItem != null;
        }

        void _BtnClear_Click(object sender, System.EventArgs e)
        {
            _PrefabsLB.Controls.Clear();
            SetChanges();
        }

        void _BtnAdd_Click(object sender, System.EventArgs e)
        {
            _PrefabsLB.Controls.Add(new ImplantObjectField());
            SetChanges();
        }

        private void SetChanges()
        {
            ImplantAsset asset = target as ImplantAsset;
            if (asset != null)
            {
                asset.Objects = new ImplantObject[_PrefabsLB.Controls.Count];
                for (int i = 0; i < _PrefabsLB.Controls.Count; i++)
                {
                    asset.Objects[i] = ((ImplantObjectField)_PrefabsLB.Controls[i]).Object;
                }
                EditorUtility.SetDirty(asset);
            }
        }
        #endregion

        void OnEnable()
        {            
            CreateUI();
            ImplantAsset asset = target as ImplantAsset;
            if (asset != null)
            {
                asset.hideFlags = HideFlags.HideInInspector;
                if (asset.Objects != null)
                {
                    foreach (var item in asset.Objects)
                    {
                        if (item != null)
                        {
                            ImplantObjectField field = new ImplantObjectField(item);
                            _PrefabsLB.Controls.Add(field);
                        }
                    }
                }
            }
            else
            {
                _PrefabsLB.Controls.Clear();
            }
        }        

        public override void OnInspectorGUI()
        {
            Rect rect = EditorGUILayout.BeginVertical();

            _MainGrid.Position = new Rect(0, 0, 300, Mathf.Max(rect.height, 300));


            //rect.y += 30;            
            _ChangeCheck.RenderArea = rect;
            _ChangeCheck.OnGUI();

            EditorGUILayout.EndVertical();
        }
    }
}
