using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.Animation;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Skill.Studio.Animation
{
    public class SkinMeshViewModel : IDataViewModel
    {
        #region Properties

        /// <summary> SkinMesh model </summary>
        public SkinMesh Model { get; private set; }

        /// <summary> list of all animation in skinmesh</summary>
        public ObservableCollection<AnimationClipViewModel> Animations { get; private set; }

        /// <summary> this list contains root view model as root skinmeshview item</summary>
        public ObservableCollection<BoneViewModel> Nodes { get; private set; }

        /// <summary> root of skinmesh</summary>
        public BoneViewModel Root { get; private set; }

        /// <summary> History to take care of undo and redo</summary>
        public UnDoRedo History { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of SkinMeshViewModel
        /// </summary>
        /// <param name="skinmesh">SkinMesh model</param>        
        public SkinMeshViewModel(SkinMesh skinmesh)
        {
            this.Model = skinmesh;
            Root = new BoneViewModel(this, skinmesh.Root);
            Nodes = new ObservableCollection<BoneViewModel>();
            Nodes.Add(Root);

            Animations = new ObservableCollection<AnimationClipViewModel>();
            if (skinmesh.Animations != null)
            {
                foreach (var clip in skinmesh.Animations)
                {
                    Animations.Add(new AnimationClipViewModel(this, clip));
                }
            }
        }
        #endregion

        #region CreateNewName
        /// <summary>
        /// Create new name that is unique in skinmesh
        /// </summary>
        /// <param name="boneVM">viewmodel to create name for it</param>
        public void CreateNewName(BoneViewModel boneVM)
        {
            var bones = GetBones();
            int i = 1;
            string name = boneVM.Name;
            while (bones.Where(b => b.Name == name).Count() > 0)
            {
                name = boneVM.Name + i++;
            }
            boneVM.Name = name;
        }
        #endregion

        #region GetBones
        /// <summary> list of all bones in skinmesh</summary>
        public ObservableCollection<BoneViewModel> GetBones()
        {
            ObservableCollection<BoneViewModel> bones = new ObservableCollection<BoneViewModel>();
            AddBones(bones, Root);
            return bones;
        }

        private void AddBones(ObservableCollection<BoneViewModel> list, BoneViewModel bone)
        {
            list.Add(bone);
            foreach (BoneViewModel b in bone)
            {
                AddBones(list, b);
            }
        }
        #endregion

        #region Replace Root
        /// <summary>
        /// Replace skeketon
        /// </summary>
        /// <param name="newRoot"></param>
        public void ReplaceRoot(BoneViewModel newRoot)
        {
            BoneViewModel preRoot = Root;
            Root = newRoot;
            Model.Root = newRoot.Model;
            Nodes.Clear();
            Nodes.Add(Root);
            History.Insert(new ReplaceRootBoneUnDoRedo(newRoot, preRoot, this));

        }
        #endregion

        #region Add Animations
        public void AddAnimations(AnimationClipViewModel[] animations)
        {
            foreach (var anim in animations)
            {
                if (!Animations.Contains(anim))
                    Animations.Add(anim);
            }
            this.History.Insert(new AddAnimationsUnDoRedo(animations, this));
        }
        #endregion

        #region IDataViewModel members
        public void NotifyEntityChange(EntityType type, string previousPath, string newPath)
        {
        }

        public void CommiteChanges()
        {
            Model.Animations = new AnimationClip[this.Animations.Count];
            for (int i = 0; i < this.Animations.Count; i++)
            {
                Model.Animations[i] = this.Animations[i].Model;
            }
        }

        public object GetDataModel()
        {
            return Model;
        }

        #endregion
    }

    #region UnDoRedo helper classes
    class ReplaceRootBoneUnDoRedo : IUnDoRedoCommand
    {
        BoneViewModel _NewRoot;
        BoneViewModel _PreRoot;
        SkinMeshViewModel _SkinMesh;

        public ReplaceRootBoneUnDoRedo(BoneViewModel newRoot, BoneViewModel preRoot, SkinMeshViewModel skinmesh)
        {
            this._NewRoot = newRoot;
            this._PreRoot = preRoot;
            this._SkinMesh = skinmesh;
        }

        public void Undo()
        {
            _SkinMesh.ReplaceRoot(_PreRoot);
        }

        public void Redo()
        {
            _SkinMesh.ReplaceRoot(_NewRoot);
        }
    }

    class AddAnimationsUnDoRedo : IUnDoRedoCommand
    {
        AnimationClipViewModel[] _NewAnimations;
        SkinMeshViewModel _SkinMesh;

        public AddAnimationsUnDoRedo(AnimationClipViewModel[] newAnimations, SkinMeshViewModel skinmesh)
        {
            this._NewAnimations = newAnimations;
            this._SkinMesh = skinmesh;
        }

        public void Undo()
        {
            foreach (var anim in _NewAnimations)
            {
                _SkinMesh.Animations.Remove(anim);
            }
        }

        public void Redo()
        {
            foreach (var anim in _NewAnimations)
            {
                if (!_SkinMesh.Animations.Contains(anim))
                    _SkinMesh.Animations.Add(anim);
            }
        }
    }
    #endregion
}
