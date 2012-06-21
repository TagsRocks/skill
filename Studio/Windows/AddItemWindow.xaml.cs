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

namespace Skill.Studio
{
    /// <summary>
    /// Interaction logic for AddItemWindow.xaml
    /// </summary>
    public partial class AddItemWindow : Window
    {
        public EntityType? NewItemType { get; private set; }
        public string NewItemName { get; private set; }

        private List<NewItemViewModel> _Items;
        private ObservableCollection<NewItemViewModel> _VisibleItems;

        public AddItemWindow(EntityType? defaultType, string defaultName)
            : this()
        {
            if (defaultType != null)
            {
                for (int i = 0; i < _VisibleItems.Count; i++)
                {
                    if (_VisibleItems[i].EntityType == defaultType.Value)
                    {
                        _LbItems.SelectedIndex = i;
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(defaultName))
                _TxtNewItemName.Text = defaultName;
        }

        public AddItemWindow()
        {
            InitializeComponent();
            _VisibleItems = new ObservableCollection<NewItemViewModel>();
            _Items = new List<NewItemViewModel>();
            CreateItems();
            RefreshVisibleList(ItemCategory.All);
            _LbItems.ItemsSource = _VisibleItems;
        }

        private void CreateItems()
        {
            _Items.Add(new NewItemViewModel() { Name = "BehaviorTree", EntityType = Studio.EntityType.BehaviorTree, ImageName = Images.BehaviorTree, Category = ItemCategory.AI });
            _Items.Add(new NewItemViewModel() { Name = "SharedAccessKeys", EntityType = Studio.EntityType.SharedAccessKeys, ImageName = Images.SharedAccessKeys, Category = ItemCategory.AI });
            _Items.Add(new NewItemViewModel() { Name = "AnimationTree", EntityType = Studio.EntityType.AnimationTree, ImageName = Images.AnimationTree, Category = ItemCategory.Animation });
            _Items.Add(new NewItemViewModel() { Name = "SkinMesh", EntityType = Studio.EntityType.SkinMesh, ImageName = Images.SkinMesh, Category = ItemCategory.Animation });
            _Items.Add(new NewItemViewModel() { Name = "SaveData", EntityType = Studio.EntityType.SaveData, ImageName = Images.SaveData, Category = ItemCategory.IO});
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void Add()
        {
            if (string.IsNullOrEmpty(_TxtNewItemName.Text))
            {
                System.Windows.MessageBox.Show("Enter valid name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_LbItems.SelectedItem != null)
            {
                NewItemViewModel item = _LbItems.SelectedItem as NewItemViewModel;
                if (item != null)
                {
                    NewItemType = item.EntityType;
                    NewItemName = System.IO.Path.GetFileNameWithoutExtension(_TxtNewItemName.Text);
                }
            }
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NewItemType = null;
            NewItemName = null;
            DialogResult = false;
            Close();
        }

        private void RefreshVisibleList(ItemCategory cat)
        {
            if (_VisibleItems == null) return;
            _VisibleItems.Clear();
            foreach (var item in _Items)
            {
                if ((item.Category & cat) != 0)
                {
                    _VisibleItems.Add(item);
                }
            }
        }

        private void CategoryChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_LbCategories.SelectedItem != null)
            {
                ListBoxItem item = _LbCategories.SelectedItem as ListBoxItem;
                if (item != null)
                {
                    ItemCategory cat = (ItemCategory)item.Tag;
                    RefreshVisibleList(cat);
                }
            }
        }                
    }

    public class NewItemViewModel
    {
        public string ImageName { get; set; }
        public string Name { get; set; }
        public EntityType EntityType { get; set; }
        public ItemCategory Category { get; set; }
    }

    public enum ItemCategory
    {
        AI = 1,
        Animation = 2,
        IO = 4,
        All = AI | Animation | IO,
    }
}
