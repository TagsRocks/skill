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
    /// Interaction logic for BehaviorTreeListView.xaml
    /// </summary>
    public partial class BehaviorTreeListView : UserControl, INotifyPropertyChanged
    {
        public BehaviorTreeEditor Editor { get; set; }
        public BehaviorTreeListView()
        {
            InitializeComponent();
        }

        private BehaviorTreeViewModel _BehaviorTree;
        public virtual BehaviorTreeViewModel BehaviorTree
        {
            get { return _BehaviorTree; }
            set
            {
                if (_BehaviorTree != value)
                {
                    _BehaviorTree = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("BehaviorTree"));
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }

        #endregion // INotifyPropertyChanged Members

        private void BtnRefreshUsage_Click(object sender, RoutedEventArgs e)
        {
            CheckForIsUnused(BehaviorTree.Conditions);
            CheckForIsUnused(BehaviorTree.Decorators);
            CheckForIsUnused(BehaviorTree.Actions);
            CheckForIsUnused(BehaviorTree.ChangeStates);
            CheckForIsUnused(BehaviorTree.Composites);
            CheckForIsUnused(BehaviorTree.Behaviors);
        }

        private void CheckForIsUnused(System.Collections.ObjectModel.ObservableCollection<BehaviorViewModel> collection)
        {
            foreach (var b in collection)
                b.IsUnused = !BehaviorTree.IsInHierarchy(b);
        }

        private void BtnUnregister_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                BehaviorViewModel bvm = btn.Tag as BehaviorViewModel;
                if (bvm != null)
                {
                    bvm.IsUnused = !BehaviorTree.IsInHierarchy(bvm);
                    if (bvm.IsUnused)
                    {
                        BehaviorTree.UnRegisterViewModel(bvm);
                        if (Editor != null) Editor.SetChanged(true);
                    }
                    else
                        System.Windows.MessageBox.Show("Behavior is in use.\nplease refresh unused behaviors to see what behaviors available for delete", "Delete", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Mnu_DeleteAllUnused(object sender, RoutedEventArgs e)
        {
            System.Collections.ObjectModel.ObservableCollection<BehaviorViewModel> collection = null;
            switch (_TbChecks.SelectedIndex)
            {
                case 0:
                    collection = BehaviorTree.Conditions;
                    break;
                case 1:
                    collection = BehaviorTree.Decorators;
                    break;
                case 2:
                    collection = BehaviorTree.Actions;
                    break;
                case 3:
                    collection = BehaviorTree.ChangeStates;
                    break;
                case 4:
                    collection = BehaviorTree.Composites;
                    break;
                default:
                    return;
            }
            if (collection != null)
            {
                CheckForIsUnused(collection);
                List<BehaviorViewModel> btoremove = new List<BehaviorViewModel>();
                foreach (var b in collection)
                {
                    if (b.IsUnused)
                        btoremove.Add(b);
                }
                if (btoremove.Count > 0)
                {
                    foreach (var b in btoremove)
                        BehaviorTree.UnRegisterViewModel(b);
                    if (Editor != null) Editor.SetChanged(true);
                }
            }
        }
    }
}
