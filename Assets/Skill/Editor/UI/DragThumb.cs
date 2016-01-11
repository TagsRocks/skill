using Skill.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.Editor.UI
{
    public class DragEventArgs : System.EventArgs
    {
        public Vector2 Delta { get; private set; }

        public DragEventArgs(Vector2 delta)
        {
            this.Delta = delta;
        }
    }

    public delegate void DragEventHandler(object sender, DragEventArgs args);

    /// <summary>
    /// Thumb to drag it's parent(panel)
    /// </summary>
    public class DragThumb : Box
    {


        private bool _IsMouseDown;
        /// <summary>
        /// Create a DragThumb
        /// </summary>
        public DragThumb()
        {
            WantsMouseEvents = true;
        }

        /// <summary>
        /// MouseDownEvent
        /// </summary>
        /// <param name="args">args</param>
        protected override void MouseDownEvent(MouseClickEventArgs args)
        {
            if (Parent != null && args.Button == MouseButton.Left)
            {
                Frame of = OwnerFrame;
                if (of != null)
                {
                    _IsMouseDown = OwnerFrame.RegisterPrecedenceEvent(this);
                    args.Handled = true;
                }
            }
            base.MouseDownEvent(args);
        }

        public event DragEventHandler Drag;
        protected virtual void OnDrag(Vector2 delta)
        {
            Parent.X += delta.x;
            Parent.Y += delta.y;
            if (Drag != null)
                Drag(this, new DragEventArgs(delta));
        }


        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="e">event to handle</param>
        public override void HandleEvent(Event e)
        {
            if (_IsMouseDown && Parent != null && e != null)
            {
                if (e.type == EventType.MouseDrag)
                {
                    Vector2 delta = e.delta / this.ScaleFactor;
                    OnDrag(delta);
                    e.Use();                    
                }
                else if ((e.type == EventType.MouseUp || e.rawType == EventType.MouseUp) && e.button == 0)
                {
                    Frame of = OwnerFrame;
                    if (of != null)
                    {
                        of.UnregisterPrecedenceEvent(this);
                        _IsMouseDown = false;
                        e.Use();
                    }
                }
            }
            else
                base.HandleEvent(e);
        }
    }
}
