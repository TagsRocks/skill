using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Documents;
using System;

namespace Skill.Studio.AI.Editor
{
    class BehaviorTreeCanvas: Canvas, INotifyPropertyChanged
    {

        public const double MinScale = 0.1;
        public const double MaxScale = 1f;

        private Cursor _ZoomCursor = null;
        private Cursor _HandCursor = null;

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
                _Scale = value;
                OnPropertyChanged("Scale");
                if (Editor != null)
                {
                    if (Editor.BehaviorTree != null)
                    {
                        Editor.BehaviorTree.Scale = value;
                    }
                }
                Zoom();
            }
        }

        public BehaviorTreeEditor Editor
        {
            get { return (BehaviorTreeEditor)GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Editor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register("Editor", typeof(BehaviorTreeEditor), typeof(BehaviorTreeCanvas), new UIPropertyMetadata(null, Editor_Changed));

        static void Editor_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BehaviorTreeCanvas canvas = d as BehaviorTreeCanvas;
            if (canvas != null)
            {
                BehaviorTreeEditor editor = e.NewValue as BehaviorTreeEditor;
                if (editor != null)
                {
                    if (editor.BehaviorTree != null)
                    {
                        canvas._Scale = editor.BehaviorTree.Scale;
                        canvas.Zoom();                        
                    }
                    editor.UpdateTreeNodes += new EventHandler(canvas.Editor_UpdateTreeNodes);
                }
            }
        }

        void Editor_UpdateTreeNodes(object sender, EventArgs e)
        {
            this.InvalidateMeasure();
        }



        public BehaviorTreeCanvas()
        {
            _PreScale = Scale = 1;            

            this._ScaleTransform = new ScaleTransform();
            this.LayoutTransform = this._ScaleTransform;

            this.MouseWheel += new MouseWheelEventHandler(DiagramCanvas_MouseWheel);

            MemoryStream stream = new MemoryStream(Properties.Resources.zoom);
            this._ZoomCursor = new Cursor(stream);
            stream.Close();

            stream = new MemoryStream(Properties.Resources.Hand);
            this._HandCursor = new Cursor(stream);
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
            double deltaScale = Scale / _PreScale;

            double halfViewportHeight = this.Editor.ScrollViewer.ViewportHeight / 2;
            double newVerticalOffset = ((this.Editor.ScrollViewer.VerticalOffset + halfViewportHeight) * deltaScale - halfViewportHeight);

            double halfViewportWidth = this.Editor.ScrollViewer.ViewportWidth / 2;
            double newHorizontalOffset = ((this.Editor.ScrollViewer.HorizontalOffset + halfViewportWidth) * deltaScale - halfViewportWidth);


            this._ScaleTransform.ScaleX = Scale;
            this._ScaleTransform.ScaleY = Scale;

            this.Editor.ScrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
            this.Editor.ScrollViewer.ScrollToVerticalOffset(newVerticalOffset);
            _PreScale = _Scale;
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
                        Cursor = this._HandCursor;
                    else
                        Cursor = this._ZoomCursor;
                }                
            }
        }

        private void Move(double deltaX, double deltaY)
        {
            Editor.ScrollViewer.ScrollToHorizontalOffset(Editor.ScrollViewer.HorizontalOffset - deltaX * 0.8);
            Editor.ScrollViewer.ScrollToVerticalOffset(Editor.ScrollViewer.VerticalOffset - deltaY * 0.8);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if ((Keyboard.Modifiers & (ModifierKeys.Alt)) != ModifierKeys.None && _StartDrag != null && (e.LeftButton == MouseButtonState.Pressed ^ e.MiddleButton == MouseButtonState.Pressed))
            {                
                Point p = e.GetPosition(this);

                if (!(double.IsInfinity(p.X) || double.IsNaN(p.X) || double.IsInfinity(p.Y) || double.IsNaN(p.Y)))
                {
                    if (_StartDrag != null)
                    {
                        double deltaX = p.X - _StartDrag.Value.X;
                        double deltaY = p.Y - _StartDrag.Value.Y;

                        if (e.MiddleButton == MouseButtonState.Pressed)
                        {
                            Cursor = this._HandCursor;
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
            }
            else
            {
                Cursor = Cursors.Arrow;
                _StartDrag = null;                
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
        public static BehaviorTreeCanvas GetCanvas(DependencyObject element)
        {
            while (element != null && !(element is BehaviorTreeCanvas))
                element = VisualTreeHelper.GetParent(element);

            return element as BehaviorTreeCanvas;
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
