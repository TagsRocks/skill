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

namespace Skill.Studio.Animation.Editor
{
    /// <summary>
    /// Interaction logic for SelectSkinMeshWindow.xaml
    /// </summary>
    public partial class SelectAnimTreeWindow : Window
    {
        private AnimNodeSubTreeViewModel _SubTree;

        public SelectAnimTreeWindow()
        {
            InitializeComponent();
        }
        public SelectAnimTreeWindow(AnimNodeSubTreeViewModel subTree)
        {
            InitializeComponent();
            this._SubTree = subTree;
            if (MainWindow.Instance.IsProjectLoaded)
            {
                List<string> addressList = MainWindow.Instance.Project.GetAddresses(EntityType.AnimationTree);

                foreach (var at in addressList)
                {
                    if (at == _SubTree.Tree.Editor.ViewModel.LocalPath)
                    {
                        addressList.Remove(at);
                        break;
                    }
                }

                _CmbAddress.ItemsSource = addressList;

                if (!string.IsNullOrEmpty(_SubTree.TreeAddress))
                {
                    for (int i = 0; i < addressList.Count; i++)
                    {
                        if (addressList[i] == _SubTree.TreeAddress)
                        {
                            _CmbAddress.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (_CmbAddress.SelectedItem != null)
            {
                _SubTree.TreeAddress = _CmbAddress.SelectedItem as string;
                Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Please select an AnimationTree");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
