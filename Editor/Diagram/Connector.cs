using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Skill.Editor.Diagram
{
    public enum ConnectorOrientation
    {
        None,
        Left,
        Top,
        Right,
        Bottom
    }

    public class Connector : Control, ISelectable, INotifyPropertyChanged
    {
        // drag start point, relative to the DesignerCanvas
        private Point? _DragStartPoint = null;


        public ConnectorOrientation Orientation
        {
            get { return (ConnectorOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(ConnectorOrientation), typeof(Connector));


        public ArrowSymbol Arrow
        {
            get { return (ArrowSymbol)GetValue(ArrowProperty); }
            set { SetValue(ArrowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Arrow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArrowProperty =
            DependencyProperty.Register("Arrow", typeof(ArrowSymbol), typeof(Connector));


        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(Connector));






        private Brush _FillBrush;
        public Brush FillBrush
        {
            get
            {
                if (_FillBrush == null)
                    SetFillBrush();
                return _FillBrush;
            }
            set
            {
                if (_FillBrush != value)
                {
                    _FillBrush = value;
                    OnPropertyChanged("FillBrush");
                }
            }
        }

        private void SetFillBrush()
        {
            if (Arrow != ArrowSymbol.None)
                FillBrush = ParentDragableContent.ContentBorderBrush;
            else
                FillBrush = ParentDragableContent.BorderBrush;
        }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    SetFillBrush();
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public Brush HoverBrsuh
        {
            get { return (Brush)GetValue(HoverBrsuhProperty); }
            set { SetValue(HoverBrsuhProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HoverBrsuh.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverBrsuhProperty =
            DependencyProperty.Register("HoverBrsuh", typeof(Brush), typeof(Connector), new FrameworkPropertyMetadata(Brushes.White));






        // center position of this Connector relative to the DesignerCanvas
        private Point _Position;
        public Point Position
        {
            get { return _Position; }
            set
            {
                switch (Orientation)
                {
                    case ConnectorOrientation.Left:
                        value.X -= Width / 2;
                        break;
                    case ConnectorOrientation.Top:
                        value.Y -= Height / 2;
                        break;
                    case ConnectorOrientation.Right:
                        value.X += Width / 2;
                        break;
                    case ConnectorOrientation.Bottom:
                        value.Y += Height / 2;
                        break;
                    default:
                        break;
                }
                if (_Position != value)
                {
                    _Position = value;
                    OnPropertyChanged("Position");
                }
            }
        }

        private EventHandler _ParentSelectedChangedHandler;
        private DragableContent _ParentDragableContent;
        public DragableContent ParentDragableContent
        {
            get
            {
                if (_ParentDragableContent == null)
                {
                    _ParentDragableContent = DragableContent.GetDragableContent(this);
                    if (_ParentDragableContent != null)
                    {
                        _ParentDragableContent.Add(this);
                        _ParentDragableContent.SelectedChanged += _ParentSelectedChangedHandler;
                    }
                }
                return _ParentDragableContent;
            }
        }


        void ParentDragableContent_SelectedChanged(object sender, EventArgs e)
        {
            if (Arrow == ArrowSymbol.None)
                if (ParentDragableContent != null)
                {
                    IsSelected = ParentDragableContent.IsSelected;
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

        // keep track of connections that link to this connector        
        private Connection _Connection;
        public Connection Connection
        {
            get { return _Connection; }
            set
            {
                if (_Connection != value)
                {
                    if (_Connection != null)
                    {
                        _Connection.Disconnect();
                    }
                    _Connection = value;
                }
            }
        }

        public Connector()
        {
            this.Index = -1;
            // fired when layout changes
            base.LayoutUpdated += new EventHandler(Connector_LayoutUpdated);
            this._ParentSelectedChangedHandler = new EventHandler(ParentDragableContent_SelectedChanged);
        }

        private bool _Disconnected = false;
        public void Disconnect()
        {
            if (_Disconnected) return;
            if (Connection != null)
                Connection.Disconnect();
            if (ParentDragableContent != null)
                ParentDragableContent.Remove(this);

            _ParentDragableContent = null;
            _ParentCanvas = null;
            _Disconnected = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_ParentDragableContent == null)
            {
                _ParentDragableContent = DragableContent.GetDragableContent(this);
                if (_ParentDragableContent != null)
                {
                    _ParentDragableContent.Add(this);
                    _ParentDragableContent.SelectedChanged += _ParentSelectedChangedHandler;
                }
            }
        }

        // when the layout changes we update the position property
        void Connector_LayoutUpdated(object sender, EventArgs e)
        {
            if (_Disconnected) return;
            DiagramCanvas canvas = ParentCanvas;
            if (canvas != null)
            {
                //get centre position of this Connector relative to the DesignerCanvas
                this.Position = this.TransformToAncestor(canvas).Transform(new Point(this.Width / 2, this.Height / 2));
            }
        }




        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DiagramCanvas canvas = ParentCanvas;
            if (canvas != null)
            {
                // position relative to DesignerCanvas
                this._DragStartPoint = new Point?(e.GetPosition(canvas));
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                this._DragStartPoint = null;

            // but if mouse button is pressed and start point value is set we do have one
            if (this._DragStartPoint.HasValue)
            {
                // create connection adorner                 
                if (ParentCanvas != null)
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(ParentCanvas);
                    if (adornerLayer != null)
                    {
                        ConnectorAdorner adorner = new ConnectorAdorner(ParentCanvas, this);
                        if (adorner != null)
                        {
                            adornerLayer.Add(adorner);
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        internal void NotifyMouseEnter()
        {
            FillBrush = HoverBrsuh;
        }

        internal void NotifyMouseLeave()
        {
            SetFillBrush();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            NotifyMouseEnter();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            NotifyMouseLeave();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            FillBrush = HoverBrsuh;
            base.OnMouseUp(e);
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
