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
    public partial class SelectSkinMeshWindow : Window
    {
        private AnimationTreeRootViewModel _Root;

        public SelectSkinMeshWindow()
        {
            InitializeComponent();
        }
        public SelectSkinMeshWindow(AnimationTreeRootViewModel root)
        {
            InitializeComponent();
            this._Root = root;
            if (MainWindow.Instance.IsProjectLoaded)
            {
                List<string> addressList = MainWindow.Instance.Project.GetAddresses(EntityType.SkinMesh);                
                _CmbAddress.ItemsSource = addressList;

                if (!string.IsNullOrEmpty(_Root.SkinMesh))
                {
                    for (int i = 0; i < addressList.Count; i++)
                    {
                        if (addressList[i] == _Root.SkinMesh)
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
                _Root.SkinMesh = _CmbAddress.SelectedItem as string;
                Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Please select a SkinMesh");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }        
    }
}
