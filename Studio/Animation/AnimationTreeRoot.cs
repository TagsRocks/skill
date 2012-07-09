
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.Animation;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Skill.Studio.Animation
{
    [DisplayName("AnimationTree")]
    public class AnimationTreeRootViewModel : AnimNodeViewModel
    {

        public AnimationTreeRootViewModel(AnimationTreeViewModel tree, AnimationTreeRoot model)
            : base(tree, model)
        {
            model.Name = "Root";
        }

        [Description("Assigned SkinMesh")]
        [Editor(typeof(Editor.SkinMeshPropertyEditor), typeof(Editor.SkinMeshPropertyEditor))]
        public string SkinMesh
        {
            get { return Tree.Model.SkinMesh; }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (Tree.Model.SkinMesh != value)
                {
                    Tree.Model.SkinMesh = value;
                    OnPropertyChanged("SkinMesh");
                    Tree.Editor.SetChanged(true);
                }
            }
        }

        [Description("Edit Profiles of AnimationTree")]
        [Editor(typeof(Editor.AnimationTreeProfilesPropertyEditor), typeof(Editor.AnimationTreeProfilesPropertyEditor))]
        public AnimationTreeProfile[] Profiles
        {
            get { return Tree.Model.Profiles; }
            set
            {
                Tree.Model.Profiles = value;
                OnPropertyChanged("Profiles");
                Tree.Editor.SetChanged(true);
            }
        }




        [Browsable(false)]
        public override string ImageName { get { return Editor.AnimationImages.Root; } }


        [Browsable(false)]
        public override bool IsPublic { get { return base.IsPublic; } set { base.IsPublic = value; } }

        [Browsable(false)]
        public override string Name { get { return base.Name; } set { base.Name = value; } }

        [Browsable(false)]
        public override float BlendTime { get { return base.BlendTime; } set { base.BlendTime = value; } }

        [Browsable(false)]
        public override bool BecameRelevant { get { return base.BecameRelevant; } set { base.BecameRelevant = value; } }

        [Browsable(false)]
        public override bool CeaseRelevant { get { return base.CeaseRelevant; } set { base.CeaseRelevant = value; } }

        [Browsable(false)]
        public override System.Windows.Visibility OutVisible { get { return System.Windows.Visibility.Hidden; } }

        [Browsable(false)]
        public override System.Windows.Media.Brush ContentBrush { get { return Editor.StaticBrushes.AnimRootContnetBrush; } }
    }
}
