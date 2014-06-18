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

        /// <summary> extend mouse hover position </summary>
        public float OverFlow { get; set; }

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
        /// area to detect mouse
        /// </summary>
        protected Rect _RenderAreaWidthOverFlow;

        /// <summary>
        /// RenderAreaChanged
        /// </summary>
        protected override void OnRenderAreaChanged()
        {
            base.OnRenderAreaChanged();
            _RenderAreaWidthOverFlow = RenderArea;
            if (Orientation == UI.Orientation.Horizontal)
            {
                _RenderAreaWidthOverFlow.yMin -= OverFlow;
                _RenderAreaWidthOverFlow.yMax += OverFlow;
            }
            else
            {
                _RenderAreaWidthOverFlow.xMin -= OverFlow;
                _RenderAreaWidthOverFlow.xMax += OverFlow;
            }
        }

        /// <summary>
        /// Create a GridSplitter
        /// </summary>
        public GridSplitter()
        {
            WantsMouseEvents = true;
            OverFlow = 2;
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
                _IsMouseDown = OwnerFrame.RegisterPrecedenceEvent(this);
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
                if (_IsMouseDown)
                    OwnerFrame.UnregisterPrecedenceEvent(this);
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
                if (Parent != null && Parent is Grid)
                {
                    if (((Grid)Parent).RenderArea.Contains(args.MousePosition))
                    {
                        BalanceGrid(args.Delta);
                        args.Handled = true;
                    }
                }
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
                    else if (type == EventType.mouseDown)
                    {
                        if (!Contains(e.mousePosition))
                        {
                            if (_IsMouseDown)
                                OwnerFrame.UnregisterPrecedenceEvent(this);
                            _IsMouseDown = false;
                        }
                    }
                    else if (type == EventType.MouseUp || e.rawType == EventType.MouseUp)
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
                {
                    if (WantsMouseEvents)
                    {
                        if (e.isMouse)
                        {
                            EventType type = e.type;
                            Vector2 localMouse = e.mousePosition;
                            if (_RenderAreaWidthOverFlow.Contains(localMouse))
                            {
                                if (type == EventType.MouseDown)
                                {
                                    MouseButton mb = ConvertButton(e.button);
                                    if (mb == MouseButton.Left)
                                    {
                                        MouseClickEventArgs args = new MouseClickEventArgs(e.mousePosition, e.modifiers, mb, e.clickCount);
                                        OnMouseDown(args);
                                        if (args.Handled)
                                            e.Use();
                                    }
                                }
                            }
                        }
                    }
                    base.HandleEvent(e);
                }
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
                    float[] minHeights = new float[grid.RowDefinitions.Count];
                    float[] maxHeights = new float[grid.RowDefinitions.Count];
                    for (int i = 0; i < grid.RowDefinitions.Count; i++)
                    {
                        renderHeights[i] = grid.RowDefinitions[i].RenderHeight / this.ScaleFactor;
                        minHeights[i] = grid.RowDefinitions[i].MinHeight;
                        maxHeights[i] = grid.RowDefinitions[i].MaxHeight;
                    }

                    // balance in pixel space
                    Balance(renderHeights, minHeights, maxHeights, Row, RowSpan, mouseDelta.y);

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
                    float[] minWidths = new float[grid.ColumnDefinitions.Count];
                    float[] maxWidths = new float[grid.ColumnDefinitions.Count];
                    for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
                    {
                        renderWidths[i] = grid.ColumnDefinitions[i].RenderWidth / this.ScaleFactor;
                        minWidths[i] = grid.ColumnDefinitions[i].MinWidth;
                        maxWidths[i] = grid.ColumnDefinitions[i].MaxWidth;
                    }

                    // balance in pixel space
                    Balance(renderWidths, minWidths, maxWidths, Column, ColumnSpan, mouseDelta.x);

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

        private void Balance(float[] sizes, float[] minSizes, float[] maxSizes, int index, int indexSpan, float delta)
        {
            int span = Mathf.Max(0, indexSpan - 1);
            if (index + span >= sizes.Length)
                span = sizes.Length - index - 1;

            if (index > 0 && sizes.Length > index)
            {
                int i = index - 1;
                float preSize = sizes[i];
                float newSize = sizes[i] + delta;

                if (newSize < minSizes[i]) newSize = minSizes[i];
                if (newSize > maxSizes[i]) newSize = maxSizes[i];

                sizes[index - 1] = newSize;

                delta = newSize - preSize;

            }

            int i2 = (index + span) + 1;
            if (sizes.Length > i2)
            {
                float preSize = sizes[i2];
                float newSize = sizes[i2] - delta;

                if (newSize < minSizes[i2]) newSize = minSizes[i2];
                if (newSize > maxSizes[i2]) newSize = maxSizes[i2];

                sizes[i2] = newSize;

                float delta2 = preSize - newSize;

                if (Mathf.Abs(delta2 - delta) > 0.9f)
                {
                    if (index > 0 && sizes.Length > index)
                        sizes[index - 1] += delta2 - delta;
                }
            }
        }
    }
}
