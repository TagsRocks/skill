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
using System.ComponentModel;
using Skill.DataModels.AI;

namespace Skill.Studio.AI.Editor
{
    /// <summary>
    /// Interaction logic for AccessKeyEditor.xaml
    /// </summary>
    public partial class SharedAccessKeysEditorWindow : Window, INotifyPropertyChanged
    {
        private UnDoRedo _History;

        public bool IsChanged { get { return _History.ChangeCount != 0; } }

        private SharedAccessKeysViewModel _SharedAccessKeys;
        public SharedAccessKeysViewModel SharedAccessKeys
        {
            get { return _SharedAccessKeys; }
            set
            {
                if (_SharedAccessKeys != value)
                {
                    _SharedAccessKeys = value;
                    OnPropertyChanged("SharedAccessKeys");
                }
            }
        }

        public SharedAccessKeysEditorWindow()
            : this(null)
        {
        }

        public SharedAccessKeysEditorWindow(SharedAccessKeysViewModel viewModle)
        {
            InitializeComponent();
            _History = new UnDoRedo();
            if (viewModle != null)
            {
                this.SharedAccessKeys = viewModle;
                viewModle.History = _History;
            }
        }


        #region Undo
        void UndoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _History.CanUndo;
        }
        void UndoCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            _History.Undo();
        }
        #endregion

        #region Redo
        void RedoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _History.CanRedo;
        }
        void RedoCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            _History.Redo();
        }
        #endregion


        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
