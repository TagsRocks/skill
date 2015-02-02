using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using Skill.Editor.Animation;
using System.Collections.Generic;

namespace Skill.Editor.Animation
{
    [UnityEditor.CustomEditor(typeof(AnimationTreeAsset))]
    public class AnimationTreeEditor : UnityEditor.Editor
    {
        private AnimationTreeAsset _Data;
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Framework.UI.Button _BtnEdit;
        private Skill.Framework.UI.Button _BtnBuild;        
        private Skill.Editor.UI.TextField _BuildPathField;
        void OnEnable()
        {
            _Data = target as AnimationTreeAsset;
            CreateUI();
        }

        private void CreateUI()
        {
            _Frame = new Skill.Framework.UI.Frame("Frame");
            _Frame.Grid.RowDefinitions.Add(30, Skill.Framework.UI.GridUnitType.Pixel); // _BtnEdit            
            _Frame.Grid.RowDefinitions.Add(30, Skill.Framework.UI.GridUnitType.Pixel); // _BtnBuild
            _Frame.Grid.RowDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel); // _BuildPathField            

            _BtnEdit = new Skill.Framework.UI.Button() { Row = 0, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _BtnEdit.Content.text = "Edit";
            _Frame.Controls.Add(_BtnEdit);

            _BtnBuild = new Skill.Framework.UI.Button() { Row = 1, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _BtnBuild.Content.text = "Build";
            _Frame.Controls.Add(_BtnBuild);            

            _BuildPathField = new TextField() { Row = 2, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _BuildPathField.Text = _Data.BuildPath;
            _BuildPathField.Label.text = "Path";
            _Frame.Controls.Add(_BuildPathField);


            _BtnBuild.Click += _BtnBuild_Click;
            _BtnEdit.Click += _BtnEdit_Click;
            _BuildPathField.TextChanged += _BuildPathField_TextChanged;
        }

        void _BuildPathField_TextChanged(object sender, System.EventArgs e)
        {
            _Data.BuildPath = _BuildPathField.Text;
            EditorUtility.SetDirty(_Data);
        }

        void _BtnBuild_Click(object sender, System.EventArgs e)
        {
            Build();
        }

        void _BtnEdit_Click(object sender, System.EventArgs e)
        {
            AnimationTreeEditorWindow.Instance.Show();
            AnimationTreeEditorWindow.Instance.Asset = _Data;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _Frame.Update();
            _Frame.OnInspectorGUI(80);
        }


        private void Build()
        {
            if (_Data.SkinMesh == null)
            {
                Debug.LogError("SkinMesh of AnimationTree is unasigned.");
                return;
            }

            SkinMeshData skin = _Data.SkinMesh.Load();
            if (skin != null)
            {
                AnimationTreeData data = _Data.Load();
                if (data != null)
                {
                    bool compiled = AnimationTreeCompiler.Compile(data, skin);
                    if (compiled)
                        Builder.Build(data, _Data.BuildPath, _Data.name);
                }
            }
        }
    }
}
