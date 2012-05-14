using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

namespace Skill.Studio.Diagram
{
    public class DragableContent : UserControl, ISelectable, INotifyPropertyChanged, IDiagramObject
    {

        #region Properties

        public event EventHandler SelectedChanged;
        protected virtual void OnSelectedChanged()
        {
            if (SelectedChanged != null)
                SelectedChanged(this, EventArgs.Empty);
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                if (value != IsSelected)
                {
                    SetValue(IsSelectedProperty, value);
                    if (value)
                        BorderBrush = SelectedBrush;
                    else
                        BorderBrush = ContentBorderBrush;
                    OnSelectedChanged();
                }
            }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool), typeof(DragableContent), new FrameworkPropertyMetadata(false));

        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }
        public static readonly DependencyProperty SelectedBrushProperty =
          DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(DragableContent), new FrameworkPropertyMetadata(Brushes.Yellow));

        public Brush ContentBorderBrush
        {
            get { return (Brush)GetValue(ContentBorderBrushProperty); }
            set { SetValue(ContentBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentBorderBrushProperty =
            DependencyProperty.Register("ContentBorderBrush", typeof(Brush), typeof(DragableContent), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(50, 255, 255, 255))));


        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(DragableContent));




        // while drag connection procedure is ongoing and the mouse moves over 
        // this item this value is true; if true the ConnectorDecorator is triggered
        // to be visible, see template
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set { SetValue(IsDragConnectionOverProperty, value); }
        }
        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                         typeof(bool),
                                         typeof(DragableContent),
                                         new FrameworkPropertyMetadata(false));


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

        public DragableContent()
        {
            _Connectors = new List<Connector>();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            // update selection
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


        private List<Connector> _Connectors;
        internal void Add(Connector c)
        {
            if (_Connectors.Contains(c))
                return;
            _Connectors.Add(c);
        }
        internal bool Remove(Connector c)
        {
            return _Connectors.Remove(c);
        }

        public IEnumerable<Connector> Connectors { get { return _Connectors; } }

        public Connector GetConnectorByIndex(int index)
        {
            foreach (var c in _Connectors)
            {
                if (c.Index == index)
                    return c;
            }
            return null;
        }

        // iterate through visual tree to get parent DesignerCanvas
        public static DragableContent GetDragableContent(DependencyObject element)
        {
            while (element != null && !(element is DragableContent))
                element = VisualTreeHelper.GetParent(element);

            return element as DragableContent;
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

        public void Disconnect()
        {
            Connector[] arr = _Connectors.ToArray();
            foreach (var c in arr)
                c.Disconnect();
            if (ParentCanvas != null)
                ParentCanvas.Children.Remove(this);
            _ParentCanvas = null;
        }
    }
}
