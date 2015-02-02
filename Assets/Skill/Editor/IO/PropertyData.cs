using Skill.Framework.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Editor.IO
{
    /// <summary>
    /// Defines type of property for SaveClass
    /// </summary>
    public enum PropertyType
    {
        Primitive,
        Class
    }

    /// <summary>
    /// Defines base class for SaveClass properties
    /// </summary>
    public abstract class PropertyData : IXmlElementSerializable
    {
        /// <summary>  Type of property </summary>
        public abstract PropertyType Type { get; }
        /// <summary>  Name of property in SaveClass </summary>
        public string Name { get; set; }
        /// <summary>  Whether this property is array</summary>
        public bool IsArray { get; set; }
        /// <summary>  Comment of property </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Create a SaveProperty
        /// </summary>
        /// <param name="name"></param>
        public PropertyData(string name)
        {
            this.Name = name;
        }


        public XmlElement ToXmlElement()
        {

            XmlElement p = new XmlElement("Property");
            p.SetAttributeValue("PropertyType", (int)Type);
            p.SetAttributeValue("Name", Name);
            p.SetAttributeValue("IsArray", IsArray);            
            if (!string.IsNullOrEmpty(Comment))
            {
                XmlElement comment = new XmlElement("Comment");
                comment.Value = Comment;
                p.AppendChild(comment);
            }
            WriteAttributes(p);

            return p;
        }
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XmlElement e) { }

        public void Load(XmlElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            this.IsArray = e.GetAttributeValueAsBoolean("IsArray", false);
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
    }


    /// <summary>
    /// A property of Primitive types
    /// </summary>
    public class PrimitivePropertyData : PropertyData
    {
        /// <summary>
        /// Type of property
        /// </summary>
        public override PropertyType Type { get { return PropertyType.Primitive; } }

        /// <summary> PrimitiveType </summary>
        public PrimitiveDataType PrimitiveType { get; set; }

        /// <summary>
        /// Create a PrimitiveProperty
        /// </summary>
        public PrimitivePropertyData()
            : base("NewPrimitiveProperty")
        {

        }

        protected override void WriteAttributes(XmlElement e)
        {
            e.SetAttributeValue("PrimitiveType", (int)this.PrimitiveType);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            this.PrimitiveType = (PrimitiveDataType)e.GetAttributeValueAsInt("PrimitiveType", 0);
            base.ReadAttributes(e);
        }
    }

    /// <summary>
    /// A property of type Savable class
    /// </summary>
    public class ClassPropertyData : PropertyData
    {
        /// <summary>
        /// Type of property
        /// </summary>
        public override PropertyType Type { get { return PropertyType.Class; } }

        /// <summary> Name of class that this property refer to </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Create a ClassProperty
        /// </summary>
        public ClassPropertyData()
            : base("NewClassProperty")
        {

        }

        protected override void WriteAttributes(XmlElement e)
        {
            e.SetAttributeValue("ClassName", this.ClassName);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            this.ClassName = e.GetAttributeValueAsString("ClassName", "");
            base.ReadAttributes(e);
        }


    }
}
