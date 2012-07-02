using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.Animation;
using System.ComponentModel;

namespace Skill.Studio.Animation
{
    public class AnimNodeSubTreeViewModel : AnimNodeViewModel
    {
        public override string ImageName { get { return Images.AnimationTree; } }

        [Description("Select AnimationTree")]
        [DisplayName("AnimationTree")]
        [Editor(typeof(Editor.TreeAddressPropertyEditor), typeof(Editor.TreeAddressPropertyEditor))]
        public string TreeAddress
        {
            get
            {
                return ((AnimNodeSubTree)Model).TreeAddress;
            }
            set
            {
                if (((AnimNodeSubTree)Model).TreeAddress != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "TreeAddress", value, ((AnimNodeSubTree)Model).TreeAddress));
                    }
                    ((AnimNodeSubTree)Model).TreeAddress = value;
                    Inputs[0].Name = value;
                    OnPropertyChanged("TreeAddress");
                }
            }
        }        

        public AnimNodeSubTreeViewModel(AnimationTreeViewModel tree, AnimNodeSubTree model)
            : base(tree, model)
        {
        }

        [Browsable(false)]
        public override System.Windows.Visibility InputsVisible { get { return System.Windows.Visibility.Hidden; } }

        [Browsable(false)]
        public override float BlendTime { get { return base.BlendTime; } set { base.BlendTime = value; } }

        [Browsable(false)]
        public override bool BecameRelevant { get { return base.BecameRelevant; } set { base.BecameRelevant = value; } }

        [Browsable(false)]
        public override bool CeaseRelevant { get { return base.CeaseRelevant; } set { base.CeaseRelevant = value; } }        

        [Browsable(false)]
        public override System.Windows.Media.Brush ContentBrush { get { return Editor.StaticBrushes.AnimRootContnetBrush; } }
    }
}
