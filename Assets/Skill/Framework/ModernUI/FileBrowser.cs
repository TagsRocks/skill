using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace Skill.Framework.ModernUI
{
    public class FileBrowser : DynamicBehaviour
    {
        [System.Serializable]
        public class IconInfo
        {
            public string Name;
            public string Extension;
            public Sprite Icon;
        }

        public DynamicScrollView ScrollView;
        public GameObject PathItemPrefab;
        public RectTransform PathContent;
        public string Filter = "*.jpg;*.png";
        //public bool DirectoryMode = false;// set as directory browser
        public Sprite DirectoryIcon;
        public Sprite DefaultIcon;
        public IconInfo[] Icons;


        private class ItemData
        {
            public bool IsDirectory;
            public string Path;
        }

        /// <summary> Defaults to working directory </summary> 
        public string CurrentDirectory
        {
            get
            {
                return _CurrentDirectory;
            }
            set
            {
                if (value == null) return;
                _CurrentDirectory = value;
                ReadDirectoryContents();
            }
        }


        public string[] SelectedFiles
        {
            get
            {
                return _SelectedFiles.ToArray();
            }
            set
            {
                _SelectedFiles.Clear();
                if (value != null && value.Length > 0)
                    _SelectedFiles.AddRange(value);
            }
        }

        // events
        /// <summary> Occurs when Select button clicked </summary>
        public event EventHandler Select;
        /// <summary> Occurs when Select button clicked </summary>
        protected virtual void OnSelect()
        {
            if (Select != null) Select(this, EventArgs.Empty);
        }

        /// <summary> Occurs when Cancel button clicked </summary>
        public event EventHandler Cancel;
        /// <summary> Occurs when Cancel button clicked </summary>
        protected virtual void OnCancel()
        {
            if (Cancel != null) Cancel(this, EventArgs.Empty);
        }


        private string _Filter;
        private string _CurrentDirectory;
        private string[] _CurrentDirectoryParts;
        private List<ItemData> _Items = new List<ItemData>();
        private List<string> _SelectedFiles = new List<string>();


        protected override void Awake()
        {
            base.Awake();
            ScrollView.Initialize = this.Initialize;
        }

        private void Initialize(GameObject obj, int index)
        {
            BrowserListItem item = obj.GetComponent<BrowserListItem>();
            item.Browser = this;
            item.Index = index;
            item.Path = _Items[index].Path;
            item.Title = System.IO.Path.GetFileNameWithoutExtension(item.Path);
            item.IsDirectory = _Items[index].IsDirectory;
            item.IsChecked = _SelectedFiles.Contains(item.Path);
            if (item.IsDirectory)
                item.Icon = DirectoryIcon;
            else
                item.Icon = GetIcon(item.Path);
        }

        public void Up()
        {
            if (_CurrentDirectoryParts != null)
            {
                if (_CurrentDirectoryParts.Length > 1)
                    GoToPathPart(_CurrentDirectoryParts.Length - 2);
            }
        }

        public void CancelClick()
        {
            OnCancel();
        }

        public void SelectClick()
        {
            OnSelect();
        }

        public void ListItemClick(BrowserListItem item)
        {
            if (item != null)
            {
                if (item.IsDirectory)
                    CurrentDirectory = item.Path;
            }
        }

        public void ListItemSelected(BrowserListItem item)
        {
            if (item != null)
            {
                if (!item.IsDirectory)
                {
                    if (item.IsChecked)
                    {
                        if (!_SelectedFiles.Contains(item.Path))
                            _SelectedFiles.Add(item.Path);
                    }
                    else
                    {
                        _SelectedFiles.Remove(item.Path);
                    }
                }
            }
        }

        protected virtual bool IsFileAccepted(FileAttributes att)
        {
            return ((att & FileAttributes.Hidden) == 0) && ((att & FileAttributes.System) == 0);
        }
        protected virtual bool IsDirectoryAccepted(FileAttributes att)
        {
            return ((att & FileAttributes.Hidden) == 0) && ((att & FileAttributes.System) == 0) && ((att & FileAttributes.Encrypted) == 0);
        }

        private void ReadDirectoryContents()
        {
            _Items.Clear();
            ScrollView.Clear();

            if (string.IsNullOrEmpty(_CurrentDirectory)) return;

            if (_CurrentDirectory == "/")
                _CurrentDirectoryParts = new string[] { "" };
            else
                _CurrentDirectoryParts = _CurrentDirectory.Split(Path.DirectorySeparatorChar);

            try
            {
                DirectoryInfo directory = new DirectoryInfo(_CurrentDirectory);
                DirectoryInfo[] dirInfoes = directory.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                if (dirInfoes != null)
                {
                    foreach (var d in dirInfoes)
                    {
                        if (IsDirectoryAccepted(d.Attributes))
                        {
                            _Items.Add(new ItemData() { Path = d.FullName, IsDirectory = true });
                            ScrollView.Add(d.FullName);
                        }
                    }
                }

                if (string.IsNullOrEmpty(Filter))
                    Filter = "*.*";
                string[] filterItems = Filter.Split(';');

                if (filterItems.Length == 0)
                    filterItems = new string[] { "*.*" };

                for (int i = 0; i < filterItems.Length; i++)
                {
                    FileInfo[] fileIndoes = directory.GetFiles(filterItems[i], SearchOption.TopDirectoryOnly);
                    if (fileIndoes != null)
                    {
                        foreach (var f in fileIndoes)
                        {
                            if (IsFileAccepted(f.Attributes))
                            {
                                _Items.Add(new ItemData() { Path = f.FullName, IsDirectory = false });
                                ScrollView.Add(f.FullName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            ReBuildDirectoryParts();
        }

        private void ReBuildDirectoryParts()
        {
            Skill.Framework.Utility.DestroyChildren(PathContent);
            if (_CurrentDirectoryParts != null)
            {
                for (int i = 0; i < _CurrentDirectoryParts.Length; i++)
                {
                    GameObject obj = Instantiate(PathItemPrefab);
                    BrowserPathItem item = obj.GetComponent<BrowserPathItem>();
                    item.Browser = this;
                    item.Index = i;
                    item.Path = _CurrentDirectoryParts[i];
                    item.Index = i;
                    item.transform.SetParent(PathContent, false);
                }
            }
        }

        public void PathButtonClick(BrowserPathItem pathItem)
        {
            GoToPathPart(pathItem.Index);
        }

        private void GoToPathPart(int index)
        {
            string parentDirectoryName = CurrentDirectory;
            for (int i = _CurrentDirectoryParts.Length - 1; i > index; --i)
                parentDirectoryName = Path.GetDirectoryName(parentDirectoryName);
            CurrentDirectory = parentDirectoryName;
        }

        private Sprite GetIcon(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower().TrimStart('.');
            if (!string.IsNullOrEmpty(ext))
            {
                foreach (var item in Icons)
                {
                    if (item.Extension == ext)
                        return item.Icon;
                }
            }
            return DefaultIcon;
        }
    }
}