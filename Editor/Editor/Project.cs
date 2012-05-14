using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Skill.Studio
{
    #region ProjectRoot
    /// <summary>
    /// Root node of project
    /// </summary>
    public class ProjectRoot : EntityNode
    {
        public override EntityType EntityType { get { return EntityType.Root; } }

        public ProjectRoot()
            : base("Untitled")
        {
        }

    }
    #endregion

    #region Project
    /// <summary>
    /// Defins a project file and handle save & load
    /// </summary>
    public class Project : IXElement
    {
        #region Variables
        public static int EditorVersion = 1; // current version of editor
        public static string Extension = ".skp"; // extension of priject file
        public static string FilterExtension = "Skill project|*" + Extension; // filer used in OpenFileDialog 
        #endregion

        #region Properties
        /// <summary> Full filename of project file </summary>
        public string FileName { get; set; }
        /// <summary> Name of project file </summary>
        public string Name { get { return Root.Name; } set { Root.Name = value; } }
        /// <summary> Project Settings </summary>
        public ProjectSettings Settings { get; private set; }
        /// <summary> Root node of project </summary>
        public ProjectRoot Root { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a project
        /// </summary>
        public Project()
        {
            this.Settings = new ProjectSettings();
            this.Root = new ProjectRoot();
        }
        #endregion

        #region IXElement methods
        /// <summary>
        /// Create XElement containing project data
        /// </summary>
        /// <returns>XElement</returns>
        public XElement ToXElement()
        {
            XElement project = new XElement("Skill_Project");
            project.SetAttributeValue("Version", EditorVersion);
            project.Add(Settings.ToXElement());
            project.Add(Root.ToXElement());
            return project;
        }
        /// <summary>
        /// Load data from a XElement
        /// </summary>
        /// <param name="e">XElement</param>
        public void Load(XElement e)
        {
            int version = int.Parse(e.Attribute("Version").Value);
            XElement sett = e.Elements().First(p => p.Name == "Settings");
            if (sett != null) Settings.Load(sett);
            XElement root = e.Elements().First(p => p.Name == EntityType.Root.ToString());
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
            // first create a tempfile and save data to it
            // so if somthing goes wrong user donot loos previos version
            string dir = Path.GetDirectoryName(fileName);
            string name = Path.GetFileNameWithoutExtension(fileName);

            int tempPostfix = 0;
            string tempFile = Path.Combine(dir, name + "_Temp" + tempPostfix + Extension);

            while (File.Exists(tempFile))
            {
                tempPostfix++;
                tempFile = Path.Combine(dir, name + "_Temp" + tempPostfix + Extension);
            }

            FileStream file = new FileStream(tempFile, FileMode.Create, FileAccess.Write);
            XDocument document = new XDocument();
            XElement project = ToXElement();
            document.Add(project);
            document.Save(file);
            file.Close();

            // then move tempfile to origin file
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.Move(tempFile, fileName);
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
            FileStream file = File.OpenRead(fileName);
            XDocument document = XDocument.Load(file);
            Project project = new Project();
            project.Load(document.Elements().First());
            project.FileName = fileName;
            project.Name = System.IO.Path.GetFileNameWithoutExtension(fileName);
            file.Close();
            return project;
        }
        #endregion
    }
    #endregion

    #region ProjectRootViewModel
    public class ProjectRootViewModel : EntityNodeViewModel
    {
        public override string ImageName { get { return Images.Project; } }

        public ProjectRootViewModel(ProjectViewModel project, ProjectRoot root)
            : base(project, root)
        {
        }

        public override string LocalFileName { get { return ""; } }
        public override void New() { }
        public override EntityNodeViewModel Clone(EntityNodeViewModel copyParent)
        {
            return null;
        }
    }
    #endregion

    public class ProjectViewModel : INotifyPropertyChanged
    {
        #region Properties
        /// <summary> Directory of project </summary>
        public string Directory
        {
            get
            {
                if (Model.FileName != null)
                {
                    return System.IO.Path.GetDirectoryName(Model.FileName);
                }
                return "";
            }
        }
        /// <summary> Filename of project </summary>
        public string FileName
        {
            get
            {
                return Model.FileName;
            }
            set
            {
                if (value != null && Model.FileName != value)
                {
                    Model.FileName = value;
                    Model.Name = System.IO.Path.GetFileNameWithoutExtension(value);
                    OnPropertyChanged(new PropertyChangedEventArgs("FileName"));
                }
            }
        }
        /// <summary> Project model</summary>
        public Project Model { get; private set; }
        /// <summary> Project Settings model</summary>
        public ProjectSettingsViewModel ProjectSettingsVM { get; private set; }
        /// <summary> Root of project </summary>
        public ProjectRootViewModel RootVM { get; private set; }
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
            this.ProjectSettingsVM = new ProjectSettingsViewModel(project.Settings);
            this.RootVM = new ProjectRootViewModel(this, Model.Root);
            this.Nodes = new ReadOnlyCollection<EntityNodeViewModel>(new EntityNodeViewModel[] { RootVM });
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
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
            return IsNodeExists(RootVM, name);
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
            if (FileName != null)
            {
                Model.Save(FileName);
            }
        }
        #endregion
    }
}
