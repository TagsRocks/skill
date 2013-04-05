using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Skill.DataModels;

namespace Skill.Studio
{

    #region Project
    /// <summary>
    /// Defins a project file and handle save & load
    /// </summary>
    public class Project : IXElement
    {
        #region Variables
        public static int EditorVersion = 2; // current version of editor
        public static string Extension = ".skproj"; // extension of priject file
        public static string FilterExtension = "Skill project|*" + Extension; // filer used in OpenFileDialog 
        public static string GetFilename(string unityProjectDir, string name) { return System.IO.Path.Combine(unityProjectDir, "Assets\\Editor\\Skill", name + Extension); }
        #endregion

        #region Properties
        /// <summary> Full path of project file </summary>
        public string Path { get; set; }
        /// <summary> Name of project file </summary>
        public string Name { get { return Root.Name; } set { Root.Name = value; } }
        /// <summary> Root node of project </summary>
        public ProjectRootNode Root { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a project
        /// </summary>
        public Project()
        {
            this.Root = new ProjectRootNode();
        }
        #endregion

        #region IXElement methods
        /// <summary>
        /// Create XElement containing project data
        /// </summary>
        /// <returns>XElement</returns>
        public XElement ToXElement()
        {
            XElement project = new XElement("Skill_Studio_Project");
            project.SetAttributeValue("Version", EditorVersion);
            project.Add(Root.ToXElement());
            return project;
        }
        /// <summary>
        /// Load data from a XElement
        /// </summary>
        /// <param name="e">XElement</param>
        public void Load(XElement e)
        {
            int version = e.GetAttributeValueAsInt("Version", 2);
            XElement root = e.FindChildByName(EntityType.Root.ToString());
            if (root != null) Root.Load(root);
        }
        #endregion


        #region Save
        /// <summary>
        /// Save project to file
        /// </summary>
        /// <param name="fileName">Full filename</param>
        public void Save(string fileName)
        {
            DataFile data = new DataFile();
            data.Document.Add(ToXElement());
            data.Save(fileName);

            Path = fileName;
            Name = System.IO.Path.GetFileNameWithoutExtension(fileName);
        }
        #endregion

        #region Load
        /// <summary>
        /// Load a project from file
        /// </summary>
        /// <param name="fileName">full filename</param>
        /// <returns>Loaded Project</returns>
        public static Project Load(string fileName)
        {
            DataFile data = new DataFile(fileName);
            Project project = new Project();
            project.Load(data.Root);
            project.Path = fileName;
            project.Name = System.IO.Path.GetFileNameWithoutExtension(fileName);
            return project;
        }
        #endregion
    }
    #endregion


    #region ProjectViewModel
    public class ProjectViewModel : INotifyPropertyChanged
    {
        #region Properties

        public string Name { get { return Model.Name; } }

        public string DesignerName { get { return "Designer"; } }

        public string GifAnimationDirectory
        {
            get
            {
                return System.IO.Path.Combine(this.Directory, "GifAnimations");
            }
        }

        /// <summary> Directory of project </summary>
        public string Directory
        {
            get
            {
                if (Model.Path != null)
                {
                    return System.IO.Path.GetDirectoryName(Model.Path);
                }
                return "";
            }
        }

        /// <summary> Filename of project </summary>
        public string Path
        {
            get
            {
                return Model.Path;
            }
            set
            {
                if (value != null && Model.Path != value)
                {
                    Model.Path = value;
                    Model.Name = System.IO.Path.GetFileNameWithoutExtension(value);
                    OnPropertyChanged("Path");
                }
            }
        }

        /// <summary> Project model</summary>
        public Project Model { get; private set; }
        /// <summary> Root of project </summary>
        public ProjectRootNodeViewModel Root { get; private set; }
        /// <summary> Contains root node to bind to treeview</summary>
        public ReadOnlyCollection<EntityNodeViewModel> Nodes { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create new ProjectViewModel
        /// </summary>
        /// <param name="project">Project model</param>
        public ProjectViewModel(Project project)
        {
            this.Model = project;
            this.Root = new ProjectRootNodeViewModel(this, Model.Root);
            this.Nodes = new ReadOnlyCollection<EntityNodeViewModel>(new EntityNodeViewModel[] { Root });
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion // INotifyPropertyChanged Members

        #region Create Valid Name
        /// <summary>
        /// Check name agains all nodes in project and create new name if exist
        /// </summary>
        /// <param name="name">name template</param>
        /// <returns>new unique name</returns>
        public string CreateValidName(string name)
        {
            string validName = name;
            int i = 0;
            while (IsNodeExists(validName))
            {
                i++;
                validName = name + i;
            }
            return validName;
        }
        /// <summary>
        /// Check whether specified name is valid for a new node in project
        /// </summary>
        /// <param name="name">name to check</param>
        /// <returns>True if valid, otherwise false</returns>
        public bool IsNodeExists(string name)
        {
            return IsNodeExists(Root, name);
        }
        /// <summary>
        /// Check whether specified name is valid for a new node in project
        /// </summary>
        /// <param name="vm">Entityviewmodel to check against and it's children</param>
        /// <param name="name">name to check</param>
        /// <returns>True if valid, otherwise false</returns>
        private bool IsNodeExists(EntityNodeViewModel vm, string name)
        {
            if (name == vm.Name) return true;
            foreach (EntityNodeViewModel item in vm)
            {
                if (IsNodeExists(item, name))
                    return true;
            }
            return false;
        }
        #endregion

        #region Save
        /// <summary>
        /// Save project
        /// </summary>
        public void Save()
        {
            if (Path != null)
            {
                Model.Save(Path);
            }
        }
        #endregion


        #region FindFirstLocalFileName

        public string FindFirstLocalFileName(string name)
        {
            return FindFirstLocalFileName(Root, name);
        }

        private string FindFirstLocalFileName(EntityNodeViewModel node, string name)
        {
            foreach (EntityNodeViewModel child in node)
            {
                if (child.Name == name)
                    return child.LocalPath;
                string result = FindFirstLocalFileName(child, name);
                if (result != null) return result;
            }
            return null;
        }
        #endregion

        #region GetNode

        /// <summary>
        /// Get full path of file in projetc directory
        /// </summary>
        /// <param name="localPath">local path of file</param>
        /// <returns>path</returns>
        public EntityNodeViewModel GetNode(string localPath)
        {
            if (string.IsNullOrEmpty(localPath)) return null;
            return GetNode(Root, localPath);
        }

        private EntityNodeViewModel GetNode(EntityNodeViewModel parentNode, string localPath)
        {
            foreach (EntityNodeViewModel child in parentNode)
            {
                if (child.LocalPath.Equals(localPath, StringComparison.OrdinalIgnoreCase))
                    return child;
                else
                {
                    EntityNodeViewModel result = GetNode(child, localPath);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Get list of localpath of entity nodes with specified type
        /// </summary>
        /// <param name="entityType">Type of nodes to list</param>
        /// <returns>List of localpath</returns>
        public List<string> GetAddresses(EntityType entityType)
        {
            List<string> addresses = new List<string>();

            AddAddressToList(addresses, entityType, Root);

            return addresses;
        }

        private void AddAddressToList(List<string> addresses, EntityType entityType, EntityNodeViewModel node)
        {
            if (node.EntityType == entityType)
                addresses.Add(node.LocalPath);
            foreach (EntityNodeViewModel childNode in node)
            {
                AddAddressToList(addresses, entityType, childNode);
            }
        }


        #endregion

        #region Path

        public string AssetsPath
        {
            get
            {
                int index = this.Path.IndexOf("assets", StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                    return this.Path.Substring(0, index + 6); // 6 : lenght of assets
                return "assets";
            }
        }

        public string EditorPath { get { return System.IO.Path.Combine(AssetsPath, "Editor\\Skill"); } }
        public string ScriptsPath { get { return System.IO.Path.Combine(AssetsPath, "Scripts"); } }

        /// <summary>
        /// Get full path of file in projetc directory
        /// </summary>
        /// <param name="localPath">local path of file</param>
        /// <returns>path</returns>
        public string GetProjectPath(string localPath)
        {
            return System.IO.Path.Combine(Directory, localPath);
        }

        /// <summary>
        /// Get full path of file in output directory
        /// </summary>
        /// <param name="localPath">local path of file</param>
        /// <returns>path</returns>
        public string GetOutputPath(string localPath)
        {
            return System.IO.Path.Combine(ScriptsPath, localPath);
        }

        /// <summary>
        /// Get full path of file in output designer directory
        /// </summary>
        /// <param name="localPath">local path of file</param>
        /// <returns>path</returns>
        public string GetDesignerOutputPath(string localPath)
        {
            return System.IO.Path.Combine(this.ScriptsPath, DesignerName, localPath);
        }

        /// <summary>
        /// Get full path of file in assets/editor directory
        /// </summary>
        /// <param name="localPath">local path of file</param>
        /// <returns>path</returns>
        //public string GetEditorOutputPath(string localPath)
        //{
        //    int index = this.Settings.OutputLocaltion.IndexOf("assets", StringComparison.OrdinalIgnoreCase);
        //    if (index >= 0)
        //    {
        //        string assetsDir = this.Settings.OutputLocaltion.Substring(0, index + 6); // 6 : lenght of assets
        //        return System.IO.Path.Combine(assetsDir, "Editor", localPath);
        //    }

        //    return GetDesignerOutputPath(localPath);
        //}

        /// <summary>
        /// Get full path of file in RequiredFile destination directory
        /// </summary>
        /// <param name="rf">RequiredFile information</param>
        /// <returns>path</returns>        
        public string GetRequiredFilePath(Skill.CodeGeneration.RequiredFile rf)
        {
            if (rf.IsEditor)
                return System.IO.Path.Combine(EditorPath, rf.DestinationDirectory, System.IO.Path.GetFileName(rf.SourceFile));
            else
                return System.IO.Path.Combine(ScriptsPath, DesignerName, rf.DestinationDirectory, System.IO.Path.GetFileName(rf.SourceFile));
        }

        /// <summary>
        /// Get full path of file in gif animation directory
        /// </summary>
        /// <param name="localPath">local path of gif file</param>
        /// <returns>path</returns>
        public string GetAnimationPath(string localPath)
        {
            return System.IO.Path.Combine(this.GifAnimationDirectory, localPath);
        }


        /// <summary>
        /// Delete file in output and designer directory
        /// </summary>
        /// <param name="localPath">local path of file</param>        
        public void Delete(string localPath)
        {
            string filename = GetProjectPath(localPath);
            string outputFilename = GetOutputPath(localPath);
            string outputDesignerFilename = GetDesignerOutputPath(localPath);

            if (System.IO.Path.HasExtension(filename))
            {
                DeleteFile(filename);
                DeleteFile(outputFilename);
                DeleteFile(outputDesignerFilename);
            }
            else
            {
                DeleteDirectory(filename);
                DeleteDirectory(outputFilename);
                DeleteDirectory(outputDesignerFilename);
            }
        }

        private void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        }
        private void DeleteDirectory(string path)
        {
            if (System.IO.Directory.Exists(path)) System.IO.Directory.Delete(path, true);
        }

        /// <summary>
        /// Create directory of path
        /// </summary>
        /// <param name="path">path</param>
        public void CreateDirectory(string path)
        {
            string dir = null;
            if (System.IO.Path.HasExtension(path))
                dir = System.IO.Path.GetDirectoryName(path);
            else
                dir = path;
            if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
        }
        #endregion

        #region Copy Node

        public void Copy(EntityNodeViewModel source, EntityNodeViewModel clone)
        {
            if (source.EntityType == Skill.Studio.EntityType.Folder)
            {
                string sourceFolder = GetProjectPath(source.LocalPath);
                if (System.IO.Directory.Exists(sourceFolder))
                {
                    clone.Model.Name = "CopyOf" + clone.Model.Name;
                    string cloneFolder = GetProjectPath(clone.LocalPath);
                    while (System.IO.Directory.Exists(cloneFolder))
                    {
                        clone.Model.Name = "CopyOf" + clone.Model.Name;
                        cloneFolder = GetProjectPath(clone.LocalPath);
                    }
                    System.IO.Directory.CreateDirectory(cloneFolder);
                    for (int i = 0; i < source.Count; i++)
                    {
                        EntityNodeViewModel childSource = (EntityNodeViewModel)source[i];
                        EntityNodeViewModel childClone = (EntityNodeViewModel)clone[i];
                        Copy(childSource, childClone);
                    }
                }
            }
            else
            {
                string sourceFileame = GetProjectPath(source.LocalPath);
                if (System.IO.File.Exists(sourceFileame))
                {
                    clone.Model.Name = "CopyOf" + clone.Model.Name; // use this trick to avoid rename original file if they are in same directory
                    string cloneFileame = GetProjectPath(clone.LocalPath);
                    while (System.IO.File.Exists(cloneFileame))
                    {
                        clone.Model.Name = "CopyOf" + clone.Model.Name;
                        cloneFileame = GetProjectPath(clone.LocalPath);
                    }
                    System.IO.File.Copy(sourceFileame, cloneFileame, false);
                }
            }
        }

        #endregion

    }
    #endregion
}
