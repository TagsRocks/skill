using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels.Animation
{
    public class AnimNodeSubTree : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.SubTree; } }

        public string TreeAddress { get; set; }

        public AnimNodeSubTree()
            : base("NewSubTree")
        {
            TreeAddress = "";

            base.Inputs = new Connector[1];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Null" };
        }


        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            e.SetAttributeValue("TreeAddress", string.IsNullOrEmpty(TreeAddress) ? "" : TreeAddress);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            this.TreeAddress = e.GetAttributeValueAsString("TreeAddress", "");
            base.ReadAttributes(e);
        }


        public override AnimNode Clone()
        {
            AnimNodeSubTree node = new AnimNodeSubTree();
            node.CopyFrom(this);
            return node;
        }

        public override void CopyFrom(AnimNode other)
        {
            ((AnimNodeSubTree)other).TreeAddress = this.TreeAddress;
            base.CopyFrom(other);
        }
    }
}
