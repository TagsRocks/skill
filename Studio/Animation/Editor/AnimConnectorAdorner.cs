using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using Skill.Studio.Diagram;

namespace Skill.Studio.Animation.Editor
{
    public class AnimConnectorAdorner : Adorner
    {
        private PathGeometry _PathGeometry;

        private Pen _DrawingPen;

        private AnimConnector _SourceConnector;
        private AnimConnector SourceConnector
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

        private AnimConnector _HitConnector;
        private AnimConnector HitConnector
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

        public AnimConnectorAdorner(AnimConnector sourceConnector)
            : base(sourceConnector.ParentCanvas)
        {
            this.SourceConnector = sourceConnector;
            _DrawingPen = new Pen(Brushes.LightSlateGray, 1);
            _DrawingPen.LineJoin = PenLineJoin.Round;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (HitConnector != null)
            {

                AnimConnector sourceConnector = this.SourceConnector;
                AnimConnector sinkConnector = this.HitConnector;

                AnimationTreeCanvas canvas = sourceConnector.ParentCanvas;
                if (canvas != null)
                {
                    canvas.Editor.AddConnection(sourceConnector, sinkConnector);
                }
            }

            if (this.IsMouseCaptured) this.ReleaseMouseCapture();

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.SourceConnector.ParentCanvas);
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

            Canvas canvas = _SourceConnector.ParentCanvas;

            if (canvas != null)
            {
                DependencyObject hitObject = canvas.InputHitTest(hitPoint) as DependencyObject;
                while (hitObject != null && hitObject.GetType() != typeof(AnimationTreeCanvas))
                {
                    if (hitObject is AnimConnector)
                    {
                        HitConnector = hitObject as AnimConnector;
                        hitConnectorFlag = true;
                    }
                    hitObject = VisualTreeHelper.GetParent(hitObject);
                }

                if (!hitConnectorFlag)
                    HitConnector = null;
            }
        }
    }
}
