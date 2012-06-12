using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels
{
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

    public interface IPrimitive
    {
        PrimitiveType PrimitiveType { get; }
    }
}
