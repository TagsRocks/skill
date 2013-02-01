using System;
using System.Collections.Generic;
using UnityEngine;


namespace Skill.Framework.UI
{

    /// <summary>
    /// Frame is a content control that supports navigation.
    /// </summary>
    public class Frame
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this element is enabled in the user interface (UI).
        /// </summary>
        /// <returns>  true if the element is enabled; otherwise, false. The default value is true. </returns>
        public virtual bool IsEnabled { get { return Grid.IsEnabled; } set { Grid.IsEnabled = value; } }

        private Rect _Position;
        /// <summary>
        /// Position of control relative to parent
        /// </summary>
        public Rect Position
        {
            get { return _Position; }
            set
            {
                if (_Position != value)
                {
                    _Position = value;
                    OnPositionChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets Position.X
        /// </summary>
        public float X
        {
            get
            {
                return _Position.x;
            }
            set
            {
                if (_Position.x != value)
                {
                    _Position.x = value;
                    OnPositionChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets Position.Y
        /// </summary>
        public float Y
        {
            get
            {
                return _Position.y;
            }
            set
            {
                if (_Position.y != value)
                {
                    _Position.y = value;
                    OnPositionChange();
                }
            }
        }


        /// <summary>
        /// Gets or sets the suggested width of the element
        /// </summary>
        public float Width
        {
            get
            {
                return _Position.width;
            }
            set
            {
                if (_Position.width != value)
                {
                    _Position.width = value;
                    OnPositionChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the suggested height of the element.
        /// </summary>
        public float Height
        {
            get
            {
                return _Position.height;
            }
            set
            {
                if (_Position.height != value)
                {
                    _Position.height = value;
                    OnPositionChange();
                }
            }
        }

        /// <summary>
        /// Retrieves Name of Frame.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Grid of Frame.
        /// </summary>
        public Grid Grid { get; private set; }

        private FocusableControl _FocusedControl;
        /// <summary>
        /// Get focused FocusableControl in last update or null if no FocusableControl got focus
        /// </summary>
        public FocusableControl FocusedControl
        {
            get { return _FocusedControl; }
            private set
            {
                if (_FocusedControl != value)
                {
                    if (_FocusedControl != null)
                        _FocusedControl.IsFocused = false;
                    _FocusedControl = value;
                    if (_FocusedControl != null)
                        _FocusedControl.IsFocused = true;                    
                }
            }
        }

        /// <summary>
        /// Retrieves parent menu of frame
        /// </summary>
        public Menu Menu { get; internal set; }

        /// <summary>
        /// Result of frame when used as a dialog. A dialog will continue modal until this value be somthing except 'None'
        /// </summary>
        public DialogResult DialogResult { get; protected set; }

        private string _FocusedRequest = null;
        /// <summary>
        /// Move keyboard focus to a named control.
        /// </summary>
        /// <param name="controlName">Name of focusable control</param>
        public void FocusControl(string controlName)
        {
            Control fc = Grid.FindControlByName(controlName);
            if (fc != null)
                _FocusedRequest = controlName;
            else
                _FocusedRequest = null;
        }

        /// <summary>
        /// Move keyboard focus to a named control.
        /// </summary>
        /// <param name="control">focusable control</param>
        public void FocusControl(FocusableControl control)
        {
            if (string.IsNullOrEmpty(control.Name))
                throw new InvalidOperationException("Can not focus unnamed control");
            Control fc = Grid.FindControlByName(control.Name);
            if (fc != null)
                _FocusedRequest = control.Name;
            else
                _FocusedRequest = null;

        }

        /// <summary>
        /// remove keyboard focus.
        /// </summary>        
        public void UnFocusControls()
        {
            FocusedControl = null;
        }

        /// <summary>
        /// Move focuced control to next tab index.
        /// </summary>
        /// <remarks>
        /// Set sequential tab index for controls inside a frame to make this method work correctly
        /// </remarks>
        public void NextTab()
        {
            FocusableControl fc = null;
            if (_FocusedControl != null)
                fc = Grid.FindControlByTabIndex(_FocusedControl.TabIndex + 1);
            if (fc == null)
            {
                uint ti = uint.MaxValue;
                fc = Grid.FindControlByMinTabIndex(ref ti);
            }
            if (fc != null)
                FocusControl(fc);
        }

        /// <summary>
        /// Move focuced control to previous tab index.
        /// </summary>
        /// <remarks>
        /// Set sequential tab index for controls inside a frame to make this method work correctly
        /// </remarks>
        public void PreviousTab()
        {
            FocusableControl fc = null;
            if (_FocusedControl != null && _FocusedControl.TabIndex > 0)
                fc = Grid.FindControlByTabIndex(_FocusedControl.TabIndex - 1);
            if (fc == null)
            {
                uint ti = uint.MinValue;
                fc = Grid.FindControlByMaxTabIndex(ref ti);
            }
            if (fc != null)
                FocusControl(fc);
        }        
        #endregion

        #region Events
        /// <summary> Occurs when position of control changed </summary>
        public event EventHandler PositionChange;
        /// <summary>
        /// when position of control changed
        /// </summary>
        protected virtual void OnPositionChange()
        {
            if (PositionChange != null)
                PositionChange(this, EventArgs.Empty);
        }

        /// <summary>
        /// Is any key events hooked?
        /// </summary>
        /// <returns>True if hooked, otherwise false</returns>
        private bool IsAnyKeyEventHooked()
        {
            return KeyDown != null || KeyUp != null;
        }

        /// <summary>
        /// Check for events
        /// </summary>
        private void CheckEvents()
        {
            if (IsAnyKeyEventHooked())
            {
                Event e = Event.current;
                if (e != null && e.isKey && e.keyCode != KeyCode.None)
                {
                    EventType type = e.type;
                    if (type == EventType.keyDown || type == EventType.KeyUp)
                    {
                        KeyEventArgs args = new KeyEventArgs(e.keyCode, e.character);
                        if (type == EventType.keyDown)
                            OnKeyDown(args);
                        else
                            OnKeyUp(args);
                        if (args.Handled)
                            e.Use();
                    }
                }
            }
        }

        /// <summary> Occurs when a keyboard key was pressed.() </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when a keyboard key was pressed.(if KeyDown hooked )
        /// </summary>
        /// <param name="args"> KeyEventArgs </param>
        protected virtual void OnKeyDown(KeyEventArgs args)
        {
            if (KeyDown != null)
                KeyDown(this, args);
        }

        /// <summary> Occurs when a keyboard key was released. </summary>
        public event KeyEventHandler KeyUp;
        /// <summary>
        /// Occurs when a keyboard key was released.(if KeyUp hooked )
        /// </summary>
        /// <param name="args"> KeyEventArgs </param>
        protected virtual void OnKeyUp(KeyEventArgs args)
        {
            if (KeyUp != null)
                KeyUp(this, args);
        }
        #endregion

        #region OnGUI

        /// <summary>
        /// Is grid rendered in local space of Frame.
        /// </summary>
        protected virtual bool LocalGrid { get { return false; } }

        /// <summary>
        /// to render control you have to call this method in OnGUI method of MonoBehavior.(call this for Frame class)
        /// </summary>
        public virtual void OnGUI()
        {
            Rect rect = Position;

            float x = 0;
            float y = 0;

            if (!LocalGrid)
            {
                x = rect.x;
                y = rect.y;
            }

            Thickness margin = Grid.Margin;

            rect.x = margin.Left;
            rect.y = margin.Top;
            rect.width -= margin.Horizontal;
            rect.height -= margin.Vertical;
            Grid.Position = rect;

            rect.x += x;
            rect.y += y;
            Grid.RenderArea = rect;

            CheckEvents();

            if (_FocusedRequest != null)
            {
                GUI.FocusControl(_FocusedRequest);
                _FocusedRequest = null;
            }
            else if (_FocusedControl != null)
                GUI.FocusControl(_FocusedControl.Name);
            else
                GUI.FocusControl(string.Empty);

            DrawControls();            

            string focusedControlName = GUI.GetNameOfFocusedControl();
            if (!string.IsNullOrEmpty(focusedControlName))
            {
                if (FocusedControl == null || FocusedControl.Name != focusedControlName)
                {
                    Control c = Grid.FindControlByName(focusedControlName);
                    if (c != null && c.Focusable)
                        FocusedControl = (FocusableControl)c;
                    else
                        FocusedControl = null;
                }
            }
            else
                FocusedControl = null;

        }

        /// <summary>
        /// Call Grid.OnGUI() to draw controls
        /// </summary>
        protected virtual void DrawControls()
        {
            Grid.OnGUI();
        }

        #endregion

        #region Construcor
        /// <summary>
        /// Initializes a new instance of the Frame class.
        /// </summary>        
        /// <param name="name">Valid and unique name of frame</param>
        public Frame(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid name of Frame");

            this.Name = name;
            Grid = new Grid();
            this.Width = 200;
            this.Height = 200;
        }
        #endregion

        #region Enter
        /// <summary>
        /// Called when Frame is about to enter view stack
        /// </summary>
        /// <returns>Whehter enter is success or still needs time to success</returns>
        internal bool EnterFrame()
        {
            FocusedControl = null;
            this.DialogResult = UI.DialogResult.None;
            return OnEnter();
        }

        /// <summary>
        /// Called when Frame is about to enter view stack
        /// </summary>
        /// <returns>Whehter enter is success or still needs time to success</returns>
        /// <remarks>
        /// It is possible frame has animations or fades. keep return false until these operations succeed.
        /// </remarks>
        protected virtual bool OnEnter()
        {
            return true;
        }
        #endregion

        #region Leave
        /// <summary>
        /// Called when Frame is about to leave view stack
        /// </summary>        
        /// <param name="cancel">Cancel leave operation do to show a dialog, ... </param>
        /// <returns>Whehter leave is success or still needs time to success</returns>        
        internal bool LeaveFrame(ref bool cancel)
        {
            FocusedControl = null;
            return OnLeave(ref cancel);
        }

        /// <summary>
        /// Called when Frame is about to leave view stack
        /// </summary>        
        /// <param name="cancel">Cancel leave operation do to show a dialog, ... </param>
        /// <returns>Whehter leave is success or still needs time to success</returns>
        /// <remarks>
        /// It is possible frame has animations or fades. keep return false until these operations succeed.
        /// In some senarios like 'Settings menu' it is possible to cancel frame leave and show a dialog to ask for 'are you sure?'
        /// and get result back by 'HandleDialogMessage' method, then if user accepts, call 'Menu.Back()' again to continue frame leave.
        /// </remarks>
        protected virtual bool OnLeave(ref bool cancel)
        {
            return true;
        }
        #endregion

        #region Hide
        /// <summary>
        /// Called when Frame is in view stack and about to hide do to another frame pushed in view stack
        /// </summary>
        /// <returns>Whehter hide is success or still needs time to success</returns>
        internal bool HideFrame()
        {
            FocusedControl = null;
            this.DialogResult = UI.DialogResult.None;
            return OnHide();
        }

        /// <summary>
        /// Called when Frame is in view stack and about to hide do to another frame pushed in view stack
        /// </summary>
        /// <returns>Whehter hide is success or still needs time to success</returns>
        /// <remarks>
        /// It is possible frame has animations or fades. keep return false until these operations succeed.
        /// </remarks>
        protected virtual bool OnHide()
        {
            return true;
        }
        #endregion

        #region Show
        /// <summary>
        /// Called when Frame is in view stack and hide, when a higher Frame in view stack leaved and cause this Frame showed again
        /// </summary>
        /// <returns>Whehter show is success or still needs time to success</returns>
        internal bool ShowFrame()
        {
            this.DialogResult = UI.DialogResult.None;
            return OnShow();
        }

        /// <summary>
        /// Called when Frame is in view stack and hide, when a higher Frame in view stack leaved and cause this Frame showed again
        /// </summary>
        /// <returns>Whehter show is success or still needs time to success</returns>
        /// <remarks>
        /// It is possible frame has animations or fades. keep return false until these operations succeed.
        /// </remarks>
        protected virtual bool OnShow()
        {
            return true;
        }
        #endregion

        #region Handle Dialog
        /// <summary>
        /// Let Frame handle requested dialog
        /// </summary>
        /// <param name="dialog">The dialog requested by this frame</param>
        internal void HandleDialog(Frame dialog)
        {
            OnHandleDialog(dialog);
        }

        /// <summary>
        /// Let Frame handle requested dialog
        /// </summary>
        /// <param name="dialog">The dialog requested by this frame</param>
        protected virtual void OnHandleDialog(Frame dialog)
        {

        }
        #endregion

        #region Commands
        /// <summary>
        /// Handle specified command
        /// </summary>
        /// <param name="command">Command to handle</param>        
        /// <param name="handleTabIndex">whether to handle tab index</param>
        /// <returns>True if command is handled, otherwise false</returns>        
        /// <remarks>
        /// 1- This method first allow focused control (if exist) to handle command
        /// 2- if not let the control that is under mouse cursor to handle command
        /// 3- if not then go through controls and let each to handle command
        /// 4- if not and handleTabIndex is true : 
        ///     if command is Up   : PreviousTab()
        ///     if command is Down : NextTab()
        /// as soon as first control handled the command ignore next steps
        /// </remarks>        
        public virtual bool HandleCommand(UICommand command, bool handleTabIndex = true)
        {
            bool handled = false;
            if (_FocusedControl != null)
                handled = _FocusedControl.HandleCommand(command);
            if (!handled)
                handled = this.Grid.HandleCommand(command);
            if (!handled && handleTabIndex)
            {
                if (command.Key == KeyCommand.Up)
                {
                    PreviousTab();
                    handled = true;
                }
                else if (command.Key == KeyCommand.Down)
                {
                    NextTab();
                    handled = true;
                }
            }
            return handled;
        }
        #endregion
    }
}