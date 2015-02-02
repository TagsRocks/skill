using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.IO
{
    /// <summary>
    /// defines a method to instantiate ISavable T
    /// </summary>
    /// <typeparam name="T">ISavable to instantiate</typeparam>
    /// <returns>A valid instance of ISavable T</returns>
    public delegate T CreateISavable<T>() where T : ISavable;

    /// <summary>
    /// This class provides all required methods to load an ISavable class from Xml format. 
    /// you do not need to use read methods of this class, Skill Studio takes care of it.
    /// </summary>
    /// <remarks>
    /// Because content of Xml format can be modfied by final users and slower than binary, Xml format is useful for debuging.
    /// Skill IO Xml saving algorithm designed to allow modify savable classes and still read from previous data (new data will be in default value).
    /// so in Xml mode (debug time) you can add or remove new variables in Savable class designed in Skill Studio, and load savable class from previous data ( from HDD or PlayerPrefs and ...).
    /// when your game is created and ready to release switch to binary format.
    /// </remarks>
    public sealed class XmlLoadStream
    {

        /// <summary>
        /// The document contains all saved data. save content of Document to file or everywhere as needed
        /// </summary>
        public XmlDocument Document { get; private set; }

        /// <summary>
        /// Create an instance of XmlSaveStream
        /// </summary>
        public XmlLoadStream(XmlDocument document)
        {
            this.Document = document;
        }

        /// <summary>
        /// Read data of given XmlElement as int
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>int</returns>
        public int ReadInt(XmlElement e)
        {
            string v = e.GetAttribute("Value");
            if (v != null)
            {
                int i;
                if (int.TryParse(v, out i))
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// Read data of given XmlElement as float
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>float</returns>
        public float ReadFloat(XmlElement e)
        {
            string v = e.GetAttribute("Value");
            if (v != null)
            {
                float f;
                if (float.TryParse(v, out f))
                    return f;
            }
            return 0.0f;
        }

        /// <summary>
        /// Read data of given XmlElement as boolean
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>bool</returns>
        public bool ReadBoolean(XmlElement e)
        {
            string v = e.GetAttribute("Value");
            if (v != null)
            {
                bool b;
                if (bool.TryParse(v, out b))
                    return b;
            }
            return false;
        }

        /// <summary>
        /// Read data of given XmlElement as string
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>string</returns>
        public string ReadString(XmlElement e)
        {
            string v = e.GetAttribute("Value");
            if (v == null)
                v = "";
            return v;
        }

        /// <summary>
        /// Read data of given XmlElement as Bounds
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>Bounds</returns>
        public Bounds ReadBounds(XmlElement e)
        {
            Vector3 center = new Vector3();
            Vector3 size = new Vector3();

            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "Center":
                        center = ReadVector3(element);
                        break;
                    case "Size":
                        size = ReadVector3(element);
                        break;
                }
                element = e.GetNextSibling(element);
            }

            return new Bounds(center, size);
        }

        /// <summary>
        /// Read data of given XmlElement as Color
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>Color</returns>
        public Color ReadColor(XmlElement e)
        {
            float a = 0, g = 0, b = 0, r = 0;

            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "A":
                        a = ReadFloat(element);
                        break;
                    case "B":
                        b = ReadFloat(element);
                        break;
                    case "G":
                        g = ReadFloat(element);
                        break;
                    case "R":
                        r = ReadFloat(element);
                        break;

                }
                element = e.GetNextSibling(element);
            }

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Read data of given XmlElement as Matrix4x4
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>Matrix4x4</returns>
        public Matrix4x4 ReadMatrix4x4(XmlElement e)
        {
            Matrix4x4 matrix = new Matrix4x4();

            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "M00":
                        matrix.m00 = ReadFloat(element);
                        break;
                    case "M01":
                        matrix.m01 = ReadFloat(element);
                        break;
                    case "M02":
                        matrix.m02 = ReadFloat(element);
                        break;
                    case "M03":
                        matrix.m03 = ReadFloat(element);
                        break;
                    case "M10":
                        matrix.m10 = ReadFloat(element);
                        break;
                    case "M11":
                        matrix.m11 = ReadFloat(element);
                        break;
                    case "M12":
                        matrix.m12 = ReadFloat(element);
                        break;
                    case "M13":
                        matrix.m13 = ReadFloat(element);
                        break;
                    case "M20":
                        matrix.m20 = ReadFloat(element);
                        break;
                    case "M21":
                        matrix.m21 = ReadFloat(element);
                        break;
                    case "M22":
                        matrix.m22 = ReadFloat(element);
                        break;
                    case "M23":
                        matrix.m23 = ReadFloat(element);
                        break;
                    case "M30":
                        matrix.m30 = ReadFloat(element);
                        break;
                    case "M31":
                        matrix.m31 = ReadFloat(element);
                        break;
                    case "M32":
                        matrix.m32 = ReadFloat(element);
                        break;
                    case "M33":
                        matrix.m33 = ReadFloat(element);
                        break;

                }
                element = e.GetNextSibling(element);
            }
            return matrix;
        }

        /// <summary>
        /// Read data of given XmlElement as Plane
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>Plane</returns>
        public Plane ReadPlane(XmlElement e)
        {

            Vector3 normal = new Vector3();
            float distance = 0;

            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "Normal":
                        normal = ReadVector3(element);
                        break;
                    case "Distance":
                        distance = ReadFloat(element);
                        break;
                }
                element = e.GetNextSibling(element);
            }

            return new Plane(normal, distance);
        }

        /// <summary>
        /// Read data of given XmlElement as Quaternion
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>Quaternion</returns>
        public Quaternion ReadQuaternion(XmlElement e)
        {
            Quaternion quaternion = new Quaternion();

            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "W":
                        quaternion.w = ReadFloat(element);
                        break;
                    case "X":
                        quaternion.x = ReadFloat(element);
                        break;
                    case "Y":
                        quaternion.y = ReadFloat(element);
                        break;
                    case "Z":
                        quaternion.z = ReadFloat(element);
                        break;
                }
                element = e.GetNextSibling(element);
            }
            return quaternion;
        }

        /// <summary>
        /// Read data of given XmlElement as Ray
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>Ray</returns>
        public Ray ReadRay(XmlElement e)
        {
            Vector3 origin = new Vector3();
            Vector3 direction = Vector3.forward;

            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "Origin":
                        origin = ReadVector3(element);
                        break;
                    case "Direction":
                        direction = ReadVector3(element);
                        break;
                }
                element = e.GetNextSibling(element);
            }
            return new Ray(origin, direction);
        }

        /// <summary>
        /// Read data of given XmlElement as Rect
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>Rect</returns>
        public Rect ReadRect(XmlElement e)
        {
            Rect rect = new Rect();

            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "X":
                        rect.x = ReadFloat(element);
                        break;
                    case "Y":
                        rect.y = ReadFloat(element);
                        break;
                    case "Width":
                        rect.width = ReadFloat(element);
                        break;
                    case "Height":
                        rect.height = ReadFloat(element);
                        break;
                }
                element = e.GetNextSibling(element);
            }
            return rect;
        }

        /// <summary>
        /// Read data of given XmlElement as Vector2
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>Vector2</returns>
        public Vector2 ReadVector2(XmlElement e)
        {
            Vector2 vector = new Vector2();
            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "X":
                        vector.x = ReadFloat(element);
                        break;
                    case "Y":
                        vector.y = ReadFloat(element);
                        break;
                }
                element = e.GetNextSibling(element);
            }
            return vector;
        }

        /// <summary>
        /// Read data of given XmlElement as Vector3
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>Vector3</returns>
        public Vector3 ReadVector3(XmlElement e)
        {
            Vector3 vector = new Vector3();
            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "X":
                        vector.x = ReadFloat(element);
                        break;
                    case "Y":
                        vector.y = ReadFloat(element);
                        break;
                    case "Z":
                        vector.z = ReadFloat(element);
                        break;
                }
                element = e.GetNextSibling(element);
            }
            return vector;
        }

        /// <summary>
        /// Read data of given XmlElement as Vector4
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>Vector4</returns>
        public Vector4 ReadVector4(XmlElement e)
        {
            Vector4 vector = new Vector4();
            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "W":
                        vector.w = ReadFloat(element);
                        break;
                    case "X":
                        vector.x = ReadFloat(element);
                        break;
                    case "Y":
                        vector.y = ReadFloat(element);
                        break;
                    case "Z":
                        vector.z = ReadFloat(element);
                        break;
                }
                element = e.GetNextSibling(element);
            }
            return vector;
        }

        /// <summary>
        /// Read data of given XmlElement as specified ISavable
        /// </summary>                
        /// <typeparam name="T">type of ISavable to read</typeparam>
        /// <param name="e">XmlElement contains data</param>
        /// <param name="creator">Helper method for instantiation of T</param>
        /// <returns>Instance of T</returns>
        public T ReadSavable<T>(XmlElement e, CreateISavable<T> creator) where T : ISavable
        {
            T newItem = creator();
            newItem.Load(e, this);
            return newItem;
        }

        /// <summary>
        /// Read data of given XmlElement as array of specified ISavable
        /// </summary>                
        /// <typeparam name="T">type of ISavable to read</typeparam>
        /// <param name="e">XmlElement contains data</param>
        /// <param name="creator">Helper method for instantiation of T</param>
        /// <returns>array of instance of T</returns>
        public T[] ReadSavableArray<T>(XmlElement e, CreateISavable<T> creator) where T : ISavable
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            T[] array = new T[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                if (element.Name != XmlSaveStream.NoData)
                {
                    T newItem = creator();
                    newItem.Load(element, this);
                    array[index] = newItem;
                }
                element = e.GetNextSibling(element);
                index++;
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of int
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of int</returns>
        public int[] ReadIntArray(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            int[] array = new int[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadInt(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of float
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of float</returns>
        public float[] ReadFloatArray(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            float[] array = new float[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadFloat(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of boolean
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of bool</returns>
        public bool[] ReadBooleanArray(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            bool[] array = new bool[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadBoolean(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of string
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of string</returns>
        public string[] ReadStringArray(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            string[] array = new string[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadString(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of Bounds
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of Bounds</returns>
        public Bounds[] ReadBoundsArray(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            Bounds[] array = new Bounds[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadBounds(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of Color
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of Color</returns>
        public Color[] ReadColorArray(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            Color[] array = new Color[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadColor(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of Matrix4x4
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of Matrix4x4</returns>
        public Matrix4x4[] ReadMatrix4x4Array(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            Matrix4x4[] array = new Matrix4x4[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadMatrix4x4(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of Plane
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of Plane</returns>
        public Plane[] ReadPlaneArray(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            Plane[] array = new Plane[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadPlane(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of Quaternion
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of Quaternion</returns>
        public Quaternion[] ReadQuaternionArray(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            Quaternion[] array = new Quaternion[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadQuaternion(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of Ray
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of Ray</returns>
        public Ray[] ReadRayArray(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            Ray[] array = new Ray[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadRay(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of Rect
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of Rect</returns>
        public Rect[] ReadRectArray(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            Rect[] array = new Rect[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadRect(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of Vector2
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of Vector2</returns>
        public Vector2[] ReadVector2Array(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            Vector2[] array = new Vector2[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadVector2(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of Vector3
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of Vector3</returns>
        public Vector3[] ReadVector3Array(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            Vector3[] array = new Vector3[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadVector3(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }

        /// <summary>
        /// Read data of given XmlElement as array of Vector4
        /// </summary>
        /// <param name="e">XmlElement contains data</param>
        /// <returns>array of Vector4</returns>
        public Vector4[] ReadVector4Array(XmlElement e)
        {
            string lenghtStr = e.GetAttribute("Lenght");
            int length = 0;
            if (!int.TryParse(lenghtStr, out length))
                length = 0;
            Vector4[] array = new Vector4[length];

            XmlElement element = e.FirstChild as XmlElement;
            int index = 0;
            while (element != null)
            {
                array[index++] = ReadVector4(element);
                element = e.GetNextSibling(element);
            }
            return array;
        }
    }
}
