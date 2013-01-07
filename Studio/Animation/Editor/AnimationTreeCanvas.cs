using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Documents;

namespace Skill.Studio.Animation.Editor
{
    public class AnimationTreeCanvas : Canvas, INotifyPropertyChanged
    {
        
        public AnimationTreeEditor Editor
        {
            get { return (AnimationTreeEditor)GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Editor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register("Editor", typeof(AnimationTreeEditor), typeof(AnimationTreeCanvas), new UIPropertyMetadata(null, Editor_Changed));

        static void Editor_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimationTreeCanvas canvas = d as AnimationTreeCanvas;
            if (canvas != null)
            {
                AnimationTreeEditor editor = e.NewValue as AnimationTreeEditor;
                if (editor != null)
                {                    
                }
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
        public static AnimationTreeCanvas GetCanvas(DependencyObject element)
        {
            while (element != null && !(element is AnimationTreeCanvas))
                element = VisualTreeHelper.GetParent(element);

            return element as AnimationTreeCanvas;
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
