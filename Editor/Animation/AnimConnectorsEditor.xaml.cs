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

namespace Skill.Editor.Animation
{
    internal interface IAnimConnectorContainer
    {
        ObservableCollection<AnimConnectorProperties> Connectors { get; }
    }


    /// <summary>
    /// Interaction logic for AnimConnectorsEditor.xaml
    /// </summary>
    public partial class AnimConnectorsEditor : UserControl, ITypeEditor
    {
        public AnimConnectorsEditor()
        {
            InitializeComponent();
        }

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            //Binding binding = new Binding("Value");
            //binding.Source = propertyItem;
            //binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            //BindingOperations.SetBinding(this, FloatCollectionEditor.ValueProperty, binding);

            Skill.Editor.Animation.IAnimConnectorContainer container = propertyItem.Instance as Skill.Editor.Animation.IAnimConnectorContainer;
            if (container != null)
                _ListBox.ItemsSource = container.Connectors;
            return this;
        }
    }
}
