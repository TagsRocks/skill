using System;
using System.Collections.Generic;
using Skill.DataModels.Animation;
using System.Text;

namespace Skill.CodeGeneration.CSharp
{
    class SkinMeshClass : Class
    {
        public SkinMeshClass(SkinMesh mesh)
            : base(mesh.Name)
        {
            IsPartial = false;
            KeyframesClass keyframes = new KeyframesClass("PositionKeyframes", mesh.Animations);
            this.Add(keyframes);
        }

    }

    class KeyframesClass : Class
    {
        public KeyframesClass(string name, AnimationClip[] animationClips)
            : base(name)
        {
            this.IsStatic = true;
            this.IsPublic = true;
            this.IsPartial = false;

            foreach (var item in animationClips)
            {
                Add(new Variable("Skill.Framework.Animation.Vector3Keyframes", item.Name) { IsStatic = true });
                Add(new Vector3KeyframesProperty(item.Name, item.RootMotion));
            }
        }
    }

    class Vector3KeyframesProperty : Property
    {
        public Vector3KeyframesProperty(string propertyName, RootMotion rootMotion)
            : base("Skill.Framework.Animation.Vector3Keyframes", propertyName, "", false)
        {
            base.IsStatic = true;
            base.Modifiers = CSharp.Modifiers.Public;
            base.Multiline = true;

            StringBuilder builder = new StringBuilder();
            string varName = Variable.GetName(propertyName);

            builder.AppendLine(string.Format("if ({0} == null)", varName));
            builder.AppendLine("{");

            builder.AppendLine("UnityEngine.Keyframe[] xKeys = null, yKeys = null, zKeys = null;");

            if (rootMotion.XKeys.Count > 0)            
                CreateKeyframesVariable(builder, rootMotion.XKeys, "xKeys ");
                
            if (rootMotion.YKeys.Count > 0)
                CreateKeyframesVariable(builder, rootMotion.YKeys, "yKeys");

            if (rootMotion.ZKeys.Count > 0)
                CreateKeyframesVariable(builder, rootMotion.ZKeys, "zKeys ");

            builder.AppendLine(string.Format("{0} = new Skill.Framework.Animation.Vector3Keyframes(xKeys, yKeys, zKeys);", varName));

            builder.AppendLine("}");
            builder.AppendLine(string.Format("return {0};", varName));

            _Get = builder.ToString();
        }

        private void CreateKeyframesVariable(StringBuilder buffer, KeyframeCollection keys, string varName)
        {
            buffer.AppendLine(string.Format("{0} = new UnityEngine.Keyframe[] {{", varName));
            for (int i = 0; i < keys.Count; i++)
            {
                var k = keys[i];
                buffer.AppendLine(string.Format("new UnityEngine.Keyframe({0}f, {1}f, {2}f, {3}f),", k.Time, k.Value, k.InTangent, k.OutTangent));
            }
            buffer.AppendLine("};");
        }
    }
}
