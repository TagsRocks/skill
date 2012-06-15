using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Skill.Studio.Animation.Editor
{
    class AnimNodeBlendBySpeedInputsPropertyEditor: Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        private AnimNodeBlendBySpeedViewModel _Node;
        private TextBlock _TextBlock;

        public AnimNodeBlendBySpeedInputsPropertyEditor()
        {
            _TextBlock = new TextBlock();
            _TextBlock.Cursor = Cursors.IBeam;
            _TextBlock.Padding = new Thickness(5, 0, 5, 0);
            _TextBlock.Text = "Connectors {...}";
            _TextBlock.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(_TextBlock_MouseLeftButtonUp);
        }

        void _TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_Node != null)
            {
                AnimNodeBlendBySpeedInputsEditor window = new AnimNodeBlendBySpeedInputsEditor(_Node);
                window.Owner = MainWindow.Instance;
                window.ShowDialog();
            }
        }

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            _Node = propertyItem.Instance as AnimNodeBlendBySpeedViewModel;
            return _TextBlock;
        }
    }
}
