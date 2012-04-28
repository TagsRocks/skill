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

namespace Skill.Editor.Animation
{

    public class AnimationBlendByPosture : AnimationNode
    {
        public override AnimationNodeType NodeType { get { return AnimationNodeType.BlendByPosture; } }
        public AnimationBlendByPosture()
            : base("AnimationBlendByPosture")
        {

        }

        public override AnimationNode Clone()
        {
            AnimationBlendByPosture node = new AnimationBlendByPosture ();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimationBlendByPostureProperties : AnimationNodeProperties
    {
        public AnimationBlendByPostureProperties(AnimationBlendByPostureViewModel viewmodel)
            : base(viewmodel)
        {

        }
    }

    /// <summary>
    /// Interaction logic for AnimationBlendByPosture.xaml
    /// </summary>
    public partial class AnimationBlendByPostureViewModel : AnimationNodeViewModel
    {
        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } } 

        public AnimationBlendByPostureViewModel()
            : this(new AnimationBlendByPosture())
        {
        }
        public AnimationBlendByPostureViewModel(AnimationBlendByPosture model)
            : base(model)
        {
            InitializeComponent();
            this.Properties = new AnimationBlendByPostureProperties(this);            
        }
    }
}
