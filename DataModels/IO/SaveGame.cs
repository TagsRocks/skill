using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.IO
{
    public class SaveGame : SaveClass
    {
        public List<SaveClass> Classes { get; private set; }

        public SaveGame()
        {
            this.Classes = new List<SaveClass>();
        }

        protected override void ReadAttributes(XElement e)
        {
            this.Clear();
            XElement classes = e.FindChildByName("Classes");
            if (classes != null)
            {
                foreach (var element in classes.Elements())
                {
                    SaveClass c = new SaveClass();
                    c.Load(element);
                    this.Classes.Add(c);
                }
            }
            base.ReadAttributes(e);
        }

        protected override void WriteAttributes(XElement e)
        {
            XElement classes = new XElement("Classes");
            foreach (var item in this.Classes)
            {
                classes.Add(item.ToXElement());
            }
            e.Add(classes);

            e.Add(classes);
            base.WriteAttributes(e);
        }
    }
}
