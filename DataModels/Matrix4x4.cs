using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
{
    public struct Matrix4x4 : IXElement, IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return DataModels.PrimitiveType.Matrix4x4; } }

        public float M00;
        public float M01;
        public float M02;
        public float M03;
        public float M10;
        public float M11;
        public float M12;
        public float M13;
        public float M20;
        public float M21;
        public float M22;
        public float M23;
        public float M30;
        public float M31;
        public float M32;
        public float M33;

        public XElement ToXElement()
        {
            XElement v = new XElement("Matrix4x4");

            v.SetAttributeValue("M00", M00);
            v.SetAttributeValue("M01", M01);
            v.SetAttributeValue("M02", M02);
            v.SetAttributeValue("M03", M03);

            v.SetAttributeValue("M10", M10);
            v.SetAttributeValue("M11", M11);
            v.SetAttributeValue("M12", M12);
            v.SetAttributeValue("M13", M13);

            v.SetAttributeValue("M20", M20);
            v.SetAttributeValue("M21", M21);
            v.SetAttributeValue("M22", M22);
            v.SetAttributeValue("M23", M23);

            v.SetAttributeValue("M30", M00);
            v.SetAttributeValue("M31", M01);
            v.SetAttributeValue("M32", M02);
            v.SetAttributeValue("M33", M03);

            return v;
        }

        public void Load(XElement e)
        {
            this.M00 = e.GetAttributeValueAsFloat("M00", 0.0f);
            this.M01 = e.GetAttributeValueAsFloat("M01", 0.0f);
            this.M02 = e.GetAttributeValueAsFloat("M02", 0.0f);
            this.M03 = e.GetAttributeValueAsFloat("M03", 0.0f);

            this.M10 = e.GetAttributeValueAsFloat("M10", 0.0f);
            this.M11 = e.GetAttributeValueAsFloat("M11", 0.0f);
            this.M12 = e.GetAttributeValueAsFloat("M12", 0.0f);
            this.M13 = e.GetAttributeValueAsFloat("M13", 0.0f);

            this.M20 = e.GetAttributeValueAsFloat("M20", 0.0f);
            this.M21 = e.GetAttributeValueAsFloat("M21", 0.0f);
            this.M22 = e.GetAttributeValueAsFloat("M22", 0.0f);
            this.M23 = e.GetAttributeValueAsFloat("M23", 0.0f);

            this.M30 = e.GetAttributeValueAsFloat("M30", 0.0f);
            this.M31 = e.GetAttributeValueAsFloat("M31", 0.0f);
            this.M32 = e.GetAttributeValueAsFloat("M32", 0.0f);
            this.M33 = e.GetAttributeValueAsFloat("M33", 0.0f);
        }
    }
}
