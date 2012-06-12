using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
{
    public struct Ray : IXElement, IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return DataModels.PrimitiveType.Ray; } }

        public Vector3 Direction;
        public Vector3 Origin;

        public XElement ToXElement()
        {
            XElement p = new XElement("Ray");

            XElement direction = Direction.ToXElement();
            XElement origin = Origin.ToXElement();

            direction.SetAttributeValue("Name", "Direction");
            origin.SetAttributeValue("Name", "Origin");

            p.Add(direction);
            p.Add(origin);
            return p;
        }

        public void Load(XElement e)
        {
            foreach (var element in e.Elements())
            {
                string name = element.GetAttributeValueAsString("Name", "");
                switch (name)
                {
                    case "Direction":
                        this.Direction.Load(element);
                        break;
                    case "Origin":
                        this.Origin.Load(element);
                        break;
                }
            }
        }
    }
}
