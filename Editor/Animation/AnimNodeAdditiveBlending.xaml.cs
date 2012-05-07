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
    public class AnimNodeAdditiveBlending : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.AdditiveBlending; } }

        public AnimNodeAdditiveBlending()
            : base("AnimNodeAdditiveBlending")
        {

        }

        public override AnimNode Clone()
        {
            AnimNodeAdditiveBlending node = new AnimNodeAdditiveBlending();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimNodeAdditiveBlendingProperties : AnimNodeProperties
    {
        public AnimNodeAdditiveBlendingProperties(AnimNodeAdditiveBlendingViewModel viewmodel)
            : base(viewmodel)
        {

        }
    }

    /// <summary>
    /// Interaction logic for AnimNodeAdditiveBlending.xaml
    /// </summary>
    public partial class AnimNodeAdditiveBlendingViewModel : AnimNodeViewModel
    {

        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } }

        public AnimNodeAdditiveBlendingViewModel()
            : this(new AnimNodeAdditiveBlending())
        {
        }
        public AnimNodeAdditiveBlendingViewModel(AnimNodeAdditiveBlending model)
            : base(model)
        {
            InitializeComponent();
            this.Properties = new AnimNodeAdditiveBlendingProperties(this);
        }
    }
}
