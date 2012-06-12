using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Input;
using Skill.Studio.Diagram;
using System.Windows.Documents;

namespace Skill.Studio.Animation.Editor
{
    public class AnimConnector : Control, ISelectable, INotifyPropertyChanged
    {
        #region Variables
        // drag start point, relative to the DesignerCanvas
        private Point? _DragStartPoint = null;
        #endregion

        public ConnectorOrientation Orientation
        {
            get { return (ConnectorOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(ConnectorOrientation), typeof(AnimConnector));


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
            if (ViewModel != null && ViewModel.Type == DataModels.Animation.ConnectorType.Input)
                FillBrush = ViewModel.AnimNode.ContentBrush;
            else
                FillBrush = ViewModel.AnimNode.BorderBrush;
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
            DependencyProperty.Register("HoverBrsuh", typeof(Brush), typeof(AnimConnector), new FrameworkPropertyMetadata(Brushes.White));


        public event EventHandler PositionChanged;
        protected void OnPositionChanged()
        {
            if (PositionChanged != null)
                PositionChanged(this, EventArgs.Empty);
        }

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
                    case ConnectorOrientation.Right:
                        value.X += Width / 2;
                        break;
                    default:
                        break;
                }
                if (_Position != value)
                {
                    _Position = value;
                    OnPositionChanged();
                }
            }
        }


        public ConnectorViewModel ViewModel
        {
            get { return (ConnectorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ConnectorViewModel), typeof(AnimConnector), new UIPropertyMetadata(null, ViewModel_Changed));

        static void ViewModel_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimConnector ac = d as AnimConnector;
            if (ac != null)
            {
                ConnectorViewModel ic = e.NewValue as ConnectorViewModel;
                if (ic != null)
                {
                    ic.Connector = ac;
                    if (ic.Type == DataModels.Animation.ConnectorType.Output)
                    {
                        ic.AnimNode.SelectedChanged += ac._ViewModelSelectedChangedHandler;
                        ac.IsSelected = ic.AnimNode.IsSelected;
                    }
                }
            }
        }

        private EventHandler _ViewModelSelectedChangedHandler;
        void ViewModel_SelectedChanged(object sender, EventArgs e)
        {
            IsSelected = ViewModel.AnimNode.IsSelected;
        }

        public AnimationTreeCanvas ParentCanvas { get { return AnimationTreeCanvas.GetCanvas(this); } }

        public AnimConnector()
        {
            // fired when layout changes            
            base.LayoutUpdated += new EventHandler(Connector_LayoutUpdated);
            this._ViewModelSelectedChangedHandler = ViewModel_SelectedChanged;
        }

        // when the layout changes we update the position property
        void Connector_LayoutUpdated(object sender, EventArgs e)
        {
            Canvas canvas = ParentCanvas;
            if (canvas != null)
            {
                //get centre position of this Connector relative to the DesignerCanvas
                this.Position = this.TransformToAncestor(canvas).Transform(new Point(this.Width / 2, this.Height / 2));
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Canvas canvas = ParentCanvas;
            if (canvas != null)
            {
                // position relative to DesignerCanvas
                this._DragStartPoint = new Point?(e.GetPosition(canvas));                
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
                Canvas canvas = ParentCanvas;
                if (canvas != null)
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
                    if (adornerLayer != null)
                    {
                        AnimConnectorAdorner adorner = new AnimConnectorAdorner(this);
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
