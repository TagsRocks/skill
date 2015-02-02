using System;
using System.Collections.Generic;
using Skill.Framework.IO;
using UnityEngine;

namespace Skill.Editor.Animation
{
    /// <summary>
    /// Represent an AnimationClip data
    /// </summary>
    public class AnimationClipData : IXmlElementSerializable
    {
        /// <summary> Name of AnimationClip </summary>
        public string Name { get; set; }
        /// <summary> WrapMode of AnimationClip </summary>
        public WrapMode WrapMode { get; set; }
        /// <summary> Length of AnimationClip </summary>
        public float Length { get; set; }
        /// <summary> RootMotion keyframes </summary>
        public RootMotionData RootMotion { get; private set; }

        /// <summary> Mixing transforms </summary>
        public MixingTransformData[] MixingTransforms { get; set; }

        public RootMotionStateData RootMotionState { get; set; }

        public AnimationClipData()
        {
            this.RootMotion = new RootMotionData();
            this.RootMotionState = new RootMotionStateData();
            this.MixingTransforms = null;
        }

        /// <summary>
        /// Convert AnimationClip data to a XmlElement
        /// </summary>
        /// <returns>XmlElement containing data</returns>
        public XmlElement ToXmlElement()
        {
            XmlElement clip = new XmlElement("AnimationClip");
            clip.SetAttributeValue("Name", Name);
            clip.SetAttributeValue("WrapMode", (int)WrapMode);
            clip.SetAttributeValue("Length", Length);

            if (this.MixingTransforms != null && this.MixingTransforms.Length > 0)
            {
                XmlElement mixingTransforms = new XmlElement("MixingTransforms");
                mixingTransforms.SetAttributeValue("Count", MixingTransforms.Length);
                foreach (var item in this.MixingTransforms)
                {
                    XmlElement transform = item.ToXmlElement();
                    mixingTransforms.AppendChild(transform);
                }
                clip.AppendChild(mixingTransforms);
            }

            clip.AppendChild(RootMotion.ToXmlElement());

            clip.AppendChild(RootMotionState.ToXmlElement());

            return clip;
        }

        /// <summary>
        /// Load AnimationClip data from XmlElement
        /// </summary>
        /// <param name="e">XmlElement to load</param>
        public void Load(XmlElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", Name);
            this.WrapMode = (WrapMode)e.GetAttributeValueAsInt("WrapMode", 0);
            this.Length = e.GetAttributeValueAsFloat("Length", 0);

            XmlElement mixingTransforms = e["MixingTransforms"];
            if (mixingTransforms != null)
            {
                int count = mixingTransforms.GetAttributeValueAsInt("Count", 0);
                int i = 0;
                MixingTransforms = new MixingTransformData[count];
                foreach (var transform in mixingTransforms)
                {
                    MixingTransformData mt = new MixingTransformData();
                    mt.Load(transform);
                    this.MixingTransforms[i++] = mt;
                }
            }
            else
            {
                this.MixingTransforms = null;
            }

            XmlElement rootMotion = e[RootMotionData.ElementName];
            if (rootMotion != null)
                this.RootMotion.Load(rootMotion);

            XmlElement rootmotionState = e[RootMotionStateData.ElementName];
            if (rootmotionState != null)
                this.RootMotionState.Load(rootmotionState);
        }
    }
}
