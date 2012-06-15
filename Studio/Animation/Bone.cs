using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.Animation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace Skill.Studio.Animation
{
    #region BoneViewModel
    public class BoneViewModel : TreeViewItemViewModel
    {
        public override string ImageName { get { return Images.Bone; } }

        /// <summary>
        /// Actual bone data
        /// </summary>
        [Browsable(false)]
        public Bone Model { get; private set; }

        /// <summary>
        /// SkinMeshViewModel that contains this bone
        /// </summary>
        [Browsable(false)]
        public SkinMeshViewModel SkinMesh { get; private set; }


        /// <summary> Address of bone </summary>
        [Browsable(false)]
        public string Address { get; private set; }

        private bool _IsChecked;
        /// <summary> Helper variable for editor </summary>
        [Browsable(false)]
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                BoneViewModel parentBone = Parent as BoneViewModel;
                if (parentBone != null)
                {
                    if (parentBone.Background != Editor.StaticBrushes.BoneSelectedBrush)
                        CheckedEnable = true;
                    else
                        CheckedEnable = false;
                    if (value && !_IsChecked && parentBone.Background == Editor.StaticBrushes.BoneSelectedBrush)
                    {
                        CheckedEnable = false;
                        return;
                    }                    
                }
                else
                    CheckedEnable = true;
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
                }

                if (parentBone != null)
                {
                    if (_IsChecked)
                        Background = Editor.StaticBrushes.BoneSelectedBrush;
                    else
                        Background = parentBone.Background;
                }
                else
                {
                    if (_IsChecked)
                        Background = Editor.StaticBrushes.BoneSelectedBrush;
                    else
                        Background = Editor.StaticBrushes.BoneDefaultBrush;
                }

                foreach (BoneViewModel bone in this)
                {
                    bone.SetChildrenState(_IsChecked);
                }
            }
        }

        private bool _CheckedEnable;
        /// <summary> Helper variable for editor </summary>
        [Browsable(false)]
        public bool CheckedEnable
        {
            get { return _CheckedEnable; }
            set
            {
                if (_CheckedEnable != value)
                {
                    _CheckedEnable = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CheckedEnable"));
                }
            }
        }


        private void SetChildrenState(bool isRootChecked)
        {
            if (isRootChecked && _IsChecked)
            {
                _IsChecked = false;
                OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
            }
            if (isRootChecked)
            {
                Background = Editor.StaticBrushes.BoneSelectedBrush;
                CheckedEnable = false;
            }
            else
            {
                Background = Editor.StaticBrushes.BoneDefaultBrush;
                CheckedEnable = true;
            }

            foreach (BoneViewModel bone in this)
            {
                bone.SetChildrenState(isRootChecked);
            }
        }

        private void SetChildBg(Brush bg)
        {
            this.Background = bg;
            foreach (BoneViewModel bone in this)
            {
                bone.SetChildBg(bg);
            }
        }

        private Brush _Background;
        [Browsable(false)]
        public Brush Background
        {
            get
            {
                if (_Background == null)
                    _Background = Editor.StaticBrushes.BoneDefaultBrush;
                return _Background;
            }
            set
            {
                if (_Background != value)
                {
                    _Background = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Background"));
                }
            }
        }

        #region Constructors
        /// <summary>
        /// Create an instance of Bone View Model
        /// </summary>
        /// <param name="skinmesh">SkinMesh that contains bone</param>
        /// <param name="bone">bone model</param>
        public BoneViewModel(SkinMeshViewModel skinmesh, Bone bone)
            : this(skinmesh, null, bone)
        {
        }
        /// <summary>
        /// Create an instance of Bone View Model. use Parent.BoneTree
        /// </summary>
        /// <param name="parent">parent bone view model</param>
        /// <param name="bone">Bone model</param>
        public BoneViewModel(BoneViewModel parent, Bone bone)
            : this(parent.SkinMesh, parent, bone)
        {
        }
        private BoneViewModel(SkinMeshViewModel skinmesh, BoneViewModel parent, Bone bone)
            : base(parent)
        {
            this.CheckedEnable = true;
            this.IsExpanded = true;
            this.Model = bone;
            this.SkinMesh = skinmesh;
            string parentAddress = parent != null ? parent.Address : "";
            this.Address = string.Format("{0}/{1}", parentAddress, this.Name);
            LoadChildren();
        }

        /// <summary>
        /// create view models for all children
        /// </summary>
        private void LoadChildren()
        {
            //iterate throw children and create appropriate view model
            foreach (var child in Model)
            {
                BoneViewModel childVM = new BoneViewModel(this, child);
                base.Add(childVM);
            }
        }

        #endregion

        #region Browsable Properties
        [DisplayName("Name")]
        [Description("Name of Bone.")]
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
                    if (SkinMesh.History != null)
                        SkinMesh.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    Model.Name = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                }
            }
        }
        #endregion

        #region Editor methods

        /// <summary>
        /// Check where is there a child that contains given bone
        /// </summary>
        /// <param name="bone">Bone</param>
        /// <returns>true if contains, otherwise false</returns>
        bool Contains(Bone bone)
        {
            foreach (BoneViewModel item in this)
                if (item.Model == bone) return true;
            return false;
        }

        private bool CheckAddCauseLoop(BoneViewModel newBone)
        {
            TreeViewItemViewModel parent = this;
            if (CheckAddCauseLoop(parent, newBone))
                return true;
            foreach (BoneViewModel item in newBone)
            {
                if (CheckAddCauseLoop(item))
                    return true;
            }
            return false;
        }
        private bool CheckAddCauseLoop(TreeViewItemViewModel parent, BoneViewModel newBone)
        {
            while (parent != null)
            {
                if (((BoneViewModel)parent).Model == newBone.Model)
                    return true;
                parent = parent.Parent;
            }
            return false;
        }

        /// <summary>
        /// Add a child bone. use this method instead of direct Add method.
        /// this method check additional conditions and take care of history
        /// </summary>
        /// <param name="child">child to add</param>
        /// <param name="duplicate">whther add a duplicated child. when undo or redo we do not need to duplicate child</param>
        /// <param name="index">index to insert child. -1 for add at last</param>
        /// <returns>Actual added child </returns>
        /// <remarks>
        /// we can use one bone more than once in bone tree, so create a duplicate from view model
        /// but not from model and add it to tree
        /// </remarks>
        public BoneViewModel AddBone(BoneViewModel child, bool duplicate = true, int index = -1)
        {
            // ignore it if already have this child. check actual model do to duplication
            if (Contains(child.Model))
                throw new Exception("Already contains this child");

            // check to prevent loop in hierarchy. if a node be twise in hierarchy cause too loop in tree
            if (CheckAddCauseLoop(child))
                throw new Exception("Adding this child cause to loop in tree");


            BoneViewModel toAdd = null;
            if (duplicate)
                toAdd = CloneBone(this, child);
            else
                toAdd = child;
            if (index >= 0 && index < Count)
            {
                this.Model.Insert(index, toAdd.Model);
                this.Insert(index, toAdd);
            }
            else
            {
                this.Model.Add(toAdd.Model);
                this.Add(toAdd);
            }

            if (SkinMesh.History != null)
                SkinMesh.History.Insert(new AddBoneUnDoRedo(toAdd, this, -1));
            return toAdd;
        }

        private BoneViewModel CloneBone(BoneViewModel parent, BoneViewModel bone)
        {
            BoneViewModel vm = new BoneViewModel(parent, new Bone(bone.Model.Name));
            foreach (BoneViewModel child in bone)
            {
                BoneViewModel childVM = CloneBone(vm, child);
                vm.Add(childVM);
            }
            return vm;
        }

        /// <summary>
        /// Create new child (action)
        /// </summary>        
        /// <returns>added child</returns>
        public BoneViewModel AddBone()
        {
            Bone bone = new Bone();
            BoneViewModel boneVM = new BoneViewModel(this, bone);
            return AddBone(boneVM, false, -1);
        }

        /// <summary>
        /// Remove specyfied child
        /// </summary>
        /// <param name="child">child to remove</param>
        /// <returns>true if sucess, otherwise false</returns>
        public bool RemoveBone(BoneViewModel child)
        {
            int index = this.IndexOf(child);
            if (this.Remove(child))
            {
                this.Model.Remove(child.Model);
                if (SkinMesh.History != null)
                    SkinMesh.History.Insert(new AddBoneUnDoRedo(child, this, index, true));
                return true;
            }
            return false;
        }

        #endregion

        public override string ToString()
        {
            return Name + " " + base.ToString();
        }
    }
    #endregion

    #region UnDoRedo helper classes
    class AddBoneUnDoRedo : IUnDoRedoCommand
    {
        int _Index;
        BoneViewModel _NewNode;
        BoneViewModel _Parent;
        bool _Reverse;
        public AddBoneUnDoRedo(BoneViewModel newNode, BoneViewModel parent, int index, bool reverse = false)
        {
            this._NewNode = newNode;
            this._Parent = parent;
            this._Reverse = reverse;
            this._Index = index;
        }

        public void Undo()
        {
            if (_Reverse)
                _Parent.AddBone(_NewNode, false, _Index);
            else
                _Parent.RemoveBone(_NewNode);
        }

        public void Redo()
        {
            if (_Reverse)
                _Parent.RemoveBone(_NewNode);
            else
                _Parent.AddBone(_NewNode, false, _Index);
        }
    }

    #endregion
}
