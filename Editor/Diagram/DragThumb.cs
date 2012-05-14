using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Skill.Studio.Diagram
{
    public class DragThumb : Thumb
    {
        private DragableContent _ParentDragableContent;
        public DragableContent ParentDragableContent
        {
            get
            {
                if (_ParentDragableContent == null)
                    _ParentDragableContent = DragableContent.GetDragableContent(this);

                return _ParentDragableContent;
            }
        }


        private DiagramCanvas _ParentCanvas;
        public DiagramCanvas ParentCanvas
        {
            get
            {
                if (_ParentCanvas == null)
                    _ParentCanvas = DiagramCanvas.GetDiagramCanvas(this);

                return _ParentCanvas;
            }
        }

        public DragThumb()
        {
            base.DragDelta += new DragDeltaEventHandler(DragThumb_DragDelta);
        }

        void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ParentDragableContent != null && ParentCanvas != null && ParentDragableContent.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;

                // we only move DragableContents
                var dragableContents = ParentCanvas.Selection.CurrentSelection.OfType<DragableContent>();

                foreach (DragableContent item in dragableContents)
                {
                    double left = Canvas.GetLeft(item);
                    double top = Canvas.GetTop(item);

                    minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                    minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);
                }

                double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                foreach (DragableContent item in dragableContents)
                {
                    double left = Canvas.GetLeft(item);
                    double top = Canvas.GetTop(item);

                    if (double.IsNaN(left)) left = 0;
                    if (double.IsNaN(top)) top = 0;

                    Canvas.SetLeft(item, left + deltaHorizontal);
                    Canvas.SetTop(item, top + deltaVertical);
                }

                ParentCanvas.InvalidateMeasure();
                e.Handled = true;
            }
        }
    }
}
