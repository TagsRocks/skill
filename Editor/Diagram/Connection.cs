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
    public enum ArrowSymbol
    {
        None,
        Arrow,
        Diamond
    }

    public class Connection : Control, ISelectable, INotifyPropertyChanged, IDiagramObject
    {
        //private Adorner _ConnectionAdorner;
        private PropertyChangedEventHandler _ConnectorPropertyChangedHandler;
        #region Properties

        // source connector

        private Connector _Source;
        public Connector Source
        {
            get
            {
                return _Source;
            }
            set
            {
                if (_Source != value)
                {
                    if (_Source != null)
                    {
                        _Source.PropertyChanged -= _ConnectorPropertyChangedHandler;
                    }
                    _Source = value;

                    if (_Source != null)
                    {
                        _Source.Connection = this;
                        IsSelected = _Source.IsSelected;
                        SetPathBrush();
                        _Source.PropertyChanged += _ConnectorPropertyChangedHandler;
                    }

                    UpdatePathGeometry();
                }
            }
        }

        private void SetPathBrush()
        {
            if (_IsSelected)
                PathBrush = SelectedBrush;
            else if (Source != null)
                PathBrush = Source.ParentDragableContent.ContentBorderBrush;
        }

        // sink connector
        private Connector _Sink;
        public Connector Sink
        {
            get { return _Sink; }
            set
            {
                if (_Sink != value)
                {
                    if (_Sink != null)
                    {
                        _Sink.PropertyChanged -= _ConnectorPropertyChangedHandler;
                    }

                    _Sink = value;

                    if (_Sink != null)
                    {
                        _Sink.Connection = this;
                        _Sink.PropertyChanged += _ConnectorPropertyChangedHandler;
                    }
                    UpdatePathGeometry();
                }
            }
        }

        // connection path geometry
        private PathGeometry _PathGeometry;
        public PathGeometry PathGeometry
        {
            get { return _PathGeometry; }
            set
            {
                if (_PathGeometry != value)
                {
                    _PathGeometry = value;
                    if (_Source != null && _Sink != null)
                        UpdateAnchorPosition();
                    OnPropertyChanged("PathGeometry");
                }
            }
        }

        // between source connector position and the beginning 
        // of the path geometry we leave some space for visual reasons; 
        // so the anchor position source really marks the beginning 
        // of the path geometry on the source side
        private Point _AnchorPositionSource;
        public Point AnchorPositionSource
        {
            get { return _AnchorPositionSource; }
            set
            {
                if (_AnchorPositionSource != value)
                {
                    _AnchorPositionSource = value;
                    OnPropertyChanged("AnchorPositionSource");
                }
            }
        }

        // slope of the path at the anchor position
        // needed for the rotation angle of the arrow
        private double _AnchorAngleSource = 0;
        public double AnchorAngleSource
        {
            get { return _AnchorAngleSource; }
            set
            {
                if (_AnchorAngleSource != value)
                {
                    _AnchorAngleSource = value;
                    OnPropertyChanged("AnchorAngleSource");
                }
            }
        }

        // analogue to source side
        private Point _AnchorPositionSink;
        public Point AnchorPositionSink
        {
            get { return _AnchorPositionSink; }
            set
            {
                if (_AnchorPositionSink != value)
                {
                    _AnchorPositionSink = value;
                    OnPropertyChanged("AnchorPositionSink");
                }
            }
        }

        // analogue to source side
        private double _AnchorAngleSink = 0;
        public double AnchorAngleSink
        {
            get { return _AnchorAngleSink; }
            set
            {
                if (_AnchorAngleSink != value)
                {
                    _AnchorAngleSink = value;
                    OnPropertyChanged("AnchorAngleSink");
                }
            }
        }


        // pattern of dashes and gaps that is used to outline the connection path
        private DoubleCollection _StrokeDashArray;
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return _StrokeDashArray;
            }
            set
            {
                if (_StrokeDashArray != value)
                {
                    _StrokeDashArray = value;
                    OnPropertyChanged("StrokeDashArray");
                }
            }
        }

        // if connected, the ConnectionAdorner becomes visible
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    SetPathBrush();
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        private ArrowSymbol _SourceArrowSymbol = ArrowSymbol.None;
        public ArrowSymbol SourceArrowSymbol
        {
            get { return _SourceArrowSymbol; }
            set
            {
                if (_SourceArrowSymbol != value)
                {
                    _SourceArrowSymbol = value;
                    OnPropertyChanged("SourceArrowSymbol");
                }
            }
        }

        public ArrowSymbol _SinkArrowSymbol = ArrowSymbol.Arrow;
        public ArrowSymbol SinkArrowSymbol
        {
            get { return _SinkArrowSymbol; }
            set
            {
                if (_SinkArrowSymbol != value)
                {
                    _SinkArrowSymbol = value;
                    OnPropertyChanged("SinkArrowSymbol");
                }
            }
        }



        public Brush PathBrush
        {
            get { return (Brush)GetValue(PathBrushProperty); }
            set { SetValue(PathBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathBrushProperty =
            DependencyProperty.Register("PathBrush", typeof(Brush), typeof(Connection), new FrameworkPropertyMetadata(Brushes.Orange));




        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(Connection), new FrameworkPropertyMetadata(Brushes.Yellow));



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

        #endregion

        public Connection(Connector source, Connector sink)
        {
            this._ConnectorPropertyChangedHandler = new PropertyChangedEventHandler(OnConnectorPropertyChanged);
            this.Source = source;
            this.Sink = sink;
        }

        public Rect Area
        {
            get
            {
                if (Source != null && Sink != null)
                {
                    return new Rect(Source.Position, Sink.Position);
                }
                return new Rect();
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            // usual selection business            
            if (ParentCanvas != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {
                        ParentCanvas.Selection.RemoveFromSelection(this);
                    }
                    else
                    {
                        ParentCanvas.Selection.Add(this);
                    }
                else if (!this.IsSelected)
                {
                    ParentCanvas.Selection.Select(this);
                }

                Focus();
            }
            e.Handled = false;
        }

        void OnConnectorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // whenever the 'Position' property of the source or sink Connector 
            // changes we must update the connection path geometry
            if (e.PropertyName.Equals("Position"))
            {
                UpdatePathGeometry();
            }
            if (e.PropertyName.Equals("IsSelected"))
            {
                Connector connector = sender as Connector;
                if (connector != null)
                {
                    if (connector.Arrow == ArrowSymbol.None)
                        IsSelected = connector.IsSelected;
                }
            }
        }

        private void UpdatePathGeometry()
        {
            SourceArrowSymbol = (Source != null) ? Source.Arrow : ArrowSymbol.None;
            SinkArrowSymbol = (Sink != null) ? Sink.Arrow : ArrowSymbol.None;

            if (Source != null && Sink != null)
            {
                this.PathGeometry = BezierCurve.GetPathGeometry(_Source, _Sink);
            }
            else
                this.PathGeometry = new PathGeometry();
        }

        private void UpdateAnchorPosition()
        {
            Point pathStartPoint, pathTangentAtStartPoint;
            Point pathEndPoint, pathTangentAtEndPoint;

            // the PathGeometry.GetPointAtFractionLength method gets the point and a tangent vector 
            // on PathGeometry at the specified fraction of its length
            this.PathGeometry.GetPointAtFractionLength(0, out pathStartPoint, out pathTangentAtStartPoint);
            this.PathGeometry.GetPointAtFractionLength(1, out pathEndPoint, out pathTangentAtEndPoint);

            // get angle from tangent vector
            this.AnchorAngleSource = Math.Atan2(-pathTangentAtStartPoint.Y, -pathTangentAtStartPoint.X) * (180 / Math.PI);
            this.AnchorAngleSink = Math.Atan2(pathTangentAtEndPoint.Y, pathTangentAtEndPoint.X) * (180 / Math.PI);

            this.AnchorPositionSource = pathStartPoint;
            this.AnchorPositionSink = pathEndPoint;
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

        private bool _Disconnected = false;
        public void Disconnect()
        {
            if (_Disconnected) return;
            if (ParentCanvas != null)
            {
                ParentCanvas.Children.Remove(this);
                _ParentCanvas = null;
            }
            Sink = null;
            Source = null;
            _Disconnected = true;
        }
    }
}
