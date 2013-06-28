using Skill.DataModels.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Skill.Studio.AI.Editor
{
    /// <summary>
    /// Interaction logic for BehaviorTreeGraphView.xaml
    /// </summary>
    public partial class BehaviorTreeGraphView : BehaviorTreeViewControl, INotifyPropertyChanged, IBehaviorTreeGraphView
    {

        public override BehaviorTreeViewModel BehaviorTree
        {
            get
            {
                return base.BehaviorTree;
            }
            set
            {
                if (base.BehaviorTree != null)
                {
                    base.BehaviorTree.GraphView = null;
                }
                base.BehaviorTree = value;
                if (base.BehaviorTree != null)
                {
                    base.BehaviorTree.GraphView = this;
                    RefreshGraph();
                }
            }
        }

        private Orientation _Orientation;
        public Orientation Orientation
        {
            get { return _Orientation; }
            set
            {
                if (_Orientation != value)
                {
                    _Orientation = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Orientation"));
                    RefreshGraph();
                }
            }
        }


        public BehaviorTreeGraphView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region IBehaviorTreeGraphView members
        private bool _NeedUpdatePosition;
        public event EventHandler UpdateTreeNodes;
        public void RefreshGraph()
        {
            if (this.BehaviorTree == null) return;
            this._NeedUpdatePosition = false;
            this.BehaviorTree.UpdateSelectedBehaviors();
            this.UpdateNodes();
            this._NeedUpdatePosition = true;
            this.OnUpdateTreeNodes();
        }
        public void UpdateNodes()
        {
            if (this.BehaviorTree == null) return;
            UpdatePosition(this.BehaviorTree.Root);
            if (_NeedUpdatePosition)
            {
                _NeedUpdatePosition = false;
                OnUpdateTreeNodes();
            }
        }
        private void OnUpdateTreeNodes()
        {
            if (UpdateTreeNodes != null)
                UpdateTreeNodes(this, EventArgs.Empty);
        }
        public void SetChanged(bool changed)
        {
            if (Editor != null)
                Editor.SetChanged(changed);
        }
        #endregion


        private static double LevelOffset = 50;
        private static double NodeOffset = 10;
        private static double OFFSET = 2;

        #region Calc Graph
        private void UpdatePosition(BehaviorViewModel bvm)
        {
            UpdatePosition(bvm, NodeOffset, NodeOffset);
            UpdateConnection(bvm);
        }
        private void UpdateConnection(BehaviorViewModel bvm)
        {
            UpdateConnectionToParent(bvm);
            foreach (BehaviorViewModel child in bvm) if (child != null) UpdateConnection(child);
        }
        private double UpdatePosition(BehaviorViewModel bvm, double x, double y)
        {
            if (_Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                bvm.X = x;

                double delta;
                if (bvm.Count == 0) // this is a leaf node
                {
                    delta = bvm.Height;
                }
                else
                {
                    delta = 0;
                    int i = 0;
                    foreach (BehaviorViewModel child in bvm)
                    {
                        if (child != null)
                            delta += UpdatePosition(child, x + bvm.Width + LevelOffset, y + delta) + NodeOffset;
                        i++;
                    }

                    if (bvm.Count > 1) delta -= NodeOffset;
                }
                bvm.Y = y + (delta - bvm.Height) * 0.5;
                return delta;
            }
            else
            {
                bvm.Y = y;

                double delta;
                if (bvm.Count == 0) // this is a leaf node
                {
                    delta = bvm.Width;
                }
                else
                {
                    delta = 0;
                    int i = 0;
                    foreach (BehaviorViewModel child in bvm)
                    {
                        if (child != null)
                            delta += UpdatePosition(child, x + delta, y + bvm.Height + LevelOffset) + NodeOffset;
                        i++;
                    }

                    if (bvm.Count > 1) delta -= NodeOffset;
                }
                bvm.X = x + (delta - bvm.Width) * 0.5;
                return delta;
            }
        }
        private void UpdateConnectionToParent(BehaviorViewModel bvm)
        {

            BehaviorViewModel parent = bvm.Parent as BehaviorViewModel;
            if (parent != null)
            {
                if (bvm.ConnectionToParent == null)
                {
                    bvm._PathFigure = new PathFigure();
                    bvm._BezierSegment = new BezierSegment();
                    bvm._BezierSegment.IsStroked = true;
                    bvm._PathFigure.Segments.Add(bvm._BezierSegment);
                }

                Point startBezierPoint, endBezierPoint, targetPosition, sourcePosition;

                if (_Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    double deltaX = parent.X - bvm.X + parent.Width;
                    double deltaY = (parent.Y + parent.Height * 0.5) - (bvm.Y + bvm.Height * 0.5);

                    targetPosition = new Point(OFFSET, bvm.Height * 0.5);
                    sourcePosition = new Point(deltaX - OFFSET, deltaY + bvm.Height * 0.5);

                    deltaX = System.Math.Abs(targetPosition.X - sourcePosition.X) * 0.5;
                    deltaY = System.Math.Abs(targetPosition.Y - sourcePosition.Y) * 0.5;

                    startBezierPoint = Skill.Studio.Diagram.BezierCurve.GetBezierPoint(deltaX, deltaY, Diagram.ConnectorOrientation.Right, sourcePosition);
                    endBezierPoint = Skill.Studio.Diagram.BezierCurve.GetBezierPoint(deltaX, deltaY, Diagram.ConnectorOrientation.Left, targetPosition);
                }
                else
                {
                    double deltaX = (parent.X + parent.Width * 0.5) - (bvm.X + bvm.Width * 0.5);
                    double deltaY = parent.Y - bvm.Y + parent.Height;

                    targetPosition = new Point(bvm.Width * 0.5, OFFSET);
                    sourcePosition = new Point(deltaX + bvm.Width * 0.5, deltaY - OFFSET);

                    deltaX = System.Math.Abs(targetPosition.X - sourcePosition.X) * 0.5;
                    deltaY = System.Math.Abs(targetPosition.Y - sourcePosition.Y) * 0.5;

                    startBezierPoint = Skill.Studio.Diagram.BezierCurve.GetBezierPoint(deltaX, deltaY, Diagram.ConnectorOrientation.Bottom, sourcePosition);
                    endBezierPoint = Skill.Studio.Diagram.BezierCurve.GetBezierPoint(deltaX, deltaY, Diagram.ConnectorOrientation.Top, targetPosition);
                }

                bvm._PathFigure.StartPoint = sourcePosition;
                bvm._BezierSegment.Point1 = startBezierPoint;
                bvm._BezierSegment.Point2 = endBezierPoint;
                bvm._BezierSegment.Point3 = targetPosition;

                if (bvm.ConnectionToParent == null)
                {
                    PathGeometry pathGeometry = new System.Windows.Media.PathGeometry();
                    pathGeometry.Figures.Add(bvm._PathFigure);
                    bvm.ConnectionToParent = pathGeometry;
                }
            }

        }
        #endregion

        private void Tg_ChangeOrientation_Click(object sender, RoutedEventArgs e)
        {
            if (Orientation == System.Windows.Controls.Orientation.Horizontal)
                Orientation = System.Windows.Controls.Orientation.Vertical;
            else
                Orientation = System.Windows.Controls.Orientation.Horizontal;
        }

        private void Item_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            BehaviorBorder border = sender as BehaviorBorder;
            if (border != null)
                border.ViewModel.IsSelected = true;
        }
    }

    #region IBehaviorTreeGraphView interface
    public interface IBehaviorTreeGraphView
    {
        event EventHandler UpdateTreeNodes;
        void UpdateNodes();
        void RefreshGraph();
        void SetChanged(bool changed);
    }
    #endregion
}
