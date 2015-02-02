using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Framework.UI;
using Skill.Framework.Audio;

namespace Skill.Editor.Audio
{
    public class AudioControllerEditorWindow : EditorWindow
    {
        #region AudioControllerEditorWindow

        private static AudioControllerEditorWindow _Instance;
        public static AudioControllerEditorWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = ScriptableObject.CreateInstance<AudioControllerEditorWindow>();
                return _Instance;
            }
        }

        private static Vector2 Size = new Vector2(800, 600);
        private static Vector2 MinSize = new Vector2(500, 300);

        public AudioControllerEditorWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.title = "Audio";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = MinSize;
            CreateUI();
        }

        void OnFocus()
        {
            if (_Frame != null)
            {
            }
        }

        void OnLostFocus()
        {
            if (_Frame != null && _Controller != null)
            {
                Save();
            }
        }

        void OnDestroy()
        {
            if (_GraphEditor != null)
            {
                _GraphEditor.DeselectInspector();
                _GraphEditor.DestroyPreviewObject();
            }
            if (_TriggerEditor != null)
                _TriggerEditor.DeselectInspector();
        }

        void Update()
        {
            if (_GraphEditor != null)
            {
                _GraphEditor.UpdatePreview();
            }
        }

        void OnEnable()
        {
            if (_Controller != null)
                Rebuild();
        }

        #endregion

        #region UI
        private bool _RefreshStyles = true;
        private Skill.Editor.UI.EditorFrame _Frame;

        private Skill.Editor.UI.GridSplitter _VSplitter;
        private AudioTriggerEditor _TriggerEditor;
        private AudioStateGraphEditor _GraphEditor;

        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);

            _Frame.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star), MinWidth = 140 });
            _Frame.Grid.ColumnDefinitions.Add(2, GridUnitType.Pixel);
            _Frame.Grid.ColumnDefinitions.Add(6, GridUnitType.Star);

            _VSplitter = new Skill.Editor.UI.GridSplitter() { Row = 0, Column = 1, Orientation = Orientation.Vertical };
            _Frame.Controls.Add(_VSplitter);

            _TriggerEditor = new AudioTriggerEditor(this) { Row = 0, Column = 0 };
            _Frame.Controls.Add(_TriggerEditor);

            _GraphEditor = new AudioStateGraphEditor(this) { Row = 0, Column = 2 };
            _Frame.Controls.Add(_GraphEditor);
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
                _TriggerEditor.RefreshStyles();
                _GraphEditor.RefreshStyles();
            }
        }

        #endregion

        [SerializeField]
        private AudioController _Controller;
        public AudioController Controller
        {
            get { return _Controller; }
            set
            {
                if (_Controller != value)
                {
                    _Controller = value;
                    Rebuild();
                }
            }
        }
        private void Rebuild()
        {
            _RefreshStyles = true;
            Clear();
            if (_Controller != null)
            {
                if (_Controller.States != null)
                {
                    foreach (var s in _Controller.States)
                        s.SortBreakPoints();
                }

                _TriggerEditor.Rebuild();
                _GraphEditor.Rebuild();
            }
        }

        private void Clear()
        {
            _TriggerEditor.Clear();
            _GraphEditor.Clear();
        }

        private void Save()
        {
            if (_Controller != null)
            {
                _GraphEditor.Save();
                EditorUtility.SetDirty(_Controller);
            }
        }
    }

}