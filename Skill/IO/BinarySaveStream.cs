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
    }
}
