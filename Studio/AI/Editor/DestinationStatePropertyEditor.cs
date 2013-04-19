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
    public class DestinationStatePropertyEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        private ChangeStateViewModel _ChangeState;
        private ComboBox _ComboBox;
        private bool _IgnoreChange = false;

        void _ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_ChangeState != null && !_IgnoreChange)
            {
                _IgnoreChange = true;
                _ChangeState.DestinationState = _ComboBox.SelectedItem as string;
                _IgnoreChange = false;
            }
        }

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            _ChangeState = propertyItem.Instance as ChangeStateViewModel;
            _ComboBox = new ComboBox();
            RefreshComboBox();
            _ComboBox.SelectionChanged += _ComboBox_SelectionChanged;
            ((System.ComponentModel.INotifyPropertyChanged)_ChangeState).PropertyChanged += DestinationStatePropertyEditor_PropertyChanged;
            return _ComboBox;
        }

        private void RefreshComboBox()
        {
            if (_ChangeState != null)
            {
                _IgnoreChange = true;
                _ComboBox.Items.Clear();                
                int selectedIndex = 0;
                if (_ChangeState.Tree != null)
                {
                    for (int i = 0; i < _ChangeState.Tree.States.Count; i++)
                    {
                        string name = _ChangeState.Tree.States[i].Name;
                        _ComboBox.Items.Add(name);
                        if (name == _ChangeState.DestinationState)
                            selectedIndex = i;
                    }
                }
                _ComboBox.SelectedIndex = selectedIndex;
                _IgnoreChange = false;
            }
        }

        void DestinationStatePropertyEditor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!_IgnoreChange && e.PropertyName == "DestinationState")
            {
                RefreshComboBox();
            }
        }
    }
}
