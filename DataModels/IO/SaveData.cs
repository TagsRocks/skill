using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.IO
{
    public class SaveData : SaveClass
    {
        public SaveClass[] Classes { get; set; }

        public SaveData()
            : base("NewSaveData")
        {

        }

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
