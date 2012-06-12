using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Skill.Studio.Animation.Editor
{
    public class RubberbandAdorner : Adorner
    {
        private Point? _StartPoint;
        private Point? _EndPoint;
        private Pen _RubberbandPen;
        private AnimationTreeCanvas _Canvas;

        public Brush RubberbandBrush
        {
            get { return (Brush)GetValue(RubberbandBrushProperty); }
            set { SetValue(RubberbandBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RubberbandBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RubberbandBrushProperty =
            DependencyProperty.Register("RubberbandBrush", typeof(Brush), typeof(RubberbandAdorner), new UIPropertyMetadata(Brushes.LightSlateGray, OnRubberbandBrushChanged));

        static void OnRubberbandBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RubberbandAdorner rubber = d as RubberbandAdorner;
            if (rubber != null)
            {
                Brush b = e.NewValue as Brush;
                if (b != null)
                    rubber._RubberbandPen.Brush = b;
            }
        }

        public bool IsBegined { get; private set; }

        public void Begin(Point? dragStartPoint)
        {
            this._StartPoint = dragStartPoint;
            IsBegined = true;
        }



        public RubberbandAdorner(AnimationTreeCanvas canvas)
            : base(canvas)
        {
            this._Canvas = canvas;
            _RubberbandPen = new Pen(RubberbandBrush, 1);
            _RubberbandPen.DashStyle = new DashStyle(new double[] { 2 }, 1);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                    this.CaptureMouse();

                _EndPoint = e.GetPosition(this);
                UpdateSelection();
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            // release mouse capture
            if (this.IsMouseCaptured) this.ReleaseMouseCapture();

            // remove this adorner from adorner layer
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this._Canvas);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
                IsBegined = false;
            }
            e.Handled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // without a background the OnMouseMove event would not be fired!
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (this._StartPoint.HasValue && this._EndPoint.HasValue)
                dc.DrawRectangle(Brushes.Transparent, _RubberbandPen, new Rect(this._StartPoint.Value, this._EndPoint.Value));
        }

        private void UpdateSelection()
        {            
            Rect rubberBand = new Rect(_StartPoint.Value, _EndPoint.Value);
            foreach (ContentPresenter item in _Canvas.Children)
            {
                AnimNodeViewModel vm = item.Content as AnimNodeViewModel;
                if (vm != null)
                {
                    Rect itemRect = VisualTreeHelper.GetDescendantBounds(item);
                    Rect itemBounds = item.TransformToAncestor(_Canvas).TransformBounds(itemRect);

                    if (rubberBand.Contains(itemBounds))
                    {
                        _Canvas.Editor.Selection.Add(vm);
                    }
                    else
                    {
                        _Canvas.Editor.Selection.Remove(vm);
                    }
                }
            }            
        }
    }
}
