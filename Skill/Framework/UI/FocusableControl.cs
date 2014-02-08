using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.UI
{    
    /// <summary>
    /// Defines focusable behavior
    /// </summary>
    public interface IFocusable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this element is enabled in the user interface (UI).
        /// </summary>
        /// <returns>  true if the element is enabled; otherwise, false. The default value is true. </returns>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Indicates whether the element can receive focus.(You must set valid name to enable this behavior)
        /// </summary>
        bool IsFocusable { get; }

        /// <summary>
        /// Occurs when this element gets logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        event EventHandler GotFocus;

        /// <summary>
        /// Occurs when this element loses logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        event EventHandler LostFocus;

        /// <summary> Tab index of control. </summary>
        int TabIndex { get; set; }

        /// <summary> Gets a value that determines whether this element has logical focus. (You must set valid name to enable this behavior) </summary>
        bool IsFocused { get; set; }

        /// <summary> Gets a value that determines whether this element is a unity built in focusable (like button, textbox, ...) or extended </summary>
        bool IsExtendedFocusable { get; }

        /// <summary>
        /// Gets or sets name of control ( should be valid ).
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Handle specified command
        /// </summary>
        /// <param name="command">Command to handle</param>
        /// <returns>True if command is handled, otherwise false</returns>        
        bool HandleCommand(UICommand command);

        /// <summary> Try focuse control </summary>
        void Focus();

        /// <summary> Disable focusable - sometimes in editor it is better to disable focusable </summary>
        void DisableFocusable();
        /// <summary> Enable focusable </summary>
        void EnableFocusable();

        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="e">Event to handle</param>
        void HandleEvent(Event e);
        
    }

    /// <summary>
    /// Base class for focusables Controls.(You must set valid name to enable this behavior)
    /// </summary>
    public abstract class FocusableControl : Control, IFocusable
    {
        /// <summary> Tab index of control. </summary>
        public int TabIndex { get; set; }


        /// <summary> Disable focusable - sometimes in editor it is better to disable focusable </summary>
        public void DisableFocusable() { _IsFocusable = false; }
        /// <summary> Enable focusable </summary>
        public void EnableFocusable() { _IsFocusable = true; }

        private bool _IsFocusable = true;
        /// <summary>
        /// Indicates whether the element can receive focus.(You must set valid name to enable this behavior)
        /// </summary>
        public override bool IsFocusable { get { return _IsFocusable; } }

        /// <summary> it is not extended focusable </summary>
        public bool IsExtendedFocusable { get { return false; } }

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


        private bool _SetFocuse;

        /// <summary>
        /// Occurs when this element gets logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        public event EventHandler GotFocus;
        /// <summary>
        /// when this element gets logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        protected virtual void OnGotFocus()
        {
            _SetFocuse = true;
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
            _SetFocuse = false;
            if (LostFocus != null) LostFocus(this, EventArgs.Empty);
        }

        /// <summary> Try focuse control </summary>
        public void Focus()
        {
            if (OwnerFrame != null)
                OwnerFrame.FocusControl(this);
        }

        /// <summary>
        /// WantsKeyEvents?
        /// </summary>
        /// <returns>True if hooked, otherwise false</returns>
        public bool WantsKeyEvents { get; set; }

        /// <summary>
        /// Check for events (KeyUp, KeyDown)
        /// </summary>
        public override void HandleEvent(Event e)
        {
            base.HandleEvent(e);

            if (e != null && e.type != EventType.Used && _IsFocused && WantsKeyEvents)
            {
                if (e.isKey && e.keyCode != KeyCode.None)
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

        /// <summary> Occurs when a keyboard key was pressed.(if WantsKeyEvents = true ) </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when a keyboard key was pressed.(if WantsKeyEvents = true )
        /// </summary>
        /// <param name="args"> KeyEventArgs </param>
        private void OnKeyDown(KeyEventArgs args)
        {
            if (KeyDown != null)
                KeyDown(this, args);
        }

        /// <summary> Occurs when a keyboard key was released.(if WantsKeyEvents = true ) </summary>
        public event KeyEventHandler KeyUp;
        /// <summary>
        /// Occurs when a keyboard key was released.(if WantsKeyEvents = true )
        /// </summary>
        /// <param name="args"> KeyEventArgs </param>
        private void OnKeyUp(KeyEventArgs args)
        {
            if (KeyUp != null)
                KeyUp(this, args);
        }

        /// <summary> End Render control's content </summary>
        protected override void EndRender()
        {
            base.EndRender();

            if (_IsFocused && _SetFocuse)
            {
                if (string.IsNullOrEmpty(Name))
                    Debug.LogWarning("Invalid name for Focusable control");
                else
                    GUI.FocusControl(Name);
            }

            _SetFocuse = false;
        }      
      
    }

}
