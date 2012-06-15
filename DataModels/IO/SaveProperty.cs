using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.IO
{
    public enum PropertyType
    {
        Primitive,
        Class
    }

    public enum CollectionType
    {
        None,
        List,
        Array
    }

    public abstract class SaveProperty : IXElement
    {
        public abstract PropertyType Type { get; }
        public string Name { get; set; }
        public CollectionType CollectionType { get; set; }
        public int ArrayLength { get; set; }
        public string Comment { get; set; }

        public SaveProperty(string name)
        {
            this.Name = name;
        }

        public XElement ToXElement()
        {

            XElement p = new XElement("Property");
            p.SetAttributeValue("PropertyType", (int)Type);
            p.SetAttributeValue("Name", Name);
            p.SetAttributeValue("CollectionType", (int)CollectionType);
            p.SetAttributeValue("ArrayLength", ArrayLength);
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
            this.CollectionType = (CollectionType)e.GetAttributeValueAsInt("CollectionType", 0);
            this.ArrayLength = e.GetAttributeValueAsInt("ArrayLength", 0);
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

    public class PrimitiveProperty : SaveProperty
    {
        public override PropertyType Type { get { return PropertyType.Primitive; } }
        public PrimitiveType PrimitiveType { get; set; }

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

    public class ClassProperty : SaveProperty
    {
        public override PropertyType Type { get { return PropertyType.Class; } }
        public string ClassName { get; set; }

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
