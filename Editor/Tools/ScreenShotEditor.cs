using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using Skill.Framework.Modules;

namespace Skill.Editor.Tools
{
    [CustomEditor(typeof(ScreenShot))]
    public class ScreenShotEditor : UnityEditor.Editor
    {
        private const int FrameHeight = 50;

        private Skill.Framework.UI.Frame _Frame;
        private Skill.Editor.UI.ToggleButton _BtnCustomSize;
        private Skill.Editor.UI.IntField _IFieldWidth;
        private Skill.Editor.UI.IntField _IFieldHeight;
        private Skill.Editor.UI.FloatField _FFieldScale;

        private ScreenShot _ScreenShot;

        void OnEnable()
        {
            _ScreenShot = base.serializedObject.targetObject as ScreenShot;

            _Frame = new Skill.Framework.UI.Frame("Frame");
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            _BtnCustomSize = new Skill.Editor.UI.ToggleButton() { Row = 0, Column = 0, IsChecked = _ScreenShot.CustomSize };
            _BtnCustomSize.Label.text = "Custom size";

            Skill.Framework.UI.Thickness margin = new Skill.Framework.UI.Thickness(20, 0, 0, 0);

            _IFieldWidth = new Skill.Editor.UI.IntField() { Row = 1, Column = 0, Value = _ScreenShot.Width, Margin = margin };
            _IFieldWidth.Label.text = "Width";

            _IFieldHeight = new Skill.Editor.UI.IntField() { Row = 2, Column = 0, Value = _ScreenShot.Height, Margin = margin };
            _IFieldHeight.Label.text = "Height";

            _FFieldScale = new Skill.Editor.UI.FloatField() { Row = 1, Column = 0, Value = _ScreenShot.Scale, Margin = margin };
            _FFieldScale.Label.text = "Scale";

            _Frame.Grid.Controls.Add(_BtnCustomSize);
            _Frame.Grid.Controls.Add(_IFieldWidth);
            _Frame.Grid.Controls.Add(_IFieldHeight);
            _Frame.Grid.Controls.Add(_FFieldScale);

            _BtnCustomSize.Changed += _BtnCustomSize_Changed;
            _IFieldWidth.ValueChanged += _Width_ValueChanged;
            _IFieldHeight.ValueChanged += _Height_ValueChanged;
            _FFieldScale.ValueChanged += _Scale_ValueChanged;

            ManageControls();
        }

        void _Scale_ValueChanged(object sender, System.EventArgs e)
        {
            _ScreenShot.Scale = _FFieldScale.Value;
        }

        void _Height_ValueChanged(object sender, System.EventArgs e)
        {
            _ScreenShot.Height = _IFieldHeight.Value;
        }

        void _Width_ValueChanged(object sender, System.EventArgs e)
        {
            _ScreenShot.Width = _IFieldWidth.Value;
        }

        private void ManageControls()
        {
            _IFieldHeight.Visibility = _IFieldWidth.Visibility = _BtnCustomSize.IsChecked ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;
            _FFieldScale.Visibility = _BtnCustomSize.IsChecked ? Skill.Framework.UI.Visibility.Hidden : Skill.Framework.UI.Visibility.Visible;
        }

        void _BtnCustomSize_Changed(object sender, System.EventArgs e)
        {
            ManageControls();
            _ScreenShot.CustomSize = _BtnCustomSize.IsChecked;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _Frame.OnInspectorGUI(FrameHeight);
        }
    }
}
