using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Skill.IO
{
    public class XmlSaveStream
    {
        public XmlDocument Document { get; private set; }

        public XmlSaveStream()
        {
            Document = new XmlDocument();
        }

        public XmlSaveStream(ISavableClass savable)
        {
            Document = new XmlDocument();
            XmlDeclaration xmlDeclaration = Document.CreateXmlDeclaration("1.0", "utf-8", null);

            XmlElement rootNode = savable.ToXmlElement(this);
            Document.InsertBefore(xmlDeclaration, Document.DocumentElement);
            Document.AppendChild(rootNode);
        }

        public XmlElement Create(string propertyName, int i)
        {
            XmlElement e = Document.CreateElement(propertyName);
            e.SetAttribute("Value", i.ToString());
            return e;
        }

        public XmlElement Create(string propertyName, float f)
        {
            XmlElement e = Document.CreateElement(propertyName);
            e.SetAttribute("Value", f.ToString());
            return e;
        }

        public XmlElement Create(string propertyName, bool b)
        {
            XmlElement e = Document.CreateElement(propertyName);
            e.SetAttribute("Value", b.ToString());
            return e;
        }
        public XmlElement Create(string propertyName, string s)
        {
            XmlElement e = Document.CreateElement(propertyName);
            if (s == null) s = "";
            e.SetAttribute("Value", s.ToString());
            return e;
        }

        public XmlElement Create(string propertyName, Bounds bound)
        {
            XmlElement e = Document.CreateElement(propertyName);
            XmlElement center = Create("Center", bound.center);
            XmlElement size = Create("Size", bound.size);
            e.AppendChild(center);
            e.AppendChild(size);
            return e;
        }
        public XmlElement Create(string propertyName, Color color)
        {
            XmlElement e = Document.CreateElement(propertyName);

            XmlElement a = Create("A", color.a);
            XmlElement b = Create("B", color.b);
            XmlElement g = Create("G", color.g);
            XmlElement r = Create("R", color.r);

            e.AppendChild(a);
            e.AppendChild(b);
            e.AppendChild(g);
            e.AppendChild(r);

            return e;
        }
        public XmlElement Create(string propertyName, Matrix4x4 matrix)
        {
            XmlElement e = Document.CreateElement(propertyName);

            XmlElement m00 = Create("M00", matrix.m00);
            XmlElement m01 = Create("M01", matrix.m01);
            XmlElement m02 = Create("M02", matrix.m02);
            XmlElement m03 = Create("M03", matrix.m03);

            XmlElement m10 = Create("M10", matrix.m10);
            XmlElement m11 = Create("M11", matrix.m11);
            XmlElement m12 = Create("M12", matrix.m12);
            XmlElement m13 = Create("M13", matrix.m13);

            XmlElement m20 = Create("M20", matrix.m20);
            XmlElement m21 = Create("M21", matrix.m21);
            XmlElement m22 = Create("M22", matrix.m22);
            XmlElement m23 = Create("M23", matrix.m23);

            XmlElement m30 = Create("M30", matrix.m30);
            XmlElement m31 = Create("M31", matrix.m31);
            XmlElement m32 = Create("M32", matrix.m32);
            XmlElement m33 = Create("M33", matrix.m33);

            e.AppendChild(m00);
            e.AppendChild(m01);
            e.AppendChild(m02);
            e.AppendChild(m03);

            e.AppendChild(m10);
            e.AppendChild(m11);
            e.AppendChild(m12);
            e.AppendChild(m13);

            e.AppendChild(m20);
            e.AppendChild(m21);
            e.AppendChild(m22);
            e.AppendChild(m23);

            e.AppendChild(m30);
            e.AppendChild(m31);
            e.AppendChild(m32);
            e.AppendChild(m33);

            return e;
        }

        public XmlElement Create(string propertyName, Plane plane)
        {
            XmlElement e = Document.CreateElement(propertyName);

            XmlElement normal = Create("Normal", plane.normal);
            XmlElement distance = Create("Distance", plane.distance);

            e.AppendChild(normal);
            e.AppendChild(distance);

            return e;
        }

        public XmlElement Create(string propertyName, Quaternion quaternion)
        {
            XmlElement e = Document.CreateElement(propertyName);

            XmlElement w = Create("W", quaternion.w);
            XmlElement x = Create("X", quaternion.x);
            XmlElement y = Create("Y", quaternion.y);
            XmlElement z = Create("Z", quaternion.z);

            e.AppendChild(w);
            e.AppendChild(x);
            e.AppendChild(y);
            e.AppendChild(z);

            return e;
        }
        public XmlElement Create(string propertyName, Ray ray)
        {
            XmlElement e = Document.CreateElement(propertyName);

            XmlElement origin = Create("Origin", ray.origin);
            XmlElement direction = Create("Direction", ray.direction);

            e.AppendChild(origin);
            e.AppendChild(direction);

            return e;
        }

        public XmlElement Create(string propertyName, Rect rect)
        {
            XmlElement e = Document.CreateElement(propertyName);

            XmlElement x = Create("X", rect.x);
            XmlElement y = Create("Y", rect.y);
            XmlElement width = Create("Width", rect.width);
            XmlElement height = Create("Height", rect.height);

            e.AppendChild(x);
            e.AppendChild(y);
            e.AppendChild(width);
            e.AppendChild(height);

            return e;
        }
        public XmlElement Create(string propertyName, Vector2 vector)
        {
            XmlElement e = Document.CreateElement(propertyName);

            XmlElement x = Create("X", vector.x);
            XmlElement y = Create("Y", vector.y);

            e.AppendChild(x);
            e.AppendChild(y);

            return e;
        }
        public XmlElement Create(string propertyName, Vector3 vector)
        {
            XmlElement e = Document.CreateElement(propertyName);

            XmlElement x = Create("X", vector.x);
            XmlElement y = Create("Y", vector.y);
            XmlElement z = Create("Z", vector.z);

            e.AppendChild(x);
            e.AppendChild(y);
            e.AppendChild(z);

            return e;
        }
        public XmlElement Create(string propertyName, Vector4 vector)
        {
            XmlElement e = Document.CreateElement(propertyName);

            XmlElement w = Create("W", vector.x);
            XmlElement x = Create("X", vector.x);
            XmlElement y = Create("Y", vector.y);
            XmlElement z = Create("Z", vector.z);

            e.AppendChild(w);
            e.AppendChild(x);
            e.AppendChild(y);
            e.AppendChild(z);

            return e;
        }

        public XmlElement Create(string propertyName, IEnumerable<ISavableClass> savables)
        {
            XmlElement e = Document.CreateElement(propertyName);

            foreach (var item in savables)
            {
                if (item != null)
                {
                    XmlElement element = item.ToXmlElement(this);
                    e.AppendChild(element);
                }
            }
            return e;
        }


    }
}
