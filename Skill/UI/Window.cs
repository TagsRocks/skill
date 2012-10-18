using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.UI
{
    /// <summary>
    /// Make a popup window.
    /// </summary>
    public class Window : Frame
    {
        private static int _IdGenerator = 0;
        private GUI.WindowFunction _DrawWindowFunction;

        /// <summary>
        /// A unique ID to use for each window. This is the ID you'll use to interface to.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Whether the window Is draggable or not?
        /// </summary>
        public bool IsDraggable { get; set; }
        /// <summary>
        /// True if you want to have the entire window background to act as a drag area, otherwise false to use DraggableArea
        /// </summary>
        public bool FullDraggable { get; set; }
        /// <summary>
        /// the part of the window that can be dragged. This is clipped to the actual window.
        /// </summary>
        public Rect DraggableArea { get; set; }
        /// <summary>
        /// Title of window
        /// </summary>
        public GUIContent Title { get; private set; }

        /// <summary>
        /// The style to use. If null, the style from the current GUISkin is used.
        /// </summary>
        public GUIStyle Style { get; set; }

        /// <summary>
        /// Grid rendered in local space
        /// </summary>
        protected override bool LocalGrid { get { return true; } }

        /// <summary>
        /// Create a window
        /// </summary>
        public Window()
        {
            _DrawWindowFunction = DrawWindow;
            Id = _IdGenerator++;
            Title = new GUIContent() { text = "Window " + Id };
            DraggableArea = new Rect(0, 0, 1000, 20);
            Grid.Margin = new Thickness(0, 20, 0, 0);
        }

        private void DrawWindow(int id)
        {
            Grid.OnGUI();
            if (IsDraggable)
            {
                if (FullDraggable)
                    GUI.DragWindow();
                else
                    GUI.DragWindow(DraggableArea);
            }
        }

        /// <summary>
        /// Draw controls inside window
        /// </summary>
        protected override void DrawControls()
        {
            if (Style != null)
                Position = GUI.Window(Id, Position, _DrawWindowFunction, Title, Style);
            else
                Position = GUI.Window(Id, Position, _DrawWindowFunction, Title);
        }
    }
}
