using Skill.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.Editor.UI.Extended
{
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
        /// OnMouse down
        /// </summary>
        /// <param name="args">args</param>
        protected override void OnMouseDown(MouseClickEventArgs args)
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
            base.OnMouseDown(args);
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

                    Parent.X += delta.x;
                    Parent.Y += delta.y;
                    e.Use();
                }
                else if (e.type == EventType.MouseUp && e.button == 0)
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
