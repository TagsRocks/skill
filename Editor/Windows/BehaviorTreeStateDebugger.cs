using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Skill.Editor.UI;
using Skill.Framework.UI;

namespace Skill.Editor
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
            hideFlags = HideFlags.DontSave;
            if (_Instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton. Self destruction in 3...");
                Destroy(this);
                return;
            }

            _Instance = this;

            title = "BehaviorTree Debugger";
            position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = Size;
            CreateGUI();
        }

        private EditorFrame _Frame;
        private ObjectField<Skill.Framework.Controller> _ControllerField;
        private Box _ConditionCaption, _DecoratorCaption, _ActionCaption;
        private ScrollView _ConditionScrollView, _DecoratorScrollView, _ActionScrollView;
        private WrapPanel _ConditionPanel, _DecoratorPanel, _ActionPanel;
        private StackPanel _InfoPanel;
        private Label _SuccessState, _FailurState, _RunningState;

        private GUIStyle _SuccessStyle, _FailurStyle, _RunningStyle;

        void CreateGUI()
        {
            _SuccessStyle = new GUIStyle() { normal = new GUIStyleState() { background = Resources.Success } };
            _FailurStyle = new GUIStyle() { normal = new GUIStyleState() { background = Resources.Failure } };
            _RunningStyle = new GUIStyle() { normal = new GUIStyleState() { background = Resources.Running } };

            _Frame = new EditorFrame("Frame", this);

            _Frame.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            _Frame.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            _Frame.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(22, GridUnitType.Pixel) }); // for _ControllerField
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) }); // for captions
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }); // for panels and scrollviews
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) }); // for infos

            _ControllerField = new ObjectField<Skill.Framework.Controller>() { Row = 0, Column = 0, ColumnSpan = 3, VerticalAlignment = VerticalAlignment.Center, Height = 18 };
            _ControllerField.ObjectChanged += new EventHandler(_ControllerField_ObjectChanged);

            _ConditionCaption = new Box() { Row = 1, Column = 0 };
            _ConditionCaption.Content.text = "Conditions";
            _ConditionCaption.Content.image = Resources.Condition;
            _DecoratorCaption = new Box() { Row = 1, Column = 1 };
            _DecoratorCaption.Content.text = "Decorators";
            _DecoratorCaption.Content.image = Resources.Decorator;
            _ActionCaption = new Box() { Row = 1, Column = 2 };
            _ActionCaption.Content.text = "Actions";
            _ActionCaption.Content.image = Resources.Action;

            _ConditionPanel = new WrapPanel() { Orientation = Orientation.Horizontal };
            _DecoratorPanel = new WrapPanel() { Orientation = Orientation.Horizontal };
            _ActionPanel = new WrapPanel() { Orientation = Orientation.Horizontal };

            _ConditionScrollView = new ScrollView() { Row = 2, Column = 0 };
            _ConditionScrollView.Controls.Add(_ConditionPanel);
            _ConditionScrollView.RenderAreaChanged += new EventHandler(_ConditionScrollView_RenderAreaChanged);

            _DecoratorScrollView = new ScrollView() { Row = 2, Column = 1 };
            _DecoratorScrollView.Controls.Add(_DecoratorPanel);
            _DecoratorScrollView.RenderAreaChanged += new EventHandler(_DecoratorScrollView_RenderAreaChanged);

            _ActionScrollView = new ScrollView() { Row = 2, Column = 2 };
            _ActionScrollView.Controls.Add(_ActionPanel);
            _ActionScrollView.RenderAreaChanged += new EventHandler(_ActionScrollView_RenderAreaChanged);


            _SuccessState = new Label() { Margin = new Thickness(2, 2, 0, 2), Width = 100 };
            _SuccessState.Content.text = "Success";
            _SuccessState.Content.image = Resources.Success;
            _RunningState = new Label() { Margin = new Thickness(2, 2, 0, 2), Width = 100 };
            _RunningState.Content.text = "Running";
            _RunningState.Content.image = Resources.Running;
            _FailurState = new Label() { Margin = new Thickness(2, 2, 0, 2), Width = 100 };
            _FailurState.Content.text = "Failure";
            _FailurState.Content.image = Resources.Failure;

            _InfoPanel = new StackPanel() { Row = 3, Column = 0, ColumnSpan = 3, Orientation = Orientation.Horizontal };
            _InfoPanel.Controls.Add(_SuccessState);
            _InfoPanel.Controls.Add(_RunningState);
            _InfoPanel.Controls.Add(_FailurState);


            _Frame.Grid.Controls.Add(_ControllerField);

            _Frame.Grid.Controls.Add(_ConditionCaption);
            _Frame.Grid.Controls.Add(_DecoratorCaption);
            _Frame.Grid.Controls.Add(_ActionCaption);

            _Frame.Grid.Controls.Add(_ConditionScrollView);
            _Frame.Grid.Controls.Add(_DecoratorScrollView);
            _Frame.Grid.Controls.Add(_ActionScrollView);

            _Frame.Grid.Controls.Add(_InfoPanel);
        }

        void _ControllerField_ObjectChanged(object sender, EventArgs e)
        {
            if (_ControllerField.Object != null)
            {
                BehaviorTree = _ControllerField.Object.Behavior;
            }
        }

        void _ActionScrollView_RenderAreaChanged(object sender, EventArgs e)
        {
            _ActionPanel.Width = _ActionScrollView.RenderArea.width - _ActionScrollView.Padding.Horizontal;
        }

        void _DecoratorScrollView_RenderAreaChanged(object sender, EventArgs e)
        {
            _DecoratorPanel.Width = _DecoratorScrollView.RenderArea.width - _DecoratorScrollView.Padding.Horizontal;
        }

        void _ConditionScrollView_RenderAreaChanged(object sender, EventArgs e)
        {
            _ConditionPanel.Width = _ConditionScrollView.RenderArea.width - _ConditionScrollView.Padding.Horizontal;
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

        private List<BehaviorTag> _Actions = new List<BehaviorTag>();
        private List<BehaviorTag> _Conditions = new List<BehaviorTag>();
        private List<BehaviorTag> _Decorators = new List<BehaviorTag>();

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
                    _BehaviorTree.Updated += _BehaviorTree_Updated;

                RebuildTree();
            }
        }

        private bool IsInExecutionSequence(Skill.Framework.AI.Behavior behavior)
        {
            if (_BehaviorTree == null) return false;
            for (int i = 0; i < _BehaviorTree.State.SequenceCount; i++)
            {
                if (_BehaviorTree.State.ExecutionSequence[i] == behavior)
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

        void _BehaviorTree_Updated(object sender, EventArgs e)
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                foreach (var bt in _Conditions) ValidateStyle(bt);
                foreach (var bt in _Actions) ValidateStyle(bt);
                foreach (var bt in _Decorators) ValidateStyle(bt);

                base.Repaint();
            }
        }

        private void RebuildTree()
        {
            _ActionPanel.Controls.Clear();
            _DecoratorPanel.Controls.Clear();
            _ConditionPanel.Controls.Clear();

            _Actions.Clear();
            _Decorators.Clear();
            _Conditions.Clear();

            if (BehaviorTree != null)
            {
                RebuildTree(BehaviorTree.Root);

                foreach (var bt in _Conditions) _ConditionPanel.Controls.Add(bt.Label);
                foreach (var bt in _Actions) _ActionPanel.Controls.Add(bt.Label);
                foreach (var bt in _Decorators) _DecoratorPanel.Controls.Add(bt.Label);

                Debug.Log(string.Format("Build Tree, Actions={0}, Conditions={1}, Decorators={2}", _Actions.Count, _Conditions.Count, _Decorators.Count));
            }
            else
                Debug.LogWarning("No BehaviorTree selected");
        }

        private void AddToList(List<BehaviorTag> list, Skill.Framework.AI.Behavior behavior)
        {
            bool exist = false;
            foreach (var bt in list)
            {
                if (bt.Behavior == behavior)
                {
                    exist = true;
                }
            }
            if (!exist)
            {
                Label lbl = new Label() { Margin = new Thickness(2, 2, 0, 0) };
                lbl.Style = _FailurStyle;
                lbl.Content.text = behavior.Name;
                BehaviorTag bt = new BehaviorTag(behavior, lbl);
                list.Add(bt);
            }
        }

        private void RebuildTree(Skill.Framework.AI.Behavior behavior)
        {
            if (behavior == null) return;
            switch (behavior.Type)
            {
                case Skill.Framework.AI.BehaviorType.Composite:
                    Skill.Framework.AI.Composite composite = (Skill.Framework.AI.Composite)behavior;
                    foreach (var b in composite)
                        RebuildTree(b.Behavior);
                    break;
                case Skill.Framework.AI.BehaviorType.Condition:
                    AddToList(_Conditions, behavior);
                    break;
                case Skill.Framework.AI.BehaviorType.Decorator:
                    AddToList(_Decorators, behavior);
                    Skill.Framework.AI.Decorator decorator = (Skill.Framework.AI.Decorator)behavior;
                    if (decorator.Child != null)
                        RebuildTree(decorator.Child.Behavior);
                    break;
                case Skill.Framework.AI.BehaviorType.Action:
                    AddToList(_Actions, behavior);
                    break;
                default:
                    break;
            }
        }

        void OnGUI()
        {
            _Frame.OnGUI();
        }
    }
}
