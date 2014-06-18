using System;
using UnityEngine;

namespace Skill.Framework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CurveEditorAttribute : Attribute
    {
        public Color Color { get; private set; }
        public string Name { get; private set; }
        public CurveEditorAttribute(float r, float g, float b, string name)
        {
            this.Color = new UnityEngine.Color(r, g, b);
            this.Name = name;
        }
    }
}
