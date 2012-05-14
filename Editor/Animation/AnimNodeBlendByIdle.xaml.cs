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
    public class AnimNodeBlendByIdle : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.BlendByIdle; } }

        public AnimNodeBlendByIdle()
            : base("AnimNodeBlendByIdle")
        {

        }

        public override AnimNode Clone()
        {
            AnimNodeBlendByIdle node = new AnimNodeBlendByIdle();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimNodeBlendByIdleProperties : AnimNodeProperties
    {
        public AnimNodeBlendByIdleProperties(AnimNodeBlendByIdleViewModel viewmodel)
            : base(viewmodel)
        {

        }
    }

    /// <summary>
    /// Interaction logic for AnimNodeBlendByIdle.xaml
    /// </summary>
    public partial class AnimNodeBlendByIdleViewModel : AnimNodeViewModel
    {
        public override Skill.Studio.Diagram.Connector Out { get { return _Out; } }

        public AnimNodeBlendByIdleViewModel()
            : this(new AnimNodeBlendByIdle())
        {
        }
        public AnimNodeBlendByIdleViewModel(AnimNodeBlendByIdle model)
            : base(model)
        {
            InitializeComponent();
            this.Properties = new AnimNodeBlendByIdleProperties(this);
        }
    }
}
