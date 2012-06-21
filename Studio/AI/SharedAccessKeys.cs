using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.DataModels.AI;
using System.Collections.ObjectModel;

namespace Skill.Studio.AI
{
    #region AccessKey

    public abstract class AccessKeyViewModel : INotifyPropertyChanged
    {
        [Browsable(false)]
        public AccessKey Model { get; private set; }

        [Browsable(false)]
        public AccessKeyType Type { get { return Model.Type; } }

        [Browsable(false)]
        public string DisplayName { get { return string.Format("{0} ( {1} )", Type, Key); } }

        [Browsable(false)]
        public SharedAccessKeysViewModel Container { get; private set; }

        [Description("Unique name of key")]
        public string Key
        {
            get { return Model.Key; }
            set
            {
                if (Model.Key != value)
                {
                    if (Container.History != null)
                        Container.History.Insert(new ChangePropertyUnDoRedo(this, "key", value, Model.Key));
                    Model.Key = value;
                    OnPropertyChanged("Key");
                    OnPropertyChanged("DisplayName");
                }
            }
        }


        public AccessKeyViewModel(SharedAccessKeysViewModel container, AccessKey model)
        {
            this.Container = container;
            this.Model = model;
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

    public class CounterLimitAccessKeyViewModel : AccessKeyViewModel
    {
        [DefaultValue(1)]
        [Description("Maximum number of Behaviors allow access this key")]
        public int MaxAccessCount
        {
            get { return ((CounterLimitAccessKey)Model).MaxAccessCount; }
            set
            {
                if (((CounterLimitAccessKey)Model).MaxAccessCount != value)
                {
                    if (Container.History != null)
                        Container.History.Insert(new ChangePropertyUnDoRedo(this, "MaxAccessCount", value, ((CounterLimitAccessKey)Model).MaxAccessCount));

                    ((CounterLimitAccessKey)Model).MaxAccessCount = value;
                    OnPropertyChanged("MaxAccessCount");
                }
            }
        }

        public CounterLimitAccessKeyViewModel(SharedAccessKeysViewModel container, CounterLimitAccessKey model)
            : base(container, model)
        {
        }
    }

    public class TimeLimitAccessKeyViewModel : AccessKeyViewModel
    {
        [Description("The time interval between each access")]
        public float TimeInterval
        {
            get { return ((TimeLimitAccessKey)Model).TimeInterval; }
            set
            {
                if (((TimeLimitAccessKey)Model).TimeInterval != value)
                {
                    if (Container.History != null)
                        Container.History.Insert(new ChangePropertyUnDoRedo(this, "TimeInterval", value, ((TimeLimitAccessKey)Model).TimeInterval));
                    ((TimeLimitAccessKey)Model).TimeInterval = value;
                    OnPropertyChanged("TimeInterval");
                }
            }
        }

        public TimeLimitAccessKeyViewModel(SharedAccessKeysViewModel container, TimeLimitAccessKey model)
            : base(container, model)
        {
        }
    }
    #endregion

    public class SharedAccessKeysViewModel : INotifyPropertyChanged, IDataViewModel
    {

        #region Properties

        public ObservableCollection<AccessKeyViewModel> Keys { get; private set; }

        public SharedAccessKeys Model { get; private set; }

        #endregion

        #region Constructor

        public SharedAccessKeysViewModel(SharedAccessKeys model)
        {
            this.Model = model;
            this.Keys = new ObservableCollection<AccessKeyViewModel>();
            LoadKeysFromModel();
        }

        private void LoadKeysFromModel()
        {
            foreach (var key in Model.Keys)
            {
                AccessKeyViewModel keyVM = CreateAccessKeyViewModel(key);
                if (keyVM != null)
                {
                    this.Keys.Add(keyVM);
                }
            }
        }

        private AccessKeyViewModel CreateAccessKeyViewModel(AccessKey key)
        {
            switch (key.Type)
            {
                case AccessKeyType.CounterLimit:
                    return new CounterLimitAccessKeyViewModel(this, (CounterLimitAccessKey)key);
                case AccessKeyType.TimeLimit:
                    return new TimeLimitAccessKeyViewModel(this, (TimeLimitAccessKey)key);
            }
            return null;
        }
        #endregion


        #region Add & Remove
        public bool AddAccessKey(AccessKey key)
        {
            if (!string.IsNullOrEmpty(key.Key))
            {
                AccessKeyViewModel keyVM = CreateAccessKeyViewModel(key);
                return this.AddAccessKey(keyVM);
            }
            return false;
        }
        public bool RemoveAccessKey(AccessKey key)
        {
            AccessKeyViewModel keyVM = null;
            foreach (var k in Keys)
            {
                if (k.Key == key.Key)
                {
                    keyVM = k;
                    break;
                }
            }

            if (keyVM != null)
                return this.RemoveAccessKey(keyVM);
            return false;
        }

        public bool AddAccessKey(AccessKeyViewModel keyVM)
        {
            if (!string.IsNullOrEmpty(keyVM.Key))
            {
                foreach (var k in Keys)
                {
                    if (k.Key == keyVM.Key)
                        return false;
                }
                this.Keys.Add(keyVM);
                if (History != null)
                    History.Insert(new AddAccessKeyUnDoRedo(keyVM, this));
                return true;
            }
            return false;
        }
        public bool RemoveAccessKey(AccessKeyViewModel keyVM)
        {
            AccessKeyViewModel keyVMToRemove = null;
            foreach (var k in Keys)
            {
                if (k.Key == keyVM.Key)
                {
                    keyVMToRemove = k;
                    break;
                }
            }
            if (keyVMToRemove != null)
            {
                if (Keys.Remove(keyVM))
                {
                    if (History != null)
                        History.Insert(new AddAccessKeyUnDoRedo(keyVMToRemove, this, true));
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion // INotifyPropertyChanged Members

        #region IDataViewModel members
        public void NotifyEntityChange(EntityType type, string previousPath, string newPath)
        {
        }

        public UnDoRedo History { get; set; }

        public void CommiteChanges()
        {
            Model.Keys = new AccessKey[this.Keys.Count];
            for (int i = 0; i < this.Keys.Count; i++)
            {
                Model.Keys[i] = this.Keys[i].Model;
            }
        }

        public object GetDataModel()
        {
            return Model;
        }

        #endregion

    }

    #region UnDoRedo helper classes
    class AddAccessKeyUnDoRedo : IUnDoRedoCommand
    {
        AccessKeyViewModel _Newkey;
        SharedAccessKeysViewModel _Container;
        bool _Reverse;
        public AddAccessKeyUnDoRedo(AccessKeyViewModel newkey, SharedAccessKeysViewModel container, bool reverse = false)
        {
            this._Newkey = newkey;
            this._Container = container;
            this._Reverse = reverse;
        }

        public void Undo()
        {
            if (_Reverse)
                _Container.AddAccessKey(_Newkey);
            else
                _Container.RemoveAccessKey(_Newkey);
        }

        public void Redo()
        {
            if (_Reverse)
                _Container.RemoveAccessKey(_Newkey);
            else
                _Container.AddAccessKey(_Newkey);
        }
    }

    #endregion
}
