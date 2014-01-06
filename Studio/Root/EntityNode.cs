using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using Skill.DataModels;

namespace Skill.Studio
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
        SharedAccessKeys,
        AnimationTree,
        SkinMesh,
        SaveData
    }
    #endregion

    #region FileExtensions
    public static class FileExtensions
    {
        public static string BehaviorTree = ".sbt";
        public static string SharedAccessKeys = ".sky";
        public static string AnimationTree = ".sat";
        public static string SkinMesh = ".ssk";
        public static string AnimationSet = ".sas";

        public static string SaveData = ".ssg";

        public static string GetExtension(EntityType type)
        {
            switch (type)
            {
                case EntityType.BehaviorTree:
                    return BehaviorTree;
                case EntityType.SharedAccessKeys:
                    return SharedAccessKeys;
                case EntityType.AnimationTree:
                    return AnimationTree;
                case EntityType.SkinMesh:
                    return SkinMesh;
                case EntityType.SaveData:
                    return SaveData;
            }
            return string.Empty;
        }
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
            WriteData(e);
            for (int i = 0; i < Count; i++)
                e.Add(_Children[i].ToXElement());
            return e;
        }
        /// <summary> Load from a XElement </summary>
        /// <param name="e">XElement </param>
        public void Load(XElement e)
        {
            this.Name = e.Attribute("Name").Value;
            this.LoadData(e);
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
                        case EntityType.SharedAccessKeys:
                            node = new SharedAccessKeysNode();
                            break;
                        case EntityType.SkinMesh:
                            node = new SkinMeshNode();
                            break;
                        case EntityType.SaveData:
                            node = new SaveDataNode();
                            break;
                        default:
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
        /// allow subclass to write aditional data to save in file
        /// </summary>
        /// <param name="e">XElement</param>
        protected virtual void WriteData(XElement e) { }
        /// <summary>
        /// Allow subclass to load additional data
        /// </summary>
        /// <param name="e">XElement</param>
        protected virtual void LoadData(XElement e) { }
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
        #region Variables
        private object _SavedData;
        #endregion

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
        /// <summary> Saved data </summary>
        [Browsable(false)]
        public object SavedData
        {
            get
            {
                if (_SavedData == null)
                    _SavedData = LoadData();
                return _SavedData;
            }
        }
        /// <summary> Retrieve local path of node based on project directory</summary>
        [Browsable(false)]
        public string LocalPath { get { return System.IO.Path.Combine(GetLocalDirectory(), Name + FileExtension); } }
        /// <summary> Retrieve absolute path of node</summary>
        [Browsable(false)]
        public string AbsolutePath { get { return Project.GetProjectPath(LocalPath); } }
        /// <summary> Retrieve local path of node in based on project directory without extension</summary>
        [Browsable(false)]
        public string LocalPathWithoutExtension { get { return System.IO.Path.Combine(GetLocalDirectory(), Name); } }
        /// <summary> Retrieve extension of data file</summary>
        [Browsable(false)]
        public string FileExtension { get { return FileExtensions.GetExtension(this.EntityType); } }
        #endregion

        #region Events

        public event EventHandler NameChanged;
        protected virtual void OnNameChanged()
        {
            if (NameChanged != null)
                NameChanged(this, EventArgs.Empty);
        }

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
                EntityNodeViewModel vm = CreateViewModel(child);
                if (vm != null)
                    Add(vm);
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
                case EntityType.SharedAccessKeys:
                    return new SharedAccessKeysNodeViewModel(this, (SharedAccessKeysNode)node);
                case EntityType.SkinMesh:
                    return new SkinMeshNodeViewModel(this, (SkinMeshNode)node);
                case EntityType.SaveData:
                    return new SaveDataNodeViewModel(this, (SaveDataNode)node);
                default:
                    throw new InvalidOperationException("Invalid EntityType");
            }            
        }
        #endregion

        #region Browsable properties
        /// <summary> Name of entity </summary>
        [Description("Name of file and class for this entity")]
        public string Name
        {
            get { return Model.Name; }
            set
            {
                if (value != Model.Name)
                {
                    if (EntityType == Studio.EntityType.Folder)
                    {
                        string dir = Project.GetProjectPath(LocalPath);
                        if (System.IO.Directory.Exists(dir))
                        {
                            // change name of directory
                            string destLocaldir = System.IO.Path.Combine(GetLocalDirectory(), value);
                            string destDir = Project.GetProjectPath(destLocaldir);
                            if (System.IO.Directory.Exists(destDir)) return;
                            System.IO.Directory.Move(dir, destDir);
                        }
                    }
                    else
                    {
                        string sourceFilename = Project.GetProjectPath(LocalPath);
                        if (System.IO.File.Exists(sourceFilename))
                        {
                            string destLocalfilename = System.IO.Path.Combine(GetLocalDirectory(), value + FileExtension);
                            string destfilename = Project.GetProjectPath(destLocalfilename);
                            if (System.IO.File.Exists(destfilename)) return;
                            System.IO.File.Move(sourceFilename, destfilename);
                        }
                    }

                    Model.Name = value;
                    // subclass will change name of file
                    // so save project needed
                    Project.Save();
                    this.OnNameChanged();
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
        public EntityNodeViewModel AddNode(Skill.Studio.EntityType type, string name = null)
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
                case EntityType.SharedAccessKeys:
                    node = new SharedAccessKeysNode();
                    break;
                case EntityType.SkinMesh:
                    node = new SkinMeshNode();
                    break;
                case EntityType.SaveData:
                    node = new SaveDataNode();
                    break;
            }
            if (node != null)
            {
                if (!string.IsNullOrEmpty(name))
                    node.Name = name;
                // create view model
                EntityNodeViewModel nodeVM = CreateViewModel(node);
                // find a name that is not in project nodes
                node.Name = Project.CreateValidName(node.Name);
                this.Model.Add(node);
                this.Add(nodeVM);
                nodeVM.New(); // create new entity in project folder
                Project.Save();
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


        public virtual void Delete()
        {
            if (Parent != null) ((EntityNodeViewModel)Parent).RemoveNode(this);
            MainWindow.Instance.CloseDocument(this);
            Project.Delete(LocalPath);
            Project.Save();
        }

        public void MoveTo(EntityNodeViewModel cutParent)
        {
            CopyTo(cutParent, false);
            Delete();
        }

        public void CopyTo(EntityNodeViewModel copyParent, bool saveProject = true)
        {
            var clone = Clone(copyParent);
            copyParent.Model.Add(clone.Model);
            copyParent.Add(clone);

            Project.Copy(this, clone);
            if (saveProject)
                Project.Save();
        }

        public abstract void New();
        public abstract object LoadData();
        public abstract EntityNodeViewModel Clone(EntityNodeViewModel copyParent);
        public virtual void SaveData(object data) { _SavedData = null; }

    }
    #endregion
}
