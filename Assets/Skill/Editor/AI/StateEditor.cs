using UnityEngine;
using System.Collections;
using System.Linq;
using Skill.Framework.UI;

namespace Skill.Editor.AI
{
    public class StateEditor : Grid
    {
        private Label _StateLabel;
        private ListBox _StateList;
        private UniformGrid _StateToolbar;
        private Button _BtnAddState;
        private Button _BtnRemoveState;
        private Button _BtnSetAsDefaultState;

        private BehaviorTreeEditorWindow _Editor;
        private GUIStyle _DefaultStateStyle;
        private GUIStyle _NormalStateStyle;


        public event System.EventHandler SelectedStateChanged;

        private void OnSelectedStateChanged()
        {
            if (SelectedStateChanged != null)
                SelectedStateChanged(this, System.EventArgs.Empty);
        }
        public BehaviorTreeStateData SelectedState
        {
            get
            {
                if (_StateList.SelectedItem != null)
                    return ((StateItem)_StateList.SelectedItem).State;
                else
                    return null;
            }
        }
        public StateEditor(BehaviorTreeEditorWindow editor)
        {
            this._Editor = editor;

            this.RowDefinitions.Add(20, GridUnitType.Pixel);
            this.RowDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(90, GridUnitType.Pixel);

            _StateLabel = new Label() { Row = 0, Column = 0, Text = "States" };
            this.Controls.Add(_StateLabel);

            _StateList = new ListBox() { Row = 1, Column = 0, ColumnSpan = 2 };
            _StateList.DisableFocusable();
            this.Controls.Add(_StateList);

            _StateToolbar = new UniformGrid() { Row = 0, Column = 1, Rows = 1, Columns = 3 };
            this.Controls.Add(_StateToolbar);

            _BtnAddState = new Button() { Column = 1 };
            _BtnAddState.Content.tooltip = "add new state";
            _StateToolbar.Controls.Add(_BtnAddState);

            _BtnRemoveState = new Button() { Column = 2 };
            _BtnRemoveState.Content.tooltip = "remove selected state";
            _StateToolbar.Controls.Add(_BtnRemoveState);

            _BtnSetAsDefaultState = new Button() { Column = 0 };
            _BtnSetAsDefaultState.Content.tooltip = "make selected state as default state";
            _StateToolbar.Controls.Add(_BtnSetAsDefaultState);

            SetButtonsEnable();

            _StateList.SelectionChanged += _StateList_SelectionChanged;
            _BtnAddState.Click += _BtnAddState_Click;
            _BtnRemoveState.Click += _BtnRemoveState_Click;
            _BtnSetAsDefaultState.Click += _BtnSetAsDefaultState_Click;
        }
        void _BtnSetAsDefaultState_Click(object sender, System.EventArgs e)
        {
            SetAsDefaultState();
        }
        void _BtnRemoveState_Click(object sender, System.EventArgs e)
        {
            RemoveSelectedState();
        }
        void _BtnAddState_Click(object sender, System.EventArgs e)
        {
            AddNewState();
        }
        void _StateList_SelectionChanged(object sender, System.EventArgs e)
        {
            OnSelectedStateChanged();
            SetButtonsEnable();
            Skill.Editor.UI.InspectorProperties.Select((StateItem)_StateList.SelectedItem);
            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
        }
        private void SetButtonsEnable()
        {
            _BtnAddState.IsEnabled = true;
            _BtnRemoveState.IsEnabled = _StateList.SelectedItem != null && _StateList.Items.Count > 1;
            _BtnSetAsDefaultState.IsEnabled = _StateList.SelectedItem != null;
        }
        internal void Clear()
        {
            _StateList.Items.Clear();
        }
        public void RefreshStyles()
        {
            _StateLabel.Style = Skill.Editor.Resources.Styles.Header;

            _BtnAddState.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnRemoveState.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnSetAsDefaultState.Style = Skill.Editor.Resources.Styles.ToolbarButton;

            _BtnAddState.Content.image = Skill.Editor.Resources.UITextures.Add;
            _BtnRemoveState.Content.image = Skill.Editor.Resources.UITextures.Remove;
            _BtnSetAsDefaultState.Content.image = Skill.Editor.Resources.UITextures.Default;
            _StateList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;

            _DefaultStateStyle = new GUIStyle(UnityEditor.EditorStyles.label);
            _DefaultStateStyle.fontStyle = FontStyle.Bold;
            _DefaultStateStyle.normal.textColor = Color.green;
            _NormalStateStyle = new GUIStyle(UnityEditor.EditorStyles.label);
            RefreshItemStyles();
        }
        public void Rebuild()
        {
            if (_Editor.BehaviorTree.States == null || _Editor.BehaviorTree.States.Length < 1)
            {
                _Editor.BehaviorTree.States = new BehaviorData[1];
                _Editor.BehaviorTree.States[0] = new BehaviorTreeStateData() { };
                _Editor.BehaviorTree.States[0].Name = "Default";
                _Editor.BehaviorTree.DefaultState = _Editor.BehaviorTree.States[0].Name;
            }

            if (string.IsNullOrEmpty(_Editor.BehaviorTree.DefaultState))
                _Editor.BehaviorTree.DefaultState = _Editor.BehaviorTree.States[0].Name;

            bool foundDefaultState = false;
            foreach (BehaviorTreeStateData state in _Editor.BehaviorTree.States)
            {
                if (state.Name == _Editor.BehaviorTree.DefaultState)
                {
                    foundDefaultState = true;
                    break;
                }
            }

            if (!foundDefaultState)
                _Editor.BehaviorTree.DefaultState = _Editor.BehaviorTree.States[0].Name;

            _StateList.Items.Clear();
            foreach (var item in _Editor.BehaviorTree.States)
            {
                BehaviorTreeStateData state = (BehaviorTreeStateData)item;
                Add(state);
            }

            //SelectDefaultState();
        }
        private void Add(BehaviorTreeStateData state)
        {
            StateItem item = new StateItem(this, state);
            _Editor.AddToList(state);
            SetStyle(item);
            _StateList.Items.Add(item);
        }
        private void RefreshItemStyles()
        {
            foreach (StateItem item in _StateList.Items)
                SetStyle(item);
        }
        private void SetStyle(StateItem item)
        {
            if (item.State.Name == _Editor.BehaviorTree.DefaultState)
                item.Style = _DefaultStateStyle;
            else
                item.Style = _NormalStateStyle;
        }
        private bool IsStateExists(BehaviorTreeStateData state)
        {
            foreach (StateItem item in _StateList.Items)
            {
                if (item.State == state)
                    return true;
            }
            return false;
        }
        public void SelectDefaultState()
        {
            bool found = false;
            foreach (StateItem item in _StateList.Items)
            {
                if (item.State.Name == _Editor.BehaviorTree.DefaultState)
                {
                    _StateList.SelectedItem = item;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                if (_StateList.Items.Count > 0)
                {
                    BehaviorTreeStateData state = ((StateItem)_StateList.Items[0]).State;
                    _Editor.BehaviorTree.DefaultState = state.Name;
                    _StateList.SelectedIndex = 0;
                    SetStyle(_StateList.SelectedItem as StateItem);
                }
            }
        }
        private void SetAsDefaultState()
        {
            if (_StateList.SelectedItem != null)
            {
                BehaviorTreeStateData state = ((StateItem)_StateList.SelectedItem).State;
                _Editor.BehaviorTree.DefaultState = state.Name;
                RefreshItemStyles();
            }
            else
            {
                Debug.LogError("there is no selected state to set as default");
            }
        }
        private void RemoveSelectedState()
        {
            if (_StateList.SelectedItem != null)
            {
                if (_StateList.Items.Count > 1)
                {
                    BehaviorTreeStateData state = ((StateItem)_StateList.SelectedItem).State;
                    BehaviorData[] preStates = _Editor.BehaviorTree.States;
                    BehaviorData[] newStates = new BehaviorData[preStates.Length - 1];

                    int preIndex = 0;
                    int newIndex = 0;
                    while (newIndex < newStates.Length && preIndex < preStates.Length)
                    {
                        if (preStates[preIndex] == state)
                        {
                            preIndex++;
                            continue;
                        }
                        newStates[newIndex] = preStates[preIndex];
                        newIndex++;
                        preIndex++;
                    }
                    _Editor.BehaviorTree.States = newStates;
                    _StateList.Items.Remove(_StateList.SelectedItem);
                    SelectDefaultState();
                    RefreshItemStyles();
                    SetButtonsEnable();
                }
                else
                {
                    Debug.LogWarning("can not delete last state");
                }
            }
            else
            {
                Debug.LogError("there is no selected state to remove");
            }
        }
        private void AddNewState()
        {
            BehaviorTreeStateData state = new BehaviorTreeStateData();
            state.Name = _Editor.GetUniqueName("NewState");
            BehaviorData[] preStates = _Editor.BehaviorTree.States;
            BehaviorData[] newStates = new BehaviorData[preStates.Length + 1];
            preStates.CopyTo(newStates, 0);
            newStates[newStates.Length - 1] = state;
            _Editor.BehaviorTree.States = newStates;
            Add(state);

            SetButtonsEnable();
        }

        public void DeselectInspector()
        {
            if (Skill.Editor.UI.InspectorProperties.GetSelected() is StateItem)
                Skill.Editor.UI.InspectorProperties.Select(null);
        }

        public void RefreshContents()
        {
            foreach (StateItem item in _StateList.Items)
            {
                item.RefreshContent();
            }
        }


        private class StateItem : Label, Skill.Editor.UI.IProperties
        {
            private StateEditor _Owner;
            public BehaviorTreeStateData State { get; private set; }
            public StateItem(StateEditor owner, BehaviorTreeStateData state)
            {
                this._Owner = owner;
                this.State = state;
                this.Text = State.Name;
            }

            [Skill.Framework.ExposeProperty(1, "Name", "name of state")]
            public string Name2
            {
                get
                {
                    return State.Name;
                }
                set
                {
                    if (string.IsNullOrEmpty(value)) return;
                    this._Owner._Editor.ChangeStateNameTo(State.Name, value);
                    State.Name = value;
                    this.Text = value;
                    this._Owner._Editor.RefreshContents();
                }
            }

            public bool IsSelectedProperties { get; set; }
            private StateItemProperties _Properties;
            public Skill.Editor.UI.PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new StateItemProperties(this);
                    return _Properties;
                }
            }
            public string Title { get { return "BehaviorTreeState"; } }

            class StateItemProperties : Skill.Editor.UI.ExposeProperties
            {
                private StateItem _Item;
                public StateItemProperties(StateItem item)
                    : base(item)
                {
                    _Item = item;
                }

                protected override void SetDirty()
                {
                    Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(_Item);
                }
            }

            internal void RefreshContent()
            {
                this.Text = State.Name;
            }
        }
    }
}
