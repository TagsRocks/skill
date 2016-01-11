using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework.UI;

namespace Skill.Editor.IO
{

    public class SaveDataEditorWindow : EditorWindow
    {
        #region SaveDataEditorWindow
        private static SaveDataEditorWindow _Instance;
        public static SaveDataEditorWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = ScriptableObject.CreateInstance<SaveDataEditorWindow>();
                return _Instance;
            }
        }

        private static Vector2 Size = new Vector2(500, 200);

        public SaveDataEditorWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.titleContent = new GUIContent("SaveData");
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = Size;
            CreateUI();
        }

        void OnFocus()
        {
            if (_Frame != null)
            {
                _RefreshStyles = true;
                if (_SaveDataItem != null)
                {
                    _SaveDataItem.UpdateText();
                }
            }
        }

        void OnLostFocus()
        {
            Save();
        }

        void OnDestroy()
        {
            var selected = Skill.Editor.UI.InspectorProperties.GetSelected();
            if (selected is IListItem)
                Skill.Editor.UI.InspectorProperties.Select(null);
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
        private Skill.Editor.UI.DropShadowLabel _LblClasses;
        private Skill.Editor.UI.DropShadowLabel _LblProperties;

        private Grid _ClassPanel;
        private ListBox _ClassList;
        private StackPanel _ClassButtonsPanel;
        private Button _BtnAddClass;
        private Button _BtnRemoveClass;

        private Grid _PropertiesPanel;
        private ListBox _PropertyList;
        private StackPanel _PropertyButtonsPanel;
        private Button _BtnPropertyUp;
        private Button _BtnAddPrimitiveProperty;
        private Button _BtnAddClassProperty;
        private Button _BtnRemoveProperty;
        private Button _BtnPropertyDown;

        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);

            _Frame.Grid.RowDefinitions.Add(20, GridUnitType.Pixel); // Label
            _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star); // Panels

            _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star); // class Panel        
            _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star); // Properties Panel
            _Frame.Grid.Padding = new Thickness(2, 4);

            _LblClasses = new Skill.Editor.UI.DropShadowLabel() { Row = 0, Column = 0, Text = "Class", Margin = new Thickness(0, 0, 20, 2) };
            _Frame.Controls.Add(_LblClasses);

            _LblProperties = new Skill.Editor.UI.DropShadowLabel() { Row = 0, Column = 1, Text = "Properties", Margin = new Thickness(0, 0, 20, 2) };
            _Frame.Controls.Add(_LblProperties);

            _ClassPanel = new Grid() { Row = 1, Column = 0 };
            _ClassPanel.ColumnDefinitions.Add(1, GridUnitType.Star);// listbox
            _ClassPanel.ColumnDefinitions.Add(36, GridUnitType.Pixel); // buttons
            _Frame.Controls.Add(_ClassPanel);

            _ClassList = new ListBox() { Row = 0, Column = 0, Margin = new Thickness(2), Padding = new Thickness(0, 2) };
            _ClassList.BackgroundVisible = true;
            _ClassList.DisableFocusable();
            _ClassPanel.Controls.Add(_ClassList);

            _ClassButtonsPanel = new StackPanel() { Row = 0, Column = 1, Orientation = Orientation.Vertical };
            _ClassPanel.Controls.Add(_ClassButtonsPanel);

            Thickness buttonMargin = new Thickness(3, 4, 5, 0);
            float buttonHeight = 26;

            _BtnAddClass = new Button() { Height = buttonHeight, Margin = buttonMargin };
            _BtnAddClass.Content.tooltip = "add new class";
            _ClassButtonsPanel.Controls.Add(_BtnAddClass);

            _BtnRemoveClass = new Button() { Height = buttonHeight, Margin = buttonMargin };
            _BtnRemoveClass.Content.tooltip = "remove selected class";
            _ClassButtonsPanel.Controls.Add(_BtnRemoveClass);


            _PropertiesPanel = new Grid() { Row = 1, Column = 1 };
            _PropertiesPanel.ColumnDefinitions.Add(1, GridUnitType.Star);// listbox
            _PropertiesPanel.ColumnDefinitions.Add(36, GridUnitType.Pixel); // buttons
            _Frame.Controls.Add(_PropertiesPanel);


            _PropertyButtonsPanel = new StackPanel() { Row = 0, Column = 1, Orientation = Orientation.Vertical };
            _PropertiesPanel.Controls.Add(_PropertyButtonsPanel);

            _PropertyList = new ListBox() { Row = 0, Column = 0, RowSpan = 4, Margin = new Thickness(2), Padding = new Thickness(0, 2) };
            _PropertyList.BackgroundVisible = true;
            _PropertyList.DisableFocusable();
            _PropertiesPanel.Controls.Add(_PropertyList);

            _BtnAddPrimitiveProperty = new Button() { Height = buttonHeight, Margin = buttonMargin };
            _BtnAddPrimitiveProperty.Content.tooltip = "add primitive property";
            _PropertyButtonsPanel.Controls.Add(_BtnAddPrimitiveProperty);

            _BtnAddClassProperty = new Button() { Height = buttonHeight, Margin = buttonMargin };
            _BtnAddClassProperty.Content.tooltip = "add class property";
            _PropertyButtonsPanel.Controls.Add(_BtnAddClassProperty);

            _BtnPropertyUp = new Button() { Height = buttonHeight, Margin = buttonMargin };
            _BtnPropertyUp.Content.tooltip = "move selected property up";
            _PropertyButtonsPanel.Controls.Add(_BtnPropertyUp);

            _BtnPropertyDown = new Button() { Height = buttonHeight, Margin = buttonMargin };
            _BtnPropertyDown.Content.tooltip = "move selected property down";
            _PropertyButtonsPanel.Controls.Add(_BtnPropertyDown);

            _BtnRemoveProperty = new Button() { Height = buttonHeight, Margin = buttonMargin };
            _BtnRemoveProperty.Content.tooltip = "remove selected property";
            _PropertyButtonsPanel.Controls.Add(_BtnRemoveProperty);

            CheckControlsEnable();


            _ClassList.SelectionChanged += _ClassList_SelectionChanged;
            _BtnAddClass.Click += _BtnAddClass_Click;
            _BtnRemoveClass.Click += _BtnRemoveClass_Click;

            _PropertyList.SelectionChanged += _PropertyList_SelectionChanged;
            _BtnAddPrimitiveProperty.Click += _BtnAddValueProperty_Click;
            _BtnAddClassProperty.Click += _BtnAddClassProperty_Click;
            _BtnRemoveProperty.Click += _BtnRemoveProperty_Click;
            _BtnPropertyUp.Click += _BtnPropertyUp_Click;
            _BtnPropertyDown.Click += _BtnPropertyDown_Click;
        }

        void _BtnPropertyDown_Click(object sender, System.EventArgs e)
        {
            PropertyDown();
        }

        void _BtnPropertyUp_Click(object sender, System.EventArgs e)
        {
            PropertyUp();
        }

        void _BtnRemoveProperty_Click(object sender, System.EventArgs e) { RemoveProperty(); }
        void _BtnAddClassProperty_Click(object sender, System.EventArgs e) { AddClassProperty(); }
        void _BtnAddValueProperty_Click(object sender, System.EventArgs e) { AddPrimitiveProperty(); }
        void _BtnRemoveClass_Click(object sender, System.EventArgs e) { RemoveClass(); }
        void _BtnAddClass_Click(object sender, System.EventArgs e) { AddClass(); }
        void _PropertyList_SelectionChanged(object sender, System.EventArgs e)
        {
            _Frame.Owner.Repaint();
            CheckControlsEnable();
            if (_PropertyList.SelectedItem != null)
                Skill.Editor.UI.InspectorProperties.Select(_PropertyList.SelectedItem as Skill.Editor.UI.IProperties);
        }
        void _ClassList_SelectionChanged(object sender, System.EventArgs e)
        {
            _Frame.Owner.Repaint();
            CheckControlsEnable();
            if (_ClassList.SelectedItem != null)
                Skill.Editor.UI.InspectorProperties.Select(_ClassList.SelectedItem as Skill.Editor.UI.IProperties);
            RefreshPropertyList();
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

                _ClassList.Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;
                _ClassList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
                _PropertyList.Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;
                _PropertyList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;

                _BtnAddClass.Content.image = Skill.Editor.Resources.UITextures.Add;
                _BtnRemoveClass.Content.image = Skill.Editor.Resources.UITextures.Remove;

                _BtnAddPrimitiveProperty.Content.image = Skill.Editor.Resources.UITextures.Add;
                _BtnAddClassProperty.Content.image = Skill.Editor.Resources.UITextures.Add;
                _BtnRemoveProperty.Content.image = Skill.Editor.Resources.UITextures.Remove;

                _BtnAddClassProperty.Content.image = Skill.Editor.Resources.UITextures.Icons.Class;
                _BtnAddPrimitiveProperty.Content.image = Skill.Editor.Resources.UITextures.Icons.Primitive;

                _BtnPropertyUp.Content.image = Skill.Editor.Resources.UITextures.ArrowUp;
                _BtnPropertyDown.Content.image = Skill.Editor.Resources.UITextures.ArrowDown;
            }
        }



        #endregion

        #region ListItems
        interface IListItem : Skill.Editor.UI.IProperties
        {
            SaveDataEditorWindow Editor { get; }
            void UpdateText();
        }

        interface IPropertyContainer
        {
            PropertyDataListItem[] GetProperties();
            void AddPrimitiveProperty();
            void AddClassProperty();
            void RemoveProperty(int propertyIndex);
            bool MoveUp(PropertyDataListItem item);
            bool MoveDown(PropertyDataListItem item);
        }

        class Properties : Skill.Editor.UI.ExposeProperties
        {
            private IListItem _Item;
            public Properties(IListItem item)
                : base(item)
            {
                this._Item = item;
            }
            protected override void SetDirty()
            {
                _Item.Editor.SetDirty2();
            }
        }

        class ClassDataListItem : Label, IListItem, IPropertyContainer
        {
            public ClassData Data { get; private set; }
            public SaveDataEditorWindow Editor { get; private set; }
            public ClassDataListItem(SaveDataEditorWindow editor, ClassData data)
            {
                this.Editor = editor;
                this.Data = data;
                LoadProperties();
                UpdateText();
            }
            public virtual void UpdateText()
            {
                if (IsStruct)
                    base.Text = string.Format("struct {0}", Data.Name);
                else
                    base.Text = string.Format("class {0}", Data.Name);
            }


            #region Expose Properties
            [Skill.Framework.ExposeProperty(101, "Name", "Name of Class")]
            public string Name2
            {
                get { return Data.Name; }
                set { Data.Name = value; UpdateText(); }
            }

            [Skill.Framework.ExposeProperty(102, "IsStruct", "Is struct? or class?")]
            public bool IsStruct
            {
                get { return Data.IsStruct; }
                set { Data.IsStruct = value; UpdateText(); }
            }

            [Skill.Framework.ExposeProperty(110, "Comment", "Comment of SaveData class")]
            public string Comment
            {
                get { return Data.Comment; }
                set { Data.Comment = value; }
            }
            #endregion

            #region IProperties members
            private bool _IsSelectedProperties;
            public bool IsSelectedProperties
            {
                get
                {
                    return _IsSelectedProperties;
                }
                set
                {
                    _IsSelectedProperties = value;
                    if (_IsSelectedProperties) UpdateText();
                }
            }

            private Properties _PropertiesPanel;
            public Skill.Editor.UI.PropertiesPanel Properties
            {
                get
                {
                    if (_PropertiesPanel == null)
                        _PropertiesPanel = new Properties(this);
                    return _PropertiesPanel;
                }
            }
            public virtual string Title { get { return "SaveClass"; } }
            #endregion

            #region IPropertyContainer members

            private List<PropertyDataListItem> _Properties;

            private void LoadProperties()
            {
                _Properties = new List<PropertyDataListItem>();
                if (Data.Properties == null)
                    Data.Properties = new PropertyData[0];
                for (int i = 0; i < Data.Properties.Length; i++)
                {
                    PropertyDataListItem property = null;
                    if (Data.Properties[i].Type == PropertyType.Primitive)
                        property = new PrimitivePropertyDataListItem(Editor, (PrimitivePropertyData)Data.Properties[i]);
                    else
                        property = new ClassPropertyDataListItem(Editor, (ClassPropertyData)Data.Properties[i]);
                    _Properties.Add(property);
                }
            }

            public PropertyDataListItem[] GetProperties()
            {
                return _Properties.ToArray();
            }

            public void AddPrimitiveProperty()
            {
                PrimitivePropertyData pd = new PrimitivePropertyData();
                pd.Name = "NewValue";
                pd.PrimitiveType = Skill.Editor.PrimitiveDataType.Integer;
                var property = new PrimitivePropertyDataListItem(Editor, pd);
                _Properties.Add(property);
                RebuildProperties();
            }
            public void AddClassProperty()
            {
                ClassPropertyData cd = new ClassPropertyData();
                cd.Name = "NewValue";
                cd.ClassName = "Unknow";
                var property = new ClassPropertyDataListItem(Editor, cd);
                _Properties.Add(property);
                RebuildProperties();
            }

            public void RemoveProperty(int propertyIndex)
            {
                _Properties.RemoveAt(propertyIndex);
                RebuildProperties();
            }
            private void RebuildProperties()
            {
                Data.Properties = new PropertyData[_Properties.Count];
                for (int i = 0; i < _Properties.Count; i++)
                    Data.Properties[i] = _Properties[i].Data;
                Editor.SetDirty2();
            }

            public bool MoveUp(PropertyDataListItem item)
            {
                int index = _Properties.IndexOf(item);
                if (index > 0)
                {
                    var selected = _Properties[index];
                    _Properties.RemoveAt(index);
                    _Properties.Insert(index - 1, selected);
                    RebuildProperties();
                    return true;
                }
                return false;
            }

            public bool MoveDown(PropertyDataListItem item)
            {
                int index = _Properties.IndexOf(item);
                if (index >= 0)
                {
                    var selected = _Properties[index];
                    _Properties.RemoveAt(index);
                    _Properties.Insert(index + 1, selected);
                    RebuildProperties();
                    return true;
                }
                return false;
            }
            #endregion
        }

        class SaveDataListItem : ClassDataListItem
        {

            public SaveDataListItem(SaveDataEditorWindow editor, SaveData data)
                : base(editor, data)
            { }
            public override void UpdateText()
            {
                if (IsStruct)
                    base.Text = string.Format("struct {0}", Editor._Asset.name);
                else
                    base.Text = string.Format("class {0}", Editor._Asset.name);
            }
            public new string Name2 { get; set; }            

            public override string Title { get { return "SaveData"; } }


            #region Expose Properties
            [Skill.Framework.ExposeProperty(105, "Namespace", "Namespace")]
            public string Namespace
            {
                get { return ((SaveData)Data).Namespace; }
                set { ((SaveData)Data).Namespace = value; UpdateText(); }
            }
            #endregion


        }

        abstract class PropertyDataListItem : Label, IListItem
        {
            public SaveDataEditorWindow Editor { get; private set; }
            public PropertyData Data { get; private set; }
            public abstract void UpdateText();
            public PropertyDataListItem(SaveDataEditorWindow editor, PropertyData data)
            {
                this.Editor = editor;
                this.Data = data;
                UpdateText();
            }

            #region Expose Properties
            [Skill.Framework.ExposeProperty(101, "Name", "Name of property")]
            public string Name2
            {
                get { return Data.Name; }
                set { Data.Name = value; UpdateText(); }
            }

            [Skill.Framework.ExposeProperty(103, "IsArray", "Whether this property is array?")]
            public bool IsArray
            {
                get { return Data.IsArray; }
                set { Data.IsArray = value; UpdateText(); }
            }

            [Skill.Framework.ExposeProperty(110, "Comment", "Comment of property")]
            public string Comment
            {
                get { return Data.Comment; }
                set { Data.Comment = value; }
            }
            #endregion

            #region IProperties members
            private bool _IsSelectedProperties;
            public bool IsSelectedProperties
            {
                get
                {
                    return _IsSelectedProperties;
                }
                set
                {
                    _IsSelectedProperties = value;
                    if (_IsSelectedProperties) UpdateText();
                }
            }

            private Properties _PropertiesPanel;
            public virtual Skill.Editor.UI.PropertiesPanel Properties
            {
                get
                {
                    if (_PropertiesPanel == null)
                        _PropertiesPanel = new Properties(this);
                    return _PropertiesPanel;
                }
            }
            public virtual string Title { get { return "PropertyData"; } }
            #endregion
        }

        class PrimitivePropertyDataListItem : PropertyDataListItem
        {

            [Skill.Framework.ExposeProperty(104, "SafeMemory", "Whether this property is safe in memory (works for float,int,bool)")]
            public bool SafeMemory
            {
                get { return ((PrimitivePropertyData)Data).SafeMemory; }
                set { ((PrimitivePropertyData)Data).SafeMemory = value; UpdateText(); }
            }

            public PrimitivePropertyDataListItem(SaveDataEditorWindow editor, PrimitivePropertyData data)
                : base(editor, data)
            {
            }
            public override void UpdateText()
            {
                PrimitivePropertyData v = (PrimitivePropertyData)Data;
                string valueStr = v.PrimitiveType.ToString();
                if (valueStr == "Integer") valueStr = v.SafeMemory ? "SafeInt" : "int";
                if (valueStr == "Float") valueStr = v.SafeMemory ? "SafeFloat" : "float";
                if (valueStr == "Boolean") valueStr = v.SafeMemory ? "SafeBool" : "bool";
                if (valueStr == "String") valueStr = "string";

                if (v.IsArray)
                    base.Text = string.Format("{0}[] {1}", valueStr, v.Name);
                else
                    base.Text = string.Format("{0} {1}", valueStr, v.Name);
            }

            #region IProperties members
            public override string Title { get { return "PrimitiveProperty"; } }
            #endregion

            #region Expose Properties
            [Skill.Framework.ExposeProperty(102, "PrimitiveType")]
            public Skill.Editor.PrimitiveDataType PrimitiveType
            {
                get { return ((PrimitivePropertyData)Data).PrimitiveType; }
                set
                {
                    ((PrimitivePropertyData)Data).PrimitiveType = value;
                    Editor.SetDirty2();
                    UpdateText();
                }
            }
            #endregion

        }

        class ClassPropertyDataListItem : PropertyDataListItem
        {
            #region IProperties members
            public override string Title { get { return "ClassProperty"; } }
            #endregion
            public ClassPropertyDataListItem(SaveDataEditorWindow editor, ClassPropertyData data)
                : base(editor, data)
            {
            }
            public override void UpdateText()
            {
                ClassPropertyData c = (ClassPropertyData)Data;
                if (c.IsArray)
                    base.Text = string.Format("{0}[] {1}", c.ClassName, c.Name);
                else
                    base.Text = string.Format("{0} {1}", c.ClassName, c.Name);
            }

            private ClassProperties _ClassPropertiesPanel;
            public override Skill.Editor.UI.PropertiesPanel Properties
            {
                get
                {
                    if (_ClassPropertiesPanel == null)
                        _ClassPropertiesPanel = new ClassProperties(this);
                    return _ClassPropertiesPanel;
                }
            }

            class ClassProperties : Properties
            {
                private Skill.Editor.UI.IntPopup _ClassNameField;
                private ClassPropertyDataListItem _Item;
                public ClassProperties(ClassPropertyDataListItem item)
                    : base(item)
                {
                    this._Item = item;
                }

                protected override void CreateCustomFileds()
                {
                    base.CreateCustomFileds();
                    _ClassNameField = new Skill.Editor.UI.IntPopup();
                    _ClassNameField.Label.text = "Class";
                    _ClassNameField.OptionChanged += _ClassNameField_OptionChanged;
                    base.Controls.Add(_ClassNameField);
                }

                void _ClassNameField_OptionChanged(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    ((ClassPropertyData)_Item.Data).ClassName = _ClassNameField.SelectedOption.Content.text;
                    _Item.UpdateText();
                }

                protected override void RefreshData()
                {
                    // because RefreshData() called in base class constructor
                    if (_Item == null) _Item = (ClassPropertyDataListItem)Object;

                    _ClassNameField.Options.Clear();
                    _ClassNameField.Options.Add(new Skill.Editor.UI.PopupOption(0, "Unknown"));

                    string[] classNames = _Item.Editor.GetClassNames();
                    for (int i = 0; i < classNames.Length; i++)
                        _ClassNameField.Options.Add(new Skill.Editor.UI.PopupOption(i + 1, classNames[i]));

                    int value = 0;
                    for (int i = 1; i < _ClassNameField.Options.Count; i++)
                    {
                        if (_ClassNameField.Options[i].Content.text == ((ClassPropertyData)_Item.Data).ClassName)
                        {
                            value = _ClassNameField.Options[i].Value;
                            break;
                        }
                    }
                    _ClassNameField.SelectedValue = value;
                    if (value == 0)
                        ((ClassPropertyData)_Item.Data).ClassName = "Unknown";
                    base.RefreshData();
                }
            }
        }
        #endregion


        private bool _IsChanged;
        private SaveDataListItem _SaveDataItem;
        [SerializeField]
        private SaveDataAsset _Asset;
        private SaveData _SaveData;
        public SaveDataAsset Asset
        {
            get { return _Asset; }
            set
            {
                if (_Asset != value)
                {
                    _Asset = value;
                    _SaveData = null;
                    Rebuild();
                }
            }
        }

        private SaveData DefaultSaveData()
        {
            SaveData saveData = new SaveData();
            saveData.Classes = new ClassData[0];
            saveData.Properties = new PropertyData[0];
            return saveData;
        }

        private void Save()
        {
            if (_IsChanged && _Asset != null && _SaveData != null)
            {
                _Asset.Save(_SaveData);
                _IsChanged = false;
            }
        }

        private void CheckControlsEnable()
        {
            _PropertiesPanel.IsEnabled = _ClassList.SelectedItem != null;
            _BtnRemoveProperty.IsEnabled = _PropertyList.SelectedItem != null;

            if (_ClassList.SelectedItem != null)
            {
                if (_ClassList.SelectedItem == _SaveDataItem)
                    _BtnRemoveClass.IsEnabled = false;
                else
                    _BtnRemoveClass.IsEnabled = true;
            }
            else
                _BtnRemoveClass.IsEnabled = false;

            _BtnPropertyDown.IsEnabled = _PropertyList.Items.Count > 1 &&
                                         _PropertyList.SelectedItem != null &&
                                         _PropertyList.SelectedIndex < _PropertyList.Items.Count - 1;

            _BtnPropertyUp.IsEnabled = _PropertyList.Items.Count > 1 &&
                                         _PropertyList.SelectedItem != null &&
                                         _PropertyList.SelectedIndex > 0;
        }
        private void Rebuild()
        {
            _RefreshStyles = true;
            Clear();
            if (_Asset != null)
            {
                if (_SaveData == null)
                {
                    if (!string.IsNullOrEmpty(_Asset.XmlData))
                    {
                        try
                        {
                            _SaveData = _Asset.Load();
                        }
                        catch (System.Exception ex)
                        {
                            _SaveData = DefaultSaveData();
                            Debug.LogError(ex.ToString());
                        }
                    }
                    else
                    {
                        _SaveData = DefaultSaveData();
                    }
                }

                _SaveDataItem = new SaveDataListItem(this, _SaveData);
                _ClassList.Items.Add(_SaveDataItem);
                _ClassList.SelectedIndex = 0;

                if (_SaveData.Classes != null)
                {
                    for (int i = 0; i < _SaveData.Classes.Length; i++)
                    {
                        if (_SaveData.Classes[i] != null)
                        {
                            ClassDataListItem item = new ClassDataListItem(this, _SaveData.Classes[i]);
                            _ClassList.Items.Add(item);
                        }
                    }
                }

                _ClassPanel.IsEnabled = true;
                CheckControlsEnable();
            }
        }
        private void Clear()
        {
            _ClassList.Items.Clear();
            _PropertyList.Items.Clear();
            _SaveDataItem = null;
            _ClassPanel.IsEnabled = false;
            CheckControlsEnable();
        }
        private void SetDirty2()
        {
            _IsChanged = true;
        }
        private void AddClass()
        {
            ClassData sc = new ClassData();
            sc.Name = "New";
            sc.Properties = new PropertyData[0];
            _ClassList.Items.Add(new ClassDataListItem(this, sc));
            _ClassList.SelectedIndex = _ClassList.Items.Count - 1;
            RebuildClasses();
        }
        private void RemoveClass()
        {
            if (_ClassList.SelectedItem != null)
            {
                _ClassList.Items.Remove(_ClassList.SelectedItem);
                RebuildClasses();
            }
        }
        private void RemoveProperty()
        {
            if (_ClassList.SelectedItem != null && _PropertyList.SelectedItem != null)
            {
                IPropertyContainer pc = _ClassList.SelectedItem as IPropertyContainer;
                if (pc != null)
                {
                    pc.RemoveProperty(_PropertyList.SelectedIndex);
                    RefreshPropertyList();
                    if (_PropertyList.Items.Count > 0)
                        _PropertyList.SelectedIndex = _PropertyList.Items.Count - 1;
                }
            }
        }
        private void AddClassProperty()
        {
            if (_ClassList.SelectedItem != null)
            {
                IPropertyContainer pc = _ClassList.SelectedItem as IPropertyContainer;
                if (pc != null)
                {
                    pc.AddClassProperty();
                    RefreshPropertyList();
                    if (_PropertyList.Items.Count > 0)
                        _PropertyList.SelectedIndex = _PropertyList.Items.Count - 1;
                }
            }
        }
        private void AddPrimitiveProperty()
        {
            if (_ClassList.SelectedItem != null)
            {
                IPropertyContainer pc = _ClassList.SelectedItem as IPropertyContainer;
                if (pc != null)
                {
                    pc.AddPrimitiveProperty();
                    RefreshPropertyList();
                    if (_PropertyList.Items.Count > 0)
                        _PropertyList.SelectedIndex = _PropertyList.Items.Count - 1;
                }
            }
        }
        private void RefreshPropertyList()
        {
            _PropertyList.Items.Clear();
            if (_ClassList.SelectedItem != null)
            {
                IPropertyContainer pc = _ClassList.SelectedItem as IPropertyContainer;
                if (pc != null)
                {
                    var properties = pc.GetProperties();
                    for (int i = 0; i < properties.Length; i++)
                        _PropertyList.Items.Add(properties[i]);
                }
            }
        }
        private void RebuildClasses()
        {
            if (_SaveData != null)
            {
                _SaveData.Classes = new ClassData[_ClassList.Items.Count - 1];
                for (int i = 1; i < _ClassList.Items.Count; i++)
                    _SaveData.Classes[i - 1] = ((ClassDataListItem)_ClassList.Items[i]).Data;
                SetDirty2();
            }
        }
        private string[] GetClassNames()
        {
            string[] classNames = new string[_ClassList.Items.Count - 1];
            for (int i = 1; i < _ClassList.Items.Count; i++)
                classNames[i - 1] = ((ClassDataListItem)_ClassList.Items[i]).Data.Name;
            return classNames;
        }

        private void PropertyDown()
        {
            if (_PropertyList.Items.Count > 1 &&
                _PropertyList.SelectedItem != null &&
                _PropertyList.SelectedIndex < _PropertyList.Items.Count - 1)
            {

                IPropertyContainer pc = _ClassList.SelectedItem as IPropertyContainer;
                if (pc != null)
                {
                    var selected = _PropertyList.SelectedItem;
                    if (pc.MoveDown((PropertyDataListItem)_PropertyList.SelectedItem))
                    {
                        RefreshPropertyList();
                        _PropertyList.SelectedItem = selected;
                    }
                }
            }
        }
        private void PropertyUp()
        {
            if (_PropertyList.Items.Count > 1 &&
                _PropertyList.SelectedItem != null &&
                _PropertyList.SelectedIndex > 0)
            {

                IPropertyContainer pc = _ClassList.SelectedItem as IPropertyContainer;
                if (pc != null)
                {
                    var selected = _PropertyList.SelectedItem;
                    if (pc.MoveUp((PropertyDataListItem)_PropertyList.SelectedItem))
                    {
                        RefreshPropertyList();
                        _PropertyList.SelectedItem = selected;
                    }
                }
            }
        }
    }

}