using System;
using System.Collections.Generic;
using UnityEngine;


namespace Skill.Framework.UI
{

    /// <summary>
    /// Frame is a content control that supports navigation.
    /// </summary>
    public class Frame : IControl
    {
        #region Properties

        /// <summary> Frame does not have parent </summary>
        public IControl Parent { get { return null; } }


        /// <summary> Specify type of Control  </summary>
        public ControlType ControlType { get { return UI.ControlType.Frame; } }

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

        private IFocusable _FocusedControl;
        /// <summary>
        /// Get focused FocusableControl in last update or null if no FocusableControl got focus
        /// </summary>
        public IFocusable FocusedControl
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
                    {
                        _FocusedControl.IsFocused = true;
                        if (_FocusedControl.IsExtendedFocusable)
                        {
                            _UnFocusRequestDotoExtendedIsFocused = true;
                            _UnFocusRequestExecuted = false;
                        }
                    }
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

        /// <summary>
        /// Move keyboard focus to a named control.
        /// </summary>
        /// <param name="controlName">Name of focusable control</param>
        public void FocusControl(string controlName)
        {
            Control fc = Grid.FindControlByName(controlName);
            if (fc != null && fc.IsFocusable)
                FocusedControl = fc as IFocusable;
            else
                FocusedControl = null;
        }

        /// <summary>
        /// Move keyboard focus to a named control.
        /// </summary>
        /// <param name="focusable">focusable control</param>
        public void FocusControl(IFocusable focusable)
        {
            if (focusable.IsFocusable)
                FocusedControl = focusable;
        }

        // because OnGUI method called multiple times per frame this is a helper value to make sure unfocuse request called at all of OnGUI calls
        private bool _UnFocusRequestExecuted;
        private bool _UnFocusRequest;
        private bool _UnFocusRequestDotoExtendedIsFocused;
        private string _FocusedControlName; // name of focusable control by GUI.GetNameOfFocusedControl()
        /// <summary>
        /// remove keyboard focus.
        /// </summary>        
        public void UnFocusControls()
        {
            FocusedControl = null;
            _UnFocusRequest = true;
            _UnFocusRequestExecuted = false;
        }

        /// <summary>
        /// Move focuced control to next tab index.
        /// </summary>
        /// <remarks>
        /// Set sequential tab index for controls inside a frame to make this method work correctly
        /// </remarks>
        public void NextTab()
        {
            IFocusable fc = null;
            if (_FocusedControl != null)
                fc = Grid.FindControlByMinTabIndexAfter(_FocusedControl.TabIndex);
            if (fc == null)
                fc = Grid.FindControlByMinTabIndex();
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
            IFocusable fc = null;
            if (_FocusedControl != null)
                fc = Grid.FindControlByMaxTabIndexBefore(_FocusedControl.TabIndex);
            if (fc == null)
                fc = Grid.FindControlByMaxTabIndex();

            if (fc != null)
                FocusControl(fc);
        }
        
        private BaseControl _PrecedenceEvent;

        /// <summary>
        /// request to have chance for handle events frst
        /// </summary>
        /// <param name="pe">IPrecedenceEvent to register</param>
        /// <returns>True if succes, oterwise false
        /// </returns>
        public bool RegisterPrecedenceEvent(BaseControl pe)
        {
            if (_PrecedenceEvent == null)
            {
                _PrecedenceEvent = pe;
                return true;
            }
            return false;
        }

        /// <summary>
        /// unregister from chance for handle events frst
        /// </summary>
        /// <param name="pe">IPrecedenceEvent to unregister</param>
        /// <returns>True if succes, oterwise false</returns>
        /// <remarks> pe must registered before </remarks>
        public bool UnregisterPrecedenceEvent(BaseControl pe)
        {
            if (_PrecedenceEvent == pe)
            {
                _PrecedenceEvent = null;
                return true;
            }
            return false;
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
        /// Check for events
        /// </summary>
        private void CheckEvents()
        {
            Event e = Event.current;
            if (e != null)
            {
                EventType type = e.type;

                if (e.type != EventType.Used && _PrecedenceEvent != null)
                    _PrecedenceEvent.HandleEvent(e);

                if (e.type != EventType.Used && _FocusedControl != null)
                    _FocusedControl.HandleEvent(e);

                if (type != EventType.Used)
                {
                    if (e.isKey && e.keyCode != KeyCode.None)
                    {
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
        }



        /// <summary> Occurs when a keyboard key was pressed.() </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when a keyboard key was pressed.
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
        /// Occurs when a keyboard key was released.
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
            UpdateLayout();

            CheckEvents();

            DrawControls();

            if (_UnFocusRequest || _UnFocusRequestDotoExtendedIsFocused)
            {
                GUI.FocusControl(string.Empty);
                if (_UnFocusRequest)
                    FocusedControl = null;
                _UnFocusRequestExecuted = true;
            }
            _FocusedControlName = GUI.GetNameOfFocusedControl();
        }

        private void UpdateLayout()
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
        }

        /// <summary>
        /// Update GUI Frame
        /// </summary>
        public virtual void Update()
        {
            if (_UnFocusRequestExecuted)
            {
                _UnFocusRequest = false;
                _UnFocusRequestDotoExtendedIsFocused = false;
                _UnFocusRequestExecuted = false;
                if (!string.IsNullOrEmpty(_FocusedControlName))
                {
                    if (_FocusedControl == null || _FocusedControl.Name != _FocusedControlName)
                    {
                        FocusedControl = Grid.FindControlByName(_FocusedControlName) as IFocusable;
                    }
                }
            }
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
            Grid = new Grid() { Parent = this };
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
            UnFocusControls();
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
            UnFocusControls();
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
            FocusedControl = null;
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
            UnFocusControls();
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
            UnFocusControls();
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
        /// 3- if not and handleTabIndex is true : 
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
                if (command.Key == KeyCommand.Down || command.Key == KeyCommand.Right || command.Key == KeyCommand.Tab)
                {
                    NextTab();
                    handled = true;
                }
                else if (command.Key == KeyCommand.Up || command.Key == KeyCommand.Left)
                {
                    PreviousTab();
                    handled = true;
                }
            }
            return handled;
        }
        #endregion

        #region Helper

        /// <summary>
        /// Grid.Controls.
        /// </summary>
        public BaseControlCollection Controls { get { return Grid.Controls; } }

        #endregion
    }
}