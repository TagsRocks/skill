using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Skill.Editor.Diagram
{
    public static class BezierCurve
    {
        private static Point GetBezierPoint(double deltaX2, double deltaY2, ConnectorOrientation targetOrientation, Point connectorPoint )
        {
            switch (targetOrientation)
            {
                case ConnectorOrientation.Left:
                    return new Point(connectorPoint.X - System.Math.Abs(deltaX2), connectorPoint.Y);
                case ConnectorOrientation.Top:
                    return new Point(connectorPoint.X, connectorPoint.Y - System.Math.Abs(deltaY2));
                case ConnectorOrientation.Right:
                    return new Point(connectorPoint.X + System.Math.Abs(deltaX2), connectorPoint.Y);
                case ConnectorOrientation.Bottom:
                    return new Point(connectorPoint.X, connectorPoint.Y + System.Math.Abs(deltaY2));
            }
            return connectorPoint;
        }

        public static PathGeometry GetPathGeometry(Point sourcePosition, Point targetPosition, ConnectorOrientation sourceOrientation, ConnectorOrientation targetOrientation)
        {

            PathGeometry pathGeometry = new PathGeometry();

            double deltaX = System.Math.Abs(targetPosition.X - sourcePosition.X) * 0.5;
            double deltaY = System.Math.Abs(targetPosition.Y - sourcePosition.Y) * 0.5;

            Point startBezierPoint = GetBezierPoint(deltaX, deltaY, sourceOrientation, sourcePosition);
            Point endBezierPoint = GetBezierPoint(deltaX, deltaY, targetOrientation, targetPosition);

            PathFigure figure = new PathFigure();

            figure.StartPoint = sourcePosition;
            figure.Segments.Add(new BezierSegment(startBezierPoint, endBezierPoint, targetPosition, true));
            pathGeometry.Figures.Add(figure);
            return pathGeometry;
        }

        public static PathGeometry GetPathGeometry(Connector source, Connector target)
        {
            return GetPathGeometry(source.Position, target.Position, source.Orientation, target.Orientation);
        }
    }
}
