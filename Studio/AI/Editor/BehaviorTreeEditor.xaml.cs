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
using Skill.Studio.AI;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Skill.DataModels.AI;
using System.Xml.Linq;
using Skill.DataModels;
using System.Windows.Threading;
using Skill.Studio.Controls;

namespace Skill.Studio.AI.Editor
{
    /// <summary>
    /// Interaction logic for BehaviorTreeEditor.xaml
    /// </summary>
    public partial class BehaviorTreeEditor : TabDocument
    {
        #region SelectedItem

        /// <summary>
        /// Get selected Behavior
        /// </summary>
        /// <returns>selected Behavior</returns>
        private BehaviorViewModel GetSelectedItem()
        {
            return _BTTree.SelectedItem as BehaviorViewModel;
        }
        #endregion

        #region Menu Availability

        private bool _IsNewAv;
        public bool IsNewAv
        {
            get { return _IsNewAv; }
            set
            {
                if (value != _IsNewAv)
                {
                    _IsNewAv = value;
                    this.RaisePropertyChanged("IsNewAv");
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
                    this.RaisePropertyChanged("IsMoveUpAv");
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
                    this.RaisePropertyChanged("IsMoveDownAv");
                }
            }
        }
        private bool _IsDeleteAv;
        public bool IsDeleteAv
        {
            get { return _IsDeleteAv; }
            set
            {
                if (value != _IsDeleteAv)
                {
                    _IsDeleteAv = value;
                    this.RaisePropertyChanged("IsDeleteAv");
                }
            }
        }
        private bool _IsCopyAv;
        public bool IsCopyAv
        {
            get { return _IsCopyAv; }
            set
            {
                if (value != _IsCopyAv)
                {
                    _IsCopyAv = value;
                    this.RaisePropertyChanged("IsCopyAv");
                }
            }
        }

        void CheckContextMnuAvailability()
        {
            BehaviorViewModel selected = GetSelectedItem();
            if (selected != null && !BehaviorTree.IsDebuging)
            {
                IsDeleteAv = IsCopyAv = selected != BehaviorTree.Root;
                if (selected.Model.BehaviorType == BehaviorType.Composite)
                {
                    IsNewAv = true;
                }
                else if (selected.Model.BehaviorType == BehaviorType.Decorator)
                {
                    IsNewAv = selected.Count == 0;
                }
                else
                {
                    IsNewAv = false;
                }

                IsMoveUpAv = selected.CanMoveUp;
                IsMoveDownAv = selected.CanMoveDown;

            }
            else
            {
                IsNewAv = false;
                IsMoveDownAv = false;
                IsMoveUpAv = false;
                IsDeleteAv = false;
                IsCopyAv = false;
            }
        }
        #endregion

        #region Properties
        /// <summary> BehaviorTree ViewModel </summary>
        public BehaviorTreeViewModel BehaviorTree { get; private set; }

        public ScrollViewer ScrollViewer { get { return _DesignerScrollViewer; } }

        public ObservableCollection<GifImage> Animations { get; private set; }

        private bool _ShowAnimations = true;
        public bool ShowAnimations
        {
            get { return _ShowAnimations; }
            set
            {
                if (_ShowAnimations != value)
                {
                    _ShowAnimations = value;
                    RaisePropertyChanged("ShowAnimations");
                }
            }
        }

        private double _AnimationSize = 128;
        public double AnimationSize
        {
            get { return _AnimationSize; }
            set
            {
                if (_AnimationSize != value)
                {
                    _AnimationSize = value;
                    RaisePropertyChanged("AnimationSize");
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Create a BehaviorTreeEditor in design mode
        /// </summary>
        public BehaviorTreeEditor()
            : this(null)
        {
        }

        public BehaviorTreeEditor(BehaviorTreeNodeViewModel viewModel)
            : base(viewModel)
        {
            this.Animations = new ObservableCollection<GifImage>();
            InitializeComponent();
            this.DataContext = this;
            if (viewModel != null)
            {
                Skill.DataModels.AI.BehaviorTree bt = viewModel.LoadData() as Skill.DataModels.AI.BehaviorTree;
                if (bt != null)
                {
                    Data = BehaviorTree = new BehaviorTreeViewModel(bt) { Editor = this };
                    RaisePropertyChanged("BehaviorTree");
                }
            }

            History.UndoChange += new UnDoRedoChangeEventHandler(History_UndoChange);
            History.RedoChange += new UnDoRedoChangeEventHandler(History_RedoChange);

            InitialDebug();
        }
        #endregion

        #region History events
        // hook events of History
        void History_RedoChange(UnDoRedo sender, UnDoRedoChangeEventArgs e)
        {
            if (e.Command is AddBehaviorUnDoRedo || e.Command is MoveUpBehaviorUnDoRedo)
                UpdatePositions();
        }

        void History_UndoChange(UnDoRedo sender, UnDoRedoChangeEventArgs e)
        {
            if (e.Command is AddBehaviorUnDoRedo || e.Command is MoveUpBehaviorUnDoRedo)
                UpdatePositions();
        }

        // when any change occurs in history, update title
        void History_Change(object sender, EventArgs e)
        {
            SetChanged(History.ChangeCount != 0);
        }
        #endregion

        #region Selected item changed

        private void Behavior_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                BehaviorViewModel vm = border.DataContext as BehaviorViewModel;
                if (vm != null)
                {
                    vm.IsSelected = true;
                }
            }
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
            }
        }

        private void _BTTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CheckContextMnuAvailability();
            ApplicationCommands.Properties.Execute(_BTTree.SelectedItem, null);

            if (!BehaviorTree.IsDebuging)
            {
                BehaviorViewModel vm = GetSelectedItem();
                if (vm != null)
                {
                    if (vm.Model.BehaviorType == BehaviorType.Action)
                    {
                        ActionViewModel ac = (ActionViewModel)vm;
                        if (!Animations.Contains(ac.GifAnimation))
                        {
                            foreach (var anim in Animations) anim.Stop();
                            Animations.Clear();
                            Animations.Add(ac.GifAnimation);
                            ac.GifAnimation.Play();
                        }
                    }
                    else
                    {
                        Animations.Clear();
                    }
                }
            }
        }
        private void TreeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
            }
        }
        #endregion

        #region UpdatePositions

        protected override void OnContentLoaded()
        {
            UpdatePositions();
            base.OnContentLoaded();
        }


        public event EventHandler UpdateTreeNodes;

        private void UpdatePositions()
        {
            this.BehaviorTree.Root.UpdatePosition();
            OnUpdateTreeNodes();
        }

        private void OnUpdateTreeNodes()
        {
            if (UpdateTreeNodes != null)
                UpdateTreeNodes(this, EventArgs.Empty);
        }
        #endregion

        #region Insert
        private void Mnu_Insert_Click(object sender, RoutedEventArgs e)
        {
            if (IsNewAv)
            {
                MenuItem menu = sender as MenuItem;
                if (menu != null)
                {
                    BehaviorViewModel insertItem = menu.Tag as BehaviorViewModel;
                    if (insertItem != null)
                    {
                        var selected = GetSelectedItem();
                        if (selected != null)
                        {
                            try
                            {
                                string message;
                                if (selected.CanAddBehavior(insertItem, out message))
                                {
                                    var newVM = selected.AddBehavior(insertItem, null, true);
                                    if (newVM != null)
                                        newVM.IsSelected = true;
                                    UpdatePositions();
                                }
                                else
                                    MainWindow.Instance.ShowError(message);
                            }
                            catch (Exception ex)
                            {
                                MainWindow.Instance.ShowError(ex.Message);
                            }
                        }
                    }
                }
            }
        }

        private TextBlock FindTexkBlock(MenuItem menu)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(menu);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(menu, i);
                if (child is TextBlock) return child as TextBlock;
                TextBlock textBlock = FindTexkBlock(child);
                if (textBlock != null) return textBlock;
            }
            return null;
        }

        private TextBlock FindTexkBlock(DependencyObject obj)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is TextBlock) return child as TextBlock;
                TextBlock textBlock = FindTexkBlock(child);
                if (textBlock != null) return textBlock;
            }
            return null;
        }
        #endregion

        #region New
        private void AddCondition()
        {
            if (IsNewAv)
            {
                var selected = GetSelectedItem();
                if (selected != null)
                {
                    var newVM = selected.AddCondition();
                    if (newVM != null)
                    {
                        newVM.IsSelected = true;
                    }
                    UpdatePositions();
                }
            }
        }

        private void AddAction()
        {
            if (IsNewAv)
            {
                var selected = GetSelectedItem();
                if (selected != null)
                {
                    var newVM = selected.AddAction();
                    if (newVM != null)
                    {

                        newVM.IsSelected = true;
                    }
                    UpdatePositions();
                }
            }
        }

        private void AddComposite(CompositeType compositeType)
        {
            if (IsNewAv)
            {
                var selected = GetSelectedItem();
                if (selected != null)
                {
                    var newVM = selected.AddComposite(compositeType);
                    if (newVM != null)
                    {
                        newVM.IsSelected = true;
                    }
                    UpdatePositions();
                }
            }
        }

        private void AddDecorator(DecoratorType decoratorType)
        {
            if (IsNewAv)
            {
                var selected = GetSelectedItem();
                if (selected != null)
                {
                    var newVM = selected.AddDecorator(decoratorType);
                    if (newVM != null)
                    {
                        newVM.IsSelected = true;
                    }
                    UpdatePositions();
                }
            }
        }

        private void Mnu_NewAction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                AddAction();
            }
            else System.Windows.MessageBox.Show("Menu Item not clicked");
        }

        private void Mnu_NewCondition_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                AddCondition();
            }
            else System.Windows.MessageBox.Show("Menu Item not clicked");
        }

        private void Mnu_NewDecorator_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                MenuItem mnu = (MenuItem)sender;
                AddDecorator((DecoratorType)mnu.Tag);
            }
            else System.Windows.MessageBox.Show("Menu Item not clicked");
        }

        private void Mnu_NewComposite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                MenuItem mnu = (MenuItem)sender;
                AddComposite((CompositeType)mnu.Tag);
            }
            else System.Windows.MessageBox.Show("Menu Item not clicked");
        }
        #endregion

        #region Move
        private void Mnu_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (IsMoveUpAv)
            {
                var selected = GetSelectedItem();
                if (selected != null && selected.CanMoveUp)
                {
                    if (selected.Parent != null)
                    {
                        ((BehaviorViewModel)selected.Parent).MoveUp(selected);
                        UpdatePositions();
                    }
                }

            }
        }

        private void Mnu_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (IsMoveDownAv)
            {
                var selected = GetSelectedItem();
                if (selected != null && selected.CanMoveDown)
                {
                    if (selected.Parent != null)
                    {
                        ((BehaviorViewModel)selected.Parent).MoveDown(selected);
                        UpdatePositions();
                    }
                }
            }
        }
        #endregion

        #region Delete
        private void DeleteSelected()
        {
            var selected = GetSelectedItem();
            if (selected != null)
            {
                if (selected.Parent != null)
                {
                    if (selected.Parent is BehaviorViewModel)
                    {
                        ((BehaviorViewModel)selected.Parent).RemoveBehavior(selected);
                        UpdatePositions();
                    }
                }
                CheckContextMnuAvailability();
            }
        }
        #endregion

        #region UnLoad
        public override void UnLoad()
        {
            BehaviorTree = null;
            base.UnLoad();
        }
        #endregion

        #region Edit

        private void CopyBehaviorToClipboard(BehaviorViewModel bvm)
        {
            XDocument document = new XDocument();
            XElement bElement = CreateHierarchyBehavior(bvm);
            document.Add(bElement);
            Clipboard.Clear();
            Clipboard.SetData(Skill.DataModels.SkillDataFormats.Behavior, document.ToString());
        }

        private XElement CreateHierarchyBehavior(BehaviorViewModel bvm)
        {
            XElement bcElement = new XElement("BehaviorContainer");

            bcElement.Add(bvm.Model.ToXElement());
            if (bvm.Parent != null)
            {
                XElement parameters = ((BehaviorViewModel)bvm.Parent).GetParameters(bvm).ToXElement();
                bcElement.Add(parameters);
            }

            if (bvm.Count > 0)
            {
                XElement childrenElement = new XElement("Children");

                foreach (BehaviorViewModel childVM in bvm)
                {
                    childrenElement.Add(CreateHierarchyBehavior(childVM));
                }

                bcElement.Add(childrenElement);
            }

            return bcElement;
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
            e.CanExecute = IsCopyAv;
            e.Handled = true;
        }
        void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            var selected = GetSelectedItem();
            if (selected != null)
                CopyBehaviorToClipboard(selected);
            e.Handled = true;
        }

        void PasteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var selected = GetSelectedItem();
            e.CanExecute = Clipboard.ContainsData(Skill.DataModels.SkillDataFormats.Behavior) && selected != null && IsNewAv;
            e.Handled = true;
        }
        void PasteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            PasteFromClipboard();
            UpdatePositions();
            e.Handled = true;
        }

        private void PasteFromClipboard()
        {
            if (Clipboard.ContainsData(Skill.DataModels.SkillDataFormats.Behavior))
            {
                var selected = GetSelectedItem();
                string text = Clipboard.GetData(Skill.DataModels.SkillDataFormats.Behavior) as string;
                if (text != null && selected != null && IsNewAv)
                {
                    try
                    {
                        XDocument document = XDocument.Parse(text);
                        History.IsEnable = false;
                        ParameterCollection parameters = null;
                        BehaviorViewModel bvm = GetHierarchyBehavior(selected, document.Elements().First(), out parameters);

                        History.IsEnable = true;
                        if (bvm != null)
                        {
                            selected.AddBehavior(bvm, parameters, false);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        History.IsEnable = true;
                    }
                }
            }
        }


        private BehaviorViewModel GetHierarchyBehavior(BehaviorViewModel parent, XElement bcElement, out ParameterCollection parameters)
        {
            BehaviorViewModel bvm = null;
            parameters = null;

            if (bcElement.Name == "BehaviorContainer")
            {
                XElement bElement = bcElement.FindChildByName("Behavior");
                if (bElement != null)
                {
                    Behavior b = Skill.DataModels.AI.BehaviorTree.CreateBehaviorFrom(bElement);
                    if (b != null)
                    {
                        b.Load(bElement);
                        bvm = parent.CreateViewModel(b);
                    }
                    else
                        return null;
                }

                XElement parametersElement = bcElement.FindChildByName("Parameters");
                if (parametersElement != null)
                {
                    parameters = new ParameterCollection();
                    parameters.Load(parametersElement);
                }

                XElement childrenElement = bcElement.FindChildByName("Children");
                if (childrenElement != null)
                {
                    foreach (var childElement in childrenElement.Elements())
                    {
                        ParameterCollection childps;
                        BehaviorViewModel childVM = GetHierarchyBehavior(bvm, childElement, out childps);
                        if (childVM != null)
                        {
                            BehaviorTree.CreateNewName(childVM);
                            bvm.AddBehavior(childVM, childps, false);
                        }
                    }
                }
            }

            return bvm;
        }

        void DeleteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsDeleteAv;
            e.Handled = true;
        }
        void DeleteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (IsDeleteAv)
                DeleteSelected();
            e.Handled = true;
        }
        #endregion

        #region AccessKeys
        private void Mnu_EditAccessKeys_Click(object sender, RoutedEventArgs e)
        {
            SharedAccessKeysEditorWindow editor = new SharedAccessKeysEditorWindow(this.BehaviorTree.AccessKeys);
            editor.ShowDialog();
            SetChanged(editor.IsChanged);
        }
        #endregion

        #region ShowParameters

        private void Mnu_ShowParameters_Click(object sender, RoutedEventArgs e)
        {
            BehaviorTree.ShowParameters = !BehaviorTree.ShowParameters;
        }

        #endregion

        #region Debug

        private void Mnu_ShowAnimations_Click(object sender, RoutedEventArgs e)
        {
            ShowAnimations = !ShowAnimations;
        }


        private TimeSpan _DebugTimeInterval;
        private DispatcherTimer _DebugTimer;
        private DebugBehaviorTree _DebugBehaviorTree;
        private DebugRandomService _DebugRandomService;

        private ObservableCollection<double> _TimeIntervals;
        public ObservableCollection<double> TimeIntervals
        {
            get { return _TimeIntervals; }
            set
            {
                if (_TimeIntervals != value)
                {
                    _TimeIntervals = value;
                    RaisePropertyChanged("TimeIntervals");
                }
            }
        }

        private void InitialDebug()
        {
            _DebugTimer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            _DebugTimer.Tick += new EventHandler(DebugTimer_Tick);
            _DebugBehaviorTree = new DebugBehaviorTree(BehaviorTree.Root.Debug.Behavior);
            _DebugRandomService = new DebugRandomService();

            ObservableCollection<double> timeIntervals = new ObservableCollection<double>();
            timeIntervals.Add(0.25f);
            timeIntervals.Add(0.5f);
            timeIntervals.Add(0.75f);
            timeIntervals.Add(1.0f);
            timeIntervals.Add(2.0f);
            this.TimeIntervals = timeIntervals;

            _CmbTimeInterval.SelectedIndex = 3;
        }

        void DebugTimer_Tick(object sender, EventArgs e)
        {
            StepDebug();
        }

        private void StartDebug()
        {
            Skill.AI.RandomSelector.RandomService = _DebugRandomService;
            BehaviorTree.Root.Debug.UpdateChildren();
            BehaviorTree.IsDebuging = true;
            BehaviorTree.DebugTimer = new TimeSpan();
            _DebugTimer.IsEnabled = _BtnPause.IsChecked == false;
        }
        private void PauseDebug()
        {
            if (_BtnPlayStop.IsChecked == true)
                _BtnPause.IsChecked = true;
            _DebugTimer.IsEnabled = false;
        }
        private void ResumeDebug()
        {
            _DebugTimer.IsEnabled = true;
        }
        private void StopDebug()
        {
            if (BehaviorTree != null)
            {
                BehaviorTree.IsDebuging = false;
                BehaviorTree.Root.Debug.ValidateBrush(false);
                BehaviorTree.DebugTimer = new TimeSpan();
            }
            _DebugTimer.IsEnabled = false;
        }
        private void StepDebug()
        {
            if (BehaviorTree == null)
            {
                if (_DebugTimer != null)
                    _DebugTimer.Stop();
                return;
            }

            if (!BehaviorTree.IsDebuging) return;
            BehaviorTree.DebugTimer += _DebugTimeInterval;
            _DebugBehaviorTree.Update();

            // notify debug behaviors that all ExecutionSequence is visited
            for (int i = 0; i < _DebugBehaviorTree.State.SequenceCount; i++)
            {
                var item = _DebugBehaviorTree.State.ExecutionSequence[i];
                if (item == null) break;
                BehaviorViewModel d = item.Tag as BehaviorViewModel;
                if (d != null)
                    d.Debug.IsVisited = true;
            }

            BehaviorTree.Root.Debug.ValidateBrush(true);
            UpdateDebugAnimations();
        }

        private void UpdateDebugAnimations()
        {
            List<GifImage> activeAnimations = new List<GifImage>();
            foreach (var item in _DebugBehaviorTree.State.RunningActions)
            {
                ActionViewModel ac = item.Action.Tag as ActionViewModel;
                if (ac != null)
                    activeAnimations.Add(ac.GifAnimation);
            }

            bool newAnimation = false;
            foreach (var anim in activeAnimations)
            {
                if (!Animations.Contains(anim))
                {
                    if (!string.IsNullOrEmpty(anim.GifSource))
                    {
                        Animations.Add(anim);
                        anim.Play();
                        newAnimation = true;
                    }
                }
            }

            if (newAnimation)
            {
                int index = 0;
                while (index < Animations.Count)
                {
                    if (!activeAnimations.Contains(Animations[index]))
                    {
                        Animations[index].Stop();
                        Animations.RemoveAt(index);
                        continue;
                    }
                    index++;
                }
            }

        }

        private void BtnPlayStop_Click(object sender, RoutedEventArgs e)
        {
            if (_BtnPlayStop.IsChecked == true)
            {
                StartDebug();
            }
            else
            {
                StopDebug();
            }

        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (_BtnPlayStop.IsChecked == true)
            {
                if (_BtnPause.IsChecked == true)
                    PauseDebug();
                else
                    ResumeDebug();
            }
        }

        private void BtnStep_Click(object sender, RoutedEventArgs e)
        {
            StepDebug();
        }

        private void CmbTimeInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_CmbTimeInterval.SelectedIndex >= 0)
            {
                _DebugTimeInterval = TimeSpan.FromMilliseconds((double)_CmbTimeInterval.SelectedItem * 1000);
                _DebugTimer.Interval = _DebugTimeInterval;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            PauseDebug();
            base.OnClosing(e);
        }

        protected override void OnClosed()
        {
            StopDebug();
            base.OnClosed();
        }
        #endregion






    }
}
