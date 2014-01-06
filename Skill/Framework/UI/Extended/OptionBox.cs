using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI.Extended
{
    /// <summary>
    /// An extended control that contains two buttons as left and right button and a label in between to show content.
    /// an example of usage is to let player select Resolution, Shadow Quality, Texture Quality, ...
    /// </summary>
    public class OptionBox : Grid, IFocusable
    {
        // content to show
        private GUIContent[] _Options;

        /// <summary> Border and Background to use when OptionBox is focused.</summary>                
        public Box FocusedBackground { get; private set; }
        /// <summary> The box that use as background  </summary>
        public Box Background { get; private set; }
        /// <summary> Left Button to select previous option </summary>
        public Button LeftButton { get; private set; }
        /// <summary> Right Button to select next option  </summary>
        public Button RightButton { get; private set; }
        /// <summary> Middle lable to show contents </summary>
        public Label OptionLabel { get; private set; }

        /// <summary>
        /// Is background box fill entire control of just in the middle of control( behind OptionLabel)
        /// </summary>
        public bool FillBackground
        {
            get { return Background.ColumnSpan == 3 && Background.Column == 0; }
            set
            {
                if (value != (Background.ColumnSpan == 3 && Background.Column == 0))
                {
                    if (value)
                    {
                        Background.ColumnSpan = 3;
                        Background.Column = 0;

                    }
                    else
                    {
                        Background.ColumnSpan = 1;
                        Background.Column = 1;
                    }
                    OnLayoutChanged();
                }
            }
        }

        /// <summary> Whether cycle through options</summary>
        public bool Loop { get; set; }

        /// <summary> Options </summary>
        public GUIContent[] Options { get { return _Options; } }

        private int _SelectedIndex;
        /// <summary> Gets or sets index of selected option </summary>
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (value < 0) value = 0;
                else if (value >= _Options.Length) value = _Options.Length - 1;
                if (_SelectedIndex != value)
                {
                    _SelectedIndex = value;
                    UpdateLabel();
                    OnSelectionChanged();
                }
            }
        }

        private void UpdateLabel()
        {
            GUIContent gc = _Options[_SelectedIndex];
            OptionLabel.Content.image = gc.image;
            OptionLabel.Content.text = gc.text;
            OptionLabel.Content.tooltip = gc.tooltip;
        }

        /// <summary> Gets or sets selected option </summary>
        public GUIContent SelectedOption
        {
            get { return _Options[_SelectedIndex]; }
            set
            {
                if (value == null) throw new ArgumentNullException("Invalid option. SelectedOption can not be null");
                int selectedIndex = -1;
                for (int i = 0; i < _Options.Length; i++)
                {
                    if (_Options[i] == value)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
                if (selectedIndex == -1) throw new ArgumentNullException("Invalid option. Option does not exist");
                SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// Occurs when SelectedOption changed
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Occurs when SelectedOption changed
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// What is the percentage of button width relative to control(0 - 50)- default is 30        
        /// </summary>
        /// <remarks>
        /// Percent of OptionLabel : 100 - (ButtonWidthPercent * 2)
        /// </remarks>
        public float ButtonWidthPercent
        {
            get { return this.ColumnDefinitions[0].Width.Value; }
            set
            {
                if (value < 0) value = 0;
                else if (value > 50) value = 50;
                this.ColumnDefinitions[0].Width = this.ColumnDefinitions[2].Width = new GridLength(value, GridUnitType.Star);
                this.ColumnDefinitions[1].Width = new GridLength(100 - (value * 2), GridUnitType.Star);
            }
        }

        /// <summary>
        /// Create an OptionBox
        /// </summary>
        /// <param name="options">Options as array of string</param>
        public OptionBox(string[] options)
            : this()
        {
            if (options == null || options.Length < 2)
                throw new ArgumentException("Invalid options");
            this._Options = new GUIContent[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                this._Options[i] = new GUIContent() { text = options[i] };
            }
            UpdateLabel();
        }

        /// <summary>
        /// Create an OptionBox
        /// </summary>
        /// <param name="options">Options as array of GUIContent</param>
        public OptionBox(GUIContent[] options)
            : this()
        {
            if (options == null || options.Length < 2)
                throw new ArgumentException("Invalid options");
            this._Options = options;
            UpdateLabel();
        }

        private OptionBox()
        {
            this.Height = 32;
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Star) });
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });

            Background = new Box() { Row = 0, Column = 0, ColumnSpan = 3 };
            FocusedBackground = new Box() { Row = 0, Column = 0, ColumnSpan = 3 , Visibility = UI.Visibility.Hidden };
            LeftButton = new Button() { Row = 0, Column = 0 };
            OptionLabel = new Label() { Row = 0, Column = 1 };
            RightButton = new Button() { Row = 0, Column = 2 };

            this.Controls.Add(Background);
            this.Controls.Add(FocusedBackground);
            this.Controls.Add(LeftButton);
            this.Controls.Add(OptionLabel);
            this.Controls.Add(RightButton);

            LeftButton.Click += _BtnLeft_Click;
            RightButton.Click += _BtnRight_Click;
        }

        private void SelectLeftOption()
        {
            if (_SelectedIndex > 0)
            {
                SelectedIndex--;
            }
            else if (Loop)
            {
                SelectedIndex = _Options.Length - 1;
            }
        }
        private void SelectRightOption()
        {
            if (_SelectedIndex < _Options.Length - 1)
            {
                SelectedIndex++;
            }
            else if (Loop)
            {
                SelectedIndex = 0;
            }
        }

        void _BtnRight_Click(object sender, EventArgs e)
        {
            SelectRightOption();
        }

        void _BtnLeft_Click(object sender, EventArgs e)
        {
            SelectLeftOption();
        }

        #region HandleCommand
        /// <summary>
        /// Handle specified command. (Up and Down command to switch selected index)
        /// </summary>
        /// <param name="command">Command to handle</param>
        /// <returns>True if command is handled, otherwise false</returns>        
        public override bool HandleCommand(UICommand command)
        {
            if (command.Key == KeyCommand.Left)
            {
                SelectLeftOption();
                return true;
            }
            else if (command.Key == KeyCommand.Right)
            {
                SelectRightOption();
                return true;
            }
            return false;
        }
        #endregion

        #region IFocusable members

        /// <summary> Tab index of control. </summary>
        public int TabIndex { get; set; }

        private bool _IsFocused;
        /// <summary>
        /// Gets a value that determines whether this element has logical focus. (You must set valid name to enable this behavior)
        /// </summary>
        /// <returns>
        /// true if this element has logical focus; otherwise, false.(You must set valid name to enable this behavior)
        /// </returns>
        /// <remarks>
        /// Set used for internal use
        /// </remarks>
        public bool IsFocused
        {
            get { return _IsFocused; }
            set
            {
                if (_IsFocused != value)
                {
                    _IsFocused = value;
                    if (_IsFocused)
                        OnGotFocus();
                    else
                        OnLostFocus();
                }
            }
        }

        /// <summary> Disable focusable - sometimes in editor it is better to disable focusable </summary>
        public void DisableFocusable() { _IsFocusable = false; }
        /// <summary> Enable focusable </summary>
        public void EnableFocusable() { _IsFocusable = true; }

        private bool _IsFocusable = true;
        /// <summary>
        /// Indicates whether the element can receive focus.(You must set valid name to enable this behavior)
        /// </summary>
        public override bool IsFocusable { get { return _IsFocusable; } }

        /// <summary> it is an extended focusable </summary>
        public bool IsExtendedFocusable { get { return true; } }

        /// <summary>
        /// Occurs when this element gets logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        public event EventHandler GotFocus;
        /// <summary>
        /// when this element gets logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        protected virtual void OnGotFocus()
        {
            FocusedBackground.Visibility = UI.Visibility.Visible;
            if (GotFocus != null) GotFocus(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when this element loses logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        public event EventHandler LostFocus;
        /// <summary>
        /// when this element loses logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        protected virtual void OnLostFocus()
        {
            FocusedBackground.Visibility = UI.Visibility.Hidden;
            if (LostFocus != null) LostFocus(this, EventArgs.Empty);
        }

        /// <summary> Try focuse control </summary>
        public void Focus()
        {
            if (OwnerFrame != null)
                OwnerFrame.FocusControl(this);
        }
        #endregion
    }
}
