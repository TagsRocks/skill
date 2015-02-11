using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework.UI;
using Skill.Editor.Animation;

namespace Skill.Editor.Animation
{

    public class AnimationTreeEditorWindow : EditorWindow
    {
        #region AnimationTreeEditorWindow
        private static AnimationTreeEditorWindow _Instance;
        public static AnimationTreeEditorWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = ScriptableObject.CreateInstance<AnimationTreeEditorWindow>();
                return _Instance;
            }
        }

        private static Vector2 Size = new Vector2(800, 600);
        private static Vector2 MinSize = new Vector2(500, 300);

        public AnimationTreeEditorWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.title = "AnimationTree";
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
            if (_Graph != null) _Graph.DeselectInspector();
            if (_Parameters != null) _Parameters.DeselectInspector();
            if (_Profiles != null) _Profiles.DeselectInspector();
        }

        void OnEnable()
        {
            if (_Asset != null)
            {
                var temp = _Asset;
                _Asset = null;
                Asset = temp;
            }
        }

        #endregion

        #region UI
        private bool _RefreshStyles = true;
        private Skill.Editor.UI.EditorFrame _Frame;

        private Skill.Framework.UI.Grid _EditPanel;
        private Skill.Framework.UI.Grid _LeftPanel;
        private Skill.Editor.UI.GridSplitter _VSplitter;
        private Skill.Editor.UI.GridSplitter _HSplitter;
        private GraphEditor _Graph;
        private ParameterEditor _Parameters;
        private ProfileEditor _Profiles;
        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
            _Frame.Grid.RowDefinitions.Add(20, GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star);

            _EditPanel = new Grid() { Row = 0, RowSpan = 2 };
            _EditPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(220, GridUnitType.Pixel), MinWidth = 160 }); // left Panel        
            _EditPanel.ColumnDefinitions.Add(2, GridUnitType.Pixel); // VSpliter
            _EditPanel.ColumnDefinitions.Add(2, GridUnitType.Star); // right Panel        
            _Frame.Controls.Add(_EditPanel);

            _VSplitter = new Skill.Editor.UI.GridSplitter() { Row = 1, Column = 1, Orientation = Orientation.Vertical };
            _EditPanel.Controls.Add(_VSplitter);

            _Graph = new GraphEditor(this) { Row = 0, Column = 2 };
            _EditPanel.Controls.Add(_Graph);

            _LeftPanel = new Grid() { Row = 0, Column = 0 };
            _LeftPanel.RowDefinitions.Add(2, GridUnitType.Star);
            _LeftPanel.RowDefinitions.Add(2, GridUnitType.Pixel);
            _LeftPanel.RowDefinitions.Add(1, GridUnitType.Star);
            _EditPanel.Controls.Add(_LeftPanel);

            _Parameters = new ParameterEditor(this) { Row = 0, Column = 0 };
            _LeftPanel.Controls.Add(_Parameters);

            _HSplitter = new Skill.Editor.UI.GridSplitter() { Row = 1, Column = 0, Orientation = Orientation.Horizontal };
            _LeftPanel.Controls.Add(_HSplitter);

            _Profiles = new ProfileEditor(this) { Row = 2, Column = 0 };
            _LeftPanel.Controls.Add(_Profiles);
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
                _Graph.RefreshStyles();
                _Parameters.RefreshStyles();
                _Profiles.RefreshStyles();

                _VSplitter.Style = Skill.Editor.Resources.Styles.VerticalSplitter;
                _HSplitter.Style = Skill.Editor.Resources.Styles.HorizontalSplitter;
            }
        }



        #endregion

        [SerializeField]
        private AnimationTreeAsset _Asset;
        private AnimationTreeData _AnimationTreeData;
        public AnimationTreeData Tree { get { return _AnimationTreeData; } }
        public AnimationTreeAsset Asset
        {
            get { return _Asset; }
            set
            {
                if (_Asset != value)
                {
                    _Asset = value;
                    _AnimationTreeData = null;
                    Rebuild();
                }
            }
        }
        private void Save()
        {
            if (_Asset != null && _AnimationTreeData != null)
            {
                _AnimationTreeData.Name = _Asset.name;
                _Graph.Save();
                _Asset.Save(_AnimationTreeData);
            }
        }
        private void Rebuild()
        {
            _RefreshStyles = true;
            Clear(); 
            if (_Asset != null)
                _AnimationTreeData = _Asset.Load();
            if (_AnimationTreeData != null)
            {
                _Graph.Rebuild();
                _Parameters.Rebuild();
                _Profiles.Rebuild();
            }
        }
        private void Clear()
        {
            _Graph.Clear();
        }


        internal void SelectAnimation(AnimNodeSequenceItem item)
        {
            if (item != null)
            {
                if (_Asset.SkinMesh != null)
                {
                    AnimationSelectorWindow.Instance.ShowAuxWindow();
                    AnimationSelectorWindow.Instance.Reload(_Asset.SkinMesh.Load());
                    AnimationSelectorWindow.Instance.SequenceNode = item;
                }
                else
                {
                    Debug.LogError("You must set valid SkinMesh to AnimationTree before select animation");
                }
            }
        }

        internal void ChangeParameterName(string oldParamName, string newParamName)
        {
            _Graph.ChangeParameterName(oldParamName, newParamName);
        }
    }

}