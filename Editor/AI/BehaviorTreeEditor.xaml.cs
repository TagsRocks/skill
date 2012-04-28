﻿using System;
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
using Skill.Editor.AI;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Skill.Editor.AI
{
    /// <summary>
    /// Interaction logic for BehaviorTreeEditor.xaml
    /// </summary>
    public partial class BehaviorTreeEditor : TabDocument
    {
        #region SelectedItem
        /// <summary>
        /// Get selected Behavior
        /// </summary>
        /// <returns>selected Behavior</returns>
        private BehaviorViewModel GetSelectedItem()
        {
            if (_BTTree.SelectedItem != null)
            {
                if (_BTTree.SelectedItem is BehaviorViewModel)
                {
                    return (BehaviorViewModel)_BTTree.SelectedItem;
                }
            }
            return null;
        }
        #endregion

        #region Menu Availability
        void CheckContextMnuAvailability()
        {
            if (_ViewModel == null) return;

            BehaviorViewModel selected = GetSelectedItem();
            if (selected != null)
            {
                _ViewModel.IsRemoveAv = _ViewModel.IsCutAv = selected != BehaviorTree.Root;
                if (selected.Model.BehaviorType == BehaviorType.Composite)
                {
                    _ViewModel.IsNewAv = true;
                }
                else if (selected.Model.BehaviorType == BehaviorType.Decorator)
                {
                    _ViewModel.IsNewAv = selected.Count == 0;
                }
                else
                {
                    _ViewModel.IsNewAv = false;
                }

                _ViewModel.IsMoveUpAv = selected.CanMoveUp;
                _ViewModel.IsMoveDownAv = selected.CanMoveDown;
                _ViewModel.IsPasteAv = _CuttedBehavior != null && _ViewModel.IsNewAv;

            }
            else
            {
                _ViewModel.IsNewAv = false;
                _ViewModel.IsMoveDownAv = false;
                _ViewModel.IsMoveUpAv = false;
                _ViewModel.IsRemoveAv = false;
                _ViewModel.IsCutAv = false;
                _ViewModel.IsPasteAv = false;
            }
        }
        #endregion

        #region Variables
        private string _FileName; // full address of BehaviorTree filename
        private BehaviorTreeEditorViewModel _ViewModel; // view model
        bool _IsChanged;
        BehaviorViewModel _CuttedBehavior;
        #endregion

        #region Properties
        /// <summary> BehaviorTree ViewModel </summary>
        public BehaviorTreeViewModel BehaviorTree { get { return _ViewModel.BehaviorTree; } }
        /// <summary> Whether BehaviorTree is changed? </summary>
        public override bool IsChanged { get { return _IsChanged; } }
        /// <summary> BehaviorTree filename</summary>
        public override string FileName { get { return _FileName; } }
        /// <summary> Is Cut action available </summary>
        public override bool CanCut { get { return _ViewModel.IsCutAv; } }
        /// <summary> Is Paste action available </summary>
        public override bool CanPaste { get { return _ViewModel.IsPasteAv; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a BehaviorTreeEditor in design mode
        /// </summary>
        public BehaviorTreeEditor()
        {
            InitializeComponent();
            _ViewModel = new BehaviorTreeEditorViewModel();
            this.DataContext = _ViewModel;
        }
        /// <summary>
        /// Create a BehaviorTreeEditor
        /// </summary>
        /// <param name="filename">BehaviorTree filename</param>
        public BehaviorTreeEditor(string filename)
            : this()
        {
            this._FileName = filename;
            BehaviorTree tree = AI.BehaviorTree.Load(filename);
            _ViewModel.BehaviorTree = new BehaviorTreeViewModel(tree, this.History);
            ChangeTitle();
            History.Change += new EventHandler(History_Change);
            History.UndoChange += new EventHandler(History_UndoChange);
            History.RedoChange += new EventHandler(History_RedoChange);
        }
        #endregion

        #region History events

        private void ResetCut()
        {
            if (_CuttedBehavior != null)
                _CuttedBehavior.Cutting = false;
            _CuttedBehavior = null;
            _ViewModel.IsPasteAv = false;
        }
        // hook events of History
        void History_RedoChange(object sender, EventArgs e)
        {
            ResetCut();
        }

        void History_UndoChange(object sender, EventArgs e)
        {
            ResetCut();
        }

        // when any change occurs in history, update title
        void History_Change(object sender, EventArgs e)
        {
            _IsChanged = History.ChangeCount != 0;
            ChangeTitle();
        }
        #endregion

        #region Title
        private void ChangeTitle()
        {
            string newTitle = System.IO.Path.GetFileNameWithoutExtension(_FileName) + (IsChanged ? "*" : "");
            if (this.Title != newTitle) this.Title = newTitle;
        }
        #endregion

        #region Selected item changed
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
            }
        }

        private void _BTTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CheckContextMnuAvailability();
            ApplicationCommands.Properties.Execute(_BTTree.SelectedItem, null);
        }
        private void TreeViewItem_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Escape)
            {
                ResetCut();
                e.Handled = true;
            }
        }
        #endregion


        #region UpdateConnections
        private void UpdateConnections()
        {            
            this._ViewModel.BehaviorTree.Root.UpdateConnection();
        } 
        #endregion

        #region Insert
        private void Mnu_Insert_Click(object sender, RoutedEventArgs e)
        {
            if (_ViewModel.IsNewAv)
            {
                if (sender is TextBlock)
                {
                    TextBlock textBlock = (TextBlock)sender;
                    var selected = GetSelectedItem();
                    if (selected != null)
                    {
                        try
                        {
                            var newVM = selected.AddBehavior((BehaviorViewModel)textBlock.Tag);
                            if (newVM != null)
                                newVM.IsSelected = true;
                            ResetCut();
                            UpdateConnections();
                        }
                        catch (Exception ex)
                        {
                            MainWindow.Instance.ShowError(ex.Message);
                        }
                    }
                }
            }
        }        
        #endregion

        #region New
        private void AddNonSelector(BehaviorType behaviorType)
        {
            if (_ViewModel.IsNewAv)
            {
                var selected = GetSelectedItem();
                if (selected != null)
                {
                    var newVM = selected.AddNonSelector(behaviorType);
                    if (newVM != null)
                        newVM.IsSelected = true;
                    UpdateConnections();
                }
            }
        }

        private void AddSelector(CompositeType selectorType)
        {
            if (_ViewModel.IsNewAv)
            {
                var selected = GetSelectedItem();
                if (selected != null)
                {
                    var newVM = selected.AddSelector(selectorType);
                    if (newVM != null)
                        newVM.IsSelected = true;
                    UpdateConnections();
                }
            }
        }

        private void Mnu_NewNonSelector_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                MenuItem mnu = (MenuItem)sender;
                AddNonSelector((BehaviorType)mnu.Tag);
                ResetCut();
            }
        }

        private void Mnu_NewSelector_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                MenuItem mnu = (MenuItem)sender;
                AddSelector((CompositeType)mnu.Tag);
                ResetCut();
            }
        }
        #endregion

        #region Move
        private void Mnu_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (_ViewModel.IsMoveUpAv)
            {
                var selected = GetSelectedItem();
                if (selected != null && selected.CanMoveUp)
                {
                    if (selected.Parent != null)
                    {
                        ((BehaviorViewModel)selected.Parent).MoveUp(selected);
                        ResetCut();
                        UpdateConnections();
                    }
                }

            }
        }

        private void Mnu_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (_ViewModel.IsMoveDownAv)
            {
                var selected = GetSelectedItem();
                if (selected != null && selected.CanMoveDown)
                {
                    if (selected.Parent != null)
                    {
                        ((BehaviorViewModel)selected.Parent).MoveDown(selected);
                        ResetCut();
                        UpdateConnections();
                    }
                }
            }
        }
        #endregion

        #region Remove
        private void Mnu_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                var selected = GetSelectedItem();
                if (selected != null)
                {
                    if (selected.Parent != null)
                    {
                        if (selected.Parent is BehaviorViewModel)
                        {
                            ((BehaviorViewModel)selected.Parent).RemoveBehavior(selected);
                            ResetCut();
                            UpdateConnections();
                        }
                    }
                    CheckContextMnuAvailability();
                }
            }
        }
        #endregion

        #region UnLoad
        public override void UnLoad()
        {
            _ViewModel.BehaviorTree = null;
        }
        #endregion

        #region Save
        public override void Save()
        {
            if (_ViewModel.BehaviorTree != null)
            {
                _ViewModel.BehaviorTree.Model.Save(_FileName);
                _IsChanged = false;
                History.ResetChangeCount();
                ChangeTitle();
                ResetCut();
            }
        }
        #endregion

        #region ChangeName
        /// <summary> Change name of BehaviorTree </summary>
        /// <param name="newName">new name of behaviortree filename</param>
        public override void OnChangeName(string newName)
        {
            string dir = System.IO.Path.GetDirectoryName(_FileName);
            string ext = System.IO.Path.GetExtension(_FileName);
            _FileName = System.IO.Path.Combine(dir, newName + ext);
            ChangeTitle();
        }
        #endregion

        #region Cut
        public override void Cut()
        {
            var selected = GetSelectedItem();
            if (selected != null)
            {
                ResetCut();
                _CuttedBehavior = selected;
                _CuttedBehavior.Cutting = true;
                CheckContextMnuAvailability();
            }
        }
        #endregion

        #region Paste
        public override void Paste()
        {
            if (_CuttedBehavior != null)
            {
                var selected = GetSelectedItem();
                if (selected != null)
                {
                    try
                    {
                        ((BehaviorViewModel)_CuttedBehavior.Parent).RemoveBehavior(_CuttedBehavior);
                        var newVM = selected.AddBehavior(_CuttedBehavior, false);
                        if (newVM != null)
                            newVM.IsSelected = true;
                    }
                    catch (Exception ex)
                    {
                        MainWindow.Instance.ShowError(ex.Message);
                    }
                    ResetCut();
                    CheckContextMnuAvailability();

                }
            }
        }
        #endregion

        private void BehaviorsTab_GotFocus(object sender, RoutedEventArgs e)
        {            
            if (BehaviorTree != null)
                BehaviorTree.CheckContainersIsInHierarchy();
        }

        private void BtnDelBContainer_Click(object sender, RoutedEventArgs e)
        {
            if (BehaviorTree == null) return;
            Button btn = e.Source as Button;
            if (btn != null)
            {
                BehaviorViewModelContainer bc = btn.Tag as BehaviorViewModelContainer;
                if (bc != null)
                {
                    BehaviorTree.RemoveBehaviorContainer(bc);
                    e.Handled = true;
                }

            }
        }
    }
}
