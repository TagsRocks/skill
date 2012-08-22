using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.Fbx;

namespace Skill.Studio.Tools
{
    /// <summary>
    /// View model for Behavior
    /// </summary>
    public class SceneNodeViewModel : TreeViewItemViewModel
    {
        #region Properties

        public override string ImageName { get { return ""; } }

        /// <summary>
        /// Actual behavior data
        /// </summary>
        [Browsable(false)]
        public SceneNode Model { get; private set; }

        [Browsable(false)]
        public string DisplayName
        {
            get
            {
                return Model.NameAndAttributeTypeName;
            }
        }

        [Browsable(false)]
        public bool IsMesh { get { return Model.AttributeType == FbxAttributeType.Mesh; } }

        private bool _IsChecked;
        [Browsable(false)]
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
                }
            }
        }        
        #endregion

        #region Constructors
        public SceneNodeViewModel(SceneNodeViewModel parent, SceneNode model)
            : base(parent)
        {
            this.Model = model;
            LoadChildren();
        }

        /// <summary>
        /// create view models for all children
        /// </summary>
        private void LoadChildren()
        {
            //iterate throw children and create appropriate view model
            for (int i = 0; i < Model.Count; i++)
            {
                SceneNodeViewModel child = new SceneNodeViewModel(this, Model[i]);
                base.Add(child);
            }
        }
        #endregion

        #region Browsable Properties

        [DisplayName("Name")]
        [Description("Name of Node.")]
        public string Name
        {
            get { return Model.Name; }
        }

        #endregion

        #region ToString
        public override string ToString()
        {
            return Name;
        }
        #endregion


    }
}
