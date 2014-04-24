using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Skill.Editor.UI.Extended;
using Skill.Framework.Audio;

namespace Skill.Editor.Tools
{
    
    public class AudioSubtitleEditorWindow : UnityEditor.EditorWindow
    {
        #region AudioField
        class AudioField : Skill.Framework.UI.Grid, IProperties
        {
            private static GUIStyle _ButtonStyle;

            private Skill.Framework.UI.Box _Bg;
            private Skill.Framework.UI.Label _LblClipName;
            private Skill.Editor.UI.Button _BtnAddNext;
            private Skill.Editor.UI.Button _BtnRemove;
            private bool _RefreshStyles;

            private AudioSubtitleEditorWindow _Editor;
            public AudioClipSubtitle Subtitle { get; private set; }

            public AudioField(AudioSubtitleEditorWindow editor, AudioClipSubtitle subtitle)
            {
                this.Height = 18;
                this.Margin = new Skill.Framework.UI.Thickness(1);
                this._RefreshStyles = true;
                this.Subtitle = subtitle;
                this._Editor = editor;

                this.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                this.ColumnDefinitions.Add(18, Skill.Framework.UI.GridUnitType.Pixel);
                this.ColumnDefinitions.Add(18, Skill.Framework.UI.GridUnitType.Pixel);

                this._Bg = new Skill.Framework.UI.Box() { Column = 0, ColumnSpan = 3 };
                this.Controls.Add(_Bg);

                this._LblClipName = new Skill.Framework.UI.Label() { Column = 0 };
                UpdateTitle();
                this.Controls.Add(_LblClipName);

                if (_ButtonStyle == null) _ButtonStyle = new GUIStyle();

                this._BtnAddNext = new Skill.Editor.UI.Button() { Column = 1, Margin = new Skill.Framework.UI.Thickness(1), Style = _ButtonStyle };
                this._BtnAddNext.Content.tooltip = "Add next";
                this.Controls.Add(_BtnAddNext);

                this._BtnRemove = new Skill.Editor.UI.Button() { Column = 2, Margin = new Skill.Framework.UI.Thickness(1), Style = _ButtonStyle };
                this._BtnRemove.Content.tooltip = "Remove";
                this.Controls.Add(_BtnRemove);

                this._BtnAddNext.Click += _BtnAddNext_Click;
                this._BtnRemove.Click += _BtnRemove_Click;
            }

            protected override void Render()
            {
                if (_RefreshStyles)
                {
                    this._BtnAddNext.Content.image = Skill.Editor.Resources.Textures.PlusNext;
                    this._BtnRemove.Content.image = Skill.Editor.Resources.Textures.Minus;
                    _RefreshStyles = false;
                }
                base.Render();
            }

            void _BtnRemove_Click(object sender, EventArgs e)
            {
                _Editor.Remove(this);
            }
            void _BtnAddNext_Click(object sender, EventArgs e)
            {
                _Editor.AddNext(this);
            }

            private void UpdateTitle()
            {
                if (Subtitle.Clip != null)
                {
                    _LblClipName.Content.text = string.Format("{0}(path : {1})", Subtitle.Clip.name, AssetDatabase.GetAssetPath(Subtitle.Clip));
                }
                else
                    _LblClipName.Content.text = "None";
            }

            public bool IsSelectedProperties { get; set; }

            private AudioFieldProperties _Properties;
            public PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new AudioFieldProperties(this);
                    return _Properties;
                }
            }

            public string Title { get { return "AudioClip Subtitle"; } }

            class AudioFieldProperties : PropertiesPanel
            {
                private Skill.Editor.UI.ObjectField<AudioClip> _ObjectField;
                private AudioField _AudioField;

                public AudioFieldProperties(AudioField af)
                    : base(af)
                {
                    _AudioField = af;
                    this._ObjectField = new Skill.Editor.UI.ObjectField<AudioClip>() { Column = 0, Margin = new Skill.Framework.UI.Thickness(2) };
                    this._ObjectField.Label.text = "Clip";
                    this.Controls.Add(_ObjectField);

                    this._ObjectField.ObjectChanged += _ObjectField_ObjectChanged;
                }
                void _ObjectField_ObjectChanged(object sender, EventArgs e)
                {
                    if (IgnoreChanges) return;
                    _AudioField.Subtitle.Clip = _ObjectField.Object;
                    _AudioField.UpdateTitle();
                    _AudioField._Editor.UpdateSelected(_AudioField);
                    SetDirty();
                }
                protected override void RefreshData()
                {
                    this._ObjectField.Object = _AudioField.Subtitle.Clip;
                    _AudioField.UpdateTitle();
                }

                protected override void SetDirty()
                {
                    _AudioField._Editor.SetDirty2();
                }
            }
        }
        #endregion

        #region Static Variables
        private static AudioSubtitleEditorWindow _Instance;
        public static AudioSubtitleEditorWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = ScriptableObject.CreateInstance<AudioSubtitleEditorWindow>();
                }
                return _Instance;
            }
        }
        private static Vector2 Size = new Vector2(800, 600);
        #endregion

        private Skill.Editor.UI.EditorFrame _Frame;
        private Skill.Editor.UI.GridSplitter _Splitter;
        private Skill.Editor.UI.Extended.PropertyGrid _PropertyGrid;
        private Skill.Framework.UI.Extended.ListBox _AudioList;
        private Skill.Framework.UI.Button _BtnNew;
        private List<AudioField> _Fields;
        private AudioClipSubtitleEditor _AudioClipEditor;
        private bool _RefreshStyles;

        private AudioSubtitle _Subtitle;
        public AudioSubtitle Subtitle
        {
            get { return _Subtitle; }
            set
            {
                if (_Subtitle != value)
                {
                    _Subtitle = value;
                    Refresh();
                }
            }
        }
        public PropertyGrid PropertyGrid { get { return _PropertyGrid; } }
        public void OnGUI()
        {
            if (_Frame != null)
            {
                if (_RefreshStyles)
                {
                    RefreshStyles();
                }
                _Frame.OnGUI();
            }
        }

        void Update()
        {
            if (_Frame != null)
            {
                _Frame.Update();
                _AudioClipEditor.UpdatePlayback();
            }
        }

        public AudioSubtitleEditorWindow()
        {
            hideFlags = HideFlags.DontSave;

            title = "Audio Subtitle";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = new Vector2(Size.x * 0.5f, Size.y * 0.5f);

            CreateUI();
        }
        private void CreateUI()
        {
            _RefreshStyles = true;
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);

            _Frame.Grid.RowDefinitions.Add(262, Skill.Framework.UI.GridUnitType.Pixel); // audio subtitle editor
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); //        

            _AudioClipEditor = new AudioClipSubtitleEditor(this) { Row = 0, Column = 0, ColumnSpan = 2 };
            _Frame.Grid.Controls.Add(_AudioClipEditor);

            Skill.Framework.UI.Grid pnl = new Skill.Framework.UI.Grid() { Row = 1, Column = 0 };
            pnl.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); //
            pnl.ColumnDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Pixel); //
            pnl.ColumnDefinitions.Add(260, Skill.Framework.UI.GridUnitType.Pixel); // _PropertyGrid

            _Fields = new List<AudioField>();
            _AudioList = new Skill.Framework.UI.Extended.ListBox() { Row = 0, Column = 0, AlwayShowVertical = true };
            _AudioList.BackgroundVisible = true;
            _AudioList.DisableFocusable();
            pnl.Controls.Add(_AudioList);

            _Splitter = new Skill.Editor.UI.GridSplitter() { Row = 0, Column = 1, Orientation = Skill.Framework.UI.Orientation.Vertical };
            pnl.Controls.Add(_Splitter);

            _PropertyGrid = new PropertyGrid() { Row = 0, Column = 2 };
            pnl.Controls.Add(_PropertyGrid);

            _BtnNew = new Skill.Framework.UI.Button()
            {
                Row = 0,
                Column = 0,
                Width = 100,
                HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Center,
                VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Top,
                Height = 26,
                Margin = new Skill.Framework.UI.Thickness(4, 10, 4, 0)
            };
            _BtnNew.Content.text = "New";

            pnl.Controls.Add(_BtnNew);

            _Frame.Grid.Controls.Add(pnl);


            _BtnNew.Click += _BtnNew_Click;
            _AudioList.SelectionChanged += _AudioList_SelectionChanged;
        }

        void _AudioList_SelectionChanged(object sender, EventArgs e)
        {
            if (_AudioList.SelectedItem != null)
            {
                AudioField af = (AudioField)_AudioList.SelectedItem;
                _AudioClipEditor.Subtitle = af.Subtitle;
                _PropertyGrid.SelectedObject = af;
            }
            else
            {
                _AudioClipEditor.Subtitle = null;
            }
        }



        void OnDestroy()
        {
            _AudioClipEditor.Destroy();
        }
        void OnDisable()
        {
            SetDirty2();
            _Instance = null;
            _AudioClipEditor.Stop();
        }
        void OnEnable()
        {
            _RefreshStyles = true;
            //if (_Frame != null)
            //    Refresh();
        }
        void OnLostFocus()
        {
            SetDirty2();
        }

        private void Refresh()
        {
            if (Selection.activeObject != null)
            {
                GameObject selectedObject = null;
                if (Selection.activeObject is GameObject)
                    selectedObject = (GameObject)Selection.activeObject;
                else if (Selection.activeObject is Component)
                    selectedObject = ((Component)Selection.activeObject).gameObject;

                if (selectedObject != null)
                {
                    AudioSubtitle newSubtitle = selectedObject.GetComponent<AudioSubtitle>();
                    if (newSubtitle != null)
                        _Subtitle = newSubtitle;
                }
            }
            if (_Subtitle != null)
            {
                _Frame.Grid.IsEnabled = true;

                if (_Subtitle.Subtitles == null)
                    _Subtitle.Subtitles = new AudioClipSubtitle[0];

                // search for new AudioClipSubtitles in AudioSubtitle that we didn't create AudioField for them 
                foreach (var t in _Subtitle.Subtitles)
                {
                    if (t != null)
                    {
                        if (!IsAudioFieldExistWithSubtitle(t))
                            CreateAudioField(t);
                    }
                }

                // search for removed AudioClipSubtitles in AudioSubtitle that we did create AudioField for them 
                int index = 0;
                while (index < _Fields.Count)
                {
                    var e = _Fields[index];
                    if (!IsAudioClipSubtitleExistInSubtitles(e.Subtitle))
                    {
                        _Fields.Remove(e);
                        _AudioList.Controls.Remove(e);
                        continue;
                    }
                    index++;
                }
                UpdateBtnNewVisibility();
            }
            else
            {
                _Frame.Grid.IsEnabled = false;
                _AudioList.Controls.Clear();
                _Fields.Clear();
                _BtnNew.Visibility = Skill.Framework.UI.Visibility.Hidden;
            }
        }

        private AudioField CreateAudioField(AudioClipSubtitle title)
        {
            AudioField af = new AudioField(this, title);
            _Fields.Add(af);
            _AudioList.Controls.Add(af);
            return af;
        }
        private bool IsAudioFieldExistWithSubtitle(AudioClipSubtitle subtitle)
        {
            foreach (AudioField af in _Fields)
                if (af.Subtitle == subtitle) return true;
            return false;
        }
        private bool IsAudioClipSubtitleExistInSubtitles(AudioClipSubtitle subtitle)
        {
            foreach (var k in _Subtitle.Subtitles)
            {
                if (k == subtitle)
                    return true;
            }
            return false;
        }

        private void RefreshStyles()
        {
            _AudioClipEditor.RefreshStyles();
            _AudioList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
            _Splitter.Style = Skill.Editor.Resources.Styles.VerticalSplitter;
            _RefreshStyles = false;
        }

        public void SetDirty2()
        {
            if (_Subtitle != null)
                UnityEditor.EditorUtility.SetDirty(_Subtitle);
        }

        private void Remove(AudioField audioField)
        {
            int index = _AudioList.Controls.IndexOf(audioField);
            if (index >= 0)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Confirm Delete", string.Format("Are you sure that you want to delete AudioClip '{0}'",
                    (audioField.Subtitle.Clip != null) ? audioField.Subtitle.Clip.name : "Null"), "Delete", "Cancel"))
                {
                    _Fields.Remove(audioField);
                    _AudioList.Controls.Remove(audioField);
                    if (index >= _Fields.Count) index = _Fields.Count - 1;
                    _AudioList.SelectedIndex = index;

                    SetDirty2();
                    UpdateBtnNewVisibility();
                    RebuildSubtitles();
                }
            }
        }
        private void AddNext(AudioField audioField)
        {
            int index = _AudioList.Controls.IndexOf(audioField) + 1;
            if (index < 0) index = 0;

            AudioClipSubtitle newSub = new AudioClipSubtitle();
            AudioField af = new AudioField(this, newSub);
            _Fields.Insert(index, af);
            _AudioList.Controls.Insert(index, af);
            _AudioList.SelectedItem = af;
            SetDirty2();
            UpdateBtnNewVisibility();
            RebuildSubtitles();
        }

        private void UpdateSelected(AudioField audioField)
        {
            if (_AudioList.SelectedItem == audioField)
                _AudioClipEditor.Subtitle = audioField.Subtitle;
        }

        void _BtnNew_Click(object sender, EventArgs e)
        {
            AddNext(null);
        }
        void UpdateBtnNewVisibility()
        {
            _BtnNew.Visibility = (_Fields.Count > 0) ? Skill.Framework.UI.Visibility.Hidden : Skill.Framework.UI.Visibility.Visible;
        }

        private void RebuildSubtitles()
        {
            if (_Subtitle != null)
            {
                _Subtitle.Subtitles = new AudioClipSubtitle[_Fields.Count];
                for (int i = 0; i < _Fields.Count; i++)
                    _Subtitle.Subtitles[i] = _Fields[i].Subtitle;
                SetDirty2();
            }
        }

        public void ShowTextPicker(ITextPickTarget pickTarget)
        {
            if (_Subtitle != null && pickTarget != null)
            {
                if (_Subtitle.Dictionary != null)
                    PickTextWindow.Instance.Show(_Subtitle.Dictionary, pickTarget);
                else
                    UnityEditor.EditorUtility.DisplayDialog("Dictionary not found", "You must assign a valid Dictionary for AudioSubtitle", "Ok");
            }
        }

        public string GetTitle(string key)
        {
            if (_Subtitle != null && _Subtitle.Dictionary != null)
            {
                if (_Subtitle.Dictionary.Data != null)
                {
                    foreach (var d in _Subtitle.Dictionary.Data)
                    {
                        if (d != null && d.Keys != null)
                        {
                            foreach (var k in d.Keys)
                            {
                                if (k.Key == key) return k.Value;
                            }
                        }
                    }
                }
            }
            return "undefined";
        }
    }    
}