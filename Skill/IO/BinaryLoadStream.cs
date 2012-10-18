using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.IO
{
    /// <summary>
    /// Defines all methods required to load ISavable object.
    /// you have to inherite from this class and Implement four Read method (ReadInt, ReadFloat, ReadBoolean and ReadString ) based on target platform
    /// all other data types can be read by these four method
    /// </summary>
    public abstract class BinaryLoadStream
    {
        /// <summary>
        /// Read an int32 value for stream
        /// </summary>
        /// <returns>Int32</returns>
        public abstract int ReadInt();
        /// <summary>
        /// Read a float value for stream
        /// </summary>
        /// <returns>float</returns>
        public abstract float ReadFloat();
        /// <summary>
        /// Read a boolean value for stream
        /// </summary>
        /// <returns>bool</returns>
        public abstract bool ReadBoolean();
        /// <summary>
        /// Read a string value for stream
        /// </summary>
        /// <returns>string</returns>
        public abstract string ReadString();

        /// <summary>
        /// Read Bounds data from stream
        /// </summary>
        /// <returns>Bounds</returns>
        public Bounds ReadBounds()
        {
            Vector3 center = ReadVector3();
            Vector3 size = ReadVector3();

            return new Bounds(center, size);
        }

        /// <summary>
        /// Read Color data from stream
        /// </summary>
        /// <returns>Color</returns>
        public Color ReadColor()
        {
            float a = ReadFloat();
            float b = ReadFloat();
            float g = ReadFloat();
            float r = ReadFloat();

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Read Matrix4x4 data from stream
        /// </summary>
        /// <returns>Matrix4x4</returns>
        public Matrix4x4 ReadMatrix4x4()
        {
            Matrix4x4 matrix = new Matrix4x4();

            matrix.m00 = ReadFloat();
            matrix.m01 = ReadFloat();
            matrix.m02 = ReadFloat();
            matrix.m03 = ReadFloat();
            matrix.m10 = ReadFloat();
            matrix.m11 = ReadFloat();
            matrix.m12 = ReadFloat();
            matrix.m13 = ReadFloat();
            matrix.m20 = ReadFloat();
            matrix.m21 = ReadFloat();
            matrix.m22 = ReadFloat();
            matrix.m23 = ReadFloat();
            matrix.m30 = ReadFloat();
            matrix.m31 = ReadFloat();
            matrix.m32 = ReadFloat();
            matrix.m33 = ReadFloat();

            return matrix;
        }

        /// <summary>
        /// Read Plane data from stream
        /// </summary>
        /// <returns>Plane</returns>
        public Plane ReadPlane()
        {
            Vector3 normal = ReadVector3();
            float distance = ReadFloat();

            return new Plane(normal, distance);
        }

        /// <summary>
        /// Read Quaternion data from stream
        /// </summary>
        /// <returns>Quaternion</returns>
        public Quaternion ReadQuaternion()
        {
            float w = ReadFloat();
            float x = ReadFloat();
            float y = ReadFloat();
            float z = ReadFloat();

            return new Quaternion(x, y, z, w);
        }

        /// <summary>
        /// Read Ray data from stream
        /// </summary>
        /// <returns>Ray</returns>
        public Ray ReadRay()
        {
            Vector3 origin = ReadVector3();
            Vector3 direction = ReadVector3();

            return new Ray(origin, direction);
        }

        /// <summary>
        /// Read Rect data from stream
        /// </summary>
        /// <returns>Rect</returns>
        public Rect ReadRect()
        {
            float x = ReadFloat();
            float y = ReadFloat();
            float width = ReadFloat();
            float height = ReadFloat();

            return new Rect(x, y, width, height);
        }

        /// <summary>
        /// Read Vector2 data from stream
        /// </summary>
        /// <returns>Vector2</returns>
        public Vector2 ReadVector2()
        {
            float x = ReadFloat();
            float y = ReadFloat();

            return new Vector2(x, y);
        }

        /// <summary>
        /// Read Vector3 data from stream
        /// </summary>
        /// <returns>Vector3</returns>
        public Vector3 ReadVector3()
        {
            float x = ReadFloat();
            float y = ReadFloat();
            float z = ReadFloat();

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Read Vector4 data from stream
        /// </summary>
        /// <returns>Vector4</returns>
        public Vector4 ReadVector4()
        {
            float w = ReadFloat();
            float x = ReadFloat();
            float y = ReadFloat();
            float z = ReadFloat();

            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Read a Savable class from stream
        /// </summary>
        /// <typeparam name="T">Type of ISavable</typeparam>
        /// <param name="creator">Helper method to instantiate ISavable class</param>
        /// <returns>ISavable data</returns>
        public T ReadSavable<T>(CreateISavable<T> creator) where T : ISavable
        {
            bool isNotNull = ReadBoolean();
            T newItem = creator();
            if (isNotNull)
                newItem.Load(this);
            return newItem;
        }

        /// <summary>
        /// Read an array of Savable class from stream
        /// </summary>
        /// <typeparam name="T">Type of ISavable</typeparam>
        /// <param name="creator">Helper method to instantiate ISavable class</param>
        /// <returns>array of ISavable data</returns>
        public T[] ReadSavableArray<T>(CreateISavable<T> creator) where T : ISavable
        {
            int length = ReadInt();
            T[] array = new T[length];

            for (int i = 0; i < length; i++)
                array[i] = ReadSavable<T>(creator);

            return array;
        }

        /// <summary>
        /// Read array of int data from stream
        /// </summary>
        /// <returns>array of int</returns>
        public int[] ReadIntArray()
        {
            int length = ReadInt();
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadInt();
            }
            return array;
        }

        /// <summary>
        /// Read array of float data from stream
        /// </summary>
        /// <returns>array of float</returns>
        public float[] ReadFloatArray()
        {
            int length = ReadInt();
            float[] array = new float[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadFloat();
            }
            return array;
        }

        /// <summary>
        /// Read array of bool data from stream
        /// </summary>
        /// <returns>array of bool</returns>
        public bool[] ReadBooleanArray()
        {
            int length = ReadInt();
            bool[] array = new bool[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadBoolean();
            }
            return array;
        }

        /// <summary>
        /// Read array of string data from stream
        /// </summary>
        /// <returns>array of string</returns>
        public string[] ReadStringArray()
        {
            int length = ReadInt();
            string[] array = new string[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadString();
            }
            return array;
        }

        /// <summary>
        /// Read array of Bounds data from stream
        /// </summary>
        /// <returns>array of Bounds</returns>
        public Bounds[] ReadBoundsArray()
        {
            int length = ReadInt();
            Bounds[] array = new Bounds[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadBounds();
            }
            return array;
        }

        /// <summary>
        /// Read array of Color data from stream
        /// </summary>
        /// <returns>array of Color</returns>
        public Color[] ReadColorArray()
        {
            int length = ReadInt();
            Color[] array = new Color[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadColor();
            }
            return array;
        }

        /// <summary>
        /// Read array of Matrix4x4 data from stream
        /// </summary>
        /// <returns>array of Matrix4x4</returns>
        public Matrix4x4[] ReadMatrix4x4Array()
        {
            int length = ReadInt();
            Matrix4x4[] array = new Matrix4x4[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadMatrix4x4();
            }
            return array;
        }

        /// <summary>
        /// Read array of Plane data from stream
        /// </summary>
        /// <returns>array of Plane</returns>
        public Plane[] ReadPlaneArray()
        {
            int length = ReadInt();
            Plane[] array = new Plane[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadPlane();
            }
            return array;
        }

        /// <summary>
        /// Read array of Quaternion data from stream
        /// </summary>
        /// <returns>array of Quaternion</returns>
        public Quaternion[] ReadQuaternionArray()
        {
            int length = ReadInt();
            Quaternion[] array = new Quaternion[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadQuaternion();
            }
            return array;
        }

        /// <summary>
        /// Read array of Ray data from stream
        /// </summary>
        /// <returns>array of Ray</returns>
        public Ray[] ReadRayArray()
        {
            int length = ReadInt();
            Ray[] array = new Ray[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadRay();
            }
            return array;
        }

        /// <summary>
        /// Read array of Rect data from stream
        /// </summary>
        /// <returns>array of Rect</returns>
        public Rect[] ReadRectArray()
        {
            int length = ReadInt();
            Rect[] array = new Rect[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadRect();
            }
            return array;
        }

        /// <summary>
        /// Read array of Vector2 data from stream
        /// </summary>
        /// <returns>array of Vector2</returns>
        public Vector2[] ReadVector2Array()
        {
            int length = ReadInt();
            Vector2[] array = new Vector2[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadVector2();
            }
            return array;
        }

        /// <summary>
        /// Read array of Vector3 data from stream
        /// </summary>
        /// <returns>array of Vector3</returns>
        public Vector3[] ReadVector3Array()
        {
            int length = ReadInt();
            Vector3[] array = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadVector3();
            }
            return array;
        }

        /// <summary>
        /// Read array of Vector4 data from stream
        /// </summary>
        /// <returns>array of Vector4</returns>
        public Vector4[] ReadVector4Array()
        {
            int length = ReadInt();
            Vector4[] array = new Vector4[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadVector4();
            }
            return array;
        }
    }
}
