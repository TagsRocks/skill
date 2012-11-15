using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio.Compiler
{
    public class ProjectCompiler : DataCompiler
    {
        private List<EntityNodeViewModel> _Nodes = new List<EntityNodeViewModel>();

        public ProjectCompiler(ICollection<CompileError> errors)
            : base(EntityType.Root, errors)
        {
        }

        protected override void Compile()
        {
            if (!System.IO.Directory.Exists(Node.Project.Settings.OutputLocaltion))
                AddError(string.Format("Project Output Localtion : '{0}' doesn not exists. fix it in 'Edit->Project Properties' ", Node.Project.Settings.OutputLocaltion));
            CreateNodeList();
            SearchForDuplicateNames(Node as ProjectRootNodeViewModel);
        }


        #region Create list of Nodes
        private void CreateNodeList()
        {
            _Nodes.Clear();
            AddChilds(Node);
        }

        private void AddChilds(EntityNodeViewModel node)
        {
            if (node.EntityType != EntityType.Folder && node.EntityType != EntityType.Root && !_Nodes.Contains(node)) _Nodes.Add(node);
            foreach (EntityNodeViewModel child in node)
            {
                AddChilds(child);
            }
        }
        #endregion


        #region Search for duplicate entities in project

        private void SearchForDuplicateNames(ProjectRootNodeViewModel root)
        {
            foreach (var n in _Nodes)
            {
                int count = _Nodes.Count(c => c.Name == n.Name);
                if (count > 1)
                    AddError(string.Format("There are {0} file in project with same name ({1}).", count, n.Name));
            }
        }
        #endregion
    }
}
