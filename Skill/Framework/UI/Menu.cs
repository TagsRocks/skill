using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Contains frames and dialogs and manage how to they be visible
    /// </summary>
    public class Menu : IEnumerable<Frame>
    {
        // Variables
        private Frame[] _Frames;            // The frames that provided by developer to do different tasks
        private Stack<Frame> _ViewStack;    // Stack of frames that prioritize frames to be visible
        private Stack<Frame> _DialogStack;  // Stack of frames that prioritize frames to be visible as dialogs
        private Frame _FrameToShowNext;     // to don't modify stack of frames when rendering frames
        private Frame _DialogToShowNext;    // to don't modify stack of dialogs when rendering dialogs
        private bool _BackRequest;          // request to show previous frame in view stack
        private bool _ClearRequest;         // request to show clear view stack

        // Find a frame by name
        private Frame FindFrame(string name)
        {
            return FindFrame(name, _Frames);
        }

        // Find a frame by name in given list
        private Frame FindFrame(string name, IEnumerable<Frame> frames)
        {
            foreach (var f in frames)
            {
                if (f.Name == name)
                    return f;
            }
            return null;
        }

        /// <summary> Retrieves top most visible frame </summary>
        public Frame TopFrame
        {
            get
            {
                if (_ViewStack.Count > 0)
                    return _ViewStack.Peek();
                else
                    return null;
            }
        }

        /// <summary> Retrieves top most visible dialog </summary>
        public Frame TopDialog
        {
            get
            {
                if (_DialogStack.Count > 0)
                    return _DialogStack.Peek();
                else
                    return null;
            }
        }

        /// <summary> Name of dialog to show before last frame removed from view stack</summary>
        public string ExitDialog { get; set; }

        /// <summary>
        /// Retrieves frames by name
        /// </summary>
        /// <param name="name">Name of frame</param>
        /// <returns>Frame</returns>
        public Frame this[string name] { get { return FindFrame(name); } }

        /// <summary>
        /// Occurs when last frame removed(backed) from menu
        /// </summary>
        public event EventHandler Exit;
        /// <summary>
        /// Occurs when last frame removed(backed) from menu
        /// </summary>
        protected virtual void OnExit()
        {
            if (Exit != null)
                Exit(this, EventArgs.Empty);
        }


        /// <summary>
        /// Create a menu
        /// </summary>
        /// <param name="frames">Frames to use by menu</param>
        public Menu(params Frame[] frames)
        {
            this._ViewStack = new Stack<Frame>();
            this._DialogStack = new Stack<Frame>();

            // check frames is valid
            this._Frames = frames;
            if (this._Frames == null)
                throw new ArgumentNullException("frames parameter is null");
            if (this._Frames.Length == 0)
                throw new ArgumentException("frames parameter is empty. you must provide at least one frame for menu");
            for (int i = 0; i < this._Frames.Length; i++)
            {
                var f = this._Frames[i];
                if (f == null)
                    throw new ArgumentNullException("frames contains null value");
                if (f.Menu != null)
                    throw new ArgumentException("Frame belongs to another Menu. A Frame can not added to more than one Menu");
                f.Menu = this; // set Menu property of frames to valid value
            }

            this._FrameToShowNext = null;
            this._DialogToShowNext = null;
            this._BackRequest = false;
        }


        /// <summary>
        /// Is specified frame already in view stack? ( it is top most visible frame or reachable by calling 'Back()' )
        /// </summary>
        /// <param name="frame">Frame to check</param>
        /// <returns>True if frame is in use, otherwise false</returns>
        public bool IsInUse(Frame frame)
        {
            return (FindFrame(frame.Name, _ViewStack) == frame || FindFrame(frame.Name, _DialogStack) == frame);
        }

        /// <summary>
        /// Show a frame in next render.do not call this when a dialog is visible or the frame already in use
        /// </summary>
        /// <param name="frameName">Valid name of frame to show in next render</param>
        public void ShowFrame(string frameName)
        {
            if (string.IsNullOrEmpty(frameName)) // check name to be valid
                throw new ArgumentException("Invalid frame name to show.");
            if (TopDialog != null)// if a dialog is visible, operation is invalid
                throw new InvalidOperationException("can not show a frame when showing a dialog.");
            Frame frame = FindFrame(frameName);// find frame by name
            if (frame == null) // frame does not exist
                throw new ArgumentException("Can not find valid frame to show");
            if (TopFrame == frame)
            {
                if (!_BackRequest || !_ClearRequest)
                    return;// frame is already valid and visible
            }
            else if (IsInUse(frame)) // frame is in stack and lower level
                throw new ArgumentException(string.Format("Can not show a frame twice( frameName : {0} )", frameName));

            // prepare to show frame at next render
            this._FrameToShowNext = frame;

        }

        /// <summary>
        /// Show a dialog in next render.do not call this when the frame already in use
        /// </summary>
        /// <param name="frameName">Valid name of frame(dialog) to show in next render</param>
        public void ShowDialog(string frameName)
        {
            if (string.IsNullOrEmpty(frameName)) // check name to be valid
                throw new ArgumentException("Invalid frame name to show dialog.");
            Frame frame = FindFrame(frameName); // find frame by name
            if (frame == null)// frame does not exist
                throw new ArgumentException("Can not find valid frame to show dialog");
            if (TopDialog == frame)
            {
                if (!_BackRequest || !_ClearRequest)
                    return; // frame is already valid and visible
            }
            else if (IsInUse(frame)) // frame is in stack and lower level
                throw new ArgumentException(string.Format("Can not show a frame twice( frameName : {0} )", frameName));

            // prepare to show frame at next render
            this._DialogToShowNext = frame;

        }

        /// <summary>
        /// Pop current frame from stack and show previous frame. do not call this when a dialog is visible 
        /// </summary>
        public void Back()
        {
            if (_DialogStack.Count > 0) // a dialog is visible
                throw new InvalidOperationException("Can not back menu while showing a dialog");
            if (_ViewStack.Count > 0) // if back operation is valid
            {
                if (_ViewStack.Count == 1 && !string.IsNullOrEmpty(ExitDialog))
                    ShowDialog(ExitDialog);
                else
                    _BackRequest = true; // prepare to back at next render
            }
        }

        /// <summary>
        /// Clear view stack
        /// </summary>
        public void Clear()
        {
            _ClearRequest = true;
        }

        /// <summary>
        /// Call this in OnGUI method of MonoBehaviour to draw menu
        /// </summary>
        public void OnGUI()
        {
            if (_ClearRequest)
            {
                _ViewStack.Clear();
                _DialogStack.Clear();
                _ClearRequest = false;
            }
            // there was a request to show previous frame
            else if (_BackRequest)
            {
                if (_ViewStack.Count > 0)
                {
                    bool cancel = false;
                    if (_ViewStack.Peek().LeaveFrame(ref cancel)) // let current frame leave
                    {
                        if (!cancel) // if current frame do not cancel operation
                        {
                            _ViewStack.Pop(); // remove top frame
                            if (_ViewStack.Count > 0)
                            {
                                _ViewStack.Peek().ShowFrame();// show lower frame in stack
                            }
                            else
                            {
                                // stack is empty
                                OnExit();
                            }
                        }
                        _BackRequest = false;
                    }
                    // else wait until current visible frame leave successfully
                }
            }
            // there was a request to show a frame
            else if (_FrameToShowNext != null)
            {
                bool enter = false; // can we show next frame or should wait?
                if (_ViewStack.Count > 0)
                    enter = _ViewStack.Peek().HideFrame();
                else
                    enter = true;
                if (enter) // previous frame hide successfully, so we can show next frame
                {
                    _FrameToShowNext.EnterFrame();
                    _ViewStack.Push(_FrameToShowNext);
                    _FrameToShowNext = null;
                }
                // else wait until current visible frame hide successfully
            }
            // there was a request to show a dialog
            else if (_DialogToShowNext != null)
            {
                _DialogToShowNext.EnterFrame();
                _DialogStack.Push(_DialogToShowNext);
                _DialogToShowNext = null;
            }


            bool preEnabledValue = true;
            // draw top most frame
            if (_ViewStack.Count > 0)
            {
                Frame top = _ViewStack.Peek();

                preEnabledValue = top.IsEnabled;// save value
                if (_DialogStack.Count > 0) // disable frame if a dialog is on top
                    top.IsEnabled = false;
                top.OnGUI();
                top.IsEnabled = preEnabledValue; // restore value
            }
            // draw dialogs
            if (_DialogStack.Count > 0)
            {
                int dialogIndex = 1;
                foreach (Frame dialog in _DialogStack)
                {
                    preEnabledValue = dialog.IsEnabled; // save value
                    if (dialogIndex < _DialogStack.Count) // disable frame if a dialog is on top
                        dialog.IsEnabled = false;
                    // draw a block screen to deavtive lower visible frames and do not let user do something undesired                    
                    dialog.OnGUI(); // draw dialog

                    dialog.IsEnabled = preEnabledValue; // restore value
                    dialogIndex++;
                }
            }
            // check to see if top most dialog provides a valid result and needs to close
            if (_DialogStack.Count > 0)
            {
                Frame dialog = _DialogStack.Peek();
                if (dialog.DialogResult != DialogResult.None) // if dialog provides valid result
                {
                    bool cancel = false;
                    if (dialog.LeaveFrame(ref cancel)) // let current dialog leave
                    {
                        if (!cancel) // if current dialog do not cancel operation
                        {
                            dialog = _DialogStack.Pop();// remove it from stack
                            if (_DialogStack.Count > 0)
                                _DialogStack.Peek().HandleDialog(dialog);// return message to lower level dialog
                            else if (_ViewStack.Count > 0)
                                _ViewStack.Peek().HandleDialog(dialog);// return message to lower level frame
                        }
                    }
                    // else wait until top dialog leave successfully                    
                }
            }
        }

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
        /// 5- Handle cancel command as Back
        /// as soon as first control handled the command ignore next steps
        /// </remarks>        
        public virtual bool HandleCommand(UICommand command, bool handleTabIndex = true)
        {
            bool handled = false;
            if (TopDialog != null)
                handled = TopDialog.HandleCommand(command, handleTabIndex);
            else if (TopFrame != null)
                handled = TopFrame.HandleCommand(command, handleTabIndex);

            if (!handled)
            {
                if (command.Key == KeyCommand.Cancel && _DialogStack.Count == 0 && _ViewStack.Count > 0)
                {
                    Back();
                    handled = true;
                }
            }
            return handled;
        }
        #endregion

        /// <summary>
        /// Returns an System.Collections.IEnumerator
        /// </summary>
        /// <returns> An System.Collections.IEnumerator </returns>
        public IEnumerator<Frame> GetEnumerator()
        {
            return this._Frames.GetEnumerator() as IEnumerator<Frame>;
        }

        /// <summary>
        /// Returns an System.Collections.IEnumerator
        /// </summary>
        /// <returns> An System.Collections.IEnumerator </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._Frames.GetEnumerator();
        }
    }
}
