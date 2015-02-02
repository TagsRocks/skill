using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skill.Editor.Animation
{
    public static class AnimationTreeCompiler
    {
        private static bool _ErrorFound;
        private static AnimationTreeData _Tree;
        private static SkinMeshData _Skin;
        public static bool Compile(AnimationTreeData data, SkinMeshData skin)
        {
            _ErrorFound = false;
            _Tree = data;
            _Skin = skin;

            if (skin == null) return false;

            CheckProfileErrors();
            CheckParameterErrors();
            CheckAnimNodeErrors();
            CheckUnusedAnimNodes();

            _Tree = null;
            _Skin = null;
            return !_ErrorFound;
        }

        private static void CheckParameterErrors()
        {
            if (_Tree.Parameters != null)
            {
                foreach (var p in _Tree.Parameters)
                {
                    int count = _Tree.Parameters.Count(c => c.Name == p.Name);
                    if (count > 1)
                    {
                        Debug.LogError(string.Format("There are more than one parameters with name '{0}'.", p.Name));
                        _ErrorFound = true;
                    }
                }
            }
        }


        private static void CheckProfileErrors()
        {
            if (_Tree.Profiles == null || _Tree.Profiles.Length == 0) return;

            List<string> nameList = new List<string>(_Tree.Profiles.Length);
            foreach (var item in _Tree.Profiles)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    Debug.LogError("There is an AnimationTree Profile with empty name.");
                    _ErrorFound = true;
                }
                else
                {
                    if (nameList.Contains(item.Name)) continue;
                    int count = _Tree.Profiles.Count(c => c.Name == item.Name);
                    if (count > 1)
                    {
                        Debug.LogError(string.Format("There are {0} AnimationTree Profile with same name ({1}).", count, item.Name));
                        _ErrorFound = true;
                    }
                    nameList.Add(item.Name);
                }
                if (string.IsNullOrEmpty(item.Format))
                {
                    Debug.LogError("There is an AnimationTree Profile with empty format.");
                    _ErrorFound = true;
                }
            }
            nameList.Clear();
        }


        private static bool IsAnyNodeConnectedTo(AnimNodeData node)
        {
            if (_Tree.Connections != null)
            {
                foreach (ConnectionData connection in _Tree.Connections)
                {
                    if (connection.End.Id == node.Id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void CheckUnusedAnimNodes()
        {
            foreach (AnimNodeData node in _Tree)
            {
                bool connected = IsAnyNodeConnectedTo(node);
                if (connected == false)
                {
                    if (node.NodeType == AnimNodeType.Root)
                    {
                        Debug.LogError("Root of AnimationTree does not assigned.");
                        _ErrorFound = true;
                    }
                    //else if (node.NodeType != AnimNodeType.Sequence && node.NodeType != AnimNodeType.SubTree)
                    //    Debug.LogWarning(string.Format("There is no connection to AnimNode '{0}'.", node.Name));
                }
            }
        }


        private static void CheckAnimNodeErrors()
        {
            List<string> nameList = new List<string>(_Tree.Count);
            foreach (var item in _Tree)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    Debug.LogError("There is an AnimNode with empty name.");
                    _ErrorFound = true;
                }
                else
                {
                    if (!nameList.Contains(item.Name))
                    {
                        int count = _Tree.Count(c => c.Name == item.Name);
                        if (count > 1)
                        {
                            Debug.LogError(string.Format("There are {0} AnimNodes with same name ({1}).", count, item.Name));
                            _ErrorFound = true;
                        }
                        nameList.Add(item.Name);
                    }
                }

                if (item.NodeType == AnimNodeType.Blend1D)
                {
                    AnimNodeBlend1DData blend1D = item as AnimNodeBlend1DData;
                    for (int i = 0; i < blend1D.Thresholds.Length - 1; i++)
                    {
                        if (blend1D.Thresholds[i] >= blend1D.Thresholds[i + 1])
                        {
                            Debug.LogError(string.Format("Thresholds of AnimNode {0} are not ascendant.", item.Name));
                            _ErrorFound = true;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(blend1D.Parameter))
                    {
                        Debug.LogError(string.Format("Parameter of AnimNode {0} is invalid.", item.Name));
                        _ErrorFound = true;
                    }
                    else if (_Tree.Parameters.Count(c => c.Name == blend1D.Parameter) == 0)
                    {
                        Debug.LogError(string.Format("Parameter of AnimNode {0} is unasigned.", item.Name));
                        _ErrorFound = true;
                    }
                }
                else if (item.NodeType == AnimNodeType.Blend2D)
                {
                    AnimNodeBlend2DData blend2D = item as AnimNodeBlend2DData;
                    if (string.IsNullOrEmpty(blend2D.Parameter1))
                    {
                        Debug.LogError(string.Format("Parameter1 of AnimNode {0} is invalid.", item.Name));
                        _ErrorFound = true;
                    }
                    else if (_Tree.Parameters.Count(c => c.Name == blend2D.Parameter1) == 0)
                    {
                        Debug.LogError(string.Format("Parameter1 of AnimNode {0} is unasigned.", item.Name));
                        _ErrorFound = true;
                    }

                    if (string.IsNullOrEmpty(blend2D.Parameter2))
                    {
                        Debug.LogError(string.Format("Parameter2 of AnimNode {0} is invalid.", item.Name));
                        _ErrorFound = true;
                    }
                    else if (_Tree.Parameters.Count(c => c.Name == blend2D.Parameter2) == 0)
                    {
                        Debug.LogError(string.Format("Parameter2 of AnimNode {0} is unasigned.", item.Name));
                        _ErrorFound = true;
                    }
                }
                else if (item.NodeType == AnimNodeType.Override)
                {
                    AnimNodeOverrideData ov = item as AnimNodeOverrideData;
                    if (string.IsNullOrEmpty(ov.Parameter))
                    {
                        Debug.LogError(string.Format("Parameter of AnimNode {0} is invalid.", item.Name));
                        _ErrorFound = true;
                    }
                    else if (_Tree.Parameters.Count(c => c.Name == ov.Parameter) == 0)
                    {
                        Debug.LogError(string.Format("Parameter of AnimNode {0} is unasigned.", item.Name));
                        _ErrorFound = true;
                    }
                }
                else if (item.NodeType == AnimNodeType.Additive)
                {
                    AnimNodeAdditiveBlendingData additive = item as AnimNodeAdditiveBlendingData;
                    if (string.IsNullOrEmpty(additive.Parameter))
                    {
                        Debug.LogError(string.Format("Parameter of AnimNode {0} is invalid.", item.Name));
                        _ErrorFound = true;
                    }
                    else if (_Tree.Parameters.Count(c => c.Name == additive.Parameter) == 0)
                    {
                        Debug.LogError(string.Format("Parameter of AnimNode {0} is unasigned.", item.Name));
                        _ErrorFound = true;
                    }
                }
                else if (item.NodeType == AnimNodeType.BlendByIndex)
                {
                    AnimNodeBlendByIndexData byIndex = item as AnimNodeBlendByIndexData;
                    if (string.IsNullOrEmpty(byIndex.Parameter))
                    {
                        Debug.LogError(string.Format("Parameter of AnimNode {0} is invalid.", item.Name));
                        _ErrorFound = true;
                    }
                    else if (_Tree.Parameters.Count(c => c.Name == byIndex.Parameter) == 0)
                    {
                        Debug.LogError(string.Format("Parameter of AnimNode {0} is unasigned.", item.Name));
                        _ErrorFound = true;
                    }
                }
                else if (item.NodeType == AnimNodeType.Sequence)
                {
                    AnimNodeSequenceData sequence = item as AnimNodeSequenceData;
                    CheckAnimationClip(sequence);
                }
                //else if (item.NodeType == AnimNodeType.SubTree)
                //{
                //    AnimationTreeNodeViewModel at = Node.Project.GetNode(((AnimNodeSubTree)item).TreeAddress) as AnimationTreeNodeViewModel;
                //    if (at == null)
                //    {
                //        Debug.LogError(string.Format("AnimationTree address of AnimNodeSubTree {0} does not exist.", item.Name));
                //        _ErrorFound = true;
                //    }
                //}
            }
            nameList.Clear();
        }

        private static void CheckAnimationClip(AnimNodeSequenceData sequence)
        {
            if (string.IsNullOrEmpty(sequence.AnimationName))
            {
                Debug.LogError(string.Format("Unassigned AnimationClip for AnimNodeSequence {0}", sequence.Name));
                _ErrorFound = true;
            }
            else if (_Skin.Animations.Count(a => a.Name == sequence.AnimationName) == 0)
            {
                Debug.LogError(string.Format("AnimationClip for AnimNodeSequence {0} is invalid", sequence.Name));
                _ErrorFound = true;
            }
        }
    }

}