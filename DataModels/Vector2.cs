using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
{
    public struct Vector2 : IXElement, IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return DataModels.PrimitiveType.Vector2; } }

        public float X, Y;

        public XElement ToXElement()
        {
            XElement v = new XElement("Vector2");
            v.SetAttributeValue("X", X);
            v.SetAttributeValue("Y", Y);
            return v;
        }

        public void Load(XElement e)
        {
            this.X = e.GetAttributeValueAsFloat("X", 0.0f);
            this.Y = e.GetAttributeValueAsFloat("Y", 0.0f);
        }
    }
}
