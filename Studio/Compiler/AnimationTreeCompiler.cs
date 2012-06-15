using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.Animation;

namespace Skill.Studio.Compiler
{
    public class AnimationTreeCompiler : DataCompiler
    {
        private AnimationTree _Tree;
        private SkinMesh _SkinMesh;

        public AnimationTreeCompiler(ICollection<CompileError> errors)
            : base(EntityType.AnimationTree, errors)
        {

        }

        protected override void Compile()
        {
            this._Tree = Node.SavedData as AnimationTree;
            if (this._Tree == null) return;
            CheckSkinMeshAddress();
            if (_SkinMesh == null) return;
            CheckProfileErrors();
            CheckAnimNodeErrors();
            CheckUnusedAnimNodes();
        }

        #region Check Profile Errors
        private void CheckProfileErrors()
        {
            if (_Tree.Profiles == null || _Tree.Profiles.Length == 0) return;

            List<string> nameList = new List<string>(_Tree.Profiles.Length);
            foreach (var item in _Tree.Profiles)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    AddError("There is an AnimationTree Profile with empty name.");
                }
                else
                {
                    if (nameList.Contains(item.Name)) continue;
                    int count = _Tree.Profiles.Count(c => c.Name == item.Name);
                    if (count > 1)
                        AddError(string.Format("There are {0} AnimationTree Profile with same name ({1}).", count, item.Name));
                    nameList.Add(item.Name);
                }
                if (string.IsNullOrEmpty(item.Format))
                {
                    AddError("There is an AnimationTree Profile with empty format.");
                }
            }
            nameList.Clear();
        }
        #endregion

        #region Check SkinMesh Address
        private void CheckSkinMeshAddress()
        {
            if (string.IsNullOrEmpty(_Tree.SkinMesh))
                AddError("SkinMesh of AnimationTree is unasigned.");
            else
            {
                SkinMeshNodeViewModel skinMesh = Node.Project.GetNode(_Tree.SkinMesh) as SkinMeshNodeViewModel;
                if (skinMesh != null)
                    _SkinMesh = skinMesh.SavedData as SkinMesh;
                else
                    AddError("SkinMesh of AnimationTree does not exist.");
            }
        }
        #endregion

        #region Check for Unused AnimNodes

        private bool IsAnyNodeConnectedTo(AnimNode node)
        {
            foreach (AnimationConnection connection in _Tree.Connections)
            {
                if (connection.Sink.Id == node.Id)
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckUnusedAnimNodes()
        {
            foreach (AnimNode node in _Tree)
            {
                bool connected = IsAnyNodeConnectedTo(node);
                if (connected == false)
                {
                    if (node.NodeType == AnimNodeType.Root)
                        AddError("Root of AnimationTree does not assigned.");
                    else if (node.NodeType != AnimNodeType.Sequence)
                        AddWarning(string.Format("There is no connection to AnimNode '{0}'.", node.Name));
                }
            }
        }

        #endregion

        #region Check AnimNode Errors
        private void CheckAnimNodeErrors()
        {
            List<string> nameList = new List<string>(_Tree.Count);
            foreach (var item in _Tree)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    AddError("There is an AnimNode with empty name.");
                }
                else
                {
                    if (!nameList.Contains(item.Name))
                    {
                        int count = _Tree.Count(c => c.Name == item.Name);
                        if (count > 1)
                            AddError(string.Format("There are {0} AnimNodes with same name ({1}).", count, item.Name));
                        nameList.Add(item.Name);
                    }

                    if (item.BlendTime < 0)
                        AddError(string.Format("BlendTime of AnimNode {0} can not be negative value.", item.Name));
                }

                if (item.NodeType == AnimNodeType.AimOffset)
                {
                    AnimNodeAimOffset aim = item as AnimNodeAimOffset;
                    foreach (var p in aim.Profiles)
                    {
                        if (string.IsNullOrEmpty(p.Name))
                        {
                            AddError("There is an AnimNodeAimOffsetProfile with empty name.");
                        }
                        else
                        {
                            int count = aim.Profiles.Count(c => c.Name == p.Name);
                            if (count > 1)
                                AddError(string.Format("There are {0} AnimNodeAimOffsetProfile with same name ({1}).", count, item.Name));
                        }

                        CheckAnimationClip(p.CenterCenter);
                        CheckAnimationClip(p.CenterUp);
                        CheckAnimationClip(p.CenterDown);

                        CheckAnimationClip(p.LeftCenter);
                        CheckAnimationClip(p.LeftDown);
                        CheckAnimationClip(p.LeftUp);

                        CheckAnimationClip(p.RightCenter);
                        CheckAnimationClip(p.RightDown);
                        CheckAnimationClip(p.RightUp);
                    }
                }
                else if (item.NodeType == AnimNodeType.BlendBySpeed)
                {
                    AnimNodeBlendBySpeed speed = item as AnimNodeBlendBySpeed;
                    if (speed.BlendUpTime < 0)
                        AddError(string.Format("BlendUpTime of AnimNode {0} can not be negative value.", item.Name));
                    if (speed.BlendDownTime < 0)
                        AddError(string.Format("BlendDownTime of AnimNode {0} can not be negative value.", item.Name));
                    if (speed.BlendUpDelay < 0)
                        AddError(string.Format("BlendUpDelay of AnimNode {0} can not be negative value.", item.Name));
                    if (speed.BlendDownDelay < 0)
                        AddError(string.Format("BlendDownDelay of AnimNode {0} can not be negative value.", item.Name));
                    if (speed.BlendDownPercent < 0)
                        AddError(string.Format("BlendDownPercent of AnimNode {0} can not be negative value.", item.Name));

                    for (int i = 0; i < speed.Constraints.Length - 1; i++)
                    {
                        if (speed.Constraints[i] >= speed.Constraints[i + 1])
                        {
                            AddError(string.Format("Constraints of AnimNode {0} are not ascendant.", item.Name));
                            break;
                        }
                    }
                }
                else if (item.NodeType == AnimNodeType.Override)
                {
                    AnimNodeOverride ov = item as AnimNodeOverride;
                    if (ov.OverridePeriod < 0)
                        AddError(string.Format("OverridePeriod of AnimNode {0} can not be negative value.", item.Name));
                }
                else if (item.NodeType == AnimNodeType.Random)
                {
                    AnimNodeRandom random = item as AnimNodeRandom;
                    for (int i = 0; i < random.Chances.Length - 1; i++)
                    {
                        if (random.Chances[i] <= 0)
                        {
                            AddError(string.Format("Chance of AnimNode {0} can not be negative or zero value.", item.Name));
                            break;
                        }
                    }
                }
                else if (item.NodeType == AnimNodeType.Sequence)
                {
                    AnimNodeSequence sequence = item as AnimNodeSequence;
                    CheckAnimationClip(sequence);
                }
            }
            nameList.Clear();
        }

        private void CheckAnimationClip(AnimNodeSequence sequence)
        {
            if (string.IsNullOrEmpty(sequence.AnimationName))
                AddError(string.Format("Unassigned AnimationClip for AnimNodeSequence {0}", sequence.Name));
            else if (_SkinMesh.Animations.Count(a => a.Name == sequence.AnimationName) == 0)
                AddError(string.Format("AnimationClip for AnimNodeSequence {0} is invalid", sequence.Name));
        }
        #endregion
    }
}
