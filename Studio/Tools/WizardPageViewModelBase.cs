using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Skill.Studio.Tools
{
    /// <summary>
    /// Abstract base class for all pages shown in the wizard.
    /// </summary>
    public abstract class WizardPageViewModelBase<T> : INotifyPropertyChanged where T : class
    {
        #region Fields

        private bool _IsCurrentPage;

        #endregion // Fields

        #region Events
        /// <summary> occurs when page get focus </summary>
        public event EventHandler Enter;
        /// <summary> occurs when page lost focus </summary>
        public event EventHandler Leave;
        #endregion

        #region Properties

        public T Data { get; private set; }

        public abstract string DisplayName { get; }

        public bool IsCurrentPage
        {
            get { return _IsCurrentPage; }
            set
            {
                if (value != _IsCurrentPage)
                {
                    _IsCurrentPage = value;
                    if (_IsCurrentPage)
                        OnEnter();
                    else
                        OnLeave();
                    this.OnPropertyChanged("IsCurrentPage");                    
                }
            }
        }

        protected virtual void OnEnter() { if (Enter != null) Enter(this, EventArgs.Empty); }
        protected virtual void OnLeave() { if (Leave != null) Leave(this, EventArgs.Empty); }

        /// <summary>
        /// Returns true if the user has filled in this page properly
        /// and the wizard should allow the user to progress to the 
        /// next page in the workflow.
        /// </summary>
        public abstract bool IsValid { get; }

        #endregion // Properties

        #region Constructor

        protected WizardPageViewModelBase(T data)
        {
            this.Data = data;
        }

        #endregion // Constructor        

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
