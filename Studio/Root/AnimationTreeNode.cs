using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.Animation;
using System.Xml.Linq;
using Skill.DataModels;

namespace Skill.Studio
{

    #region AnimationTreeNode
    public class AnimationTreeNode : EntityNode
    {
        public override EntityType EntityType { get { return EntityType.AnimationTree; } }

        public AnimationTreeNode()
            : base("NewAnimationTree")
        {

        }
    }
    #endregion

    public class AnimationTreeNodeViewModel : EntityNodeViewModel
    {
        public AnimationTreeNodeViewModel(EntityNodeViewModel parent, AnimationTreeNode anim)
            : base(parent, anim)
        {
        }

        public override string ImageName { get { return Images.AnimationTree; } }

        public override void New()
        {
            AnimationTree tree = new AnimationTree();
            tree.Name = Name;
            SaveData(tree);
        }

        public override EntityNodeViewModel Clone(EntityNodeViewModel copyParent)
        {
            AnimationTreeNode cloneModel = new AnimationTreeNode();
            cloneModel.Name = Name;
            AnimationTreeNodeViewModel atree = new AnimationTreeNodeViewModel(copyParent, cloneModel);
            return atree;
        }


        public override void SaveData(object data)
        {
            if (data != null)
            {
                AnimationTree at = data as AnimationTree;
                if (at != null)
                {
                    at.Name = this.Name;

                    string filename = AbsolutePath;
                    Project.CreateDirectory(filename);

                    DataFile datafile = new DataFile();
                    datafile.Document.Add(at.ToXElement());
                    datafile.Save(filename);
                }
            }
            base.SaveData(data);
        }

        public override object LoadData()
        {
            string filename = AbsolutePath;
            if (System.IO.File.Exists(filename))
            {
                AnimationTree animationTree = new AnimationTree();
                DataFile data = new DataFile(filename);
                animationTree.Load(data.Root);
                animationTree.Name = System.IO.Path.GetFileNameWithoutExtension(filename);
                return animationTree;
            }
            return null;
        }
    }
}
