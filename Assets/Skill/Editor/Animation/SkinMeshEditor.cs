using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using Skill.Editor.Animation;
using System.Collections.Generic;
using System.Linq;

namespace Skill.Editor.Animation
{
    [UnityEditor.CustomEditor(typeof(SkinMeshAsset))]
    public class SkinMeshEditor : UnityEditor.Editor
    {
        private SkinMeshAsset _Data;
        private Skill.Framework.UI.Frame _Frame;

        private Skill.Editor.UI.ObjectField<UnityEngine.Animation> _AnimationField;
        private Skill.Editor.UI.ObjectField<UnityEngine.Transform> _RootField;

        private Skill.Framework.UI.Button _BtnRebuildSkeleton;
        private Skill.Framework.UI.Button _BtnImportAnimations;
        private Skill.Framework.UI.Button _BtnEdit;
        private Skill.Framework.UI.Button _BtnBuild;
        private Skill.Editor.UI.TextField _BuildPathField;

        void OnEnable()
        {
            _Data = target as SkinMeshAsset;
            CreateUI();
        }

        private void CreateUI()
        {
            _Frame = new Skill.Framework.UI.Frame("Frame");
            _Frame.Grid.RowDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel); // _RootField
            _Frame.Grid.RowDefinitions.Add(30, Skill.Framework.UI.GridUnitType.Pixel); // _BtnRebuildSkeleton
            _Frame.Grid.RowDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel); // _AnimationField
            _Frame.Grid.RowDefinitions.Add(30, Skill.Framework.UI.GridUnitType.Pixel); // _BtnRebuildAnimations
            _Frame.Grid.RowDefinitions.Add(30, Skill.Framework.UI.GridUnitType.Pixel); // _BtnEdit
            _Frame.Grid.RowDefinitions.Add(30, Skill.Framework.UI.GridUnitType.Pixel); // _BtnBuild
            _Frame.Grid.RowDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel); // _BuildPathField            

            _RootField = new ObjectField<UnityEngine.Transform>() { Row = 0, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _RootField.Label.text = "Root";
            _Frame.Controls.Add(_RootField);

            _BtnRebuildSkeleton = new Framework.UI.Button() { Row = 1, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2), IsEnabled = false };
            _BtnRebuildSkeleton.Content.text = "Rebuild Skeleton";
            _Frame.Controls.Add(_BtnRebuildSkeleton);

            _AnimationField = new ObjectField<UnityEngine.Animation>() { Row = 2, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _AnimationField.Label.text = "Animation";
            _Frame.Controls.Add(_AnimationField);

            _BtnImportAnimations = new Framework.UI.Button() { Row = 3, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2), IsEnabled = false };
            _BtnImportAnimations.Content.text = "Import Animations";
            _Frame.Controls.Add(_BtnImportAnimations);

            _BtnEdit = new Skill.Framework.UI.Button() { Row = 4, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _BtnEdit.Content.text = "Edit Imported Animations";
            _Frame.Controls.Add(_BtnEdit);

            _BtnBuild = new Skill.Framework.UI.Button() { Row = 5, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _BtnBuild.Content.text = "Build";
            _Frame.Controls.Add(_BtnBuild);

            _BuildPathField = new TextField() { Row = 6, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _BuildPathField.Text = _Data.BuildPath;
            _BuildPathField.Label.text = "Path";
            _Frame.Controls.Add(_BuildPathField);

            _AnimationField.ObjectChanged += _AnimationField_ObjectChanged;
            _RootField.ObjectChanged += _RootField_ObjectChanged;
            _BtnRebuildSkeleton.Click += _BtnRebuildSkeleton_Click;
            _BtnImportAnimations.Click += _BtnImportAnimations_Click;
            _BtnBuild.Click += _BtnBuild_Click;
            _BtnEdit.Click += _BtnEdit_Click;
            _BuildPathField.TextChanged += _BuildPathField_TextChanged;
        }

        void _BtnImportAnimations_Click(object sender, System.EventArgs e)
        {
            UpdateAnimations();
        }

        void _BtnRebuildSkeleton_Click(object sender, System.EventArgs e)
        {
            UpdateSkeleton();
        }

        void _RootField_ObjectChanged(object sender, System.EventArgs e)
        {
            _BtnRebuildSkeleton.IsEnabled = _RootField.Object != null;
        }

        void _AnimationField_ObjectChanged(object sender, System.EventArgs e)
        {
            _BtnImportAnimations.IsEnabled = _AnimationField.Object != null;
        }

        void _BtnEdit_Click(object sender, System.EventArgs e)
        {
            AnimationClipEditorWindow.Instance.Show();
            AnimationClipEditorWindow.Instance.Asset = _Data;
        }

        void _BuildPathField_TextChanged(object sender, System.EventArgs e)
        {
            _Data.BuildPath = _BuildPathField.Text;
            EditorUtility.SetDirty(_Data);
        }

        void _BtnBuild_Click(object sender, System.EventArgs e)
        {
            Build();
        }

        public override void OnInspectorGUI()
        {
            _Frame.Update();
            _Frame.OnInspectorGUI(180);
        }


        private void Build()
        {
            SkinMeshData skinData = _Data.Load();
            if (skinData != null)
            {
                Optimize(skinData);
                _Data.Save(skinData);
                Builder.Build(skinData, _Data.BuildPath, _Data.name);
            }
        }

        #region Optimize

        private List<MixingTransformData> _MixList;
        private Dictionary<string, MixingTransformData> _MixingTransforms;
        private BoneData _Root;
        private void Optimize(SkinMeshData skinData)
        {
            if (_MixList == null) _MixList = new List<MixingTransformData>();
            if (_MixingTransforms == null) _MixingTransforms = new Dictionary<string, MixingTransformData>();

            if (skinData.Animations != null && skinData.Root != null)
            {
                _Root = skinData.Root;
                foreach (var anim in skinData.Animations)
                    Optimize(anim);
            }
        }

        private void Optimize(AnimationClipData anim)
        {
            _MixingTransforms.Clear();
            _MixList.Clear();
            if (anim.MixingTransforms != null)
            {
                foreach (var ms in anim.MixingTransforms)
                    _MixingTransforms.Add(ms.TransformPath, ms);

                CheckForRecursive(_Root);
                anim.MixingTransforms = _MixList.ToArray();
            }
        }

        private void CheckForRecursive(BoneData bone)
        {
            if (_MixingTransforms.ContainsKey(bone.RootPath))
            {
                MixingTransformData ms = _MixingTransforms[bone.RootPath];
                _MixList.Add(ms);
                if (!ms.Recursive)
                {
                    if (IsAllChildMixed(bone))
                        ms.Recursive = true;
                }
                if (ms.Recursive)
                {
                    foreach (var b in bone)
                        RemoveRecursive(b);
                    return;
                }
            }

            foreach (var b in bone)
            {
                CheckForRecursive(b);
            }
        }

        private void RemoveRecursive(BoneData bone)
        {
            _MixingTransforms.Remove(bone.RootPath);
            foreach (var b in bone)
                RemoveRecursive(b);
        }

        private bool IsAllChildMixed(BoneData bone)
        {
            if (!_MixingTransforms.ContainsKey(bone.RootPath))
                return false;
            foreach (var b in bone)
            {
                if (!IsAllChildMixed(b))
                    return false;
            }
            return true;
        }

        #endregion


        private Skill.Editor.Animation.AnimationClipData Find(string name, List<AnimationClipData> anims)
        {
            if (anims == null) return null;
            foreach (var item in anims)
            {
                if (item.Name == name)
                    return item;
            }
            return null;
        }

        private void UpdateAnimations()
        {
            if (_AnimationField.Object == null)
            {
                Debug.LogError("Set valid Animation");
            }
            else
            {
                if (_AnimationField.Object != null)
                {
                    SkinMeshData skin = _Data.Load();
                    if (skin.Root == null)
                    {
                        Debug.LogError("First build skeleton");
                        return;
                    }

                    List<AnimationClipData> anims = new List<AnimationClipData>();
                    if (skin.Animations != null && skin.Animations.Length > 0)
                    {
                        foreach (var a in skin.Animations)
                        {
                            if (_AnimationField.Object[a.Name] != null)
                                anims.Add(a);
                        }
                    }

                    foreach (AnimationState state in _AnimationField.Object)
                    {
                        if (state == null || state.clip == null) continue;
                        Skill.Editor.Animation.AnimationClipData clip = Find(state.name, anims);
                        if (clip == null)
                        {
                            clip = new Animation.AnimationClipData()
                            {
                                Name = state.name,
                                WrapMode = (UnityEngine.WrapMode)state.wrapMode,
                            };
                            anims.Add(clip);
                        }
                        clip.Length = state.length;

                        EditorCurveBinding binding = new EditorCurveBinding() { path = skin.Root.Name, type = typeof(Transform) };
                        binding.propertyName = "m_LocalPosition.x";
                        var xCurve = AnimationUtility.GetEditorCurve(state.clip, binding);
                        binding.propertyName = "m_LocalPosition.y";
                        var yCurve = AnimationUtility.GetEditorCurve(state.clip, binding);
                        binding.propertyName = "m_LocalPosition.z";
                        var zCurve = AnimationUtility.GetEditorCurve(state.clip, binding);

                        AddKeys(xCurve, clip.RootMotion.XKeys);
                        AddKeys(yCurve, clip.RootMotion.YKeys);
                        AddKeys(zCurve, clip.RootMotion.ZKeys);


                        binding.propertyName = "m_LocalRotation.x";
                        var rxCurve = AnimationUtility.GetEditorCurve(state.clip, binding);
                        binding.propertyName = "m_LocalRotation.y";
                        var ryCurve = AnimationUtility.GetEditorCurve(state.clip, binding);
                        binding.propertyName = "m_LocalRotation.z";
                        var rzCurve = AnimationUtility.GetEditorCurve(state.clip, binding);

                        AddKeys(rxCurve, clip.RootMotion.RXKeys);
                        AddKeys(ryCurve, clip.RootMotion.RYKeys);
                        AddKeys(rzCurve, clip.RootMotion.RZKeys);
                    }
                    if (anims.Count > 0)
                    {

                        skin.Animations = anims.ToArray();
                        _Data.Save(skin);
                    }
                    else
                        Debug.LogWarning("can not find any AnimationClip");

                }
            }
        }

        private void AddKeys(AnimationCurve curve, Skill.Editor.Animation.KeyframeDataCollection collection)
        {
            collection.Clear();
            if (curve == null) return;
            if (curve.keys == null || curve.keys.Length == 0) return;

            for (int i = 0; i < curve.keys.Length; i++)
            {
                var k = curve.keys[i];
                collection.Add(new Animation.KeyframeData(k.time, k.value, k.inTangent, k.outTangent));
            }
        }

        private void UpdateSkeleton()
        {
            if (_RootField.Object != null)
            {
                Skill.Editor.Animation.BoneData root = CreateBone(_RootField.Object, string.Empty);
                SkinMeshData skin = _Data.Load();
                skin.Root = root;
                _Data.Save(skin);
            }
        }

        private Animation.BoneData CreateBone(UnityEngine.Transform transform, string path)
        {
            Animation.BoneData bone = new Animation.BoneData();
            bone.Name = transform.name;
            bone.RootPath = string.Format("{0}/{1}", path, bone.Name).TrimStart(new char[] { '/' });
            foreach (Transform t in transform)
            {
                bone.Add(CreateBone(t, bone.RootPath));
            }
            return bone;
        }
    }
}
