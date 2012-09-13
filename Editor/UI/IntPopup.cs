using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.UI;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a generic popup selection field.
    /// </summary>
    public class IntPopup : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }
        /// <summary>
        /// An array with the options shown in the popup.
        /// </summary>
        public PopupOptionCollection Options { get; private set; }

        private int FindIndexByValue(int value)
        {
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].Value == value)
                    return i;
            }
            return -1;
        }

        private int _SelectedValue = -1;
        /// <summary>
        /// The value of the option the field shows.
        /// </summary>
        public int SelectedValue
        {
            get { return _SelectedValue; }
            set
            {
                if (_SelectedValue != value)
                {
                    int index = FindIndexByValue(_SelectedValue);
                    if (index >= 0 && index < Options.Count)
                        Options[index].IsSelected = false;

                    _SelectedValue = value;
                    OnOptionChanged();

                    index = FindIndexByValue(_SelectedValue);
                    if (index >= 0 && index < Options.Count)
                        Options[index].IsSelected = true;
                }
            }
        }

        /// <summary>
        /// The PopupOption of the option the field shows.
        /// </summary>
        public PopupOption SelectedOption
        {
            get
            {
                int index = FindIndexByValue(_SelectedValue);
                if (index >= 0 && index < Options.Count)
                    return Options[index];
                return null;
            }
            set
            {
                if (value != null)
                {
                    if (Options.Contains(value))
                        SelectedValue = value.Value;
                }
            }
        }

        /// <summary>
        /// Ocuurs when Option of Popup changed
        /// </summary>
        public event EventHandler OptionChanged;
        protected virtual void OnOptionChanged()
        {
            if (OptionChanged != null)
                OptionChanged(this, EventArgs.Empty);
        }
        /// <summary>
        /// Create an instace of Popup
        /// </summary>
        public IntPopup()
        {
            this.Options = new PopupOptionCollection();
            this.Label = new GUIContent();
            this.Height = 16;
        }

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                SelectedValue = EditorGUI.IntPopup(PaintArea, Label, _SelectedValue, Options.Contents, Options.Values, Style);
            }
            else
            {
                SelectedValue = EditorGUI.IntPopup(PaintArea, Label, _SelectedValue, Options.Contents, Options.Values);
            }
        }
    }
}
