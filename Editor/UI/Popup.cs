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
    public class Popup : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }
        /// <summary>
        /// An array with the options shown in the popup.
        /// </summary>
        public PopupOptionCollection Options { get; private set; }

        private int _SelectedIndex = -1;
        /// <summary>
        /// The index of the option the field shows.
        /// </summary>
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (_SelectedIndex != value)
                {
                    if (_SelectedIndex >= 0 && _SelectedIndex < Options.Count)
                        Options[_SelectedIndex].IsSelected = false;

                    _SelectedIndex = value;
                    OnOptionChanged();

                    if (_SelectedIndex >= 0 && _SelectedIndex < Options.Count)
                        Options[_SelectedIndex].IsSelected = true;
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
                if (_SelectedIndex >= 0 && _SelectedIndex < Options.Count)
                    return Options[_SelectedIndex];
                return null;
            }
            set
            {
                if (value == null)
                {
                    SelectedIndex = 0;
                }
                else
                {
                    SelectedIndex = Options.IndexOf(value);
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
        public Popup()
        {
            this.Options = new PopupOptionCollection();
            this.Label = new GUIContent();
            this.Height = 16;
        }

        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                SelectedIndex = EditorGUI.Popup(RenderArea, Label, _SelectedIndex, Options.Contents, Style);
            }
            else
            {
                SelectedIndex = EditorGUI.Popup(RenderArea, Label, _SelectedIndex, Options.Contents);
            }
        }        
    }
}
