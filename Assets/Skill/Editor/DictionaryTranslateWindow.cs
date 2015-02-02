using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System.Collections.Generic;
using Skill.Framework;


namespace Skill.Editor
{
    public class DictionaryTranslateWindow : UnityEditor.EditorWindow
    {

        #region TextKeyTranslateView
        class TextKeyTranslateView : Grid
        {
            public TextKey SourceKey { get; private set; }
            public TextKey TranslateKey { get; private set; }

            private Label _LblSource;
            private Label _LblTranslate;
            private PasteTextField _TxtTranslate;

            private bool _Selected;
            public bool Selected
            {
                get { return _Selected; }
                set
                {
                    if (_Selected != value)
                    {
                        _Selected = value;
                        _TxtTranslate.Visibility = _Selected ? Visibility.Visible : Visibility.Hidden;
                        _LblTranslate.Visibility = _Selected ? Visibility.Hidden : Visibility.Visible;
                        if (!_Selected)
                            Save();
                    }
                }
            }

            public event System.EventHandler Changed;
            private void OnChanged()
            {
                if (Changed != null)
                    Changed(this, System.EventArgs.Empty);
            }

            public void Save()
            {
                _LblTranslate.Text = _TxtTranslate.TextField.Text;
                TranslateKey.Value = _TxtTranslate.TextField.Text;
            }

            public TextKeyTranslateView(TextKey sourcekey, TextKey translatekey)
            {
                this.SourceKey = sourcekey;
                this.TranslateKey = translatekey;

                ColumnDefinitions.Add(1, GridUnitType.Star);
                ColumnDefinitions.Add(1, GridUnitType.Star);

                _LblSource = new Label() { Column = 0, Text = SourceKey.Value };
                _LblTranslate = new Label() { Column = 1, Text = TranslateKey.Value };
                _TxtTranslate = new PasteTextField() { Column = 1, Visibility = Skill.Framework.UI.Visibility.Hidden };
                _TxtTranslate.TextField.Text = TranslateKey.Value;

                this.Controls.Add(_LblSource);
                this.Controls.Add(_LblTranslate);
                this.Controls.Add(_TxtTranslate);

                _TxtTranslate.TextField.TextChanged += _TxtTranslate_TextChanged;
            }

            void _TxtTranslate_TextChanged(object sender, System.EventArgs e)
            {
                OnChanged();
            }
        }
        #endregion

        #region EditorWindow
        private static DictionaryTranslateWindow _Instance;
        public static DictionaryTranslateWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = ScriptableObject.CreateInstance<DictionaryTranslateWindow>();
                }
                return _Instance;
            }
        }

        private static Vector2 Size = new Vector2(800, 600);

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
        public DictionaryTranslateWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.title = "Translator";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = new Vector2(Size.x * 0.5f, Size.y * 0.5f);
            CreateUI();
            HookEvents();
            EnableUI();
        }
        void OnDestroy()
        {
            _Instance = null;
        }
        void OnDisable()
        {
            if (_IsChanged)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Save", "Save changes to translate Dictionary?", "Save", "Don't Save"))
                    Save();
            }
            _Instance = null;
        }
        void OnEnable()
        {
            _RefreshStyles = true;
            _SourceField.Object = _Source;
            _TranslateField.Object = _Translate;
        }

        void OnFocus()
        {
            if (_Frame != null)
            {
                _RefreshStyles = true;
            }
        }
        #endregion

        #region UI

        private Skill.Editor.UI.EditorFrame _Frame;

        private Skill.Editor.UI.ObjectField<Dictionary> _SourceField;
        private Skill.Editor.UI.ObjectField<Dictionary> _TranslateField;
        private Skill.Framework.UI.Extended.ListBox _ListBox;
        private Skill.Framework.UI.Panel _PnlItems;

        private Skill.Framework.UI.Button _BtnCopyKeys;
        private Skill.Framework.UI.Button _BtnSave;

        private Skill.Framework.UI.Box _SourceCaption;
        private Skill.Framework.UI.Box _TranslateCaption;
        private void CreateUI()
        {
            _RefreshStyles = true;
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
            _Frame.Grid.Padding = new Thickness(2);

            _Frame.Grid.RowDefinitions.Add(50, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            CreateBox1();
            CreateBox2();

        }
        private void CreateBox1()
        {
            Grid grid = new Grid() { Row = 0, Column = 0, Padding = new Thickness(2) };
            grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(50, GridUnitType.Pixel);

            grid.RowDefinitions.Add(1, GridUnitType.Star);
            grid.RowDefinitions.Add(1, GridUnitType.Star);

            Box box = new Box() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10 };
            grid.Controls.Add(box);

            _SourceField = new Skill.Editor.UI.ObjectField<Dictionary>() { Row = 0, Column = 0, Margin = new Thickness(2), VerticalAlignment = VerticalAlignment.Bottom };
            _SourceField.Label.text = "Source";
            grid.Controls.Add(_SourceField);

            _TranslateField = new Skill.Editor.UI.ObjectField<Dictionary>() { Row = 1, Column = 0, Margin = new Thickness(2), VerticalAlignment = VerticalAlignment.Top };
            _TranslateField.Label.text = "Translate";
            grid.Controls.Add(_TranslateField);

            _BtnSave = new Skill.Framework.UI.Button() { Row = 0, Column = 1, RowSpan = 2, Margin = new Thickness(2) };
            _BtnSave.Content.tooltip = "Save Changes";
            grid.Controls.Add(_BtnSave);

            _Frame.Controls.Add(grid);
        }
        private void CreateBox2()
        {
            Grid grid = new Grid() { Row = 1, Column = 0, Padding = new Thickness(2) };
            _PnlItems = grid;
            grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(1, GridUnitType.Star);

            grid.RowDefinitions.Add(32, GridUnitType.Pixel);
            grid.RowDefinitions.Add(24, GridUnitType.Pixel);
            grid.RowDefinitions.Add(1, GridUnitType.Star);

            Box box = new Box() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10 };
            grid.Controls.Add(box);

            _BtnCopyKeys = new Skill.Framework.UI.Button() { Row = 0, Column = 0, ColumnSpan = 2, Width = 46, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(2) };
            _BtnCopyKeys.Content.tooltip = "Copy keys to translate dictionary";

            _SourceCaption = new Box() { Row = 1, Column = 0 }; _SourceCaption.Content.text = "Source";
            _TranslateCaption = new Box() { Row = 1, Column = 1 }; _TranslateCaption.Content.text = "Translate";

            _ListBox = new Skill.Framework.UI.Extended.ListBox() { Row = 2, Column = 0, ColumnSpan = 2, Margin = new Thickness(2) };
            _ListBox.BackgroundVisible = true;
            _ListBox.DisableFocusable();
            _ListBox.AlwayShowVertical = true;
            _ListBox.AutoScroll = true;

            grid.Controls.Add(_BtnCopyKeys);
            grid.Controls.Add(_SourceCaption);
            grid.Controls.Add(_TranslateCaption);
            grid.Controls.Add(_ListBox);

            _Frame.Controls.Add(grid);
        }

        private void RefreshStyles()
        {
            _ListBox.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
            _BtnCopyKeys.Content.image = Skill.Editor.Resources.UITextures.Remove;
            _BtnSave.Content.image = Skill.Editor.Resources.UITextures.Save;
            _BtnCopyKeys.Content.image = Skill.Editor.Resources.UITextures.ArrowRight;
            _RefreshStyles = false;
        }
        private void EnableUI()
        {
            if (_Translate == null || _Source == null)
            {
                _BtnSave.IsEnabled = false;
                Clear();
                _PnlItems.IsEnabled = false;
                _BtnSave.IsEnabled = false;
            }
            else
            {
                _BtnSave.IsEnabled = _IsChanged;
                _PnlItems.IsEnabled = true;
            }
        }
        #endregion


        public Dictionary Source { get { return _Source; } set { _SourceField.Object = value; } }
        public Dictionary Translate { get { return _Translate; } set { _TranslateField.Object = value; } }


        [SerializeField]
        private Dictionary _Source;
        [SerializeField]
        private Dictionary _Translate;
        private bool _RefreshStyles;
        private TextKeyTranslateView _SelectedView;
        private bool _IsChanged;




        private void SetChanged(bool changed)
        {
            if (_IsChanged != changed)
            {
                _IsChanged = changed;
                if (_IsChanged)
                    base.title = "Translator*";
                else
                    base.title = "Translator";

                if (_BtnSave != null)
                    _BtnSave.IsEnabled = _IsChanged;
            }
        }

        private void HookEvents()
        {
            _BtnSave.Click += _BtnSave_Click;
            _ListBox.SelectionChanged += _ListBox_SelectionChanged;
            _SourceField.ObjectChanged += _SourceField_ObjectChanged;
            _TranslateField.ObjectChanged += _TranslateField_ObjectChanged;
            _BtnCopyKeys.Click += _BtnCopyKeys_Click;
        }

        void _BtnCopyKeys_Click(object sender, System.EventArgs e)
        {
            CopyKeys();
        }
        void _ListBox_SelectionChanged(object sender, System.EventArgs e)
        {
            if (_SelectedView != null)
                _SelectedView.Selected = false;
            _SelectedView = null;

            object item = _ListBox.SelectedItem;
            if (item != null)
            {
                _SelectedView = (TextKeyTranslateView)item;
                _SelectedView.Selected = true;
            }
            else
                _SelectedView = null;

            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(_Frame.Grid);
        }
        void _BtnSave_Click(object sender, System.EventArgs e)
        {
            Save();
        }

        void _SourceField_ObjectChanged(object sender, System.EventArgs e)
        {
            _Source = _SourceField.Object;
            Rebuild();
            EnableUI();
        }
        void _TranslateField_ObjectChanged(object sender, System.EventArgs e)
        {
            _Translate = _TranslateField.Object;
            Rebuild();
            EnableUI();
        }

        private void Clear()
        {
            if (_SelectedView != null)
                _SelectedView.Selected = false;
            _SelectedView = null;
            _ListBox.Controls.Clear();
        }
        private void Rebuild()
        {
            Clear();
            if (_Source != null && _Translate != null)
            {
                if (_Source.Keys != null)
                {
                    foreach (var sKey in _Source.Keys)
                    {
                        TextKey tKey = GetKeyInTranslate(sKey.Key);
                        if (tKey != null)
                        {
                            TextKeyTranslateView view = new TextKeyTranslateView(sKey, tKey);
                            view.Changed += view_Changed;
                            _ListBox.Controls.Add(view);
                        }
                    }
                }
            }
            SetChanged(false);
            _ListBox.SelectedIndex = -1;
        }

        void view_Changed(object sender, System.EventArgs e)
        {
            SetChanged(true);
        }
        private void CopyKeys()
        {
            bool change = false;
            if (_Translate != null && _Source != null)
            {

                List<TextKey> textList = new List<TextKey>();
                if (_Translate.Keys != null)
                {
                    foreach (var tKey in _Translate.Keys)
                        textList.Add(tKey);
                }
                if (_Source.Keys != null)
                {
                    foreach (var sKey in _Source.Keys)
                    {
                        TextKey tKey = GetKeyInTranslate(sKey.Key);
                        if (tKey == null)
                        {
                            tKey = new TextKey() { Key = sKey.Key, Comment = sKey.Comment, Value = string.Empty };
                            textList.Add(tKey);
                            change = true;
                        }
                    }
                }
                _Translate.Keys = textList.ToArray();
                Rebuild();
                if (change)
                    SetChanged(true);
            }
        }

        private TextKey GetKeyInTranslate(string key) { return GetKey(_Translate, key); }
        private TextKey GetKeyInSource(string key) { return GetKey(_Source, key); }
        private TextKey GetKey(Dictionary dictionary, string key)
        {
            if (dictionary != null && dictionary.Keys != null)
            {
                foreach (var item in dictionary.Keys)
                {
                    if (item.Key == key)
                        return item;
                }
            }
            return null;
        }


        private AudioClipSubtitle GetSubtitleInTranslate(AudioClip clip) { return GetSubtitle(_Translate, clip); }
        private AudioClipSubtitle GetSubtitleInSource(AudioClip clip) { return GetSubtitle(_Source, clip); }
        private AudioClipSubtitle GetSubtitle(Dictionary dictionary, AudioClip clip)
        {
            if (dictionary != null && dictionary.Subtitles != null)
            {
                foreach (var item in dictionary.Subtitles)
                {
                    if (item.Clip == clip)
                        return item;
                }
            }
            return null;
        }

        private void Save()
        {
            if (_Translate != null && _IsChanged)
            {
                if (_SelectedView != null)
                    _SelectedView.Selected = false;

                _Translate.Keys = new TextKey[_ListBox.Controls.Count];
                for (int i = 0; i < _ListBox.Controls.Count; i++)
                    _Translate.Keys[i] = ((TextKeyTranslateView)_ListBox.Controls[i]).TranslateKey;
                UnityEditor.EditorUtility.SetDirty(_Translate);

                if (_SelectedView != null)
                    _SelectedView.Selected = true;

                SetChanged(false);
            }
        }

    }

}



