using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Skill.IO
{
    public abstract class BinarySaveStream
    {
        public abstract void Write(int i);
        public abstract void Write(float f);
        public abstract void Write(bool b);
        public abstract void Write(string s);

        public void Write(Bounds bound)
        {
            Write(bound.center);
            Write(bound.size);
        }
        public void Write(Color color)
        {
            Write(color.a);
            Write(color.b);
            Write(color.g);
            Write(color.r);
        }
        public void Write(Matrix4x4 matrix)
        {
            Write(matrix.m00);
            Write(matrix.m01);
            Write(matrix.m02);
            Write(matrix.m03);
            Write(matrix.m10);
            Write(matrix.m11);
            Write(matrix.m12);
            Write(matrix.m13);
            Write(matrix.m20);
            Write(matrix.m21);
            Write(matrix.m22);
            Write(matrix.m23);
            Write(matrix.m30);
            Write(matrix.m31);
            Write(matrix.m32);
            Write(matrix.m33);
        }
        public void Write(Plane plane)
        {
            Write(plane.normal);
            Write(plane.distance);
        }
        public void Write(Quaternion quaternion)
        {
            Write(quaternion.w);
            Write(quaternion.x);
            Write(quaternion.y);
            Write(quaternion.z);
        }
        public void Write(Ray ray)
        {
            Write(ray.origin);
            Write(ray.direction);
        }
        public void Write(Rect rect)
        {
            Write(rect.x);
            Write(rect.y);
            Write(rect.width);
            Write(rect.height);
        }
        public void Write(Vector2 vector)
        {
            Write(vector.x);
            Write(vector.y);
        }
        public void Write(Vector3 vector)
        {
            Write(vector.x);
            Write(vector.y);
            Write(vector.z);
        }
        public void Write(Vector4 vector)
        {
            Write(vector.w);
            Write(vector.x);
            Write(vector.y);
            Write(vector.z);
        }

        public void Write<T>(T savable) where T : ISavable
        {
            savable.Save(this);
        }

        public void Write<T>(T[] savables) where T : ISavable
        {
            int length = 0;
            if (savables != null)
                length = savables.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(savables[i]);
            }
        }

        public void Write(int[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(float[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(bool[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(string[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(Bounds[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(Color[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(Matrix4x4[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(Plane[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(Quaternion[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(Ray[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(Rect[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(Vector2[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(Vector3[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
        public void Write(Vector4[] primitives)
        {
            int length = 0;
            if (primitives != null)
                length = primitives.Length;
            Write(length);
            for (int i = 0; i < length; i++)
            {
                Write(primitives[i]);
            }
        }
    }
}
