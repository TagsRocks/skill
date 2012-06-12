using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;

namespace Skill.Studio.AI.Editor
{
    public class AccessKeyPropertyEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        private AccessLimitDecoratorViewModel _Decorator;
        private TextBlock _TextBlock;

        public AccessKeyPropertyEditor()
        {
            _TextBlock = new TextBlock();
            _TextBlock.Cursor = Cursors.IBeam;
            _TextBlock.Padding = new Thickness(5,0,5,0);
            _TextBlock.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(_TextBlock_MouseLeftButtonUp);
        }

        void _TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_Decorator != null)
            {
                SelectAccessKeyWindow window = new SelectAccessKeyWindow(_Decorator);
                window.Owner = MainWindow.Instance;
                window.ShowDialog();
            }
        }

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            _Decorator = propertyItem.Instance as AccessLimitDecoratorViewModel;
            //create the binding from the bound property item to the editor
            var binding = new Binding("Value"); //bind to the Value property of the PropertyItem
            binding.Source = propertyItem;
            binding.ValidatesOnExceptions = true;
            binding.ValidatesOnDataErrors = true;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(_TextBlock, TextBlock.TextProperty, binding);
            return _TextBlock;
        }
    }
}
