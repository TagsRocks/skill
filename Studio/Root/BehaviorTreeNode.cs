using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;
using Skill.DataModels;

namespace Skill.Studio
{
    #region BehaviorTreeNode
    public class BehaviorTreeNode : EntityNode
    {
        public override EntityType EntityType { get { return EntityType.BehaviorTree; } }

        public BehaviorTreeNode()
            : base("NewBehaviorTree")
        {

        }
    }
    #endregion

    #region BehaviorTreeNodeViewModel
    public class BehaviorTreeNodeViewModel : EntityNodeViewModel
    {
        public BehaviorTreeNodeViewModel(EntityNodeViewModel parent, BehaviorTreeNode tree)
            : base(parent, tree)
        {

        }

        public override string ImageName { get { return Images.BehaviorTree; } }

        /// <summary>
        /// Create new file
        /// </summary>
        public override void New()
        {
            BehaviorTree tree = new BehaviorTree();
            tree.Name = Name;
            SaveData(tree);
        }


        public override EntityNodeViewModel Clone(EntityNodeViewModel copyParent)
        {
            BehaviorTreeNode cloneModel = new BehaviorTreeNode();
            cloneModel.Name = Name;
            BehaviorTreeNodeViewModel btree = new BehaviorTreeNodeViewModel(copyParent, cloneModel);
            return btree;
        }


        public override void SaveData(object data)
        {

            if (data != null)
            {
                BehaviorTree bt = data as BehaviorTree;
                if (bt != null)
                {
                    bt.Name = this.Name;

                    string filename = AbsolutePath;
                    Project.CreateDirectory(filename);

                    DataFile datafile = new DataFile();
                    datafile.Document.Add(bt.ToXElement());
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
                BehaviorTree behaviorTree = new BehaviorTree();
                DataFile data = new DataFile(filename);
                behaviorTree.Load(data.Root);
                return behaviorTree;
            }
            return null;
        }
    }
    #endregion
}
