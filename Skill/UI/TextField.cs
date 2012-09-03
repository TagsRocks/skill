using UnityEngine;
using System.Collections;
using System;
namespace Skill.UI
{
    /// <summary>
    /// Make a single-line text field where the user can edit a string.
    /// </summary>
    public class TextField : FocusableControl
    {
        private string _Text;
        /// <summary>
        /// Text to edit. The return value of this function should be assigned back to the string as shown in the example.
        /// </summary>
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (_Text != value)
                {
                    _Text = value;
                    OnTextChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when text of TextField changed
        /// </summary>
        public event EventHandler TextChanged;
        protected virtual void OnTextChanged()
        {
            if (TextChanged != null) TextChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// The maximum length of the string. If left out, the user can type for ever and ever.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Create an instance of TextField
        /// </summary>
        public TextField()
        {
            this._Text = string.Empty;
            this.MaxLength = 0;
        }

        protected override void Paint()
        {
            string result;

            if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);

            if (Style != null)
            {
                if (MaxLength > 0)
                    result = GUI.TextField(PaintArea, _Text, MaxLength, Style);
                else
                    result = GUI.TextField(PaintArea, _Text, Style);
            }
            else
            {
                if (MaxLength > 0)
                    result = GUI.TextField(PaintArea, _Text, MaxLength);
                else
                    result = GUI.TextField(PaintArea, _Text);
            }

            Text = result;
        }        
    }
}