using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public class AnimNodeAimOffsetProfile : IXElement
    {
        public static string ElementName = "AimOffsetProfile";

        public string Name { get; private set; }
        public string CenterCenter { get; private set; }
        public string CenterUp { get; private set; }
        public string CenterDown { get; private set; }
        public string LeftCenter { get; private set; }
        public string LeftUp { get; private set; }
        public string LeftDown { get; private set; }
        public string RightCenter { get; private set; }
        public string RightUp { get; private set; }
        public string RightDown { get; private set; }


        public XElement ToXElement()
        {
            XElement e = new XElement(ElementName);
            e.SetAttributeValue("Name", Name);
            e.SetAttributeValue("CenterCenter", CenterCenter);
            e.SetAttributeValue("CenterUp", CenterUp);
            e.SetAttributeValue("CenterDown", CenterDown);
            e.SetAttributeValue("LeftCenter", LeftCenter);
            e.SetAttributeValue("LeftUp", LeftUp);
            e.SetAttributeValue("LeftDown", LeftDown);
            e.SetAttributeValue("RightCenter", RightCenter);
            e.SetAttributeValue("RightUp", RightUp);
            e.SetAttributeValue("RightDown", RightDown);
            return e;
        }

        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", "");
            this.CenterCenter = e.GetAttributeValueAsString("CenterCenter", "");
            this.CenterUp = e.GetAttributeValueAsString("CenterUp", "");
            this.CenterDown = e.GetAttributeValueAsString("CenterDown", "");
            this.LeftCenter = e.GetAttributeValueAsString("LeftCenter", "");
            this.LeftUp = e.GetAttributeValueAsString("LeftUp", "");
            this.LeftDown = e.GetAttributeValueAsString("LeftDown", "");
            this.RightCenter = e.GetAttributeValueAsString("RightCenter", "");
            this.RightUp = e.GetAttributeValueAsString("RightUp", "");
            this.RightDown = e.GetAttributeValueAsString("RightDown", "");
        }

        public AnimNodeAimOffsetProfile Clone()
        {
            return new AnimNodeAimOffsetProfile()
            {
                CenterCenter = this.CenterCenter,
                CenterDown = this.CenterDown,
                CenterUp = this.CenterUp,
                LeftCenter = this.LeftCenter,
                LeftDown = this.LeftDown,
                LeftUp = this.LeftUp,
                Name = this.Name,
                RightCenter = this.RightCenter,
                RightDown = this.RightDown,
                RightUp = this.RightUp,
            };
        }
    }
}
