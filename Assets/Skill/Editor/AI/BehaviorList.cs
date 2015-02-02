using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System.Collections.Generic;


namespace Skill.Editor.AI
{
    public class BehaviorList : Grid
    {
        private Toolbar _BehaviorTabs;
        private Box _ListViewToolbarBg;
        private ToolbarButton _BtnActions;
        private ToolbarButton _BtnConditions;
        private ToolbarButton _BtnDecorators;
        private ToolbarButton _BtnComposites;

        private Skill.Framework.UI.Extended.ListBox _LbItems;
        private BehaviorTreeEditorWindow _Editor;

        public BehaviorList(BehaviorTreeEditorWindow editor)
        {
            this._Editor = editor;
            this.RowDefinitions.Add(20, GridUnitType.Pixel);
            this.RowDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(400, GridUnitType.Pixel);
            this.ColumnDefinitions.Add(1, GridUnitType.Star);

            _ListViewToolbarBg = new Box() { Row = 0, Column = 1 };
            this.Controls.Add(_ListViewToolbarBg);

            _BehaviorTabs = new Toolbar() { Row = 0, Column = 0 };
            this.Controls.Add(_BehaviorTabs);

            _BtnActions = new ToolbarButton();
            _BtnActions.Content.text = "Actions";
            _BehaviorTabs.Items.Add(_BtnActions);

            _BtnConditions = new ToolbarButton();
            _BtnConditions.Content.text = "Conditions";
            _BehaviorTabs.Items.Add(_BtnConditions);

            _BtnDecorators = new ToolbarButton();
            _BtnDecorators.Content.text = "Decorators";
            _BehaviorTabs.Items.Add(_BtnDecorators);

            _BtnComposites = new ToolbarButton();
            _BtnComposites.Content.text = "Composites";
            _BehaviorTabs.Items.Add(_BtnComposites);

            _LbItems = new Framework.UI.Extended.ListBox() { Row = 1, Column = 0, ColumnSpan = 2, BackgroundVisible = true };
            _LbItems.DisableFocusable();
            this.Controls.Add(_LbItems);

            _BehaviorTabs.SelectedIndex = 0;

            _BtnActions.Selected += TabChanged;
            _BtnConditions.Selected += TabChanged;
            _BtnDecorators.Selected += TabChanged;
            _BtnComposites.Selected += TabChanged;

            _LbItems.SelectionChanged += _LbItems_SelectionChanged;
        }

        void _LbItems_SelectionChanged(object sender, System.EventArgs e)
        {
            if (_LbItems.SelectedItem == null)
                Skill.Editor.UI.Extended.InspectorProperties.Select(null);
            else
                Skill.Editor.UI.Extended.InspectorProperties.Select(_LbItems.SelectedItem as ListItem);
            _Editor.Repaint();
        }

        void TabChanged(object sender, System.EventArgs e)
        {
            Rebuild();
        }
        public void RefreshStyles()
        {

            GUIStyle toolbarStyle = new GUIStyle(Skill.Editor.Resources.Styles.ToolbarButton);
            toolbarStyle.onNormal = toolbarStyle.normal;
            toolbarStyle.onHover = toolbarStyle.hover;
            toolbarStyle.onActive = toolbarStyle.active;

            _LbItems.Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;
            _LbItems.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;

            _ListViewToolbarBg.Style = toolbarStyle;
            _BehaviorTabs.Style = Skill.Editor.Resources.Styles.ToolbarButton;

            _BtnActions.Content.image = Skill.Editor.Resources.UITextures.BTree.Action;
            _BtnConditions.Content.image = Skill.Editor.Resources.UITextures.BTree.Condition;
            _BtnDecorators.Content.image = Skill.Editor.Resources.UITextures.BTree.Decorator;
            _BtnComposites.Content.image = Skill.Editor.Resources.UITextures.BTree.Sequence;
        }

        public void DeselectInspector()
        {
            if (Skill.Editor.UI.Extended.InspectorProperties.GetSelected() is ListItem)
                Skill.Editor.UI.Extended.InspectorProperties.Select(null);
        }

        public void Rebuild()
        {
            if (_BtnActions.IsSelected)
                Rebuild(Skill.Framework.AI.BehaviorType.Action);
            else if (_BtnConditions.IsSelected)
                Rebuild(Skill.Framework.AI.BehaviorType.Condition);
            else if (_BtnDecorators.IsSelected)
                Rebuild(Skill.Framework.AI.BehaviorType.Decorator);
            else if (_BtnComposites.IsSelected)
                Rebuild(Skill.Framework.AI.BehaviorType.Composite);
        }
        private void Rebuild(Skill.Framework.AI.BehaviorType type)
        {
            _LbItems.Controls.Clear();
            var behaviors = _Editor.GetBehaviors();
            foreach (var b in behaviors)
            {
                if (b.BehaviorType == Framework.AI.BehaviorType.Composite)
                {
                    if (((CompositeData)b).CompositeType == Framework.AI.CompositeType.State)
                        continue;
                }
                if (b.BehaviorType == type)
                {
                    ListItem item = new ListItem(this, b, !_Editor.BehaviorTree.IsInHierarchy(b));
                    _LbItems.Items.Add(item);
                }
            }
        }


        private void Remove(ListItem item)
        {
            _LbItems.Items.Remove(item);
            _Editor.RemoveFromList(item.Data);
        }


        class ListItem : Skill.Framework.UI.Grid, Skill.Editor.UI.Extended.IProperties
        {
            private Image _ImgIcon;
            private Label _LblName;

            public BehaviorData Data { get; private set; }
            public BehaviorList OwnerList { get; private set; }
            public bool CanRemove { get; private set; }

            public ListItem(BehaviorList ownerList, BehaviorData data, bool canRemove)
            {
                this.CanRemove = canRemove;
                this.OwnerList = ownerList;
                this.Data = data;
                this.Height = 16;
                this.Margin = new Thickness(0, 4, 0, 0);

                this.ColumnDefinitions.Add(20, GridUnitType.Pixel);
                this.ColumnDefinitions.Add(1, GridUnitType.Star);

                if (CanRemove)
                {
                    Skill.Editor.UI.Rectangle redRect = new UI.Rectangle() { Row = 0, Column = 0, ColumnSpan = 2, Color = new Color(1, 0, 0, 0.2f) };
                    this.Controls.Add(redRect);
                }

                _ImgIcon = new Image() { Row = 0, Column = 0, Scale = ScaleMode.ScaleToFit, Texture = data.GetIcon() };
                this.Controls.Add(_ImgIcon);

                _LblName = new Label() { Row = 0, Column = 1 };
                this.Controls.Add(_LblName);

                UpdateContent();
            }

            private void UpdateContent()
            {
                string name = Data.Name;
                if (Data is IParameterData)
                {
                    var parameters = ((IParameterData)Data).ParameterDifinition;
                    if (parameters != null && parameters.Count > 0)
                        name += parameters.GetTemplate();
                }
                _LblName.Text = name;
            }

            #region IProperties members
            public string Title { get { return Data.BehaviorType.ToString(); } }
            public bool IsSelectedProperties { get; set; }

            private ListItemProperties _Properties;
            public UI.Extended.PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new ListItemProperties(this);
                    return _Properties;
                }
            }


            class ListItemProperties : UI.Extended.PropertiesPanel
            {
                private Button _BtnRemove;

                public ListItemProperties(ListItem item)
                    : base(item)
                {

                    _BtnRemove = new Button() { Margin = ControlMargin, IsEnabled = item.CanRemove, Height = 30 };
                    _BtnRemove.Content.text = "Remove Permanently";
                    Controls.Add(_BtnRemove);
                    _BtnRemove.Click += _BtnRemove_Click;
                    if (item.Data is IParameterData)
                        Controls.Add(new ParameterDifinitionEditor(item, ((IParameterData)item.Data).ParameterDifinition));
                }

                void _BtnRemove_Click(object sender, System.EventArgs e)
                {
                    ((ListItem)Object).OwnerList.Remove((ListItem)Object);
                    Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow((ListItem)Object);
                }

                protected override void RefreshData()
                {

                }

                protected override void SetDirty()
                {
                }
            }

            class ParameterDifinitionEditor : Skill.Framework.UI.Grid
            {

                public override float LayoutHeight { get { return 24 + 18 + _LbItems.LayoutHeight + 20 + 4; } }

                private ListItem _OwnerListItem;
                private ParameterDataCollection _Data;
                private Skill.Framework.UI.Box _HeaderBg;
                private Skill.Framework.UI.Grid _Header;
                private Skill.Framework.UI.Extended.ListBox _LbItems;
                private Skill.Framework.UI.Grid _PnlButtons;
                private Skill.Framework.UI.Button _BtnAdd;
                private Skill.Framework.UI.Button _BtnRemove;
                public ParameterDifinitionEditor(ListItem owner, ParameterDataCollection data)
                {
                    this._OwnerListItem = owner;
                    this._Data = data;
                    this.RowDefinitions.Add(24, GridUnitType.Pixel); // title
                    this.RowDefinitions.Add(18, GridUnitType.Pixel); // header
                    this.RowDefinitions.Add(1, GridUnitType.Star); // items
                    this.RowDefinitions.Add(20, GridUnitType.Pixel); // buttons

                    Skill.Editor.UI.DropShadowLabel title = new UI.DropShadowLabel() { Text = "Property Difinition", Height = 20, Margin = new Thickness(0, 0, 0, 4) };
                    this.Controls.Add(title);

                    _HeaderBg = new Box() { Row = 1, Style = (GUIStyle)"RL Header" };
                    this.Controls.Add(_HeaderBg);
                    _Header = new Grid() { Row = 1 };
                    _Header.ColumnDefinitions.Add(1, GridUnitType.Star);
                    _Header.ColumnDefinitions.Add(2, GridUnitType.Star);
                    _Header.ColumnDefinitions.Add(2, GridUnitType.Star);
                    _Header.Controls.Add(new Label() { Column = 0, Text = "Type" });
                    _Header.Controls.Add(new Label() { Column = 1, Text = "Name" });
                    _Header.Controls.Add(new Label() { Column = 2, Text = "Default Value" });
                    this.Controls.Add(_Header);


                    _LbItems = new Framework.UI.Extended.ListBox() { Row = 2 };
                    _LbItems.DisableFocusable();
                    _LbItems.BackgroundVisible = true;
                    _LbItems.Background.Style = (GUIStyle)"RL Background";
                    this.Controls.Add(_LbItems);

                    _PnlButtons = new Grid() { Row = 3 };
                    _PnlButtons.ColumnDefinitions.Add(1, GridUnitType.Star);
                    _PnlButtons.ColumnDefinitions.Add(20, GridUnitType.Pixel);// btn add
                    _PnlButtons.ColumnDefinitions.Add(20, GridUnitType.Pixel); // btn remove
                    this.Controls.Add(_PnlButtons);

                    _BtnAdd = new Button { Column = 1 };
                    _PnlButtons.Controls.Add(_BtnAdd);

                    _BtnRemove = new Button() { Column = 2, IsEnabled = false };
                    _PnlButtons.Controls.Add(_BtnRemove);

                    _LbItems.SelectionChanged += _LbItems_SelectionChanged;
                    _BtnAdd.Click += _BtnAdd_Click;
                    _BtnRemove.Click += _BtnRemove_Click;

                    for (int i = 0; i < _Data.Count; i++)
                    {
                        ParameterItem item = new ParameterItem(_OwnerListItem, _Data[i]);
                        _LbItems.Items.Add(item);
                    }
                }

                void _LbItems_SelectionChanged(object sender, System.EventArgs e)
                {
                    _BtnRemove.IsEnabled = _LbItems.SelectedItem != null;
                }

                void _BtnRemove_Click(object sender, System.EventArgs e)
                {
                    if (_LbItems.SelectedItem != null)
                    {
                        ParameterItem pi = (ParameterItem)_LbItems.SelectedItem;
                        _Data.Remove(pi.Data);
                        _LbItems.Items.Remove(pi);
                        _LbItems.SelectedItem = null;
                        this._OwnerListItem.UpdateContent();
                    }
                }

                void _BtnAdd_Click(object sender, System.EventArgs e)
                {
                    ParameterData data = new ParameterData() { Name = "NewInt", Type = ParameterType.Int, Value = "0" };
                    _Data.Add(data);
                    ParameterItem pi = new ParameterItem(_OwnerListItem, data);
                    _LbItems.Items.Add(pi);
                    _LbItems.SelectedItem = pi;

                    this._OwnerListItem.UpdateContent();
                }

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


                class ParameterItem : Skill.Framework.UI.Grid
                {
                    public ParameterData Data { get { return _Data; } }

                    private ListItem _OwnerListItem;
                    private ParameterData _Data;
                    private Skill.Editor.UI.EnumPopup _EnumType;
                    private Skill.Editor.UI.TextField _TxtName;
                    private Skill.Editor.UI.IntField _IntValue;
                    private Skill.Editor.UI.FloatField _FloatValue;
                    private Skill.Editor.UI.ToggleButton _BoolValue;
                    private Skill.Editor.UI.TextField _StringValue;

                    public ParameterItem(ListItem owner, ParameterData data)
                    {
                        this._OwnerListItem = owner;
                        this._Data = data;
                        this.Height = 24;
                        this.Padding = new Thickness(0, 4);

                        this.ColumnDefinitions.Add(1, GridUnitType.Star);
                        this.ColumnDefinitions.Add(2, GridUnitType.Star);
                        this.ColumnDefinitions.Add(2, GridUnitType.Star);

                        _EnumType = new UI.EnumPopup() { Column = 0, Margin = new Thickness(2, 0) };
                        _EnumType.Value = _Data.Type;
                        _EnumType.ValueChanged += _EnumType_ValueChanged;
                        this.Controls.Add(_EnumType);

                        _TxtName = new UI.TextField() { Column = 1, Text = data.Name, Margin = new Thickness(2, 0) };
                        _TxtName.TextChanged += _TxtName_TextChanged;
                        this.Controls.Add(_TxtName);

                        _IntValue = new UI.IntField() { Column = 2, Margin = new Thickness(2, 0, 6, 0), Visibility = Framework.UI.Visibility.Hidden };
                        this.Controls.Add(_IntValue);
                        _IntValue.ValueChanged += _IntField_ValueChanged;


                        _BoolValue = new UI.ToggleButton() { Column = 2, Margin = new Thickness(2, 0, 6, 0), Visibility = Framework.UI.Visibility.Hidden };
                        this.Controls.Add(_BoolValue);
                        _BoolValue.Changed += _ToggleButton_Changed;

                        _FloatValue = new UI.FloatField() { Column = 2, Margin = new Thickness(2, 0, 6, 0), Visibility = Framework.UI.Visibility.Hidden };
                        this.Controls.Add(_FloatValue);
                        _FloatValue.ValueChanged += _FloatField_ValueChanged;

                        _StringValue = new UI.TextField() { Column = 2, Text = _Data.Value, Margin = new Thickness(2, 0, 6, 0), Visibility = Framework.UI.Visibility.Hidden };
                        this.Controls.Add(_StringValue);
                        _StringValue.TextChanged += _TextField_TextChanged;

                        UpdateValue();

                    }

                    void _EnumType_ValueChanged(object sender, System.EventArgs e)
                    {
                        _Data.Type = (ParameterType)_EnumType.Value;
                        UpdateValue();
                        _OwnerListItem.UpdateContent();
                    }

                    void _TextField_TextChanged(object sender, System.EventArgs e)
                    {
                        _Data.Value = _StringValue.Text;
                        _OwnerListItem.UpdateContent();
                    }

                    void _ToggleButton_Changed(object sender, System.EventArgs e)
                    {
                        _Data.Value = _BoolValue.IsChecked.ToString();
                        _OwnerListItem.UpdateContent();
                    }

                    void _FloatField_ValueChanged(object sender, System.EventArgs e)
                    {
                        _Data.Value = _FloatValue.Value.ToString();
                        _OwnerListItem.UpdateContent();
                    }

                    void _IntField_ValueChanged(object sender, System.EventArgs e)
                    {
                        _Data.Value = _IntValue.Value.ToString();
                        _OwnerListItem.UpdateContent();
                    }

                    void _TxtName_TextChanged(object sender, System.EventArgs e)
                    {
                        if (!string.IsNullOrEmpty(_TxtName.Text))
                        {
                            _Data.Name = _TxtName.Text;
                            _OwnerListItem.UpdateContent();
                        }
                    }

                    private void UpdateValue()
                    {
                        _IntValue.Visibility = Framework.UI.Visibility.Hidden;
                        _FloatValue.Visibility = Framework.UI.Visibility.Hidden;
                        _BoolValue.Visibility = Framework.UI.Visibility.Hidden;
                        _StringValue.Visibility = Framework.UI.Visibility.Hidden;

                        switch (_Data.Type)
                        {
                            case ParameterType.Int:
                                _IntValue.Visibility = Framework.UI.Visibility.Visible;
                                if (string.IsNullOrEmpty(_Data.Value))
                                {
                                    int v = 0;
                                    if (int.TryParse(_Data.Value, out v))
                                    {
                                        _IntValue.Value = v;
                                    }
                                    else
                                    {
                                        _IntValue.Value = 0;
                                        _Data.Value = "0";
                                    }
                                }
                                break;
                            case ParameterType.Bool:
                                _BoolValue.Visibility = Framework.UI.Visibility.Visible;
                                if (string.IsNullOrEmpty(_Data.Value))
                                {
                                    bool v = false;
                                    if (bool.TryParse(_Data.Value, out v))
                                    {
                                        _BoolValue.IsChecked = v;
                                    }
                                    else
                                    {
                                        _BoolValue.IsChecked = false;
                                        _Data.Value = "false";
                                    }
                                }
                                break;
                            case ParameterType.Float:
                                _FloatValue.Visibility = Framework.UI.Visibility.Visible;
                                if (string.IsNullOrEmpty(_Data.Value))
                                {
                                    float v = 0;
                                    if (float.TryParse(_Data.Value, out v))
                                    {
                                        _FloatValue.Value = v;
                                    }
                                    else
                                    {
                                        _FloatValue.Value = 0;
                                        _Data.Value = "0";
                                    }
                                }
                                break;
                            case ParameterType.String:
                                _StringValue.Visibility = Framework.UI.Visibility.Visible;
                                _StringValue.Text = _Data.Value;
                                break;
                        }
                    }
                }
            }

            #endregion
        }
    }
}