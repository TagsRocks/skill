using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Skill.IO
{
    public delegate T CreateISavable<T>() where T : ISavableClass;

    public class XmlLoadStream
    {

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

        public string ReadString(XmlElement e)
        {
            string v = e.GetAttribute("Value");
            if (v == null)
                v = "";
            return v;
        }

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
                element = element.NextSibling as XmlElement;
            }

            return new Bounds(center, size);
        }
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
                element = element.NextSibling as XmlElement;
            }

            return new Color(r, g, b, a);
        }
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
                element = element.NextSibling as XmlElement;
            }
            return matrix;
        }

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
                element = element.NextSibling as XmlElement;
            }

            return new Plane(normal, distance);
        }

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
                element = element.NextSibling as XmlElement;
            }
            return quaternion;
        }
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
                element = element.NextSibling as XmlElement;
            }
            return new Ray(origin, direction);
        }

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
                element = element.NextSibling as XmlElement;
            }
            return rect;
        }
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
                element = element.NextSibling as XmlElement;
            }
            return vector;
        }
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
                element = element.NextSibling as XmlElement;
            }
            return vector;
        }
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
                element = element.NextSibling as XmlElement;
            }
            return vector;
        }

        public List<T> ReadList<T>(XmlElement e, CreateISavable<T> creator) where T : ISavableClass
        {
            List<T> list = new List<T>();

            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                T newItem = creator();
                newItem.Load(element);
                list.Add(newItem);
                element = element.NextSibling as XmlElement;
            }
            return list;
        }

        public List<T> ReadList<T>(XmlElement e)
        {
            Type parameterType = typeof(T);
            System.Collections.IList list = null;

            if (parameterType == typeof(int))
                list = new List<int>();
            else if (parameterType == typeof(float))
                list = new List<float>();
            else if (parameterType == typeof(bool))
                list = new List<bool>();
            else if (parameterType == typeof(string))
                list = new List<string>();
            else if (parameterType == typeof(Vector2))
                list = new List<Vector2>();
            else if (parameterType == typeof(Vector3))
                list = new List<Vector3>();
            else if (parameterType == typeof(Vector4))
                list = new List<Vector4>();
            else if (parameterType == typeof(Bounds))
                list = new List<Bounds>();
            else if (parameterType == typeof(Color))
                list = new List<Color>();
            else if (parameterType == typeof(Matrix4x4))
                list = new List<Matrix4x4>();
            else if (parameterType == typeof(Plane))
                list = new List<Plane>();
            else if (parameterType == typeof(Quaternion))
                list = new List<Quaternion>();
            else if (parameterType == typeof(Ray))
                list = new List<Ray>();
            else if (parameterType == typeof(Rect))
                list = new List<Rect>();


            XmlElement element = e.FirstChild as XmlElement;
            while (element != null)
            {
                if (parameterType == typeof(int))
                    ((List<int>)list).Add(ReadInt(element));
                else if (parameterType == typeof(float))
                    ((List<float>)list).Add(ReadFloat(element));
                else if (parameterType == typeof(bool))
                    ((List<bool>)list).Add(ReadBoolean(element));
                else if (parameterType == typeof(string))
                    ((List<string>)list).Add(ReadString(element));
                else if (parameterType == typeof(Vector2))
                    ((List<Vector2>)list).Add(ReadVector2(element));
                else if (parameterType == typeof(Vector3))
                    ((List<Vector3>)list).Add(ReadVector3(element));
                else if (parameterType == typeof(Vector4))
                    ((List<Vector4>)list).Add(ReadVector4(element));
                else if (parameterType == typeof(Bounds))
                    ((List<Bounds>)list).Add(ReadBounds(element));
                else if (parameterType == typeof(Color))
                    ((List<Color>)list).Add(ReadColor(element));
                else if (parameterType == typeof(Matrix4x4))
                    ((List<Matrix4x4>)list).Add(ReadMatrix4x4(element));
                else if (parameterType == typeof(Plane))
                    ((List<Plane>)list).Add(ReadPlane(element));
                else if (parameterType == typeof(Quaternion))
                    ((List<Quaternion>)list).Add(ReadQuaternion(element));
                else if (parameterType == typeof(Ray))
                    ((List<Ray>)list).Add(ReadRay(element));
                else if (parameterType == typeof(Rect))
                    ((List<Rect>)list).Add(ReadRect(element));

                element = element.NextSibling as XmlElement;
            }

            return (List<T>)list;
        }
    }
}
