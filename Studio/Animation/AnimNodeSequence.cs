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

    [DisplayName("AnimNodeSequence")]
    public class AnimNodeSequenceViewModel : AnimNodeViewModel
    {
        public AnimNodeSequenceViewModel(AnimationTreeViewModel tree, AnimNodeSequence model)
            : base(tree, model != null ? model : new AnimNodeSequence())
        {
            MixingTransforms = new List<string>();
            if (model.MixingTransforms != null)
            {
                foreach (var item in model.MixingTransforms)
                {
                    this.MixingTransforms.Add(item);
                }
            }
        }

        public override void CommiteChangesToModel()
        {
            ((AnimNodeSequence)Model).MixingTransforms = MixingTransforms.ToArray();
            base.CommiteChangesToModel();
        }

        [Browsable(false)]
        public override Visibility InputsVisible { get { return Visibility.Hidden; } }

        [Browsable(false)]
        public List<string> MixingTransforms { get; private set; }

        [Browsable(false)]
        public override string ImageName { get { return Editor.AnimationImages.Sequence; } }

        [Browsable(false)]
        public override System.Windows.Media.Brush ContentBrush { get { return Editor.StaticBrushes.AnimSequenceContnetBrush; } }


        [DefaultValue(false)]
        [Description("Code generator generates a public property for this node.")]
        public override bool IsPublic { get { return base.IsPublic; } set { base.IsPublic = value; } }

        [Category("Sequence")]
        [Description("Edit Name of AnimationClip and MixingTransforms")]
        [DisplayName("AnimationClip")]
        [Editor(typeof(Editor.AnimNodeSequenceClipPropertyEditor), typeof(Editor.AnimNodeSequenceClipPropertyEditor))]
        public string AnimationName
        {
            get { return ((AnimNodeSequence)Model).AnimationName; }
            set
            {
                if (((AnimNodeSequence)Model).AnimationName != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "AnimationName", value, ((AnimNodeSequence)Model).AnimationName));
                    }
                    ((AnimNodeSequence)Model).AnimationName = value;
                    Inputs[0].Name = AnimationName;
                    OnPropertyChanged("AnimationName");
                }
            }
        }

        [DefaultValue(1)]
        [Category("Sequence")]
        [Description("Speed at which the animation will be played back.Default is 1.0")]
        public float Speed
        {
            get { return ((AnimNodeSequence)Model).Speed; }
            set
            {
                if (((AnimNodeSequence)Model).Speed != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Speed", value, ((AnimNodeSequence)Model).Speed));
                    }
                    ((AnimNodeSequence)Model).Speed = value;
                }
            }
        }

        [DefaultValue(true)]
        [Category("Sequence")]
        [Description("if false, do not take effect by AnimationTree profiles and always use AnimationName")]
        public bool UseTreeProfile
        {
            get { return ((AnimNodeSequence)Model).UseTreeProfile; }
            set
            {
                if (((AnimNodeSequence)Model).UseTreeProfile != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "UseTreeProfile", value, ((AnimNodeSequence)Model).UseTreeProfile));
                    }
                    ((AnimNodeSequence)Model).UseTreeProfile = value;
                }
            }
        }

        [DefaultValue(false)]
        [Category("Sequence")]
        [Description("Synchronize animations with other animations in same Layer.(set true for sequences connected to AnimNode4Direction)")]
        public bool Synchronize
        {
            get { return ((AnimNodeSequence)Model).Synchronize; }
            set
            {
                if (((AnimNodeSequence)Model).Synchronize != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Synchronize", value, ((AnimNodeSequence)Model).Synchronize));
                    }
                    ((AnimNodeSequence)Model).Synchronize = value;
                }
            }
        }

        [Browsable(false)]
        public override float BlendTime
        {
            get
            {
                return base.BlendTime;
            }
            set
            {
                base.BlendTime = value;
            }
        }

        [DefaultValue(WrapMode.Default)]
        [Category("Sequence")]
        [Description("WrapMode")]
        public WrapMode WrapMode
        {
            get { return ((AnimNodeSequence)Model).WrapMode; }
            set
            {
                if (((AnimNodeSequence)Model).WrapMode != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "WrapMode", value, ((AnimNodeSequence)Model).WrapMode));
                    }
                    ((AnimNodeSequence)Model).WrapMode = value;
                }
            }
        }


        [DefaultValue(false)]
        [Category("RootMotion")]
        [Description("Enable or Disable RootMotion for PositionX")]
        public  bool PositionX
        {
            get
            {
                return ((AnimNodeSequence)Model).RootMotion.PositionX;
            }
            set
            {
                if (((AnimNodeSequence)Model).RootMotion.PositionX != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "PositionX", value, ((AnimNodeSequence)Model).RootMotion.PositionX));
                    }
                    ((AnimNodeSequence)Model).RootMotion.PositionX = value;
                    OnPropertyChanged("PositionX");
                }
            }
        }

        [DefaultValue(false)]
        [Category("RootMotion")]
        [Description("Enable or Disable RootMotion for PositionY")]
        public bool PositionY
        {
            get
            {
                return ((AnimNodeSequence)Model).RootMotion.PositionY;
            }
            set
            {
                if (((AnimNodeSequence)Model).RootMotion.PositionY != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "PositionY", value, ((AnimNodeSequence)Model).RootMotion.PositionY));
                    }
                    ((AnimNodeSequence)Model).RootMotion.PositionY = value;
                    OnPropertyChanged("PositionY");
                }
            }
        }

        [DefaultValue(false)]
        [Category("RootMotion")]
        [Description("Enable or Disable RootMotion for PositionZ")]
        public bool PositionZ
        {
            get
            {
                return ((AnimNodeSequence)Model).RootMotion.PositionZ;
            }
            set
            {
                if (((AnimNodeSequence)Model).RootMotion.PositionZ != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "PositionZ", value, ((AnimNodeSequence)Model).RootMotion.PositionZ));
                    }
                    ((AnimNodeSequence)Model).RootMotion.PositionZ = value;
                    OnPropertyChanged("PositionZ");
                }
            }
        }
    }
}
