using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Represents the control that redistributes space between columns or rows of a Grid control.
    /// </summary>
    public class GridSplitter : Skill.Framework.UI.Box
    {

        /// <summary> Parent Panel that host this control.(this value should be setted by parent) </summary>
        public override IControl Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                if (value != null)
                {
                    if (!(value is Grid))
                    {
                        throw new InvalidOperationException("you must add GridSplitter to a Grid");
                    }
                }
                base.Parent = value;
            }
        }

        /// <summary> Vertical if split columns, Horizontal if split rows </summary>
        public Orientation Orientation { get; set; }

        /// <summary>
        /// Create a GridSplitter
        /// </summary>
        public GridSplitter()
        {
            WantsMouseEvents = true;
        }


        private bool _IsMouseDown;
        /// <summary>
        /// Occurs when mouse button was pressed.(if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseClickEventArgs </param>
        protected override void OnMouseDown(MouseClickEventArgs args)
        {
            if (args.Button == MouseButton.Left)
            {
                _IsMouseDown = true;
                args.Handled = true;
            }

            base.OnMouseDown(args);
        }

        /// <summary>
        /// Occurs when mouse button was released.(if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseClickEventArgs </param>
        protected override void OnMouseUp(MouseClickEventArgs args)
        {
            if (args.Button == MouseButton.Left)
            {
                _IsMouseDown = false;
                args.Handled = true;
            }
            base.OnMouseUp(args);
        }

        /// <summary>
        /// Occurs when mouse was dragged.(if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseMoveEventArgs </param>
        protected override void OnMouseDrag(MouseMoveEventArgs args)
        {
            if (_IsMouseDown)
            {
                BalanceGrid(args.Delta);
                args.Handled = true;
            }
            base.OnMouseDrag(args);
        }

        public override void HandleEvent(Event e)
        {
            if (e != null && e.type != EventType.Used)
            {
                if (_IsMouseDown)
                {
                    EventType type = e.type;
                    if (type == EventType.MouseDrag)
                    {
                        MouseButton mb = ConvertButton(e.button);
                        MouseMoveEventArgs args = new MouseMoveEventArgs(e.mousePosition, e.modifiers, mb, e.delta);
                        OnMouseDrag(args);
                        if (args.Handled)
                            e.Use();
                    }
                    if (type == EventType.MouseUp)
                    {
                        MouseButton mb = ConvertButton(e.button);
                        MouseClickEventArgs args = new MouseClickEventArgs(e.mousePosition, e.modifiers, mb, e.clickCount);
                        OnMouseUp(args);
                        if (args.Handled)
                            e.Use();
                    }
                    else
                        base.HandleEvent(e);
                }
                else
                    base.HandleEvent(e);
            }
        }

        private void BalanceGrid(Vector2 mouseDelta)
        {
            if (Parent != null && Parent is Grid)
            {
                Grid grid = (Grid)Parent;
                if (Orientation == Skill.Framework.UI.Orientation.Horizontal)
                {
                    #region Horizontal

                    float[] renderHeights = new float[grid.RowDefinitions.Count];
                    for (int i = 0; i < grid.RowDefinitions.Count; i++)
                        renderHeights[i] = grid.RowDefinitions[i].RenderHeight / this.ScaleFactor;

                    // balance in pixel space
                    Balance(renderHeights, Row, RowSpan, mouseDelta.y);

                    float sumOfStarPixels = 0;
                    for (int i = 0; i < grid.RowDefinitions.Count; i++)
                    {
                        if (grid.RowDefinitions[i].Height.GridUnitType != GridUnitType.Pixel)
                            sumOfStarPixels += renderHeights[i];
                    }

                    for (int i = 0; i < grid.RowDefinitions.Count; i++)
                    {
                        if (grid.RowDefinitions[i].Height.GridUnitType == GridUnitType.Pixel)
                            grid.RowDefinitions[i].Height = new GridLength(renderHeights[i], GridUnitType.Pixel);
                        else
                            grid.RowDefinitions[i].Height = new GridLength((renderHeights[i] / sumOfStarPixels) * 1000, GridUnitType.Star);
                    }

                    #endregion
                }
                else
                {
                    #region Vertical
                    float[] renderWidths = new float[grid.ColumnDefinitions.Count];
                    for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
                        renderWidths[i] = grid.ColumnDefinitions[i].RenderWidth / this.ScaleFactor;

                    // balance in pixel space
                    Balance(renderWidths, Column, ColumnSpan, mouseDelta.x);

                    float sumOfStarPixels = 0;
                    for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
                    {
                        if (grid.ColumnDefinitions[i].Width.GridUnitType != GridUnitType.Pixel)
                            sumOfStarPixels += renderWidths[i];
                    }

                    for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
                    {
                        if (grid.ColumnDefinitions[i].Width.GridUnitType == GridUnitType.Pixel)
                            grid.ColumnDefinitions[i].Width = new GridLength(renderWidths[i], GridUnitType.Pixel);
                        else
                            grid.ColumnDefinitions[i].Width = new GridLength((renderWidths[i] / sumOfStarPixels) * 1000, GridUnitType.Star);
                    }
                    #endregion
                }
            }
        }

        private void Balance(float[] sizes, int index, int indexSpan, float delta)
        {
            
            if (index > 0 && sizes.Length > index)
                sizes[index - 1] += delta;

            int span = Mathf.Max(0, indexSpan - 1);
            if (index + span >= sizes.Length)
                span = sizes.Length - index - 1;

            if (sizes.Length > (index + span) + 1)
                sizes[index + span + 1] -= delta;
        }
    }
}
