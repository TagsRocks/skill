using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework.UI;
using Skill.Framework.UI.Extended;
using Skill.Editor.Animation;

namespace Skill.Editor.Animation
{

    public class AnimationClipEditorWindow : EditorWindow
    {
        #region AnimationClipEditorWindow
        private static AnimationClipEditorWindow _Instance;
        public static AnimationClipEditorWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = ScriptableObject.CreateInstance<AnimationClipEditorWindow>();
                return _Instance;
            }
        }

        private static Vector2 Size = new Vector2(800, 600);
        private static Vector2 MinSize = new Vector2(500, 300);

        public AnimationClipEditorWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.title = "Animations";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = MinSize;
            CreateUI();
        }

        void OnFocus()
        {
            if (_Frame != null)
            {
                _RefreshStyles = true;
            }
        }

        void OnLostFocus()
        {
            Save();
        }

        void OnDestroy()
        {
            var selected = Skill.Editor.UI.Extended.InspectorProperties.GetSelected();
            if (selected != null && selected is AnimationClipListItem)
                Skill.Editor.UI.Extended.InspectorProperties.Select(null);
        }

        #endregion

        #region UI
        private bool _RefreshStyles = true;
        private Skill.Editor.UI.EditorFrame _Frame;
        private Skill.Editor.UI.GridSplitter _VSplitter;
        private Skill.Framework.UI.Extended.ListBox _AnimationList;
        private Skill.Editor.UI.Extended.TreeView _TransformTree;

        private Skill.Editor.UI.DropShadowLabel _LblAnimations;
        private Skill.Editor.UI.DropShadowLabel _LblHierarchy;
        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);

            _Frame.Grid.RowDefinitions.Add(20, GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star);

            _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            _Frame.Grid.ColumnDefinitions.Add(2, GridUnitType.Pixel);
            _Frame.Grid.ColumnDefinitions.Add(2, GridUnitType.Star);

            _LblAnimations = new Skill.Editor.UI.DropShadowLabel() { Row = 0, Column = 0, Text = "Animations" };
            _Frame.Controls.Add(_LblAnimations);

            _LblHierarchy = new Skill.Editor.UI.DropShadowLabel() { Row = 0, Column = 2, Text = "Hierarchy" };
            _Frame.Controls.Add(_LblHierarchy);

            _AnimationList = new ListBox() { Row = 1, Column = 0, Margin = new Thickness(2) };
            _AnimationList.DisableFocusable();
            _AnimationList.BackgroundVisible = true;
            _Frame.Controls.Add(_AnimationList);

            _VSplitter = new Skill.Editor.UI.GridSplitter() { Row = 0, RowSpan = 2, Column = 1, Orientation = Orientation.Vertical };
            _Frame.Controls.Add(_VSplitter);

            _TransformTree = new UI.Extended.TreeView() { Row = 1, Column = 2, Margin = new Thickness(2), IsEnabled = false };
            _TransformTree.DisableFocusable();
            _TransformTree.Background.Visibility = Visibility.Visible;
            _TransformTree.SelectedStyle = new GUIStyle();
            _Frame.Controls.Add(_TransformTree);

            _AnimationList.SelectionChanged += _AnimationList_SelectionChanged;

        }

        void _AnimationList_SelectionChanged(object sender, System.EventArgs e)
        {
            _TransformTree.IsEnabled = _AnimationList.SelectedItem != null;
            if (_AnimationList.SelectedItem != null)
                Skill.Editor.UI.Extended.InspectorProperties.Select((AnimationClipListItem)_AnimationList.SelectedItem);
            RefreshTree();
        }

        void OnGUI()
        {
            if (_Frame != null)
            {
                RefreshStyles();
                _Frame.Update();
                _Frame.OnGUI();
            }
        }
        private void RefreshStyles()
        {
            if (_RefreshStyles)
            {
                _RefreshStyles = false;
                _VSplitter.Style = Skill.Editor.Resources.Styles.VerticalSplitter;
                _AnimationList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
                _AnimationList.Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;
                _TransformTree.Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;
            }
        }

        #endregion


        private Dictionary<string, MixingTransformData> _MixingTransforms = new Dictionary<string, MixingTransformData>();
        private Dictionary<string, BaseControl> _TreeMap = new Dictionary<string, BaseControl>();
        private bool _IgnoreChanges;
        private SkinMeshAsset _Asset;
        private SkinMeshData _SkinMeshData;
        public SkinMeshData Skin { get { return _SkinMeshData; } }
        public SkinMeshAsset Asset
        {
            get { return _Asset; }
            set
            {
                if (_Asset != value)
                {
                    _Asset = value;
                    _SkinMeshData = null;
                    Rebuild();
                }
            }
        }

        private void Save()
        {
            if (_Asset != null && _SkinMeshData != null)
            {
                _SkinMeshData.Name = _Asset.name;
                _Asset.Save(_SkinMeshData);
            }
        }

        private void Rebuild()
        {
            Clear();
            if (_Asset != null)
                _SkinMeshData = _Asset.Load();

            if (_SkinMeshData.Animations != null)
            {
                for (int i = 0; i < _SkinMeshData.Animations.Length; i++)
                {
                    AnimationClipData anim = _SkinMeshData.Animations[i];
                    AnimationClipListItem item = new AnimationClipListItem(anim) { Text = anim.Name , UserData = anim };
                    _AnimationList.Items.Add(item);
                }
            }

            if (_SkinMeshData.Root != null)
            {
                CreateTree(_SkinMeshData.Root, _TransformTree);
            }
        }
        private void CreateTree(BoneData bone, Panel panel)
        {
            if (bone.Count == 0)
            {
                Skill.Editor.UI.ToggleButton item = new UI.ToggleButton() { Left = true, Margin = new Thickness(10, 0, 0, 0) };
                item.Label.text = bone.Name;
                item.UserData = bone;
                item.Changed += Bone_Changed;
                _TreeMap.Add(bone.RootPath, item);
                panel.Controls.Add(item);

            }
            else if (bone.Count > 0)
            {
                TreeViewFolder item = new TreeViewFolder(bone.Name);
                item.UserData = item.Toggle.UserData = bone;
                item.Toggle.Changed += Bone_Changed;
                _TreeMap.Add(bone.RootPath, item);
                panel.Controls.Add(item);
                foreach (var b in bone)
                    CreateTree(b, item);
            }
        }

        void Bone_Changed(object sender, System.EventArgs e)
        {
            if (_AnimationList.SelectedItem == null || _IgnoreChanges) return;
            AnimationClipData data = ((AnimationClipListItem)_AnimationList.SelectedItem).Data;
            if (data != null)
            {
                Skill.Editor.UI.ToggleButton item = (Skill.Editor.UI.ToggleButton)sender;
                if (item.IsChecked)
                {
                    if (item.UserData == item.Parent.UserData)
                        AddMixingTransformRecursive(item.Parent as BaseControl);
                    else
                        AddMixingTransform(item);
                }
                else
                {
                    if (item.UserData == item.Parent.UserData)
                        RemoveMixingTransformRecursive(item.Parent as BaseControl);
                    else
                        RemoveMixingTransform(item);

                }

                data.MixingTransforms = new MixingTransformData[_MixingTransforms.Count];
                _MixingTransforms.Values.CopyTo(data.MixingTransforms, 0);

                RefreshToggles();
            }
        }
        private void Clear()
        {
            _AnimationList.Items.Clear();
            _TransformTree.Controls.Clear();
            _TreeMap.Clear();
        }
        private void RefreshTree()
        {
            _MixingTransforms.Clear();
            if (_AnimationList.SelectedItem != null)
            {
                AnimationClipData data = _AnimationList.SelectedItem.UserData as AnimationClipData;
                if (data != null && data.MixingTransforms != null)
                {
                    foreach (var ms in data.MixingTransforms)
                    {
                        if (ms.Recursive)
                        {
                            BaseControl c;
                            if (_TreeMap.TryGetValue(ms.TransformPath, out c))
                            {
                                AddMixingTransformRecursive(c);
                            }
                        }
                        else
                        {
                            AddMixingTransform(ms);
                        }
                    }
                    RefreshToggles();
                }
                else
                {
                    UncheckTree();
                }
            }
            else
            {
                UncheckTree();
            }
            Repaint();
        }

        private void RefreshToggles()
        {
            _IgnoreChanges = true;
            foreach (var c in _TransformTree.Controls)
                RefreshTree(c);
            _IgnoreChanges = false;
        }

        private void RefreshTree(BaseControl control)
        {
            if (control is Skill.Editor.UI.ToggleButton)
            {
                ((Skill.Editor.UI.ToggleButton)control).IsChecked = _MixingTransforms.ContainsKey(((BoneData)control.UserData).RootPath);
            }
            else if (control is TreeViewFolder)
            {
                ((TreeViewFolder)control).Toggle.IsChecked = _MixingTransforms.ContainsKey(((BoneData)control.UserData).RootPath);
                foreach (var c in ((TreeViewFolder)control).Controls)
                    RefreshTree(c);
            }
        }


        private void UncheckTree()
        {
            if (_TransformTree.Controls.Count > 0)
            {
                foreach (var item in _TransformTree.Controls)
                {
                    UncheckTree(item);
                }
            }
        }
        private void UncheckTree(BaseControl control)
        {
            if (control is Skill.Editor.UI.ToggleButton)
            {
                ((Skill.Editor.UI.ToggleButton)control).IsChecked = false;
            }
            else if (control is TreeViewFolder)
            {
                ((TreeViewFolder)control).Toggle.IsChecked = false;
                foreach (var c in ((TreeViewFolder)control).Controls)
                    UncheckTree(c);
            }
        }

        private void AddMixingTransform(MixingTransformData ms)
        {
            if (!_MixingTransforms.ContainsKey(ms.TransformPath))
                _MixingTransforms.Add(ms.TransformPath, ms);
        }

        private void AddMixingTransform(BaseControl control)
        {
            if (!_MixingTransforms.ContainsKey(((BoneData)control.UserData).RootPath))
            {
                MixingTransformData ms = new MixingTransformData();
                ms.Recursive = false;
                ms.TransformPath = ((BoneData)control.UserData).RootPath;
                _MixingTransforms.Add(ms.TransformPath, ms);
            }
        }
        private void AddMixingTransformRecursive(BaseControl control)
        {
            AddMixingTransform(control);
            if (control is Panel)
            {
                foreach (var c in ((Panel)control).Controls)
                    AddMixingTransformRecursive(c);
            }
        }

        private void RemoveMixingTransform(BaseControl control)
        {
            _MixingTransforms.Remove(((BoneData)control.UserData).RootPath);
        }
        private void RemoveMixingTransformRecursive(BaseControl control)
        {
            _MixingTransforms.Remove(((BoneData)control.UserData).RootPath);
            if (control is Panel)
            {
                foreach (var c in ((Panel)control).Controls)
                    RemoveMixingTransformRecursive(c);
            }
        }
    }

    class TreeViewFolder : Skill.Editor.UI.Extended.FolderView
    {
        public Skill.Editor.UI.ToggleButton Toggle { get; private set; }

        public TreeViewFolder(string label)
        {
            this.Foldout.Content.text = string.Format("   {0}", label);
            this.Foldout.IsOpen = true;
            Toggle = new UI.ToggleButton() { Parent = this };
        }

        protected override void OnRenderAreaChanged()
        {
            base.OnRenderAreaChanged();
            Rect rect = RenderArea;
            rect.x += 11;
            rect.width = 16;
            rect.height = 16;
            Toggle.RenderArea = rect;
        }

        protected override void Render()
        {
            Toggle.OnGUI();
            base.Render();
        }
    }

    class AnimationClipListItem : Label, Skill.Editor.UI.Extended.IProperties
    {
        public AnimationClipData Data { get; private set; }

        public AnimationClipListItem(AnimationClipData data)
        {
            this.Data = data;
        }


        #region ExposeProperties

        [Skill.Framework.ExposeProperty(10, "Root Motion PX", "")]
        public bool PositionX { get { return Data.RootMotionState.PositionX; } set { Data.RootMotionState.PositionX = value; } }

        [Skill.Framework.ExposeProperty(11, "Root Motion PY", "")]
        public bool PositionY { get { return Data.RootMotionState.PositionY; } set { Data.RootMotionState.PositionY = value; } }

        [Skill.Framework.ExposeProperty(12, "Root Motion PZ", "")]
        public bool PositionZ { get { return Data.RootMotionState.PositionZ; } set { Data.RootMotionState.PositionZ = value; } }

        [Skill.Framework.ExposeProperty(13, "Root Motion RX", "")]
        public bool RotationX { get { return Data.RootMotionState.RotationX; } set { Data.RootMotionState.RotationX = value; } }

        [Skill.Framework.ExposeProperty(14, "Root Motion RY", "")]
        public bool RotationY { get { return Data.RootMotionState.RotationY; } set { Data.RootMotionState.RotationY = value; } }

        [Skill.Framework.ExposeProperty(15, "Root Motion RZ", "")]
        public bool RotationZ { get { return Data.RootMotionState.RotationZ; } set { Data.RootMotionState.RotationZ = value; } }

        #endregion

        public string Title { get { return "AnimationClip"; } }

        private AnimProperties _Properties;
        public UI.Extended.PropertiesPanel Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = new AnimProperties(this);
                return _Properties;
            }
        }

        public bool IsSelectedProperties { get; set; }
        class AnimProperties : Skill.Editor.UI.Extended.ExposeProperties
        {
            public AnimProperties(AnimationClipListItem item)
                : base(item)
            {

            }

            protected override void SetDirty()
            {

            }
        }
    }
}