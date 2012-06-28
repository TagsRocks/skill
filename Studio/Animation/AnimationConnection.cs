using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using Skill.DataModels.Animation;
using Skill.Studio.Diagram;
using System.Windows.Media;
using System.Windows;

namespace Skill.Studio.Animation
{
    public class AnimationConnectionViewModel : ISelectable, INotifyPropertyChanged
    {
        public AnimationConnection Model { get; private set; }

        public int SinkConnectorIndex { get { return Model.SinkConnectorIndex; } }

        public AnimationTreeViewModel Tree { get; private set; }

        public AnimationConnectionViewModel(AnimationTreeViewModel tree, AnimationConnection model)
        {
            this._ConnectorPropertyChangedHandler = OnConnectorPropertyChanged;
            this._ConnectorPositionChangedHandler = Connector_PositionChanged;
            this.Tree = tree;
            this.Model = model;
            this.Source = Tree.FindByModel(model.Source);
            this.Sink = Tree.FindByModel(model.Sink);
        }

        public AnimationConnectionViewModel(AnimationTreeViewModel tree, AnimNodeViewModel source, AnimNodeViewModel sink, int sinkConnectorIndex)
        {
            this._ConnectorPropertyChangedHandler = OnConnectorPropertyChanged;
            this.Tree = tree;
            this.Model = new AnimationConnection(source.Model, sink.Model, sinkConnectorIndex);
            this.Source = source;
            this.Sink = sink;
        }

        //private Adorner _ConnectionAdorner;
        private PropertyChangedEventHandler _ConnectorPropertyChangedHandler;
        #region Properties

        // source connector

        private AnimNodeViewModel _Source;
        public AnimNodeViewModel Source
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
                PathBrush = Source.SelectedBrush;
            else if (Source != null)
                PathBrush = Source.ContentBrush;
        }

        // sink connector
        private AnimNodeViewModel _Sink;
        public AnimNodeViewModel Sink
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

        private Brush _PathBrush;
        public Brush PathBrush
        {
            get { return _PathBrush; }
            private set
            {
                if (_PathBrush != value)
                {
                    _PathBrush = value;
                    OnPropertyChanged("PathBrush");
                }
            }
        }

        #endregion


        void OnConnectorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // whenever the 'Position' property of the source or sink Connector 
            // changes we must update the connection path geometry
            if ((SourceConnector == null || SinkConnector == null) && (e.PropertyName.Equals("X") || e.PropertyName.Equals("Y")))
            {
                UpdatePathGeometry();
                UpdateAnchorPosition();
            }
            if (e.PropertyName.Equals("IsSelected"))
            {
                if (sender == Source)
                {
                    IsSelected = Source.IsSelected;
                }
            }
        }

        private Editor.AnimConnector _SourceConnector;
        private Editor.AnimConnector SourceConnector
        {
            get { return _SourceConnector; }
            set
            {
                if (_SourceConnector != null)
                {
                    _SourceConnector.PositionChanged -= _ConnectorPositionChangedHandler;
                }
                _SourceConnector = value;
                if (_SourceConnector != null)
                {
                    _SourceConnector.PositionChanged += _ConnectorPositionChangedHandler;
                }
            }
        }

        private Editor.AnimConnector _SinkConnector;
        private Editor.AnimConnector SinkConnector
        {
            get { return _SinkConnector; }
            set
            {
                if (_SinkConnector != null)
                {
                    _SinkConnector.PositionChanged -= _ConnectorPositionChangedHandler;
                }
                _SinkConnector = value;
                if (_SinkConnector != null)
                {
                    _SinkConnector.PositionChanged += _ConnectorPositionChangedHandler;
                }
            }
        }

        private EventHandler _ConnectorPositionChangedHandler;
        void Connector_PositionChanged(object sender, EventArgs e)
        {
            UpdatePathGeometry();
            UpdateAnchorPosition();
        }

        private PathFigure _PathFigure;
        private BezierSegment _BezierSegment;
        private void UpdatePathGeometry()
        {
            if (Source != null && Sink != null)
            {
                if (PathGeometry == null)
                {
                    _PathFigure = new PathFigure();
                    _BezierSegment = new BezierSegment();
                    _BezierSegment.IsStroked = true;
                    _PathFigure.Segments.Add(_BezierSegment);
                }

                if (SourceConnector == null)
                    SourceConnector = _Source.OutConnector.Connector;
                if (SinkConnector == null)
                    SinkConnector = _Sink.GetConnector(SinkConnectorIndex);

                if (SourceConnector == null || SinkConnector == null) return;

                Point targetPosition = SinkConnector.Position;
                Point sourcePosition = SourceConnector.Position;

                double deltaX = System.Math.Abs(targetPosition.X - sourcePosition.X) * 0.5;
                double deltaY = System.Math.Abs(targetPosition.Y - sourcePosition.Y) * 0.5;

                Point startBezierPoint = BezierCurve.GetBezierPoint(deltaX, deltaY, SourceConnector.Orientation, sourcePosition);
                Point endBezierPoint = BezierCurve.GetBezierPoint(deltaX, deltaY, SinkConnector.Orientation, targetPosition);

                _PathFigure.StartPoint = sourcePosition;
                _BezierSegment.Point1 = startBezierPoint;
                _BezierSegment.Point2 = endBezierPoint;
                _BezierSegment.Point3 = targetPosition;

                if (PathGeometry == null)
                {
                    PathGeometry pathGeometry = new System.Windows.Media.PathGeometry();
                    pathGeometry.Figures.Add(_PathFigure);
                    this.PathGeometry = pathGeometry;
                }
            }
        }

        private void UpdateAnchorPosition()
        {
            if (PathGeometry == null) return;

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
    }
}
