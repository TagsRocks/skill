using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Skill.Studio
{
    /// <summary>
    /// Defines base class for all editor tabs
    /// </summary>
    public class TabDocument : AvalonDock.DocumentContent
    {
        #region Variables
        private EventHandler _ChangeNameHandler;
        private bool _IsChanged = true;
        #endregion

        #region Properties
        /// <summary> EntityNodeViewModel in project </summary>
        public EntityNodeViewModel ViewModel { get; private set; }

        private IDataViewModel _Data;
        /// <summary> DataViewModel  </summary>
        public IDataViewModel Data
        {
            get { return _Data; }
            protected set
            {
                if (_Data != null)
                    _Data.History = null;
                this._Data = value;
                if (_Data != null)
                    _Data.History = History;
            }

        }

        /// <summary> Take care of undo and redo </summary>
        public UnDoRedo History { get; private set; }

        /// <summary> Whether is changed or not </summary>
        public virtual bool IsChanged { get { return _IsChanged; } }
        #endregion

        #region Constructor
        public TabDocument()
        {
            History = new UnDoRedo();
        }

        public TabDocument(EntityNodeViewModel vm)
            : this()
        {
            ViewModel = vm;
            _ChangeNameHandler = ViewModel_NameChanged;
            vm.NameChanged += _ChangeNameHandler;
            //SetChanged(false);
            History.Change += new EventHandler(History_Change);

            this.Loaded += new System.Windows.RoutedEventHandler(TabDocument_Loaded);
        }

        void TabDocument_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ViewModel != null)
                SetChanged(false);
        }        

        void History_Change(object sender, EventArgs e)
        {
            SetChanged(History.ChangeCount != 0);
        }

        private void ViewModel_NameChanged(object sender, EventArgs e)
        {
            ChangeTitle();
            ChangeName(ViewModel.Name);
        }

        protected virtual void ChangeTitle()
        {
            string newTitle = ViewModel.Name + (IsChanged ? "*" : "");
            if (this.Title != newTitle) this.Title = newTitle;
        }

        public virtual void SetChanged(bool changed)
        {
            bool titleChanged = changed != _IsChanged;
            _IsChanged = changed;
            if (titleChanged)
                ChangeTitle();
        }

        /// <summary> notify that filename changed elsewhere  </summary>
        /// <param name="newName">New name of file without directory</param>
        protected virtual void ChangeName(string newName) { }
        #endregion

        #region Save
        /// <summary> Unload content </summary>
        public virtual void UnLoad()
        {
            if (ViewModel != null) ViewModel.NameChanged -= _ChangeNameHandler; ViewModel = null;
        }

        /// <summary> Save content </summary>
        public virtual void Save()
        {
            if (Data != null)
            {
                Data.CommiteChanges();
                ViewModel.SaveData(Data.GetDataModel());
                History.ResetChangeCount();
                SetChanged(false);
            }
        }
        #endregion

        #region Undo
        protected void UndoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = History.CanUndo;
            e.Handled = true;
        }
        protected void UndoCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            History.Undo();
            e.Handled = true;
        }
        #endregion

        #region Redo
        protected void RedoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = History.CanRedo;
            e.Handled = true;
        }
        protected void RedoCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            History.Redo();
            e.Handled = true;
        }
        #endregion
    }
}
