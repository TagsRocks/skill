
using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using System.Collections.Generic;
using Skill.Framework.Audio;

namespace Skill.Editor.Audio
{
    [UnityEditor.CustomEditor(typeof(AudioController))]
    public class AnimationTreeEditor : UnityEditor.Editor
    {
        private AudioController _Controller;
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Framework.UI.Button _BtnEdit;
        void OnEnable()
        {
            _Controller = target as AudioController;
            CreateUI();
        }

        private void CreateUI()
        {
            _Frame = new Skill.Framework.UI.Frame("Frame");
            _Frame.Grid.RowDefinitions.Add(30, Skill.Framework.UI.GridUnitType.Pixel); // _BtnEdit                    

            _BtnEdit = new Skill.Framework.UI.Button() { Row = 0, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _BtnEdit.Content.text = "Edit";
            _Frame.Controls.Add(_BtnEdit);

            _BtnEdit.Click += _BtnEdit_Click;
        }

        void _BtnEdit_Click(object sender, System.EventArgs e)
        {
            AudioControllerEditorWindow.Instance.Show();
            AudioControllerEditorWindow.Instance.Controller = _Controller;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _Frame.Update();
            _Frame.OnInspectorGUI(32);
        }
    }
}


