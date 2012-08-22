using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Skill.Studio.Tools
{
    /// <summary>
    /// The main ViewModel class for the wizard.
    /// This class contains the various pages shown
    /// in the workflow and provides navigation
    /// between the pages.
    /// </summary>
    public abstract class WizardViewModel<T> : INotifyPropertyChanged where T : class
    {
        #region Fields

        private RelayCommand _CancelCommand;
        private RelayCommand _MoveNextCommand;
        private RelayCommand _MovePreviousCommand;

        private WizardPageViewModelBase<T> _CurrentPage;
        private ReadOnlyCollection<WizardPageViewModelBase<T>> _Pages;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Returns the cup of coffee ordered by the customer.
        /// If this returns null, the user cancelled the order.
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Returns the page ViewModel that the user is currently viewing.
        /// </summary>
        public WizardPageViewModelBase<T> CurrentPage
        {
            get { return _CurrentPage; }
            private set
            {
                if (value == _CurrentPage)
                    return;

                if (_CurrentPage != null)
                    _CurrentPage.IsCurrentPage = false;

                _CurrentPage = value;

                if (_CurrentPage != null)
                    _CurrentPage.IsCurrentPage = true;

                this.OnPropertyChanged("CurrentPage");
                this.OnPropertyChanged("IsOnLastPage");
            }
        }

        /// <summary>
        /// Returns true if the user is currently viewing the last page 
        /// in the workflow.  This property is used by CoffeeWizardView
        /// to switch the Next button's text to "Finish" when the user
        /// has reached the final page.
        /// </summary>
        public bool IsOnLastPage
        {
            get { return this.CurrentPageIndex == this.Pages.Count - 1; }
        }

        /// <summary>
        /// Returns a read-only collection of all page ViewModels.
        /// </summary>
        public ReadOnlyCollection<WizardPageViewModelBase<T>> Pages
        {
            get
            {
                if (_Pages == null)
                    _Pages = this.CreatePages();

                return _Pages;
            }
        }

        public int CurrentPageIndex
        {
            get
            {

                if (this.CurrentPage == null)
                {
                    System.Diagnostics.Debug.Fail("Why is the current page null?");
                    return -1;
                }
                return this.Pages.IndexOf(this.CurrentPage);
            }
        }

        #endregion // Properties

        #region Constructor

        public WizardViewModel()
        {
            this.Data = CreateData();
            this.CurrentPage = this.Pages[0];
        }

        #endregion // Constructor

        #region Commands

        #region CancelCommand

        /// <summary>
        /// Returns the command which, when executed, cancels the order 
        /// and causes the Wizard to be removed from the user interface.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                    _CancelCommand = new RelayCommand(() => this.CancelOrder());

                return _CancelCommand;
            }
        }

        void CancelOrder()
        {
            Data = null;
            this.OnRequestClose();
        }

        #endregion // CancelCommand

        #region MovePreviousCommand

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the previous page in the workflow.
        /// </summary>
        public ICommand MovePreviousCommand
        {
            get
            {
                if (_MovePreviousCommand == null)
                    _MovePreviousCommand = new RelayCommand(
                        () => this.MoveToPreviousPage(),
                        () => this.CanMoveToPreviousPage);

                return _MovePreviousCommand;
            }
        }

        bool CanMoveToPreviousPage
        {
            get { return 0 < this.CurrentPageIndex; }
        }

        void MoveToPreviousPage()
        {
            if (this.CanMoveToPreviousPage)
                this.CurrentPage = this.Pages[this.CurrentPageIndex - 1];
        }

        #endregion // MovePreviousCommand

        #region MoveNextCommand

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the next page in the workflow.  If the user
        /// is viewing the last page in the workflow, this causes the Wizard
        /// to finish and be removed from the user interface.
        /// </summary>
        public ICommand MoveNextCommand
        {
            get
            {
                if (_MoveNextCommand == null)
                    _MoveNextCommand = new RelayCommand(
                        () => this.MoveToNextPage(),
                        () => this.CanMoveToNextPage);

                return _MoveNextCommand;
            }
        }

        bool CanMoveToNextPage
        {
            get { return this.CurrentPage != null && this.CurrentPage.IsValid; }
        }

        void MoveToNextPage()
        {
            if (this.CanMoveToNextPage)
            {
                if (this.CurrentPageIndex < this.Pages.Count - 1)
                    this.CurrentPage = this.Pages[this.CurrentPageIndex + 1];
                else
                    this.OnRequestClose();
            }
        }

        #endregion // MoveNextCommand

        #endregion // Commands

        #region Events

        /// <summary>
        /// Raised when the wizard should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        #endregion // Events

        #region Methods
        protected abstract ReadOnlyCollection<WizardPageViewModelBase<T>> CreatePages();
        protected abstract T CreateData();

        protected virtual void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public virtual void Dispose() { }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
