using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.IO
{
    /// <summary>
    /// Represents an element.
    /// </summary>
    public class XmlElement : IEnumerable<XmlElement>
    {
        private List<XmlElement> _Elements;
        private List<XmlAttribute> _Attributes;

        internal XmlElement(XmlDocument ownerDocument, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid name for XmlElement");
            this.Name = name;
            this.OwnerDocument = ownerDocument;
            _Elements = new List<XmlElement>();
            _Attributes = new List<XmlAttribute>();
        }

        private XmlAttribute FindAttribute(string name)
        {
            foreach (var att in _Attributes)
            {
                if (att.Name == name)
                    return att;
            }
            return null;
        }

        private XmlElement FindChild(string name)
        {
            foreach (var child in _Elements)
            {
                if (child.Name == name)
                    return child;
            }
            return null;
        }

        /// <summary> Parent element </summary>
        public XmlElement Parent { get; internal set; }

        /// <summary>
        /// Gets the first child element with the specified Name.
        /// </summary>
        /// <param name="name"> The qualified name of the element to retrieve. </param>
        /// <returns> The first XmlElement that matches the specified name. </returns>
        public XmlElement this[string name] { get { return FindChild(name); } }

        /// <summary>
        /// Gets the first child of the node.
        /// </summary>
        public XmlElement FirstChild { get { if (_Elements.Count > 0)return _Elements[0]; else return null; } }

        /// <summary>
        /// Gets the qualified name of the node.
        /// </summary>
        /// <returns>
        /// The qualified name of the node. For XmlElement nodes, this is the tag name of the element.
        /// </returns>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the XmlElement immediately following this element.
        /// </summary>
        public XmlElement NextSibling
        {
            get
            {
                if (Parent != null)
                    return Parent.GetNextSibling(this);
                else

                    return OwnerDocument.GetNextSibling(this);
            }
        }


        /// <summary>
        /// Gets the XmlDocument to which this node belongs.
        /// </summary>
        public XmlDocument OwnerDocument { get; private set; }

        /// <summary>
        /// Gets a boolean value indicating whether the current node has any attributes.
        /// </summary>
        /// <returns>
        /// true if the current node has attributes; otherwise, false.
        /// </returns>
        public bool HasAttributes { get { return _Attributes.Count > 0; } }


        /// <summary>
        /// Adds the specified node to the end of the list of child nodes, of this node.
        /// </summary>
        /// <param name="newChild"> The node to add. All the contents of the node to be added are moved into the specified location.</param>
        /// <returns>The node added.</returns>
        public virtual XmlElement AppendChild(XmlElement newChild)
        {
            newChild.Parent = this;
            this._Elements.Add(newChild);
            return newChild;
        }

        /// <summary>
        /// Returns the value for the attribute with the specified name.
        /// </summary>
        /// <param name="name"> The name of the attribute to retrieve. This is a qualified name. It is matched against the Name property of the matching node.</param>
        /// <returns>
        /// The value of the specified attribute. An empty string is returned if a matching
        ///  attribute is not found or if the attribute does not have a specified or default value.
        /// </returns>
        public string GetAttribute(string name)
        {
            XmlAttribute att = FindAttribute(name);
            if (att != null)
                return att.Value;
            else
                return null;
        }

        /// <summary>
        /// Sets the value of the attribute with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the attribute to create or alter. This is a qualified name. If
        /// the name contains a colon it is parsed into prefix and local name components.
        /// </param>
        /// <param name="value">The value to set for the attribute.</param>
        public void SetAttribute(string name, string value)
        {
            XmlAttribute att = FindAttribute(name);
            if (att != null)
                att.Value = value;
            else
            {
                att = new XmlAttribute(name, value);
                _Attributes.Add(att);
            }
        }

        /// <summary>
        /// Determines whether the current node has an attribute with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute to find. This is a qualified name. It is matched against the Name property of the matching node.</param>
        /// <returns>true if the current node has the specified attribute; otherwise, false.</returns>
        public bool HasAttribute(string name)
        {
            XmlAttribute att = FindAttribute(name);
            return att != null;
        }

        /// <summary>
        /// Removes all specified attributes and children of the current node. Default attributes are not removed.
        /// </summary>
        public void RemoveAll()
        {
            foreach (var e in _Elements)
                if (e != null) e.Parent = null;
            _Elements.Clear();
            RemoveAllAttributes();
        }

        /// <summary>
        /// Removes all specified attributes from the element. Default attributes are not removed.
        /// </summary>
        public void RemoveAllAttributes()
        {
            _Attributes.Clear();
        }

        /// <summary>
        /// Removes an attribute by name.
        /// </summary>
        /// <param name="name"> The name of the attribute to remove.This is a qualified name. It is matched against the Name property of the matching node.</param>
        public void RemoveAttribute(string name)
        {
            XmlAttribute att = FindAttribute(name);
            if (att != null) _Attributes.Remove(att);
        }

        internal XmlElement GetNextSibling(XmlElement xmlElement)
        {
            int index = _Elements.IndexOf(xmlElement);
            if (index >= 0 && index < _Elements.Count - 1)
                return _Elements[index + 1];
            else
                return null;
        }

        internal void ToString(StringBuilder buffer)
        {
            buffer.Append('<');
            buffer.Append(Name);
            foreach (var att in _Attributes)
            {
                buffer.Append(' ');
                buffer.Append(att.ToString());
            }
            buffer.Append(' ');

            if (_Elements.Count > 0)
            {
                buffer.Append('>');
                foreach (var child in _Elements)
                    child.ToString(buffer);

                buffer.Append("</");
                buffer.Append(Name);
                buffer.Append(" >");
            }
            else
                buffer.Append("/>");
        }

        public IEnumerator<XmlElement> GetEnumerator()
        {
            return _Elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Elements.GetEnumerator();
        }
    }
}
