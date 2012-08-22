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
    public class AnimationPropertyEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        private ActionViewModel _Action;
        private TextBlock _TextBlock;

        public AnimationPropertyEditor()
        {
            _TextBlock = new TextBlock();
            _TextBlock.Cursor = Cursors.IBeam;
            _TextBlock.Padding = new Thickness(5, 0, 5, 0);
            _TextBlock.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(_TextBlock_MouseLeftButtonUp);
        }

        void _TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_Action != null)
            {
                GifBrowser browser = new GifBrowser(_Action.Animation);
                browser.Owner = MainWindow.Instance;
                browser.ShowDialog();
                _Action.Animation = browser.Animation;
            }
        }

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            _Action = propertyItem.Instance as ActionViewModel;

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
