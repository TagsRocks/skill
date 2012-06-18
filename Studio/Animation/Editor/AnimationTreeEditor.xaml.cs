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
using Skill.DataModels.Animation;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Skill.DataModels;

namespace Skill.Studio.Animation.Editor
{
    /// <summary>
    /// Interaction logic for AnimationTreeEditor.xaml
    /// </summary>
    public partial class AnimationTreeEditor : TabDocument
    {
        #region Commands
        static RoutedCommand _PasteCommand = new RoutedCommand();
        public static RoutedCommand PasteCommand { get { return _PasteCommand; } }
        #endregion

        #region Variables
        private bool _RefreshSelection;
        private Point _CreatePosition;
        #endregion

        #region Properties

        public AnimationTreeViewModel AnimationTree { get; private set; }

        public CompositeCollection Elements { get; private set; }

        public Selection Selection { get; private set; }

        public ScrollViewer ScrollViewer { get { return _DesignerScrollViewer; } }

        #endregion

        #region Constructor
        public AnimationTreeEditor()
            : this(null)
        {
        }

        /// <summary>
        /// Create an AnimationTreeEditor
        /// </summary>
        /// <param name="viewModel"></param>
        public AnimationTreeEditor(AnimationTreeNodeViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            Elements = new CompositeCollection();
            Selection = new Selection();
            this.DataContext = this;
            if (viewModel != null)
            {
                Skill.DataModels.Animation.AnimationTree at = viewModel.LoadData() as Skill.DataModels.Animation.AnimationTree;
                if (at != null)
                {
                    Data = this.AnimationTree = new AnimationTreeViewModel(at) { Editor = this };

                    Elements.Add(new CollectionContainer() { Collection = this.AnimationTree.Nodes });
                    Elements.Add(new CollectionContainer() { Collection = this.AnimationTree.Connections });
                }
            }

            this.Loaded += new RoutedEventHandler(AnimationTreeEditor_Loaded);
            this.Selection.Change += new EventHandler(Selection_Change);
        }

        void Selection_Change(object sender, EventArgs e)
        {
            if (Selection.SelectedObjects.Count == 1)
            {
                AnimNodeViewModel vm = Selection.SelectedObjects[0] as AnimNodeViewModel;
                if (vm != null)
                    System.Windows.Input.ApplicationCommands.Properties.Execute(vm, null);
                else
                    System.Windows.Input.ApplicationCommands.Properties.Execute(null, null);
            }
            else
            {
                //System.Windows.Input.ApplicationCommands.Properties.Execute(Selection.CurrentSelection, null);
                System.Windows.Input.ApplicationCommands.Properties.Execute(null, null);
            }
        }

        void AnimationTreeEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.AnimationTree != null)
            {
                foreach (var item in this.AnimationTree)
                {
                    item.RefreshConnections();
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (AnimationTree != null)
            {
                _DesignerScrollViewer.ScrollToHorizontalOffset(AnimationTree.HorizontalOffset);
                _DesignerScrollViewer.ScrollToVerticalOffset(AnimationTree.VerticalOffset);
            }
        }
        #endregion

        #region Save

        public override void Save()
        {
            AnimationTree.HorizontalOffset = _DesignerScrollViewer.HorizontalOffset;
            AnimationTree.VerticalOffset = _DesignerScrollViewer.VerticalOffset;
            base.Save();
        }
        #endregion

        #region Create Node

        private void Mnu_Create_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            if (mnu != null && mnu.Tag is AnimNodeType)
            {
                AnimNodeType nodeType = (AnimNodeType)mnu.Tag;
                AnimNodeViewModel newNode = CreateNode(nodeType);
                if (newNode != null)
                {
                    SetChanged(true);
                    Selection.Select(newNode);
                    e.Handled = true;
                }

            }
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = e.OriginalSource as Canvas;
            if (canvas != null)
            {
                _CreatePosition = e.GetPosition(canvas);
            }
        }


        private AnimNodeViewModel CreateNode(AnimNodeType nodeType)
        {
            AnimNode node = Skill.DataModels.Animation.AnimationTree.CreateNode(nodeType);
            node.Name = AnimationTree.GetValidName(node.Name);
            AnimNodeViewModel vm = AnimationTree.CreateViewModel(node);

            if (vm != null)
            {
                vm.Y = _CreatePosition.Y;
                vm.X = _CreatePosition.X;
                AnimationTree.Add(vm);
            }
            return vm;
        }
        #endregion

        #region Unload
        /// <summary> Unload content </summary>
        public override void UnLoad()
        {
            Selection.Clear();
            base.UnLoad();
        }
        #endregion

        #region OnMouseMove

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_RefreshSelection)
            {
                foreach (AnimNodeViewModel item in Selection.SelectedObjects)
                {
                    item.RefreshConnections();
                }
                _RefreshSelection = false;
            }
            base.OnMouseMove(e);
        }
        #endregion

        #region Edit

        class AnimNodeToCopy
        {
            public int Id;
            public AnimNodeViewModel Source;
        }

        private void CopySelectionToClipboard(bool samePosition = false)
        {
            XDocument document = new XDocument();

            double xMin = double.MaxValue;
            double yMin = double.MaxValue;

            // find AnimNodeViewModel s to copy            
            List<AnimNodeToCopy> animNodesToCopy = new List<AnimNodeToCopy>();
            foreach (AnimNodeViewModel animnode in Selection.SelectedObjects)
            {
                if (animnode.NodeType == AnimNodeType.Root) continue;

                xMin = Math.Min(xMin, animnode.X);
                yMin = Math.Min(yMin, animnode.Y);
                animNodesToCopy.Add(new AnimNodeToCopy() { Id = animNodesToCopy.Count, Source = animnode });
            }

            List<AnimationConnectionInfo> connectionsToCopy = new List<AnimationConnectionInfo>();
            for (int i = 0; i < animNodesToCopy.Count; i++)
            {
                for (int j = 0; j < animNodesToCopy.Count; j++)
                {
                    if (i == j) continue;

                    var query = this.AnimationTree.Connections.Where(c => c.Source == animNodesToCopy[i].Source && c.Sink == animNodesToCopy[j].Source);
                    foreach (var connection in query)
                    {
                        AnimationConnectionInfo connectionInfo = new AnimationConnectionInfo() { SourceId = animNodesToCopy[i].Id, SinkId = animNodesToCopy[j].Id, SinkConnectorIndex = connection.SinkConnectorIndex };
                        connectionsToCopy.Add(connectionInfo);
                    }
                }
            }

            XElement copyData = new XElement("CopyData");
            copyData.SetAttributeValue("SamePosition", samePosition);

            XElement nodes = new XElement("Nodes");
            foreach (var node in animNodesToCopy)
            {
                XElement e = node.Source.Model.ToXElement();
                e.SetAttributeValue("Id", node.Id);

                XElement ui = e.FindChildByName("UI");
                if (!samePosition)
                {                    
                    ui.SetAttributeValue("X", node.Source.X - xMin);
                    ui.SetAttributeValue("Y", node.Source.Y - yMin);
                }
                else
                {
                    ui.SetAttributeValue("X", node.Source.X + 30);
                    ui.SetAttributeValue("Y", node.Source.Y + 30);
                }
                nodes.Add(e);
            }
            copyData.Add(nodes);

            XElement connections = new XElement("Connections");
            foreach (var connection in connectionsToCopy)
            {
                connections.Add(connection.ToXElement());
            }
            copyData.Add(connections);

            document.Add(copyData);
            Clipboard.Clear();
            Clipboard.SetData(Skill.DataModels.SkillDataFormats.AnimNode, document.ToString());
        }


        void CutCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }
        void CutCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Selection.SelectedObjects.Count > 0;
            e.Handled = true;
        }
        void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            CopySelectionToClipboard();
            e.Handled = true;
        }


        void PasteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }
        void PasteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }


        void PasteCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(Skill.DataModels.SkillDataFormats.AnimNode);
            e.Handled = true;
        }
        void PasteCommandExecuted(object target, ExecutedRoutedEventArgs e)
        {
            PasteFromClipboard();
            e.Handled = true;
        }


        private AnimNodeViewModel FindById(List<AnimNodeViewModel> list, int id)
        {
            foreach (var item in list)
            {
                if (item.Id == id)
                    return item;
            }
            return null;
        }

        private List<AnimNodeViewModel> PasteFromClipboard(bool addToSelection = true)
        {

            if (Clipboard.ContainsData(Skill.DataModels.SkillDataFormats.AnimNode))
            {
                string text = Clipboard.GetData(Skill.DataModels.SkillDataFormats.AnimNode) as string;
                if (!string.IsNullOrEmpty(text))
                {
                    try
                    {

                        XDocument document = XDocument.Parse(text);
                        XElement copyData = document.Elements().First();

                        bool samePosition = copyData.GetAttributeValueAsBoolean("SamePosition", false);

                        XElement nodes = copyData.FindChildByName("Nodes");
                        if (nodes != null)
                        {
                            List<AnimNodeViewModel> nodesToPaste = new List<AnimNodeViewModel>();
                            foreach (var item in nodes.Elements())
                            {
                                AnimNode an = Skill.DataModels.Animation.AnimationTree.CreateNode(item);
                                if (an != null)
                                {
                                    an.Load(item);
                                    if (!samePosition)
                                    {
                                        an.X += _CreatePosition.X;
                                        an.Y += _CreatePosition.Y;
                                    }
                                    nodesToPaste.Add(this.AnimationTree.CreateViewModel(an));
                                }
                            }

                            if (nodesToPaste.Count > 0)
                            {
                                List<AnimationConnectionViewModel> connectionsToPaste = new List<AnimationConnectionViewModel>();

                                XElement connections = copyData.FindChildByName("Connections");
                                if (connections != null)
                                {
                                    foreach (var item in connections.Elements())
                                    {
                                        AnimationConnectionInfo info = new AnimationConnectionInfo();
                                        info.Load(item);

                                        AnimNodeViewModel source = FindById(nodesToPaste, info.SourceId);
                                        AnimNodeViewModel sink = FindById(nodesToPaste, info.SinkId);

                                        if (source == null || sink == null) continue;

                                        connectionsToPaste.Add(new AnimationConnectionViewModel(AnimationTree, source, sink, info.SinkConnectorIndex));
                                    }
                                }


                                this.AnimationTree.Add(nodesToPaste.ToArray(), connectionsToPaste.ToArray());


                                if (addToSelection)
                                {
                                    Selection.Clear();
                                    foreach (var item in nodesToPaste)
                                    {
                                        Selection.Add(item);
                                    }
                                    _RefreshSelection = true;
                                }
                            }
                            return nodesToPaste;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return null;
        }

        void DeleteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Selection.SelectedObjects.Count > 0;
            e.Handled = true;
        }
        void DeleteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            List<AnimNodeViewModel> nodesToDelete = new List<AnimNodeViewModel>();
            List<AnimationConnectionViewModel> connectionsToDelete = new List<AnimationConnectionViewModel>();

            for (int i = 0; i < Selection.SelectedObjects.Count; i++)
            {
                AnimNodeViewModel vm = Selection.SelectedObjects[i] as AnimNodeViewModel;
                if (vm != null && vm.NodeType != AnimNodeType.Root)
                {
                    nodesToDelete.Add(vm);

                    var qurey = this.AnimationTree.Connections.Where(c => c.Source == vm);
                    foreach (var connection in qurey)
                    {
                        connectionsToDelete.Add(connection);
                    }
                }
            }

            if (nodesToDelete.Count > 0)
            {
                this.AnimationTree.Remove(nodesToDelete.ToArray(), connectionsToDelete.ToArray());
            }

            e.Handled = true;
        }
        #endregion


        #region PreviewMouseDown
        
        private void AnimNode_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                // update selection
                ContentPresenter presenter = sender as ContentPresenter;
                if (presenter != null)
                {
                    AnimNodeViewModel vm = presenter.Content as AnimNodeViewModel;
                    if (vm != null)
                    {
                        if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                        {
                            if (vm.IsSelected)
                            {
                                Selection.Remove(vm);
                            }
                            else
                            {
                                Selection.Add(vm);
                            }
                        }
                        else if (!vm.IsSelected)
                        {
                            Selection.Select(vm);
                        }
                        Focus();
                    }
                }
            }
        }

        #endregion

        #region KeyDown
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
            {
                if (e.Key == Key.D && e.IsDown && !e.IsRepeat)
                {
                    DuplicateSelection();
                }
            }
            base.OnKeyDown(e);
        }
        #endregion

        #region AddConnection
        public void AddConnection(AnimConnector sourceConnector, AnimConnector sinkConnector)
        {
            if (!AllowConnect(sourceConnector, sinkConnector)) return;

            AnimationConnection newConnectionModel;
            if (sourceConnector.ViewModel.Type == ConnectorType.Output)
                newConnectionModel = new AnimationConnection(sourceConnector.ViewModel.AnimNode.Model, sinkConnector.ViewModel.AnimNode.Model, sinkConnector.ViewModel.Index);
            else
                newConnectionModel = new AnimationConnection(sinkConnector.ViewModel.AnimNode.Model, sourceConnector.ViewModel.AnimNode.Model, sourceConnector.ViewModel.Index);

            AnimationConnectionViewModel newConnectionVM = new AnimationConnectionViewModel(this.AnimationTree, newConnectionModel);

            this.AnimationTree.Add(newConnectionVM);
        }

        public bool AllowConnect(AnimConnector sourceConnector, AnimConnector sinkConnector)
        {
            return (sourceConnector.ViewModel != sinkConnector.ViewModel) &&
                    (sourceConnector.ViewModel.Type == ConnectorType.Output ^ sinkConnector.ViewModel.Type == ConnectorType.Output);
        }
        #endregion

        #region Delete Connection
        private void Mnu_BreakConnection_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                if (menuItem.DataContext is ConnectorViewModel) // this is input connector
                {
                    ConnectorViewModel connector = menuItem.DataContext as ConnectorViewModel;
                    if (connector != null)
                    {
                        this.AnimationTree.RemoveConnectionsTo(connector.AnimNode, connector.Index);
                    }
                }
                else // this is output connector
                {
                    AnimNodeViewModel vm = menuItem.DataContext as AnimNodeViewModel;
                    if (vm != null)
                    {
                        this.AnimationTree.RemoveConnectionsFrom(vm);
                    }
                }
            }
        }
        #endregion


        #region Duplicate

        public void DuplicateSelection()
        {
            CopySelectionToClipboard(true);
            PasteFromClipboard();
        }

        #endregion
    }
}
