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
using System.ComponentModel;
using System.Collections.ObjectModel;
using Skill.DataModels.AI;

namespace Skill.Studio.AI.Editor
{
    /// <summary>
    /// Interaction logic for AccessKeysControl.xaml
    /// </summary>
    public partial class SharedAccessKeysEditorControl : UserControl, INotifyPropertyChanged
    {
        private bool _HasSelected;
        public bool HasSelected
        {
            get { return _HasSelected; }
            set
            {
                if (_HasSelected != value)
                {
                    _HasSelected = value;
                    OnPropertyChanged("HasSelected");
                }
            }
        }

        public SharedAccessKeysViewModel ViewModel
        {
            get { return (SharedAccessKeysViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SharedAccessKeysViewModel), typeof(SharedAccessKeysEditorControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ViewModel_Changed)));


        static void ViewModel_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SharedAccessKeysEditorControl control = d as SharedAccessKeysEditorControl;
            if (d != null)
            {
                SharedAccessKeysViewModel vm = e.NewValue as SharedAccessKeysViewModel;
                if (vm != null)
                {
                    control._LbKeys.ItemsSource = vm.Keys;
                    if (vm.Keys.Count > 0)
                        control._LbKeys.SelectedIndex = 0;
                }
            }
        }

        public SharedAccessKeysEditorControl()
        {
            InitializeComponent();
        }

        private string GetValidKeyName(string keyName)
        {
            int i = 0;
            string name = keyName + i;
            while (ViewModel.Keys.Count(p => p.Key == name) > 0)
            {
                i++;
                name = keyName + i;
            }
            return name;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_CmbKeys.SelectedItem != null)
            {
                AccessKeyType keyType = (AccessKeyType)((ComboBoxItem)_CmbKeys.SelectedItem).Tag;
                switch (keyType)
                {
                    case AccessKeyType.CounterLimit:
                        ViewModel.AddAccessKey(new CounterLimitAccessKey() { Key = GetValidKeyName("CounterLimit") });
                        break;
                    case AccessKeyType.TimeLimit:
                        ViewModel.AddAccessKey(new TimeLimitAccessKey() { Key = GetValidKeyName("TimeLimit"), TimeInterval = 1 });
                        break;
                }
                _LbKeys.SelectedIndex = _LbKeys.Items.Count - 1;
            }
        }

        private void LbKeys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSelected = _LbKeys.SelectedItem != null;
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (_LbKeys.SelectedItem != null)
            {
                int index = _LbKeys.SelectedIndex;
                if (ViewModel.RemoveAccessKey(_LbKeys.SelectedItem as AccessKeyViewModel))
                {
                    index = Math.Max(0, index - 1);
                    if (_LbKeys.Items.Count > index)
                        _LbKeys.SelectedIndex = index;                    
                }
            }
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

        private void LbKeys_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (_LbKeys.SelectedItem != null)
                    ViewModel.RemoveAccessKey(_LbKeys.SelectedItem as AccessKeyViewModel);
            }
        }
    }
}
