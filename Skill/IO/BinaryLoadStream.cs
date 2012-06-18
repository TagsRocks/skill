using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.IO
{
    public abstract class BinaryLoadStream
    {
        public abstract int ReadInt();
        public abstract float ReadFloat();
        public abstract bool ReadBoolean();
        public abstract string ReadString();

        public Bounds ReadBounds()
        {
            Vector3 center = ReadVector3();
            Vector3 size = ReadVector3();

            return new Bounds(center, size);
        }
        public Color ReadColor()
        {
            float a = ReadFloat();
            float b = ReadFloat();
            float g = ReadFloat();
            float r = ReadFloat();

            return new Color(r, g, b, a);
        }
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
        public Plane ReadPlane()
        {
            Vector3 normal = ReadVector3();
            float distance = ReadFloat();

            return new Plane(normal, distance);
        }
        public Quaternion ReadQuaternion()
        {
            float w = ReadFloat();
            float x = ReadFloat();
            float y = ReadFloat();
            float z = ReadFloat();

            return new Quaternion(x, y, z, w);
        }

        public Ray ReadRay()
        {
            Vector3 origin = ReadVector3();
            Vector3 direction = ReadVector3();

            return new Ray(origin, direction);
        }
        public Rect ReadRect()
        {
            float x = ReadFloat();
            float y = ReadFloat();
            float width = ReadFloat();
            float height = ReadFloat();

            return new Rect(x, y, width, height);
        }
        public Vector2 ReadVector2()
        {
            float x = ReadFloat();
            float y = ReadFloat();

            return new Vector2(x, y);
        }
        public Vector3 ReadVector3()
        {
            float x = ReadFloat();
            float y = ReadFloat();
            float z = ReadFloat();

            return new Vector3(x, y, z);
        }
        public Vector4 ReadVector4()
        {
            float w = ReadFloat();
            float x = ReadFloat();
            float y = ReadFloat();
            float z = ReadFloat();

            return new Vector4(x, y, z, w);
        }

        public T ReadSavable<T>(CreateISavable<T> creator) where T : ISavable
        {
            T newItem = creator();
            newItem.Load(this);
            return newItem;
        }

        public T[] ReadSavableArray<T>(CreateISavable<T> creator) where T : ISavable
        {
            int length = ReadInt();
            T[] array = new T[length];

            for (int i = 0; i < length; i++)
            {
                T newItem = creator();
                newItem.Load(this);
                array[i] = newItem;
            }
            return array;
        }

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
