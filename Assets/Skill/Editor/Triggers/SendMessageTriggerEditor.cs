using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Skill.Editor.UI;
using Skill.Framework.UI;
using Skill.Framework.Triggers;

namespace Skill.Editor.Triggers
{
    [CustomEditor(typeof(SendMessageTrigger))]
    public class SendMessageTriggerEditor : UnityEditor.Editor
    {

        #region UI
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Framework.UI.StackPanel _Panel;

        private ObjectField<GameObject> _Receiver;
        private Skill.Editor.UI.TextField _FunctionName;
        private EnumPopup _MessageOptions;
        private EnumPopup _ParameterType;
        private FloatField _Float;
        private IntField _Int;
        private Skill.Editor.UI.ToggleButton _Boolean;
        private ObjectField<Object> _ObjectReference;
        private Skill.Editor.UI.TextField _String;

        private float _FrameHeight;

        private void CreateUI()
        {

            _Frame = new Skill.Framework.UI.Frame("Frame");
            _Panel = new Skill.Framework.UI.StackPanel();
            _Frame.Grid.Controls.Add(_Panel);

            _FrameHeight = 2;
            Thickness margin = new Thickness(2);

            _Receiver = new ObjectField<GameObject>() { Margin = margin }; _Receiver.Label.text = "Receiver";
            _Panel.Controls.Add(_Receiver);
            _FrameHeight += _Receiver.Height + margin.Vertical;

            _FunctionName = new Skill.Editor.UI.TextField() { Margin = margin }; _FunctionName.Label.text = "Function";
            _Panel.Controls.Add(_FunctionName);
            _FrameHeight += _FunctionName.Height + margin.Vertical;


            _MessageOptions = new EnumPopup() { Margin = margin }; _MessageOptions.Label.text = "Message Options";
            _Panel.Controls.Add(_MessageOptions);
            _FrameHeight += _MessageOptions.Height + margin.Vertical;

            _ParameterType = new EnumPopup() { Margin = margin }; _ParameterType.Label.text = "Parameter Type";
            _Panel.Controls.Add(_ParameterType);
            _FrameHeight += _ParameterType.Height + margin.Vertical;


            _Float = new FloatField() { Margin = margin }; _Float.Label.text = "Float";
            _Panel.Controls.Add(_Float);
            _FrameHeight += _Float.Height + margin.Vertical;

            _Int = new IntField() { Margin = margin }; _Int.Label.text = "Float";
            _Panel.Controls.Add(_Int);
            _FrameHeight += _Int.Height + margin.Vertical;

            _Boolean = new Skill.Editor.UI.ToggleButton() { Margin = margin, Left = true }; _Boolean.Label.text = "Boolean";
            _Panel.Controls.Add(_Boolean);
            _FrameHeight += _Boolean.Height + margin.Vertical;

            _ObjectReference = new ObjectField<Object>() { Margin = margin }; _ObjectReference.Label.text = "Object";
            _Panel.Controls.Add(_ObjectReference);
            _FrameHeight += _ObjectReference.Height + margin.Vertical;

            _String = new Skill.Editor.UI.TextField() { Margin = margin }; _String.Label.text = "String";
            _Panel.Controls.Add(_String);
            _FrameHeight += _String.Height + margin.Vertical;


            _Receiver.Object = _Data.Receiver;
            _FunctionName.Text = _Data.FunctionName;
            _MessageOptions.Value = _Data.MessageOptions;
            _ParameterType.Value = _Data.ParameterType;
            _Float.Value = _Data.Float;
            _Int.Value = _Data.Int;
            _Boolean.IsChecked = _Data.Boolean;
            _ObjectReference.Object = _Data.ObjectReference;
            _String.Text = _Data.String;

            UpdateVisibility();

            _Receiver.ObjectChanged += _Receiver_ObjectChanged;
            _FunctionName.TextChanged += _FunctionName_TextChanged;
            _MessageOptions.ValueChanged += _MessageOptions_ValueChanged;
            _ParameterType.ValueChanged += _ParameterType_ValueChanged;
            _Float.ValueChanged += _Float_ValueChanged;
            _Int.ValueChanged += _Int_ValueChanged;
            _Boolean.Changed += _Boolean_Changed;
            _ObjectReference.ObjectChanged += _ObjectReference_ObjectChanged;
            _String.TextChanged += _String_TextChanged;

        }

        void _Receiver_ObjectChanged(object sender, System.EventArgs e)
        {
            _Data.Receiver = _Receiver.Object;
            UnityEditor.EditorUtility.SetDirty(_Data);
        }

        void _FunctionName_TextChanged(object sender, System.EventArgs e)
        {
            _Data.FunctionName = _FunctionName.Text;
            UnityEditor.EditorUtility.SetDirty(_Data);
        }

        void _MessageOptions_ValueChanged(object sender, System.EventArgs e)
        {
            _Data.MessageOptions = (SendMessageOptions)_MessageOptions.Value;
            UnityEditor.EditorUtility.SetDirty(_Data);
        }

        void _ParameterType_ValueChanged(object sender, System.EventArgs e)
        {
            _Data.ParameterType = (Skill.Framework.Sequence.SendMessageParameter)_ParameterType.Value;
            UnityEditor.EditorUtility.SetDirty(_Data);
            UpdateVisibility();
        }

        void _Float_ValueChanged(object sender, System.EventArgs e)
        {
            _Data.Float = _Float.Value;
            UnityEditor.EditorUtility.SetDirty(_Data);
        }

        void _Int_ValueChanged(object sender, System.EventArgs e)
        {
            _Data.Int = _Int.Value;
            UnityEditor.EditorUtility.SetDirty(_Data);
        }

        void _Boolean_Changed(object sender, System.EventArgs e)
        {
            _Data.Boolean = _Boolean.IsChecked;
            UnityEditor.EditorUtility.SetDirty(_Data);
        }

        void _ObjectReference_ObjectChanged(object sender, System.EventArgs e)
        {
            _Data.ObjectReference = _ObjectReference.Object;
            UnityEditor.EditorUtility.SetDirty(_Data);
        }

        void _String_TextChanged(object sender, System.EventArgs e)
        {
            _Data.String = _String.Text;
            UnityEditor.EditorUtility.SetDirty(_Data);
        }

        private void UpdateVisibility()
        {
            _Float.Visibility = (_Data.ParameterType == Skill.Framework.Sequence.SendMessageParameter.Float) ? Visibility.Visible : Visibility.Collapsed;
            _Int.Visibility = (_Data.ParameterType == Skill.Framework.Sequence.SendMessageParameter.Int) ? Visibility.Visible : Visibility.Collapsed;
            _Boolean.Visibility = (_Data.ParameterType == Skill.Framework.Sequence.SendMessageParameter.Bool) ? Visibility.Visible : Visibility.Collapsed;
            _ObjectReference.Visibility = (_Data.ParameterType == Skill.Framework.Sequence.SendMessageParameter.ObjectReference) ? Visibility.Visible : Visibility.Collapsed;
            _String.Visibility = (_Data.ParameterType == Skill.Framework.Sequence.SendMessageParameter.String) ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion


        private SendMessageTrigger _Data;

        void OnEnable()
        {
            _Data = target as SendMessageTrigger;
            CreateUI();
        }

        public override void OnInspectorGUI()
        {
            _Frame.OnInspectorGUI(_FrameHeight);
        }
    }
}