using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
{
    public struct Vector4 : IXElement, IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return DataModels.PrimitiveType.Vector4; } }

        public float X, Y, Z, W;

        public XElement ToXElement()
        {
            XElement v = new XElement("Vector4");
            v.SetAttributeValue("X", X);
            v.SetAttributeValue("Y", Y);
            v.SetAttributeValue("Z", Z);
            v.SetAttributeValue("W", W);
            return v;
        }

        public void Load(XElement e)
        {
            this.X = e.GetAttributeValueAsFloat("X", 0.0f);
            this.Y = e.GetAttributeValueAsFloat("Y", 0.0f);
            this.Z = e.GetAttributeValueAsFloat("Z", 0.0f);
            this.W = e.GetAttributeValueAsFloat("W", 0.0f);
        }

        
    }
}
