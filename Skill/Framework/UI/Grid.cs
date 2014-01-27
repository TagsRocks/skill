using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Defines a flexible grid area that consists of columns and rows.
    /// </summary>
    public class Grid : Panel
    {

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
            this.ColumnDefinitions = new ColumnDefinitionCollection();
            this.RowDefinitions = new RowDefinitionCollection();

            this.ColumnDefinitions.Change += RowColumnDefinitions_Change;
            this.RowDefinitions.Change += RowColumnDefinitions_Change;
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
                    int rowspan = Mathf.Max(0, Mathf.Min(row + c.RowSpan - 1, RowDefinitions.Count - 1));
                    cellRect.yMax = cells[rowspan, column].yMax;
                }
                if (c.ColumnSpan > 1)
                {
                    int columnspan = Mathf.Max(0, Mathf.Min(column + c.ColumnSpan - 1, ColumnDefinitions.Count - 1));
                    cellRect.xMax = cells[row, columnspan].xMax;
                }
                SetControlRenderArea(c, cellRect);
            }
        }



        private Rect[] CalcRowsLayout()
        {
            Rect renderArea = RenderAreaShrinksByPadding;
            Rect[] rowRects = new Rect[Mathf.Max(1, RowDefinitions.Count)];

            if (RowDefinitions.Count > 0)
            {
                float absAndAutoH = 0;// sum of row's height that is Absolute or Auto
                float starCount = 0; // sum of Row's star

                for (int i = 0; i < RowDefinitions.Count; i++)
                {
                    if (i > 0) rowRects[i].y = rowRects[i - 1].yMax;// estimate y to be after previous row
                    else rowRects[i].y = renderArea.y;

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
                                rowRects[i].height = Mathf.Max(rowRects[i].height, c.LayoutHeight + c.Margin.Vertical);
                            }
                            else if (c.RowSpan > 1 && c.Row + c.RowSpan - 1 == i) // control is partially in row (ends with this row)
                            {
                                float previousRowheights = 0;
                                for (int r = c.Row; r < i; r++) // calc height of previous rows that affected by this control
                                    previousRowheights += rowRects[r].height;

                                rowRects[i].height = Mathf.Max(rowRects[i].height, c.LayoutHeight + c.Margin.Vertical - previousRowheights);
                            }
                        }
                    }
                    else if (rd.Height.IsStar) // count stars
                    {
                        starCount += Mathf.Max(rd.Height.Value, 1.0f);
                    }

                    if (rd.Height.IsAuto)
                    {
                        if (rowRects[i].height < rd.MinHeight)
                            rowRects[i].height = rd.MinHeight;
                        else if (rd.MaxHeight > rd.MinHeight && rowRects[i].height > rd.MaxHeight)
                            rowRects[i].height = rd.MaxHeight;
                    }

                    // do not let row go outside of grid
                    if (rowRects[i].yMax > renderArea.yMax)
                        rowRects[i].height = renderArea.yMax - rowRects[i].y;

                    absAndAutoH += rowRects[i].height;
                }

                if (starCount > 0)// if there is any row in star mode
                {
                    float availableHeight = renderArea.height - absAndAutoH;
                    if (availableHeight > 0)
                    {
                        for (int i = 0; i < RowDefinitions.Count; i++)
                        {
                            RowDefinition rd = RowDefinitions[i];
                            if (rd.Height.IsStar)
                            {
                                // calc percent of row's height depend of available height
                                rowRects[i].height = availableHeight / starCount * rd.Height.Value;
                                if (rowRects[i].height < rd.MinHeight)
                                    rowRects[i].height = rd.MinHeight;
                                else if (rd.MaxHeight > rd.MinHeight && rowRects[i].height > rd.MaxHeight)
                                    rowRects[i].height = rd.MaxHeight;
                            }
                        }
                    }
                }

                // calc y of rows
                float y = renderArea.y;
                for (int i = 0; i < RowDefinitions.Count; i++)
                {
                    rowRects[i].y = y;
                    y += rowRects[i].height;
                }
            }
            else
                rowRects[0] = renderArea;

            for (int i = 0; i < RowDefinitions.Count; i++)
                RowDefinitions[i].RenderHeight = rowRects[i].height;
            return rowRects;
        }

        private Rect[] CalcColumnsLayout()
        {
            Rect renderArea = RenderAreaShrinksByPadding;
            Rect[] columnRects = new Rect[Mathf.Max(1, ColumnDefinitions.Count)];

            if (ColumnDefinitions.Count > 0)
            {
                float absAndAutoW = 0;// sum of row's width that is Absolute or Auto
                float starCount = 0; // sum of Row's star

                for (int j = 0; j < ColumnDefinitions.Count; j++)
                {
                    if (j > 0) columnRects[j].x = columnRects[j - 1].xMax;// estimate x to be after previous column
                    else columnRects[j].x = renderArea.x;

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
                                columnRects[j].width = Mathf.Max(columnRects[j].width, c.LayoutWidth + c.Margin.Horizontal);
                            }
                            else if (c.ColumnSpan > 1 && c.Column + c.ColumnSpan - 1 == j) // control is partially in column (ends with this column)
                            {
                                float previousColumnsWidths = 0;
                                for (int cl = c.Column; cl < j; cl++) // calc width of previous columns that affected by this control
                                    previousColumnsWidths += columnRects[cl].width;

                                columnRects[j].width = Mathf.Max(columnRects[j].width, c.LayoutWidth + c.Margin.Horizontal - previousColumnsWidths);
                            }
                        }
                    }
                    else if (cd.Width.IsStar) // count stars
                    {
                        starCount += Mathf.Max(cd.Width.Value, 1.0f);
                    }

                    if (cd.Width.IsAuto)
                    {
                        if (columnRects[j].width < cd.MinWidth)
                            columnRects[j].width = cd.MinWidth;
                        else if (cd.MaxWidth > cd.MinWidth && columnRects[j].width > cd.MaxWidth)
                            columnRects[j].width = cd.MaxWidth;
                    }

                    // do not let column go outside of grid
                    if (columnRects[j].xMax > renderArea.xMax)
                        columnRects[j].width = renderArea.xMax - columnRects[j].x;

                    absAndAutoW += columnRects[j].width;
                }

                if (starCount > 0)// if there is any column in star mode
                {
                    float availableWidth = renderArea.width - absAndAutoW;
                    if (availableWidth > 0)
                    {
                        for (int j = 0; j < ColumnDefinitions.Count; j++)
                        {
                            ColumnDefinition cd = ColumnDefinitions[j];
                            if (cd.Width.IsStar)
                            {
                                // calc percent of row's height depend of available height
                                columnRects[j].width = availableWidth / starCount * cd.Width.Value;
                                if (columnRects[j].width < cd.MinWidth)
                                    columnRects[j].width = cd.MinWidth;
                                else if (cd.MaxWidth > cd.MinWidth && columnRects[j].width > cd.MaxWidth)
                                    columnRects[j].width = cd.MaxWidth;
                            }
                        }
                    }
                }

                // calc x of rows
                float x = renderArea.x;
                for (int i = 0; i < ColumnDefinitions.Count; i++)
                {
                    columnRects[i].x = x;
                    x += columnRects[i].width;
                }
            }
            else
                columnRects[0] = renderArea;

            for (int i = 0; i < ColumnDefinitions.Count; i++)
                ColumnDefinitions[i].RenderWidth = columnRects[i].width;
            return columnRects;
        }
    }
}