using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public enum AnimationTreePropertyType
    {
        Int,
        Float,
        Bool,
        String,
        Posture
    }

    public class AnimationTreeProperty : IXElement
    {
        public AnimationTreePropertyType Type { get; set; }
        public string Name { get; set; }

        public XElement ToXElement()
        {
            XElement property = new XElement("AnimationTreeProperty");

            property.SetAttributeValue("Type", (int)this.Type);
            property.SetAttributeValue("Name", this.Name);

            return property;
        }

        public void Load(XElement e)
        {
            this.Type = (AnimationTreePropertyType)e.GetAttributeValueAsInt("AnimNode", 0);
            this.Name = e.GetAttributeValueAsString("Property", this.Name);
        }
    }
}
