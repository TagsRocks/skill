using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
{
    public struct Plane : IXElement, IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return DataModels.PrimitiveType.Plane; } }

        public float Distance;
        public Vector3 Normal;

        public XElement ToXElement()
        {
            XElement p = new XElement("Plane");
            p.SetAttributeValue("Distance", Distance);
            p.Add(Normal.ToXElement());
            return p;
        }

        public void Load(XElement e)
        {
            this.Distance = e.GetAttributeValueAsFloat("Distance", 0.0f);
            XElement normal = e.FindChildByName("Vector3");
            if (normal != null)
            {
                this.Normal.Load(normal);
            }
        }
    }
}
