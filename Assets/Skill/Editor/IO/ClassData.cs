using Skill.Framework.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Editor.IO
{
    /// <summary>
    /// Defines a class with properties
    /// </summary>
    public class ClassData : IXmlElementSerializable
    {
        /// <summary> Name of Class </summary>
        public string Name { get; set; }
        /// <summary> Comment of Class </summary>
        public string Comment { get; set; }
        /// <summary> properties of Class </summary>
        public PropertyData[] Properties { get; set; }
        /// <summary> Is struct? </summary>
        public bool IsStruct { get; set; }

        /// <summary>
        /// Create an instance of SaveClass
        /// </summary>
        public ClassData()
            : this("NewClass")
        {

        }

        /// <summary>
        /// Create an instance of SaveClass
        /// </summary>
        /// <param name="name">Name of class</param>
        public ClassData(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Convert data in XmlElement to save in file
        /// </summary>
        /// <returns>XmlElement</returns>
        public XmlElement ToXmlElement()
        {
            XmlElement saveclass = new XmlElement("SaveClass");
            saveclass.SetAttributeValue("Name", Name != null ? Name : "");
            saveclass.SetAttributeValue("IsStruct", IsStruct);
            if (this.Properties != null)
            {
                XmlElement properties = new XmlElement("Properties");
                properties.SetAttributeValue("Count", this.Properties.Length);
                foreach (var item in this.Properties)
                {
                    properties.AppendChild(item.ToXmlElement());
                }
                saveclass.AppendChild(properties);
            }

            if (!string.IsNullOrEmpty(Comment))
            {
                XmlElement comment = new XmlElement("Comment");
                comment.Value = Comment;
                saveclass.AppendChild(comment);
            }

            WriteAttributes(saveclass);
            return saveclass;
        }
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XmlElement e) { }

        /// <summary>
        /// Load data for Xelement that loaded from file
        /// </summary>
        /// <param name="e">XmlElement containing data</param>
        public void Load(XmlElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            this.IsStruct = e.GetAttributeValueAsBoolean("IsStruct", false);
            XmlElement properties = e["Properties"];
            if (properties != null)
            {
                int count = properties.GetAttributeValueAsInt("Count", 0);
                this.Properties = new PropertyData[count];
                int i = 0;
                foreach (var element in properties)
                {
                    PropertyData p = CreateProperty(element);
                    if (p != null)
                    {
                        p.Load(element);
                        this.Properties[i++] = p;
                    }
                }
            }

            XmlElement comment = e["Comment"];
            if (comment != null)
            {
                Comment = comment.Value;
            }

            ReadAttributes(e);
        }

        /// <summary>
        /// subclass can load additional data here
        /// </summary>
        /// <param name="e">contains behavior data</param>
        protected virtual void ReadAttributes(XmlElement e) { }

        /// <summary>
        /// Create a SaveProperty from given XmlElement
        /// </summary>
        /// <param name="e">XmlElement</param>
        /// <returns>SaveProperty</returns>
        private static PropertyData CreateProperty(XmlElement e)
        {
            PropertyData result = null;

            int type = e.GetAttributeValueAsInt("PropertyType", -1);
            if (type > -1)
            {
                return CreateProperty((PropertyType)type);
            }

            return result;
        }

        /// <summary>
        /// Create a SaveProperty by type
        /// </summary>
        /// <param name="type">Type of property</param>
        /// <returns>SaveProperty</returns>
        public static PropertyData CreateProperty(PropertyType type)
        {
            switch (type)
            {
                case PropertyType.Primitive:
                    return new PrimitivePropertyData();
                case PropertyType.Class:
                    return new ClassPropertyData();
                default:
                    return null;
            }
        }
    }
}
