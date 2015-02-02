using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Make a text field where the user can enter a password.
    /// </summary>
    public class PasswordField : FocusableControl
    {
        private string _Password;
        /// <summary>
        /// Password to edit. The return value of this function should be assigned back to the string as shown in the example.
        /// </summary>
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (_Password != value)
                {
                    _Password = value;
                    OnPasswordChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when password changed
        /// </summary>
        public event EventHandler PasswordChanged;
        /// <summary>
        /// when password changed
        /// </summary>
        protected virtual void OnPasswordChanged()
        {
            if (PasswordChanged != null) PasswordChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// The maximum length of the string. If left out, the user can type for ever and ever.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Character to mask the password with.(default '•')
        /// </summary>
        public char MaskChar { get; set; }

        /// <summary>
        /// Create an instance of PasswordField
        /// </summary>
        public PasswordField()
        {
            this._Password = string.Empty;
            this.MaxLength = 0;
            this.MaskChar = '•';
        }

        /// <summary>
        /// Render PasswordField
        /// </summary>
        protected override void Render()
        {
            string result;
            if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                if (MaxLength > 0)
                    result = GUI.PasswordField(RenderArea, _Password, MaskChar, MaxLength, Style);
                else
                    result = GUI.PasswordField(RenderArea, _Password, MaskChar, Style);
            }
            else
            {
                if (MaxLength > 0)
                    result = GUI.PasswordField(RenderArea, _Password, MaskChar, MaxLength);
                else
                    result = GUI.PasswordField(RenderArea, _Password, MaskChar);
            }

            Password = result;
        }        
    }
}