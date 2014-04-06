using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using System.Collections.Generic;
using Skill.Framework.Modules;


namespace Skill.Editor.Tools
{
    [CustomEditor(typeof(ChildCounter))]
    public class ChildCounterEditor : UnityEditor.Editor
    {
        #region UI
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Framework.UI.StackPanel _Panel;
        private Button _BtnUpdate;
        private LabelField _LblCount;

        private void CreateUI()
        {
            _Frame = new Skill.Framework.UI.Frame("Frame");
            _Panel = new Skill.Framework.UI.StackPanel();
            _BtnUpdate = new Button() { Height = 20, Margin = new Skill.Framework.UI.Thickness(0, 0, 0, 4) }; _BtnUpdate.Content.text = "Count";
            _LblCount = new LabelField(); _LblCount.Label.text = "Count";
            _LblCount.Label2.text = _ChildCounter.Count.ToString();

            _Panel.Controls.Add(_LblCount);
            _Panel.Controls.Add(_BtnUpdate);

            _Frame.Grid.Controls.Add(_Panel);
            _BtnUpdate.Click += _BtnUpdate_Click;
        }

        void _BtnUpdate_Click(object sender, System.EventArgs e)
        {
            Count();
        }
        #endregion

        private ChildCounter _ChildCounter;

        void OnEnable()
        {
            _ChildCounter = target as ChildCounter;
            CreateUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _Frame.OnInspectorGUI(40);
        }

        private void Count()
        {
            _ChildCounter.Count = _ChildCounter.transform.childCount;
            EditorUtility.SetDirty(_ChildCounter);
            _LblCount.Label2.text = _ChildCounter.Count.ToString();
        }
    }
}