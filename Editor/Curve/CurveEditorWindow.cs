using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using Skill.Editor.UI.Extended;
using System.Reflection;
using System;
using System.Collections.Generic;
using Skill.Framework.UI;

namespace Skill.Editor.Curve
{
    public class CurveEditorWindow : UnityEditor.EditorWindow
    {
        private static Vector2 Size = new Vector2(800, 600);
        private static CurveEditorWindow _Instance;

        public static CurveEditorWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = ScriptableObject.CreateInstance<CurveEditorWindow>();
                return _Instance;
            }
        }

        void OnDestroy()
        {            
            _Instance = null;
        }
        void OnFocus()
        {
            if (_Frame != null)
            {
                _ObjectField.Object = _Object;
                Rebuild();
            }
        }

        [SerializeField]
        private GameObject _Object;

        public GameObject Object
        {
            get { return _Object; }
            set
            {
                if (_Object != value)
                {
                    _Object = value;
                    Rebuild();
                }
            }
        }

        private EditorFrame _Frame;
        private CurveEditor _CurveEditor;
        private Skill.Framework.UI.Grid _PnlLeft;
        private TreeView _CurveTreeView;
        private ObjectField<UnityEngine.GameObject> _ObjectField;        
        private Skill.Editor.UI.GridSplitter _GridSplitter;
        private CurvePresetLibrary _PresetPanel;

        public CurveEditorWindow()
        {
            hideFlags = HideFlags.DontSave;

            title = "Curve Editor";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = new Vector2(Size.x, Size.y);

            CreateUI();
        }

        private void AddTestCurves()
        {
            AnimationCurve curve1 = new AnimationCurve();
            curve1.AddKey(new Keyframe() { time = 0, value = 0 });
            curve1.AddKey(new Keyframe() { time = 1, value = 1 });
            curve1.AddKey(new Keyframe() { time = 2, value = 0.5f });
            curve1.AddKey(new Keyframe() { time = 3, value = 1 });
            _CurveEditor.AddCurve(curve1, Color.green);


            AnimationCurve curve2 = new AnimationCurve();
            curve2.AddKey(new Keyframe() { time = 0.2f, value = 0 });
            curve2.AddKey(new Keyframe() { time = 1, value = 2 });
            curve2.AddKey(new Keyframe() { time = 2.4f, value = 1.5f });
            curve2.AddKey(new Keyframe() { time = 3.6f, value = -2 });
            _CurveEditor.AddCurve(curve2, Color.red);
        }

        private void CreateUI()
        {
            _Frame = new EditorFrame("Frame", this);

            _Frame.Grid.ColumnDefinitions.Add(224, Skill.Framework.UI.GridUnitType.Pixel);// _PnlLeft
            _Frame.Grid.ColumnDefinitions[0].MinWidth = 224;
            _Frame.Grid.ColumnDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Pixel); // _GridSplitter
            _Frame.Grid.ColumnDefinitions.Add(5, Skill.Framework.UI.GridUnitType.Star);  // _CurveEditor        

            _PnlLeft = new Skill.Framework.UI.Grid() { Row = 0, Column = 0 };
            _PnlLeft.RowDefinitions.Add(26, Skill.Framework.UI.GridUnitType.Pixel); // _ObjectField
            _PnlLeft.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // _CurveTreeView        
            _PnlLeft.RowDefinitions.Add(32, Skill.Framework.UI.GridUnitType.Pixel); // _PresetPanel

            _ObjectField = new ObjectField<GameObject>() { Row = 0, Column = 0, VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center };
            _CurveTreeView = new TreeView() { Row = 1, Column = 0 };
            _CurveTreeView.DisableFocusable();
            _PresetPanel = new CurvePresetLibrary() { Row = 2, Column = 0 };            

            _PnlLeft.Controls.Add(_ObjectField);
            _PnlLeft.Controls.Add(_CurveTreeView);
            _PnlLeft.Controls.Add(_PresetPanel);


            _GridSplitter = new Skill.Editor.UI.GridSplitter() { Row = 0, Column = 1, Orientation = Skill.Framework.UI.Orientation.Vertical };
            _CurveEditor = new CurveEditor() { Row = 0, Column = 2 };

            _Frame.Controls.Add(_PnlLeft);
            _Frame.Controls.Add(_GridSplitter);
            _Frame.Controls.Add(_CurveEditor);

            _ObjectField.ObjectChanged += _ObjectField_ObjectChanged;
            _PresetPanel.PresetSelected += _PresetPanel_PresetSelected;

        }

        void _ObjectField_ObjectChanged(object sender, System.EventArgs e)
        {
            Object = _ObjectField.Object;
        }
        void _PresetPanel_PresetSelected(object sender, EventArgs e)
        {
            if (_PresetPanel.Preset != null && _CurveTreeView.SelectedItem != null && _CurveTreeView.SelectedItem is CurveTrackTreeViewItem)
            {
                CurveTrackTreeViewItem item = (CurveTrackTreeViewItem)_CurveTreeView.SelectedItem;
                while (item.Track.Curve.length > 0)
                    item.Track.Curve.RemoveKey(item.Track.Curve.length - 1);
                for (int i = 0; i < _PresetPanel.Preset.Keys.Length; i++)
                    item.Track.Curve.AddKey(_PresetPanel.Preset.Keys[i]);
                item.Track.RebuildKeys();
            }
        }
        private void UpdateStyles()
        {
            if (_GridSplitter.Style == null)
            {
                _GridSplitter.Style = Skill.Editor.Resources.Styles.VerticalSplitter;
            }
        }
        void OnGUI()
        {
            UpdateStyles();
            _Frame.OnGUI();
        }


        private void Rebuild()
        {
            _CurveTreeView.Controls.Clear();
            _CurveEditor.ClearSelection();
            _CurveEditor.RemoveAllCurves();
            if (_Object != null)
            {
                Component[] components = _Object.GetComponents<Component>();
                foreach (var c in components)
                {
                    AddCurves(c);
                }
            }
        }

        private void AddCurves(Component component)
        {
            CurveEditor.EditCurveInfo[] curves = CurveEditor.GetCurves(component);
            if (curves != null && curves.Length > 0)
            {
                FolderView folder = new FolderView();
                folder.Foldout.Content.text = component.GetType().Name;
                folder.Foldout.IsOpen = true;

                foreach (var c in curves)
                {
                    CurveTrack track = _CurveEditor.AddCurve(c.GetCurve(), c.Attribute.Color);
                    CurveTrackTreeViewItem item = new CurveTrackTreeViewItem(track, c);
                    folder.Controls.Add(item);
                }

                _CurveTreeView.Controls.Add(folder);
            }

        }


        class CurveTrackTreeViewItem : Grid
        {
            private Skill.Editor.UI.ToggleButton _TbVisible;
            private Skill.Framework.UI.Label _LblName;
            private Skill.Editor.UI.ColorField _CFColor;

            public CurveTrack Track { get; private set; }
            public CurveEditor.EditCurveInfo Info { get; private set; }

            public CurveTrackTreeViewItem(CurveTrack track, CurveEditor.EditCurveInfo info)
            {
                this.Track = track;
                this.Info = info;

                this.ColumnDefinitions.Add(20, GridUnitType.Pixel);
                this.ColumnDefinitions.Add(1, GridUnitType.Star);
                this.ColumnDefinitions.Add(36, GridUnitType.Pixel);

                _TbVisible = new Skill.Editor.UI.ToggleButton() { Column = 0, IsChecked = track.Visibility == Skill.Framework.UI.Visibility.Visible };
                _LblName = new Label() { Column = 1, Text = info.Attribute.Name };
                _CFColor = new Skill.Editor.UI.ColorField() { Column = 2, Color = track.Color };

                this.Controls.Add(_TbVisible);
                this.Controls.Add(_LblName);
                this.Controls.Add(_CFColor);

                _TbVisible.Changed += _TbVisible_Changed;
                _CFColor.ColorChanged += _CFColor_ColorChanged;
            }

            void _CFColor_ColorChanged(object sender, EventArgs e)
            {
                Track.Color = _CFColor.Color;
            }

            void _TbVisible_Changed(object sender, EventArgs e)
            {
                Track.Visibility = _TbVisible.IsChecked ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;
            }
        }



    }
}