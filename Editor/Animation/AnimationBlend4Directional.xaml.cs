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
    public class AnimationBlend4Directional : AnimationNode
    {
        public override AnimationNodeType NodeType { get { return AnimationNodeType.Blend4Directional; } }

        public AnimationBlend4Directional()
            : base("AnimationBlend4Directional")
        {

        }

        public override AnimationNode Clone()
        {
            AnimationBlend4Directional node = new AnimationBlend4Directional();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimationBlend4DirectionalProperties : AnimationNodeProperties
    {
        public AnimationBlend4DirectionalProperties(AnimationBlend4DirectionalViewModel viewmodel)
            : base(viewmodel)
        {

        }
    }

    /// <summary>
    /// Interaction logic for AnimationBlend4Directional.xaml
    /// </summary>
    public partial class AnimationBlend4DirectionalViewModel : AnimationNodeViewModel
    {
        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } } 

        public AnimationBlend4DirectionalViewModel()
            : this(new AnimationBlend4Directional())
        {
        }
        public AnimationBlend4DirectionalViewModel(AnimationBlend4Directional model)
            : base(model)
        {
            InitializeComponent();
            this.Properties = new AnimationBlend4DirectionalProperties(this);            
        }
    }
}
