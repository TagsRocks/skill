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
using System.ComponentModel;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Skill.DataModels.Animation;
using Skill.DataModels;

namespace Skill.Studio.Animation
{
    public class AnimNodeAimOffsetViewModel : AnimNodeViewModel
    {        
        public AnimNodeAimOffsetViewModel(AnimationTreeViewModel tree, AnimNodeAimOffset model)
            : base(tree, model)
        {
            Profiles = new ObservableCollection<AnimNodeAimOffsetProfileViewModel>();
            if (model.Profiles != null)
            {
                for (int i = 0; i < model.Profiles.Length; i++)
                {
                    AnimNodeAimOffsetProfileViewModel vm = new AnimNodeAimOffsetProfileViewModel(this, model.Profiles[i]);
                    Profiles.Add(vm);
                }
            }
        }

        public override void CommiteChangesToModel()
        {
            ((AnimNodeAimOffset)Model).Profiles = new AnimNodeAimOffsetProfile[this.Profiles.Count];
            for (int i = 0; i < this.Profiles.Count; i++)
            {
                this.Profiles[i].CommiteChangesToModel();
                ((AnimNodeAimOffset)Model).Profiles[i] = this.Profiles[i].Model;
            }            
            base.CommiteChangesToModel();
        }

        [DisplayName("Profiles")]
        [Editor(typeof(Editor.AnimNodeAimOffsetProfilePropertyEditor), typeof(Editor.AnimNodeAimOffsetProfilePropertyEditor))]
        public ObservableCollection<AnimNodeAimOffsetProfileViewModel> Profiles { get; private set; }

        [Browsable(false)]
        public override string ImageName { get { return Editor.AnimationImages.Aim; } }

        [Browsable(false)]
        public override System.Windows.Media.Brush ContentBrush { get { return Editor.StaticBrushes.AnimOtherContnetBrush; } }


        [Category("Events")]
        [Description("If true code generator create an method and hook it to ProfileChanged event")]
        public bool ProfileChanged
        {
            get { return ((AnimNodeAimOffset)Model).ProfileChanged; }
            set
            {
                if (((AnimNodeAimOffset)Model).ProfileChanged != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "ProfileChanged", value, ((AnimNodeAimOffset)Model).ProfileChanged));
                    }
                    ((AnimNodeAimOffset)Model).ProfileChanged = value;
                }
            }
        }
        

        [Description("Whether children AnimationSequences use AnimationTree profile method or not")]
        public bool UseTreeProfile
        {
            get { return ((AnimNodeAimOffset)Model).UseTreeProfile; }
            set
            {
                if (((AnimNodeAimOffset)Model).UseTreeProfile != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "UseTreeProfile", value, ((AnimNodeAimOffset)Model).UseTreeProfile));
                    }
                    ((AnimNodeAimOffset)Model).UseTreeProfile = value;
                }
            }
        }

        [Description("Whether children aim sequence animations are loop?")]
        public bool IsLoop
        {
            get { return ((AnimNodeAimOffset)Model).IsLoop; }
            set
            {
                if (((AnimNodeAimOffset)Model).IsLoop != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "IsLoop", value, ((AnimNodeAimOffset)Model).IsLoop));
                    }
                    ((AnimNodeAimOffset)Model).IsLoop = value;
                }
            }
        }
    }
}
