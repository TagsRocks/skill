using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor
{
    #region FolderNode
    /// <summary>
    /// Defines a folder in project that contains another entities
    /// </summary>
    public class FolderNode : EntityNode
    {
        public override EntityType EntityType { get { return EntityType.Folder; } }

        public FolderNode()
            : base("NewFolder")
        {

        }
    }
    #endregion

    #region FolderNodeViewModel
    public class FolderNodeViewModel : EntityNodeViewModel
    {
        public FolderNodeViewModel(EntityNodeViewModel parent, FolderNode folder)
            : base(parent, folder)
        {
        }

        public override bool IsExpanded
        {
            get
            {
                return base.IsExpanded;
            }
            set
            {
                if (value != base.IsExpanded)
                {
                    base.IsExpanded = value;
                    OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ImageName"));
                }
            }
        }
        /// <summary>
        /// Image name will chang base on whether folder is expanded or not
        /// </summary>
        public override string ImageName { get { return IsExpanded ? Images.FolderOpen : Images.FolderClosed; } }

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
                    string dir = System.IO.Path.Combine(Project.Directory, LocalFileName);
                    if (System.IO.Directory.Exists(dir))
                    {
                        // change name of directory
                        string destLocaldir = System.IO.Path.Combine(GetLocalDirectory(), value);
                        string destDir = System.IO.Path.Combine(Project.Directory, destLocaldir);
                        if (System.IO.Directory.Exists(destDir)) return;
                        System.IO.Directory.Move(dir, destDir);
                        base.Name = value;
                    }
                }
            }
        }

        /// <summary>
        /// Create new folder in project directory
        /// </summary>
        public override void New()
        {
            string localDir = GetLocalDirectory();
            string dir = Project.Directory;
            if (string.IsNullOrEmpty(localDir))
                dir = System.IO.Path.Combine(dir, Name);
            else
                dir = System.IO.Path.Combine(dir, localDir, Name);

            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            base.New();
        }

        /// <summary>
        /// Delete folder from project directory
        /// </summary>
        public override void Delete()
        {
            string localDir = GetLocalDirectory();
            string dir = Project.Directory;
            if (string.IsNullOrEmpty(localDir))
                dir = System.IO.Path.Combine(dir, Name);
            else
                dir = System.IO.Path.Combine(dir, localDir, Name);

            if (System.IO.Directory.Exists(dir))
                System.IO.Directory.Delete(dir, true);
            base.Delete();
        }

        public override string LocalFileName { get { return System.IO.Path.Combine(GetLocalDirectory(), Name); } }

        public override EntityNodeViewModel Clone(EntityNodeViewModel copyParent)
        {
            FolderNode cloneModel = new FolderNode();
            cloneModel.Name = Name;
            FolderNodeViewModel folder = new FolderNodeViewModel(copyParent, cloneModel);
            foreach (EntityNodeViewModel item in this)
            {
                var childClone = item.Clone(folder);
                folder.Model.Add(childClone.Model);
                folder.Add(childClone);
            }
            return folder;
        }
    }
    #endregion
}
