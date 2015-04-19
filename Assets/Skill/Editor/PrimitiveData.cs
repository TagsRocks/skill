using UnityEngine;
using System.Collections;
using Skill.Framework.IO;

namespace Skill.Editor
{
    /// <summary>
    /// Defines Primitive Types
    /// </summary>
    public enum PrimitiveDataType
    {
        Integer,
        Float,
        Boolean,
        String,

        Bounds,
        Color,
        Matrix4x4,
        Plane,
        Quaternion,
        Ray,
        Rect,
        Vector2,
        Vector3,
        Vector4,
    }

    /// <summary>
    /// instance of this class contains Primitive data
    /// </summary>
    public interface IPrimitiveData
    {
        PrimitiveDataType PrimitiveType { get; }
    }

    public struct RectData : IXmlElementSerializable, IPrimitiveData
    {
        public PrimitiveDataType PrimitiveType { get { return PrimitiveDataType.Rect; } }

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public XmlElement ToXmlElement()
        {
            XmlElement r = new XmlElement("Rect");
            r.SetAttributeValue("X", X);
            r.SetAttributeValue("Y", Y);
            r.SetAttributeValue("Width", Width);
            r.SetAttributeValue("Height", Height);
            return r;
        }

        public void Load(XmlElement e)
        {
            this.X = e.GetAttributeValueAsFloat("X", 0.0f);
            this.Y = e.GetAttributeValueAsFloat("Y", 0.0f);
            this.Width = e.GetAttributeValueAsFloat("Width", 0.0f);
            this.Height = e.GetAttributeValueAsFloat("Height", 0.0f);
        }
    }

    public struct BoundsData : IXmlElementSerializable, IPrimitiveData
    {
        public PrimitiveDataType PrimitiveType { get { return PrimitiveDataType.Bounds; } }

        public Vector3Data Center;
        public Vector3Data Size;

        public XmlElement ToXmlElement()
        {
            XmlElement p = new XmlElement("Bounds");

            XmlElement center = Center.ToXmlElement();
            XmlElement size = Size.ToXmlElement();

            center.SetAttributeValue("Name", "Center");
            size.SetAttributeValue("Name", "Size");

            p.AppendChild(center);
            p.AppendChild(size);
            return p;
        }

        public void Load(XmlElement e)
        {
            foreach (var element in e)
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

    public struct RayData : IXmlElementSerializable, IPrimitiveData
    {
        public PrimitiveDataType PrimitiveType { get { return PrimitiveDataType.Ray; } }

        public Vector3Data Direction;
        public Vector3Data Origin;

        public XmlElement ToXmlElement()
        {
            XmlElement p = new XmlElement("Ray");

            XmlElement direction = Direction.ToXmlElement();
            XmlElement origin = Origin.ToXmlElement();

            direction.SetAttributeValue("Name", "Direction");
            origin.SetAttributeValue("Name", "Origin");

            p.AppendChild(direction);
            p.AppendChild(origin);
            return p;
        }

        public void Load(XmlElement e)
        {
            foreach (var element in e)
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
    public struct ColorData : IXmlElementSerializable, IPrimitiveData
    {
        public PrimitiveDataType PrimitiveType { get { return PrimitiveDataType.Color; } }

        public float A;
        public float B;
        public float G;
        public float R;


        public XmlElement ToXmlElement()
        {
            XmlElement v = new XmlElement("Color");
            v.SetAttributeValue("A", A);
            v.SetAttributeValue("B", B);
            v.SetAttributeValue("G", G);
            v.SetAttributeValue("R", R);
            return v;
        }

        public void Load(XmlElement e)
        {
            this.A = e.GetAttributeValueAsFloat("A", 0.0f);
            this.B = e.GetAttributeValueAsFloat("B", 0.0f);
            this.G = e.GetAttributeValueAsFloat("G", 0.0f);
            this.R = e.GetAttributeValueAsFloat("R", 0.0f);
        }
    }

    public struct Color32Data : IXmlElementSerializable
    {
        public byte A;
        public byte B;
        public byte G;
        public byte R;

        public XmlElement ToXmlElement()
        {
            XmlElement v = new XmlElement("Color32");
            v.SetAttributeValue("A", A);
            v.SetAttributeValue("B", B);
            v.SetAttributeValue("G", G);
            v.SetAttributeValue("R", R);
            return v;
        }

        public void Load(XmlElement e)
        {
            this.A = (byte)e.GetAttributeValueAsInt("A", 0);
            this.B = (byte)e.GetAttributeValueAsInt("B", 0);
            this.G = (byte)e.GetAttributeValueAsInt("G", 0);
            this.R = (byte)e.GetAttributeValueAsInt("R", 0);
        }
    }
    public struct Vector2Data : IXmlElementSerializable, IPrimitiveData
    {
        public PrimitiveDataType PrimitiveType { get { return PrimitiveDataType.Vector2; } }

        public float X, Y;

        public XmlElement ToXmlElement()
        {
            XmlElement v = new XmlElement("Vector2");
            v.SetAttributeValue("X", X);
            v.SetAttributeValue("Y", Y);
            return v;
        }

        public void Load(XmlElement e)
        {
            this.X = e.GetAttributeValueAsFloat("X", 0.0f);
            this.Y = e.GetAttributeValueAsFloat("Y", 0.0f);
        }

        public void Import(Vector2 v2)
        {
            this.X = v2.x;
            this.Y = v2.y;            
        }

        public Vector2 ToVector2()
        {
            return new Vector2(this.X, this.Y);
        }
    }

    public struct PlaneData : IXmlElementSerializable, IPrimitiveData
    {
        public PrimitiveDataType PrimitiveType { get { return PrimitiveDataType.Plane; } }

        public float Distance;
        public Vector3Data Normal;

        public XmlElement ToXmlElement()
        {
            XmlElement p = new XmlElement("Plane");
            p.SetAttributeValue("Distance", Distance);
            p.AppendChild(Normal.ToXmlElement());
            return p;
        }

        public void Load(XmlElement e)
        {
            this.Distance = e.GetAttributeValueAsFloat("Distance", 0.0f);
            XmlElement normal = e["Vector3"];
            if (normal != null)
            {
                this.Normal.Load(normal);
            }
        }
    }

    public struct QuaternionData : IXmlElementSerializable, IPrimitiveData
    {
        public PrimitiveDataType PrimitiveType { get { return PrimitiveDataType.Quaternion; } }

        public float W;
        public float X;
        public float Y;
        public float Z;

        public XmlElement ToXmlElement()
        {
            XmlElement v = new XmlElement("Quaternion");
            v.SetAttributeValue("W", W);
            v.SetAttributeValue("X", X);
            v.SetAttributeValue("Y", Y);
            v.SetAttributeValue("Z", Z);
            return v;
        }

        public void Load(XmlElement e)
        {
            this.W = e.GetAttributeValueAsFloat("W", 0.0f);
            this.X = e.GetAttributeValueAsFloat("X", 0.0f);
            this.Y = e.GetAttributeValueAsFloat("Y", 0.0f);
            this.Z = e.GetAttributeValueAsFloat("Z", 0.0f);
        }

        public void Import(Quaternion q)
        {
            this.X = q.x;
            this.Y = q.y;
            this.Z = q.z;
            this.W = q.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(this.X, this.Y, this.Z, this.W);
        }
    }

    public struct Matrix4x4Data : IXmlElementSerializable, IPrimitiveData
    {
        public PrimitiveDataType PrimitiveType { get { return PrimitiveDataType.Matrix4x4; } }

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

        public XmlElement ToXmlElement()
        {
            XmlElement v = new XmlElement("Matrix4x4");
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

        public void Load(XmlElement e)
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

    public struct Vector3Data : IXmlElementSerializable, IPrimitiveData
    {
        public PrimitiveDataType PrimitiveType { get { return PrimitiveDataType.Vector3; } }

        public float X, Y, Z;

        public XmlElement ToXmlElement()
        {
            XmlElement v = new XmlElement("Vector3");
            v.SetAttributeValue("X", X);
            v.SetAttributeValue("Y", Y);
            v.SetAttributeValue("Z", Z);
            return v;
        }

        public void Load(XmlElement e)
        {
            this.X = e.GetAttributeValueAsFloat("X", 0.0f);
            this.Y = e.GetAttributeValueAsFloat("Y", 0.0f);
            this.Z = e.GetAttributeValueAsFloat("Z", 0.0f);
        }


        public void Import(Vector3 v3)
        {
            this.X = v3.x;
            this.Y = v3.y;
            this.Z = v3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(this.X, this.Y, this.Z);
        }
    }

    public struct Vector4Data : IXmlElementSerializable, IPrimitiveData
    {
        public PrimitiveDataType PrimitiveType { get { return PrimitiveDataType.Vector4; } }

        public float X, Y, Z, W;

        public XmlElement ToXmlElement()
        {
            XmlElement v = new XmlElement("Vector4");
            v.SetAttributeValue("X", X);
            v.SetAttributeValue("Y", Y);
            v.SetAttributeValue("Z", Z);
            v.SetAttributeValue("W", W);
            return v;
        }

        public void Load(XmlElement e)
        {
            this.X = e.GetAttributeValueAsFloat("X", 0.0f);
            this.Y = e.GetAttributeValueAsFloat("Y", 0.0f);
            this.Z = e.GetAttributeValueAsFloat("Z", 0.0f);
            this.W = e.GetAttributeValueAsFloat("W", 0.0f);
        }


        public void Import(Vector4 v4)
        {
            this.X = v4.x;
            this.Y = v4.y;
            this.Z = v4.z;
            this.W = v4.w;
        }

        public Vector4 ToVector3()
        {
            return new Vector4(this.X, this.Y, this.Z, this.W);
        }

    }
}