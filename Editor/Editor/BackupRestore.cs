using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

namespace Skill.Editor
{

    public abstract class BackupItem : TreeViewItemViewModel
    {
        public string Path { get; private set; }
        public string Name { get; private set; }

        public abstract DateTime LastModifyTime { get; }

        public BackupItem(BackupItem parent, string path)
            : base(parent)
        {
            this.Path = path;
            this.Name = System.IO.Path.GetFileNameWithoutExtension(path);
        }
    }
    public class BackupFolder : BackupItem
    {
        DirectoryInfo _Info;
        public DirectoryInfo Info
        {
            get
            {
                if (_Info == null)
                {
                    _Info = new DirectoryInfo(Path);
                }
                return _Info;
            }
        }

        public override DateTime LastModifyTime { get { return Info.CreationTime; } }

        public override string ImageName
        {
            get { return IsExpanded ? Images.FolderOpen : Images.FolderClosed; }

        }
        public BackupFolder(BackupFolder parent, string path)
            : base(parent, path)
        {
        }
    }
    public class UserCodeFile : BackupItem
    {
        FileInfo _Info;
        public FileInfo Info
        {
            get
            {
                if (_Info == null)
                {
                    _Info = new FileInfo(Path);
                }
                return _Info;
            }
        }

        public override DateTime LastModifyTime { get { return Info.LastWriteTime; } }

        public string Extension { get; private set; }

        public override string ImageName
        {
            get
            {
                switch (Extension)
                {
                    case ".cs":
                        return Images.CSharpCode;
                }

                return "";
            }
        }

        public UserCodeFile(BackupFolder parent, string path)
            : base(parent, path)
        {
            Extension = System.IO.Path.GetExtension(path);
        }
    }

    public class BackupRestore
    {
        private string _UnityDirectory;
        private string _ProjectName;
        private string _UserCodePostfix;


        public static string BackupRoot
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skill\\Backup");
            }
        }
        public string BackupDirectory { get; private set; }

        private void UpdateData()
        {
            if (MainWindow.Instance.Project == null)
                throw new InvalidOperationException("Can not create instance of backup file when no project loaded");

            this._ProjectName = MainWindow.Instance.Project.Model.Name;
            this._UnityDirectory = Path.GetFullPath(MainWindow.Instance.Project.ProjectSettingsVM.UnityProjectLocaltion);
            this.BackupDirectory = Path.Combine(BackupRoot, _ProjectName);
        }

        public BackupRestore()
        {
            UpdateData();
        }

        private char[] _TrimStartChars = new char[] { '\\', ' ' };

        public void CreateAuto()
        {
            Create("Auto");
        }

        private void Copy(string sourceFile, string destFile)
        {
            if (!File.Exists(sourceFile)) return;
            string dir = Path.GetDirectoryName(destFile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (File.Exists(destFile))
                File.Delete(destFile);

            File.Copy(sourceFile, destFile);
        }


        public bool IsBackupNameValid(string backupName)
        {
            bool valid = true;
            try
            {
                string path = System.IO.Path.Combine("c:\\", backupName);
                System.IO.Path.GetFullPath(path);
            }
            catch
            {
                valid = false;
            }
            return valid;
        }

        public bool IsBackupExists(string backupName)
        {
            string destination = Path.Combine(BackupDirectory, backupName);
            return Directory.Exists(destination);
        }

        public void Create(string backupName)
        {
            if (!Directory.Exists(_UnityDirectory)) return;
            string searchPattern = "*" + _UserCodePostfix + ".cs";
            string[] files = Directory.GetFiles(_UnityDirectory, searchPattern, SearchOption.AllDirectories);
            if (files == null || files.Length == 0) return;

            string destination = Path.Combine(BackupDirectory, backupName);

            foreach (var file in files)
            {
                string localPath = file.Replace(_UnityDirectory, string.Empty).TrimStart(_TrimStartChars);
                string backupFile = Path.Combine(destination, localPath);

                if (File.Exists(backupFile))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    FileInfo backupfileInfo = new FileInfo(backupFile);

                    if (fileInfo.LastWriteTime <= backupfileInfo.LastWriteTime)
                        continue;
                }
                Copy(file, backupFile);
            }
        }


        public BackupItem CreateTree(string backupName)
        {
            string destination = Path.Combine(BackupDirectory, backupName);
            BackupFolder root = new BackupFolder(null, destination);
            

            string searchPattern = "*" + ".cs";
            string[] files = Directory.GetFiles(destination, searchPattern, SearchOption.AllDirectories);

            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    string dir = Path.GetDirectoryName(file);
                    string localdir = dir.Replace(destination, string.Empty).Trim(_TrimStartChars);
                    string[] folders = localdir.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                    BackupFolder parent = CreateFolder(root, folders);
                    if (parent != null)
                    {
                        UserCodeFile uf = new UserCodeFile(parent, file);
                        parent.Add(uf);
                    }
                }
            }
            return root;
        }

        private BackupFolder CreateFolder(BackupFolder root, string[] folders)
        {
            return CreateFolder(root, folders, 0);
        }

        private BackupFolder CreateFolder(BackupFolder parent, string[] folders, int index)
        {
            if (index >= folders.Length) return parent;
            BackupFolder child = Find(parent, folders[index]);
            if (child == null)
            {
                child = new BackupFolder(parent, Path.Combine(parent.Path, folders[index]));
                parent.Add(child);
            }
            return CreateFolder(child, folders, index + 1);
        }

        private BackupFolder Find(BackupFolder parent, string childFolder)
        {
            foreach (var item in parent)
            {
                if (item is BackupFolder)
                {
                    BackupFolder f = (BackupFolder)item;
                    if (f.Name == childFolder) return f;
                }
            }
            return null;
        }

        private void RestoreCode(UserCodeFile code)
        {
            if (!Directory.Exists(_UnityDirectory)) return;

            string path = Path.GetFullPath(code.Path);
            string localPath = path.Replace(BackupDirectory, string.Empty).Trim(_TrimStartChars);
            string destFile = Path.Combine(_UnityDirectory, localPath);
            Copy(code.Path, destFile);
        }

        public void Restore(BackupItem item)
        {
            if (item == null) return;
            else if (item is BackupFolder)
            {
                foreach (var child in item)
                {
                    Restore(child as BackupItem);
                }
            }
            else if (item is UserCodeFile) RestoreCode(item as UserCodeFile);
        }

        public void Delete(BackupItem item)
        {
            if (item == null) return;
            else if (item is BackupFolder) Directory.Delete(item.Path,true);
            else if (item is UserCodeFile) File.Delete(item.Path);
        }
    }
}
