using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Provides a way to arrange content in a grid where all the cells in the grid have the same size.
    /// </summary>
    public class UniformGrid : Panel
    {
        /// <summary> Gets or sets the number of columns that are in the grid. </summary>
        public int Columns { get; set; }

        //// <summary> Gets or sets the number of leading blank cells in the first row of the grid. </summary>
        //public int FirstColumn { get; set; }

        /// <summary> Gets or sets the number of rows that are in the grid. </summary>
        public int Rows { get; set; }        

        /// <summary>
        /// Initializes a new instance of the UniformGrid class.
        /// </summary>
        public UniformGrid()
        {

        }

        /// <summary>
        ///  Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        public override void UpdateLayout()
        {
            Rect renderArea = RenderAreaShrinksByPadding;

            int rows = Mathf.Max(Rows, 1);
            int columns = Mathf.Max(Columns, 1);

            float cellWidth = renderArea.width / columns;
            float cellHeight = renderArea.height / rows;

            Rect[,] cells = new Rect[rows, columns];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Rect rect = new Rect();

                    rect.x = renderArea.x + j * cellWidth;
                    rect.y = renderArea.y + i * cellHeight;
                    rect.width = cellWidth;
                    rect.height = cellHeight;

                    cells[i, j] = rect;
                }
            }

            foreach (var c in Controls)
            {
                c.ScaleFactor = this.ScaleFactor;
                int row = Mathf.Max(0, Mathf.Min(c.Row, rows - 1));
                int column = Mathf.Max(0, Mathf.Min(c.Column, columns - 1));

                Rect cellRect = cells[row, column];
                if (c.RowSpan > 1)
                {
                    int rowspan = Mathf.Min(row + c.RowSpan - 1, rows - 1);
                    cellRect.yMax = cells[rowspan, column].yMax;
                }
                if (c.ColumnSpan > 1)
                {
                    int columnspan = Mathf.Min(column + c.ColumnSpan - 1, columns - 1);
                    cellRect.xMax = cells[row, columnspan].xMax;
                }

                SetControlRenderArea(c, cellRect);
            }
        }
    }
}
