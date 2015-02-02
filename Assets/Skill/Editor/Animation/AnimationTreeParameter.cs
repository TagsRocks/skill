using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public enum AnimationTreeParameterType
    {
        Float,
        Integer,
    }

    public class AnimationTreeParameter : IXmlElementSerializable
    {
        public string Name { get; set; }
        public AnimationTreeParameterType Type { get; set; }
        public float DefaultValue { get; set; }
        public string Comment { get; set; }

        public AnimationTreeParameter()
        {
            this.Name = "NewParameter";
            this.DefaultValue = 0;
            this.Type = AnimationTreeParameterType.Float;
        }

        public XmlElement ToXmlElement()
        {
            XmlElement e = new XmlElement("Parameter");
            e.SetAttributeValue("Name", Name);
            e.SetAttributeValue("Type", Type.ToString());
            e.SetAttributeValue("DefaultValue", DefaultValue);

            if (!string.IsNullOrEmpty(Comment))
            {
                XmlElement comment = new XmlElement("Comment");
                comment.Value = Comment;
                e.AppendChild(comment);
            }

            return e;
        }

        public void Load(XmlElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", "NewParameter");
            this.Type = e.GetAttributeValueAsEnum("Type", AnimationTreeParameterType.Float);
            this.DefaultValue = e.GetAttributeValueAsFloat("DefaultValue", 0);

            XmlElement comment = e["Comment"];
            if (comment != null)
            {
                Comment = comment.Value;
            }
        }
    }
}
