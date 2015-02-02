using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a text field for entering ints.
    /// </summary>
    public class IntField : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// ValueChanged event occurs when user press Return
        /// </summary>
        public bool ChangeOnReturn { get; set; }

        /// <summary>
        /// Occurs when Value of IntField changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when Value of IntField changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private int _Value;
        private int _ValueBeforChange;
        /// <summary>
        /// int - The value entered by the user.
        /// </summary>
        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _ValueBeforChange = _Value = value;
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Create an instance of IntField
        /// </summary>
        public IntField()
        {
            Label = new GUIContent();
            this.Height = 16;
            this.ChangeOnReturn = false;
        }

        /// <summary>
        /// Render IntField
        /// </summary>
        protected override void Render()
        {
            if (Style != null)
            {
                if (ChangeOnReturn)
                    _ValueBeforChange = EditorGUI.IntField(RenderArea, Label, _ValueBeforChange, Style);
                else
                    Value = EditorGUI.IntField(RenderArea, Label, _Value, Style);
            }
            else
            {
                if (ChangeOnReturn)
                    _ValueBeforChange = EditorGUI.IntField(RenderArea, Label, _ValueBeforChange);
                else
                    Value = EditorGUI.IntField(RenderArea, Label, _Value);
            }

            if (ChangeOnReturn)
            {
                if (_Value != _ValueBeforChange)
                {
                    Event e = Event.current;
                    if (e != null && e.isKey)
                    {
                        if (e.keyCode == KeyCode.Return)
                        {
                            Value = _ValueBeforChange;
                        }
                        else if (e.keyCode == KeyCode.Escape)
                        {
                            _ValueBeforChange = _Value;
                        }
                    }
                }
            }
        }
    }
}
