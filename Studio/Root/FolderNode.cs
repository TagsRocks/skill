using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio
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

        public override void SaveData(object data)
        {
            string dir = Project.GetProjectPath(LocalPath);            
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            base.SaveData(data);
        }

        public override void New()
        {
            SaveData(null);
        }

        public override object LoadData()
        {
            return null;
        }
    }
    #endregion
}
