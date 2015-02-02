using UnityEngine;
using System.Collections;
using System.Linq;
using Skill.Framework.UI;
using Skill.Framework.UI.Extended;

namespace Skill.Editor.Animation
{
    public class ParameterEditor : Grid
    {

        private Label _ParameterLabel;
        private ListBox _ParameterList;
        private UniformGrid _ParameterToolbar;

        private Skill.Editor.UI.Extended.MenuBar _MnuParameter;
        private Skill.Editor.UI.Extended.MenuBarItem _MnuAddParameter;
        private Skill.Editor.UI.MenuItem _MnuAddFloatParameter;
        private Skill.Editor.UI.MenuItem _MnuAddIntegerParameter;
        private Button _BtnRemoveParameter;
        private AnimationTreeEditorWindow _Editor;

        public event System.EventHandler SelectedParameterChanged;

        private void OnSelectedParameterChanged()
        {
            if (SelectedParameterChanged != null)
                SelectedParameterChanged(this, System.EventArgs.Empty);
        }
        public AnimationTreeParameter SelectedParameter
        {
            get
            {
                if (_ParameterList.SelectedItem != null)
                    return ((ParameterItem)_ParameterList.SelectedItem).Data;
                else
                    return null;
            }
        }
        public ParameterEditor(AnimationTreeEditorWindow editor)
        {
            this._Editor = editor;

            this.RowDefinitions.Add(20, GridUnitType.Pixel);
            this.RowDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(60, GridUnitType.Pixel);

            _ParameterLabel = new Label() { Row = 0, Column = 0, Text = "Parameters" };
            this.Controls.Add(_ParameterLabel);

            _ParameterList = new ListBox() { Row = 1, Column = 0, ColumnSpan = 2 };
            _ParameterList.DisableFocusable();
            this.Controls.Add(_ParameterList);

            _ParameterToolbar = new UniformGrid() { Row = 0, Column = 1, Rows = 1, Columns = 2 };
            this.Controls.Add(_ParameterToolbar);

            _MnuParameter = new UI.Extended.MenuBar() { Column = 0 };
            _ParameterToolbar.Controls.Add(_MnuParameter);
            _MnuAddParameter = new UI.Extended.MenuBarItem();
            _MnuParameter.Controls.Add(_MnuAddParameter);
            _MnuAddFloatParameter = new UI.MenuItem("Float") { UserData = AnimationTreeParameterType.Float };
            _MnuAddParameter.Add(_MnuAddFloatParameter);
            _MnuAddIntegerParameter = new UI.MenuItem("Integer") { UserData = AnimationTreeParameterType.Integer };
            _MnuAddParameter.Add(_MnuAddIntegerParameter);

            _BtnRemoveParameter = new Button() { Column = 1 };
            _BtnRemoveParameter.Content.tooltip = "remove selected parameter";
            _ParameterToolbar.Controls.Add(_BtnRemoveParameter);

            SetButtonsEnable();

            _ParameterList.SelectionChanged += _ParameterList_SelectionChanged;
            _MnuAddFloatParameter.Click += _MnuAddParameter_Click;
            _MnuAddIntegerParameter.Click += _MnuAddParameter_Click;
            _BtnRemoveParameter.Click += _BtnRemoveParameter_Click;
        }

        void _BtnRemoveParameter_Click(object sender, System.EventArgs e)
        {
            RemoveSelectedParameter();
        }
        void _MnuAddParameter_Click(object sender, System.EventArgs e)
        {
            UI.MenuItem mnu = sender as UI.MenuItem;
            if (mnu != null)
                AddNewParameter((AnimationTreeParameterType)mnu.UserData);
        }
        void _ParameterList_SelectionChanged(object sender, System.EventArgs e)
        {
            OnSelectedParameterChanged();
            SetButtonsEnable();
            Skill.Editor.UI.Extended.InspectorProperties.Select((ParameterItem)_ParameterList.SelectedItem);
            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
        }
        private void SetButtonsEnable()
        {
            _MnuAddParameter.IsEnabled = true;
            _BtnRemoveParameter.IsEnabled = _ParameterList.SelectedItem != null;
        }
        internal void Clear()
        {
            _ParameterList.Items.Clear();
        }
        public void RefreshStyles()
        {
            _ParameterLabel.Style = Skill.Editor.Resources.Styles.Header;

            _MnuParameter.Background.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnRemoveParameter.Style = Skill.Editor.Resources.Styles.ToolbarButton;

            _MnuAddParameter.Content.image = Skill.Editor.Resources.UITextures.Add;
            _BtnRemoveParameter.Content.image = Skill.Editor.Resources.UITextures.Remove;
            _ParameterList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
        }


        public void Rebuild()
        {

            if (_Editor.Tree.Parameters == null)
                _Editor.Tree.Parameters = new AnimationTreeParameter[0];

            _ParameterList.Items.Clear();
            foreach (var item in _Editor.Tree.Parameters)
                Add(item);

        }
        private void Add(AnimationTreeParameter data)
        {
            ParameterItem item = new ParameterItem(this, data);
            _ParameterList.Items.Add(item);
        }

        private bool IsParameterExists(AnimationTreeParameter data)
        {
            foreach (ParameterItem item in _ParameterList.Items)
            {
                if (item.Data == data)
                    return true;
            }
            return false;
        }

        private void RemoveSelectedParameter()
        {
            if (_ParameterList.SelectedItem != null)
            {
                AnimationTreeParameter data = ((ParameterItem)_ParameterList.SelectedItem).Data;
                AnimationTreeParameter[] preParameters = _Editor.Tree.Parameters;
                AnimationTreeParameter[] newParameters = new AnimationTreeParameter[preParameters.Length - 1];

                int preIndex = 0;
                int newIndex = 0;
                while (newIndex < newParameters.Length && preIndex < preParameters.Length)
                {
                    if (preParameters[preIndex] == data)
                    {
                        preIndex++;
                        continue;
                    }
                    newParameters[newIndex] = preParameters[preIndex];
                    newIndex++;
                    preIndex++;
                }
                _Editor.Tree.Parameters = newParameters;
                _ParameterList.Items.Remove(_ParameterList.SelectedItem);
                _ParameterList.SelectedIndex = 0;
                SetButtonsEnable();
            }
            else
            {
                Debug.LogError("there is no selected Parameter to remove");
            }
        }
        private void AddNewParameter(AnimationTreeParameterType type)
        {
            AnimationTreeParameter Parameter = new AnimationTreeParameter() { Type = type };
            Parameter.Name = GetUniqueParameterName(string.Format("new{0}", type));
            AnimationTreeParameter[] preParameters = _Editor.Tree.Parameters;
            AnimationTreeParameter[] newParameters = new AnimationTreeParameter[preParameters.Length + 1];
            preParameters.CopyTo(newParameters, 0);
            newParameters[newParameters.Length - 1] = Parameter;
            _Editor.Tree.Parameters = newParameters;
            Add(Parameter);
            SetButtonsEnable();
        }

        private string GetUniqueParameterName(string name)
        {
            int i = 1;
            string newName = name;
            while (_Editor.Tree.Parameters.Where(b => b.Name == newName).Count() > 0)
                newName = name + i++;
            return newName;
        }

        public void DeselectInspector()
        {
            if (Skill.Editor.UI.Extended.InspectorProperties.GetSelected() is ParameterItem)
                Skill.Editor.UI.Extended.InspectorProperties.Select(null);
        }

        public void RefreshContents()
        {
            foreach (ParameterItem item in _ParameterList.Items)
            {
                item.RefreshContent();
            }
        }

        private class ParameterItem : Label, Skill.Editor.UI.Extended.IProperties
        {
            private ParameterEditor _Editor;
            public AnimationTreeParameter Data { get; private set; }
            public ParameterItem(ParameterEditor editor, AnimationTreeParameter data)
            {
                this._Editor = editor;
                this.Data = data;
                this.Text = data.Name;
                //if (data.Type == AnimationTreeParameterType.Float)
                //    this.Content.image = Skill.Editor.Resources.UITextures.Matinee.Float;
                //else
                //    this.Content.image = Skill.Editor.Resources.UITextures.Matinee.Integer;
            }

            [Skill.Framework.ExposeProperty(1, "Name", "name of parameter")]
            public string Name2
            {
                get
                {
                    return Data.Name;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!_Editor._Editor.Tree.IsParameterExist(value))
                        {
                            _Editor._Editor.ChangeParameterName(Data.Name, value);
                            Data.Name = value;
                            this.Text = value;
                        }
                    }
                }
            }

            [Skill.Framework.ExposeProperty(10, "Comment", "comment of parameter")]
            public string Comment
            {
                get
                {
                    return Data.Comment;
                }
                set
                {
                    Data.Comment = value;
                }
            }

            public bool IsSelectedProperties { get; set; }
            private ParameterItemProperties _Properties;
            public Skill.Editor.UI.Extended.PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new ParameterItemProperties(this);
                    return _Properties;
                }
            }
            public string Title
            {
                get
                {
                    if (Data.Type == AnimationTreeParameterType.Float)
                        return "Float Parameter";
                    else
                        return "Integer Parameter";
                }
            }

            class ParameterItemProperties : Skill.Editor.UI.Extended.ExposeProperties
            {
                private Skill.Editor.UI.FloatField _FloatField;
                private Skill.Editor.UI.IntField _IntField;

                private ParameterItem _Item;
                public ParameterItemProperties(ParameterItem item)
                    : base(item)
                {
                    _Item = item;
                }

                protected override void SetDirty()
                {
                    Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(_Item);
                }

                protected override void CreateCustomFileds()
                {
                    base.CreateCustomFileds();
                    _Item = (ParameterItem)Object;
                    if (_Item.Data.Type == AnimationTreeParameterType.Float)
                    {
                        _FloatField = new UI.FloatField();
                        _FloatField.Label.text = "Default Value";
                        _FloatField.ValueChanged += _FloatField_ValueChanged;
                        Controls.Add(_FloatField);
                    }
                    else
                    {
                        _IntField = new UI.IntField();
                        _IntField.Label.text = "Default Value";
                        _IntField.ValueChanged += _IntField_ValueChanged;
                        Controls.Add(_IntField);
                    }
                }

                void _IntField_ValueChanged(object sender, System.EventArgs e)
                {
                    _Item.Data.DefaultValue = _IntField.Value;
                }

                void _FloatField_ValueChanged(object sender, System.EventArgs e)
                {
                    _Item.Data.DefaultValue = _FloatField.Value;
                }

                protected override void RefreshData()
                {
                    base.RefreshData();
                    if (_Item.Data.Type == AnimationTreeParameterType.Float)
                        _FloatField.Value = _Item.Data.DefaultValue;
                    else
                        _IntField.Value = (int)_Item.Data.DefaultValue;
                }
            }

            internal void RefreshContent()
            {
                this.Text = Data.Name;
            }
        }
    }
}
