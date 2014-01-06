using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Manage ToggleButtons and make sure only one of ToggleButtons is checked.
    /// </summary>
    public class ToggleButtonGroup
    {
        /// <summary>
        /// Make sure that one button is always checked (default true)
        /// </summary>
        public bool ForceChecked { get; set; }

        /// <summary> Gets or sets Name of group (optional) </summary>
        public string Name { get; set; }

        private List<IToggleButton> _Buttons;
        private bool _IsInChange;

        /// <summary>
        /// Create a ToggleButtonGroup
        /// </summary>
        public ToggleButtonGroup()
        {
            this.ForceChecked = true;
            _Buttons = new List<IToggleButton>();
        }

        /// <summary>
        /// Add a ToggleButton to group
        /// </summary>
        /// <param name="button">Button to add</param>
        public void Add(IToggleButton button)
        {
            if (_Buttons.Contains(button)) return;
            _Buttons.Add(button);
            button.Checked += button_Checked;
            button.Unchecked += button_Unchecked;
        }        

        /// <summary>
        /// Remove ToggleButton to group
        /// </summary>
        /// <param name="button">Button to remove</param>
        /// <returns>True if success otherwise false</returns>
        public bool Remove(IToggleButton button)
        {
            bool result = _Buttons.Remove(button);
            if (result)
            {
                button.Checked -= button_Checked;
                button.Unchecked -= button_Unchecked;
            }
            return result;
        }

        void button_Checked(object sender, EventArgs e)
        {
            if (_IsInChange) return; // avoid other buttons to reach this method
            _IsInChange = true;// lock
            foreach (var btn in _Buttons)
            {
                if (btn != sender)
                    btn.IsChecked = false;
            }
            _IsInChange = false;// unlock
        }

        void button_Unchecked(object sender, EventArgs e)
        {
            if (_IsInChange) return; // avoid other buttons to reach this method
            _IsInChange = true;// lock
            foreach (var btn in _Buttons)
            {
                if (btn == sender) // do not allow user to click again and uncheck button
                {
                    btn.IsChecked = true;
                    break;
                }
            }
            _IsInChange = false;// unlock
        }
    }
}
