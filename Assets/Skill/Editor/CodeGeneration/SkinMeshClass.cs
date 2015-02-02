using System;
using System.Collections.Generic;
using Skill.Editor.Animation;
using System.Text;

namespace Skill.Editor.CodeGeneration
{
    class SkinMeshClass : Class
    {
        private SkinMeshData _SkinMesh;

        public SkinMeshClass(SkinMeshData mesh)
            : base(mesh.Name)
        {
            _SkinMesh = mesh;
            IsPartial = false;
            RootMotionKeyframesClass keyframes = new RootMotionKeyframesClass("RootMotionKeyframes", mesh.Animations);
            this.Add(keyframes);

            CreateApplyMixingTransformsMethod();
        }

        private void CreateApplyMixingTransformsMethod()
        {
            StringBuilder body = new StringBuilder();

            if (_SkinMesh.Animations != null && _SkinMesh.Animations.Length > 0)
            {
                body.AppendLine("UnityEngine.AnimationState state = null;");
                body.AppendLine("UnityEngine.Transform t = null;");
                bool found = false;

                foreach (var anim in _SkinMesh.Animations)
                {
                    if (anim.MixingTransforms != null && anim.MixingTransforms.Length > 0)
                    {
                        found = true;
                        body.AppendLine(string.Format("state = animation[\"{0}\"];", anim.Name));
                        body.AppendLine("if (state != null)");
                        body.AppendLine("{");

                        foreach (var ms in anim.MixingTransforms)
                        {
                            body.AppendLine(string.Format("t = animation.transform.Find(\"{0}\");", ms.TransformPath));
                            body.AppendLine(string.Format("if (t != null) state.AddMixingTransform(t, {0});", ms.Recursive ? "true" : "false"));
                        }
                        body.AppendLine("}");
                    }
                }

                if (!found) body.Remove(0, body.Length);
            }
            Method applyMixingTransforms = new Method("void", "ApplyMixingTransforms", body.ToString(), "UnityEngine.Animation animation") { IsStatic = true, Modifier = Modifiers.Public, SubMethod = SubMethod.None };
            this.Add(applyMixingTransforms);
        }

    }

    class RootMotionKeyframesClass : Class
    {
        public RootMotionKeyframesClass(string name, AnimationClipData[] animationClips)
            : base(name)
        {
            this.ClassModifier = ClassModifiers.Static;
            this.Modifier = Modifiers.Public;
            this.IsPartial = false;

            foreach (var item in animationClips)
            {
                Add(new Variable("Skill.Framework.Animation.RootMotionKeyframes", item.Name,"null") { IsStatic = true });
                Add(new RootMotionKeyframesProperty(item.Name, item.RootMotion, item.RootMotionState));
            }
        }
    }

    class RootMotionKeyframesProperty : Property
    {
        public RootMotionKeyframesProperty(string propertyName, RootMotionData rootMotion, RootMotionStateData rootMotionState)
            : base("Skill.Framework.Animation.RootMotionKeyframes", propertyName, "", false)
        {
            base.IsStatic = true;
            base.Modifier = Modifiers.Public;            
            string varName = Variable.GetName(propertyName);

            if (!rootMotionState.PositionX && !rootMotionState.PositionY && !rootMotionState.PositionZ && !rootMotionState.RotationX && !rootMotionState.RotationY && !rootMotionState.RotationZ)
            {
                _Get = string.Format("return {0};", varName);
                base.Multiline = false;
            }
            else
            {
                base.Multiline = true;
                StringBuilder builder = new StringBuilder();

                builder.AppendLine(string.Format("if ({0} == null)", varName));
                builder.AppendLine("{");

                builder.AppendLine("UnityEngine.Keyframe[] xKeys = null, yKeys = null, zKeys = null, rxKeys = null, ryKeys = null, rzKeys = null;");

                if (rootMotionState.PositionX && rootMotion.XKeys.Count > 0)
                    CreateKeyframesVariable(builder, rootMotion.XKeys, "xKeys ");

                if (rootMotionState.PositionY && rootMotion.YKeys.Count > 0)
                    CreateKeyframesVariable(builder, rootMotion.YKeys, "yKeys");

                if (rootMotionState.PositionZ && rootMotion.ZKeys.Count > 0)
                    CreateKeyframesVariable(builder, rootMotion.ZKeys, "zKeys ");

                if (rootMotionState.RotationX && rootMotion.RXKeys.Count > 0)
                    CreateKeyframesVariable(builder, rootMotion.RXKeys, "rxKeys ");

                if (rootMotionState.RotationY && rootMotion.RYKeys.Count > 0)
                    CreateKeyframesVariable(builder, rootMotion.RYKeys, "ryKeys ");

                if (rootMotionState.RotationZ && rootMotion.RZKeys.Count > 0)
                    CreateKeyframesVariable(builder, rootMotion.RZKeys, "rzKeys ");

                builder.AppendLine(string.Format("{0} = new Skill.Framework.Animation.RootMotionKeyframes(xKeys, yKeys, zKeys, rxKeys, ryKeys, rzKeys);", varName));

                builder.AppendLine("}");
                builder.AppendLine(string.Format("return {0};", varName));

                _Get = builder.ToString();
            }
        }

        private void CreateKeyframesVariable(StringBuilder buffer, KeyframeDataCollection keys, string varName)
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
