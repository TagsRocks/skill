using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Documents;

namespace Skill.Studio.Animation.Editor
{
    public class AnimationTreeCanvas : Canvas, INotifyPropertyChanged
    {

        public const double MinScale = 0.1;
        public const double MaxScale = 1f;

        private Cursor _ZoomCursor = null;

        private Point? _RubberbandSelectionStartPoint = null;
        private RubberbandAdorner _RubberbandAdorner;

        private ScaleTransform _ScaleTransform;

        private double _PreScale;
        private double _Scale;

        public double Scale
        {
            get { return _Scale; }
            set
            {
                if (value < MinScale)
                    value = MinScale;
                else if (value > MaxScale)
                    value = MaxScale;
                _PreScale = _Scale;
                _Scale = value;
                OnPropertyChanged("Scale");
                if (Editor != null)
                {
                    if (Editor.AnimationTree != null)
                    {
                        Editor.AnimationTree.Scale = value;
                    }
                }
                Zoom();
            }
        }

        public AnimationTreeEditor Editor
        {
            get { return (AnimationTreeEditor)GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Editor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register("Editor", typeof(AnimationTreeEditor), typeof(AnimationTreeCanvas), new UIPropertyMetadata(null, Editor_Changed));

        static void Editor_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimationTreeCanvas canvas = d as AnimationTreeCanvas;
            if (canvas != null)
            {
                AnimationTreeEditor editor = e.NewValue as AnimationTreeEditor;
                if (editor != null)
                {
                    if (editor.AnimationTree != null)
                    {
                        canvas._Scale = editor.AnimationTree.Scale;
                        canvas.Zoom();
                    }
                }
            }
        }



        public AnimationTreeCanvas()
        {
            _PreScale = Scale = 1;
            _RubberbandAdorner = new RubberbandAdorner(this);

            this._ScaleTransform = new ScaleTransform();
            this.LayoutTransform = this._ScaleTransform;

            this.MouseWheel += new MouseWheelEventHandler(DiagramCanvas_MouseWheel);

            MemoryStream stream = new MemoryStream(Properties.Resources.zoom_in);
            this._ZoomCursor = new Cursor(stream);
            stream.Close();
        }

        void DiagramCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Scale += e.Delta * 0.0002;
            e.Handled = true;

        }
        private void Zoom()
        {
            if (Editor == null || Editor.ScrollViewer == null) return;
            double scale = Scale;
            double deltaScale = scale / _PreScale;

            double halfViewportHeight = this.Editor.ScrollViewer.ViewportHeight / 2;
            double newVerticalOffset = ((this.Editor.ScrollViewer.VerticalOffset + halfViewportHeight) * deltaScale - halfViewportHeight);

            double halfViewportWidth = this.Editor.ScrollViewer.ViewportWidth / 2;
            double newHorizontalOffset = ((this.Editor.ScrollViewer.HorizontalOffset + halfViewportWidth) * deltaScale - halfViewportWidth);


            this._ScaleTransform.ScaleX = scale;
            this._ScaleTransform.ScaleY = scale;

            this.Editor.ScrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
            this.Editor.ScrollViewer.ScrollToVerticalOffset(newVerticalOffset);
        }



        Point? _StartDrag = null;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Source == this)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Alt)) != ModifierKeys.None && (e.LeftButton == MouseButtonState.Pressed ^ e.MiddleButton == MouseButtonState.Pressed))
                {
                    _StartDrag = e.GetPosition(this);
                    if (e.MiddleButton == MouseButtonState.Pressed)
                        Cursor = Cursors.SizeAll;
                    else
                        Cursor = this._ZoomCursor;

                }
                else
                {
                    _StartDrag = null;
                    // in case that this click is the start of a 
                    // drag operation we cache the start point
                    this._RubberbandSelectionStartPoint = new Point?(e.GetPosition(this));

                    // if you click directly on the canvas all 
                    // selected items are 'de-selected'
                    Editor.Selection.Clear();

                    Focus();
                    //e.Handled = true;
                }
            }
        }

        private void Move(double deltaX, double deltaY)
        {
            Editor.ScrollViewer.ScrollToHorizontalOffset(Editor.ScrollViewer.HorizontalOffset - deltaX * 0.6);
            Editor.ScrollViewer.ScrollToVerticalOffset(Editor.ScrollViewer.VerticalOffset - deltaY * 0.6);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if ((Keyboard.Modifiers & (ModifierKeys.Alt)) != ModifierKeys.None && _StartDrag != null && (e.LeftButton == MouseButtonState.Pressed ^ e.MiddleButton == MouseButtonState.Pressed))
            {
                this._RubberbandSelectionStartPoint = null;
                Point p = e.GetPosition(this);
                if (_StartDrag != null)
                {
                    double deltaX = p.X - _StartDrag.Value.X;
                    double deltaY = p.Y - _StartDrag.Value.Y;

                    if (e.MiddleButton == MouseButtonState.Pressed)
                    {
                        Cursor = Cursors.SizeAll;
                        Move(this.Scale * deltaX, this.Scale * deltaY);
                        InvalidateMeasure();
                        e.Handled = true;
                    }
                    else
                    {
                        Cursor = this._ZoomCursor;
                        this.Scale += this.Scale * deltaX * 0.0005;
                        e.Handled = true;
                    }
                }
                _StartDrag = p;
            }
            else
            {
                Cursor = Cursors.Arrow;
                _StartDrag = null;
                // if mouse button is not pressed we have no drag operation, ...
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    this._RubberbandSelectionStartPoint = null;
                }
                // ... but if mouse button is pressed and start
                // point value is set we do have one
                if (this._RubberbandSelectionStartPoint.HasValue && !_RubberbandAdorner.IsBegined)
                {
                    // create rubberband adorner
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    if (adornerLayer != null)
                    {
                        _RubberbandAdorner.Begin(_RubberbandSelectionStartPoint);
                        adornerLayer.Add(_RubberbandAdorner);
                    }
                    e.Handled = true;
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();

            foreach (UIElement element in this.InternalChildren)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                //measure desired size for each child
                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }
            // add margin 
            size.Width += 100;
            size.Height += 100;
            return size;
        }


        // iterate through visual tree to get parent DesignerCanvas
        public static AnimationTreeCanvas GetCanvas(DependencyObject element)
        {
            while (element != null && !(element is AnimationTreeCanvas))
                element = VisualTreeHelper.GetParent(element);

            return element as AnimationTreeCanvas;
        }

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
