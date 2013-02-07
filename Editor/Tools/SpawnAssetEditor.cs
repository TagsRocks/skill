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

    [UnityEditor.CustomEditor(typeof(Skill.Framework.SpawnAsset))]
    class SpawnAssetEditor : UnityEditor.Editor
    {
        #region CreateUI
        private Skill.Framework.UI.Extended.ListBox _PrefabsLB;
        private Skill.Framework.UI.StackPanel _ButtonsPanel;
        private Skill.Editor.UI.Button _BtnAdd;
        private Skill.Editor.UI.Button _BtnRemove;
        private Skill.Editor.UI.Button _BtnClear;
        private Skill.Editor.UI.ChangeCheck _ChangeCheck;
        private Skill.Framework.UI.Frame _Frame;

        private void CreateUI()
        {
            _PrefabsLB = new  ListBox() { Margin = new Thickness(2), SelectionMode = Skill.Framework.UI.Extended.SelectionMode.Single, Position = new Rect(0, 22, 320, 560) };            
            _PrefabsLB.SelectionChanged += new System.EventHandler(_PrefabsLB_SelectionChanged);

            GUIStyleState selectedItemState = new GUIStyleState() { background = Resources.SelectedItemBackground };
            _PrefabsLB.SelectedStyle = new GUIStyle()
            {
                normal = selectedItemState,
            };

            _ButtonsPanel = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Left, Orientation = Orientation.Horizontal, Position = new Rect(0, 0, 300, 20) };

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

            _ChangeCheck = new ChangeCheck();
            _ChangeCheck.Controls.Add(_ButtonsPanel);
            _ChangeCheck.Controls.Add(_PrefabsLB);
            _ChangeCheck.Changed += new EventHandler(_ChangeCheck_Changed);

            _Frame = new Frame("MainFrame");
            _Frame.Grid.Controls.Add(_ChangeCheck);
        }

        void _ChangeCheck_Changed(object sender, EventArgs e)
        {
            SpawnAsset data = target as Skill.Framework.SpawnAsset;
            if (data != null)
            {
                EditorUtility.SetDirty(data);
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
            _PrefabsLB.Controls.Add(new SpawnObjectField());
            SetChanges();
        }

        private void SetChanges()
        {
            SpawnAsset data = target as SpawnAsset;
            if (data != null)
            {
                data.Objects = new SpawnObject[_PrefabsLB.Controls.Count];
                for (int i = 0; i < _PrefabsLB.Controls.Count; i++)
                {
                    data.Objects[i] = ((SpawnObjectField)_PrefabsLB.Controls[i]).Object;
                }
                EditorUtility.SetDirty(data);
            }
        }
        #endregion

        void OnEnable()
        {
            CreateUI();
            Skill.Framework.SpawnAsset data = target as Skill.Framework.SpawnAsset;
            if (data != null)
            {
                data.hideFlags = HideFlags.HideInInspector;
                if (data.Objects != null)
                {
                    foreach (var item in data.Objects)
                    {
                        if (item != null)
                        {
                            SpawnObjectField field = new SpawnObjectField(item);
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
            Rect rect = EditorGUILayout.BeginVertical(); // because we do not know top offset of Inspector
            EditorGUILayout.EndVertical();

            rect.width = 320;
            rect.height = 600;
            _Frame.Position = rect;

            rect.x = rect.y = 0;
            _ChangeCheck.Position = rect;

            _Frame.OnGUI();            
        }
    }
}
