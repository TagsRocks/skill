using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.IO
{
    /// <summary>
    /// Defines a class with properties
    /// </summary>
    public class SaveClass : IXElement
    {
        /// <summary> Name of Class </summary>
        public string Name { get; set; }
        /// <summary> Comment of Class </summary>
        public string Comment { get; set; }
        /// <summary> properties of Class </summary>
        public SaveProperty[] Properties { get; set; }

        /// <summary>
        /// Create an instance of SaveClass
        /// </summary>
        public SaveClass()
            :this("NewClass")
        {

        }

        /// <summary>
        /// Create an instance of SaveClass
        /// </summary>
        /// <param name="name">Name of class</param>
        public SaveClass(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Convert data in XElement to save in file
        /// </summary>
        /// <returns>XElement</returns>
        public XElement ToXElement()
        {
            XElement saveclass = new XElement("SaveClass");
            saveclass.SetAttributeValue("Name", Name != null ? Name : "");
            if (this.Properties != null)
            {
                XElement properties = new XElement("Properties");
                properties.SetAttributeValue("Count", this.Properties.Length);
                foreach (var item in this.Properties)
                {
                    properties.Add(item.ToXElement());
                }
                saveclass.Add(properties);
            }

            if (!string.IsNullOrEmpty(Comment))
            {
                XElement comment = new XElement("Comment");
                comment.SetValue(Comment);
                saveclass.Add(comment);
            }

            WriteAttributes(saveclass);
            return saveclass;
        }
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XElement e) { }

        /// <summary>
        /// Load data for Xelement that loaded from file
        /// </summary>
        /// <param name="e">XElement containing data</param>
        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            XElement properties = e.FindChildByName("Properties");
            if (properties != null)
            {
                int count = properties.GetAttributeValueAsInt("Count", 0);
                this.Properties = new SaveProperty[count];
                int i = 0;
                foreach (var element in properties.Elements())
                {
                    SaveProperty p = CreateProperty(element);
                    if (p != null)
                    {
                        p.Load(element);
                        this.Properties[i++] = p;
                    }
                }
            }

            XElement comment = e.FindChildByName("Comment");
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
        protected virtual void ReadAttributes(XElement e) { }

        /// <summary>
        /// Create a SaveProperty from given XElement
        /// </summary>
        /// <param name="e">XElement</param>
        /// <returns>SaveProperty</returns>
        private static SaveProperty CreateProperty(XElement e)
        {
            SaveProperty result = null;

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
        public static SaveProperty CreateProperty(PropertyType type)
        {
            switch (type)
            {
                case PropertyType.Primitive:
                    return new PrimitiveProperty();
                case PropertyType.Class:
                    return new ClassProperty();
                default:
                    return null;
            }
        }        
    }
}
