using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Media;
using System.ComponentModel;

namespace Skill.Editor.Diagram
{
    public class DiagramCanvas : Canvas, INotifyPropertyChanged
    {



        private Cursor _ZoomCursor = null;

        private Point? _RubberbandSelectionStartPoint = null;
        private RubberbandAdorner _RubberbandAdorner;

        public SelectionService Selection { get; private set; }
        private ScaleTransform _ScaleTransform;

        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register("MinScale", typeof(double), typeof(DiagramCanvas));


        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register("MaxScale", typeof(double), typeof(DiagramCanvas));



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
                if (_Scale != value)
                {
                    _PreScale = _Scale;
                    _Scale = value;
                    OnPropertyChanged("Scale");
                    Zoom();
                }
            }
        }


        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(DiagramCanvas));


        public DiagramCanvas()
        {
            MaxScale = 1;
            MinScale = 0.1;
            _PreScale = Scale = 1;
            Selection = new SelectionService(this);
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
            //if ((Keyboard.Modifiers & (ModifierKeys.Alt)) != ModifierKeys.None)
            //{
            this.Scale += e.Delta * 0.0002;
            e.Handled = true;
            //}
        }
        private void Zoom()
        {
            if (ScrollViewer == null) return;
            double scale = Scale;
            double deltaScale = scale / _PreScale;

            double halfViewportHeight = this.ScrollViewer.ViewportHeight / 2;
            double newVerticalOffset = ((this.ScrollViewer.VerticalOffset + halfViewportHeight) * deltaScale - halfViewportHeight);

            double halfViewportWidth = this.ScrollViewer.ViewportWidth / 2;
            double newHorizontalOffset = ((this.ScrollViewer.HorizontalOffset + halfViewportWidth) * deltaScale - halfViewportWidth);


            this._ScaleTransform.ScaleX = scale;
            this._ScaleTransform.ScaleY = scale;

            this.ScrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
            this.ScrollViewer.ScrollToVerticalOffset(newVerticalOffset);
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
                    Selection.Clear();

                    Focus();
                    e.Handled = true;
                }
            }
        }

        private void Move(double deltaX, double deltaY)
        {
            Point p = new Point();
            foreach (FrameworkElement item in Children)
            {
                if (item is DragableContent)
                {
                    p.X = Canvas.GetLeft(item) + deltaX;
                    p.Y = Canvas.GetTop(item) + deltaY;

                    Canvas.SetLeft(item, p.X);
                    Canvas.SetTop(item, p.Y);
                }
            }
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
                    }
                    else
                    {
                        Cursor = this._ZoomCursor;
                        this.Scale += this.Scale * deltaX * 0.0005;
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
            size.Width += 10;
            size.Height += 10;
            return size;
        }


        // iterate through visual tree to get parent DesignerCanvas
        public static DiagramCanvas GetDiagramCanvas(DependencyObject element)
        {
            while (element != null && !(element is DiagramCanvas))
                element = VisualTreeHelper.GetParent(element);

            return element as DiagramCanvas;
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


        public string GetValidHeader(string header)
        {
            int i = 1;
            string name = header;
            while (IsheaderExists(name))
            {
                name = header + i++;
            }
            return name;
        }
        private bool IsheaderExists(string header)
        {
            foreach (var item in InternalChildren)
            {
                if (item is DragableContent)
                {
                    if (((DragableContent)item).Header == header) return true;
                }
            }
            return false;
        }


        public event EventHandler AddConnection;
        protected virtual void OnAddConnection()
        {
            if (AddConnection != null)
                AddConnection(this, EventArgs.Empty);
        }

        public void Add(Connection connection)
        {
            this.InternalChildren.Add(connection);
            OnAddConnection();
        }

        public virtual bool AllowConnect(Connector sourceConnector, Connector sinkConnector)
        {
            return (sourceConnector.ParentDragableContent != sinkConnector.ParentDragableContent) &&
                    (sourceConnector.Arrow == ArrowSymbol.Arrow ^ sinkConnector.Arrow == ArrowSymbol.Arrow);
        }
    }
}
