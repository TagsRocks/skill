using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.UI
{

    /// <summary>
    /// Base class for focusables Controls.(You must set valid name to enable this behavior)
    /// </summary>
    public abstract class FocusableControl : Control
    {
        /// <summary> Tab index of control. </summary>
        public uint TabIndex { get; set; }

        /// <summary>
        /// Indicates whether the element can receive focus.(You must set valid name to enable this behavior)
        /// </summary>
        public override bool Focusable { get { return true; } }

        private bool _IsFocused;
        /// <summary>
        /// Gets a value that determines whether this element has logical focus. (You must set valid name to enable this behavior)
        /// </summary>
        /// <returns>
        /// true if this element has logical focus; otherwise, false.(You must set valid name to enable this behavior)
        /// </returns>
        public bool IsFocused
        {
            get { return _IsFocused; }
            internal set
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

        /// <summary>
        /// Occurs when this element gets logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        public event EventHandler GotFocus;
        /// <summary>
        /// when this element gets logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        protected virtual void OnGotFocus()
        {
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
            if (LostFocus != null) LostFocus(this, EventArgs.Empty);
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
        protected override void CheckEvents()
        {
            base.CheckEvents();

            if (_IsFocused && IsAnyKeyEventHooked())
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
        private void OnKeyDown(KeyEventArgs args)
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
        private void OnKeyUp(KeyEventArgs args)
        {
            if (KeyUp != null)
                KeyUp(this, args);
        }
    }

}
