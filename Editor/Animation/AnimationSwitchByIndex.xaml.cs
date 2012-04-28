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
    public class AnimationSwitchByIndex : AnimationChildrenEditable
    {
        public AnimationSwitchByIndex()
            : base("AnimationSwitchByIndex")
        {
        }

        public override AnimationNodeType NodeType { get { return AnimationNodeType.SwitchByIndex; } }

        protected override void WriteAttributes(XElement e)
        {
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimationNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            //AnimationSwitchByIndex s = other as AnimationSwitchByIndex;            
        }

        public override AnimationNode Clone()
        {
            AnimationSwitchByIndex node = new AnimationSwitchByIndex();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimationSwitchByIndexProperties : AnimationChildrenEditableProperties, IAnimConnectorContainer
    {
        public AnimationSwitchByIndexProperties(AnimationSwitchByIndexViewModel viewmodel)
            : base(viewmodel)
        {

        }
    }


    /// <summary>
    /// Interaction logic for AnimationSwitchByIndex.xaml
    /// </summary>
    public partial class AnimationSwitchByIndexViewModel : AnimationChildrenEditableViewModel
    {
        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } }

        protected override StackPanel ConnectorPanel { get { return _ConnectorPanel; } }

        public AnimationSwitchByIndexViewModel()
            : this(new AnimationSwitchByIndex())
        {
        }

        public AnimationSwitchByIndexViewModel(AnimationSwitchByIndex model)
            : base(model)
        {
            InitializeComponent();
            InitializeChildNames();
            this.Properties = new AnimationSwitchByIndexProperties(this);
        }
    }
}
