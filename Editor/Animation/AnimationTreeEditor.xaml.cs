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
using Skill.Editor.Diagram;

namespace Skill.Editor.Animation
{
    /// <summary>
    /// Interaction logic for AnimationTreeEditor.xaml
    /// </summary>
    public partial class AnimationTreeEditor : TabDocument
    {
        #region Variables
        private Point _CreatePosition;
        private string _FileName; // full address of BehaviorTree filename
        private AnimationTree _AnimationTree; // view model
        bool _IsChanged;
        #endregion

        #region Constructor
        public AnimationTreeEditor()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Create a BehaviorTreeEditor
        /// </summary>
        /// <param name="filename">BehaviorTree filename</param>
        public AnimationTreeEditor(string filename)
            : this()
        {
            this._FileName = filename;
            _AnimationTree = AnimationTree.Load(filename);
            Load();
            ChangeTitle();
            History.Change += new EventHandler(History_Change);
            History.UndoChange += new EventHandler(History_UndoChange);
            History.RedoChange += new EventHandler(History_RedoChange);
            _Canvas.AddConnection += new EventHandler(_Canvas_AddConnection);
        }

        void _Canvas_AddConnection(object sender, EventArgs e)
        {
            SetChanged(true);
        }

        private void Load()
        {
            _Canvas.Children.Clear();
            foreach (var node in _AnimationTree)
            {
                CreateNode(node);
            }
            _ConnectionsToLoad = _AnimationTree.Connections;

            _Canvas.Scale = _AnimationTree.Scale;
            _Canvas.ScrollViewer.ScrollToHorizontalOffset(_AnimationTree.HorizontalOffset);
            _Canvas.ScrollViewer.ScrollToVerticalOffset(_AnimationTree.VerticalOffset);
        }

        private AnimationConnection[] _ConnectionsToLoad = null;
        private void LoadConnections()
        {
            if (_ConnectionsToLoad == null) return;
            foreach (var c in _ConnectionsToLoad)
            {
                AnimNodeViewModel sourceNode = _Canvas.FindById(c.SourceId);
                AnimNodeViewModel sinkNode = _Canvas.FindById(c.SinkId);
                if (sinkNode != null && sourceNode != null)
                {
                    Connector source = sourceNode.Out;
                    Connector sink = sinkNode.GetConnectorByIndex(c.SinkConnectorIndex);
                    if (source != null && sink != null)
                    {
                        Connection connection = new Connection(source, sink);
                        Canvas.SetZIndex(connection, _Canvas.Children.Count);
                        _Canvas.Children.Add(connection);
                    }
                }
            }
            _ConnectionsToLoad = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            LoadConnections();
            base.OnMouseMove(e);
        }

        private AnimNodeViewModel CreateNode(AnimNode node)
        {
            AnimNodeViewModel vm = null;
            switch (node.NodeType)
            {
                case AnimNodeType.Sequence:
                    vm = new AnimNodeSequenceViewModel((AnimNodeSequence)node);
                    break;
                case AnimNodeType.Override:
                    vm = new AnimNodeOverrideViewModel((AnimNodeOverride)node);
                    break;
                case AnimNodeType.BlendBySpeed:
                    vm = new AnimNodeBlendBySpeedViewModel((AnimNodeBlendBySpeed)node);
                    break;
                case AnimNodeType.BlendByPosture:
                    vm = new AnimNodeBlendByPostureViewModel((AnimNodeBlendByPosture)node);
                    break;
                case AnimNodeType.BlendByIdle:
                    vm = new AnimNodeBlendByIdleViewModel((AnimNodeBlendByIdle)node);
                    break;
                case AnimNodeType.Blend4Directional:
                    vm = new AnimNodeBlend4DirectionalViewModel((AnimNodeBlend4Directional)node);
                    break;
                case AnimNodeType.AimOffset:
                    vm = new AnimNodeAimOffsetViewModel((AnimNodeAimOffset)node);
                    break;
                case AnimNodeType.AdditiveBlending:
                    vm = new AnimNodeAdditiveBlendingViewModel((AnimNodeAdditiveBlending)node);
                    break;
                case AnimNodeType.Random:
                    vm = new AnimNodeRandomViewModel((AnimNodeRandom)node);
                    break;
                case AnimNodeType.BlendByIndex:
                    vm = new AnimNodeBlendByIndexViewModel((AnimNodeBlendByIndex)node);
                    break;
                case AnimNodeType.Root:
                    vm = new AnimationTreeRootViewModel((AnimationTreeRoot)node);
                    break;
                default:
                    break;
            }
            if (vm != null)
            {
                Canvas.SetTop(vm, node.Y);
                Canvas.SetLeft(vm, node.X);
                _Canvas.Children.Add(vm);
            }
            return vm;
        }
        #endregion

        #region History events

        // hook events of History
        void History_RedoChange(object sender, EventArgs e)
        {
        }

        void History_UndoChange(object sender, EventArgs e)
        {
        }

        // when any change occurs in history, update title
        void History_Change(object sender, EventArgs e)
        {
            SetChanged(History.ChangeCount != 0);
        }
        #endregion

        #region Title
        private void ChangeTitle()
        {
            string newTitle = System.IO.Path.GetFileNameWithoutExtension(_FileName) + (IsChanged ? "*" : "");
            if (this.Title != newTitle) this.Title = newTitle;
        }
        #endregion

        #region Create Node

        private int _IdGenerator = int.MaxValue - 1;
        private void Mnu_Create_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            if (mnu != null && mnu.Tag is AnimNodeType)
            {
                AnimNodeType nodeType = (AnimNodeType)mnu.Tag;
                CreateNode(nodeType);
                SetChanged(true);
            }
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _CreatePosition = e.GetPosition(_Canvas);
        }


        private AnimNodeViewModel CreateNode(AnimNodeType nodeType)
        {
            AnimNodeViewModel vm = null;
            switch (nodeType)
            {
                case AnimNodeType.Sequence:
                    vm = new AnimNodeSequenceViewModel();
                    break;
                case AnimNodeType.Override:
                    vm = new AnimNodeOverrideViewModel();
                    break;
                case AnimNodeType.BlendBySpeed:
                    vm = new AnimNodeBlendBySpeedViewModel();
                    break;
                case AnimNodeType.BlendByPosture:
                    vm = new AnimNodeBlendByPostureViewModel();
                    break;
                case AnimNodeType.BlendByIdle:
                    vm = new AnimNodeBlendByIdleViewModel();
                    break;
                case AnimNodeType.Blend4Directional:
                    vm = new AnimNodeBlend4DirectionalViewModel();
                    break;
                case AnimNodeType.AimOffset:
                    vm = new AnimNodeAimOffsetViewModel();
                    break;
                case AnimNodeType.AdditiveBlending:
                    vm = new AnimNodeAdditiveBlendingViewModel();
                    break;
                case AnimNodeType.Random:
                    vm = new AnimNodeRandomViewModel();
                    break;
                case AnimNodeType.BlendByIndex:
                    vm = new AnimNodeBlendByIndexViewModel();
                    break;
                case AnimNodeType.Root:
                    vm = new AnimationTreeRootViewModel();
                    break;
                default:
                    break;
            }
            if (vm != null)
            {
                vm.Model.Id = _IdGenerator--;
                Canvas.SetTop(vm, _CreatePosition.Y);
                Canvas.SetLeft(vm, _CreatePosition.X);
                _Canvas.Children.Add(vm);
                vm.Properties.Name = _Canvas.GetValidHeader(vm.Properties.Name);
            }
            return vm;
        }
        #endregion


        #region Unload
        /// <summary> Unload content </summary>
        public override void UnLoad() { _Canvas.Selection.Clear(); }
        #endregion

        #region FileName
        /// <summary> content filename </summary>
        public override string FileName { get { return _FileName; } }
        #endregion

        #region Save
        private void GenerateIds()
        {
            int id = 0;
            // set all nodes id to -1
            foreach (var item in _Canvas.Children)
            {
                if (item is AnimNodeViewModel)
                {
                    AnimNodeViewModel vm = (AnimNodeViewModel)item;
                    vm.Model.Id = id++;
                }
            }
        }
        /// <summary> Save content </summary>
        public override void Save()
        {
            if (_AnimationTree != null)
            {
                _AnimationTree.Clear();
                GenerateIds();
                List<AnimationConnection> connections = new List<AnimationConnection>();
                foreach (var item in _Canvas.Children)
                {
                    if (item is AnimNodeViewModel)
                    {
                        AnimNodeViewModel vm = (AnimNodeViewModel)item;
                        vm.CommiteChangesToModel();
                        _AnimationTree.Add(vm.Model);
                    }
                    else if (item is Skill.Editor.Diagram.Connection)
                    {
                        Skill.Editor.Diagram.Connection connection = (Skill.Editor.Diagram.Connection)item;
                        AnimNodeViewModel source = (AnimNodeViewModel)connection.Source.ParentDragableContent;
                        AnimNodeViewModel sink = (AnimNodeViewModel)connection.Sink.ParentDragableContent;
                        connections.Add(new AnimationConnection(source.Model.Id, sink.Model.Id, connection.Sink.Index));
                    }
                }
                _AnimationTree.Connections = connections.ToArray();

                _AnimationTree.Scale = _Canvas.Scale;
                _AnimationTree.HorizontalOffset = _Canvas.ScrollViewer.HorizontalOffset;
                _AnimationTree.VerticalOffset = _Canvas.ScrollViewer.VerticalOffset;

                _AnimationTree.Save(_FileName);
                SetChanged(false);
                History.ResetChangeCount();
            }
        }
        #endregion

        #region IsChanged

        public void SetChanged(bool changed)
        {
            bool titleChanged = changed != _IsChanged;
            _IsChanged = changed;
            if (titleChanged)
                ChangeTitle();
        }

        /// <summary> Whether is changed or not </summary>
        public override bool IsChanged { get { return _IsChanged; } }
        #endregion

        #region ChangeName
        /// <summary> Change name of AnimationTree </summary>
        /// <param name="newName">new name of AnimationTree filename</param>
        public override void OnChangeName(string newName)
        {
            string dir = System.IO.Path.GetDirectoryName(_FileName);
            string ext = System.IO.Path.GetExtension(_FileName);
            _FileName = System.IO.Path.Combine(dir, newName + ext);
            ChangeTitle();
        }
        #endregion


        #region Copy

        private static Rect GetRect(FrameworkElement e)
        {
            double x = Canvas.GetLeft(e);
            double y = Canvas.GetTop(e);

            double w = e.DesiredSize.Width;
            double h = e.DesiredSize.Height;

            if (double.IsNaN(w)) w = e.MinWidth;
            if (double.IsNaN(h)) h = e.MinHeight;

            return new Rect(x, y, w, h);
        }

        public static Point Min(Point p1, FrameworkElement e)
        {
            return new Point(Math.Min(p1.X, Canvas.GetLeft(e)), Math.Min(p1.Y, Canvas.GetTop(e)));
        }
        public static Point Max(Point p1, FrameworkElement e)
        {
            double w = e.DesiredSize.Width;
            double h = e.DesiredSize.Height;

            if (double.IsNaN(w)) w = e.MinWidth;
            if (double.IsNaN(h)) h = e.MinHeight;

            return new Point(Math.Max(p1.X, Canvas.GetLeft(e) + w), Math.Max(p1.Y, Canvas.GetTop(e) + h));
        }

        private CopyObject CopyCurrentSelection()
        {
            CopyObject copy = new CopyObject(CopyType.Animation);

            bool first = true;

            IEnumerable<AnimNodeViewModel> selectedItems =
                _Canvas.Selection.CurrentSelection.OfType<AnimNodeViewModel>();

            IEnumerable<Connection> selectedConnections =
                _Canvas.Selection.CurrentSelection.OfType<Connection>();

            foreach (var item in selectedItems)
            {
                if (item.Model.NodeType == AnimNodeType.Root) continue;
                Rect rect = GetRect(item);
                if (first)
                {
                    copy.CopyArea = rect;
                    first = false;
                }
                else
                    copy.CopyArea = Rect.Union(rect, copy.CopyArea);

                copy.Items.Add(item.Model.Clone());
            }

            foreach (var item in selectedConnections)
            {
                Rect rect = item.Area;
                if (first)
                {
                    copy.CopyArea = rect;
                    first = false;
                }
                else
                    copy.CopyArea = Rect.Union(rect, copy.CopyArea);

                if (selectedItems.Contains(item.Source.ParentDragableContent) && selectedItems.Contains(item.Sink.ParentDragableContent))
                    copy.Items.Add(new AnimationConnection(((AnimNodeViewModel)item.Source.ParentDragableContent).Model.Id, ((AnimNodeViewModel)item.Sink.ParentDragableContent).Model.Id, item.Sink.Index));
            }

            return copy;
        }

        /// <summary> Do copy </summary>
        public override void Copy()
        {
            CopyObject copy = CopyCurrentSelection();
            copy.IsCut = false;
            CopyObject.Instance = copy;
        }

        /// <summary> is copy possible now</summary>
        public override bool CanCopy { get { return _Canvas.Selection.CurrentSelection.Count > 0; } }
        #endregion

        #region Cut
        /// <summary> Do cut </summary>
        public override void Cut()
        {
            CopyObject copy = CopyCurrentSelection();
            copy.IsCut = true;
            DeleteSelection(false);
            CopyObject.Instance = copy;

        }

        /// <summary> is cut possible now</summary>
        public override bool CanCut { get { return _Canvas.Selection.CurrentSelection.Count > 0; } }
        #endregion


        #region Paste

        private static Point GetCanvasLocation(FrameworkElement e)
        {
            return new Point(Canvas.GetLeft(e), Canvas.GetTop(e));
        }

        private class CopyPair
        {
            public AnimNode Original;
            public AnimNodeViewModel Copy;
        }

        /// <summary> Do paste </summary>
        public override void Paste()
        {
            CopyObject copy = CopyObject.Instance;
            if (copy == null || copy.Type != CopyType.Animation) return;

            Point p = _CreatePosition;
            if (_Canvas.ContextMenu.IsVisible == false)
                p = Mouse.GetPosition(_Canvas);
            if (p.X < 0 || p.Y < 0 || p.X > _Canvas.Width || p.Y > _Canvas.Height)
            {
                MessageBox.Show("Please hold mouse in valid position inside diagram");
                return;
            }

            Point center = copy.CopyArea.Location;
            center.X += copy.CopyArea.Width / 2;
            center.Y += copy.CopyArea.Height / 2;

            List<CopyPair> items = new List<CopyPair>();
            foreach (var item in copy.Items.OfType<AnimNode>())
            {
                CopyPair pair = new CopyPair();
                pair.Original = item;

                Vector dis = new Vector(item.X - center.X, item.Y - center.Y);
                _CreatePosition = p + dis;
                pair.Copy = CreateNode(item.NodeType);
                int id = pair.Copy.Model.Id; // because we need new id
                pair.Copy.Model.CopyFrom(pair.Original);
                pair.Copy.Model.Id = id;// restore id after copy
                pair.Copy.Model.Name = _Canvas.GetValidHeader(pair.Copy.Model.Name);
                items.Add(pair);
            }

            List<AnimationConnection> connections = new List<AnimationConnection>();
            foreach (var item in copy.Items.OfType<AnimationConnection>())
            {
                CopyPair source = FindInPairsWithOrigin(items, item.SourceId);
                CopyPair sink = FindInPairsWithOrigin(items, item.SinkId);
                if (source != null && sink != null)
                {
                    connections.Add(new AnimationConnection(source.Copy.Model.Id, sink.Copy.Model.Id, item.SinkConnectorIndex));
                }
            }

            if (connections.Count > 0)
                _ConnectionsToLoad = connections.ToArray();

            _Canvas.Selection.Clear();            
            items.Clear();
            SetChanged(true);
        }


        private CopyPair FindInPairsWithOrigin(List<CopyPair> items, int id)
        {
            foreach (var item in items)
            {
                if (item.Original.Id == id) return item;
            }
            return null;
        }

        /// <summary> is paste possible now </summary>
        public override bool CanPaste
        {
            get
            {

                CopyObject copy = CopyObject.Instance;
                if (copy != null)
                {
                    return copy.Type == CopyType.Animation && copy.Items.Count > 0;
                }
                return false;
            }
        }

        #endregion



        #region Delete

        private void DeleteSelection(bool history)
        {
            foreach (IDiagramObject item in _Canvas.Selection.CurrentSelection)
            {
                if (item is AnimNodeViewModel)
                {
                    if (((AnimNodeViewModel)item).Model.NodeType == AnimNodeType.Root)
                        continue;
                }
                item.Disconnect();
            }
            _Canvas.Selection.Clear();
            SetChanged(true);
        }

        /// <summary> Do delete </summary>
        public override void Delete()
        {
            DeleteSelection(true);
        }

        /// <summary> is delete possible now</summary>
        public override bool CanDelete { get { return _Canvas.Selection.CurrentSelection.Count > 0; } }
        #endregion


        #region Commands
        static RoutedCommand _BringForward = new RoutedCommand();
        public static RoutedCommand BringForward
        {
            get
            {
                return _BringForward;
            }
        }


        static RoutedCommand _BringToFront = new RoutedCommand();
        public static RoutedCommand BringToFront { get { return _BringToFront; } }

        static RoutedCommand _SendBackward = new RoutedCommand();
        public static RoutedCommand SendBackward { get { return _SendBackward; } }

        static RoutedCommand _SendToBack = new RoutedCommand();
        public static RoutedCommand SendToBack { get { return _SendToBack; } }

        


        #region BringForward Command

        private void BringForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> ordered = (from item in _Canvas.Selection.CurrentSelection
                                       orderby Canvas.GetZIndex(item as UIElement) descending
                                       select item as UIElement).ToList();

            int count = _Canvas.Children.Count;

            for (int i = 0; i < ordered.Count; i++)
            {
                int currentIndex = Canvas.GetZIndex(ordered[i]);
                int newIndex = Math.Min(count - 1 - i, currentIndex + 1);
                if (currentIndex != newIndex)
                {
                    Canvas.SetZIndex(ordered[i], newIndex);
                    IEnumerable<UIElement> it = _Canvas.Children.OfType<UIElement>().Where(item => Canvas.GetZIndex(item) == newIndex);

                    foreach (UIElement elm in it)
                    {
                        if (elm != ordered[i])
                        {
                            Canvas.SetZIndex(elm, currentIndex);
                            break;
                        }
                    }
                }
            }
            e.Handled = true;
        }

        private void Order_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
            e.CanExecute = true;
        }

        #endregion

        #region BringToFront Command

        private void BringToFront_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> selectionSorted = (from item in _Canvas.Selection.CurrentSelection
                                               orderby Canvas.GetZIndex(item as UIElement) ascending
                                               select item as UIElement).ToList();

            List<UIElement> childrenSorted = (from UIElement item in _Canvas.Children
                                              orderby Canvas.GetZIndex(item as UIElement) ascending
                                              select item as UIElement).ToList();

            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (selectionSorted.Contains(item))
                {
                    int idx = Canvas.GetZIndex(item);
                    Canvas.SetZIndex(item, childrenSorted.Count - selectionSorted.Count + j++);
                }
                else
                {
                    Canvas.SetZIndex(item, i++);
                }
            }
            e.Handled = true;
        }

        #endregion

        #region SendBackward Command

        private void SendBackward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> ordered = (from item in _Canvas.Selection.CurrentSelection
                                       orderby Canvas.GetZIndex(item as UIElement) ascending
                                       select item as UIElement).ToList();

            int count = _Canvas.Children.Count;

            for (int i = 0; i < ordered.Count; i++)
            {
                int currentIndex = Canvas.GetZIndex(ordered[i]);
                int newIndex = Math.Max(i, currentIndex - 1);
                if (currentIndex != newIndex)
                {
                    Canvas.SetZIndex(ordered[i], newIndex);
                    IEnumerable<UIElement> it = _Canvas.Children.OfType<UIElement>().Where(item => Canvas.GetZIndex(item) == newIndex);

                    foreach (UIElement elm in it)
                    {
                        if (elm != ordered[i])
                        {
                            Canvas.SetZIndex(elm, currentIndex);
                            break;
                        }
                    }
                }
            }
            e.Handled = true;
        }

        #endregion

        #region SendToBack Command

        private void SendToBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> selectionSorted = (from item in _Canvas.Selection.CurrentSelection
                                               orderby Canvas.GetZIndex(item as UIElement) ascending
                                               select item as UIElement).ToList();

            List<UIElement> childrenSorted = (from UIElement item in _Canvas.Children
                                              orderby Canvas.GetZIndex(item as UIElement) ascending
                                              select item as UIElement).ToList();
            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (selectionSorted.Contains(item))
                {
                    int idx = Canvas.GetZIndex(item);
                    Canvas.SetZIndex(item, j++);

                }
                else
                {
                    Canvas.SetZIndex(item, selectionSorted.Count + i++);
                }
            }
            e.Handled = true;
        }

        #endregion

        #endregion
    }
}
