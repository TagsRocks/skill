using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
{
    public struct Bounds : IXElement, IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return DataModels.PrimitiveType.Bounds; } }

        public Vector3 Center;
        public Vector3 Size;

        public XElement ToXElement()
        {
            XElement p = new XElement("Bounds");

            XElement center = Center.ToXElement();
            XElement size = Size.ToXElement();

            center.SetAttributeValue("Name", "Center");
            size.SetAttributeValue("Name", "Size");

            p.Add(center);
            p.Add(size);
            return p;
        }

        public void Load(XElement e)
        {
            foreach (var element in e.Elements())
            {
                string name = element.GetAttributeValueAsString("Name", "");
                switch (name)
                {
                    case "Center":
                        this.Center.Load(element);
                        break;
                    case "Size":
                        this.Size.Load(element);
                        break;
                }
            }
        }
    }
}
