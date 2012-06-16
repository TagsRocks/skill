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
    }
}
