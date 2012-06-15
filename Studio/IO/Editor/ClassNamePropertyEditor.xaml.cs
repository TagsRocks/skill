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

namespace Skill.Studio.IO.Editor
{
    /// <summary>
    /// Interaction logic for ClassNamePropertyEditor.xaml
    /// </summary>
    public partial class ClassNamePropertyEditor : UserControl, Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        private ClassPropertyViewModel _Property;

        public ClassNamePropertyEditor()
        {
            InitializeComponent();
        }

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            _Property = propertyItem.Instance as ClassPropertyViewModel;

            _CmbClasses.ItemsSource = _Property.OwnerClass.SaveGame.Classes;

            bool found = false;
            for (int i = 0; i < _Property.OwnerClass.SaveGame.Classes.Count; i++)
            {
                if (_Property.ClassName == _Property.OwnerClass.SaveGame.Classes[i].Name)
                {
                    _CmbClasses.SelectedIndex = i;
                    found = true;
                    break;
                }
            }
            if (!found)
                _CmbClasses.SelectedIndex = -1;

            return this;
        }

        private void CmbClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveClassViewModel saveClass = _CmbClasses.SelectedItem as SaveClassViewModel;
            if (saveClass != null)
                _Property.ClassName = saveClass.Name;
            else
                _Property.ClassName = "";
        }
    }
}
