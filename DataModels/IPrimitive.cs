using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels
{
    /// <summary>
    /// Defines Primitive Types
    /// </summary>
    public enum PrimitiveType
    {
        Int,
        Float,
        Bool,
        String,

        Bounds,
        Color,
        Matrix4x4,
        Plane,
        Quaternion,
        Ray,
        Rect,
        Vector2,
        Vector3,
        Vector4,
    }

    /// <summary>
    /// instance of this class contains Primitive data
    /// </summary>
    public interface IPrimitive
    {
        PrimitiveType PrimitiveType { get; }
    }
}
