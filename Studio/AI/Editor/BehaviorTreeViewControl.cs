using Skill.DataModels.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Skill.Studio.AI.Editor
{
    public interface IBehaviorActionAV
    {
        bool IsNewAv { get; set; }
        bool IsMoveUpAv { get; set; }
        bool IsMoveDownAv { get; set; }
        bool IsDeleteAv { get; set; }
        bool IsCopyAv { get; set; }
    }

    public class BehaviorTreeViewControl : UserControl, INotifyPropertyChanged, IBehaviorActionAV
    {
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

        public BehaviorTreeEditor Editor { get; set; }

        #region Menu Availability
        private bool _IsNewAv;
        public bool IsNewAv
        {
            get { return _IsNewAv; }
            set
            {
                if (value != _IsNewAv)
                {
                    _IsNewAv = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsNewAv"));
                }
            }
        }

        private bool _IsMoveUpAv;
        public bool IsMoveUpAv
        {
            get { return _IsMoveUpAv; }
            set
            {
                if (value != _IsMoveUpAv)
                {
                    _IsMoveUpAv = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsMoveUpAv"));
                }
            }
        }

        private bool _IsMoveDownAv;
        public bool IsMoveDownAv
        {
            get { return _IsMoveDownAv; }
            set
            {
                if (value != _IsMoveDownAv)
                {
                    _IsMoveDownAv = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsMoveDownAv"));
                }
            }
        }

        private bool _IsDeleteAv;
        public bool IsDeleteAv
        {
            get { return _IsDeleteAv; }
            set
            {
                if (value != _IsDeleteAv)
                {
                    _IsDeleteAv = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsDeleteAv"));
                }
            }
        }

        private bool _IsCopyAv;
        public bool IsCopyAv
        {
            get { return _IsCopyAv; }
            set
            {
                if (value != _IsCopyAv)
                {
                    _IsCopyAv = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsCopyAv"));
                }
            }
        }
        #endregion

        #region Move item
        protected void Mnu_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (Editor != null)
                Editor.MoveSelectedUp();
        }
        protected void Mnu_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (Editor != null)
                Editor.MoveSelectedDown();
        }
        #endregion

        #region Add new item

        protected void Mnu_NewAction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                if (Editor != null)
                    Editor.AddAction();
            }
        }

        protected void Mnu_NewChangeState_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                if (Editor != null)
                    Editor.AddChangeState();
            }
        }

        protected void Mnu_NewCondition_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                if (Editor != null)
                    Editor.AddCondition();
            }
        }

        protected void Mnu_NewDecorator_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                if (Editor != null)
                {
                    MenuItem mnu = (MenuItem)sender;
                    Editor.AddDecorator((DecoratorType)mnu.Tag);
                }
            }
        }

        protected void Mnu_NewComposite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                if (Editor != null)
                {
                    MenuItem mnu = (MenuItem)sender;
                    Editor.AddComposite((CompositeType)mnu.Tag);
                }
            }
        }

        #endregion

        #region Insert

        protected void Mnu_Insert_Click(object sender, RoutedEventArgs e)
        {
            if (IsNewAv)
            {
                MenuItem menu = sender as MenuItem;
                if (menu != null)
                {
                    BehaviorViewModel insertItem = menu.Tag as BehaviorViewModel;
                    if (Editor != null)
                        Editor.Insert(insertItem);
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }

        #endregion // INotifyPropertyChanged Members
    }
}
