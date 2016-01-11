using Skill.Framework.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Editor.IO
{
    /// <summary>
    /// Containes definitions of classes and properties to save
    /// this data structure designed ti simplify process of save data in games
    /// by defining this types of structure user must fill data in SaveData and call save method
    /// the implemented SaveData take care of save and load in efficient and fast way
    /// </summary>
    public class SaveData : ClassData
    {
        public static SaveData GeneratingInstance { get; set; }


        /// <summary> Namespace  </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// List of Savable classes
        /// </summary>
        public ClassData[] Classes { get; set; }

        /// <summary>
        /// Create an instance of SaveData
        /// </summary>
        public SaveData()
            : base("NewSaveData")
        {

        }

        /// <summary>
        /// Read data from given element that loaded from file
        /// </summary>
        /// <param name="e">Xelement containing data</param>
        protected override void ReadAttributes(XmlElement e)
        {
            Namespace = e.GetAttributeValueAsString("Namespace", string.Empty);
            XmlElement classes = e["Classes"];
            if (classes != null)
            {
                int count = classes.GetAttributeValueAsInt("Count", 0);
                int i = 0;
                this.Classes = new ClassData[count];
                foreach (var element in classes)
                {
                    ClassData c = new ClassData();
                    c.Load(element);
                    this.Classes[i++] = c;
                }
            }
            base.ReadAttributes(e);
        }

        /// <summary>
        /// Add data to given XmlElement to save in file
        /// </summary>
        /// <param name="e">XmlElement to save data</param>
        protected override void WriteAttributes(XmlElement e)
        {
            e.SetAttributeValue("Namespace", this.Namespace);
            if (Classes != null)
            {
                XmlElement classes = new XmlElement("Classes");
                classes.SetAttributeValue("Count", Classes.Length);
                foreach (var item in this.Classes)
                {
                    classes.AppendChild(item.ToXmlElement());
                }
                e.AppendChild(classes);
            }
            base.WriteAttributes(e);
        }
    }
}
