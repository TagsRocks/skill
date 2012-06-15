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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AvalonDock;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Skill.DataModels.Animation;
using Skill.DataModels.AI;
using Skill.Studio.Compiler;

namespace Skill.Studio.Controls
{
    /// <summary>
    /// Interaction logic for ErrorList.xaml
    /// </summary>
    public partial class ErrorList : DockableContent
    {
        #region SortType
        private enum SortType
        {
            AscendingCategory,
            DescendingCategory,
            AscendingOrder,
            DescendingOrder,
            AscendingDescription,
            DescendingDescription,
            AscendingFile,
            DescendingFile,

        }
        #endregion

        #region Variables
        private List<CompileError> _AllErrors;
        private SortType _Sort;
        #endregion

        #region Compilers
        private ProjectCompiler _ProjectCompiler;
        private BehaviorTreeCompiler _BehaviorTreeCompiler;
        private AnimationTreeCompiler _AnimationTreeCompiler;
        private SkinMeshCompiler _SkinMeshCompiler;
        private SaveGameCompiler _SaveGameCompiler;        
        #endregion

        #region Properties
        public ObservableCollection<CompileError> Errors { get; private set; }
        #endregion

        #region Methods
        public int GetErrorCount(ErrorType errortype)
        {
            return _AllErrors.Count(e => (e.Type & errortype) != 0);
        }
        public void GetErrorCount(out int errCount, out int warCount, out int msgCount)
        {
            errCount = 0;
            warCount = 0;
            msgCount = 0;
            foreach (var item in _AllErrors)
            {
                switch (item.Type)
                {
                    case ErrorType.Error:
                        errCount++;
                        break;
                    case ErrorType.Warning:
                        warCount++;
                        break;
                    case ErrorType.Message:
                        msgCount++;
                        break;
                }
            }
        }
        #endregion

        #region Constructor
        public ErrorList()
        {
            InitializeComponent();
            Errors = new ObservableCollection<CompileError>();
            _AllErrors = new List<CompileError>();
            _ListView.ItemsSource = Errors;
            _Sort = SortType.AscendingOrder;

            this._ProjectCompiler = new ProjectCompiler(_AllErrors);
            this._BehaviorTreeCompiler = new BehaviorTreeCompiler(_AllErrors);
            this._AnimationTreeCompiler = new AnimationTreeCompiler(_AllErrors);
            this._SkinMeshCompiler = new SkinMeshCompiler(_AllErrors);
            this._SaveGameCompiler = new SaveGameCompiler(_AllErrors);
        }

        #endregion

        #region Sort Errors
        private void _BtnErrors_Click(object sender, RoutedEventArgs e)
        {
            SortErrors();
        }

        private void _BtnWarnings_Click(object sender, RoutedEventArgs e)
        {
            SortErrors();
        }

        private void _BtnMessages_Click(object sender, RoutedEventArgs e)
        {
            SortErrors();
        }

        private void SortCategory_Click(object sender, RoutedEventArgs e)
        {
            if (_Sort == SortType.AscendingCategory)
                _Sort = SortType.DescendingCategory;
            else
                _Sort = SortType.AscendingCategory;
            SortErrors();
        }

        private void SortOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_Sort == SortType.AscendingOrder)
                _Sort = SortType.DescendingOrder;
            else
                _Sort = SortType.AscendingOrder;
            SortErrors();
        }

        private void SortDescription_Click(object sender, RoutedEventArgs e)
        {
            if (_Sort == SortType.AscendingDescription)
                _Sort = SortType.DescendingDescription;
            else
                _Sort = SortType.AscendingDescription;
            SortErrors();
        }

        private void SortFile_Click(object sender, RoutedEventArgs e)
        {
            if (_Sort == SortType.AscendingFile)
                _Sort = SortType.DescendingFile;
            else
                _Sort = SortType.AscendingFile;
            SortErrors();
        }

        private void SortErrors()
        {
            Errors.Clear();

            ErrorType errType = ErrorType.None;
            if (_BtnErrors.IsChecked != null && _BtnErrors.IsChecked.Value) errType |= ErrorType.Error;
            if (_BtnWarnings.IsChecked != null && _BtnWarnings.IsChecked.Value) errType |= ErrorType.Warning;
            if (_BtnMessages.IsChecked != null && _BtnMessages.IsChecked.Value) errType |= ErrorType.Message;

            IEnumerable<CompileError> query = null;

            switch (_Sort)
            {
                case SortType.AscendingCategory:
                    query = _AllErrors.Where(e => (e.Type & errType) != 0).OrderBy(e => e.Type);
                    break;
                case SortType.DescendingCategory:
                    query = _AllErrors.Where(e => (e.Type & errType) != 0).OrderByDescending(e => e.Type);
                    break;
                case SortType.AscendingOrder:
                    query = _AllErrors.Where(e => (e.Type & errType) != 0).OrderBy(e => e.Order);
                    break;
                case SortType.DescendingOrder:
                    query = _AllErrors.Where(e => (e.Type & errType) != 0).OrderByDescending(e => e.Order);
                    break;
                case SortType.AscendingDescription:
                    query = _AllErrors.Where(e => (e.Type & errType) != 0).OrderBy(e => e.Description);
                    break;
                case SortType.DescendingDescription:
                    query = _AllErrors.Where(e => (e.Type & errType) != 0).OrderByDescending(e => e.Description);
                    break;
                case SortType.AscendingFile:
                    query = _AllErrors.Where(e => (e.Type & errType) != 0).OrderBy(e => e.File);
                    break;
                case SortType.DescendingFile:
                    query = _AllErrors.Where(e => (e.Type & errType) != 0).OrderByDescending(e => e.File);
                    break;
                default:
                    break;
            }

            foreach (var e in query)
            {
                Errors.Add(e);
            }
        }

        #endregion

        #region CheckForErrors
        public void CheckForErrors()
        {
            if (MainWindow.Instance.Project == null) return;
            CompileError.ResetCounter();
            _AllErrors.Clear();
            CheckErrors();
            SortErrors();
            SetButtonTexts();
        }

        private void CheckErrors()
        {
            CheckErrors(MainWindow.Instance.Project.Root);
        }

        private void CheckErrors(EntityNodeViewModel vm)
        {
            switch (vm.EntityType)
            {
                case EntityType.Root:
                    _ProjectCompiler.Compile(vm);
                    break;
                case EntityType.BehaviorTree:
                    _BehaviorTreeCompiler.Compile(vm);
                    break;
                case EntityType.SharedAccessKeys:
                    _BehaviorTreeCompiler.AccessKeysCompiler.Compile(vm);
                    break;
                case EntityType.AnimationTree:
                    _AnimationTreeCompiler.Compile(vm);
                    break;
                case EntityType.SkinMesh:
                    _SkinMeshCompiler.Compile(vm);
                    break;
                case EntityType.SaveGame:
                    _SaveGameCompiler.Compile(vm);
                    break;
                default:
                    break;
            }
            foreach (EntityNodeViewModel item in vm)
            {
                CheckErrors(item);
            }
        }

        private void SetButtonTexts()
        {
            int errCount, warCount, msgCount;
            GetErrorCount(out errCount, out warCount, out msgCount);
            _TxtErrors.Text = (errCount > 0) ? string.Format("Errors ({0})", errCount) : "Errors";
            _TxtWarnings.Text = (warCount > 0) ? string.Format("Warnings ({0})", warCount) : "Warnings";
            _TxtMessages.Text = (msgCount > 0) ? string.Format("Messages ({0})", msgCount) : "Messages";
        }

        #endregion
    }
}
