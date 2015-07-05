using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using Skill.Editor.UI;

namespace Skill.Editor.AI
{
    public class TreeViewEditor : Grid
    {
        private TreeView _TreeView;
        private StackPanel _TreeViewToolbar;
        private Box _TreeViewToolbarBg;
        private Button _BtnMoveUp;
        private Button _BtnMoveDown;
        private Button _BtnRemoveBehavior;
        private MenuBar _TreeViewMenuBar;
        private MenuBarItem _InsertBehaviorMenuItem;
        private Skill.Editor.UI.MenuItem _InsertActions;
        private Skill.Editor.UI.MenuItem _InsertConditions;
        private Skill.Editor.UI.MenuItem _InsertDecorators;
        private Skill.Editor.UI.MenuItem _InsertComposites;
        private Skill.Editor.UI.MenuItem _InsertChangeStates;

        private MenuBarItem _AddBehaviorMenuItem;
        private Skill.Editor.UI.MenuItem _AddAction;
        private Skill.Editor.UI.MenuItem _AddCondition;
        private Skill.Editor.UI.MenuItem _AddDecorator;
        private Skill.Editor.UI.MenuItem _AddDefaultDecorator;
        private Skill.Editor.UI.MenuItem _AddAccessLimitDecorator;
        private Skill.Editor.UI.MenuItem _AddComposite;
        private Skill.Editor.UI.MenuItem _AddConcurrentComposite;
        private Skill.Editor.UI.MenuItem _AddSequenceComposite;
        private Skill.Editor.UI.MenuItem _AddRandomComposite;
        private Skill.Editor.UI.MenuItem _AddPriorityComposite;
        private Skill.Editor.UI.MenuItem _AddLoopComposite;

        private BehaviorTreeEditorWindow _Editor;

        private bool _BehaviorsChanged;
        internal void BehaviorsChanged()
        {
            _BehaviorsChanged = true;
        }


        internal void BehaviorNameChanged(BehaviorData b)
        {
            switch (b.BehaviorType)
            {
                case Skill.Framework.AI.BehaviorType.Action:
                    BehaviorNameChanged(b, _InsertActions);
                    break;
                case Skill.Framework.AI.BehaviorType.Condition:
                    BehaviorNameChanged(b, _InsertConditions);
                    break;
                case Skill.Framework.AI.BehaviorType.Decorator:
                    BehaviorNameChanged(b, _InsertDecorators);
                    break;
                case Skill.Framework.AI.BehaviorType.Composite:
                    BehaviorNameChanged(b, _InsertComposites);
                    break;
                case Skill.Framework.AI.BehaviorType.ChangeState:
                    BehaviorNameChanged(b, _InsertChangeStates);
                    break;
                default:
                    break;
            }
        }
        private void BehaviorNameChanged(BehaviorData b, MenuItem items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (b == items[i].UserData)
                {
                    items[i].Name = b.Name;
                    break;
                }
            }
        }

        public TreeViewEditor(BehaviorTreeEditorWindow editor)
        {
            this._Editor = editor;
            this.RowDefinitions.Add(20, GridUnitType.Pixel);
            this.RowDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(70, GridUnitType.Pixel);
            this.ColumnDefinitions.Add(90, GridUnitType.Pixel);
            this.ColumnDefinitions.Add(1, GridUnitType.Star);

            _TreeView = new Skill.Editor.UI.TreeView() { Row = 1, Column = 0, ColumnSpan = 4 };
            _TreeView.DisableFocusable();
            _TreeView.Background.Visibility = Visibility.Visible;
            this.Controls.Add(_TreeView);

            _TreeViewMenuBar = new Skill.Editor.UI.MenuBar() { Row = 0, Column = 0 };
            this.Controls.Add(_TreeViewMenuBar);

            #region Add menu
            _AddBehaviorMenuItem = new MenuBarItem();
            _TreeViewMenuBar.Controls.Add(_AddBehaviorMenuItem);

            _AddAction = new Skill.Editor.UI.MenuItem("Action") { UserData = Skill.Framework.AI.BehaviorType.Action };
            _AddAction.Click += AddMenuItem_Click;
            _AddBehaviorMenuItem.Add(_AddAction);

            _AddCondition = new Skill.Editor.UI.MenuItem("Condition") { UserData = Skill.Framework.AI.BehaviorType.Condition };
            _AddCondition.Click += AddMenuItem_Click;
            _AddBehaviorMenuItem.Add(_AddCondition);

            _AddDecorator = new Skill.Editor.UI.MenuItem("Decorator");
            _AddBehaviorMenuItem.Add(_AddDecorator);

            _AddDefaultDecorator = new Skill.Editor.UI.MenuItem("Default") { Tag = "Decorator", UserData = Skill.Framework.AI.DecoratorType.Default };
            _AddDefaultDecorator.Click += AddMenuItem_Click;
            _AddDecorator.Add(_AddDefaultDecorator);

            _AddAccessLimitDecorator = new Skill.Editor.UI.MenuItem("AccessLimit") { Tag = "Decorator", UserData = Skill.Framework.AI.DecoratorType.AccessLimit };
            _AddAccessLimitDecorator.Click += AddMenuItem_Click;
            _AddDecorator.Add(_AddAccessLimitDecorator);

            _AddComposite = new Skill.Editor.UI.MenuItem("Composite");
            _AddBehaviorMenuItem.Add(_AddComposite);

            _AddSequenceComposite = new Skill.Editor.UI.MenuItem("Sequence") { Tag = "Composite", UserData = Skill.Framework.AI.CompositeType.Sequence };
            _AddSequenceComposite.Click += AddMenuItem_Click;
            _AddComposite.Add(_AddSequenceComposite);

            _AddPriorityComposite = new Skill.Editor.UI.MenuItem("Priority") { Tag = "Composite", UserData = Skill.Framework.AI.CompositeType.Priority };
            _AddPriorityComposite.Click += AddMenuItem_Click;
            _AddComposite.Add(_AddPriorityComposite);

            _AddConcurrentComposite = new Skill.Editor.UI.MenuItem("Concurrent") { Tag = "Composite", UserData = Skill.Framework.AI.CompositeType.Concurrent };
            _AddConcurrentComposite.Click += AddMenuItem_Click;
            _AddComposite.Add(_AddConcurrentComposite);

            _AddRandomComposite = new Skill.Editor.UI.MenuItem("Random") { Tag = "Composite", UserData = Skill.Framework.AI.CompositeType.Random };
            _AddRandomComposite.Click += AddMenuItem_Click;
            _AddComposite.Add(_AddRandomComposite);

            _AddLoopComposite = new Skill.Editor.UI.MenuItem("Loop") { Tag = "Composite", UserData = Skill.Framework.AI.CompositeType.Loop };
            _AddLoopComposite.Click += AddMenuItem_Click;
            _AddComposite.Add(_AddLoopComposite);

            #endregion

            #region Insert menu
            _InsertBehaviorMenuItem = new Skill.Editor.UI.MenuBarItem();
            _TreeViewMenuBar.Controls.Add(_InsertBehaviorMenuItem);

            _InsertActions = new Skill.Editor.UI.MenuItem("Actions");
            _InsertBehaviorMenuItem.Add(_InsertActions);

            _InsertConditions = new Skill.Editor.UI.MenuItem("Conditions");
            _InsertBehaviorMenuItem.Add(_InsertConditions);

            _InsertDecorators = new Skill.Editor.UI.MenuItem("Decorators");
            _InsertBehaviorMenuItem.Add(_InsertDecorators);

            _InsertComposites = new Skill.Editor.UI.MenuItem("Composites");
            _InsertBehaviorMenuItem.Add(_InsertComposites);

            _InsertChangeStates = new Skill.Editor.UI.MenuItem("ChangeStates");
            _InsertBehaviorMenuItem.Add(_InsertChangeStates);

            #endregion

            _TreeViewToolbarBg = new Box() { Row = 0, Column = 2 };
            this.Controls.Add(_TreeViewToolbarBg);

            _TreeViewToolbar = new StackPanel() { Row = 0, Column = 1, Orientation = Orientation.Horizontal };
            this.Controls.Add(_TreeViewToolbar);


            _BtnRemoveBehavior = new Button() { Width = 30 };
            _TreeViewToolbar.Controls.Add(_BtnRemoveBehavior);

            _BtnMoveUp = new Button() { Width = 30 };
            _TreeViewToolbar.Controls.Add(_BtnMoveUp);

            _BtnMoveDown = new Button() { Width = 30 };
            _TreeViewToolbar.Controls.Add(_BtnMoveDown);

            CheckMenuAvailable();
            _TreeView.SelectedItemChanged += _TreeView_SelectedItemChanged;

            _BtnMoveUp.Click += _BtnMoveUp_Click;
            _BtnMoveDown.Click += _BtnMoveDown_Click;
            _BtnRemoveBehavior.Click += _BtnRemoveBehavior_Click;
        }

        void _BtnRemoveBehavior_Click(object sender, System.EventArgs e)
        {
            RemoveSelectedBehavior();
        }

        void _BtnMoveDown_Click(object sender, System.EventArgs e)
        {
            MoveDown();
        }

        void _BtnMoveUp_Click(object sender, System.EventArgs e)
        {
            MoveUp();
        }

        void _TreeView_SelectedItemChanged(object sender, System.EventArgs e)
        {
            CheckMenuAvailable();
            if (_TreeView.SelectedItem != null)
            {
                Skill.Editor.UI.InspectorProperties.Select(_TreeView.SelectedItem as Skill.Editor.UI.IProperties);
            }
            else
            {
                var selected = Skill.Editor.UI.InspectorProperties.GetSelected();
                if (selected != null && (selected is TreeViewFolder || selected is TreeViewItem))
                    Skill.Editor.UI.InspectorProperties.Select(null);
            }
            _Editor.Repaint();
        }

        private void CheckMenuAvailable()
        {
            IBehaviorItem selected = null;
            if (_TreeView.SelectedItem != null)
                selected = _TreeView.SelectedItem as IBehaviorItem;

            bool canDelete = true;
            bool canNew = false;
            bool canMoveUp = false;
            bool canMoveDown = false;
            if (selected != null)
            {
                if (selected.Data.BehaviorType == Framework.AI.BehaviorType.Composite)
                {
                    CompositeData composite = (CompositeData)selected.Data;
                    if (composite.CompositeType == Framework.AI.CompositeType.State)
                        canDelete = false;

                    canNew = true;
                }
                else if (selected.Data.BehaviorType == Framework.AI.BehaviorType.Decorator)
                {
                    DecoratorData decorator = (DecoratorData)selected.Data;
                    canNew = decorator.Count == 0;
                }
                else
                {
                    canNew = false;
                }

                canMoveUp = selected.CanMoveUp;
                canMoveDown = selected.CanMoveDown;
            }
            else
            {
                canDelete = false;
                canNew = false;
                canMoveUp = false;
                canMoveDown = false;
            }

            _BtnMoveUp.IsEnabled = canMoveUp;
            _BtnMoveDown.IsEnabled = canMoveDown;
            _BtnRemoveBehavior.IsEnabled = canDelete;
            //_BtnCopyBehavior.IsEnabled = canDelete;
            _AddBehaviorMenuItem.IsEnabled = canNew;
            _InsertBehaviorMenuItem.IsEnabled = canNew;
        }

        public void RefreshStyles()
        {
            GUIStyle toolbarStyle = new GUIStyle(Skill.Editor.Resources.Styles.ToolbarButton);
            toolbarStyle.onNormal = toolbarStyle.normal;
            toolbarStyle.onHover = toolbarStyle.hover;
            toolbarStyle.onActive = toolbarStyle.active;

            _TreeView.Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;
            _TreeViewMenuBar.Background.Style = toolbarStyle;
            _AddBehaviorMenuItem.Style = toolbarStyle;
            _AddBehaviorMenuItem.Content.image = Skill.Editor.Resources.UITextures.Plus;
            _InsertBehaviorMenuItem.Style = toolbarStyle;
            _InsertBehaviorMenuItem.Content.image = Skill.Editor.Resources.UITextures.PlusNext;

            _TreeViewToolbarBg.Style = Skill.Editor.Resources.Styles.Header;
            _BtnMoveUp.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnMoveDown.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnRemoveBehavior.Style = Skill.Editor.Resources.Styles.ToolbarButton;

            _TreeView.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;

            _BtnMoveUp.Content.image = Skill.Editor.Resources.UITextures.ArrowUp;
            _BtnMoveDown.Content.image = Skill.Editor.Resources.UITextures.ArrowDown;
            _BtnRemoveBehavior.Content.image = Skill.Editor.Resources.UITextures.Remove;

            _AddAction.Image = Skill.Editor.Resources.UITextures.BTree.Action;
            _AddCondition.Image = Skill.Editor.Resources.UITextures.BTree.Condition;
            _AddDecorator.Image = Skill.Editor.Resources.UITextures.BTree.Decorator;
            _AddDefaultDecorator.Image = Skill.Editor.Resources.UITextures.BTree.Decorator;
            _AddAccessLimitDecorator.Image = Skill.Editor.Resources.UITextures.BTree.AccessLimitDecorator;
            _AddComposite.Image = Skill.Editor.Resources.UITextures.BTree.Composite;
            _AddConcurrentComposite.Image = Skill.Editor.Resources.UITextures.BTree.Concurrent;
            _AddSequenceComposite.Image = Skill.Editor.Resources.UITextures.BTree.Sequence;
            _AddRandomComposite.Image = Skill.Editor.Resources.UITextures.BTree.Random;
            _AddPriorityComposite.Image = Skill.Editor.Resources.UITextures.BTree.Priority;
            _AddLoopComposite.Image = Skill.Editor.Resources.UITextures.BTree.Loop;

            _InsertActions.Image = Skill.Editor.Resources.UITextures.BTree.Action;
            _InsertConditions.Image = Skill.Editor.Resources.UITextures.BTree.Condition;
            _InsertDecorators.Image = Skill.Editor.Resources.UITextures.BTree.Decorator;
            _InsertComposites.Image = Skill.Editor.Resources.UITextures.BTree.Composite;
            _InsertChangeStates.Image = Skill.Editor.Resources.UITextures.BTree.ChangeState;
        }

        private BehaviorTreeStateData _State;
        public BehaviorTreeStateData State
        {
            get { return _State; }
            set
            {
                if (_State != value)
                {
                    Clear();
                    _State = value;
                    if (_State != null)
                        Rebuild();
                }
            }
        }

        private void Rebuild()
        {
            if (_State != null)
            {
                BehaviorTreeStateItem root = new BehaviorTreeStateItem(_State);
                _TreeView.Controls.Add(root);
                _TreeView.ExpandAll();
            }
        }

        private void Clear()
        {
            _TreeView.Controls.Clear();
        }

        protected override void BeginRender()
        {
            ApplyInsertChanges();
            base.BeginRender();
        }

        public void DeselectInspector()
        {
            if (Skill.Editor.UI.InspectorProperties.GetSelected() is IBehaviorItem)
                Skill.Editor.UI.InspectorProperties.Select(null);
        }

        internal void RefresTree()
        {
            foreach (TreeViewFolder item in _TreeView.Controls)
                item.RefreshChildren();
        }

        internal void RefreshContents()
        {
            foreach (TreeViewFolder item in _TreeView.Controls)
                item.RefreshContent();
        }

        internal void RefreshSameContent(IBehaviorItem item)
        {
            foreach (TreeViewFolder tvf in _TreeView.Controls)
                tvf.RefreshSameContent(item);
        }

        private void ApplyInsertChanges()
        {
            if (_BehaviorsChanged)
            {
                _BehaviorsChanged = false;
                BehaviorData[] behaviors = _Editor.GetBehaviors();

                _InsertActions.Clear();
                _InsertConditions.Clear();
                _InsertDecorators.Clear();
                _InsertComposites.Clear();
                _InsertChangeStates.Clear();

                foreach (var b in behaviors)
                {
                    switch (b.BehaviorType)
                    {
                        case Skill.Framework.AI.BehaviorType.Action:
                            Skill.Editor.UI.MenuItem actionItem = new Skill.Editor.UI.MenuItem(b.Name) { UserData = b, Image = b.GetIcon() };
                            _InsertActions.Add(actionItem);
                            actionItem.Click += InsertMenuItem_Click;
                            break;

                        case Skill.Framework.AI.BehaviorType.Condition:
                            Skill.Editor.UI.MenuItem conditionItem = new Skill.Editor.UI.MenuItem(b.Name) { UserData = b, Image = b.GetIcon() };
                            _InsertConditions.Add(conditionItem);
                            conditionItem.Click += InsertMenuItem_Click;
                            break;

                        case Skill.Framework.AI.BehaviorType.Decorator:
                            Skill.Editor.UI.MenuItem decoratorItem = new Skill.Editor.UI.MenuItem(b.Name) { UserData = b, Image = b.GetIcon() };
                            _InsertDecorators.Add(decoratorItem);
                            decoratorItem.Click += InsertMenuItem_Click;
                            break;

                        case Skill.Framework.AI.BehaviorType.Composite:
                            Skill.Editor.UI.MenuItem compositeItem = new Skill.Editor.UI.MenuItem(b.Name) { UserData = b, Image = b.GetIcon() };
                            _InsertComposites.Add(compositeItem);
                            compositeItem.Click += InsertMenuItem_Click;
                            break;

                        case Skill.Framework.AI.BehaviorType.ChangeState:
                            Skill.Editor.UI.MenuItem changeStateItem = new Skill.Editor.UI.MenuItem(b.Name) { UserData = b, Image = b.GetIcon() };
                            _InsertChangeStates.Add(changeStateItem);
                            changeStateItem.Click += InsertMenuItem_Click;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        void InsertMenuItem_Click(object sender, System.EventArgs e)
        {
            if (_TreeView.SelectedItem == null) return;
            if (!(_TreeView.SelectedItem is TreeViewFolder)) return;

            Skill.Editor.UI.MenuItem item = (Skill.Editor.UI.MenuItem)sender;
            BehaviorData behavior = (BehaviorData)item.UserData;

            if (behavior != null)
            {
                TreeViewFolder tvf = (TreeViewFolder)_TreeView.SelectedItem;

                string msg;
                if (tvf.CanAddBehavior(behavior, out msg))
                {
                    tvf.Foldout.IsOpen = true;
                    tvf.AddBehavior(behavior);
                    if (behavior is IParameterData)
                    {
                        var parameters = tvf.Data.GetParameters(tvf.Controls.Count - 1);
                        if (parameters != null)
                            parameters.Match(((IParameterData)behavior).ParameterDifinition);
                    }

                    var addedControl = tvf.Controls[tvf.Controls.Count - 1];
                    if (addedControl is IBehaviorItem)
                        ((IBehaviorItem)addedControl).RefreshContent();
                    _Editor.AddToList(behavior);
                    _Editor.RefreshTree();
                    SelectItem(tvf, behavior);
                }
                else
                {
                    Debug.LogError(msg);
                }
            }
        }

        void AddMenuItem_Click(object sender, System.EventArgs e)
        {
            if (_TreeView.SelectedItem == null) return;
            if (!(_TreeView.SelectedItem is TreeViewFolder)) return;


            Skill.Editor.UI.MenuItem item = (Skill.Editor.UI.MenuItem)sender;
            BehaviorData behavior = null;
            if (string.IsNullOrEmpty(item.Tag))
            {
                Skill.Framework.AI.BehaviorType type = (Framework.AI.BehaviorType)item.UserData;
                switch (type)
                {
                    case Skill.Framework.AI.BehaviorType.Action:
                        behavior = new ActionData() { Name = _Editor.GetUniqueName("NewAction") };
                        break;
                    case Skill.Framework.AI.BehaviorType.Condition:
                        behavior = new ConditionData() { Name = _Editor.GetUniqueName("NewCondition") };
                        break;
                    default:
                        behavior = null;
                        break;
                }
            }
            else if (item.Tag == "Decorator")
            {
                Skill.Framework.AI.DecoratorType type = (Framework.AI.DecoratorType)item.UserData;
                switch (type)
                {
                    case Skill.Framework.AI.DecoratorType.Default:
                        behavior = new DecoratorData() { Name = _Editor.GetUniqueName("NewDecorator") };
                        break;
                    case Skill.Framework.AI.DecoratorType.AccessLimit:
                        behavior = new AccessLimitDecoratorData() { Name = _Editor.GetUniqueName("NewAccessDecorator") };
                        break;
                    default:
                        behavior = null;
                        break;
                }
            }
            else if (item.Tag == "Composite")
            {
                Skill.Framework.AI.CompositeType type = (Framework.AI.CompositeType)item.UserData;
                switch (type)
                {
                    case Skill.Framework.AI.CompositeType.Sequence:
                        behavior = new SequenceSelectorData() { Name = _Editor.GetUniqueName("NewSequence") };
                        break;
                    case Skill.Framework.AI.CompositeType.Concurrent:
                        behavior = new ConcurrentSelectorData() { Name = _Editor.GetUniqueName("NewConcurrent") };
                        break;
                    case Skill.Framework.AI.CompositeType.Random:
                        behavior = new RandomSelectorData() { Name = _Editor.GetUniqueName("NewRandom") };
                        break;
                    case Skill.Framework.AI.CompositeType.Priority:
                        behavior = new PrioritySelectorData() { Name = _Editor.GetUniqueName("NewPriority") };
                        break;
                    case Skill.Framework.AI.CompositeType.Loop:
                        behavior = new LoopSelectorData() { Name = _Editor.GetUniqueName("NewLoop") };
                        break;
                    default:
                        behavior = null;
                        break;
                }
            }

            if (behavior != null)
            {
                TreeViewFolder tvf = (TreeViewFolder)_TreeView.SelectedItem;
                tvf.Foldout.IsOpen = true;
                tvf.AddBehavior(behavior);
                _Editor.AddToList(behavior);
                _Editor.RefreshTree();
                SelectItem(tvf, behavior);
            }
        }

        private void SelectItem(BehaviorData behavior)
        {
            foreach (var control in _TreeView.Controls)
            {
                if (control is TreeViewFolder)
                    SelectItem((TreeViewFolder)control, behavior);
            }
        }

        private void SelectItem(TreeViewFolder folder, BehaviorData behavior)
        {
            foreach (var control in folder.Controls)
            {
                IBehaviorItem item = (IBehaviorItem)control;
                if (item.Data == behavior)
                {
                    _TreeView.SelectedItem = control;
                    return;
                }
                if (control is TreeViewFolder)
                    SelectItem((TreeViewFolder)control, behavior);
            }
        }


        private void MoveUp()
        {
            if (_TreeView.SelectedItem == null) return;
            IBehaviorItem item = _TreeView.SelectedItem as IBehaviorItem;
            if (item.CanMoveUp)
            {
                TreeViewFolder tvf = (TreeViewFolder)_TreeView.SelectedItem.Parent;
                tvf.MoveUp(item);
                SelectItem(tvf, item.Data);
            }
        }
        private void MoveDown()
        {
            if (_TreeView.SelectedItem == null) return;
            IBehaviorItem item = _TreeView.SelectedItem as IBehaviorItem;
            if (item.CanMoveDown)
            {
                TreeViewFolder tvf = (TreeViewFolder)_TreeView.SelectedItem.Parent;
                tvf.MoveDown(item);
                SelectItem(tvf, item.Data);
            }
        }
        private void RemoveSelectedBehavior()
        {
            if (_TreeView.SelectedItem == null) return;
            IBehaviorItem item = _TreeView.SelectedItem as IBehaviorItem;
            TreeViewFolder tvf = (TreeViewFolder)_TreeView.SelectedItem.Parent;
            tvf.RemoveBehavior(item);
            SelectItem(null);
        }


    }
}