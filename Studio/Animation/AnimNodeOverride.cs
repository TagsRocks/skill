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
using System.ComponentModel;
using Skill.DataModels.Animation;

namespace Skill.Studio.Animation
{
    public class AnimNodeOverrideViewModel : AnimNodeViewModel
    {
        public AnimNodeOverrideViewModel(AnimationTreeViewModel tree, AnimNodeOverride model)
            : base(tree, model)
        {
        }

        [Browsable(false)]
        public override string ImageName { get { return Editor.AnimationImages.Blank; } }

        [Browsable(false)]
        public override System.Windows.Media.Brush ContentBrush { get { return Editor.StaticBrushes.AnimOtherContnetBrush; } }

        [Description("Edit Connectors")]
        [Editor(typeof(Editor.AnimNodeInputsPropertyEditor), typeof(Editor.AnimNodeInputsPropertyEditor))]
        public string Connectors
        {
            get { return "Connectors"; }
        }

        [Description("if more than zero then AnimNodeOverride node automatically enable overriding at specified time.")]
        public float OverridePeriod
        {
            get { return ((AnimNodeOverride)Model).OverridePeriod; }
            set
            {
                if (((AnimNodeOverride)Model).OverridePeriod != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "OverridePeriod", value, ((AnimNodeOverride)Model).OverridePeriod));
                    }
                    ((AnimNodeOverride)Model).OverridePeriod = value;
                }
            }
        }
    }
}
