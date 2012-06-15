using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.IO
{
    public class SaveClass : IXElement
    {
        public string Name { get; set; }
        public SaveProperty[] Properties { get; set; }

        public SaveClass()
            :this("NewClass")
        {

        }

        public SaveClass(string name)
        {
            this.Name = name;
        }

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
            WriteAttributes(saveclass);
            return saveclass;
        }
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XElement e) { }

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
            ReadAttributes(e);
        }

        /// <summary>
        /// subclass can load additional data here
        /// </summary>
        /// <param name="e">contains behavior data</param>
        protected virtual void ReadAttributes(XElement e) { }

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
