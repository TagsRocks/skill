using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using UnityEditor;
using Skill.Framework;
using System.Collections.Generic;
using Skill.Editor.UI.Extended;
using System;


namespace Skill.Editor.Tools
{
    public class DictionaryEditorWindow : UnityEditor.EditorWindow
    {
        #region AudioField
        class AudioField : Skill.Framework.UI.Grid, Skill.Editor.UI.Extended.IProperties
        {
            private static GUIStyle _ButtonStyle;

            private Skill.Framework.UI.Box _Bg;
            private Skill.Framework.UI.Label _LblClipName;
            private Skill.Editor.UI.Button _BtnAddNext;
            private Skill.Editor.UI.Button _BtnRemove;
            private bool _RefreshStyles;

            private DictionaryEditorWindow _Editor;
            public AudioClipSubtitle Subtitle { get; private set; }

            public AudioField(DictionaryEditorWindow editor, AudioClipSubtitle subtitle)
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

        #region EditorWindow
        private static DictionaryEditorWindow _Instance;
        public static DictionaryEditorWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = ScriptableObject.CreateInstance<DictionaryEditorWindow>();
                }
                return _Instance;
            }
        }

        private static Vector2 Size = new Vector2(1200, 800);

        public void OnGUI()
        {
            if (_Frame != null)
            {
                if (_RefreshStyles)
                    RefreshStyles();
                _Frame.OnGUI();
            }
        }
        public DictionaryEditorWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.title = "Dictionary";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = new Vector2(Size.x * 0.5f, Size.y * 0.5f);
            CreateUI();
            HookEvents();
            EnableUI();
        }
        void OnDestroy()
        {
            _AudioClipEditor.Destroy();
            _Instance = null;
        }
        void OnDisable()
        {
            _AudioClipEditor.Stop();
            if (_IsChanged)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Confirm Save", "Save changes to Dictionary?", "Save", "Don't Save"))
                    Save();
            }
            _Instance = null;
        }
        void OnEnable()
        {
            _RefreshStyles = true;
            _ObjectField.Object = _Dictionary;
        }

        void Update()
        {
            if (_Frame != null)
            {
                _Frame.Update();
                _AudioClipEditor.UpdatePlayback();
            }
        }

        void OnLostFocus()
        {
            SetDirty2();
        }

        #endregion

        #region UI

        private Skill.Editor.UI.EditorFrame _Frame;
        private Skill.Framework.UI.Grid _FieldPanel;
        private Skill.Framework.UI.Grid _AudioPanel;
        private Skill.Framework.UI.Grid _TextPanel;
        private Skill.Editor.UI.GridSplitter _PanelSplitter;

        private Skill.Editor.UI.ObjectField<Dictionary> _ObjectField;
        private Skill.Framework.UI.Extended.ListBox _ListBox;

        private Skill.Framework.UI.Grid _EditPanel;

        private Skill.Framework.UI.Label _LblName;
        private Skill.Framework.UI.Label _LblValue;
        private Skill.Framework.UI.Label _LblComment;
        private PasteTextField _TxtName;
        private PasteTextField _TxtValue;
        private PasteTextField _TxtComment;
        private Skill.Editor.UI.Button _BtnAdd;
        private Skill.Editor.UI.Button _BtnRemove;
        private Skill.Editor.UI.Button _BtnSave;

        private Skill.Framework.UI.Box _NameCaption;
        private Skill.Framework.UI.Box _ValueCaption;


        private Skill.Editor.UI.GridSplitter _Splitter;
        private Skill.Editor.UI.Extended.PropertyGrid _PropertyGrid;
        private Skill.Framework.UI.Extended.ListBox _AudioList;
        private Skill.Framework.UI.Button _BtnNew;
        private List<AudioField> _Fields;
        private AudioClipSubtitleEditor _AudioClipEditor;

        public Skill.Editor.UI.Extended.PropertyGrid PropertyGrid { get { return _PropertyGrid; } }

        private void CreateUI()
        {
            _RefreshStyles = true;
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
            _Frame.Grid.Padding = new Thickness(2);

            _Frame.Grid.RowDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(3, Skill.Framework.UI.GridUnitType.Star);
            _Frame.Grid.RowDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Star);

            _PanelSplitter = new UI.GridSplitter() { Row = 2, Orientation = Orientation.Horizontal };

            CreateAudioPanel();
            CreateFieldPanel();
            CreateTextPanel();

            _Frame.Grid.Controls.Add(_FieldPanel);
            _Frame.Grid.Controls.Add(_AudioPanel);
            _Frame.Grid.Controls.Add(_PanelSplitter);
            _Frame.Grid.Controls.Add(_TextPanel);

        }

        private void CreateAudioPanel()
        {

            _AudioPanel = new Grid() { Row = 1, Padding = new Thickness(2) };

            _AudioPanel.RowDefinitions.Add(262, Skill.Framework.UI.GridUnitType.Pixel); // audio subtitle editor
            _AudioPanel.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); //        

            _AudioClipEditor = new AudioClipSubtitleEditor(this) { Row = 0, Column = 0, ColumnSpan = 2 };
            _AudioPanel.Controls.Add(_AudioClipEditor);

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

            _AudioPanel.Controls.Add(pnl);


            _BtnNew.Click += _BtnNew_Click;
            _AudioList.SelectionChanged += _AudioList_SelectionChanged;
        }

        private void CreateFieldPanel()
        {
            _FieldPanel = new Grid() { Row = 0, Padding = new Thickness(2) };            
            Box box = new Box() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10 };
            _FieldPanel.Controls.Add(box);

            _ObjectField = new Skill.Editor.UI.ObjectField<Dictionary>() { Margin = new Thickness(2), VerticalAlignment = VerticalAlignment.Top };
            _ObjectField.Label.text = "Dictionary";
            _FieldPanel.Controls.Add(_ObjectField);            
        }

        private void CreateTextPanel()
        {
            _TextPanel = new Grid() { Row = 3, Padding = new Thickness(2) };
            _TextPanel.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _TextPanel.RowDefinitions.Add(80, Skill.Framework.UI.GridUnitType.Pixel);

            Grid viewPanel = new Grid() { Row = 0, Column = 0, Padding = new Thickness(2) };
            viewPanel.ColumnDefinitions.Add(1, GridUnitType.Star);
            viewPanel.ColumnDefinitions.Add(2, GridUnitType.Star);
            viewPanel.ColumnDefinitions.Add(17, GridUnitType.Pixel);
            viewPanel.ColumnDefinitions.Add(50, GridUnitType.Pixel);

            viewPanel.RowDefinitions.Add(24, GridUnitType.Pixel);
            viewPanel.RowDefinitions.Add(50, GridUnitType.Pixel);
            viewPanel.RowDefinitions.Add(50, GridUnitType.Pixel);            
            viewPanel.RowDefinitions.Add(1, GridUnitType.Star);

            Box box = new Box() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10 };
            viewPanel.Controls.Add(box);


            _NameCaption = new Box() { Row = 0, Column = 0 }; _NameCaption.Content.text = "Key";
            _ValueCaption = new Box() { Row = 0, Column = 1 }; _ValueCaption.Content.text = "Value";

            _ListBox = new Skill.Framework.UI.Extended.ListBox() { Row = 1, RowSpan = 3, Column = 0, ColumnSpan = 3, Margin = new Thickness(2) };
            _ListBox.BackgroundVisible = true;
            _ListBox.DisableFocusable();
            _ListBox.AlwayShowVertical = true;
            _ListBox.AutoScroll = true;

            _BtnAdd = new Skill.Editor.UI.Button() { Row = 1, Column = 3, Margin = new Thickness(2) }; _BtnAdd.Content.tooltip = "Add";
            _BtnRemove = new Skill.Editor.UI.Button() { Row = 2, Column = 3, Margin = new Thickness(2) }; _BtnAdd.Content.tooltip = "remove selected";
            _BtnSave = new Skill.Editor.UI.Button() { Row = 3, Column = 3, Margin = new Thickness(2,2,2,4), VerticalAlignment = VerticalAlignment.Bottom, Height = 46 }; _BtnSave.Content.tooltip = "Save Changes";            

            viewPanel.Controls.Add(_NameCaption);
            viewPanel.Controls.Add(_ValueCaption);
            viewPanel.Controls.Add(_BtnAdd);
            viewPanel.Controls.Add(_BtnRemove);
            viewPanel.Controls.Add(_BtnSave);
            viewPanel.Controls.Add(_ListBox);

            _TextPanel.Controls.Add(viewPanel);
            _EditPanel = new Grid() { Row = 1, Column = 0, Padding = new Thickness(2) };
            _EditPanel.ColumnDefinitions.Add(60, GridUnitType.Pixel);
            _EditPanel.ColumnDefinitions.Add(2, GridUnitType.Star);
            _EditPanel.ColumnDefinitions.Add(60, GridUnitType.Pixel);
            _EditPanel.ColumnDefinitions.Add(1, GridUnitType.Star);

            _EditPanel.RowDefinitions.Add(1, GridUnitType.Star);
            _EditPanel.RowDefinitions.Add(2, GridUnitType.Star);

            Box box1 = new Box() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10 };
            _EditPanel.Controls.Add(box1);


            _LblName = new Label() { Row = 0, Column = 0, Margin = new Thickness(2), Text = "Key" };
            _LblValue = new Label() { Row = 1, Column = 0, Margin = new Thickness(2), Text = "Value" };
            _LblComment = new Label() { Row = 0, Column = 2, ColumnSpan = 2, Margin = new Thickness(2), Text = "Comment" };

            _TxtName = new PasteTextField() { Row = 0, Column = 1, Margin = new Thickness(2, 2, 20, 2) };
            _TxtValue = new PasteTextField() { Row = 1, Column = 1, Margin = new Thickness(2, 2, 20, 2) };
            _TxtComment = new PasteTextField() { Row = 1, Column = 2, ColumnSpan = 2, Margin = new Thickness(2) };

            _EditPanel.Controls.Add(_LblName);
            _EditPanel.Controls.Add(_LblValue);
            _EditPanel.Controls.Add(_LblComment);

            _EditPanel.Controls.Add(_TxtName);
            _EditPanel.Controls.Add(_TxtValue);
            _EditPanel.Controls.Add(_TxtComment);

            _TextPanel.Controls.Add(_EditPanel);
        }

        private void RefreshStyles()
        {
            _PanelSplitter.Style = Skill.Editor.Resources.Styles.HorizontalSplitter;
            _ListBox.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
            _BtnAdd.Content.image = Skill.Editor.Resources.Textures.Add;
            _BtnRemove.Content.image = Skill.Editor.Resources.Textures.Remove;
            _BtnSave.Content.image = Skill.Editor.Resources.Textures.Save;
            if (_LblComment.Style == null)
                _LblComment.Style = new GUIStyle(UnityEditor.EditorGUIUtility.GetBuiltinSkin(UnityEditor.EditorSkin.Inspector).label);
            _LblComment.Style.alignment = TextAnchor.MiddleCenter;

            _AudioClipEditor.RefreshStyles();
            _AudioList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
            _Splitter.Style = Skill.Editor.Resources.Styles.VerticalSplitter;

            _RefreshStyles = false;
        }
        private void EnableUI()
        {
            if (_Dictionary == null)
            {
                _TextPanel.IsEnabled = false;
                _AudioPanel.IsEnabled = false;
                _BtnSave.IsEnabled = false;
                _TxtName.TextField.Text = string.Empty;
                _TxtValue.TextField.Text = string.Empty;
                _TxtComment.TextField.Text = string.Empty;
            }
            else
            {
                _TextPanel.IsEnabled = true;
                _AudioPanel.IsEnabled = true;
                _BtnSave.IsEnabled = _IsChanged;
            }
        }
        #endregion


        public Dictionary Dictionary { get { return _ObjectField.Object; } set { _ObjectField.Object = value; } }

        [SerializeField]
        private Dictionary _Dictionary;
        private bool _RefreshStyles;
        private bool _IsChanged;
        private bool _IgnoreChanges;
        private TextKeyView _SelectedView;

        private void SetChanged(bool changed)
        {
            if (_IsChanged != changed)
            {
                _IsChanged = changed;
                if (_IsChanged)
                    base.title = "Dictionary*";
                else
                    base.title = "Dictionary";

                if (_BtnSave != null)
                    _BtnSave.IsEnabled = _IsChanged;
            }
        }

        private void HookEvents()
        {
            _ObjectField.ObjectChanged += _ObjectField_ObjectChanged;
            _BtnSave.Click += _BtnSave_Click;
            _BtnAdd.Click += _BtnAdd_Click;
            _BtnRemove.Click += _BtnRemove_Click;
            _ListBox.SelectionChanged += _ListBox_SelectionChanged;

            _TxtName.TextField.TextChanged += _TextChanged;
            _TxtValue.TextField.TextChanged += _TextChanged;
            _TxtComment.TextField.TextChanged += _TextChanged;

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

        void _BtnNew_Click(object sender, EventArgs e)
        {
            AddNext(null);
        }

        void _TextChanged(object sender, System.EventArgs e)
        {
            if (_IgnoreChanges) return;
            if (_SelectedView != null)
            {
                _SelectedView.Key.Key = _TxtName.TextField.Text;
                _SelectedView.Key.Value = _TxtValue.TextField.Text;
                _SelectedView.Key.Comment = _TxtComment.TextField.Text;
                _SelectedView.UpdateTexts();
            }
        }

        void _ListBox_SelectionChanged(object sender, System.EventArgs e)
        {
            object item = _ListBox.SelectedItem;
            _BtnRemove.IsEnabled = item != null;
            _EditPanel.IsEnabled = item != null;
            if (item != null)
            {
                _SelectedView = (TextKeyView)item;
                _IgnoreChanges = true;
                _TxtName.TextField.Text = _SelectedView.Key.Key;
                _TxtValue.TextField.Text = _SelectedView.Key.Value;
                _TxtComment.TextField.Text = _SelectedView.Key.Comment;
                _IgnoreChanges = false;
            }
            else
                _SelectedView = null;

            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(_Frame.Grid);
        }

        void _BtnRemove_Click(object sender, System.EventArgs e)
        {
            int index = _ListBox.SelectedIndex;
            if (index >= 0)
            {
                _ListBox.Controls.Remove(_ListBox.SelectedItem);
                if (index >= _ListBox.Controls.Count)
                    index--;
                _ListBox.SelectedIndex = index;
                SetChanged(true);
            }
        }

        void _BtnAdd_Click(object sender, System.EventArgs e)
        {
            TextKeyView view = new TextKeyView(new TextKey() { Key = "New" });
            _ListBox.Controls.Add(view);
            _ListBox.SelectedItem = view;
            SetChanged(true);
        }

        void _BtnSave_Click(object sender, System.EventArgs e)
        {
            Save();
        }

        void _ObjectField_ObjectChanged(object sender, System.EventArgs e)
        {
            _Dictionary = _ObjectField.Object;
            Rebuild();
            EnableUI();
        }

        private void Rebuild()
        {
            _ListBox.Controls.Clear();
            if (_Dictionary != null)
            {
                if (_Dictionary.Keys != null)
                {
                    foreach (var key in _Dictionary.Keys)
                    {
                        TextKeyView view = new TextKeyView(key);
                        _ListBox.Controls.Add(view);
                    }
                }

                if (_Dictionary.Subtitles == null)
                    _Dictionary.Subtitles = new AudioClipSubtitle[0];

                // search for new AudioClipSubtitles in AudioSubtitle that we didn't create AudioField for them 
                foreach (var t in _Dictionary.Subtitles)
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
                _AudioList.Controls.Clear();
                _Fields.Clear();
                _BtnNew.Visibility = Skill.Framework.UI.Visibility.Hidden;
            }
            SetChanged(false);
            _ListBox.SelectedIndex = 0;
        }

        void UpdateBtnNewVisibility()
        {
            _BtnNew.Visibility = (_Fields.Count > 0) ? Skill.Framework.UI.Visibility.Hidden : Skill.Framework.UI.Visibility.Visible;
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
            foreach (var k in _Dictionary.Subtitles)
            {
                if (k == subtitle)
                    return true;
            }
            return false;
        }

        private void Save()
        {
            if (_Dictionary != null && _IsChanged)
            {
                _Dictionary.Keys = new TextKey[_ListBox.Controls.Count];
                for (int i = 0; i < _ListBox.Controls.Count; i++)
                    _Dictionary.Keys[i] = ((TextKeyView)_ListBox.Controls[i]).Key;
                UnityEditor.EditorUtility.SetDirty(_Dictionary);
                SetChanged(false);
            }
        }

        public void SetDirty2()
        {
            if (_Dictionary != null)
                UnityEditor.EditorUtility.SetDirty(_Dictionary);
        }

        private void RebuildSubtitles()
        {
            if (_Dictionary != null)
            {
                _Dictionary.Subtitles = new AudioClipSubtitle[_Fields.Count];
                for (int i = 0; i < _Fields.Count; i++)
                    _Dictionary.Subtitles[i] = _Fields[i].Subtitle;
                SetDirty2();
            }
        }

        public void ShowTextPicker(ITextPickTarget pickTarget)
        {
            if (_Dictionary != null && pickTarget != null)
            {
                if (_Dictionary != null)
                    PickTextWindow.Instance.Show(_Dictionary, pickTarget);
                else
                    UnityEditor.EditorUtility.DisplayDialog("Dictionary not found", "You must assign a valid Dictionary for AudioSubtitle", "Ok");
            }
        }

        public string GetTitle(string key)
        {
            if (_Dictionary != null && _Dictionary.Keys != null)
            {
                foreach (var k in _Dictionary.Keys)
                {
                    if (k.Key == key) return k.Value;
                }
            }
            return "undefined";
        }
    }


    #region TextKeyView
    class TextKeyView : Grid
    {
        public TextKey Key { get; private set; }

        private Label _LblName;
        private Label _LblValue;

        public TextKeyView(TextKey key)
        {
            this.Key = key;
            ColumnDefinitions.Add(1, GridUnitType.Star);
            ColumnDefinitions.Add(2, GridUnitType.Star);

            _LblName = new Label() { Column = 0 };
            _LblValue = new Label() { Column = 1 };

            this.Controls.Add(_LblName);
            this.Controls.Add(_LblValue);

            UpdateTexts();
        }

        public void UpdateTexts()
        {
            _LblName.Text = Key.Key;
            _LblValue.Text = Key.Value;
        }
    }
    #endregion

}
