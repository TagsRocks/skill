using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
{
    public struct Color : IXElement, IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return DataModels.PrimitiveType.Color; } }

        public float A;
        public float B;
        public float G;
        public float R;


        public XElement ToXElement()
        {
            XElement v = new XElement("Color");
            v.SetAttributeValue("A", A);
            v.SetAttributeValue("B", B);
            v.SetAttributeValue("G", G);
            v.SetAttributeValue("R", R);
            return v;
        }

        public void Load(XElement e)
        {
            this.A = e.GetAttributeValueAsFloat("A", 0.0f);
            this.B = e.GetAttributeValueAsFloat("B", 0.0f);
            this.G = e.GetAttributeValueAsFloat("G", 0.0f);
            this.R = e.GetAttributeValueAsFloat("R", 0.0f);
        }
    }

    public struct Color32 : IXElement
    {
        public byte A;
        public byte B;
        public byte G;
        public byte R;

        public XElement ToXElement()
        {
            XElement v = new XElement("Color32");
            v.SetAttributeValue("A", A);
            v.SetAttributeValue("B", B);
            v.SetAttributeValue("G", G);
            v.SetAttributeValue("R", R);
            return v;
        }

        public void Load(XElement e)
        {
            this.A = (byte)e.GetAttributeValueAsInt("A", 0);
            this.B = (byte)e.GetAttributeValueAsInt("B", 0);
            this.G = (byte)e.GetAttributeValueAsInt("G", 0);
            this.R = (byte)e.GetAttributeValueAsInt("R", 0);
        }
    }
}
