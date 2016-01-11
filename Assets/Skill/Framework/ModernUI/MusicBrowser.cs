using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Skill.Framework.ModernUI
{
    public class MusicBrowser : MonoBehaviour
    {
        public DynamicScrollView ScrollView;
        public GameObject Loading;
        public string Filter = "*.mp3";
        public string[] Roots = new string[] { "mnt" };
        public string FileName = "AudioFiles.xml";
        public System.Threading.ThreadPriority Priority = System.Threading.ThreadPriority.Normal;


        private bool _IsScaning;
        private bool _BreakScan;
        private System.Threading.Thread _Thread;
        private bool _ScanBegined;
        private bool _ScanEnded;

        private List<string> _FileToAdd = new List<string>();
        private List<string> _Files = new List<string>();
        private List<string> _Logs = new List<string>();

        public string[] SelectedFiles
        {
            get
            {
                List<string> files = new List<string>();
                for (int i = 0; i < ScrollView.Count; i++)
                {
                    GameObject obj = ScrollView.GetObject(i);
                    if (obj != null)
                    {
                        MusicListItem item = obj.GetComponent<MusicListItem>();
                        if (item != null && item.IsChecked)
                            files.Add(item.Path);
                    }
                }
                return files.ToArray();
            }
        }

        protected virtual void OnDestroy()
        {
            _BreakScan = true;
        }

        private void Initialize(GameObject obj, int index)
        {
            MusicListItem item = obj.GetComponent<MusicListItem>();
            if (item != null)
            {
                item.Path = _Files[index];
                item.Index = index;
            }
        }

        protected virtual void Awake()
        {
            ScrollView.Initialize = this.Initialize;
            if (Loading != null) Loading.SetActive(false);
            LoadFiles();
        }

        protected virtual void Update()
        {
            if (_ScanBegined)
            {
                _ScanBegined = false;
                if (Loading != null) Loading.SetActive(true);
            }
            if (_ScanEnded)
            {
                _ScanEnded = false;
                if (Loading != null)
                    Loading.SetActive(false);
                SaveFiles();

            }

            if (_FileToAdd.Count > 0)
            {
                lock (_FileToAdd)
                {
                    foreach (var item in _FileToAdd)
                    {
                        ScrollView.Add(item);
                    }
                    _FileToAdd.Clear();
                }
            }
            if (_Logs.Count > 0)
            {
                lock (_Logs)
                {
                    foreach (var item in _Logs)
                    {
                        Debug.LogWarning(item);
                    }
                    _Logs.Clear();
                }
            }
        }
        // events
        /// <summary> Occurs when Select button clicked </summary>
        public event EventHandler Select;
        /// <summary> Occurs when Select button clicked </summary>
        protected virtual void OnSelect()
        {
            _BreakScan = true;
            if (Select != null) Select(this, EventArgs.Empty);
        }

        /// <summary> Occurs when Cancel button clicked </summary>
        public event EventHandler Cancel;
        /// <summary> Occurs when Cancel button clicked </summary>
        protected virtual void OnCancel()
        {
            _BreakScan = true;
            if (Cancel != null) Cancel(this, EventArgs.Empty);
        }

        public void CancelClick()
        {
            OnCancel();
        }

        public void SelectClick()
        {
            OnSelect();
        }

        protected virtual bool IsFileAccepted(FileAttributes att)
        {
            return ((att & FileAttributes.Hidden) == 0) && ((att & FileAttributes.System) == 0);
        }
        protected virtual bool IsDirectoryAccepted(FileAttributes att)
        {
            return ((att & FileAttributes.Hidden) == 0) && ((att & FileAttributes.System) == 0) && ((att & FileAttributes.Encrypted) == 0);
        }



        public void StopScane()
        {
            _BreakScan = true;
        }

        public void StartScan()
        {
            if (_IsScaning) return;
            _BreakScan = false;
            _FileToAdd.Clear();
            _Files.Clear();
            ScrollView.Clear();
            _Thread = new System.Threading.Thread(new System.Threading.ThreadStart(Scan));
            _Thread.Priority = Priority;
            _Thread.Start();
        }

        private void Scan()
        {
            if (string.IsNullOrEmpty(Filter))
                Filter = "*.mp3";
            string[] filterItems = Filter.Split(';');

            if (filterItems.Length == 0)
                filterItems = new string[] { "*.mp3" };

            _ScanBegined = true;
            _IsScaning = true;
            Queue<string> directoriesQueue = new Queue<string>();
            foreach (var r in Roots)
                directoriesQueue.Enqueue(r);

            try
            {
                while (directoriesQueue.Count > 0)
                {
                    if (_BreakScan) break;

                    string dir = directoriesQueue.Dequeue();
                    if (!Directory.Exists(dir)) continue;
                    DirectoryInfo directory = new DirectoryInfo(dir);
                    try
                    {
                        DirectoryInfo[] dirInfoes = directory.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                        if (dirInfoes != null)
                        {
                            foreach (var d in dirInfoes)
                            {
                                if (IsDirectoryAccepted(d.Attributes))
                                {
                                    directoriesQueue.Enqueue(d.FullName);
                                }
                            }
                        }
                    }
                    catch (UnauthorizedAccessException uaex)
                    {
                        lock (_Logs)
                        {
                            _Logs.Add(uaex.ToString());                            
                        }
                        continue;
                    }

                    for (int i = 0; i < filterItems.Length; i++)
                    {
                        FileInfo[] fileIndoes = directory.GetFiles(filterItems[i], SearchOption.TopDirectoryOnly);
                        if (fileIndoes != null)
                        {
                            lock (_FileToAdd)
                            {
                                foreach (var f in fileIndoes)
                                {
                                    if (IsFileAccepted(f.Attributes))
                                    {
                                        _Files.Add(f.FullName);
                                        _FileToAdd.Add(f.FullName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (_Logs)
                {
                    _Logs.Add(ex.ToString());
                    _Logs.Add(ex.StackTrace);
                }
            }
            _IsScaning = false;
            _ScanEnded = true;
        }        

        private static char[] InvalidXmlCharacters = new char[] { '&', '<', '>', '"', '\'' };
        private static string[] ValidXmlCharacters = new string[] { "&amp;", "&lt;", "&gt;", "&quot;", "&apos;" };

        private void SaveFiles()
        {
            if (string.IsNullOrEmpty(FileName))
                FileName = "AudioFiles.xml";

            string path = System.IO.Path.Combine(Application.persistentDataPath, FileName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            FileList fl = new FileList();
            fl.Files = _Files.ToArray();

            // safe files for xml
            for (int i = 0; i < fl.Files.Length; i++)
            {
                if (fl.Files[i].IndexOfAny(InvalidXmlCharacters) >= 0)
                {
                    for (int j = 0; j < InvalidXmlCharacters.Length; j++)
                    {
                        if (fl.Files[i].IndexOf(InvalidXmlCharacters[j]) >= 0)
                            fl.Files[i] = fl.Files[i].Replace(InvalidXmlCharacters[j].ToString(), ValidXmlCharacters[j]);
                    }
                }
            }

            Skill.Framework.IO.PCSaveGame.SaveToXmlFile(fl, path);
        }

        private void LoadFiles()
        {
            if (string.IsNullOrEmpty(FileName))
                FileName = "AudioFiles.xml";

            string path = System.IO.Path.Combine(Application.persistentDataPath, FileName);

            if (System.IO.File.Exists(path))
            {
                _FileToAdd.Clear();
                _Files.Clear();
                ScrollView.Clear();

                FileList fl = new FileList();
                Skill.Framework.IO.PCSaveGame.LoadFromXmlFile(fl, path);

                if (fl.Files != null)
                {
                    foreach (var f in fl.Files)
                    {
                        if (System.IO.File.Exists(f))
                        {
                            _FileToAdd.Add(f);
                            _Files.Add(f);
                        }
                    }
                }
            }
        }






        [System.Serializable]
        public class FileList : Skill.Framework.IO.ISavable
        {

            // Variables
            private bool _IsDirty;
            private string[] _Files;

            // Properties
            /// <summary> is any changes happened to savable object </summary>
            public bool IsDirty
            {
                get
                {
                    return _IsDirty;
                }
            }
            public string[] Files { get { return _Files; } set { if (_Files != value) { _Files = value; _IsDirty = true; } } }

            // Methods
            public FileList()
            {
            }
            public void SetAsClean()
            {
                _IsDirty = false;

            }
            public static FileList CreateFileList()
            {
                return new FileList();
            }
            public void Save(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
            {
                Skill.Framework.IO.XmlElement _FilesElement = stream.Create("Files", _Files);
                e.AppendChild(_FilesElement);

            }
            public void Save(Skill.Framework.IO.BinarySaveStream stream)
            {
                stream.Write(_Files);

            }
            public void Load(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
            {
                Skill.Framework.IO.XmlElement element = e.FirstChild as Skill.Framework.IO.XmlElement;
                while (element != null)
                {
                    switch (element.Name)
                    {
                        case "Files":
                            this._Files = stream.ReadStringArray(element);
                            break;
                    }
                    element = e.GetNextSibling(element);
                }
                SetAsClean();

            }
            public void Load(Skill.Framework.IO.BinaryLoadStream stream)
            {
                this._Files = stream.ReadStringArray();
                SetAsClean();

            }

        }
    }
}