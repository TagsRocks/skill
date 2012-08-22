using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.AI
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
    public class Parameter : IXElement
    {
        /// <summary> Name of parameter </summary>
        public string Name { get; set; }
        /// <summary> Type of parameter </summary>
        public ParameterType Type { get; set; }
        /// <summary> Value of parameter </summary>
        public string Value { get; set; }

        /// <summary> Fill a XElement with parameter data </summary>
        /// <returns>XElement</returns>
        public XElement ToXElement()
        {
            XElement p = new XElement("Parameter");
            p.SetAttributeValue("Name", this.Name);
            p.SetAttributeValue("Type", this.Type);
            p.SetAttributeValue("Value", (this.Value != null) ? this.Value : "");
            return p;
        }

        /// <summary> Load data from XElement loaded from file </summary>
        /// <param name="e">XElement containing data</param>
        public void Load(XElement e)
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
    }
    #endregion

    #region ParameterCollection

    /// <summary>
    /// A Collection of parameters. each collection defines all parameters for one Behavior
    /// </summary>
    public class ParameterCollection : ICollection<Parameter>, IXElement
    {
        private List<Parameter> _Parameters;

        public Parameter this[int index] { get { return _Parameters[index]; } }

        /// <summary>
        /// Create an instance of ParameterCollection
        /// </summary>
        public ParameterCollection()
        {
            _Parameters = new List<Parameter>();
        }

        /// <summary>
        /// Add new parameter
        /// </summary>
        /// <param name="item">Parameter to add</param>
        public void Add(Parameter item)
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

        public bool Contains(Parameter item)
        {
            return _Parameters.Contains(item);

        }

        public void CopyTo(Parameter[] array, int arrayIndex)
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
        public bool Remove(Parameter item)
        {
            return _Parameters.Remove(item);
        }

        public IEnumerator<Parameter> GetEnumerator()
        {
            return _Parameters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Parameters as System.Collections.IEnumerable).GetEnumerator();
        }

        /// <summary>
        /// Create an XElement and fill it with Parameters
        /// </summary>
        /// <returns>Xelement containing Parameter data</returns>
        public XElement ToXElement()
        {
            XElement parameters = new XElement("Parameters");

            foreach (var p in this)
            {
                parameters.Add(p.ToXElement());
            }

            return parameters;
        }

        /// <summary>
        /// Load Parameters from XElement
        /// </summary>
        /// <param name="e"></param>
        public void Load(XElement e)
        {
            Clear();
            foreach (var item in e.Elements())
            {
                Parameter p = new Parameter();
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
                Parameter item = _Parameters[i];
                builder.Append(item.ToString());
                if (i < Count - 1)
                    builder.Append(", ");
            }

            builder.Append(')');

            return builder.ToString();
        }
    }
    #endregion
}
