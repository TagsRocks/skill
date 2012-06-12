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
using Skill.DataModels.Animation;
using System.ComponentModel;

namespace Skill.Studio.Animation
{

    public class AnimNodeBlendByPostureViewModel : AnimNodeViewModel
    {
        public AnimNodeBlendByPostureViewModel(AnimationTreeViewModel tree, AnimNodeBlendByPosture model)
            : base(tree, model)
        {
        }

        [Browsable(false)]
        public override string ImageName { get { return Editor.AnimationImages.BlendByPosture; } }

        [Browsable(false)]
        public override System.Windows.Media.Brush ContentBrush { get { return Editor.StaticBrushes.AnimBlendContnetBrush; } }
    }
}
