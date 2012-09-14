using UnityEngine;
using System.Collections;
using System;
namespace Skill.UI
{
    /// <summary>
    /// Make a Multi-line text area where the user can edit a string.
    /// </summary>
    public class TextArea : FocusableControl
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
        /// Occurs when text of TextArea changed
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
        /// Create an TextArea
        /// </summary>
        public TextArea()
        {
            this._Text = string.Empty;
            this.MaxLength = 0;
        }

        protected override void Paint(PaintParameters paintParams)
        {
            string result;
            if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                if (MaxLength > 0)
                    result = GUI.TextArea(PaintArea, _Text, MaxLength, Style);
                else
                    result = GUI.TextArea(PaintArea, _Text, Style);
            }
            else
            {
                if (MaxLength > 0)
                    result = GUI.TextArea(PaintArea, _Text, MaxLength);
                else
                    result = GUI.TextArea(PaintArea, _Text);
            }

            Text = result;
        }        
    }

}