using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio
{
    #region ProjectRoot
    /// <summary>
    /// Root node of project
    /// </summary>
    public class ProjectRootNode : EntityNode
    {
        public override EntityType EntityType { get { return EntityType.Root; } }

        public ProjectRootNode()
            : base("Untitled")
        {
        }

    }
    #endregion

    #region ProjectRootNodeViewModel
    public class ProjectRootNodeViewModel : EntityNodeViewModel
    {
        public override string ImageName { get { return Images.Project; } }

        public ProjectRootNodeViewModel(ProjectViewModel project, ProjectRootNode root)
            : base(project, root)
        {
        }

        public override void New() { }

        public override EntityNodeViewModel Clone(EntityNodeViewModel copyParent)
        {
            return null;
        }                

        public override object LoadData()
        {
            return null;
        }

        public override void SaveData(object data)
        {
            base.SaveData(data);
        }
    }
    #endregion
}
