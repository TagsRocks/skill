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

namespace Skill.Studio.AI.Editor
{
    /// <summary>
    /// Interaction logic for SelectAccessKeyWindow.xaml
    /// </summary>
    public partial class SelectAccessKeyWindow : Window
    {
        AccessLimitDecoratorViewModel _Decorator;

        public SelectAccessKeyWindow()
            : this(null)
        {
        }
        public SelectAccessKeyWindow(AccessLimitDecoratorViewModel decorator)
        {
            InitializeComponent();
            this._Decorator = decorator;
            if (MainWindow.Instance.IsProjectLoaded)
            {
                List<string> addressList = MainWindow.Instance.Project.GetAddresses(EntityType.SharedAccessKeys);
                addressList.Insert(0, "Internal");
                _CmbAddress.ItemsSource = addressList;

                if (!string.IsNullOrEmpty(_Decorator.AccessKey))
                {
                    if (string.IsNullOrEmpty(_Decorator.Address) || _Decorator.Address == "Internal")
                        _CmbAddress.SelectedIndex = 0;
                    else
                    {
                        for (int i = 0; i < addressList.Count; i++)
                        {
                            if (addressList[i] == _Decorator.Address)
                            {
                                _CmbAddress.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (_CmbAddress.SelectedItem != null && _CmbKey.SelectedItem != null)
            {
                string address = _CmbAddress.SelectedItem as string;
                if (address == "Internal")
                    address = "";
                string key = _CmbKey.SelectedItem as string;
                _Decorator.Address = address;
                _Decorator.AccessKey = key;
                Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Please select an  AccessKey");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CmbAddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_CmbAddress.SelectedIndex == 0)
            {
                List<string> keys = new List<string>();
                foreach (var item in _Decorator.Tree.AccessKeys.Keys)
                {
                    keys.Add(item.Key);
                }
                _CmbKey.ItemsSource = keys;
            }
            else
            {
                SharedAccessKeysNodeViewModel node = MainWindow.Instance.Project.GetNode(_CmbAddress.SelectedItem as string) as SharedAccessKeysNodeViewModel;
                if (node != null)
                {
                    Skill.DataModels.AI.SharedAccessKeys sharedAccessKeys = node.SavedData as Skill.DataModels.AI.SharedAccessKeys;
                    if (sharedAccessKeys != null)
                    {
                        List<string> keys = new List<string>();
                        foreach (var item in sharedAccessKeys.Keys)
                        {
                            keys.Add(item.Key);
                        }
                        _CmbKey.ItemsSource = keys;
                    }
                }
            }

            if (_CmbAddress.SelectedIndex >= 0)
            {
                if (!string.IsNullOrEmpty(_Decorator.AccessKey))
                {
                    foreach (string key in _CmbKey.ItemsSource)
                    {
                        if (_Decorator.AccessKey == key)
                        {
                            _CmbKey.SelectedItem = key;
                            break;
                        }
                    }
                }
                else
                    _CmbKey.SelectedIndex = -1;
            }
            else
                _CmbKey.SelectedIndex = -1;
        }
    }
}
