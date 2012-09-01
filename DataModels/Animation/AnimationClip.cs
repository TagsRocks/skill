using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    /// <summary>
    /// Represent an AnimationClip data
    /// </summary>
    public class AnimationClip : IXElement
    {
        /// <summary> Name of AnimationClip </summary>
        public string Name { get; set; }
        /// <summary> WrapMode of AnimationClip </summary>
        public WrapMode WrapMode { get; set; }
        /// <summary> Length of AnimationClip </summary>
        public float Length { get; set; }
        /// <summary> RootMotion keyframes </summary>
        public RootMotion RootMotion { get; private set; }


        public AnimationClip()
        {
            this.RootMotion = new RootMotion();
        }

        /// <summary>
        /// Convert AnimationClip data to a XElement
        /// </summary>
        /// <returns>XElement containing data</returns>
        public XElement ToXElement()
        {
            XElement clip = new XElement("AnimationClip");
            clip.SetAttributeValue("Name", Name);
            clip.SetAttributeValue("WrapMode", (int)WrapMode);
            clip.SetAttributeValue("Length", Length);

            clip.Add(RootMotion.ToXElement());

            return clip;
        }

        /// <summary>
        /// Load AnimationClip data from XElement
        /// </summary>
        /// <param name="e">XElement to load</param>
        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", Name);
            this.WrapMode = (WrapMode)e.GetAttributeValueAsInt("WrapMode", 0);
            this.Length = e.GetAttributeValueAsFloat("Length", 0);

            XElement rootMotion = e.FindChildByName(RootMotion.ElementName);
            if (rootMotion != null)
                this.RootMotion.Load(rootMotion);
        }
    }
}
