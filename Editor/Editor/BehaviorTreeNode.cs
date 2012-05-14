using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            AI.BehaviorTree tree = new AI.BehaviorTree();
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

        public override string LocalFileName { get { return LocalFileNameWithoutExtension + AI.BehaviorTree.Extension; } }


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
                        string destLocalfilename = System.IO.Path.Combine(GetLocalDirectory(), value + AI.BehaviorTree.Extension);
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
            BehaviorTreeNode cloneModel = new BehaviorTreeNode();
            cloneModel.Name = Name;
            BehaviorTreeNodeViewModel btree = new BehaviorTreeNodeViewModel(copyParent, cloneModel);
            return btree;
        }

    }
    #endregion
}
