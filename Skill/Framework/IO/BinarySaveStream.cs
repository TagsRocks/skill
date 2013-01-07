using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Skill.Framework.IO
{
    /// <summary>
    /// Defines all methods required to save an ISavable object.
    /// you have to inherite from this class and Implement four write method (WriteInt, WriteFloat, WriteBoolean and WriteString ) based on target platform
    /// all other data types can be saved by these four method
    /// </summary>
    public abstract class BinarySaveStream
    {
        /// <summary>
        /// Write given int value to stream
        /// </summary>
        /// <param name="i">value to write</param>
        public abstract void Write(int i);
        /// <summary>
        /// Write given float value to stream
        /// </summary>
        /// <param name="f">value to write</param>
        public abstract void Write(float f);
        /// <summary>
        /// Write given bool value to stream
        /// </summary>
        /// <param name="b">value to write</param>
        public abstract void Write(bool b);
        /// <summary>
        /// Write given string value to stream
        /// </summary>
        /// <param name="s">value to write</param>
        public abstract void Write(string s);

        /// <summary>
        /// Write given bounds value to stream
        /// </summary>
        /// <param name="bound">value to write</param>
        public void Write(Bounds bound)
        {
            Write(bound.center);
            Write(bound.size);
        }

        /// <summary>
        /// Write given color value to stream
        /// </summary>
        /// <param name="color">value to write</param>
        public void Write(Color color)
        {
            Write(color.a);
            Write(color.b);
            Write(color.g);
            Write(color.r);
        }

        /// <summary>
        /// Write given Matrix4x4 value to stream
        /// </summary>
        /// <param name="matrix">value to write</param>
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

        /// <summary>
        /// Write given plane value to stream
        /// </summary>
        /// <param name="plane">value to write</param>
        public void Write(Plane plane)
        {
            Write(plane.normal);
            Write(plane.distance);
        }

        /// <summary>
        /// Write given quaternion value to stream
        /// </summary>
        /// <param name="quaternion">value to write</param>
        public void Write(Quaternion quaternion)
        {
            Write(quaternion.w);
            Write(quaternion.x);
            Write(quaternion.y);
            Write(quaternion.z);
        }

        /// <summary>
        /// Write given ray value to stream
        /// </summary>
        /// <param name="ray">value to write</param>
        public void Write(Ray ray)
        {
            Write(ray.origin);
            Write(ray.direction);
        }

        /// <summary>
        /// Write given rect value to stream
        /// </summary>
        /// <param name="rect">value to write</param>
        public void Write(Rect rect)
        {
            Write(rect.x);
            Write(rect.y);
            Write(rect.width);
            Write(rect.height);
        }

        /// <summary>
        /// Write given vector2 value to stream
        /// </summary>
        /// <param name="vector">value to write</param>
        public void Write(Vector2 vector)
        {
            Write(vector.x);
            Write(vector.y);
        }

        /// <summary>
        /// Write given vector3 value to stream
        /// </summary>
        /// <param name="vector">value to write</param>
        public void Write(Vector3 vector)
        {
            Write(vector.x);
            Write(vector.y);
            Write(vector.z);
        }

        /// <summary>
        /// Write given vector4 value to stream
        /// </summary>
        /// <param name="vector">value to write</param>
        public void Write(Vector4 vector)
        {
            Write(vector.w);
            Write(vector.x);
            Write(vector.y);
            Write(vector.z);
        }


        /// <summary>
        /// Write given ISavable value to stream
        /// </summary>
        /// <typeparam name="T">Type of ISavable</typeparam>
        /// <param name="savable">value to write</param>
        public void Write<T>(T savable) where T : ISavable
        {
            if (savable != null)
            {
                Write(true);
                savable.Save(this);
            }
            else
                Write(false);
        }

        /// <summary>
        /// Write given array of ISavable value to stream
        /// </summary>
        /// <typeparam name="T">Type of ISavable</typeparam>
        /// <param name="savables">array of ISavable to write</param>
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

        /// <summary>
        /// Write given array of int value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of float value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of bool value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of string value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of Bounds value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of Color value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of Matrix4x4 value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of Plane value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of Quaternion value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of Ray value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of Rect value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of Vector2 value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of Vector3 value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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

        /// <summary>
        /// Write given array of Vector4 value to stream
        /// </summary>
        /// <param name="primitives">array of int value to write</param>
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
