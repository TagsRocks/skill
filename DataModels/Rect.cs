using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
{
    public struct Rect : IXElement, IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return DataModels.PrimitiveType.Rect; } }

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public XElement ToXElement()
        {
            XElement r = new XElement("Rect");
            r.SetAttributeValue("X", X);
            r.SetAttributeValue("Y", Y);
            r.SetAttributeValue("Width", Width);
            r.SetAttributeValue("Height", Height);
            return r;
        }

        public void Load(XElement e)
        {
            this.X = e.GetAttributeValueAsFloat("X", 0.0f);
            this.Y = e.GetAttributeValueAsFloat("Y", 0.0f);
            this.Width = e.GetAttributeValueAsFloat("Width", 0.0f);
            this.Height = e.GetAttributeValueAsFloat("Height", 0.0f);
        }
    }
}
