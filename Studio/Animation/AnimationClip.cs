﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.DataModels.Animation;

namespace Skill.Studio.Animation
{

    #region AnimationClipViewModel
    public class AnimationClipViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// AnimationClip data
        /// </summary>
        [Browsable(false)]
        public AnimationClip Model { get; private set; }

        /// <summary>
        /// SkinMeshViewModel that contains this AnimationClip
        /// </summary>
        [Browsable(false)]
        public SkinMeshViewModel SkinMesh { get; private set; }

        #region Constructors
        /// <summary>
        /// Create an instance of AnimationClip View Model
        /// </summary>        
        /// <param name="skinmesh">SkinMesh that contains AnimationClip</param>
        /// <param name="clip">AnimationClip model</param>
        public AnimationClipViewModel(SkinMeshViewModel skinmesh, AnimationClip clip)
        {
            this.SkinMesh = skinmesh;
            this.Model = clip;
        }

        #endregion

        #region Browsable Properties
        [DisplayName("Name")]
        [Description("Name of AnimationClip.")]
        public string Name
        {
            get { return Model.Name; }
            set
            {
                if (value != Model.Name && !string.IsNullOrEmpty(value))
                {
                    if (!Validation.VariableNameValidator.IsValid(value))
                    {
                        MainWindow.Instance.ShowError("Invalid name");
                        return;
                    }

                    SkinMesh.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    Model.Name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [DisplayName("WrapMode")]
        [Description("WrapMode of AnimationClip.")]
        public WrapMode WrapMode
        {
            get { return Model.WrapMode; }
            set
            {
                if (value != Model.WrapMode)
                {
                    SkinMesh.History.Insert(new ChangePropertyUnDoRedo(this, "WrapMode", value, Model.Name));
                    Model.WrapMode = value;
                    this.OnPropertyChanged("WrapMode");
                }
            }
        }
        #endregion


        public override string ToString()
        {
            return Name;
        }

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
    #endregion



    #region UnDoRedo helper classes
    class AddAnimationClipUnDoRedo : IUnDoRedoCommand
    {
        int _Index;
        AnimationClipViewModel _NewClip;
        SkinMeshViewModel _Parent;
        bool _Reverse;
        public AddAnimationClipUnDoRedo(AnimationClipViewModel newClip, SkinMeshViewModel parent, int index, bool reverse = false)
        {
            this._NewClip = newClip;
            this._Parent = parent;
            this._Reverse = reverse;
            this._Index = index;
        }

        public void Undo()
        {
            if (_Reverse)
                _Parent.Animations.Insert(_Index, _NewClip);
            else
                _Parent.Animations.Remove(_NewClip);
        }

        public void Redo()
        {
            if (_Reverse)
                _Parent.Animations.Remove(_NewClip);
            else
                _Parent.Animations.Insert(_Index, _NewClip);
        }
    }

    #endregion
}
