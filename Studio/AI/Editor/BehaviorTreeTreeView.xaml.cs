using Skill.DataModels.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Skill.Studio.AI.Editor
{
    /// <summary>
    /// Interaction logic for BehaviorTreeTreeView.xaml
    /// </summary>
    public partial class BehaviorTreeTreeView : BehaviorTreeViewControl , INotifyPropertyChanged
    {

        public BehaviorViewModel SelectedItem
        {
            get
            {
                return _BTTree.SelectedItem as BehaviorViewModel;
            }           
        }        

        public BehaviorTreeTreeView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
            }
        }

        private void TreeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
            }
        }

        private void _BTTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Editor != null)
            {
                Editor.SelectedItemChanged();
            }
        }        
    }
}
