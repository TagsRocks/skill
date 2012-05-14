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
using Skill.Studio.Diagram;
using System.Xml.Linq;

namespace Skill.Studio.Animation
{

    public class AnimNodeBlendByPosture : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.BlendByPosture; } }
        public AnimNodeBlendByPosture()
            : base("AnimNodeBlendByPosture")
        {

        }

        public override AnimNode Clone()
        {
            AnimNodeBlendByPosture node = new AnimNodeBlendByPosture ();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimNodeBlendByPostureProperties : AnimNodeProperties
    {
        public AnimNodeBlendByPostureProperties(AnimNodeBlendByPostureViewModel viewmodel)
            : base(viewmodel)
        {

        }
    }

    /// <summary>
    /// Interaction logic for AnimNodeBlendByPosture.xaml
    /// </summary>
    public partial class AnimNodeBlendByPostureViewModel : AnimNodeViewModel
    {
        public override Skill.Studio.Diagram.Connector Out { get { return _Out; } } 

        public AnimNodeBlendByPostureViewModel()
            : this(new AnimNodeBlendByPosture())
        {
        }
        public AnimNodeBlendByPostureViewModel(AnimNodeBlendByPosture model)
            : base(model)
        {
            InitializeComponent();
            this.Properties = new AnimNodeBlendByPostureProperties(this);            
        }
    }
}
