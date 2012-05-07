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

    public class AnimNodeOverride : AnimNodeChildrenEditable
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.Override; } }

        public float OverridePeriod { get; set; }

        public AnimNodeOverride()
            : base("AnimNodeOverride")
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


        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeOverride s = other as AnimNodeOverride;
            this.OverridePeriod = s.OverridePeriod;
        }


        public override AnimNode Clone()
        {
            AnimNodeOverride node = new AnimNodeOverride();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimNodeOverrideProperties : AnimNodeChildrenEditableProperties
    {
        public AnimNodeOverrideProperties(AnimNodeOverrideViewModel viewmodel)
            : base(viewmodel)
        {

        }

        [Description("if more than zero then AnimNodeOverride node automatically enable overriding at specified time.")]
        public float OverridePeriod
        {
            get { return ((AnimNodeOverride)ViewModel.Model).OverridePeriod; }
            set
            {
                if (((AnimNodeOverride)ViewModel.Model).OverridePeriod != value)
                {

                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "OverridePeriod", value, ((AnimNodeOverride)ViewModel.Model).OverridePeriod));
                    }
                    ((AnimNodeOverride)ViewModel.Model).OverridePeriod = value;
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for AnimNodeOverride.xaml
    /// </summary>
    public partial class AnimNodeOverrideViewModel : AnimNodeChildrenEditableViewModel
    {
        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } }

        protected override StackPanel ConnectorPanel { get { return _ConnectorPanel; } }

        public AnimNodeOverrideViewModel()
            : this(new AnimNodeOverride())
        {
        }
        public AnimNodeOverrideViewModel(AnimNodeOverride model)
            : base(model)
        {
            InitializeComponent();
            InitializeChildNames();
            this.Properties = new AnimNodeOverrideProperties(this);
        }

        protected override void InitializeChildNames()
        {
            string[] names = ((AnimNodeChildrenEditable)Model).ChildrenNames;
            if (names != null)
            {
                names[0] = "Normal";
            }
            base.InitializeChildNames();
        }
        
    }
}
