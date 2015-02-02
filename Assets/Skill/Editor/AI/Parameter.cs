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
        private ParameterDataCollection _Data;
        private IBehaviorItem _Item;
        private Skill.Framework.UI.StackPanel _FieldsPanel;

        public override float LayoutHeight { get { return 24 + _FieldsPanel.LayoutHeight + 4; } }

        public ParameterEditor(IBehaviorItem item, ParameterDataCollection data)
        {
            _Item = item;
            _Data = data;

            this.RowDefinitions.Add(24, Framework.UI.GridUnitType.Pixel);
            this.RowDefinitions.Add(1, Framework.UI.GridUnitType.Star);

            Skill.Editor.UI.DropShadowLabel title = new UI.DropShadowLabel() { Row = 0, Text = "Properties", Height = 20, Margin = new Framework.UI.Thickness(0, 0, 0, 4) };
            Controls.Add(title);

            Skill.Framework.UI.Box bg = new Framework.UI.Box() { Row = 1, Style = (UnityEngine.GUIStyle)"RL Background" };
            Controls.Add(bg);

            _FieldsPanel = new Framework.UI.StackPanel() { Row = 1 };
            Controls.Add(_FieldsPanel);

            Rebuild();
        }

        void textField_TextChanged(object sender, EventArgs e)
        {
            ((ParameterData)((Skill.Editor.UI.TextField)sender).UserData).Value = ((Skill.Editor.UI.TextField)sender).Text;
            _Item.RefreshContent();
        }

        void boolField_Changed(object sender, EventArgs e)
        {
            ((ParameterData)((Skill.Editor.UI.ToggleButton)sender).UserData).Value = ((Skill.Editor.UI.ToggleButton)sender).IsChecked.ToString();
            _Item.RefreshContent();
        }

        void floatField_ValueChanged(object sender, EventArgs e)
        {
            ((ParameterData)((Skill.Editor.UI.FloatField)sender).UserData).Value = ((Skill.Editor.UI.FloatField)sender).Value.ToString();
            _Item.RefreshContent();
        }

        void intField_ValueChanged(object sender, EventArgs e)
        {
            ((ParameterData)((Skill.Editor.UI.IntField)sender).UserData).Value = ((Skill.Editor.UI.IntField)sender).Value.ToString();
            _Item.RefreshContent();
        }

        public void Rebuild()
        {
            foreach (var c in _FieldsPanel.Controls)
            {
                if (c is Skill.Editor.UI.IntField) ((Skill.Editor.UI.IntField)c).ValueChanged -= intField_ValueChanged;
                else if (c is Skill.Editor.UI.FloatField) ((Skill.Editor.UI.FloatField)c).ValueChanged -= floatField_ValueChanged;
                else if (c is Skill.Editor.UI.ToggleButton) ((Skill.Editor.UI.ToggleButton)c).Changed -= boolField_Changed;
                else if (c is Skill.Editor.UI.TextField) ((Skill.Editor.UI.TextField)c).TextChanged -= textField_TextChanged;
            }

            _FieldsPanel.Controls.Clear();
            Skill.Framework.UI.Thickness margin = new Framework.UI.Thickness(2);
            foreach (var pd in _Data)
            {
                switch (pd.Type)
                {
                    case ParameterType.Int:
                        Skill.Editor.UI.IntField intField = new UI.IntField() { UserData = pd, Margin = margin };
                        intField.Label.text = pd.Name;
                        int v = 0;
                        if (int.TryParse(pd.Value, out v))
                        {
                            intField.Value = v;
                        }
                        else
                        {
                            intField.Value = 0;
                            pd.Value = "0";
                        }
                        _FieldsPanel.Controls.Add(intField);
                        intField.ValueChanged += intField_ValueChanged;
                        break;
                    case ParameterType.Bool:
                        Skill.Editor.UI.ToggleButton boolField = new UI.ToggleButton() { UserData = pd, Margin = margin };
                        boolField.Label.text = pd.Name;
                        bool b = false;
                        if (bool.TryParse(pd.Value, out b))
                        {
                            boolField.IsChecked = b;
                        }
                        else
                        {
                            boolField.IsChecked = false;
                            pd.Value = "false";
                        }
                        _FieldsPanel.Controls.Add(boolField);
                        boolField.Changed += boolField_Changed;
                        break;
                    case ParameterType.Float:
                        Skill.Editor.UI.FloatField floatField = new UI.FloatField() { UserData = pd, Margin = margin };
                        floatField.Label.text = pd.Name;
                        float f = 0;
                        if (float.TryParse(pd.Value, out f))
                        {
                            floatField.Value = f;
                        }
                        else
                        {
                            floatField.Value = 0;
                            pd.Value = "0";
                        }
                        _FieldsPanel.Controls.Add(floatField);
                        floatField.ValueChanged += floatField_ValueChanged;
                        break;
                    case ParameterType.String:
                        Skill.Editor.UI.TextField textField = new UI.TextField() { UserData = pd, Margin = margin };
                        textField.Label.text = pd.Name;
                        textField.Text = pd.Value;
                        _FieldsPanel.Controls.Add(textField);
                        textField.TextChanged += textField_TextChanged;
                        break;
                }
            }
        }
    }
    #endregion
}
