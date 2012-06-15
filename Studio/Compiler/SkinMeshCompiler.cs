using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.Animation;

namespace Skill.Studio.Compiler
{
    public class SkinMeshCompiler : DataCompiler
    {
        private List<Bone> _Bones = new List<Bone>();
        private SkinMesh _Mesh;


        public SkinMeshCompiler(ICollection<CompileError> errors)
            : base(EntityType.SkinMesh, errors)
        {

        }

        protected override void Compile()
        {
            this._Mesh = Node.SavedData as SkinMesh;
            if (this._Mesh == null) return;
            CreateBoenList();
        }

        #region Create list of Bones
        private void CreateBoenList()
        {
            _Bones.Clear();
            AddChilds(_Mesh.Root);
        }

        private void AddChilds(Bone b)
        {
            if (!_Bones.Contains(b)) _Bones.Add(b);
            foreach (var child in b)
            {
                AddChilds(child);
            }
        }
        #endregion

        #region Search For AnimationClip Errors
        private void SearchForAnimationClipErrors()
        {
            foreach (AnimationClip a in _Mesh.Animations)
            {
                if (string.IsNullOrEmpty(a.Name))
                {
                    AddError("There is an AnimationClip with empty name.");
                }
                else
                {
                    int count = _Mesh.Animations.Count(c => c.Name == a.Name);
                    if (count > 1)
                        AddError(string.Format("There are {0} AnimationClip in SkinMesh with same name ({1}).", count, a.Name));

                    if (a.Length <= 0)
                        AddError(string.Format("The AnimationClip {0} has invalid length.(must be more than 0)", a.Name));
                }
            }
        }
        #endregion


        #region Search For Bone Errors

        private void SearchForBoneErrors()
        {
            foreach (Bone b in _Bones)
            {
                if (string.IsNullOrEmpty(b.Name))
                {
                    AddError("There is a Bone with empty name.");
                }
                else
                {
                    foreach (Bone chilBone in b)
                    {
                        int count = b.Count(c => c.Name == chilBone.Name);
                        if (count > 1)
                            AddError(string.Format("There are {0} Bones with same name ({1}).", count, chilBone.Name));
                    }
                }
            }
        }
        #endregion
    }
}
