using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.Animation;
using System.ComponentModel;

namespace Skill.Studio.Animation
{
    [DisplayName("AnimNodeAdditiveBlending")]
    public class AnimNodeAdditiveBlendingViewModel : AnimNodeViewModel
    {
        public AnimNodeAdditiveBlendingViewModel(AnimationTreeViewModel tree, AnimNodeAdditiveBlending model)
            : base(tree, model)
        {            
        }

        [Browsable(false)]
        public override string ImageName { get { return Editor.AnimationImages.Additive; } }

        [Browsable(false)]
        public override System.Windows.Media.Brush ContentBrush { get { return Editor.StaticBrushes.AnimAdditiveContnetBrush; } }
    }
}
