﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Data;

namespace Skill.Studio.Animation.Editor
{
    public class TreeAddressPropertyEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        private AnimNodeSubTreeViewModel _SubTree;
        private TextBlock _TextBlock;

        public TreeAddressPropertyEditor()
        {
            _TextBlock = new TextBlock();
            _TextBlock.Cursor = Cursors.IBeam;
            _TextBlock.Padding = new Thickness(5, 0, 5, 0);
            _TextBlock.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(_TextBlock_MouseLeftButtonUp);
        }

        void _TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_SubTree != null)
            {
                SelectAnimTreeWindow window = new SelectAnimTreeWindow(_SubTree);
                window.Owner = MainWindow.Instance;
                window.ShowDialog();
            }
        }

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            _SubTree = propertyItem.Instance as AnimNodeSubTreeViewModel;
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
