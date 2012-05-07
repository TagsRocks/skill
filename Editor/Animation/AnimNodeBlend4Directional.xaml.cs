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
    public class AnimNodeBlend4Directional : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.Blend4Directional; } }

        public AnimNodeBlend4Directional()
            : base("AnimNodeBlend4Directional")
        {

        }

        public override AnimNode Clone()
        {
            AnimNodeBlend4Directional node = new AnimNodeBlend4Directional();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimNodeBlend4DirectionalProperties : AnimNodeProperties
    {
        public AnimNodeBlend4DirectionalProperties(AnimNodeBlend4DirectionalViewModel viewmodel)
            : base(viewmodel)
        {

        }
    }

    /// <summary>
    /// Interaction logic for AnimNodeBlend4Directional.xaml
    /// </summary>
    public partial class AnimNodeBlend4DirectionalViewModel : AnimNodeViewModel
    {
        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } } 

        public AnimNodeBlend4DirectionalViewModel()
            : this(new AnimNodeBlend4Directional())
        {
        }
        public AnimNodeBlend4DirectionalViewModel(AnimNodeBlend4Directional model)
            : base(model)
        {
            InitializeComponent();
            this.Properties = new AnimNodeBlend4DirectionalProperties(this);            
        }
    }
}
