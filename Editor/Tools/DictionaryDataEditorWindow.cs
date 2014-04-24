using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using UnityEditor;
using Skill.Framework;


namespace Skill.Editor.Tools
{
    public class DictionaryDataEditorWindow : UnityEditor.EditorWindow
    {



        #region EditorWindow
        private static DictionaryDataEditorWindow _Instance;
        public static DictionaryDataEditorWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = ScriptableObject.CreateInstance<DictionaryDataEditorWindow>();
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
        public DictionaryDataEditorWindow()
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
            _Instance = null;
        }
        void OnDisable()
        {
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
        #endregion

        #region UI

        private Skill.Editor.UI.EditorFrame _Frame;

        private Skill.Editor.UI.ObjectField<DictionaryData> _ObjectField;
        private Skill.Framework.UI.Extended.ListBox _ListBox;
        private Skill.Framework.UI.Panel _EditPanel;
        private Skill.Framework.UI.Panel _ViewPanel;

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
        private void CreateUI()
        {
            _RefreshStyles = true;
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
            _Frame.Grid.Padding = new Thickness(2);

            _Frame.Grid.RowDefinitions.Add(50, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _Frame.Grid.RowDefinitions.Add(80, Skill.Framework.UI.GridUnitType.Pixel);

            CreateBox1();
            CreateBox2();
            CreateBox3();

        }
        private void CreateBox1()
        {
            Grid grid = new Grid() { Row = 0, Column = 0, Padding = new Thickness(2) };
            grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(50, GridUnitType.Pixel);
            Box box = new Box() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10 };
            grid.Controls.Add(box);

            _ObjectField = new Skill.Editor.UI.ObjectField<DictionaryData>() { Margin = new Thickness(2), VerticalAlignment = VerticalAlignment.Top };
            _ObjectField.Label.text = "Dictionary";
            grid.Controls.Add(_ObjectField);

            _BtnSave = new Skill.Editor.UI.Button() { Column = 1, Margin = new Thickness(2) };
            _BtnSave.Content.tooltip = "Save Changes";
            grid.Controls.Add(_BtnSave);

            _Frame.Controls.Add(grid);
        }
        private void CreateBox2()
        {
            Grid grid = new Grid() { Row = 1, Column = 0, Padding = new Thickness(2) };
            _ViewPanel = grid;
            grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(2, GridUnitType.Star);
            grid.ColumnDefinitions.Add(17, GridUnitType.Pixel);
            grid.ColumnDefinitions.Add(50, GridUnitType.Pixel);

            grid.RowDefinitions.Add(24, GridUnitType.Pixel);
            grid.RowDefinitions.Add(50, GridUnitType.Pixel);
            grid.RowDefinitions.Add(50, GridUnitType.Pixel);
            grid.RowDefinitions.Add(1, GridUnitType.Star);

            Box box = new Box() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10 };
            grid.Controls.Add(box);


            _NameCaption = new Box() { Row = 0, Column = 0 }; _NameCaption.Content.text = "Key";
            _ValueCaption = new Box() { Row = 0, Column = 1 }; _ValueCaption.Content.text = "Value";

            _ListBox = new Skill.Framework.UI.Extended.ListBox() { Row = 1, RowSpan = 3, Column = 0, ColumnSpan = 3, Margin = new Thickness(2) };
            _ListBox.BackgroundVisible = true;
            _ListBox.DisableFocusable();
            _ListBox.AlwayShowVertical = true;
            _ListBox.AutoScroll = true;

            _BtnAdd = new Skill.Editor.UI.Button() { Row = 1, Column = 3, Margin = new Thickness(2) }; _BtnAdd.Content.tooltip = "Add";
            _BtnRemove = new Skill.Editor.UI.Button() { Row = 2, Column = 3, Margin = new Thickness(2) }; _BtnAdd.Content.tooltip = "remove selected";

            grid.Controls.Add(_NameCaption);
            grid.Controls.Add(_ValueCaption);
            grid.Controls.Add(_BtnAdd);
            grid.Controls.Add(_BtnRemove);
            grid.Controls.Add(_ListBox);

            _Frame.Controls.Add(grid);
        }
        private void CreateBox3()
        {
            Grid grid = new Grid() { Row = 2, Column = 0, Padding = new Thickness(2) };
            _EditPanel = grid;
            grid.ColumnDefinitions.Add(60, GridUnitType.Pixel);
            grid.ColumnDefinitions.Add(2, GridUnitType.Star);
            grid.ColumnDefinitions.Add(60, GridUnitType.Pixel);
            grid.ColumnDefinitions.Add(1, GridUnitType.Star);

            grid.RowDefinitions.Add(1, GridUnitType.Star);
            grid.RowDefinitions.Add(2, GridUnitType.Star);

            Box box = new Box() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10 };
            grid.Controls.Add(box);


            _LblName = new Label() { Row = 0, Column = 0, Margin = new Thickness(2), Text = "Key" };
            _LblValue = new Label() { Row = 1, Column = 0, Margin = new Thickness(2), Text = "Value" };
            _LblComment = new Label() { Row = 0, Column = 2, ColumnSpan = 2, Margin = new Thickness(2), Text = "Comment" };

            _TxtName = new PasteTextField() { Row = 0, Column = 1, Margin = new Thickness(2, 2, 20, 2) };
            _TxtValue = new PasteTextField() { Row = 1, Column = 1, Margin = new Thickness(2, 2, 20, 2) };
            _TxtComment = new PasteTextField() { Row = 1, Column = 2, ColumnSpan = 2, Margin = new Thickness(2) };

            grid.Controls.Add(_LblName);
            grid.Controls.Add(_LblValue);
            grid.Controls.Add(_LblComment);

            grid.Controls.Add(_TxtName);
            grid.Controls.Add(_TxtValue);
            grid.Controls.Add(_TxtComment);

            _Frame.Controls.Add(grid);
        }

        private void RefreshStyles()
        {
            _ListBox.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
            _BtnAdd.Content.image = Skill.Editor.Resources.Textures.Add;
            _BtnRemove.Content.image = Skill.Editor.Resources.Textures.Remove;
            _BtnSave.Content.image = Skill.Editor.Resources.Textures.Save;
            if (_LblComment.Style == null)
                _LblComment.Style = new GUIStyle(UnityEditor.EditorGUIUtility.GetBuiltinSkin(UnityEditor.EditorSkin.Inspector).label);
            _LblComment.Style.alignment = TextAnchor.MiddleCenter;
            _RefreshStyles = false;
        }
        private void EnableUI()
        {
            if (_Dictionary == null)
            {

                _EditPanel.IsEnabled = false;
                _ViewPanel.IsEnabled = false;
                _BtnSave.IsEnabled = false;
                _TxtName.TextField.Text = string.Empty;
                _TxtValue.TextField.Text = string.Empty;
                _TxtComment.TextField.Text = string.Empty;
            }
            else
            {
                _EditPanel.IsEnabled = true;
                _ViewPanel.IsEnabled = true;
                _BtnSave.IsEnabled = _IsChanged;
            }
        }
        #endregion


        public DictionaryData Data { get { return _ObjectField.Object; } set { _ObjectField.Object = value; } }

        [SerializeField]
        private DictionaryData _Dictionary;
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
            }
            SetChanged(false);
            _ListBox.SelectedIndex = 0;
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
