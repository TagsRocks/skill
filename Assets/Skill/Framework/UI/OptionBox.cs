using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// An extended control that contains two buttons as left and right button and a label in between to show content.
    /// an example of usage is to let player select Resolution, Shadow Quality, Texture Quality, ...
    /// </summary>
    public class OptionBox : Grid, IFocusable
    {
        // content to show
        private GUIContent[] _Options;

        public Orientation Orientation { get; private set; }

        /// <summary> Border and Background to use when OptionBox is focused.</summary>                
        public Box FocusedBackground { get; private set; }
        /// <summary> The box that use as background  </summary>
        public Box Background { get; private set; }
        /// <summary> Left or down Button to select previous option </summary>
        public Button DownButton { get; private set; }
        /// <summary> Right or up Button to select next option  </summary>
        public Button UpButton { get; private set; }
        /// <summary> Middle lable to show contents </summary>
        public Label OptionLabel { get; private set; }

        /// <summary>
        /// Is background box fill entire control of just in the middle of control( behind OptionLabel)
        /// </summary>
        public bool FillBackground
        {
            get
            {
                if (Orientation == UI.Orientation.Horizontal)
                    return Background.ColumnSpan == 3 && Background.Column == 0;
                else
                    return Background.RowSpan == 3 && Background.Row == 0;
            }
            set
            {
                if (Orientation == UI.Orientation.Horizontal)
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
                else
                {
                    if (value != (Background.RowSpan == 3 && Background.Row == 0))
                    {
                        if (value)
                        {
                            Background.RowSpan = 3;
                            Background.Row = 0;

                        }
                        else
                        {
                            Background.RowSpan = 1;
                            Background.Row = 1;
                        }
                        OnLayoutChanged();
                    }
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
        public float ButtonPercent
        {
            get
            {
                if (Orientation == UI.Orientation.Horizontal)
                    return this.ColumnDefinitions[0].Width.Value;
                else
                    return this.RowDefinitions[0].Height.Value;
            }
            set
            {
                if (value < 0) value = 0;
                else if (value > 50) value = 50;

                if (Orientation == UI.Orientation.Horizontal)
                {
                    this.ColumnDefinitions[0].Width = this.ColumnDefinitions[2].Width = new GridLength(value, GridUnitType.Star);
                    this.ColumnDefinitions[1].Width = new GridLength(100 - (value * 2), GridUnitType.Star);
                }
                else
                {
                    this.RowDefinitions[0].Height = this.RowDefinitions[2].Height = new GridLength(value, GridUnitType.Star);
                    this.RowDefinitions[1].Height = new GridLength(100 - (value * 2), GridUnitType.Star);
                }
            }
        }

        /// <summary>
        /// Create an OptionBox
        /// </summary>
        /// <param name="options">Options as array of string</param>
        public OptionBox(Orientation orientation, params string[] options)
            : this(orientation)
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
        public OptionBox(Orientation orientation, GUIContent[] options)
            : this(orientation)
        {
            if (options == null || options.Length < 2)
                throw new ArgumentException("Invalid options");
            this._Options = options;
            UpdateLabel();
        }

        private OptionBox(Orientation orientation)
        {
            this.Orientation = orientation;

            Background = new Box() { Row = 0, Column = 0 };
            FocusedBackground = new Box() { Row = 0, RowSpan = 3, Column = 0, ColumnSpan = 3, Visibility = UI.Visibility.Hidden };
            DownButton = new Button();
            OptionLabel = new Label();
            UpButton = new Button();

            if (orientation == UI.Orientation.Horizontal)
            {
                this.Height = 32;
                this.ColumnDefinitions.Add(30, GridUnitType.Star);
                this.ColumnDefinitions.Add(40, GridUnitType.Star);
                this.ColumnDefinitions.Add(30, GridUnitType.Star);

                Background.ColumnSpan = 3;
                DownButton.Row = 0; DownButton.Column = 0;
                OptionLabel.Row = 0; OptionLabel.Column = 1;
                UpButton.Row = 0; UpButton.Column = 2;
            }
            else
            {
                this.Width = 32;
                this.RowDefinitions.Add(30, GridUnitType.Star);
                this.RowDefinitions.Add(40, GridUnitType.Star);
                this.RowDefinitions.Add(30, GridUnitType.Star);

                Background.RowSpan = 3;
                DownButton.Row = 2; DownButton.Column = 0;
                OptionLabel.Row = 1; Column = 0;
                UpButton.Row = 0; Column = 0;
            }


            this.Controls.Add(Background);
            this.Controls.Add(FocusedBackground);
            this.Controls.Add(DownButton);
            this.Controls.Add(OptionLabel);
            this.Controls.Add(UpButton);

            DownButton.Click += _BtnLeft_Click;
            UpButton.Click += _BtnRight_Click;
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
