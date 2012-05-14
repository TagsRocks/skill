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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Skill.Studio.AI
{
    /// <summary>
    /// Interaction logic for AccessKeyEditor.xaml
    /// </summary>
    public partial class AccessKeyEditor : Window, INotifyPropertyChanged
    {
        private Dictionary<string, AccessKey> _DestinationAccessKeys;
        public ObservableCollection<AccessKeyViewModel> _Keys;

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

        public AccessKeyEditor()
            : this(null)
        {
        }

        public AccessKeyEditor(Dictionary<string, AccessKey> destinationAccessKeys)
        {
            this._DestinationAccessKeys = destinationAccessKeys;
            InitializeComponent();

            _Keys = new ObservableCollection<AccessKeyViewModel>();
            if (destinationAccessKeys != null)
            {
                foreach (var item in destinationAccessKeys)
                {
                    switch (item.Value.Type)
                    {
                        case AccessKeyType.CounterLimit:
                            _Keys.Add(new CounterLimitAccessKeyViewModel(item.Value as CounterLimitAccessKey));
                            break;
                        case AccessKeyType.TimeLimit:
                            _Keys.Add(new TimeLimitAccessKeyViewModel(item.Value as TimeLimitAccessKey));
                            break;
                    }
                }
            }

            _LbKeys.ItemsSource = _Keys;
            if (_Keys.Count > 0)
                _LbKeys.SelectedIndex = 0;
        }


        private string GetValidKeyName(string keyName)
        {
            int i = 0;
            string name = keyName + i;
            while (_Keys.Count(p => p.Key == name) > 0)
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
                        _Keys.Add(new CounterLimitAccessKeyViewModel(new CounterLimitAccessKey() { Key = GetValidKeyName("CounterLimit") }));
                        break;
                    case AccessKeyType.TimeLimit:
                        _Keys.Add(new TimeLimitAccessKeyViewModel(new TimeLimitAccessKey() { Key = GetValidKeyName("TimeLimit"), TimeInterval = 1 }));
                        break;
                }
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (_LbKeys.SelectedItem != null)
                _Keys.Remove(_LbKeys.SelectedItem as AccessKeyViewModel);
        }



        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (_DestinationAccessKeys != null)
            {
                foreach (var key in _Keys)
                {
                    string accessKey = key.Key;
                    if (_Keys.Count(p => p.Key == accessKey) > 1)
                    {
                        MessageBox.Show("There is duplicated key : " + accessKey);
                        return;
                    }
                }

                _DestinationAccessKeys.Clear();
                foreach (var key in _Keys)
                {
                    _DestinationAccessKeys.Add(key.Key, key.Model);
                }
            }
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LbKeys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSelected = _LbKeys.SelectedItem != null;
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
    }
}
