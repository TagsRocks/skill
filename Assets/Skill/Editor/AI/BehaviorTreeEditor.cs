using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;

namespace Skill.Editor.AI
{
    [UnityEditor.CustomEditor(typeof(BehaviorTreeAsset))]
    public class BehaviorTreeEditor : UnityEditor.Editor
    {
        private BehaviorTreeAsset _Data;
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Framework.UI.Button _BtnEdit;
        private Skill.Framework.UI.Button _BtnBuild;
        private Skill.Framework.UI.Label _BuildPathLabel;
        private Skill.Editor.UI.TextField _TFBuildPath;

        void OnEnable()
        {
            _Data = target as BehaviorTreeAsset;
            CreateUI();
        }

        private void CreateUI()
        {
            _Frame = new Skill.Framework.UI.Frame("Frame");
            _Frame.Grid.RowDefinitions.Add(30, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(30, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel);

            _Frame.Grid.ColumnDefinitions.Add(70, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            _BtnEdit = new Skill.Framework.UI.Button() { Row = 0, Column = 0, ColumnSpan = 2, Margin = new Skill.Framework.UI.Thickness(2) };
            _BtnEdit.Content.text = "Edit";
            _Frame.Controls.Add(_BtnEdit);

            _BtnBuild = new Skill.Framework.UI.Button() { Row = 1, Column = 0, ColumnSpan = 2, Margin = new Skill.Framework.UI.Thickness(2) };
            _BtnBuild.Content.text = "Build";
            _Frame.Controls.Add(_BtnBuild);

            _BuildPathLabel = new Skill.Framework.UI.Label() { Row = 2, Column = 0, Margin = new Skill.Framework.UI.Thickness(2), Text = "Build Path" };
            _Frame.Controls.Add(_BuildPathLabel);

            _TFBuildPath = new TextField() { Row = 2, Column = 1, Margin = new Skill.Framework.UI.Thickness(2), Text = _Data.BuildPath };
            _Frame.Controls.Add(_TFBuildPath);

            _BtnBuild.Click += _BtnBuild_Click;
            _BtnEdit.Click += _BtnEdit_Click;
            _TFBuildPath.TextChanged += _TFBuildPath_TextChanged;
        }
        void _TFBuildPath_TextChanged(object sender, System.EventArgs e)
        {
            _Data.BuildPath = _TFBuildPath.Text;
            EditorUtility.SetDirty(_Data);
        }
        void _BtnBuild_Click(object sender, System.EventArgs e)
        {
            Build();
        }
        void _BtnEdit_Click(object sender, System.EventArgs e)
        {
            BehaviorTreeEditorWindow.Instance.Show();
            BehaviorTreeEditorWindow.Instance.Asset = _Data;
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            _Frame.Update();
            _Frame.OnInspectorGUI(80);
        }

        #region Compile and Build
        private void Build()
        {
            BehaviorTreeData bTree = _Data.Load();
            if (bTree != null)
            {
                SharedAccessKeysData[] sharedAccessKeys = new SharedAccessKeysData[_Data.SharedKeys.Length];
                for (int i = 0; i < _Data.SharedKeys.Length; i++)
                {
                    if (_Data.SharedKeys[i] != null)
                        sharedAccessKeys[i] = _Data.SharedKeys[i].Load();
                }

                bool compiled = BehaviorTreeCompiler.Compile(bTree, sharedAccessKeys);
                if (compiled)
                    Builder.Build(bTree, _Data.BuildPath, _Data.name);
            }
            else
            {
                Debug.LogError("Invalid SaveData");
            }
        }
        #endregion
    }
}