using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.AI
{
    #region ParameterType
    /// <summary>
    /// Defines type of parameters for Behavior
    /// </summary>
    public enum ParameterType
    {
        Int,
        Bool,
        Float,
        String
    }
    #endregion

    #region Parameter
    /// <summary>
    /// Parameter of Behavior
    /// </summary>
    public class ParameterData : IXmlElementSerializable
    {
        /// <summary> Name of parameter </summary>
        public string Name { get; set; }
        /// <summary> Type of parameter </summary>
        public ParameterType Type { get; set; }
        /// <summary> Value of parameter </summary>
        public string Value { get; set; }

        /// <summary> Fill a XmlElement with parameter data </summary>
        /// <returns>XmlElement</returns>
        public XmlElement ToXmlElement()
        {
            XmlElement p = new XmlElement("Parameter");
            p.SetAttributeValue("Name", this.Name);
            p.SetAttributeValue("Type", this.Type.ToString());
            p.SetAttributeValue("Value", (this.Value != null) ? this.Value : "");
            return p;
        }

        /// <summary> Load data from XmlElement loaded from file </summary>
        /// <param name="e">XmlElement containing data</param>
        public void Load(XmlElement e)
        {
            try
            {
                this.Name = e.GetAttributeValueAsString("Name", string.Empty);
                string type = e.GetAttributeValueAsString("Type", this.Type.ToString());
                this.Type = (ParameterType)Enum.Parse(typeof(ParameterType), type, false);
                this.Value = e.GetAttributeValueAsString("Value", string.Empty);

            }
            catch
            {
            }
        }
        public override string ToString()
        {
            return string.Format("{0} = {1}", Name, Value);
        }

        public string GetTemplate()
        {
            return string.Format("{0} {1}", this.Type.ToString().ToLower(), Name);
        }
    }
    #endregion

    #region ParameterCollection

    /// <summary>
    /// A Collection of parameters. each collection defines all parameters for one Behavior
    /// </summary>
    public class ParameterDataCollection : ICollection<ParameterData>, IXmlElementSerializable
    {
        public const string ElementName = "Parameters";

        private List<ParameterData> _Parameters;

        public ParameterData this[int index] { get { return _Parameters[index]; } }

        /// <summary>
        /// Create an instance of ParameterCollection
        /// </summary>
        public ParameterDataCollection()
        {
            _Parameters = new List<ParameterData>();
        }

        /// <summary>
        /// Add new parameter
        /// </summary>
        /// <param name="item">Parameter to add</param>
        public void Add(ParameterData item)
        {
            _Parameters.Add(item);
        }

        /// <summary>
        /// Remove all Parameters
        /// </summary>
        public void Clear()
        {
            _Parameters.Clear();
        }

        public bool Contains(ParameterData item)
        {
            return _Parameters.Contains(item);

        }

        public void CopyTo(ParameterData[] array, int arrayIndex)
        {
            _Parameters.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Number of Parameters in collection
        /// </summary>
        public int Count
        {
            get { return _Parameters.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Remove a Parameter
        /// </summary>
        /// <param name="item">Parameter to remove</param>
        /// <returns>True for success, otherwise false</returns>
        public bool Remove(ParameterData item)
        {
            return _Parameters.Remove(item);
        }

        public IEnumerator<ParameterData> GetEnumerator()
        {
            return _Parameters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Parameters as System.Collections.IEnumerable).GetEnumerator();
        }

        /// <summary>
        /// Create an XmlElement and fill it with Parameters
        /// </summary>
        /// <returns>Xelement containing Parameter data</returns>
        public XmlElement ToXmlElement()
        {
            XmlElement parameters = new XmlElement(ElementName);

            foreach (var p in this)
            {
                parameters.AppendChild(p.ToXmlElement());
            }

            return parameters;
        }

        /// <summary>
        /// Load Parameters from XmlElement
        /// </summary>
        /// <param name="e"></param>
        public void Load(XmlElement e)
        {
            Clear();
            foreach (var item in e)
            {
                ParameterData p = new ParameterData();
                p.Load(item);
                this.Add(p);
            }
        }

        /// <summary>
        /// Return all Parameters as single line of string
        /// </summary>
        /// <returns>Parameters as string</returns>
        public override string ToString()
        {
            if (Count == 0) return string.Empty;

            StringBuilder builder = new StringBuilder();

            builder.Append('(');

            for (int i = 0; i < Count; i++)
            {
                ParameterData item = _Parameters[i];
                builder.Append(item.ToString());
                if (i < Count - 1)
                    builder.Append(", ");
            }

            builder.Append(')');

            return builder.ToString();
        }


        /// <summary>
        /// Return all Parameters as single line of string
        /// </summary>
        /// <returns>Parameters as string</returns>
        public string GetTemplate()
        {
            if (Count == 0) return string.Empty;

            StringBuilder builder = new StringBuilder();

            builder.Append('(');

            for (int i = 0; i < Count; i++)
            {
                ParameterData item = _Parameters[i];
                builder.Append(item.GetTemplate());
                if (i < Count - 1)
                    builder.Append(", ");
            }

            builder.Append(')');

            return builder.ToString();
        }


        public void Match(ParameterDataCollection difinition)
        {
            foreach (var p in difinition)
            {
                bool found = false;
                foreach (var item in this)
                {
                    if (item.Name == p.Name)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    this.Add(new ParameterData() { Name = p.Name, Type = p.Type, Value = p.Value });
            }

            int index = 0;
            while (index < this.Count)
            {
                var mp = this[index];
                bool found = false;
                foreach (var item in difinition)
                {
                    if (item.Name == mp.Name)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    this.Remove(mp);
                    continue;
                }
                index++;
            }
        }
    }
    #endregion

    #region IParameterData
    public interface IParameterData : IXmlElementSerializable
    {
        ParameterDataCollection ParameterDifinition { get; }
    }
    #endregion

    #region ParameterEditor
    public class ParameterEditor : Skill.Framework.UI.Grid
    {
        private bool _RefreshStyle;
        private ParameterDataCollection _Data;
        private ParameterDataCollection _DataDifinition;
        private IBehaviorItem _Item;
        private Skill.Framework.UI.ListBox _FieldsList;
        private Skill.Framework.UI.Label _Title;
        private Skill.Framework.UI.Grid _ButtonsPanel;
        private Skill.Editor.UI.IntPopup _BtnAdd;
        private Skill.Framework.UI.Image _BtnAddImage;
        private Skill.Framework.UI.Button _BtnRemove;

        public override float LayoutHeight
        {
            get
            {
                return UnityEngine.Mathf.Max(44, _FieldsList.Items.Count * 22) + 16 + 16 + 4;
            }
        }

        public ParameterEditor(IBehaviorItem item, ParameterDataCollection dataDifinition, ParameterDataCollection data)
        {
            _Item = item;
            _DataDifinition = dataDifinition;
            _Data = data;

            this._RefreshStyle = true;

            this.RowDefinitions.Add(16, Skill.Framework.UI.GridUnitType.Pixel); // title
            this.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // list
            this.RowDefinitions.Add(16, Skill.Framework.UI.GridUnitType.Pixel); // buttons

            _Title = new Framework.UI.Label { Row = 0, Text = "Parameters" };
            this.Controls.Add(_Title);

            _FieldsList = new Skill.Framework.UI.ListBox() { Row = 1 };
            _FieldsList.DisableFocusable();
            _FieldsList.BackgroundVisible = true;
            this.Controls.Add(_FieldsList);

            _ButtonsPanel = new Framework.UI.Grid() { Row = 2 };
            _ButtonsPanel.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _ButtonsPanel.ColumnDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel);
            _ButtonsPanel.ColumnDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel);
            this.Controls.Add(_ButtonsPanel);

            _BtnAddImage = new Framework.UI.Image() { Column = 1 };
            _ButtonsPanel.Controls.Add(_BtnAddImage);


            _BtnAdd = new UI.IntPopup() { Column = 1 };
            _BtnAdd.Options.Add(new UI.PopupOption(1, "float"));
            _BtnAdd.Options.Add(new UI.PopupOption(2, "int"));
            _BtnAdd.Options.Add(new UI.PopupOption(3, "bool"));
            _BtnAdd.Options.Add(new UI.PopupOption(4, "string"));
            _ButtonsPanel.Controls.Add(_BtnAdd);

            _BtnRemove = new Framework.UI.Button() { Column = 2, IsEnabled = false };
            _ButtonsPanel.Controls.Add(_BtnRemove);


            _BtnAdd.OptionChanged += _BtnAdd_OptionChanged;
            _BtnRemove.Click += _BtnRemove_Click;
            _FieldsList.SelectionChanged += _FieldsList_SelectionChanged;

            Rebuild();
        }

        void _BtnAdd_OptionChanged(object sender, EventArgs e)
        {
            if (_BtnAdd.SelectedValue > 0)
            {
                if (_BtnAdd.SelectedOption.Content.text == "float")
                    AddNewParameter(ParameterType.Float);
                else if (_BtnAdd.SelectedOption.Content.text == "int")
                    AddNewParameter(ParameterType.Int);
                else if (_BtnAdd.SelectedOption.Content.text == "bool")
                    AddNewParameter(ParameterType.Bool);
                else if (_BtnAdd.SelectedOption.Content.text == "string")
                    AddNewParameter(ParameterType.String);

                _BtnAdd.SelectedValue = 0;
            }
        }

        void _FieldsList_SelectionChanged(object sender, EventArgs e)
        {
            _BtnRemove.IsEnabled = _FieldsList.SelectedItem != null;
        }

        void _BtnRemove_Click(object sender, System.EventArgs e)
        {
            RemoveSelectedParameter();
        }

        private void RemoveSelectedParameter()
        {
            if (_FieldsList.SelectedItem != null)
            {
                ParameterData pd = (ParameterData)_FieldsList.SelectedItem.UserData;
                _Data.Remove(pd);
                _DataDifinition.Match(_Data);
                Rebuild();
                _Item.RefreshContent();
                _Item.Editor.RefreshSameContent(_Item);
            }
        }

        private void AddNewParameter(ParameterType type)
        {
            ParameterData pd = new ParameterData();
            pd.Type = type;
            pd.Name = "new" + type.ToString();

            switch (pd.Type)
            {
                case ParameterType.Int:
                    pd.Value = "0";
                    break;
                case ParameterType.Bool:
                    pd.Value = "false";
                    break;
                case ParameterType.Float:
                    pd.Value = "0";
                    break;
                case ParameterType.String:
                    pd.Value = "";
                    break;
            }


            int i = 0;
            string name = pd.Name;
            while (HasName(_DataDifinition, pd.Name))
            {
                pd.Name = name + (i++).ToString();
            }


            _DataDifinition.Add(pd);
            _Data.Match(_DataDifinition);

            Rebuild();
            _Item.RefreshContent();
            _Item.Editor.RefreshSameContent(_Item);
        }

        private bool HasName(ParameterDataCollection collection, string parameterName)
        {
            foreach (var item in collection)
            {
                if (item.Name == parameterName)
                    return true;
            }
            return false;
        }
        protected override void BeginRender()
        {
            if (_RefreshStyle)
            {
                _RefreshStyle = false;

                if (_Title.Style == null)
                {
                    _Title.Style = new UnityEngine.GUIStyle((UnityEngine.GUIStyle)"RL Header");
                    _Title.Style.alignment = UnityEngine.TextAnchor.MiddleCenter;
                }

                _FieldsList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;

                _BtnAddImage.Texture = Skill.Editor.Resources.UITextures.Plus;
                _BtnRemove.Content.image = Skill.Editor.Resources.UITextures.Minus;

                _BtnAdd.Style = Skill.Editor.Resources.Styles.SmallButton;
                _BtnRemove.Style = Skill.Editor.Resources.Styles.SmallButton;
            }
            base.BeginRender();
        }




        public void Rebuild()
        {
            _FieldsList.Items.Clear();
            foreach (var pd in _Data)
            {
                ParameterField field = new ParameterField(this, pd);
                _FieldsList.Items.Add(field);
            }
        }

        private void RenameParameter(ParameterData data)
        {
            int index = -1;
            for (int i = 0; i < _Data.Count; i++)
            {
                if (data == _Data[i])
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                _DataDifinition[index].Name = data.Name;
            }
        }

        class ParameterField : Skill.Framework.UI.Grid
        {
            private ParameterEditor _Owner;
            private ParameterData _Data;


            private Skill.Framework.UI.Image _Icon;
            private Skill.Editor.UI.TextField _ParameterName;
            private Skill.Framework.UI.BaseControl _ParameterField;

            public ParameterField(ParameterEditor owner, ParameterData data)
            {
                this._Owner = owner;
                this._Data = data;
                this.UserData = data;
                this.Height = 22;

                this.ColumnDefinitions.Add(22, Framework.UI.GridUnitType.Pixel);
                this.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);
                this.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);

                _Icon = new Framework.UI.Image() { Column = 0, Scale = UnityEngine.ScaleMode.ScaleToFit, Texture = UnityEditor.EditorGUIUtility.FindTexture("Toolbar Minus") };
                this.Controls.Add(_Icon);

                _ParameterName = new UI.TextField() { Column = 1, Margin = new Framework.UI.Thickness(2), Text = _Data.Name };
                this.Controls.Add(_ParameterName);
                _ParameterName.TextChanged += _ParameterName_TextChanged;

                switch (_Data.Type)
                {
                    case ParameterType.Int:
                        _ParameterField = new UI.IntField();
                        int v = 0;
                        if (int.TryParse(_Data.Value, out v))
                        {
                            ((UI.IntField)_ParameterField).Value = v;
                        }
                        else
                        {
                            ((UI.IntField)_ParameterField).Value = 0;
                            _Data.Value = "0";
                        }
                        ((UI.IntField)_ParameterField).ValueChanged += intField_ValueChanged;
                        break;
                    case ParameterType.Bool:
                        _ParameterField = new UI.ToggleButton();

                        bool b = false;
                        if (bool.TryParse(_Data.Value, out b))
                        {
                            ((UI.ToggleButton)_ParameterField).IsChecked = b;
                        }
                        else
                        {
                            ((UI.ToggleButton)_ParameterField).IsChecked = false;
                            _Data.Value = "false";
                        }
                        ((UI.ToggleButton)_ParameterField).Changed += boolField_Changed;
                        break;
                    case ParameterType.Float:
                        _ParameterField = new UI.FloatField();

                        float f = 0;
                        if (float.TryParse(_Data.Value, out f))
                        {
                            ((UI.FloatField)_ParameterField).Value = f;
                        }
                        else
                        {
                            ((UI.FloatField)_ParameterField).Value = 0;
                            _Data.Value = "0";
                        }
                        ((UI.FloatField)_ParameterField).ValueChanged += floatField_ValueChanged;
                        break;
                    case ParameterType.String:
                        _ParameterField = new UI.TextField();
                        ((UI.TextField)_ParameterField).Text = _Data.Value;
                        ((UI.TextField)_ParameterField).TextChanged += textField_TextChanged;
                        break;
                }
                if (_ParameterField != null)
                {
                    _ParameterField.Column = 2;
                    _ParameterField.Margin = new Framework.UI.Thickness(2);
                    this.Controls.Add(_ParameterField);
                }
            }

            void _ParameterName_TextChanged(object sender, EventArgs e)
            {
                if (!string.IsNullOrEmpty(_ParameterName.Text))
                {
                    _Data.Name = _ParameterName.Text;
                    _Owner.RenameParameter(_Data);
                    _Owner._Item.RefreshContent();
                }
            }


            void textField_TextChanged(object sender, EventArgs e)
            {
                _Data.Value = ((Skill.Editor.UI.TextField)sender).Text;
                _Owner._Item.RefreshContent();
            }

            void boolField_Changed(object sender, EventArgs e)
            {
                _Data.Value = ((Skill.Editor.UI.ToggleButton)sender).IsChecked.ToString();
                _Owner._Item.RefreshContent();
            }

            void floatField_ValueChanged(object sender, EventArgs e)
            {
                _Data.Value = ((Skill.Editor.UI.FloatField)sender).Value.ToString();
                _Owner._Item.RefreshContent();
            }

            void intField_ValueChanged(object sender, EventArgs e)
            {
                _Data.Value = ((Skill.Editor.UI.IntField)sender).Value.ToString();
                _Owner._Item.RefreshContent();
            }
        }
    }
    #endregion
}
