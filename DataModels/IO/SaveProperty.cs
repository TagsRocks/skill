using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.IO
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
    public abstract class SaveProperty : IXElement
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
        public SaveProperty(string name)
        {
            this.Name = name;
        }


        public XElement ToXElement()
        {

            XElement p = new XElement("Property");
            p.SetAttributeValue("PropertyType", (int)Type);
            p.SetAttributeValue("Name", Name);
            p.SetAttributeValue("IsArray", IsArray);            
            if (!string.IsNullOrEmpty(Comment))
            {
                XElement comment = new XElement("Comment");
                comment.SetValue(Comment);
                p.Add(comment);
            }
            WriteAttributes(p);

            return p;
        }
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XElement e) { }

        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            this.IsArray = e.GetAttributeValueAsBoolean("IsArray", false);            
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
    }


    /// <summary>
    /// A property of Primitive types
    /// </summary>
    public class PrimitiveProperty : SaveProperty
    {
        /// <summary>
        /// Type of property
        /// </summary>
        public override PropertyType Type { get { return PropertyType.Primitive; } }

        /// <summary> PrimitiveType </summary>
        public PrimitiveType PrimitiveType { get; set; }

        /// <summary>
        /// Create a PrimitiveProperty
        /// </summary>
        public PrimitiveProperty()
            : base("NewPrimitiveProperty")
        {

        }

        protected override void WriteAttributes(XElement e)
        {
            e.SetAttributeValue("PrimitiveType", (int)this.PrimitiveType);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            this.PrimitiveType = (DataModels.PrimitiveType)e.GetAttributeValueAsInt("PrimitiveType", 0);
            base.ReadAttributes(e);
        }
    }

    /// <summary>
    /// A property of type Savable class
    /// </summary>
    public class ClassProperty : SaveProperty
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
        public ClassProperty()
            : base("NewClassProperty")
        {

        }

        protected override void WriteAttributes(XElement e)
        {
            e.SetAttributeValue("ClassName", this.ClassName);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            this.ClassName = e.GetAttributeValueAsString("ClassName", "");
            base.ReadAttributes(e);
        }


    }
}
