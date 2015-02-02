using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework.UI;
using Skill.Framework.UI.Extended;

namespace Skill.Editor.AI
{
    public class BehaviorTreeEditorWindow : EditorWindow
    {
        #region BehaviorTreeDataEditorWindow

        private static BehaviorTreeEditorWindow _Instance;
        public static BehaviorTreeEditorWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = ScriptableObject.CreateInstance<BehaviorTreeEditorWindow>();
                return _Instance;
            }
        }

        private static Vector2 Size = new Vector2(800, 600);
        private static Vector2 MinSize = new Vector2(500, 300);

        public BehaviorTreeEditorWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.title = "BehaviorTree";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = MinSize;
            CreateUI();
        }

        void OnFocus()
        {
            if (_Frame != null)
            {
                _RefreshStyles = true;
            }
        }

        void OnLostFocus()
        {
            Save();
        }

        void OnDestroy()
        {
            if (_States != null)
                _States.DeselectInspector();
            if (_TreeViewEditor != null)
                _TreeViewEditor.DeselectInspector();
            if (_BehaviorList != null)
                _BehaviorList.DeselectInspector();
        }

        void OnEnable()
        {
            if (_Asset != null)
            {
                var temp = _Asset;
                _Asset = null;
                Asset = temp;
            }
        }

        #endregion

        #region UI
        private bool _RefreshStyles = true;
        private Skill.Editor.UI.EditorFrame _Frame;

        private Skill.Framework.UI.Grid _EditPanel;
        private Skill.Framework.UI.Toolbar _EditModeToolbar;
        private Skill.Framework.UI.ToolbarButton _BtnTreeView;
        private Skill.Framework.UI.ToolbarButton _BtnListView;
        private Skill.Editor.UI.GridSplitter _VSplitter;
        private BehaviorList _BehaviorList;
        private TreeViewEditor _TreeViewEditor;
        private StateEditor _States;

        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
            _Frame.Grid.RowDefinitions.Add(20, GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star);

            _EditPanel = new Grid() { Row = 0, RowSpan = 2 };
            _EditPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(220, GridUnitType.Pixel), MinWidth = 220 }); // left Panel        
            _EditPanel.ColumnDefinitions.Add(2, GridUnitType.Pixel); // VSpliter
            _EditPanel.ColumnDefinitions.Add(2, GridUnitType.Star); // right Panel        
            _Frame.Controls.Add(_EditPanel);

            _VSplitter = new Skill.Editor.UI.GridSplitter() { Row = 1, Column = 1, Orientation = Orientation.Vertical };
            _EditPanel.Controls.Add(_VSplitter);

            _TreeViewEditor = new TreeViewEditor(this) { Row = 0, Column = 2 };
            _EditPanel.Controls.Add(_TreeViewEditor);

            _States = new StateEditor(this) { Row = 0, Column = 0 };
            _EditPanel.Controls.Add(_States);

            _BehaviorList = new BehaviorList(this) { Row = 0, RowSpan = 2, Visibility = Visibility.Hidden };
            _Frame.Controls.Add(_BehaviorList);

            _EditModeToolbar = new Toolbar() { Row = 0, HorizontalAlignment = HorizontalAlignment.Right, Width = 100 };
            _Frame.Controls.Add(_EditModeToolbar);

            _BtnTreeView = new ToolbarButton();
            _BtnTreeView.Content.text = "Edit";
            _EditModeToolbar.Items.Add(_BtnTreeView);

            _BtnListView = new ToolbarButton();
            _BtnListView.Content.text = "View";
            _EditModeToolbar.Items.Add(_BtnListView);

            _BtnTreeView.Selected += _BtnTreeView_Selected;
            _BtnListView.Selected += _BtnListView_Selected;

            _EditModeToolbar.SelectedIndex = 0;

            _States.SelectedStateChanged += _States_SelectedStateChanged;
        }

        void _States_SelectedStateChanged(object sender, System.EventArgs e)
        {
            _TreeViewEditor.State = _States.SelectedState;
        }

        void _BtnListView_Selected(object sender, System.EventArgs e)
        {
            _EditPanel.Visibility = Visibility.Hidden;
            _BehaviorList.Visibility = Visibility.Visible;
            _States.DeselectInspector();
            _TreeViewEditor.DeselectInspector();
            _BehaviorList.DeselectInspector();
            _BehaviorList.Rebuild();
        }

        void _BtnTreeView_Selected(object sender, System.EventArgs e)
        {

            if (_BehaviorTree != null)
                _BehaviorTree.MatchParameters();
            _TreeViewEditor.RefreshContents();
            _EditPanel.Visibility = Visibility.Visible;
            _BehaviorList.Visibility = Visibility.Hidden;
        }

        void OnGUI()
        {
            if (_Frame != null)
            {
                RefreshStyles();
                _Frame.Update();
                _Frame.OnGUI();
            }
        }
        private void RefreshStyles()
        {
            if (_RefreshStyles)
            {
                _RefreshStyles = false;
                _States.RefreshStyles();
                _TreeViewEditor.RefreshStyles();
                _BehaviorList.RefreshStyles();

                _VSplitter.Style = Skill.Editor.Resources.Styles.VerticalSplitter;
                _EditModeToolbar.Style = Skill.Editor.Resources.Styles.ToolbarButton;
                GUIStyle toolbarStyle = new GUIStyle(Skill.Editor.Resources.Styles.ToolbarButton);
                toolbarStyle.onNormal = toolbarStyle.normal;
                toolbarStyle.onHover = toolbarStyle.hover;
                toolbarStyle.onActive = toolbarStyle.active;
                _EditModeToolbar.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            }
        }

        #endregion

        [SerializeField]
        private BehaviorTreeAsset _Asset;
        private BehaviorTreeData _BehaviorTree;
        private List<BehaviorData> _Behaviors = new List<BehaviorData>();

        public BehaviorTreeAsset Asset
        {
            get { return _Asset; }
            set
            {
                if (_Asset != value)
                {
                    _Asset = value;
                    _BehaviorTree = null;
                    Rebuild();
                }
            }
        }
        public BehaviorTreeData BehaviorTree { get { return _BehaviorTree; } }

        private void Save()
        {
            if (_Asset != null && _BehaviorTree != null)
            {
                _BehaviorTree.Name = _Asset.name;
                this._BehaviorTree.ExtraBehaviors = GetExtraBehaviors();
                _Asset.Save(_BehaviorTree);
            }
        }

        private void Rebuild()
        {
            _RefreshStyles = true;
            Clear();
            if (_Asset != null)
                _BehaviorTree = _Asset.Load();

            if (_BehaviorTree != null)
            {
                _Behaviors.Clear();
                foreach (var state in _BehaviorTree.States)
                    AddToList(state);
                if (_BehaviorTree.ExtraBehaviors != null)
                {
                    foreach (var b in _BehaviorTree.ExtraBehaviors)
                        AddToList(b);
                }

                _States.Clear();
                _States.Rebuild();
            }
        }

        internal void AddToList(BehaviorData behavior)
        {
            if (!_Behaviors.Contains(behavior))
            {
                _Behaviors.Add(behavior);
                _TreeViewEditor.BehaviorsChanged();
            }
            foreach (var item in behavior)
                AddToList(item);
        }

        internal void RemoveFromList(BehaviorData behavior)
        {
            if (_Behaviors.Contains(behavior))
            {
                if (!_BehaviorTree.IsInHierarchy(behavior))
                {
                    _Behaviors.Remove(behavior);
                    _TreeViewEditor.BehaviorsChanged();
                }
                else
                {
                    Debug.LogError("can not delete behavior. this behavior is in use");
                }
            }
        }

        private void Clear()
        {
            _States.Clear();
            _Behaviors.Clear();
        }
        internal void RefreshTree()
        {
            _TreeViewEditor.RefresTree();
            Repaint();
        }
        internal void RefreshContents()
        {
            _TreeViewEditor.RefreshContents();
            _States.RefreshContents();
            Repaint();
        }

        internal void RefreshStateNames()
        {
            _States.RefreshContents();
            Repaint();
        }
        private BehaviorData[] GetExtraBehaviors()
        {
            ValidateBehaviors();
            List<BehaviorData> extra = new List<BehaviorData>();
            foreach (var b in _Behaviors)
            {
                if (b.BehaviorType == Framework.AI.BehaviorType.Composite)
                {
                    if (((CompositeData)b).CompositeType == Framework.AI.CompositeType.State)
                        continue;
                }

                if (!_BehaviorTree.IsInHierarchy(b))
                    extra.Add(b);
            }
            return extra.ToArray();
        }
        private bool HasState(string stateName)
        {
            foreach (var s in _BehaviorTree.States)
            {
                if (s.Name == stateName)
                    return true;
            }
            return false;
        }
        private ChangeStateData GetChangeState(string stateName)
        {
            if (!HasState(stateName))
                Debug.LogError(string.Format("Invalid state name '{0}'", stateName));

            foreach (var b in _Behaviors)
            {
                if (b.BehaviorType == Framework.AI.BehaviorType.ChangeState)
                {
                    ChangeStateData cs = (ChangeStateData)b;
                    if (cs.DestinationState == stateName)
                        return cs;
                }
            }

            ChangeStateData state = new ChangeStateData();
            state.DestinationState = stateName;
            state.Comment = "Change state to " + stateName;
            state.Name = "GoTo" + stateName;
            AddToList(state);
            return state;
        }

        private void ValidateBehaviors()
        {
            int index = 0;
            while (index < _Behaviors.Count)
            {
                BehaviorData behavior = _Behaviors[index];
                if (behavior.BehaviorType == Framework.AI.BehaviorType.ChangeState)
                {
                    if (!HasState(((ChangeStateData)behavior).DestinationState))
                    {
                        _Behaviors.RemoveAt(index);
                        continue;
                    }
                }
                index++;
            }

            for (int i = 0; i < _BehaviorTree.States.Length; i++)
                GetChangeState(_BehaviorTree.States[i].Name);
        }
        public BehaviorData[] GetBehaviors()
        {
            ValidateBehaviors();
            return _Behaviors.ToArray();
        }

        internal string GetUniqueName(string name)
        {
            int i = 1;
            string newName = name;
            while (_Behaviors.Where(b => b.Name == newName).Count() > 0)
                newName = name + i++;
            return newName;
        }

        internal void ChangeStateNameTo(string oldStateName, string newStateName)
        {
            foreach (var behavior in _Behaviors)
            {
                if (behavior.BehaviorType == Framework.AI.BehaviorType.ChangeState)
                {
                    ChangeStateData cs = (ChangeStateData)behavior;
                    if (cs.DestinationState == oldStateName)
                    {
                        cs.DestinationState = newStateName;
                        cs.Name = "GoTo" + newStateName;
                        break;
                    }
                }
            }
            if (_BehaviorTree.DefaultState == oldStateName)
                _BehaviorTree.DefaultState = newStateName;
        }


    }
}