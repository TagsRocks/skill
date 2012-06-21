using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.IO
{
    /// <summary>
    /// Containes definitions of classes and properties to save
    /// this data structure designed ti simplify process of save data in games
    /// by defining this types of structure user must fill data in SaveData and call save method
    /// the implemented SaveData take care of save and load in efficient and fast way
    /// </summary>
    public class SaveData : SaveClass
    {
        /// <summary>
        /// List of Savable classes
        /// </summary>
        public SaveClass[] Classes { get; set; }

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
        protected override void ReadAttributes(XElement e)
        {
            XElement classes = e.FindChildByName("Classes");
            if (classes != null)
            {
                int count = classes.GetAttributeValueAsInt("Count", 0);
                int i = 0;
                this.Classes = new SaveClass[count];
                foreach (var element in classes.Elements())
                {
                    SaveClass c = new SaveClass();
                    c.Load(element);
                    this.Classes[i++] = c;
                }
            }
            base.ReadAttributes(e);
        }

        /// <summary>
        /// Add data to given XElement to save in file
        /// </summary>
        /// <param name="e">XElement to save data</param>
        protected override void WriteAttributes(XElement e)
        {
            if (Classes != null)
            {
                XElement classes = new XElement("Classes");
                classes.SetAttributeValue("Count", Classes.Length);
                foreach (var item in this.Classes)
                {
                    classes.Add(item.ToXElement());
                }
                e.Add(classes);
            }
            base.WriteAttributes(e);
        }
    }
}
