using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace Skill.Studio.Controls
{
    public class ProjectExplorerViewModel : INotifyPropertyChanged
    {
        #region Context menu

        bool _CanBuild;
        public bool CanBuild
        {
            get { return _CanBuild; }
            set { if (value != _CanBuild) { _CanBuild = value; OnPropertyChanged(new PropertyChangedEventArgs("CanBuild")); } }
        }
        bool _CanAdd;
        public bool CanAdd
        {
            get { return _CanAdd; }
            set { if (value != _CanAdd) { _CanAdd = value; OnPropertyChanged(new PropertyChangedEventArgs("CanAdd")); } }
        }
        bool _CanCut;
        public bool CanCut
        {
            get { return _CanCut; }
            set { if (value != _CanCut) { _CanCut = value; OnPropertyChanged(new PropertyChangedEventArgs("CanCut")); } }
        }
        bool _CanCopy;
        public bool CanCopy
        {
            get { return _CanCopy; }
            set { if (value != _CanCopy) { _CanCopy = value; OnPropertyChanged(new PropertyChangedEventArgs("CanCopy")); } }
        }
        bool _CanPaste;
        public bool CanPaste
        {
            get { return _CanPaste; }
            set { if (value != _CanPaste) { _CanPaste = value; OnPropertyChanged(new PropertyChangedEventArgs("CanPaste")); } }
        }
        bool _CanDelete;
        public bool CanDelete
        {
            get { return _CanDelete; }
            set { if (value != _CanDelete) { _CanDelete = value; OnPropertyChanged(new PropertyChangedEventArgs("CanDelete")); } }
        }
        bool _CanRename;
        public bool CanRename
        {
            get { return _CanRename; }
            set { if (value != _CanRename) { _CanRename = value; OnPropertyChanged(new PropertyChangedEventArgs("CanRename")); } }
        }
        bool _CanProperties;
        public bool CanProperties
        {
            get { return _CanProperties; }
            set { if (value != _CanProperties) { _CanProperties = value; OnPropertyChanged(new PropertyChangedEventArgs("CanProperties")); } }
        }

        #endregion


        ProjectViewModel _Project;
        public ProjectViewModel Project
        {
            get { return _Project; }
            set
            {
                if (_Project != value)
                {
                    _Project = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Project"));                    
                }                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
