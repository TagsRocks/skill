using UnityEngine;
using System.Collections;
using System;

namespace Skill.UI
{
    /// <summary>
    /// Defines a flexible grid area that consists of columns and rows.
    /// </summary>
    public class Grid : Panel
    {
        // Variables
        private EventHandler _RowColumnDefinitionsChangeHandler;

        /// <summary>
        /// Gets a ColumnDefinitionCollection defined on this instance of Grid.
        /// </summary>        
        public ColumnDefinitionCollection ColumnDefinitions { get; private set; }
        /// <summary>
        /// Gets a RowDefinitionCollection defined on this instance of Grid.
        /// </summary>    
        public RowDefinitionCollection RowDefinitions { get; private set; }

        /// <summary>
        /// Initializes a new instance of Grid.        
        /// </summary>
        public Grid()
        {
            this._RowColumnDefinitionsChangeHandler = RowColumnDefinitions_Change;
            this.ColumnDefinitions = new ColumnDefinitionCollection();
            this.RowDefinitions = new RowDefinitionCollection();

            this.ColumnDefinitions.Change += _RowColumnDefinitionsChangeHandler;
            this.RowDefinitions.Change += _RowColumnDefinitionsChangeHandler;
        }

        private void RowColumnDefinitions_Change(object sender, System.EventArgs e)
        {
            base.RequestUpdateLayout();
        }

        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        public override void UpdateLayout()
        {
            Rect[] rowRects = CalcRowsLayout();
            Rect[] columnRects = CalcColumnsLayout();

            Rect[,] cells = new Rect[rowRects.Length, columnRects.Length];

            for (int i = 0; i < rowRects.Length; i++)
            {
                for (int j = 0; j < columnRects.Length; j++)
                {
                    Rect rect = new Rect();

                    rect.x = columnRects[j].x;
                    rect.y = rowRects[i].y;
                    rect.width = columnRects[j].width;
                    rect.height = rowRects[i].height;

                    cells[i, j] = rect;
                }
            }

            foreach (var c in Controls)
            {
                int row = Mathf.Max(0, Mathf.Min(c.Row, RowDefinitions.Count - 1));
                int column = Mathf.Max(0, Mathf.Min(c.Column, ColumnDefinitions.Count - 1));

                Rect cellRect = cells[row, column];
                if (c.RowSpan > 1)
                {
                    int rowspan = Mathf.Min(row + c.RowSpan - 1, RowDefinitions.Count - 1);
                    cellRect.yMax = cells[rowspan, column].yMax;
                }
                if (c.ColumnSpan > 1)
                {
                    int columnspan = Mathf.Min(column + c.ColumnSpan - 1, ColumnDefinitions.Count - 1);
                    cellRect.xMax = cells[row, columnspan].xMax;
                }

                SetControlPaintArea(c, cellRect);
            }
        }

        private void SetControlPaintArea(BaseControl c, Rect cellRect)
        {
            Rect paintArea = cellRect;

            // check for VerticalAlignment
            paintArea.height = Mathf.Min(c.Size.Height, cellRect.height);
            switch (c.VerticalAlignment)
            {
                case VerticalAlignment.Top: // only Margin.Top is important
                    paintArea.y = Mathf.Min(cellRect.y + c.Margin.Top, cellRect.yMax - paintArea.height);
                    break;
                case VerticalAlignment.Center: // none of Margin.Top and Margin.Bottom is important
                    paintArea.y = cellRect.y + (cellRect.height - paintArea.height) / 2;
                    break;
                case VerticalAlignment.Bottom: // only Margin.Bottom is important
                    paintArea.y = Mathf.Max(cellRect.y, cellRect.yMax - paintArea.height - c.Margin.Bottom);
                    break;
                case VerticalAlignment.Stretch: // both Margin.Top and Margin.Bottom is important
                    paintArea.height = cellRect.height - c.Margin.Vertical;
                    paintArea.y += c.Margin.Top;
                    break;
            }

            if (paintArea.yMax > cellRect.yMax)
                paintArea.y = cellRect.yMax - paintArea.height;



            // check for HorizontalAlignment
            paintArea.width = Mathf.Min(c.Size.Width, cellRect.width);
            switch (c.HorizontalAlignment)
            {
                case HorizontalAlignment.Left: // only Margin.Left is important
                    paintArea.x = Mathf.Min(cellRect.x + c.Margin.Left, cellRect.xMax - paintArea.width);
                    break;
                case HorizontalAlignment.Center: // none of Margin.Left and Margin.Right is important
                    paintArea.x = cellRect.x + (cellRect.width - paintArea.width) / 2;
                    break;
                case HorizontalAlignment.Right: // only Margin.Right is important
                    paintArea.x = Mathf.Max(cellRect.x, cellRect.xMax - paintArea.width - c.Margin.Right);
                    break;
                case HorizontalAlignment.Stretch: // both Margin.Left and Margin.Right is important
                    paintArea.width = cellRect.width - c.Margin.Horizontal;
                    paintArea.x += c.Margin.Left;
                    break;
            }

            if (paintArea.xMax > cellRect.xMax)
                paintArea.x = cellRect.xMax - paintArea.width;


            c.PaintArea = paintArea;
        }

        private Rect[] CalcRowsLayout()
        {
            Rect paintArea = PaintArea;
            Rect[] rowRects = new Rect[Mathf.Max(1, RowDefinitions.Count)];

            if (RowDefinitions.Count > 0)
            {
                float absAndAutoH = 0;// sum of row's height that is Absolute or Auto
                float starCount = 0; // sum of Row's star

                for (int i = 0; i < RowDefinitions.Count; i++)
                {
                    if (i > 0) rowRects[i].y = rowRects[i - 1].yMax;// estimate y to be after previous row
                    else rowRects[i].y = paintArea.y;

                    RowDefinition rd = RowDefinitions[i];
                    if (rd.Height.IsAbsolute) // no calculation needs
                    {
                        rowRects[i].height = rd.Height.Value;
                    }
                    else if (rd.Height.IsAuto)
                    {
                        foreach (var c in Controls) // try to find all controls that affect this row
                        {
                            if (c.Row == i && c.RowSpan <= 1) // control is fully in row 
                            {
                                rowRects[i].height = Mathf.Max(rowRects[i].height, c.Size.Height + c.Margin.Vertical);
                            }
                            else if (c.RowSpan > 1 && c.Row + c.RowSpan - 1== i) // control is partially in row (ends with this row)
                            {
                                float previousRowheights = 0;
                                for (int r = c.Row; r < i; r++) // calc height of previous rows that affected by this control
                                    previousRowheights += rowRects[r].height;

                                rowRects[i].height = Mathf.Max(rowRects[i].height, c.Size.Height + c.Margin.Vertical - previousRowheights);
                            }
                        }
                    }
                    else if (rd.Height.IsStar) // count stars
                    {
                        starCount += Mathf.Max(rd.Height.Value, 1.0f);
                    }

                    if (!rd.Height.IsStar)
                    {
                        if (rowRects[i].height < rd.MinHeight)
                            rowRects[i].height = rd.Height.Value;
                        else if (rd.MaxHeight > rd.MinHeight && rowRects[i].height > rd.MaxHeight)
                            rowRects[i].height = rd.MaxHeight;
                    }

                    // do not let row go outside of grid
                    if (rowRects[i].yMax > paintArea.yMax)
                        rowRects[i].height = paintArea.yMax - rowRects[i].y;

                    absAndAutoH += rowRects[i].height;
                }

                if (starCount > 0)// if there is any row in star mode
                {
                    float availableHeight = paintArea.height - absAndAutoH;
                    if (availableHeight > 0)
                    {
                        for (int i = 0; i < RowDefinitions.Count; i++)
                        {
                            RowDefinition rd = RowDefinitions[i];
                            if (rd.Height.IsStar)
                            {
                                // calc percent of row's height depend of available height
                                rowRects[i].height = availableHeight / starCount * rd.Height.Value;
                            }
                        }
                    }
                }

                // calc y of rows
                float y = paintArea.y;
                for (int i = 0; i < RowDefinitions.Count; i++)
                {
                    rowRects[i].y = y;
                    y += rowRects[i].height;
                }
            }
            else
                rowRects[0] = paintArea;
            return rowRects;
        }

        private Rect[] CalcColumnsLayout()
        {
            Rect paintArea = PaintArea;
            Rect[] columnRects = new Rect[Mathf.Max(1, ColumnDefinitions.Count)];

            if (ColumnDefinitions.Count > 0)
            {
                float absAndAutoW = 0;// sum of row's width that is Absolute or Auto
                float starCount = 0; // sum of Row's star

                for (int j = 0; j < ColumnDefinitions.Count; j++)
                {
                    if (j > 0) columnRects[j].x = columnRects[j - 1].xMax;// estimate x to be after previous column
                    else columnRects[j].x = paintArea.x;

                    ColumnDefinition cd = ColumnDefinitions[j];
                    if (cd.Width.IsAbsolute) // no calculation needs
                    {
                        columnRects[j].width = cd.Width.Value;
                    }
                    else if (cd.Width.IsAuto)
                    {
                        foreach (var c in Controls) // try to find all controls that affect this row
                        {
                            if (c.Column == j && c.ColumnSpan <= 1) // control is fully in row 
                            {
                                columnRects[j].width = Mathf.Max(columnRects[j].width, c.Size.Width + c.Margin.Horizontal);
                            }
                            else if (c.ColumnSpan > 1 && c.Column + c.ColumnSpan - 1 == j) // control is partially in column (ends with this column)
                            {
                                float previousColumnsWidths = 0;
                                for (int cl = c.Column; cl < j; cl++) // calc width of previous columns that affected by this control
                                    previousColumnsWidths += columnRects[cl].width;

                                columnRects[j].width = Mathf.Max(columnRects[j].width, c.Size.Width + c.Margin.Horizontal - previousColumnsWidths);
                            }
                        }
                    }
                    else if (cd.Width.IsStar) // count stars
                    {
                        starCount += Mathf.Max(cd.Width.Value, 1.0f);
                    }

                    if (!cd.Width.IsStar)
                    {
                        if (columnRects[j].width < cd.MinWidth)
                            columnRects[j].width = cd.Width.Value;
                        else if (cd.MaxWidth > cd.MinWidth && columnRects[j].width > cd.MaxWidth)
                            columnRects[j].width = cd.MaxWidth;
                    }

                    // do not let column go outside of grid
                    if (columnRects[j].xMax > paintArea.xMax)
                        columnRects[j].width = paintArea.xMax - columnRects[j].x;

                    absAndAutoW += columnRects[j].width;
                }

                if (starCount > 0)// if there is any column in star mode
                {
                    float availableWidth = paintArea.width - absAndAutoW;
                    if (availableWidth > 0)
                    {
                        for (int j = 0; j < ColumnDefinitions.Count; j++)
                        {
                            ColumnDefinition cd = ColumnDefinitions[j];
                            if (cd.Width.IsStar)
                            {
                                // calc percent of row's height depend of available height
                                columnRects[j].width = availableWidth / starCount * cd.Width.Value;
                            }
                        }
                    }
                }

                // calc x of rows
                float x = paintArea.x;
                for (int i = 0; i < ColumnDefinitions.Count; i++)
                {
                    columnRects[i].x = x;
                    x += columnRects[i].width;
                }
            }
            else
                columnRects[0] = paintArea;
            return columnRects;
        }
    }
}