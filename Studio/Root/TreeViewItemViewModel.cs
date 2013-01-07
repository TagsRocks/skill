using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Skill.Studio
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public abstract class TreeViewItemViewModel : ObservableCollection<TreeViewItemViewModel>
    {
        // just for disable show Count property in PropertyGrid
        [Browsable(false)]        
        public new int Count { get { return base.Count; } }

        /// <summary>
        /// Parent node (can be null if it is root)
        /// </summary>
        [Browsable(false)]        
        public TreeViewItemViewModel Parent { get; private set; }

        /// <summary>
        /// Address of image to show in treeview
        /// </summary>
        [Browsable(false)]
        public abstract string ImageName { get; }

        #region Constructor
        protected TreeViewItemViewModel(TreeViewItemViewModel parent)
        {
            this.Parent = parent;
            _IsExpanded = true;
        } 
        #endregion

        bool _IsExpanded;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                if (value != _IsExpanded)
                {
                    _IsExpanded = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsExpanded"));
                }

                // Expand all the way up to the root.
                if (_IsExpanded && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        private bool _IsSelected;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (value != _IsSelected)
                {
                    _IsSelected = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }


        bool _Editing;
        /// <summary>
        /// true to enabled editing mode
        /// </summary>
        [Browsable(false)]
        public virtual bool Editing
        {
            get { return _Editing; }
            set
            {
                if (_Editing != value)
                {
                    _Editing = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Editing"));
                }
            }
        }

        /// <summary>
        /// true to enable cutting mode
        /// </summary>
        bool _Cutting;
        [Browsable(false)]
        public bool Cutting
        {
            get { return _Cutting; }
            set
            {
                if (_Cutting != value)
                {
                    _Cutting = value; 
                    OnPropertyChanged(new PropertyChangedEventArgs("Cutting"));
                }
            }
        }
    }
}
