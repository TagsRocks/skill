using Skill.Framework.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Editor.Animation
{
    public class MixingTransformData : IXmlElementSerializable
    {
        public string TransformPath { get; set; }
        public bool Recursive { get; set; }

        public MixingTransformData()
        {
        }

        public XmlElement ToXmlElement()
        {
            XmlElement e = new XmlElement("MixingTransform");
            e.SetAttributeValue("TPath", this.TransformPath);
            e.SetAttributeValue("Recursive", this.Recursive);
            return e;
        }

        public void Load(XmlElement e)
        {
            this.TransformPath = e.GetAttribute("TPath");
            this.Recursive = e.GetAttributeValueAsBoolean("Recursive", true);
        }
    }
}