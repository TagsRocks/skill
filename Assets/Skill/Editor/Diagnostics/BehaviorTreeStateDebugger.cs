using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Skill.Editor.UI;
using Skill.Framework.UI;

namespace Skill.Editor.Diagnostics
{
    class BehaviorTreeStateDebugger : UnityEditor.EditorWindow
    {
        #region Constructor
        private static Vector2 Size = new Vector2(340, 200);
        private static BehaviorTreeStateDebugger _Instance;

        public void OnDestroy()
        {
            _Instance = null;
        }
        public static BehaviorTreeStateDebugger Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = EditorWindow.GetWindow<BehaviorTreeStateDebugger>();
                }
                return _Instance;
            }
        }

        public BehaviorTreeStateDebugger()
        {
            if (_Instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton. Self destruction in 3...");
                Destroy(this);
                return;
            }

            _Instance = this;

            title = "BT State";
            position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = Size;
            CreateGUI();
        }

        void OnFocus()
        {
            if (EditorApplication.isPlaying)
                FindController();
        }

        private EditorFrame _Frame;
        private ObjectField<Skill.Framework.Controller> _ControllerField;
        private Editor.UI.Extended.TreeView _TreeView;
        private StackPanel _InfoPanel;
        private Label _SuccessState, _FailurState, _RunningState;
        private GUIStyle _SuccessStyle, _FailurStyle, _RunningStyle;
        private string _State;

        void CreateGUI()
        {
            _SuccessStyle = new GUIStyle() { normal = new GUIStyleState() { background = Resources.UITextures.BTree.Success } };
            _FailurStyle = new GUIStyle() { normal = new GUIStyleState() { background = Resources.UITextures.BTree.Failure } };
            _RunningStyle = new GUIStyle() { normal = new GUIStyleState() { background = Resources.UITextures.BTree.Running } };

            _Frame = new EditorFrame("Frame", this);
            _Frame.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(22, GridUnitType.Pixel) });    // _ControllerField            
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });      // _TreeView
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });    // _InfoPanel

            _ControllerField = new ObjectField<Skill.Framework.Controller>() { Row = 0, Column = 0, ColumnSpan = 3, VerticalAlignment = VerticalAlignment.Center, Height = 18 };
            _ControllerField.ObjectChanged += new EventHandler(_ControllerField_ObjectChanged);

            _TreeView = new UI.Extended.TreeView() { Row = 1, Column = 0 };
            _TreeView.DisableFocusable();
            _TreeView.AutoHeight = true;
            _TreeView.AutoWidth = true;


            _SuccessState = new Label() { Margin = new Thickness(2, 2, 0, 2), Width = 100 };
            _SuccessState.Content.text = "Success";
            _SuccessState.Content.image = Resources.UITextures.BTree.Success;
            _RunningState = new Label() { Margin = new Thickness(2, 2, 0, 2), Width = 100 };
            _RunningState.Content.text = "Running";
            _RunningState.Content.image = Resources.UITextures.BTree.Running;
            _FailurState = new Label() { Margin = new Thickness(2, 2, 0, 2), Width = 100 };
            _FailurState.Content.text = "Failure";
            _FailurState.Content.image = Resources.UITextures.BTree.Failure;
            _InfoPanel = new StackPanel() { Row = 2, Column = 0, ColumnSpan = 3, Orientation = Orientation.Horizontal };
            _InfoPanel.Controls.Add(_SuccessState);
            _InfoPanel.Controls.Add(_RunningState);
            _InfoPanel.Controls.Add(_FailurState);


            _Frame.Grid.Controls.Add(_ControllerField);
            _Frame.Grid.Controls.Add(_TreeView);
            _Frame.Grid.Controls.Add(_InfoPanel);
        }

        void _ControllerField_ObjectChanged(object sender, EventArgs e)
        {
            if (_ControllerField.Object != null)
                BehaviorTree = _ControllerField.Object.Behavior;

            if (_ControllerField.Object != null)
            {
                _EdittingControllerId = _ControllerField.Object.gameObject.GetInstanceID();
                UnityEditor.EditorUtility.SetDirty(this);
            }

        }

        #endregion

        class BehaviorTag
        {
            public Skill.Framework.AI.Behavior Behavior { get; private set; }
            public Label Label { get; private set; }

            public BehaviorTag(Skill.Framework.AI.Behavior behavior, Label label)
            {
                this.Behavior = behavior;
                this.Label = label;
            }
        }
        class CompositeTag
        {
            public Skill.Framework.AI.Behavior Behavior { get; private set; }
            public Skill.Editor.UI.Extended.FolderView Folder { get; private set; }

            public CompositeTag(Skill.Framework.AI.Behavior behavior, Skill.Editor.UI.Extended.FolderView folder)
            {
                this.Behavior = behavior;
                this.Folder = folder;
            }
        }

        private List<BehaviorTag> _Behaviors = new List<BehaviorTag>();
        private List<CompositeTag> _Composites = new List<CompositeTag>();

        private Skill.Framework.AI.BehaviorTree _BehaviorTree;
        public Skill.Framework.AI.BehaviorTree BehaviorTree
        {
            get { return _BehaviorTree; }
            set
            {
                if (_BehaviorTree != null)
                    _BehaviorTree.Updated -= _BehaviorTree_Updated;
                _BehaviorTree = value;
                if (_BehaviorTree != null)
                {
                    _BehaviorTree.Updated += _BehaviorTree_Updated;
                    _BehaviorTree.StateChanged += _BehaviorTree_StateChanged;
                }

                RebuildTree();
            }
        }

        void _BehaviorTree_StateChanged(object sender, Framework.AI.ChangeStateEventArgs args)
        {
            if (_State != args.NextState)
            {
                _State = args.NextState;
                RebuildTree();
            }
        }

        private bool IsInExecutionSequence(Skill.Framework.AI.Behavior behavior)
        {
            if (_BehaviorTree == null) return false;
            for (int i = 0; i < _BehaviorTree.Status.SequenceCount; i++)
            {
                if (_BehaviorTree.Status.ExecutionSequence[i] == behavior)
                    return true;
            }
            return false;
        }

        private void ValidateStyle(BehaviorTag bt)
        {
            if (IsInExecutionSequence(bt.Behavior))
            {
                switch (bt.Behavior.Result)
                {
                    case Skill.Framework.AI.BehaviorResult.Success:
                        bt.Label.Style = _SuccessStyle;
                        break;
                    case Skill.Framework.AI.BehaviorResult.Running:
                        bt.Label.Style = _RunningStyle;
                        break;
                    default:
                        bt.Label.Style = _FailurStyle;
                        break;
                }
            }
            else
                bt.Label.Style = null;
        }

        private void ValidateStyle(CompositeTag ct)
        {
            if (IsInExecutionSequence(ct.Behavior))
            {
                switch (ct.Behavior.Result)
                {
                    case Skill.Framework.AI.BehaviorResult.Success:
                        ct.Folder.Foldout.Style = _SuccessStyle;
                        break;
                    case Skill.Framework.AI.BehaviorResult.Running:
                        ct.Folder.Foldout.Style = _RunningStyle;
                        break;
                    default:
                        ct.Folder.Foldout.Style = _FailurStyle;
                        break;
                }
            }
            else
                ct.Folder.Foldout.Style = null;
        }

        void _BehaviorTree_Updated(object sender, EventArgs e)
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                foreach (var bt in _Behaviors) ValidateStyle(bt);
                foreach (var ct in _Composites) ValidateStyle(ct);
                base.Repaint();
            }
        }

        private void RebuildTree()
        {
            _TreeView.Controls.Clear();
            _Behaviors.Clear();
            _Composites.Clear();

            if (BehaviorTree != null && BehaviorTree.CurrentState != null)
                RebuildTree(BehaviorTree.CurrentState, _TreeView);
        }

        private void RebuildTree(Skill.Framework.AI.Behavior behavior, Panel parent)
        {
            if (behavior == null) return;
            BaseControl control = CreateNode(behavior);
            parent.Controls.Add(control);
            switch (behavior.Type)
            {
                case Skill.Framework.AI.BehaviorType.Composite:
                    Skill.Framework.AI.Composite composite = (Skill.Framework.AI.Composite)behavior;
                    foreach (var b in composite)
                        RebuildTree(b.Behavior, (Panel)control);
                    break;
                case Skill.Framework.AI.BehaviorType.Decorator:
                    Skill.Framework.AI.Decorator decorator = (Skill.Framework.AI.Decorator)behavior;
                    if (decorator.Child != null)
                        RebuildTree(decorator.Child.Behavior, (Panel)control);
                    break;
                default:
                    break;
            }
        }


        private BaseControl CreateNode(Skill.Framework.AI.Behavior behavior)
        {
            BaseControl control = null;
            switch (behavior.Type)
            {
                case Skill.Framework.AI.BehaviorType.Action:
                case Skill.Framework.AI.BehaviorType.Condition:
                case Skill.Framework.AI.BehaviorType.ChangeState:
                    Label lbl = new Label();
                    lbl.Style = _FailurStyle;
                    lbl.Content.text = behavior.Name;
                    BehaviorTag bt = new BehaviorTag(behavior, lbl);
                    _Behaviors.Add(bt);
                    control = lbl;

                    if (behavior.Type == Framework.AI.BehaviorType.Action) lbl.Content.image = Resources.UITextures.BTree.Action;
                    else if (behavior.Type == Framework.AI.BehaviorType.ChangeState) lbl.Content.image = Resources.UITextures.BTree.ChangeState;
                    else if (behavior.Type == Framework.AI.BehaviorType.Condition) lbl.Content.image = Resources.UITextures.BTree.Condition;

                    break;
                case Skill.Framework.AI.BehaviorType.Decorator:
                case Skill.Framework.AI.BehaviorType.Composite:

                    Skill.Editor.UI.Extended.FolderView fw = new UI.Extended.FolderView();
                    fw.Foldout.Style = _FailurStyle;
                    fw.Foldout.Content.text = behavior.Name;
                    fw.Foldout.IsOpen = true;
                    CompositeTag ct = new CompositeTag(behavior, fw);
                    _Composites.Add(ct);
                    control = fw;

                    if (behavior.Type == Framework.AI.BehaviorType.Composite)
                    {
                        Skill.Framework.AI.Composite composite = (Skill.Framework.AI.Composite)behavior;
                        switch (composite.CompositeType)
                        {
                            case Skill.Framework.AI.CompositeType.Sequence:
                                fw.Foldout.Content.image = Resources.UITextures.BTree.Sequence;
                                break;
                            case Skill.Framework.AI.CompositeType.Concurrent:
                                fw.Foldout.Content.image = Resources.UITextures.BTree.Concurrent;
                                break;
                            case Skill.Framework.AI.CompositeType.Random:
                                fw.Foldout.Content.image = Resources.UITextures.BTree.Random;
                                break;
                            case Skill.Framework.AI.CompositeType.Priority:
                            case Skill.Framework.AI.CompositeType.State:
                                fw.Foldout.Content.image = Resources.UITextures.BTree.Priority;
                                break;
                            case Skill.Framework.AI.CompositeType.Loop:
                                fw.Foldout.Content.image = Resources.UITextures.BTree.Loop;
                                break;
                        }

                    }
                    else if (behavior.Type == Framework.AI.BehaviorType.Decorator)
                    {
                        Skill.Framework.AI.Decorator decorator = (Skill.Framework.AI.Decorator)behavior;
                        switch (decorator.DecoratorType)
                        {
                            case Skill.Framework.AI.DecoratorType.Default:
                                fw.Foldout.Content.image = Resources.UITextures.BTree.Decorator;
                                break;
                            case Skill.Framework.AI.DecoratorType.AccessLimit:
                                fw.Foldout.Content.image = Resources.UITextures.BTree.AccessLimitDecorator;
                                break;
                        }
                    }
                    break;
            }
            return control;
        }

        void OnGUI()
        {
            _Frame.Update();
            _Frame.OnGUI();
        }


        [SerializeField]
        private int _EdittingControllerId;

        private void FindController()
        {
            if (_EdittingControllerId != 0)
            {
                var obj = EditorUtility.InstanceIDToObject(_EdittingControllerId);
                if (obj != null)
                {
                    if (obj is GameObject)
                    {
                        _ControllerField.Object = ((GameObject)obj).GetComponent<Skill.Framework.Controller>();
                        return;
                    }
                }
            }
            _ControllerField.Object = null;
        }
    }
}
