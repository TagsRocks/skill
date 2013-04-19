using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.UI
{

    /// <summary>
    /// Base class for UI events
    /// </summary>
    public class UIEventArgs : EventArgs
    {
        /// <summary> Is event handled </summary>
        public bool Handled { get; set; }
    }

    /// <summary>
    /// Mouse Buttons
    /// </summary>
    public enum MouseButton
    {
        /// <summary> Mouse0 </summary>
        Mouse0 = 0,
        /// <summary> Mouse1 </summary>
        Mouse1 = 1,
        /// <summary> Mouse2 </summary>
        Mouse2 = 2,
        /// <summary> Mouse3 </summary>
        Mouse3 = 3,
        /// <summary> Mouse4 </summary>
        Mouse4 = 4,
        /// <summary> Mouse5 </summary>
        Mouse5 = 5,
        /// <summary> Mouse6 </summary>
        Mouse6 = 6,
        /// <summary> Left mouse button </summary>
        Left = Mouse0,
        /// <summary> Right mouse button </summary>
        Right = Mouse1,
        /// <summary> Middle mouse button </summary>
        Middle = Mouse2,
        /// <summary> Another button in the mouse </summary>
        Other = 7,
    }

    /// <summary>
    /// Mouse event args
    /// </summary>
    public abstract class MouseEventArgs : UIEventArgs
    {
        /// <summary> The mouse position. </summary>
        public Vector2 MousePosition { get; private set; }
        /// <summary> Which modifier keys are held down. </summary>
        public EventModifiers Modifiers { get; private set; }
        /// <summary> Is Shift held down? (Read Only) </summary>
        public bool Shift { get { return (Modifiers & EventModifiers.Shift) == EventModifiers.Shift; } }
        /// <summary> Is Control key held down? (Read Only) </summary>
        public bool Control { get { return (Modifiers & EventModifiers.Control) == EventModifiers.Control; } }
        /// <summary> Is Alt/Option key held down? (Read Only) </summary>
        public bool Alt { get { return (Modifiers & EventModifiers.Alt) == EventModifiers.Alt; } }

        /// <summary>
        /// Create a MouseEventArgs
        /// </summary>
        /// <param name="mousePosition"> The mouse position. </param>
        /// <param name="modifiers"> Which modifier keys are held down. </param>
        public MouseEventArgs(Vector2 mousePosition, EventModifiers modifiers)
        {
            this.MousePosition = mousePosition;
            this.Modifiers = modifiers;
        }
    }

    /// <summary>
    /// Mouse Click Event Args
    /// </summary>
    public class MouseClickEventArgs : MouseEventArgs
    {
        /// <summary> Which mouse button was pressed. Used in EventType.MouseDown, EventType.MouseUp events.  </summary>
        public MouseButton Button { get; private set; }
        /// <summary> How many consecutive mouse clicks have we received. </summary>
        public int ClickCount { get; private set; }

        /// <summary>
        /// Create a MouseClickEventArgs
        /// </summary>
        /// <param name="mousePosition"> The mouse position. </param>
        /// <param name="modifiers"> Which modifier keys are held down. </param>
        /// <param name="button"> Which mouse button was pressed. </param>
        /// <param name="clickCount">  How many consecutive mouse clicks have we received. </param>
        public MouseClickEventArgs(Vector2 mousePosition, EventModifiers modifiers, MouseButton button, int clickCount)
            : base(mousePosition, modifiers)
        {
            this.Button = button;
            this.ClickCount = clickCount;
        }
    }

    /// <summary>
    /// Mouse Move Event Args
    /// </summary>
    public class MouseMoveEventArgs : MouseEventArgs
    {
        /// <summary> Which mouse button was pressed. Used in EventType.MouseDown, EventType.MouseUp events.  </summary>
        public MouseButton Button { get; private set; }
        /// <summary> The relative movement of the mouse compared to last event. </summary>
        public Vector2 Delta { get; private set; }

        /// <summary>
        /// Create a MouseMoveEventArgs
        /// </summary>
        /// <param name="mousePosition"> The mouse position. </param>
        /// <param name="modifiers"> Which modifier keys are held down. </param>
        /// <param name="button"> Which mouse button was pressed. </param>
        /// <param name="delta"> The relative movement of the mouse compared to last event. </param>
        public MouseMoveEventArgs(Vector2 mousePosition, EventModifiers modifiers, MouseButton button, Vector2 delta)
            : base(mousePosition, modifiers)
        {
            this.Button = button;
            this.Delta = delta;
        }
    }

    /// <summary>
    /// MouseClickEventHandler
    /// </summary>
    /// <param name="sender"> Owner of event  </param>
    /// <param name="args"> MouseClickEventArgs </param>
    public delegate void MouseClickEventHandler(object sender, MouseClickEventArgs args);

    /// <summary>
    /// MouseMoveEventHandler
    /// </summary>
    /// <param name="sender"> Owner of event  </param>
    /// <param name="args"> MouseMoveEventHandler </param>
    public delegate void MouseMoveEventHandler(object sender, MouseMoveEventArgs args);


    /// <summary>
    /// KeyEventArgs
    /// </summary>
    public class KeyEventArgs : UIEventArgs
    {
        /// <summary> The character typed. </summary>
        public char Character { get; private set; }
        /// <summary> The raw key code for keyboard events. </summary>
        public KeyCode Key { get; private set; }

        /// <summary>
        /// Create a KeyEventArgs
        /// </summary>
        /// <param name="key">  The raw key code for keyboard events. </param>
        /// <param name="character"> The character typed. </param>
        public KeyEventArgs(KeyCode key, char character)
        {
            this.Key = key;
            this.Character = character;
        }
    }

    /// <summary>
    /// KeyEventHandler
    /// </summary>
    /// <param name="sender"> Owner of event  </param>
    /// <param name="args"> KeyEventArgs </param>
    public delegate void KeyEventHandler(object sender, KeyEventArgs args);
}
