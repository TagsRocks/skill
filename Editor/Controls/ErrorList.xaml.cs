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

namespace Skill.Editor.Controls
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
        private ObservableCollection<CompileError> _AllErrors;
        private SortType _Sort;
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
            _AllErrors = new ObservableCollection<CompileError>();
            _ListView.ItemsSource = Errors;
            _Sort = SortType.AscendingOrder;
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
            CheckProjectErrors();
            CheckProjectNodeErrors();
            SortErrors();
            SetButtonTexts();
        }

        private void CheckProjectNodeErrors()
        {
            var project = MainWindow.Instance.Project;
            if (project == null) return;
            CheckForErrors(MainWindow.Instance.Project.RootVM);

        }

        private void CheckForErrors(EntityNodeViewModel vm)
        {
            if (vm.EntityType == EntityType.BehaviorTree)
            {
                string fileName = System.IO.Path.Combine(MainWindow.Instance.Project.Directory, vm.LocalFileName);
                if (System.IO.File.Exists(fileName))
                {
                    AI.BehaviorTree tree = AI.BehaviorTree.Load(fileName);
                    SearchForDuplicateNames(vm, tree);
                    SearchForInvalidAccessKeys(vm, tree);
                }
            }
            else if (vm.EntityType == EntityType.AnimationTree)
            {
                string fileName = System.IO.Path.Combine(MainWindow.Instance.Project.Directory, vm.LocalFileName);
                if (System.IO.File.Exists(fileName))
                {
                    Animation.AnimationTree tree = Animation.AnimationTree.Load(fileName);
                    SearchForDuplicateNames(vm, tree);
                    CheckUnusedAnimNodes(vm, tree);
                }
            }
            else
            {
                foreach (EntityNodeViewModel item in vm)
                {
                    CheckForErrors(item);
                }
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

        #region Shared Animation methods
        private bool IsAnyNodeConnectedTo(Animation.AnimNode node, Animation.AnimationTree tree)
        {
            foreach (Animation.AnimationConnection connection in tree.Connections)
            {
                if (connection.SinkId == node.Id)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Check for Unused AnimNodes
        private void CheckUnusedAnimNodes(EntityNodeViewModel vm, Animation.AnimationTree tree)
        {
            foreach (Animation.AnimNode node in tree)
            {
                bool connected = IsAnyNodeConnectedTo(node, tree);
                if (connected == false)
                {
                    if (node.NodeType == Animation.AnimNodeType.Root)
                        AddAnimationRootIsNotInvalidError(vm);
                    else if (node.NodeType != Animation.AnimNodeType.Sequence)
                        AddAnimNodeUnusedWarning(vm, node);
                }
            }
        }
        void AddAnimationRootIsNotInvalidError(EntityNodeViewModel node)
        {
            CompileError err = new CompileError() { Node = node, Type = ErrorType.Error };
            err.Description = string.Format("Root of AnimationTree does not assigned.");
            _AllErrors.Add(err);
        }

        void AddAnimNodeUnusedWarning(EntityNodeViewModel node, Animation.AnimNode animNode)
        {
            CompileError war = new CompileError() { Node = node, Type = ErrorType.Warning };
            war.Description = string.Format("There is no connection to AnimNode '{0}'.", animNode.Name);
            _AllErrors.Add(war);
        }
        #endregion

        #region Search for duplicate entities in project

        private void CheckProjectErrors()
        {
            var project = MainWindow.Instance.Project;
            if (project == null) return;

            SearchForDuplicateNames(project.RootVM);
        }

        class NodeCount
        {
            public EntityNodeViewModel Node { get; set; }
            public int count { get; set; }
        }

        void AddDuplicateNodeError(NodeCount nc)
        {
            CompileError err = new CompileError() { Node = nc.Node, Type = ErrorType.Error };
            err.Description = string.Format("There are {0} file in project with same name ({1}).", nc.count, nc.Node.Name);
            _AllErrors.Add(err);
        }

        private void SearchForDuplicateNames(ProjectRootViewModel vm)
        {
            List<NodeCount> nodes = new List<NodeCount>();
            AddChilds(vm, nodes);
            foreach (var nc in nodes)
            {
                if (nc.count > 1)
                    AddDuplicateNodeError(nc);
            }
        }

        private void AddChilds(EntityNodeViewModel vm, List<NodeCount> nodes)
        {
            foreach (EntityNodeViewModel child in vm)
            {
                if (child.EntityType != EntityType.Folder)
                {
                    NodeCount nc = nodes.FirstOrDefault(n => n.Node.Name == child.Name);
                    if (nc != null)
                        nc.count++;
                    else
                    {
                        nc = new NodeCount() { Node = child, count = 1 };
                        nodes.Add(nc);
                    }
                }
                AddChilds(child, nodes);
            }
        }
        #endregion

        #region SearchForDuplicateNames in BehaviorTree
        void AddDuplicateBehaviorTreeNodeError(EntityNodeViewModel node, string name, int count)
        {
            CompileError err = new CompileError() { Node = node, Type = ErrorType.Error };
            err.Description = string.Format("There are {0} behavior node in BehaviorTree with same name ({1}).", count, name);
            _AllErrors.Add(err);
        }

        private void SearchForDuplicateNames(EntityNodeViewModel node, AI.BehaviorTree tree)
        {
            List<string> nameList = new List<string>(tree.Count);
            foreach (AI.Behavior b in tree)
            {
                if (nameList.Contains(b.Name)) continue;
                int count = tree.Count(c => c.Name == b.Name);
                if (count > 1)
                    AddDuplicateBehaviorTreeNodeError(node, b.Name, count);
                nameList.Add(b.Name);
            }
            nameList.Clear();
        }

        private void SearchForInvalidAccessKeys(EntityNodeViewModel vm, AI.BehaviorTree tree)
        {
            foreach (AI.Behavior b in tree)
            {
                if (b.BehaviorType == AI.BehaviorType.Decorator)
                {
                    AI.Decorator decorator = (AI.Decorator)b;
                    if (decorator.Type == AI.DecoratorType.AccessLimit)
                    {
                        AI.AccessLimitDecorator accessLimitDecorator = (AI.AccessLimitDecorator)decorator;
                        if (!tree.AccessKeys.ContainsKey(accessLimitDecorator.AccessKey))
                        {
                            AddInvalidAccessKeyError(vm, accessLimitDecorator.AccessKey, accessLimitDecorator.Name);
                        }
                    }
                }
            }
        }
        void AddInvalidAccessKeyError(EntityNodeViewModel node, string accessKey, string name)
        {
            CompileError err = new CompileError() { Node = node, Type = ErrorType.Error };
            err.Description = string.Format("The provided AccessKey '{0}' for behavior node '{1}' does not exist.", accessKey, name);
            _AllErrors.Add(err);
        }
        #endregion

        #region SearchForDuplicateNames in AnimationTree
        void AddDuplicateAnimationTreeNodeError(EntityNodeViewModel node, string name, int count)
        {
            CompileError err = new CompileError() { Node = node, Type = ErrorType.Error };
            err.Description = string.Format("There are {0} animation node in AnimationTree with same name ({1}).", count, name);
            _AllErrors.Add(err);
        }

        private void SearchForDuplicateNames(EntityNodeViewModel node, Animation.AnimationTree tree)
        {
            List<string> nameList = new List<string>(tree.Count);
            foreach (Animation.AnimNode an in tree)
            {
                if (nameList.Contains(an.Name)) continue;
                int count = tree.Count(c => c.Name == an.Name);
                if (count > 1)
                    AddDuplicateAnimationTreeNodeError(node, an.Name, count);
                nameList.Add(an.Name);
            }
            nameList.Clear();
        }
        #endregion

        #endregion
    }
}
