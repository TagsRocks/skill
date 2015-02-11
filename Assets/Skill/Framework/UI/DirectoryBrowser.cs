using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;


namespace Skill.Framework.UI
{
    public class DirectoryBrowser : Grid
    {
        /// <summary> Defaults to working directory </summary> 
        public string CurrentDirectory
        {
            get
            {
                return _CurrentDirectory;
            }
            set
            {
                if (value == null || _CurrentDirectory == value) return;
                _CurrentDirectory = value;
                ReadDirectoryContents();
            }
        }


        /// <summary> text of Select button </summary>
        public string SelectText { get { return _BtnSelect.Content.text; } set { _BtnSelect.Content.text = value; } }
        /// <summary> text of Cancel button </summary>
        public string CancelText { get { return _BtnCancel.Content.text; } set { _BtnCancel.Content.text = value; } }
        /// <summary> text of up directory button </summary>
        public string UpText { get { return _BtnUp.Content.text; } set { _BtnUp.Content.text = value; } }

        /// <summary> optional skin for rest of controls </summary>
        public GUISkin Skin { get; set; }

        /// <summary> Height of items </summary>
        public float ItemHeight
        {
            get { return _ItemHeight; }
            set
            {
                if (_ItemHeight != value)
                {
                    _ItemHeight = value;
                    foreach (var item in _List.Items)
                        item.Height = _ItemHeight;
                }
            }
        }

        public string SelectedPath
        {
            get
            {
                Label item = null;
                if (_List.SelectedItem != null)
                    item = (Label)_List.SelectedItem;
                if (item != null)
                    return Path.Combine(CurrentDirectory, item.Text);
                return null;
            }
            set
            {
                if (value == null || _CurrentDirectory == value) return;
                string parentDir = Path.GetDirectoryName(value);
                if (_CurrentDirectory == parentDir) return;
                _CurrentDirectory = parentDir;
                ReadDirectoryContents();

                if (_List.Items.Count > 0)
                {
                    int index = value.LastIndexOf(Path.DirectorySeparatorChar);
                    if (index > 0 && index < value.Length - 1)
                    {
                        string pathName = value.Substring(index + 1);
                        foreach (Label item in _List.Items)
                        {
                            if (item.Text == pathName)
                            {
                                _List.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // events
        /// <summary> Occurs when Select button clicked </summary>
        public event EventHandler Select;
        /// <summary> Occurs when Select button clicked </summary>
        protected virtual void OnSelect()
        {
            if (Select != null) Select(this, EventArgs.Empty);
        }

        /// <summary> Occurs when Cancel button clicked </summary>
        public event EventHandler Cancel;
        /// <summary> Occurs when Cancel button clicked </summary>
        protected virtual void OnCancel()
        {
            if (Cancel != null) Cancel(this, EventArgs.Empty);
        }


        private string _Filter;
        private string _CurrentDirectory;
        private Texture2D _DirectoryImage;
        private GUIStyle _DirectoryStyle;
        private GUIStyle _PathButtonStyle;
        private GUIStyle _PathDividerStyle;
        private GUIStyle _BackgroundStyle;
        private string[] _CurrentDirectoryParts;
        private float _ItemHeight;
        private string[] _Directories;
        private bool _ShowNewFolderButton;

        // UI
        private ListBox _List;
        private StackPanel _PnlPathParts;
        private Grid _PnlButtons;
        private Button _BtnSelect;
        private Button _BtnCancel;
        private Button _BtnUp;
        private Box _Background;
        private Box _Separator;

        private Grid _PnlGetName;
        private TextField _TxtNewFolderName;
        private Button _BtnNewFolderOk;
        private Button _BtnNewFolderCancel;

        private bool _UpdatePathWidth;
        private float _PathWidth;


        public DirectoryBrowser()
        {
            this.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(2, GridUnitType.Pixel);
            this.ColumnDefinitions.Add(30, GridUnitType.Pixel);

            this.RowDefinitions.Add(30, GridUnitType.Pixel); // path parts
            this.RowDefinitions.Add(1, GridUnitType.Star); // list        
            this.RowDefinitions.Add(30, GridUnitType.Pixel); // buttons


            this._Background = new Box() { Row = 0, Column = 0, RowSpan = 4, ColumnSpan = 3 };
            this.Controls.Add(_Background);

            this._BtnUp = new Button() { Row = 0, Column = 2, Margin = new Thickness(1) };
            this.Controls.Add(_BtnUp);

            this._Separator = new Box() { Row = 0, Column = 1 };
            this.Controls.Add(_Separator);

            this._PnlPathParts = new StackPanel() { Row = 0, Column = 0, Orientation = Orientation.Horizontal, HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Left };
            this.Controls.Add(_PnlPathParts);

            this._List = new ListBox() { Row = 1, Column = 0, ColumnSpan = 3 };
            this._List.DisableFocusable();
            this.Controls.Add(_List);

            this._PnlButtons = new Grid() { Row = 2, Column = 0, ColumnSpan = 3 };
            this._PnlButtons.ColumnDefinitions.Add(1, GridUnitType.Star); // new folder button
            this._PnlButtons.ColumnDefinitions.Add(3, GridUnitType.Star);
            this._PnlButtons.ColumnDefinitions.Add(1, GridUnitType.Star);
            this._PnlButtons.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.Controls.Add(_PnlButtons);

            _BtnSelect = new Button() { IsEnabled = false, Column = 3, Margin = new Thickness(2) };
            this._PnlButtons.Controls.Add(_BtnSelect);

            _BtnCancel = new Button() { Column = 2, Margin = new Thickness(2) };
            this._PnlButtons.Controls.Add(_BtnCancel);

            this._ItemHeight = 20;
            this.CurrentDirectory = Directory.GetCurrentDirectory();

            SelectText = "Select";
            CancelText = "Cancel";
            UpText = "Up";
            this._List.SelectionChanged += _List_SelectionChanged;
            this._List.SelectedDoubleClick += _List_SelectedDoubleClick;
            this._BtnSelect.Click += _BtnSelect_Click;
            this._BtnCancel.Click += _BtnCancel_Click;
            this._BtnUp.Click += _BtnUp_Click;
        }

        void _BtnUp_Click(object sender, EventArgs e)
        {
            if (_CurrentDirectoryParts != null)
            {
                if (_CurrentDirectoryParts.Length > 1)
                    GoToPathPart(_CurrentDirectoryParts.Length - 2);
            }
        }

        protected void AddButton(Button btn)
        {
            if (btn != null)
                this._PnlButtons.Controls.Add(btn);
        }

        void _BtnCancel_Click(object sender, EventArgs e)
        {
            OnCancel();
        }

        void _BtnSelect_Click(object sender, EventArgs e)
        {
            OnSelect();
        }

        void _List_SelectedDoubleClick(object sender, EventArgs e)
        {
            if (_List.SelectedItem != null)
            {
                Label li = (Label)_List.SelectedItem;
                CurrentDirectory = Path.Combine(CurrentDirectory, li.Text);
            }
        }

        void _List_SelectionChanged(object sender, EventArgs e)
        {
            _BtnSelect.IsEnabled = _List.SelectedItem != null;
        }

        private void ReadDirectoryContents()
        {
            if (_CurrentDirectory == "/")
                _CurrentDirectoryParts = new string[] { "" };
            else
                _CurrentDirectoryParts = _CurrentDirectory.Split(Path.DirectorySeparatorChar);

            _Directories = Directory.GetDirectories(_CurrentDirectory);

            for (int i = 0; i < _Directories.Length; ++i)
                _Directories[i] = _Directories[i].Substring(_Directories[i].LastIndexOf(Path.DirectorySeparatorChar) + 1);
            Array.Sort(_Directories);

            RebuildList();
            RebuildStyles();
            ReBuildDirectoryParts();
        }

        private void ReBuildDirectoryParts()
        {

            foreach (BaseControl bt in _PnlPathParts.Controls)
            {
                if (bt is Button)
                    ((Button)bt).Click -= PathButton_Click;
            }

            _PnlPathParts.Controls.Clear();
            _PathWidth = 0;
            if (_CurrentDirectoryParts != null)
            {
                for (int i = 0; i < _CurrentDirectoryParts.Length; i++)
                {
                    Button btn = new Button() { Margin = new Thickness(2, 2, 0, 2), Style = _PathButtonStyle };
                    btn.Content.text = _CurrentDirectoryParts[i];
                    btn.UserData = i;
                    if (_PathButtonStyle != null)
                        btn.Width = _PathButtonStyle.CalcSize(btn.Content).x + 4;
                    _PnlPathParts.Controls.Add(btn);

                    if (i < _CurrentDirectoryParts.Length - 1)
                    {
                        Box divider = new Box() { Width = 8, Style = _PathDividerStyle };
                        _PnlPathParts.Controls.Add(divider);
                        _PathWidth += divider.LayoutWidth;
                    }

                    btn.Click += PathButton_Click;
                    _PathWidth += btn.LayoutWidth;
                }
            }
            _UpdatePathWidth = true;
        }

        void PathButton_Click(object sender, EventArgs e)
        {
            GoToPathPart((int)((Button)sender).UserData);
        }

        private void GoToPathPart(int index)
        {
            string parentDirectoryName = CurrentDirectory;
            for (int i = _CurrentDirectoryParts.Length - 1; i > index; --i)
                parentDirectoryName = Path.GetDirectoryName(parentDirectoryName);
            CurrentDirectory = parentDirectoryName;
        }

        protected override void Render()
        {
            if (_UpdatePathWidth)
            {
                _UpdatePathWidth = false;
                float renderWidth = _PnlPathParts.RenderArea.width;
                Thickness margin = _PnlPathParts.Margin;
                margin.Left = Mathf.Min(margin.Right, renderWidth - _PathWidth - 10);
                _PnlPathParts.Margin = margin;
            }

            GUISkin savedSkin = GUI.skin;
            if (this.Skin != null)
                GUI.skin = this.Skin;
            base.Render();

            if (this.Skin != null)
                GUI.skin = savedSkin;
        }

        private void RebuildList()
        {
            _List.Items.Clear();
            if (_Directories != null)
            {
                foreach (var dir in _Directories)
                {
                    Label item = new Label() { Text = dir, Height = ItemHeight };
                    _List.Items.Add(item);
                }
            }
        }

        private void RebuildStyles()
        {
            foreach (Label item in _List.Items)
            {
                item.Height = _ItemHeight;
                item.Content.image = _DirectoryImage;
                item.Style = _DirectoryStyle;
            }

            foreach (BaseControl bt in _PnlPathParts.Controls)
            {
                if (bt is Button)
                    ((Button)bt).Click -= PathButton_Click;
            }
            _Background.Style = _BackgroundStyle;
        }

        public virtual void ImportStyles(BrowserStyles styles)
        {
            if (styles == null) return;
            this._ItemHeight = styles.ItemHeight;

            this.ColumnDefinitions[2].Width = new GridLength(styles.UpWidth, GridUnitType.Pixel);
            this.RowDefinitions[0].Height = new GridLength(styles.UpHeigth, GridUnitType.Pixel);
            this.RowDefinitions[2].Height = new GridLength(styles.ButtonHeigth, GridUnitType.Pixel);

            this._List.BackgroundVisible = styles.ListBackgroundStyle != null;
            this._List.Background.Style = styles.ListBackgroundStyle;
            this._DirectoryImage = styles.DirectoryImage;
            this._PathDividerStyle = styles.PathDividerStyle;
            this._BtnSelect.Content.image = styles.SelectIcon;
            this._BtnCancel.Content.image = styles.CancelIcon;
            this._BtnUp.Content.image = styles.UpIcon;

            this._Separator.Style = styles.DividerStyle;
            this._BtnUp.Style = styles.PathButtonStyle;
            this._BackgroundStyle = styles.BackgroundStyle;
            this._DirectoryStyle = styles.DirectoryStyle;
            this._PathButtonStyle = styles.PathButtonStyle;
            this._List.SelectedStyle = styles.SelectedItemStyle;

            RebuildStyles();
            ReBuildDirectoryParts();
        }
    }
}