using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
{
    public struct Quaternion : IXElement, IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return DataModels.PrimitiveType.Quaternion; } }

        public float W;
        public float X;
        public float Y;
        public float Z;

        public XElement ToXElement()
        {
            XElement v = new XElement("Quaternion");
            v.SetAttributeValue("W", W);
            v.SetAttributeValue("X", X);
            v.SetAttributeValue("Y", Y);
            v.SetAttributeValue("Z", Z);            
            return v;
        }

        public void Load(XElement e)
        {
            this.W = e.GetAttributeValueAsFloat("W", 0.0f);
            this.X = e.GetAttributeValueAsFloat("X", 0.0f);
            this.Y = e.GetAttributeValueAsFloat("Y", 0.0f);
            this.Z = e.GetAttributeValueAsFloat("Z", 0.0f);            
        }
    }
}
