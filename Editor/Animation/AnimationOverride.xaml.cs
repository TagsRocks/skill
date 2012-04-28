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
using Skill.Editor.Diagram;
using System.Xml.Linq;
using System.ComponentModel;

namespace Skill.Editor.Animation
{

    public class AnimationOverride : AnimationChildrenEditable
    {
        public override AnimationNodeType NodeType { get { return AnimationNodeType.Override; } }

        public float OverridePeriod { get; set; }

        public AnimationOverride()
            : base("AnimationOverride")
        {

        }

        protected override void WriteAttributes(XElement e)
        {
            XElement data = new XElement("OverrideData");
            data.SetAttributeValue("OverridePeriod", this.OverridePeriod);
            e.Add(data);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("OverrideData");
            if (data != null)
            {
                this.OverridePeriod = data.GetAttributeValueAsFloat("OverridePeriod", 0.0f);
            }
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimationNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimationOverride s = other as AnimationOverride;
            this.OverridePeriod = s.OverridePeriod;
        }


        public override AnimationNode Clone()
        {
            AnimationOverride node = new AnimationOverride();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimationOverrideProperties : AnimationChildrenEditableProperties
    {
        public AnimationOverrideProperties(AnimationOverrideViewModel viewmodel)
            : base(viewmodel)
        {

        }

        [Description("if more than zero then AnimationOverride node automatically enable overriding at specified time.")]
        public float OverridePeriod
        {
            get { return ((AnimationOverride)ViewModel.Model).OverridePeriod; }
            set
            {
                if (((AnimationOverride)ViewModel.Model).OverridePeriod != value)
                {

                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "OverridePeriod", value, ((AnimationOverride)ViewModel.Model).OverridePeriod));
                    }
                    ((AnimationOverride)ViewModel.Model).OverridePeriod = value;
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for AnimationOverride.xaml
    /// </summary>
    public partial class AnimationOverrideViewModel : AnimationChildrenEditableViewModel
    {
        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } }

        protected override StackPanel ConnectorPanel { get { return _ConnectorPanel; } }

        public AnimationOverrideViewModel()
            : this(new AnimationOverride())
        {
        }
        public AnimationOverrideViewModel(AnimationOverride model)
            : base(model)
        {
            InitializeComponent();
            InitializeChildNames();
            this.Properties = new AnimationOverrideProperties(this);
        }

        protected override void InitializeChildNames()
        {
            string[] names = ((AnimationChildrenEditable)Model).ChildrenNames;
            if (names != null)
            {
                names[0] = "Normal";
            }
            base.InitializeChildNames();
        }
        
    }
}
