using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio
{

    public class AnimationTreeNode : EntityNode
    {
        public override EntityType EntityType { get { return EntityType.AnimationTree; } }

        public AnimationTreeNode()
            : base("NewAnimationTree")
        {

        }
    }

    public class AnimationTreeNodeViewModel : EntityNodeViewModel
    {
        public AnimationTreeNodeViewModel(EntityNodeViewModel parent, AnimationTreeNode anim)
            : base(parent, anim)
        {

        }

        public override string ImageName { get { return Images.AnimationTree; } }

        public override void New()
        {
            Animation.AnimationTree tree = new Animation.AnimationTree();
            tree.Name = Name;

            string dir = System.IO.Path.Combine(Project.Directory, GetLocalDirectory());
            if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);

            string filename = System.IO.Path.Combine(Project.Directory, LocalFileName);
            tree.Save(filename);
            base.New();
        }

        public override void Delete()
        {
            string filename = System.IO.Path.Combine(Project.Directory, LocalFileName);
            if (System.IO.File.Exists(filename)) System.IO.File.Delete(filename);
            base.Delete();
        }

        public override string LocalFileName { get { return LocalFileNameWithoutExtension + Animation.AnimationTree.Extension; } }


        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                if (value != base.Name)
                {
                    string sourceFilename = System.IO.Path.Combine(Project.Directory, LocalFileName);
                    if (System.IO.File.Exists(sourceFilename))
                    {
                        string destLocalfilename = System.IO.Path.Combine(GetLocalDirectory(), value + Animation.AnimationTree.Extension);
                        string destfilename = System.IO.Path.Combine(Project.Directory, destLocalfilename);
                        if (System.IO.File.Exists(destfilename)) return;
                        System.IO.File.Move(sourceFilename, destfilename);
                        MainWindow.Instance.ChangeDocumentName(LocalFileName, value);
                        base.Name = value;
                    }
                }
            }
        }

        public override EntityNodeViewModel Clone(EntityNodeViewModel copyParent)
        {
            AnimationTreeNode cloneModel = new AnimationTreeNode();
            cloneModel.Name = Name;
            AnimationTreeNodeViewModel atree = new AnimationTreeNodeViewModel(copyParent, cloneModel);
            return atree;
        }
    }
}
