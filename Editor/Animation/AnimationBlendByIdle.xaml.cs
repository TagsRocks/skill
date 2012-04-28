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
    public class AnimationBlendByIdle : AnimationNode
    {
        public override AnimationNodeType NodeType { get { return AnimationNodeType.BlendByIdle; } }

        public AnimationBlendByIdle()
            : base("AnimationBlendByIdle")
        {

        }

        public override AnimationNode Clone()
        {
            AnimationBlendByIdle node = new AnimationBlendByIdle();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimationBlendByIdleProperties : AnimationNodeProperties
    {
        public AnimationBlendByIdleProperties(AnimationBlendByIdleViewModel viewmodel)
            : base(viewmodel)
        {

        }
    }

    /// <summary>
    /// Interaction logic for AnimationBlendByIdle.xaml
    /// </summary>
    public partial class AnimationBlendByIdleViewModel : AnimationNodeViewModel
    {
        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } }

        public AnimationBlendByIdleViewModel()
            : this(new AnimationBlendByIdle())
        {
        }
        public AnimationBlendByIdleViewModel(AnimationBlendByIdle model)
            : base(model)
        {
            InitializeComponent();
            this.Properties = new AnimationBlendByIdleProperties(this);
        }
    }
}
