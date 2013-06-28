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

        #region Menu Availability

        void CheckContextMnuAvailability()
        {
            BehaviorViewModel selected = _TreeView.SelectedItem;
            if (selected != null && !BehaviorTree.IsDebuging)
            {
                _TreeView.IsDeleteAv = _GraphView.IsCopyAv = selected != BehaviorTree.Root;
                if (selected.Model.BehaviorType == BehaviorType.Composite)
                {
                    _TreeView.IsNewAv = _GraphView.IsNewAv = true;
                }
                else if (selected.Model.BehaviorType == BehaviorType.Decorator)
                {
                    _TreeView.IsNewAv = _GraphView.IsNewAv = selected.Count == 0;
                }
                else
                {
                    _TreeView.IsNewAv = _GraphView.IsNewAv = false;
                }

                _TreeView.IsMoveUpAv = _GraphView.IsMoveUpAv = selected.CanMoveUp;
                _TreeView.IsMoveDownAv = _GraphView.IsMoveDownAv = selected.CanMoveDown;

            }
            else
            {
                _TreeView.IsNewAv = _GraphView.IsNewAv = false;
                _TreeView.IsMoveDownAv = _GraphView.IsMoveDownAv = false;
                _TreeView.IsMoveUpAv = _GraphView.IsMoveUpAv = false;
                _TreeView.IsDeleteAv = _GraphView.IsDeleteAv = false;
                _TreeView.IsCopyAv = _GraphView.IsCopyAv = false;
            }
        }
        #endregion

        #region Properties

        protected override bool UndoAvailable { get { return !BehaviorTree.IsDebuging; } }
        protected override bool RedoAvailable { get { return !BehaviorTree.IsDebuging; } }

        /// <summary> BehaviorTree ViewModel </summary>
        public BehaviorTreeViewModel BehaviorTree { get; private set; }

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
                    ShowAnimations = _AnimationSize > _SliAnims.Minimum;
                }
            }
        }


        private bool _AllowEdit = true;
        public bool AllowEdit
        {
            get { return _AllowEdit; }
            set
            {
                if (_AllowEdit != value)
                {
                    _AllowEdit = value;
                    RaisePropertyChanged("AllowEdit");                    
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
                    Data = BehaviorTree = _GraphView.BehaviorTree = _TreeView.BehaviorTree = new BehaviorTreeViewModel(bt);
                    RaisePropertyChanged("BehaviorTree");
                }
            }

            this._GraphView.Editor = this;
            this._TreeView.Editor = this;
            this.ParentDocument.Closed += ParentDocument_Closed;

            InitialDebug();

            History.RedoChange += History_RedoChange;
            History.UndoChange += History_UndoChange;

            for (int i = 0; i < BehaviorTree.States.Count; i++)
            {
                if (BehaviorTree.States[i].Name == BehaviorTree.DefaultState)
                {
                    BehaviorTree.States[i].IsDefaultState = true;
                    SetState(i);
                    break;
                }
            }
        }

        private void SetState(int index)
        {
            _CmbStatesDesign.SelectedIndex = index;
        }

        #endregion

        #region History events
        // hook events of History
        void History_RedoChange(UnDoRedo sender, UnDoRedoChangeEventArgs e)
        {
            if (e.Command is AddBehaviorUnDoRedo || e.Command is MoveUpBehaviorUnDoRedo)
                _GraphView.RefreshGraph();
        }

        void History_UndoChange(UnDoRedo sender, UnDoRedoChangeEventArgs e)
        {
            if (e.Command is AddBehaviorUnDoRedo || e.Command is MoveUpBehaviorUnDoRedo)
                _GraphView.RefreshGraph();
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
                    this.Focus();
                }
            }
        }


        public void SelectedItemChanged()
        {
            CheckContextMnuAvailability();
            BehaviorViewModel vm = _TreeView.SelectedItem;
            ApplicationCommands.Properties.Execute(vm, null);
            if (!BehaviorTree.IsDebuging)
            {

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

        #endregion

        #region Insert
        public void Insert(BehaviorViewModel insertItem)
        {
            if (insertItem != null)
            {
                var selected = _TreeView.SelectedItem;
                if (selected != null)
                {
                    try
                    {
                        string message;
                        if (selected.CanAddBehavior(insertItem, out message))
                        {
                            var newVM = selected.AddBehavior(insertItem, null, true);
                            if (newVM != null)
                            {
                                newVM.IsSelected = true;
                                foreach (BehaviorViewModel vm in BehaviorTree.GetSharedModel(newVM.Model))
                                {
                                    if (vm != newVM)
                                    {
                                        ParameterCollectionViewModel parameters = null;
                                        if (vm.Model.BehaviorType == DataModels.AI.BehaviorType.Action)
                                            parameters = ((ActionViewModel)vm).Parameters;
                                        else if (vm.Model.BehaviorType == DataModels.AI.BehaviorType.Decorator)
                                            parameters = ((DecoratorViewModel)vm).Parameters;
                                        else if (vm.Model.BehaviorType == DataModels.AI.BehaviorType.Condition)
                                            parameters = ((ConditionViewModel)vm).Parameters;

                                        if (parameters != null)
                                        {
                                            newVM.MatchParameters(parameters.Model);
                                            break;
                                        }
                                    }
                                }

                            }
                            _GraphView.RefreshGraph();
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
        public void AddCondition()
        {
            var selected = _TreeView.SelectedItem;
            if (selected != null)
            {
                var newVM = selected.AddCondition();
                if (newVM != null)
                {
                    newVM.IsSelected = true;
                }
                _GraphView.RefreshGraph();
            }
        }

        public void AddAction()
        {

            var selected = _TreeView.SelectedItem;
            if (selected != null)
            {
                var newVM = selected.AddAction();
                if (newVM != null)
                {

                    newVM.IsSelected = true;
                }
                _GraphView.RefreshGraph();
            }

        }

        public void AddChangeState()
        {
            var selected = _TreeView.SelectedItem;
            if (selected != null)
            {
                var newVM = selected.AddChangeState();
                if (newVM != null)
                {
                    newVM.IsSelected = true;
                }
            }
        }

        public void AddComposite(CompositeType compositeType)
        {

            var selected = _TreeView.SelectedItem;
            if (selected != null)
            {
                var newVM = selected.AddComposite(compositeType);
                if (newVM != null)
                {
                    newVM.IsSelected = true;
                }
                _GraphView.RefreshGraph();
            }
        }

        public void AddDecorator(DecoratorType decoratorType)
        {
            var selected = _TreeView.SelectedItem;
            if (selected != null)
            {
                var newVM = selected.AddDecorator(decoratorType);
                if (newVM != null)
                {
                    newVM.IsSelected = true;
                }
                _GraphView.RefreshGraph();
            }
        }


        #endregion

        #region Move
        public void MoveSelectedUp()
        {
            var selected = _TreeView.SelectedItem;
            if (selected != null && selected.CanMoveUp)
            {
                if (selected.Parent != null)
                {
                    ((BehaviorViewModel)selected.Parent).MoveUp(selected);
                    _GraphView.RefreshGraph();
                }
            }
        }

        public void MoveSelectedDown()
        {
            var selected = _TreeView.SelectedItem;
            if (selected != null && selected.CanMoveDown)
            {
                if (selected.Parent != null)
                {
                    ((BehaviorViewModel)selected.Parent).MoveDown(selected);
                    _GraphView.RefreshGraph();
                }
            }
        }
        #endregion

        #region Delete
        private void DeleteSelected()
        {
            var selected = _TreeView.SelectedItem;
            if (selected != null)
            {
                if (selected.Parent != null)
                {
                    if (selected.Parent is BehaviorViewModel)
                    {
                        ((BehaviorViewModel)selected.Parent).RemoveBehavior(selected);
                        _GraphView.RefreshGraph();
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
            e.CanExecute = _TreeView.IsCopyAv;
            e.Handled = true;
        }
        void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            var selected = _TreeView.SelectedItem;
            if (selected != null)
                CopyBehaviorToClipboard(selected);
            e.Handled = true;
        }

        void PasteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var selected = _TreeView.SelectedItem;
            e.CanExecute = Clipboard.ContainsData(Skill.DataModels.SkillDataFormats.Behavior) && selected != null && _TreeView.IsNewAv;
            e.Handled = true;
        }
        void PasteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            PasteFromClipboard();
            _GraphView.RefreshGraph();
            e.Handled = true;
        }

        private void PasteFromClipboard()
        {
            if (Clipboard.ContainsData(Skill.DataModels.SkillDataFormats.Behavior))
            {
                var selected = _TreeView.SelectedItem;
                string text = Clipboard.GetData(Skill.DataModels.SkillDataFormats.Behavior) as string;
                if (text != null && selected != null && _TreeView.IsNewAv)
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
            e.CanExecute = _TreeView.IsDeleteAv;
            e.Handled = true;
        }
        void DeleteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (_TreeView.IsDeleteAv)
                DeleteSelected();
            e.Handled = true;
        }
        #endregion

        #region AccessKeys
        private void BtnEditAccessKeys_Click(object sender, RoutedEventArgs e)
        {
            SharedAccessKeysEditorWindow editor = new SharedAccessKeysEditorWindow(this.BehaviorTree.AccessKeys);
            editor.ShowDialog();
            SetChanged(editor.IsChanged);
        }
        #endregion

        #region Focus on click
        private void Back_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }
        #endregion

        #region State

        public bool IsDefaultStateSelected
        {
            get
            {
                if (BehaviorTree.Root != null)
                    return BehaviorTree.DefaultState == BehaviorTree.Root.Name;
                return false;
            }
        }

        private void BtnAddNewState_Click(object sender, RoutedEventArgs e)
        {
            BehaviorTree.AddNewState();
        }

        private void BtnDeleteState_Click(object sender, RoutedEventArgs e)
        {
            if (BehaviorTree.States.Count <= 1)
            {
                System.Windows.MessageBox.Show("Can not delete state. Behavior Tree needs at least one state");
                return;
            }
            BehaviorTree.RemoveState(BehaviorTree.Root);
            if (BehaviorTree.Root != null)
                SetState(_CmbStatesDesign.Items.IndexOf(BehaviorTree.Root));
            else
                SetState(-1);
        }

        private void BtnSetasDefaultState_Click(object sender, RoutedEventArgs e)
        {
            if (BehaviorTree.Root != null && !BehaviorTree.Root.IsDefaultState)
            {
                BehaviorTree.DefaultState = BehaviorTree.Root.Name;
                RaisePropertyChanged("IsDefaultStateSelected");
                SetChanged(true);
            }
        }
        private void CmbStates_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (BehaviorTree.IsDebuging) return;
            BehaviorViewModel b = ((ComboBox)sender).SelectedItem as BehaviorViewModel;

            if (b != null)
            {
                BehaviorTree.ChangeState(b.Name);
                RaisePropertyChanged("IsDefaultStateSelected");
                SetState(((ComboBox)sender).Items.IndexOf(b));
            }

        }
        #endregion

        #region Debug

        private string _PreState;
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
            _DebugBehaviorTree = new DebugBehaviorTree(BehaviorTree);
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
            AllowEdit = false;
            _PreState = BehaviorTree.Root.Name;
            Skill.Framework.AI.RandomSelector.RandomService = _DebugRandomService;
            foreach (var s in BehaviorTree.States)
            {
                s.Debug.UpdateChildren();
            }
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
            AllowEdit = true;
            if (BehaviorTree != null)
            {
                if (!string.IsNullOrEmpty(_PreState))
                    BehaviorTree.ChangeState(_PreState);
                BehaviorTree.IsDebuging = false;
                foreach (var s in BehaviorTree.States)
                {
                    s.Debug.ValidateBrush(false);
                }
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
            _DebugBehaviorTree.Update(null);

            // notify debug behaviors that all ExecutionSequence is visited
            for (int i = 0; i < _DebugBehaviorTree.Status.SequenceCount; i++)
            {
                var item = _DebugBehaviorTree.Status.ExecutionSequence[i];
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
            foreach (var item in _DebugBehaviorTree.Status.RunningActions)
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
                if (IsChanged)
                {
                    if (System.Windows.MessageBox.Show("Save all changes?", "Save", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    {
                        _BtnPlayStop.IsChecked = false;
                        return;
                    }
                }
                MainWindow.Instance.Compile();
                if (MainWindow.Instance.ErrorList.ErrorCount > 0)
                {
                    System.Windows.MessageBox.Show("You have to fix errors.");
                    _BtnPlayStop.IsChecked = false;
                    return;
                }

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

        void ParentDocument_Closed(object sender, EventArgs e)
        {
            StopDebug();
        }

        #endregion
    }
}
