using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.IO
{

    public class XmlDocument : IEnumerable<XmlElement>
    {

        class XmlDocumentContentHandler : SmallXmlParser.IContentHandler
        {
            private XmlDocument _Document;
            private Stack<XmlElement> _Stack;

            public XmlDocumentContentHandler(XmlDocument document)
            {
                this._Document = document;
                _Stack = new Stack<XmlElement>();
            }

            public void OnStartParsing(SmallXmlParser parser)
            {
                _Stack.Clear();
            }

            public void OnEndParsing(SmallXmlParser parser)
            {

            }

            public void OnStartElement(string name, SmallXmlParser.IAttrList attrs)
            {
                XmlElement element = _Document.CreateElement(name);
                for (int i = 0; i < attrs.Length; i++)
                    element.SetAttribute(attrs.Names[i], attrs.Values[i]);

                if (_Stack.Count > 0)
                    _Stack.Peek().AppendChild(element);
                else
                    _Document.AppendChild(element);

                _Stack.Push(element);
            }

            public void OnEndElement(string name)
            {
                if (_Stack.Count > 0 && _Stack.Peek().Name == name)
                    _Stack.Pop();
                else
                    throw new Exception("Invalid xml content");
            }

            public void OnProcessingInstruction(string name, string text)
            {
                if (name == "xml")
                    this._Document.SetDeclaration(text);
            }

            public void OnChars(string text)
            {

            }

            public void OnIgnorableWhitespace(string text)
            {

            }
        }

        private List<XmlElement> _Elements;
        private string _Declaration;

        public XmlDocument()
        {
            _Elements = new List<XmlElement>();
            SetDeclaration("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
        }

        internal void SetDeclaration(string declaration)
        {
            _Declaration = declaration;
        }

        private XmlElement FindElement(string name)
        {
            foreach (var e in _Elements)
            {
                if (e.Name == name)
                    return e;
            }
            return null;
        }

        /// <summary>
        /// Gets the first child element with the specified Name.
        /// </summary>
        /// <param name="name"> The qualified name of the element to retrieve. </param>
        /// <returns> The first XmlElement that matches the specified name. </returns>
        public XmlElement this[string name] { get { return FindElement(name); } }

        /// <summary>
        /// Gets the markup representing this node and all its child nodes.
        /// </summary>
        public string OuterXml
        {
            get
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append(_Declaration); // append declaration
                foreach (var e in _Elements)
                {
                    e.ToString(buffer);
                }
                return buffer.ToString();
            }
        }

        /// <summary>
        /// Creates an element with the specified name.
        /// </summary>
        /// <param name="name"> The qualified name of the element. </param>
        /// <returns> The new XmlElement. </returns>
        public XmlElement CreateElement(string name)
        {
            return new XmlElement(this, name);
        }

        /// <summary>
        /// Adds the specified node to the end of the list of child nodes, of this node.
        /// </summary>
        /// <param name="newChild"> The node to add. All the contents of the node to be added are moved into the specified location.</param>
        /// <returns>The node added.</returns>
        public XmlElement AppendChild(XmlElement newChild)
        {
            this._Elements.Add(newChild);
            return newChild;
        }

        /// <summary>
        /// Loads the XML document from the specified string.
        /// </summary>
        /// <param name="xml">String containing the XML document to load.</param>
        public void LoadXml(string xml)
        {
            this._Elements.Clear();
            SmallXmlParser parser = new SmallXmlParser();
            XmlDocumentContentHandler contentHandler = new XmlDocumentContentHandler(this);
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            parser.Parse(reader, contentHandler);
            reader.Close();
        }

        internal XmlElement GetNextSibling(XmlElement xmlElement)
        {
            int index = _Elements.IndexOf(xmlElement);
            if (index >= 0 && index < _Elements.Count - 1)
                return _Elements[index + 1];
            else
                return null;
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
