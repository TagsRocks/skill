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

        public string Name { get; set; }
        public AnimNodeSequence CenterCenter { get; private set; }
        public AnimNodeSequence CenterUp { get; private set; }
        public AnimNodeSequence CenterDown { get; private set; }
        public AnimNodeSequence LeftCenter { get; private set; }
        public AnimNodeSequence LeftUp { get; private set; }
        public AnimNodeSequence LeftDown { get; private set; }
        public AnimNodeSequence RightCenter { get; private set; }
        public AnimNodeSequence RightUp { get; private set; }
        public AnimNodeSequence RightDown { get; private set; }

        public AnimNodeAimOffsetProfile()
        {
            this.Name = "NewProfile";
            this.CenterCenter = new AnimNodeSequence() { Name = "CenterCenter" };
            this.CenterUp = new AnimNodeSequence() { Name = "CenterUp" };
            this.CenterDown = new AnimNodeSequence() { Name = "CenterDown" };
            this.LeftCenter = new AnimNodeSequence() { Name = "LeftCenter" };
            this.LeftUp = new AnimNodeSequence() { Name = "LeftUp" };
            this.LeftDown = new AnimNodeSequence() { Name = "LeftDown" };
            this.RightCenter = new AnimNodeSequence() { Name = "RightCenter" };
            this.RightUp = new AnimNodeSequence() { Name = "RightUp" };
            this.RightDown = new AnimNodeSequence() { Name = "RightDown" };
        }

        private XElement CreateChild(string childName, IXElement child)
        {
            XElement e = new XElement(childName);
            e.Add(child.ToXElement());
            return e;
        }

        public XElement ToXElement()
        {
            XElement e = new XElement(ElementName);
            e.SetAttributeValue("Name", Name);

            e.Add(CreateChild("CenterCenter", CenterCenter));
            e.Add(CreateChild("CenterUp", CenterUp));
            e.Add(CreateChild("CenterDown", CenterDown));
            e.Add(CreateChild("LeftCenter", LeftCenter));
            e.Add(CreateChild("LeftUp", LeftUp));
            e.Add(CreateChild("LeftDown", LeftDown));
            e.Add(CreateChild("RightCenter", RightCenter));
            e.Add(CreateChild("RightUp", RightUp));
            e.Add(CreateChild("RightDown", RightDown));

            return e;
        }

        private void LoadSequenceChild(XElement e, AnimNodeSequence child)
        {
            XElement seqElement = e.FindChildByName(AnimNodeType.Sequence.ToString());
            if (seqElement != null)
            {
                child.Load(seqElement);
            }
        }

        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", "");

            foreach (var element in e.Elements())
            {
                switch (element.Name.ToString())
                {
                    case "CenterCenter":
                        LoadSequenceChild(element, this.CenterCenter);
                        break;
                    case "CenterUp":
                        LoadSequenceChild(element, this.CenterUp);
                        break;
                    case "CenterDown":
                        LoadSequenceChild(element, this.CenterDown);
                        break;
                    case "LeftCenter":
                        LoadSequenceChild(element, this.LeftCenter);
                        break;
                    case "LeftUp":
                        LoadSequenceChild(element, this.LeftUp);
                        break;
                    case "LeftDown":
                        LoadSequenceChild(element, this.LeftDown);
                        break;
                    case "RightCenter":
                        LoadSequenceChild(element, this.RightCenter);
                        break;
                    case "RightUp":
                        LoadSequenceChild(element, this.RightUp);
                        break;
                    case "RightDown":
                        LoadSequenceChild(element, this.RightDown);
                        break;
                    default:
                        break;
                }
            }
        }

        public AnimNodeAimOffsetProfile Clone()
        {
            AnimNodeAimOffsetProfile clone = new AnimNodeAimOffsetProfile() { Name = this.Name, };

            clone.CenterCenter.CopyFrom(this.CenterCenter);
            clone.CenterDown.CopyFrom(this.CenterDown);
            clone.CenterUp.CopyFrom(this.CenterUp);
            clone.LeftCenter.CopyFrom(this.LeftCenter);
            clone.LeftDown.CopyFrom(this.LeftDown);
            clone.LeftUp.CopyFrom(this.LeftUp);
            clone.RightCenter.CopyFrom(this.RightCenter);
            clone.RightDown.CopyFrom(this.RightDown);
            clone.RightUp.CopyFrom(this.RightUp);

            return clone;
        }
    }
}
