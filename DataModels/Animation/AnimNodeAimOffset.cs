using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public class AnimNodeAimOffset : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.AimOffset; } }

        public bool ProfileChanged { get; set; }

        private AnimNodeAimOffsetProfile[] _Profiles;
        public AnimNodeAimOffsetProfile[] Profiles
        {
            get
            {
                if (this._Profiles == null)
                    this._Profiles = new AnimNodeAimOffsetProfile[0];
                return this._Profiles;
            }
            set
            {
                this._Profiles = value;
            }
        }

        public AnimNodeAimOffset()
            : base("AnimNodeAimOffset")
        {
            UseTreeProfile = true;
            base.Inputs = new Connector[1];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Input" };            
        }

        public bool UseTreeProfile { get; set; }

        public bool IsLoop { get; set; }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("AimOffsetData");
            if (data != null)
            {
                this.ProfileChanged = data.GetAttributeValueAsBoolean("ProfileChanged", false);
                this.UseTreeProfile = data.GetAttributeValueAsBoolean("UseTreeProfile", true);
                this.IsLoop = data.GetAttributeValueAsBoolean("IsLoop", false);

                XElement profiles = data.FindChildByName("Profiles");
                if (profiles != null)
                {
                    int count = profiles.GetAttributeValueAsInt("Count", 0);
                    this.Profiles = new AnimNodeAimOffsetProfile[count];
                    int i = 0;
                    foreach (var item in profiles.Elements().Where(p => p.Name == AnimNodeAimOffsetProfile.ElementName))
                    {
                        this.Profiles[i] = new AnimNodeAimOffsetProfile();
                        this.Profiles[i].Load(item);
                        i++;
                    }
                }
            }
            base.ReadAttributes(e);
        }
        protected override void WriteAttributes(XElement e)
        {
            XElement data = new XElement("AimOffsetData");

            data.SetAttributeValue("ProfileChanged", this.ProfileChanged);
            data.SetAttributeValue("UseTreeProfile", this.UseTreeProfile);
            data.SetAttributeValue("IsLoop", this.IsLoop);

            if (this.Profiles != null)
            {
                XElement profiles = new XElement("Profiles");

                profiles.SetAttributeValue("Count", this.Profiles.Length);
                for (int i = 0; i < this.Profiles.Length; i++)
                {
                    profiles.Add(this.Profiles[i].ToXElement());
                }
                data.Add(profiles);
            }
            e.Add(data);
            base.WriteAttributes(e);
        }

        public override AnimNode Clone()
        {
            AnimNodeAimOffset node = new AnimNodeAimOffset();
            node.CopyFrom(this);
            return node;
        }

        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeAimOffset offset = other as AnimNodeAimOffset;
            this.ProfileChanged = offset.ProfileChanged;
            this.Profiles = new AnimNodeAimOffsetProfile[offset.Profiles.Length];
            for (int i = 0; i < offset.Profiles.Length; i++)
            {
                this.Profiles[i] = offset.Profiles[i].Clone();
            }
        }
    }
}
