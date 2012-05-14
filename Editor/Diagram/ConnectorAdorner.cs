using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

namespace Skill.Studio.Diagram
{
    public class ConnectorAdorner : Adorner
    {
        private PathGeometry _PathGeometry;
        private DiagramCanvas _DiagramCanvas;

        private Pen _DrawingPen;

        private DragableContent _HitDragableContent;
        private DragableContent HitDragableContent
        {
            get { return _HitDragableContent; }
            set
            {
                if (_HitDragableContent != value)
                {
                    if (_HitDragableContent != null)
                        _HitDragableContent.IsDragConnectionOver = false;

                    _HitDragableContent = value;

                    if (_HitDragableContent != null)
                        _HitDragableContent.IsDragConnectionOver = true;
                }
            }
        }

        private Connector _SourceConnector;
        private Connector SourceConnector
        {
            get { return _SourceConnector; }
            set
            {
                if (_SourceConnector != value)
                {
                    _SourceConnector = value;
                }
            }
        }

        private Connector _HitConnector;
        private Connector HitConnector
        {
            get { return _HitConnector; }
            set
            {
                if (_HitConnector != value)
                {
                    if (_HitConnector != null) _HitConnector.NotifyMouseLeave();
                    _HitConnector = value;
                    if (_HitConnector != null) _HitConnector.NotifyMouseEnter();
                }
            }
        }

        public ConnectorAdorner(DiagramCanvas canvas, Connector sourceConnector)
            : base(canvas)
        {
            this._DiagramCanvas = canvas;
            this.SourceConnector = sourceConnector;
            _DrawingPen = new Pen(Brushes.LightSlateGray, 1);
            _DrawingPen.LineJoin = PenLineJoin.Round;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (HitConnector != null)
            {

                Connector sourceConnector = this.SourceConnector;
                Connector sinkConnector = this.HitConnector;

                if (_DiagramCanvas.AllowConnect(sourceConnector,sinkConnector))                
                {
                    Connection newConnection;
                    if (sourceConnector.Arrow == ArrowSymbol.None)
                        newConnection = new Connection(sourceConnector, sinkConnector);
                    else
                        newConnection = new Connection(sinkConnector, sourceConnector);
                    Canvas.SetZIndex(newConnection, _DiagramCanvas.Children.Count);
                    this._DiagramCanvas.Add(newConnection);
                }

            }
            if (HitDragableContent != null)
            {
                this.HitDragableContent.IsDragConnectionOver = false;
            }

            if (this.IsMouseCaptured) this.ReleaseMouseCapture();

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this._DiagramCanvas);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
            }

            HitConnector = null;
            SourceConnector = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured) this.CaptureMouse();
                HitTesting(e.GetPosition(this));
                this._PathGeometry = GetPathGeometry(e.GetPosition(this));
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawGeometry(null, _DrawingPen, this._PathGeometry);

            // without a background the OnMouseMove event would not be fired
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        }
        private Point GetBezierPoint(double deltaX, double deltaY, ConnectorOrientation targetOrientation, Point connector)
        {
            switch (targetOrientation)
            {
                case ConnectorOrientation.Left:
                    return new Point(connector.X - deltaX, connector.Y);
                case ConnectorOrientation.Top:
                    return new Point(connector.X, connector.Y - deltaY);
                case ConnectorOrientation.Right:
                    return new Point(connector.X + deltaX, connector.Y);
                case ConnectorOrientation.Bottom:
                    return new Point(connector.X, connector.Y + deltaY);
            }
            return connector;
        }

        private PathGeometry GetPathGeometry(Point position)
        {
            ConnectorOrientation targetOrientation;
            if (HitConnector != null)
            {
                targetOrientation = HitConnector.Orientation;
            }
            else
            {
                if (position.X > SourceConnector.Position.X)
                    targetOrientation = ConnectorOrientation.Left;
                else
                    targetOrientation = ConnectorOrientation.Right;
            }
            return BezierCurve.GetPathGeometry(SourceConnector.Position, position, SourceConnector.Orientation, targetOrientation);
        }

        private void HitTesting(Point hitPoint)
        {
            bool hitConnectorFlag = false;

            DependencyObject hitObject = _DiagramCanvas.InputHitTest(hitPoint) as DependencyObject;
            while (hitObject != null &&
                   hitObject != SourceConnector.ParentDragableContent &&
                   hitObject.GetType() != typeof(DiagramCanvas))
            {
                if (hitObject is Connector)
                {
                    HitConnector = hitObject as Connector;
                    hitConnectorFlag = true;
                }

                if (hitObject is DragableContent)
                {
                    HitDragableContent = hitObject as DragableContent;
                    if (!hitConnectorFlag)
                        HitConnector = null;
                    return;
                }
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }

            HitConnector = null;
            HitDragableContent = null;
        }
    }
}
