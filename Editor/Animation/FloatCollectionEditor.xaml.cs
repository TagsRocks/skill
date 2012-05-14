using System;
using System.Collections.Generic;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using System.Collections.ObjectModel;

namespace Skill.Studio.Animation
{
    internal interface IFloatCollectionContainer
    {
        ObservableCollection<FloatContainer> FloatCollection { get; }
    }


    /// <summary>
    /// Interaction logic for FloatCollectionEditor.xaml
    /// </summary>
    public partial class FloatCollectionEditor : UserControl, ITypeEditor
    {
        public FloatCollectionEditor()
        {
            InitializeComponent();
        }

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            Skill.Studio.Animation.IFloatCollectionContainer container = propertyItem.Instance as Skill.Studio.Animation.IFloatCollectionContainer;
            if (container != null)
                _ListBox.ItemsSource = container.FloatCollection;
            return this;
        }
    }
}
