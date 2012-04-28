using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace Skill.Editor
{


    #region EntityType
    /// <summary>
    /// Defines types of nodes in project
    /// </summary>
    public enum EntityType
    {
        Root,
        Folder,
        BehaviorTree,
        AnimationTree
    }
    #endregion

    #region EntityNode
    /// <summary>
    /// Defines base class for all entity in project
    /// </summary>
    public abstract class EntityNode : ICollection<EntityNode>, IXElement
    {
        #region Variables
        List<EntityNode> _Children;
        #endregion

        #region Properties
        /// <summary> Parent node </summary>
        public EntityNode Parent { get; private set; }
        /// <summary> name of node synced with file on hdd</summary>
        public virtual string Name { get; set; }
        /// <summary> Type of entity </summary>
        public abstract EntityType EntityType { get; }
        #endregion

        #region Save & Load
        /// <summary> Convert to XElement </summary>
        /// <returns>XElement</returns>
        public XElement ToXElement()
        {
            XElement e = new XElement(EntityType.ToString());
            e.SetAttributeValue("Name", Name);
            AddAttributes(e);
            for (int i = 0; i < Count; i++)
                e.Add(_Children[i].ToXElement());
            return e;
        }
        /// <summary> Load from a XElement </summary>
        /// <param name="e">XElement </param>
        public void Load(XElement e)
        {
            this.Name = e.Attribute("Name").Value;
            this.LoadAttributes(e);
            foreach (XElement childE in e.Elements())
            {
                EntityType t;
                if (Enum.TryParse<EntityType>(childE.Name.ToString(), false, out t))
                {
                    EntityNode node = null;
                    switch (t)
                    {
                        case EntityType.Folder:
                            node = new FolderNode();
                            break;
                        case EntityType.BehaviorTree:
                            node = new BehaviorTreeNode();
                            break;
                        case EntityType.AnimationTree:
                            node = new AnimationTreeNode();
                            break;
                    }
                    if (node != null)
                    {
                        node.Load(childE);
                        Add(node);
                    }
                }
            }
        }

        /// <summary>
        /// allow subclass to add aditional data to save in file
        /// </summary>
        /// <param name="e">XElement</param>
        protected virtual void AddAttributes(XElement e) { }
        /// <summary>
        /// Allow subclass to load additional data
        /// </summary>
        /// <param name="e">XElement</param>
        protected virtual void LoadAttributes(XElement e) { }
        #endregion

        #region Constructor
        public EntityNode(string name)
        {
            this.Name = name;
            _Children = new List<EntityNode>();
        }
        #endregion

        #region ICollection

        public void Add(EntityNode item)
        {
            if (!Contains(item))
            {
                item.Parent = this;
                _Children.Add(item);
            }
        }

        public void Clear()
        {
            foreach (var item in _Children)
                item.Parent = null;
            _Children.Clear();
        }

        public bool Contains(EntityNode item)
        {
            return _Children.Contains(item);
        }

        public void CopyTo(EntityNode[] array, int arrayIndex)
        {
            _Children.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Children.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(EntityNode item)
        {
            return _Children.Remove(item);
        }

        public IEnumerator<EntityNode> GetEnumerator()
        {
            return _Children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Children as System.Collections.IEnumerable).GetEnumerator();
        }

        #endregion
    }
    #endregion


    #region EntityNodeViewModel
    /// <summary>
    /// Entity View Model
    /// </summary>
    public abstract class EntityNodeViewModel : TreeViewItemViewModel
    {
        #region Properties
        /// <summary> Project that contains this view model </summary>     
        [Browsable(false)]
        public ProjectViewModel Project { get; private set; }
        /// <summary> Entity view model </summary>
        [Browsable(false)]
        public EntityNode Model { get; private set; }
        /// <summary> Type of entity </summary>
        [Browsable(false)]
        public EntityType EntityType { get { return Model.EntityType; } }
        #endregion

        #region Constructor
        public EntityNodeViewModel(ProjectViewModel project, EntityNode model)
            : this(project, null, model)
        {
        }

        public EntityNodeViewModel(EntityNodeViewModel parent, EntityNode model)
            : this(parent.Project, parent, model)
        {
        }

        private EntityNodeViewModel(ProjectViewModel project, EntityNodeViewModel parent, EntityNode model)
            : base(parent)
        {
            this.Project = project;
            this.Model = model;
            this.LoadChildren();
        }

        // load children nodes
        private void LoadChildren()
        {
            foreach (var child in Model)
            {
                Add(CreateViewModel(child));
            }
        }

        /// <summary>
        /// Create an EntityViewModel
        /// </summary>
        /// <param name="node">Entity model</param>
        /// <returns>EntityNodeViewModel</returns>
        EntityNodeViewModel CreateViewModel(EntityNode node)
        {
            switch (node.EntityType)
            {
                case EntityType.Folder:
                    return new FolderNodeViewModel(this, (FolderNode)node);
                case EntityType.BehaviorTree:
                    return new BehaviorTreeNodeViewModel(this, (BehaviorTreeNode)node);
                case EntityType.AnimationTree:
                    return new AnimationTreeNodeViewModel(this, (AnimationTreeNode)node);
                default:
                    throw new InvalidOperationException("Invalid EntityType");
            }
        }
        #endregion

        #region Browsable properties
        /// <summary> Name of entity </summary>
        [Description("Name of file and class for this entity")]
        public virtual string Name
        {
            get { return Model.Name; }
            set
            {
                if (value != Model.Name)
                {
                    Model.Name = value;
                    // subclass will change name of file
                    // so save project needed
                    Project.Save();
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                }
            }
        }
        #endregion

        #region Add
        /// <summary>
        /// Add new node
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public EntityNodeViewModel AddNode(Editor.EntityType type)
        {
            if (EntityType != EntityType.Folder && EntityType != EntityType.Root)
                return null;
            EntityNode node = null;

            switch (type)
            {
                case EntityType.Folder:
                    node = new FolderNode();
                    break;
                case EntityType.BehaviorTree:
                    node = new BehaviorTreeNode();
                    break;
                case EntityType.AnimationTree:
                    node = new AnimationTreeNode();
                    break;
            }
            if (node != null)
            {
                // create view model
                EntityNodeViewModel nodeVM = CreateViewModel(node);
                // find a name that is not in project nodes
                node.Name = Project.CreateValidName(node.Name);
                this.Model.Add(node);
                this.Add(nodeVM);
                nodeVM.New(); // create new entity in project folder
                return nodeVM;
            }
            return null;
        }
        #endregion

        #region Remove
        /// <summary>
        /// Remove entity from this node and project
        /// </summary>
        /// <param name="entity">entity to remove</param>
        public void RemoveNode(EntityNodeViewModel entity)
        {
            if (EntityType != EntityType.Folder && EntityType != EntityType.Root)
                return;

            if (base.Remove(entity))
            {
                this.Model.Remove(entity.Model);
                entity.Delete();
            }
        }
        #endregion

        #region LocalDirectory
        /// <summary>
        /// Local directory based on root of project
        /// </summary>
        /// <returns>Local Directory</returns>
        public string GetLocalDirectory()
        {
            string dir = "";
            if (Parent != null)
                ((EntityNodeViewModel)Parent).AppendLocalDirectory(ref dir);
            return dir.TrimEnd(new char[] { '\\' });
        }
        private void AppendLocalDirectory(ref string dir)
        {
            if (EntityType == EntityType.Folder)
            {
                if (Parent != null)
                    ((EntityNodeViewModel)Parent).AppendLocalDirectory(ref dir);
                dir = Name + "\\" + dir;
            }
        }
        #endregion

        public virtual void New() { Project.Save(); }
        public virtual void Delete() { if (Parent != null) ((EntityNodeViewModel)Parent).RemoveNode(this); Project.Save(); }
        public virtual string LocalFileName { get { return ""; } }

        private void CloseAll(EntityNodeViewModel node)
        {
            MainWindow.Instance.CloseContent(node.LocalFileName);
            foreach (EntityNodeViewModel item in node)
                CloseAll(item);
        }

        public void MoveTo(EntityNodeViewModel cutParent)
        {
            CloseAll(this);
            CopyTo(cutParent);
            Delete();
        }

        public void CopyTo(EntityNodeViewModel copyParent)
        {
            var clone = Clone(copyParent);
            copyParent.Model.Add(clone.Model);
            copyParent.Add(clone);

            Copy(this, clone);
            Project.Save();
        }

        private void Copy(EntityNodeViewModel source, EntityNodeViewModel clone)
        {
            if (source.EntityType == Editor.EntityType.Folder)
            {
                string sourceFolder = System.IO.Path.Combine(Project.Directory, LocalFileName);
                if (System.IO.Directory.Exists(sourceFolder))
                {
                    string name = Project.CreateValidName(clone.Name);
                    string cloneFolder = System.IO.Path.Combine(Project.Directory, clone.GetLocalDirectory(), name);
                    while (System.IO.Directory.Exists(cloneFolder))
                    {
                        name = "CopyOf" + name;
                        cloneFolder = System.IO.Path.Combine(Project.Directory, clone.GetLocalDirectory(), name);
                    }
                    System.IO.Directory.CreateDirectory(cloneFolder);
                    clone.Model.Name = name;
                    for (int i = 0; i < Count; i++)
                    {
                        EntityNodeViewModel childSource = (EntityNodeViewModel)source[i];
                        EntityNodeViewModel childClone = (EntityNodeViewModel)clone[i];
                        Copy(childSource, childClone);
                    }
                }
            }
            else
            {

                string sourceFileame = System.IO.Path.Combine(Project.Directory, source.LocalFileName);
                if (System.IO.File.Exists(sourceFileame))
                {
                    string ext = System.IO.Path.GetExtension(clone.LocalFileName);
                    string name = Project.CreateValidName(clone.Name);
                    string cloneFileame = System.IO.Path.Combine(Project.Directory, clone.GetLocalDirectory(), name + ext);
                    while (System.IO.File.Exists(cloneFileame))
                    {
                        name = "CopyOf" + name;
                        cloneFileame = System.IO.Path.Combine(Project.Directory, clone.GetLocalDirectory(), name + ext);
                    }
                    System.IO.File.Copy(sourceFileame, cloneFileame);
                    clone.Model.Name = name; // set name of model to aviod moving original file
                }
            }
        }

        public abstract EntityNodeViewModel Clone(EntityNodeViewModel copyParent);
    }
    #endregion
}
