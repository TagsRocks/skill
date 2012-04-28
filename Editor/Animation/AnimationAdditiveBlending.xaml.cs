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

namespace Skill.Editor.Animation
{
    public class AnimationAdditiveBlending : AnimationNode
    {
        public override AnimationNodeType NodeType { get { return AnimationNodeType.AdditiveBlending; } }

        public AnimationAdditiveBlending()
            : base("AnimationAdditiveBlending")
        {

        }

        public override AnimationNode Clone()
        {
            AnimationAdditiveBlending node = new AnimationAdditiveBlending();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimationAdditiveBlendingProperties : AnimationNodeProperties
    {
        public AnimationAdditiveBlendingProperties(AnimationAdditiveBlendingViewModel viewmodel)
            : base(viewmodel)
        {

        }
    }

    /// <summary>
    /// Interaction logic for AnimationAdditiveBlending.xaml
    /// </summary>
    public partial class AnimationAdditiveBlendingViewModel : AnimationNodeViewModel
    {

        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } }

        public AnimationAdditiveBlendingViewModel()
            : this(new AnimationAdditiveBlending())
        {
        }
        public AnimationAdditiveBlendingViewModel(AnimationAdditiveBlending model)
            : base(model)
        {
            InitializeComponent();
            this.Properties = new AnimationAdditiveBlendingProperties(this);
        }
    }
}
