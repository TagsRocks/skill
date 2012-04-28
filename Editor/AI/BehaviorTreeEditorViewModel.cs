using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Skill.Editor.AI
{
    class BehaviorTreeEditorViewModel : INotifyPropertyChanged
    {
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
        private bool _IsRemoveAv;
        public bool IsRemoveAv
        {
            get { return _IsRemoveAv; }
            set
            {
                if (value != _IsRemoveAv)
                {
                    _IsRemoveAv = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsRemoveAv"));
                }
            }
        }
        private bool _IsCutAv;
        public bool IsCutAv
        {
            get { return _IsCutAv; }
            set
            {
                if (value != _IsCutAv)
                {
                    _IsCutAv = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsCutAv"));
                }
            }
        }
        private bool _IsPasteAv;
        public bool IsPasteAv
        {
            get { return _IsPasteAv; }
            set
            {
                if (value != _IsPasteAv)
                {
                    _IsPasteAv = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsPasteAv"));
                }
            }
        }

        private AI.BehaviorTreeViewModel _BehaviorTree;
        public AI.BehaviorTreeViewModel BehaviorTree
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
