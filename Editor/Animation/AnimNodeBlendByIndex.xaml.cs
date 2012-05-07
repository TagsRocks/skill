using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Skill.Editor.Animation
{
    public class AnimNodeBlendByIndex : AnimNodeChildrenEditable
    {
        public AnimNodeBlendByIndex()
            : base("AnimNodeBlendByIndex")
        {
        }

        public override AnimNodeType NodeType { get { return AnimNodeType.BlendByIndex; } }

        protected override void WriteAttributes(XElement e)
        {
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            //AnimNodeBlendByIndex s = other as AnimNodeBlendByIndex;            
        }

        public override AnimNode Clone()
        {
            AnimNodeBlendByIndex node = new AnimNodeBlendByIndex();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimNodeBlendByIndexProperties : AnimNodeChildrenEditableProperties, IAnimNodeConnectorContainer
    {
        public AnimNodeBlendByIndexProperties(AnimNodeBlendByIndexViewModel viewmodel)
            : base(viewmodel)
        {

        }
    }


    /// <summary>
    /// Interaction logic for AnimNodeBlendByIndex.xaml
    /// </summary>
    public partial class AnimNodeBlendByIndexViewModel : AnimNodeChildrenEditableViewModel
    {
        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } }

        protected override StackPanel ConnectorPanel { get { return _ConnectorPanel; } }

        public AnimNodeBlendByIndexViewModel()
            : this(new AnimNodeBlendByIndex())
        {
        }

        public AnimNodeBlendByIndexViewModel(AnimNodeBlendByIndex model)
            : base(model)
        {
            InitializeComponent();
            InitializeChildNames();
            this.Properties = new AnimNodeBlendByIndexProperties(this);
        }
    }
}
