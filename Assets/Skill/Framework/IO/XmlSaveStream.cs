using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.IO
{

    /// <summary>
    /// This class provides all required methods to save an ISavable class in Xml format. 
    /// you do not need to use read methods of this class, Skill Studio takes care of it.
    /// </summary>
    /// <remarks>
    /// Because content of Xml format can be modfied by final users and slower than binary, Xml format is useful for debuging.
    /// Skill IO Xml saving algorithm designed to allow modify savable classes and still read from previous data (new data will be in default value).
    /// so in Xml mode (debug time) you can add or remove new variables in Savable class designed in Skill Studio, and load savable class from previous data ( from HDD or PlayerPrefs and ...).
    /// when your game is created and ready to release switch to binary format.
    /// </remarks>
    public sealed class XmlSaveStream
    {
        /// <summary>
        /// when an instance of ISavable class is null the content of Xml element will be NoData
        /// </summary>
        public const string NoData = "___NoData___";

        /// <summary>
        /// The document contains all saved data. save content of Document to file or everywhere as needed
        /// </summary>
        public XmlDocument Document { get; private set; }

        /// <summary>
        /// Create an instance of XmlSaveStream
        /// </summary>
        public XmlSaveStream()
        {
            Document = new XmlDocument();
        }

        /// <summary>
        /// Create an instance of XmlSaveStream and save given ISavable
        /// </summary>
        /// <param name="savable">ISavable to save</param>
        public XmlSaveStream(ISavable savable)
        {
            Document = new XmlDocument();
            XmlElement rootNode = new XmlElement("SaveData");
            savable.Save(rootNode, this);
            Document.AppendChild(rootNode);
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="i">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, int i)
        {
            XmlElement e = new XmlElement(propertyName);
            e.SetAttributeValue("Value", i.ToString());
            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="f">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, float f)
        {
            XmlElement e = new XmlElement(propertyName);
            e.SetAttributeValue("Value", f.ToString());
            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="b">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, bool b)
        {
            XmlElement e = new XmlElement(propertyName);
            e.SetAttributeValue("Value", b.ToString());
            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="s">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, string s)
        {
            XmlElement e = new XmlElement(propertyName);
            if (s == null) s = "";
            e.SetAttributeValue("Value", s.ToString());
            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="bound">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Bounds bound)
        {
            XmlElement e = new XmlElement(propertyName);
            XmlElement center = Create("Center", bound.center);
            XmlElement size = Create("Size", bound.size);
            e.AppendChild(center);
            e.AppendChild(size);
            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="color">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Color color)
        {
            XmlElement e = new XmlElement(propertyName);

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

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="matrix">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Matrix4x4 matrix)
        {
            XmlElement e = new XmlElement(propertyName);

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

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="plane">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Plane plane)
        {
            XmlElement e = new XmlElement(propertyName);

            XmlElement normal = Create("Normal", plane.normal);
            XmlElement distance = Create("Distance", plane.distance);

            e.AppendChild(normal);
            e.AppendChild(distance);

            return e;
        }
        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="quaternion">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Quaternion quaternion)
        {
            XmlElement e = new XmlElement(propertyName);

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

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="ray">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Ray ray)
        {
            XmlElement e = new XmlElement(propertyName);

            XmlElement origin = Create("Origin", ray.origin);
            XmlElement direction = Create("Direction", ray.direction);

            e.AppendChild(origin);
            e.AppendChild(direction);

            return e;
        }
        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="rect">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Rect rect)
        {
            XmlElement e = new XmlElement(propertyName);

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

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="vector">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Vector2 vector)
        {
            XmlElement e = new XmlElement(propertyName);

            XmlElement x = Create("X", vector.x);
            XmlElement y = Create("Y", vector.y);

            e.AppendChild(x);
            e.AppendChild(y);

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="vector">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Vector3 vector)
        {
            XmlElement e = new XmlElement(propertyName);

            XmlElement x = Create("X", vector.x);
            XmlElement y = Create("Y", vector.y);
            XmlElement z = Create("Z", vector.z);

            e.AppendChild(x);
            e.AppendChild(y);
            e.AppendChild(z);

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="vector">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Vector4 vector)
        {
            XmlElement e = new XmlElement(propertyName);

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

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <typeparam name="T">Type of ISavable</typeparam>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="savable">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create<T>(string propertyName, T savable) where T : ISavable
        {
            XmlElement e = new XmlElement(propertyName);
            if (savable != null)
                savable.Save(e, this);
            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <typeparam name="T">Type of ISavable</typeparam>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="savables">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create<T>(string propertyName, T[] savables) where T : ISavable
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (savables != null) length = savables.Length;
            e.SetAttributeValue("Lenght", length.ToString());
            for (int i = 0; i < length; i++)
            {
                XmlElement element = null;
                if (savables[i] != null)
                    element = Create<T>(string.Format("Item{0}", i), savables[i]);
                else
                    element = new XmlElement(NoData);
                e.AppendChild(element);
            }
            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, int[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, float[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, bool[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, string[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Bounds[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Color[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Matrix4x4[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Plane[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Quaternion[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Ray[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Rect[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Vector2[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Vector3[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }

        /// <summary>
        /// Create an XmlElement containing property data
        /// </summary>
        /// <param name="propertyName">Name of property (XmlElement)</param>
        /// <param name="primitives">data to save</param>
        /// <returns>XmlElement containig data</returns>
        public XmlElement Create(string propertyName, Vector4[] primitives)
        {
            XmlElement e = new XmlElement(propertyName);
            int length = 0;
            if (primitives != null) length = primitives.Length;
            e.SetAttributeValue("Lenght", length.ToString());

            for (int i = 0; i < length; i++)
            {
                XmlElement element = Create("Element" + i, primitives[i]);
                e.AppendChild(element);
            }

            return e;
        }
    }
}
