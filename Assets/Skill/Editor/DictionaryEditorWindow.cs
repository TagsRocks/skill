using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using UnityEditor;
using Skill.Framework;
using System.Collections.Generic;
using Skill.Editor.UI;
using System;


namespace Skill.Editor
{
    public class DictionaryEditorWindow : UnityEditor.EditorWindow
    {
        #region AudioField
        class AudioField : Skill.Framework.UI.Grid, Skill.Editor.UI.IProperties
        {
            private static GUIStyle _ButtonStyle;

            //private Skill.Framework.UI.Box _Bg;
            private Skill.Framework.UI.Label _LblClipName;
            private Skill.Framework.UI.Button _BtnAddNext;
            private Skill.Framework.UI.Button _BtnRemove;
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
                this.ColumnDefinitions.Add(16, Skill.Framework.UI.GridUnitType.Pixel);

                //this._Bg = new Skill.Framework.UI.Box() { Column = 0, ColumnSpan = 3 };
                //this.Controls.Add(_Bg);

                this._LblClipName = new Skill.Framework.UI.Label() { Column = 0 };
                UpdateTitle();
                this.Controls.Add(_LblClipName);

                if (_ButtonStyle == null) _ButtonStyle = new GUIStyle();

                this._BtnAddNext = new Skill.Framework.UI.Button() { Column = 1, Margin = new Skill.Framework.UI.Thickness(1), Style = _ButtonStyle };
                this._BtnAddNext.Content.tooltip = "Add next";
                this.Controls.Add(_BtnAddNext);

                this._BtnRemove = new Skill.Framework.UI.Button() { Column = 2, Margin = new Skill.Framework.UI.Thickness(1), Style = _ButtonStyle };
                this._BtnRemove.Content.tooltip = "Remove";
                this.Controls.Add(_BtnRemove);

                this._BtnAddNext.Click += _BtnAddNext_Click;
                this._BtnRemove.Click += _BtnRemove_Click;
            }

            protected override void Render()
            {
                if (_RefreshStyles)
                {
                    this._BtnAddNext.Content.image = Skill.Editor.Resources.UITextures.PlusNext;
                    this._BtnRemove.Content.image = Skill.Editor.Resources.UITextures.Minus;
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
                    this._ObjectField = new Skill.Editor.UI.ObjectField<AudioClip>() { Column = 0, Margin = ControlMargin };
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

            base.titleContent = new GUIContent("Dictionary");
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = new Vector2(Size.x * 0.5f, Size.y * 0.5f);
            CreateUI();
            EnableUI();
        }
        void OnDestroy()
        {
            if (Skill.Editor.UI.InspectorProperties.GetSelected() is TextKeyView)
                Skill.Editor.UI.InspectorProperties.Select(null);

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

        void OnFocus()
        {
            if (_Frame != null)
            {
                _RefreshStyles = true;
                if (this._Dictionary != null)
                    SetLayout(_Dictionary.LayoutType);
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

        private Skill.Editor.UI.ObjectField<Dictionary> _ObjectField;
        private Skill.Editor.UI.TextField _TxtFilter;
        private Skill.Framework.UI.ListBox _ListBox;

        private Skill.Framework.UI.Button _BtnAdd;
        private Skill.Framework.UI.Button _BtnRemove;
        private Skill.Framework.UI.Button _BtnSave;

        private Skill.Framework.UI.Box _NameCaption;
        private Skill.Framework.UI.Box _ValueCaption;

        private Skill.Framework.UI.ListBox _AudioList;
        private Skill.Framework.UI.Button _BtnNew;
        private List<AudioField> _Fields;
        private AudioClipSubtitleEditor _AudioClipEditor;


        private Toolbar _LayoutButtonsPanel;
        private ToolbarButton _TBtnAudioPanel;
        private ToolbarButton _TBtnTextPanel;

        private void CreateUI()
        {
            _RefreshStyles = true;
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
            _Frame.Grid.Padding = new Thickness(2);

            _Frame.Grid.RowDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            CreateAudioPanel();
            CreateFieldPanel();
            CreateTextPanel();

            _Frame.Grid.Controls.Add(_FieldPanel);
            _Frame.Grid.Controls.Add(_AudioPanel);
            _Frame.Grid.Controls.Add(_TextPanel);

            SetLayout(0);
        }



        private void CreateAudioPanel()
        {

            _AudioPanel = new Grid() { Row = 1, Padding = new Thickness(2) };

            _AudioPanel.RowDefinitions.Add(262, Skill.Framework.UI.GridUnitType.Pixel); // audio subtitle editor
            _AudioPanel.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); //        

            _AudioClipEditor = new AudioClipSubtitleEditor(this) { Row = 0, Column = 0, ColumnSpan = 2 };
            _AudioPanel.Controls.Add(_AudioClipEditor);

            Skill.Framework.UI.Grid pnl = new Skill.Framework.UI.Grid() { Row = 1, Column = 0 };

            _Fields = new List<AudioField>();
            _AudioList = new Skill.Framework.UI.ListBox() { Row = 0, Column = 0, AlwayShowVertical = true };
            _AudioList.BackgroundVisible = true;
            _AudioList.DisableFocusable();
            pnl.Controls.Add(_AudioList);

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
            _FieldPanel.ColumnDefinitions.Add(1, GridUnitType.Star); // object field
            _FieldPanel.ColumnDefinitions.Add(40, GridUnitType.Pixel); // filter label
            _FieldPanel.ColumnDefinitions.Add(180, GridUnitType.Pixel); // filter textfiald                        
            _FieldPanel.ColumnDefinitions.Add(180, GridUnitType.Pixel); // tabs

            Box box = new Box() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 3 };
            _FieldPanel.Controls.Add(box);

            _ObjectField = new Skill.Editor.UI.ObjectField<Dictionary>() { Column = 0, Margin = new Thickness(2, 2, 30, 2), VerticalAlignment = VerticalAlignment.Top };
            _ObjectField.Label.text = "Dictionary";
            _FieldPanel.Controls.Add(_ObjectField);

            _ObjectField.ObjectChanged += _ObjectField_ObjectChanged;


            Label lblFilter = new Label() { Column = 1, Text = "filter :" };
            _FieldPanel.Controls.Add(lblFilter);

            _TxtFilter = new UI.TextField() { Column = 2 };
            _FieldPanel.Controls.Add(_TxtFilter);
            _TxtFilter.TextChanged += _TxtFilter_TextChanged;

            _LayoutButtonsPanel = new Toolbar() { Column = 3 };
            _TBtnAudioPanel = new ToolbarButton();
            _TBtnTextPanel = new ToolbarButton();

            _TBtnAudioPanel.Content.text = "Audio"; _TBtnAudioPanel.Content.tooltip = "set subtitles for audio files";
            _TBtnTextPanel.Content.text = "Text"; _TBtnTextPanel.Content.tooltip = "add text";


            _LayoutButtonsPanel.Items.Add(_TBtnTextPanel);
            _LayoutButtonsPanel.Items.Add(_TBtnAudioPanel);
            _LayoutButtonsPanel.SelectedIndex = 0;

            _FieldPanel.Controls.Add(_LayoutButtonsPanel);

            _TBtnAudioPanel.Selected += LayoutButtons_Selected;
            _TBtnTextPanel.Selected += LayoutButtons_Selected;
        }

        void _TxtFilter_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter(_TxtFilter.Text);
        }

        private void ApplyFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                foreach (TextKeyView view in _ListBox.Items)
                    view.Visibility = Skill.Framework.UI.Visibility.Visible;
            }
            else
            {
                foreach (TextKeyView view in _ListBox.Items)
                {
                    if (view.Key.Key.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        view.Visibility = Skill.Framework.UI.Visibility.Visible;
                    }
                    else
                    {
                        view.Visibility = Skill.Framework.UI.Visibility.Collapsed;
                        if (_SelectedView == view)
                            Select(null);
                    }

                }
            }
        }

        private void CreateTextPanel()
        {
            _TextPanel = new Grid() { Row = 1, Padding = new Thickness(2) };
            _TextPanel.ColumnDefinitions.Add(1, GridUnitType.Star);
            _TextPanel.ColumnDefinitions.Add(2, GridUnitType.Star);
            _TextPanel.ColumnDefinitions.Add(17, GridUnitType.Pixel);
            _TextPanel.ColumnDefinitions.Add(30, GridUnitType.Pixel);

            _TextPanel.RowDefinitions.Add(24, GridUnitType.Pixel);
            _TextPanel.RowDefinitions.Add(30, GridUnitType.Pixel);
            _TextPanel.RowDefinitions.Add(30, GridUnitType.Pixel);
            _TextPanel.RowDefinitions.Add(1, GridUnitType.Star);

            Box box = new Box() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10 };
            _TextPanel.Controls.Add(box);


            _NameCaption = new Box() { Row = 0, Column = 0 }; _NameCaption.Content.text = "Key";
            _ValueCaption = new Box() { Row = 0, Column = 1 }; _ValueCaption.Content.text = "Value";

            _ListBox = new Skill.Framework.UI.ListBox() { Row = 1, RowSpan = 3, Column = 0, ColumnSpan = 3, Margin = new Thickness(2) };
            _ListBox.BackgroundVisible = true;
            _ListBox.DisableFocusable();
            _ListBox.AlwayShowVertical = true;
            _ListBox.AutoScroll = true;

            _BtnAdd = new Skill.Framework.UI.Button() { Row = 1, Column = 3, Margin = new Thickness(2) }; _BtnAdd.Content.tooltip = "Add";
            _BtnRemove = new Skill.Framework.UI.Button() { Row = 2, Column = 3, Margin = new Thickness(2) }; _BtnAdd.Content.tooltip = "remove selected";
            _BtnSave = new Skill.Framework.UI.Button() { Row = 3, Column = 3, Margin = new Thickness(2, 2, 2, 4), VerticalAlignment = VerticalAlignment.Bottom, Height = 28 }; _BtnSave.Content.tooltip = "Save Changes";

            _TextPanel.Controls.Add(_NameCaption);
            _TextPanel.Controls.Add(_ValueCaption);
            _TextPanel.Controls.Add(_BtnAdd);
            _TextPanel.Controls.Add(_BtnRemove);
            _TextPanel.Controls.Add(_BtnSave);
            _TextPanel.Controls.Add(_ListBox);

            _BtnSave.Click += _BtnSave_Click;
            _BtnAdd.Click += _BtnAdd_Click;
            _BtnRemove.Click += _BtnRemove_Click;
            _ListBox.SelectionChanged += _ListBox_SelectionChanged;
        }

        private void RefreshStyles()
        {
            _LayoutButtonsPanel.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _ListBox.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
            _BtnAdd.Content.image = Skill.Editor.Resources.UITextures.Add;
            _BtnRemove.Content.image = Skill.Editor.Resources.UITextures.Remove;
            _BtnSave.Content.image = Skill.Editor.Resources.UITextures.Save;
            _AudioClipEditor.RefreshStyles();
            _AudioList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;

            _RefreshStyles = false;
        }
        private void EnableUI()
        {
            if (_Dictionary == null)
            {
                _TextPanel.IsEnabled = false;
                _AudioPanel.IsEnabled = false;
                _BtnSave.IsEnabled = false;
            }
            else
            {
                _TextPanel.IsEnabled = true;
                _AudioPanel.IsEnabled = true;
                _BtnSave.IsEnabled = _IsChanged;
            }
        }




        void LayoutButtons_Selected(object sender, System.EventArgs e)
        {
            SetLayout(_LayoutButtonsPanel.SelectedIndex);
        }

        private bool _IgnoreLayout;
        private void SetLayout(int layoutType)
        {
            if (_IgnoreLayout) return;
            if (_Dictionary != null)
                _Dictionary.LayoutType = layoutType;

            _IgnoreLayout = true;
            switch (layoutType)
            {
                case 0: // text

                    _TextPanel.Visibility = Visibility.Visible;
                    _AudioPanel.Visibility = Visibility.Hidden;

                    break;
                default: // audio

                    _TextPanel.Visibility = Visibility.Hidden;
                    _AudioPanel.Visibility = Visibility.Visible;
                    break;
            }
            _LayoutButtonsPanel.SelectedIndex = layoutType;
            _IgnoreLayout = false;
        }
        #endregion


        public Dictionary Dictionary { get { return _ObjectField.Object; } set { _ObjectField.Object = value; } }

        [SerializeField]
        private Dictionary _Dictionary;
        private bool _RefreshStyles;
        private bool _IsChanged;
        private TextKeyView _SelectedView;

        public void SetChanged(bool changed)
        {
            if (_IsChanged != changed)
            {
                _IsChanged = changed;
                if (_IsChanged)
                    base.titleContent.text = "Dictionary*";
                else
                    base.titleContent.text = "Dictionary";

                if (_BtnSave != null)
                    _BtnSave.IsEnabled = _IsChanged;
            }
        }

        void _AudioList_SelectionChanged(object sender, EventArgs e)
        {
            if (_AudioList.SelectedItem != null)
            {
                AudioField af = (AudioField)_AudioList.SelectedItem;
                _AudioClipEditor.Subtitle = af.Subtitle;
                InspectorProperties.Select(af);
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

        void _ListBox_SelectionChanged(object sender, System.EventArgs e)
        {
            object item = _ListBox.SelectedItem;
            Select(item);
        }

        private void Select(object item)
        {
            _BtnRemove.IsEnabled = item != null;
            if (item != null)
            {
                _SelectedView = (TextKeyView)item;
            }
            else
                _SelectedView = null;

            Skill.Editor.UI.InspectorProperties.Select(_SelectedView);
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
            TextKeyView view = new TextKeyView(new TextKey() { Key = "New" }, this);
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
                        TextKeyView view = new TextKeyView(key, this);
                        _ListBox.Controls.Add(view);
                    }
                }

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
                TextKey[] keys = new TextKey[_ListBox.Controls.Count];
                for (int i = 0; i < _ListBox.Controls.Count; i++)
                    keys[i] = ((TextKeyView)_ListBox.Controls[i]).Key;
                _Dictionary.SetKeys(keys);
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
                AudioClipSubtitle[] subtitles = new AudioClipSubtitle[_Fields.Count];
                for (int i = 0; i < _Fields.Count; i++)
                    subtitles[i] = _Fields[i].Subtitle;
                _Dictionary.SetSubtitles(subtitles);
                SetDirty2();
            }
        }

        public void ShowTextPicker(ITextPickTarget pickTarget)
        {
            if (_Dictionary != null && pickTarget != null)
            {
                if (_Dictionary != null)
                {
                    Save();
                    PickTextWindow.Instance.Show(_Dictionary, pickTarget);
                }
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
    class TextKeyView : Grid, IProperties
    {
        public TextKey Key { get; private set; }

        private Label _LblName;
        private Label _LblValue;
        private DictionaryEditorWindow _OwnerEditor;


        public TextKeyView(TextKey key, DictionaryEditorWindow owner)
        {
            this._OwnerEditor = owner;
            this.Key = key;
            ColumnDefinitions.Add(1, GridUnitType.Star);
            ColumnDefinitions.Add(2, GridUnitType.Star);

            _LblName = new Label() { Column = 0 };
            _LblValue = new Label() { Column = 1 };

            this.Controls.Add(_LblName);
            this.Controls.Add(_LblValue);

            UpdateTexts();
        }

        private void UpdateTexts()
        {
            _LblName.Text = Key.Key;
            _LblValue.Text = Key.Value;
        }

        public string Title { get { return "TextKey"; } }
        public bool IsSelectedProperties { get; set; }

        private MyProperties _Properties;
        public PropertiesPanel Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = new MyProperties(this);
                return _Properties;
            }
        }

        [ExposeProperty(100, "Key")]
        public string Key_Key { get { return Key.Key; } set { Key.Key = value; _LblName.Text = value; } }


        [ExposeProperty(101, "Value")]
        [PasteTextField(true)]
        public string Key_Value { get { return Key.Value; } set { Key.Value = value; _LblValue.Text = value; } }

        [ExposeProperty(102, "Comment")]
        [PasteTextField(true)]
        public string Key_Comment { get { return Key.Comment; } set { Key.Comment = value; } }

        class MyProperties : ExposeProperties
        {
            public MyProperties(TextKeyView owner)
                : base(owner)
            {

            }

            protected override void SetDirty()
            {
                TextKeyView owner = (TextKeyView)Object;
                if (owner._OwnerEditor != null)
                    owner._OwnerEditor.SetChanged(true);
            }

            protected override void RefreshData()
            {
                base.RefreshData();
                TextKeyView owner = (TextKeyView)Object;
                owner.UpdateTexts();
            }
        }
    }
    #endregion

}
