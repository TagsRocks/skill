using UnityEngine;
using System.Collections;

using Skill.Framework.UI;
using System.Collections.Generic;

namespace Skill.Editor.Animation
{
    public class AnimNodeItem : Grid, Skill.Editor.UI.IProperties, Skill.Editor.UI.ISelectable
    {
        private const float TitleHeight = 30;

        private Box _Border;
        private Label _LblTitle;
        private AnimNodeItemDragThumb _Drag;
        private Skill.Editor.UI.Connector _OutputConnector;
        private StackPanel _PnlItems;
        private bool _RecalcSize = true;

        public AnimNodeData Data { get; private set; }
        public string Header
        {
            get { return _LblTitle.Text; }
            set
            {
                _LblTitle.Text = value;
                _RecalcSize = true;
            }
        }
        public override float LayoutHeight { get { return TitleHeight + Mathf.Max(20, _PnlItems.LayoutHeight); } }
        protected virtual bool HasOutput { get { return true; } }
        protected virtual Texture2D Icon { get { return Data.GetIcon(); } }

        /// <summary> Retrieves number of input connectors </summary>
        public int ConnectorCount
        {
            get
            {
                int count = 0;
                foreach (var item in _PnlItems.Controls)
                {
                    if (item is InputConnectorItem)
                        count++;
                }
                return count;
            }
        }

        public AnimNodeItem(AnimNodeData data)
        {
            this.Data = data;
            this.RowDefinitions.Add(TitleHeight, GridUnitType.Pixel);
            this.RowDefinitions.Add(1, GridUnitType.Star);

            _Border = new Box() { Row = 0, Column = 0, RowSpan = 2 };
            this.Controls.Add(_Border);

            _Drag = new AnimNodeItemDragThumb(this) { Row = 0, Column = 0 };
            this.Controls.Add(_Drag);

            _LblTitle = new Label() { Row = 0, Column = 0, Height = TitleHeight, VerticalAlignment = Framework.UI.VerticalAlignment.Center };
            this.Controls.Add(_LblTitle);

            _PnlItems = new StackPanel() { Row = 1, Column = 0, Orientation = Orientation.Vertical, Height = 20 };
            this.Controls.Add(_PnlItems);
            _RefreshSyles = true;

            if (HasOutput)
                CreateOutputConnector();

            this.Header = data.Name;
            this.X = Data.X;
            this.Y = Data.Y;

            if (Data.Inputs != null)
            {
                for (int i = 0; i < Data.Inputs.Length; i++)
                {
                    if (string.IsNullOrEmpty(Data.Inputs[i].Name))
                        Data.Inputs[i].Name = "Input";
                    AddInputConnector(Data.Inputs[i].Name);
                }
            }

            this.ContextMenu = AnimNodeItemContextMenu.Instance;
        }
        private bool _RefreshSyles;
        protected override void BeginRender()
        {
            if (_RecalcSize)
            {
                float w = 0;
                if (!string.IsNullOrEmpty(_LblTitle.Text))
                {
                    _LblTitle.Content.image = Icon;
                    w = 20 + UnityEditor.EditorStyles.label.CalcSize(_LblTitle.Content).x; 
                }

                w = Mathf.Max(50, w);
                this.Width = w;

                _RecalcSize = false;
            }
            if (_RefreshSyles)
            {
                _RefreshSyles = false;
                if (_OutputConnector != null)
                    _OutputConnector.Style = Skill.Editor.Resources.Styles.Connector;
            }
            base.BeginRender();
        }
        private void CreateOutputConnector()
        {
            if (_OutputConnector != null) return;
            _OutputConnector = new UI.Connector(UI.ConnectorType.Output)
            {
                Row = 1,
                Column = 0,
                VerticalAlignment = Framework.UI.VerticalAlignment.Center,
                HorizontalAlignment = Framework.UI.HorizontalAlignment.Left,
                Width = 20,
                Height = 12,
                Margin = new Thickness(-20, 0, 0, 0),
                UserData = this,
                SingleConnection = true,
                Name = "Connection"
            };
            this.Controls.Add(_OutputConnector);
        }
        protected void AddInputConnector(string name)
        {
            InputConnectorItem ci = new InputConnectorItem(this) { Header = name };
            _PnlItems.Controls.Add(ci);
        }
        protected Label AddLabel()
        {
            Label lbl = new Label() { Height = 20 };
            _PnlItems.Controls.Add(lbl);
            return lbl;
        }
        public int GetConnectorIndex(Skill.Editor.UI.IConnector connector)
        {
            int index = -1;
            foreach (var item in _PnlItems.Controls)
            {
                if (item is InputConnectorItem)
                {
                    index++;
                    if (((InputConnectorItem)item).Connector == connector)
                        return index;
                }
            }

            return -1;
        }
        public Skill.Editor.UI.IConnector GetInputConnector(int connectorIndex)
        {
            var item = GetInputConnectorItem(connectorIndex);
            if (item != null)
                return item.Connector;
            return null;
        }

        private InputConnectorItem GetInputConnectorItem(int connectorIndex)
        {
            foreach (var item in _PnlItems.Controls)
            {
                if (item is InputConnectorItem)
                {
                    connectorIndex--;
                    if (connectorIndex < 0)
                        return (InputConnectorItem)item;
                }
            }
            return null;
        }

        public void RemoveInputConnector(int connectorIndex)
        {
            InputConnectorItem connector = GetInputConnectorItem(connectorIndex);
            List<Skill.Editor.UI.Connection> connections = new List<UI.Connection>();
            if (connector != null)
            {
                _PnlItems.Controls.Remove(connector);
                foreach (var c in connector.Connector)
                    connections.Add(c);
            }

            foreach (var c in connections)
                c.Break();
            connections.Clear();
        }
        public void RemoveAllConnections()
        {
            List<Skill.Editor.UI.Connection> connections = new List<UI.Connection>();
            foreach (var item in _PnlItems.Controls)
            {
                if (item is InputConnectorItem)
                {
                    foreach (var c in ((InputConnectorItem)item).Connector)
                        connections.Add(c);
                }
            }
            foreach (var c in connections)
                c.Break();
            connections.Clear();
        }
        public Skill.Editor.UI.IConnector OutConnector { get { return _OutputConnector; } }

        public string GetInputName(int index)
        {
            var item = GetInputConnectorItem(index);
            if (item != null)
                return item.Header;
            return string.Empty;
        }
        public void SetInputName(int index, string name)
        {
            var item = GetInputConnectorItem(index);
            if (item != null) item.Header = name;
        }

        public void Save()
        {
            Data.X = this.X;
            Data.Y = this.Y;

            List<InputConnectorItem> inputs = new List<InputConnectorItem>();
            foreach (var item in _PnlItems.Controls)
            {
                if (item is InputConnectorItem)
                    inputs.Add((InputConnectorItem)item);
            }

            Data.Inputs = new ConnectorData[inputs.Count];
            for (int i = 0; i < inputs.Count; i++)
                Data.Inputs[i] = new ConnectorData() { Index = i, Name = inputs[i].Header };
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (value != _IsSelected)
                {
                    _IsSelected = value;
                    if (_IsSelected)
                        _Border.Style = Skill.Editor.Resources.Styles.SelectedEventBorder;
                    else
                        _Border.Style = null;
                }
            }
        }

        #region IProperties members

        protected virtual ItemProperties CreateProperties() { return new ItemProperties(this); }

        protected class ItemProperties : UI.ExposeProperties
        {
            AnimNodeItem _Item;
            public ItemProperties(AnimNodeItem item)
                : base(item)
            {
                _Item = item;
            }

            protected override void SetDirty()
            {
                Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(_Item);
            }
        }

        private ItemProperties _Properties;
        public UI.PropertiesPanel Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = CreateProperties();
                return _Properties;
            }
        }
        public bool IsSelectedProperties { get; set; }
        public string Title { get { return Data.NodeType.ToString(); } }

        #endregion

        #region DragThum
        internal Vector2 StartDrag;
        class AnimNodeItemDragThumb : Skill.Editor.UI.DragThumb
        {
            private AnimNodeItem _Item;

            public AnimNodeItemDragThumb(AnimNodeItem item)
            {
                this._Item = item;
            }
            protected override void OnDrag(Vector2 delta)
            {
                GraphEditor editor = _Item.FindInParents<GraphEditor>();
                if (editor != null)
                    editor.ItemDrag(_Item, delta);
            }
            protected override void OnMouseDown(MouseClickEventArgs args)
            {
                if (args.Button == MouseButton.Left)
                {
                    GraphEditor editor = _Item.FindInParents<GraphEditor>();
                    if (editor != null)
                    {
                        if (args.Ctrl)
                        {
                            if (_Item.IsSelected)
                                editor.Selection.Remove(_Item);
                            else
                                editor.Selection.Add(_Item);
                        }
                        else if (args.Shift)
                        {
                            if (!_Item.IsSelected)
                                editor.Selection.Add(_Item);
                        }
                        else
                        {
                            editor.Selection.Select(_Item);
                        }
                    }
                }
                base.OnMouseDown(args);
            }
        }
        #endregion

        #region Context Menu

        class AnimNodeItemContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static AnimNodeItemContextMenu _Instance;
            public static AnimNodeItemContextMenu Instance
            {
                get
                {
                    if (_Instance == null)
                        _Instance = new AnimNodeItemContextMenu();
                    return _Instance;
                }
            }


            protected override void BeginShow()
            {
                if (Owner is Skill.Editor.Animation.AnimationTreeRootItem)
                    _DeleteItem.IsEnabled = false;
                else
                    _DeleteItem.IsEnabled = true;
                base.BeginShow();
            }

            private Skill.Editor.UI.MenuItem _DeleteItem;

            private AnimNodeItemContextMenu()
            {
                _DeleteItem = new UI.MenuItem("Delete");
                Add(_DeleteItem);
                _DeleteItem.Click += _DeleteItem_Click;
            }

            void _DeleteItem_Click(object sender, System.EventArgs e)
            {
                AnimNodeItem item = (AnimNodeItem)Owner;
                GraphEditor editor = item.FindInParents<GraphEditor>();
                if (editor != null)
                    editor.Remove(item);
            }
        }

        #endregion

        #region Expose Properties

        [Skill.Framework.ExposeProperty(10, "Name", "Name if animation node")]
        public string Name2
        {
            get { return Data.Name; }
            set
            {
                Data.Name = value;
                Header = value;
            }
        }

        [Skill.Framework.ExposeProperty(12, "Public", "Whether code generator create a public property for this node")]
        public bool IsPublic
        {
            get { return Data.IsPublic; }
            set { Data.IsPublic = value; }
        }

        [Skill.Framework.ExposeProperty(13, "Became Relevant", "If true code generator create an method and hook it to BecameRelevant event")]
        public bool BecameRelevant
        {
            get { return Data.BecameRelevant; }
            set { Data.BecameRelevant = value; }
        }


        [Skill.Framework.ExposeProperty(14, "Cease Relevant", "If true code generator create an method and hook it to CeaseRelevant event")]
        public bool CeaseRelevant
        {
            get { return Data.CeaseRelevant; }
            set { Data.CeaseRelevant = value; }
        }


        [Skill.Framework.ExposeProperty(30, "Comment", "User comment for this Animation Node")]
        public string Comment
        {
            get { return Data.Comment; }
            set { Data.Comment = value; }
        }



        #endregion

        #region InputConnectorItem
        protected class InputConnectorItem : Grid
        {
            private static GUIStyle _TitleStyle;
            private static GUIStyle TitleStyle
            {
                get
                {
                    if (_TitleStyle == null)
                    {
                        _TitleStyle = new GUIStyle(UnityEditor.EditorStyles.label);
                        _TitleStyle.alignment = TextAnchor.MiddleRight;
                    }
                    return _TitleStyle;
                }
            }

            private Label _LblHeader;
            private Skill.Editor.UI.Connector _Connector;
            public string Header { get { return _LblHeader.Text; } set { _LblHeader.Text = value; } }
            public Skill.Editor.UI.IConnector Connector { get { return _Connector; } }
            public AnimNodeItem Node { get; private set; }
            public InputConnectorItem(AnimNodeItem node)
            {
                this.Node = node;
                this.Height = 20;
                _LblHeader = new Label();
                this.Controls.Add(_LblHeader);

                _Connector = new UI.Connector(UI.ConnectorType.Input)
                {
                    VerticalAlignment = Framework.UI.VerticalAlignment.Center,
                    HorizontalAlignment = Framework.UI.HorizontalAlignment.Right,
                    Width = 20,
                    Height = 12,
                    Margin = new Thickness(0, 0, -20, 0),
                    UserData = this.Node,
                    SingleConnection = true
                };
                this.Controls.Add(_Connector);

                _RefreshSyles = true;
            }

            private bool _RefreshSyles;
            protected override void BeginRender()
            {
                if (_RefreshSyles)
                {
                    _RefreshSyles = false;
                    _Connector.Style = Skill.Editor.Resources.Styles.Connector;
                    _LblHeader.Style = TitleStyle;
                }
                base.BeginRender();
            }
        }
        #endregion

        #region InputConnectorManager
        protected abstract class InputConnectorManager : Skill.Framework.UI.Grid
        {
            protected abstract Skill.Framework.UI.Grid CreateHeader();
            protected abstract InputItem CreateItem();
            public int ConnectorCount { get { return _LbItems.Items.Count; } }
            public AnimNodeItem Item { get { return _Item; } }


            public override float LayoutHeight { get { return 18 + _LbItems.LayoutHeight + 20 + 4; } }


            private AnimNodeItem _Item;
            private Skill.Framework.UI.ListBox _LbItems;
            private Skill.Framework.UI.Grid _PnlButtons;
            private Skill.Framework.UI.Button _BtnAdd;
            private Skill.Framework.UI.Button _BtnRemove;
            public InputConnectorManager(AnimNodeItem item)
            {
                this._Item = item;
                this.RowDefinitions.Add(18, GridUnitType.Pixel); // header
                this.RowDefinitions.Add(1, GridUnitType.Star); // items
                this.RowDefinitions.Add(20, GridUnitType.Pixel); // buttons
                this.Controls.Add(CreateHeader());

                _LbItems = new Framework.UI.ListBox() { Row = 1 };
                _LbItems.DisableFocusable();
                _LbItems.BackgroundVisible = true;
                _LbItems.Background.Style = (GUIStyle)"RL Background";
                this.Controls.Add(_LbItems);

                _PnlButtons = new Grid() { Row = 2 };
                _PnlButtons.ColumnDefinitions.Add(1, GridUnitType.Star);
                _PnlButtons.ColumnDefinitions.Add(20, GridUnitType.Pixel);// btn add
                _PnlButtons.ColumnDefinitions.Add(20, GridUnitType.Pixel); // btn remove
                this.Controls.Add(_PnlButtons);

                _BtnAdd = new Button() { Column = 1 };
                _PnlButtons.Controls.Add(_BtnAdd);
                _BtnRemove = new Button() { Column = 2, IsEnabled = false };
                _PnlButtons.Controls.Add(_BtnRemove);

                _LbItems.SelectionChanged += _LbItems_SelectionChanged;
                _BtnAdd.Click += _BtnAdd_Click;
                _BtnRemove.Click += _BtnRemove_Click;

                for (int i = 0; i < _Item.ConnectorCount; i++)
                {
                    var listItem = CreateItem();
                    listItem.Index = i;
                    _LbItems.Items.Add(listItem);
                }
            }

            void _LbItems_SelectionChanged(object sender, System.EventArgs e)
            {
                _BtnRemove.IsEnabled = _LbItems.SelectedItem != null && _LbItems.Items.Count > 2;
            }

            void _BtnRemove_Click(object sender, System.EventArgs e)
            {
                if (_LbItems.SelectedItem != null)
                {
                    int index = _LbItems.SelectedIndex;
                    _Item.RemoveInputConnector(_LbItems.SelectedIndex);
                    _LbItems.Items.Remove(_LbItems.SelectedItem);
                    _LbItems.SelectedItem = null;

                    OnConnectorRemove(index);
                    RebuildIndices();

                }
            }

            void _BtnAdd_Click(object sender, System.EventArgs e)
            {
                _Item.AddInputConnector("Input");
                _LbItems.Items.Add(CreateItem());
                OnConnectorAdd();
                RebuildIndices();
            }

            private void RebuildIndices()
            {
                for (int i = 0; i < _LbItems.Items.Count; i++)
                    ((InputItem)_LbItems.Items[i]).Index = i;
            }

            protected virtual void OnConnectorRemove(int index) { }
            protected virtual void OnConnectorAdd() { }

            private bool _RefreshStyles;
            protected override void BeginRender()
            {
                if (!_RefreshStyles)
                {
                    _BtnAdd.Style = Skill.Editor.Resources.Styles.SmallButton;
                    _BtnRemove.Style = Skill.Editor.Resources.Styles.SmallButton;
                    _BtnAdd.Content.image = Skill.Editor.Resources.UITextures.Plus;
                    _BtnRemove.Content.image = Skill.Editor.Resources.UITextures.Minus;
                    _LbItems.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
                    _RefreshStyles = true;
                }
                base.BeginRender();
            }


            protected abstract class InputItem : Skill.Framework.UI.Grid
            {
                private AnimNodeItem _Item;
                public AnimNodeItem Item { get { return _Item; } }

                private int _Index;
                public int Index
                {
                    get { return _Index; }
                    set
                    {
                        _Index = value;
                        RefreshValue();
                    }
                }

                protected abstract void RefreshValue();

                public InputItem(AnimNodeItem item)
                {
                    this._Item = item;
                }
            }
        }
        #endregion

        #region ParameterSelector
        protected abstract class ParameterSelector : Grid
        {
            private Label _Label;
            private Skill.Editor.UI.IntPopup[] _Params;
            private AnimationTreeParameterType _ParamType;

            public AnimNodeItem Item { get; private set; }

            public ParameterSelector(AnimNodeItem item, AnimationTreeParameterType type, int parameterCount)
            {
                this.Height = 20;
                this.Item = item;
                this.ColumnDefinitions.Add(1, GridUnitType.Star);
                for (int i = 0; i < parameterCount; i++)
                    this.ColumnDefinitions.Add(1, GridUnitType.Star);

                this._Label = new Label() { Column = 0, VerticalAlignment = Framework.UI.VerticalAlignment.Center };
                this._Label.Text = (parameterCount > 1) ? "Parameters" : "Parameter";
                this.Controls.Add(this._Label);

                this._ParamType = type;
                this._Params = new UI.IntPopup[parameterCount];
                for (int i = 0; i < parameterCount; i++)
                {
                    this._Params[i] = new UI.IntPopup() { Column = i + 1, UserData = i, VerticalAlignment = Framework.UI.VerticalAlignment.Center };
                    this._Params[i].OptionChanged += ParameterSelector_OptionChanged;
                    this.Controls.Add(this._Params[i]);
                }
            }

            void ParameterSelector_OptionChanged(object sender, System.EventArgs e)
            {
                UI.IntPopup ip = (UI.IntPopup)sender;
                SetParameter((int)ip.UserData, ip.SelectedOption.Content.text);
            }

            protected abstract void SetParameter(int parameterIndex, string value);
            protected abstract string GetParameter(int parameterIndex);

            public void Rebuild(AnimationTreeParameter[] source)
            {
                for (int i = 0; i < _Params.Length; i++)
                {
                    string parameter = GetParameter(i);
                    _Params[i].Options.Clear();
                    int value = 0;
                    Skill.Editor.UI.PopupOption selectedOP = null;

                    if (source != null)
                    {
                        foreach (var p in source)
                        {
                            if (p.Type == _ParamType)
                            {
                                Skill.Editor.UI.PopupOption op = new UI.PopupOption(value++, p.Name);
                                this._Params[i].Options.Add(op);
                                if (p.Name == parameter)
                                    selectedOP = op;
                            }
                        }
                    }
                    this._Params[i].SelectedOption = selectedOP;
                }
            }
        }
        #endregion

        public virtual void ChangeParameterName(string oldParamName, string newParamName) { }
    }


    public class AnimNodeBlendBaseItem : AnimNodeItem
    {
        public AnimNodeBlendBaseItem(AnimNodeBlendBaseData data)
            : base(data)
        {

        }


        [Skill.Framework.ExposeProperty(15, "New Layer", "place childs in new layer?")]
        public bool NewLayer
        {
            get { return ((AnimNodeBlendBaseData)Data).NewLayer; }
            set { ((AnimNodeBlendBaseData)Data).NewLayer = value; }
        }        
    }
}