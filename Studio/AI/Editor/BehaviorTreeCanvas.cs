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
    class BehaviorTreeCanvas : Canvas, INotifyPropertyChanged
    {

        public IBehaviorTreeGraphView Viewer
        {
            get { return (IBehaviorTreeGraphView)GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Editor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register("Viewer", typeof(IBehaviorTreeGraphView), typeof(BehaviorTreeCanvas), new UIPropertyMetadata(null, Editor_Changed));

        static void Editor_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BehaviorTreeCanvas canvas = d as BehaviorTreeCanvas;
            if (canvas != null)
            {
                IBehaviorTreeGraphView editor = e.NewValue as IBehaviorTreeGraphView;
                if (editor != null)
                {
                    editor.UpdateTreeNodes += new EventHandler(canvas.Editor_UpdateTreeNodes);
                }
            }
        }

        void Editor_UpdateTreeNodes(object sender, EventArgs e)
        {
            this.InvalidateMeasure();
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

            if (Viewer != null)
                Viewer.UpdateNodes();
            return size;
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
