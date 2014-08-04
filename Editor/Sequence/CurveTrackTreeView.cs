using Skill.Editor.Curve;
using Skill.Editor.UI.Extended;
using Skill.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor.Sequence
{
    internal class CurveTrackTreeView : Skill.Framework.UI.Grid
    {
        private TreeView _TreeView;
        private CurveEditor _CurveEditor;
        private Skill.Framework.UI.Grid _Toolbar;
        private Skill.Framework.UI.Label _Title;
        private Button _BtnAddKey;
        private Button _BtnClear;

        internal CurveTrackTreeView(CurveEditor curveEditor)
        {
            this._CurveEditor = curveEditor;
            RowDefinitions.Add(25, Skill.Framework.UI.GridUnitType.Pixel);
            RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            _Toolbar = new Grid();
            _Toolbar.ColumnDefinitions.Add(30, GridUnitType.Pixel);
            _Toolbar.ColumnDefinitions.Add(1, GridUnitType.Star);
            _Toolbar.ColumnDefinitions.Add(30, GridUnitType.Pixel);

            // create header
            _Title = new Skill.Framework.UI.Label() { Row = 0, Column = 1 };
            _Title.Text = "Curves";
            _Toolbar.Controls.Add(_Title);


            _BtnClear = new Button() { Row = 0, Column = 0 };
            _BtnClear.Content.tooltip = "Remove all curves";
            _Toolbar.Controls.Add(_BtnClear);

            _BtnAddKey = new Button() { Row = 0, Column = 2, IsEnabled = false };
            _BtnAddKey.Content.tooltip = "Add key to Selected";
            _Toolbar.Controls.Add(_BtnAddKey);


            Controls.Add(_Toolbar);

            _TreeView = new TreeView() { Row = 1, UserData = this, HandleScrollWheel = true };
            _TreeView.DisableFocusable();
            Controls.Add(_TreeView);

            _BtnClear.Click += _BtnClear_Click;
            _BtnAddKey.Click += _BtnAddKey_Click;
            _CurveEditor.Changed += _CurveEditor_Changed;
            _TreeView.SelectedItemChanged += _TreeView_SelectedItemChanged;
        }

        void _TreeView_SelectedItemChanged(object sender, EventArgs e)
        {
            _BtnAddKey.IsEnabled = _TreeView.SelectedItem != null;
        }

        void _BtnClear_Click(object sender, EventArgs e)
        {
            foreach (FolderView item in _TreeView.Controls)
                Remove(item);
        }

        void _BtnAddKey_Click(object sender, EventArgs e)
        {
            if (_TreeView.SelectedItem != null)
            {
                if (_TreeView.SelectedItem is FolderView)
                {
                    foreach (CurveTrackTreeViewItem item in ((FolderView)_TreeView.SelectedItem).Controls)
                        item.AddKey();
                }
                else if (_TreeView.SelectedItem is CurveTrackTreeViewItem)
                {
                    ((CurveTrackTreeViewItem)_TreeView.SelectedItem).AddKey();
                }
            }
        }

        private bool _RefreshStyle;
        protected override void Render()
        {
            if (!_RefreshStyle)
            {
                _BtnClear.Style = Skill.Editor.Resources.Styles.ToolbarButton;
                _BtnClear.Content.image = UnityEditor.EditorGUIUtility.FindTexture("d_winbtn_win_close");
                _BtnAddKey.Style = Skill.Editor.Resources.Styles.ToolbarButton;
                _BtnAddKey.Content.image = UnityEditor.EditorGUIUtility.FindTexture("d_Animation.AddKeyframe");
                _Title.Style = Skill.Editor.Resources.Styles.Header;
                _TreeView.Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;
                _RefreshStyle = true;
            }

            // update name if changed by user
            foreach (FolderView folder in _TreeView.Controls)
            {
                BaseTrackBar trackBar = (BaseTrackBar)(folder.UserData);
                if (!trackBar.Track.IsDestroyed)
                    folder.Foldout.Content.text = trackBar.Track.gameObject.name;
            }

            base.Render();
        }

        public void Add(BaseTrackBar trackBar)
        {
            // avoid edit same trackBar twice
            foreach (FolderView folder in _TreeView.Controls)
            {
                if ((BaseTrackBar)folder.UserData == trackBar)
                    return;
            }

            Curve.CurveEditor.EditCurveInfo[] curves = Curve.CurveEditor.GetCurves(trackBar.Track);
            if (curves != null && curves.Length > 0)
            {
                trackBar.IsEditingCurves = true;
                FolderView folder = new FolderView();
                folder.UserData = trackBar;
                folder.Foldout.Content.text = trackBar.Track.gameObject.name;
                folder.Foldout.IsOpen = true;
                folder.ContextMenu = new CurveGroupContextMenu(this);

                int keyTypeValue = 1;
                foreach (var c in curves)
                {
                    CurveTrack track = _CurveEditor.AddCurve(c.GetCurve(), c.Attribute.Color);
                    CurveTrackTreeViewItem item = new CurveTrackTreeViewItem(c.Attribute.Name, track, trackBar, (KeyType)keyTypeValue);
                    folder.Controls.Add(item);
                    _TreeView.Controls.Add(folder);
                    keyTypeValue *= 2;
                }
            }
        }

        private void Remove(FolderView group)
        {
            BaseTrackBar trackBar = group.UserData as BaseTrackBar;
            if (trackBar != null)
            {
                trackBar.IsEditingCurves = false;
                foreach (CurveTrackTreeViewItem item in group.Controls)
                    _CurveEditor.RemoveCurve(item.CurveTrack);

                trackBar.UserData = null;
                _TreeView.Controls.Remove(group);
            }
        }

        public void Remove(BaseTrackBar trackBar)
        {
            foreach (FolderView f in _TreeView.Controls)
            {
                if ((BaseTrackBar)f.UserData == trackBar)
                {
                    Remove(f);
                    break;
                }
            }
        }

        public void RemoveDestroyed()
        {
            int index = 0;
            while (index < _TreeView.Controls.Count)
            {
                FolderView fv = (FolderView)_TreeView.Controls[index];
                BaseTrackBar trackBar = (BaseTrackBar)fv.UserData;
                if (trackBar.Track.IsDestroyed)
                    Remove(fv);
                else
                    index++;
            }
        }


        public void RemoveAll(bool clearIsEditingCurves)
        {
            foreach (FolderView group in _TreeView.Controls)
            {
                BaseTrackBar trackBar = group.UserData as BaseTrackBar;
                if (trackBar != null)
                {
                    if (clearIsEditingCurves)
                        trackBar.IsEditingCurves = false;
                    foreach (CurveTrackTreeViewItem item in group.Controls)
                        _CurveEditor.RemoveCurve(item.CurveTrack);

                    trackBar.UserData = null;
                }
            }
            _TreeView.Controls.Clear();
        }
        void _CurveEditor_Changed(object sender, System.EventArgs e)
        {
            foreach (FolderView folder in _TreeView.Controls)
            {
                foreach (CurveTrackTreeViewItem item in folder.Controls)
                {
                    item.TrackBar.Invalidate();
                }
            }
        }

        class CurveTrackTreeViewItem : Grid
        {
            private Skill.Editor.UI.ToggleButton _TbVisible;
            private Skill.Framework.UI.Label _LblName;

            public CurveTrack CurveTrack { get; private set; }
            public BaseTrackBar TrackBar { get; private set; }
            public KeyType KeyType { get; private set; }

            public CurveTrackTreeViewItem(string name, CurveTrack curveTrack, BaseTrackBar trackBar, KeyType keyType)
            {
                this.CurveTrack = curveTrack;
                this.TrackBar = trackBar;
                this.KeyType = keyType;


                this.ColumnDefinitions.Add(22, GridUnitType.Pixel);
                this.ColumnDefinitions.Add(1, GridUnitType.Star);
                this.ColumnDefinitions.Add(20, GridUnitType.Pixel);

                _TbVisible = new Skill.Editor.UI.ToggleButton() { Column = 0, IsChecked = TrackBar.Visibility == Skill.Framework.UI.Visibility.Visible, Margin = new Thickness(2, 0, 0, 0) };
                _LblName = new Label() { Column = 1, Text = name };

                this.Controls.Add(_TbVisible);
                this.Controls.Add(_LblName);

                _TbVisible.Changed += _TbVisible_Changed;
            }

            public void AddKey()
            {
                TrackBar.AddCurveKey(this.KeyType);
            }

            void _TbVisible_Changed(object sender, System.EventArgs e)
            {
                CurveTrack.Visibility = _TbVisible.IsChecked ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;
            }
        }
        class CurveGroupContextMenu : Skill.Editor.UI.ContextMenu
        {
            private CurveTrackTreeView _TreeView;

            public CurveGroupContextMenu(CurveTrackTreeView treeView)
            {
                _TreeView = treeView;

                Skill.Editor.UI.MenuItem removeItem = new Skill.Editor.UI.MenuItem("Remove");
                removeItem.Click += CurveGroupRemove_Click;
                Add(removeItem);
            }

            private void CurveGroupRemove_Click(object sender, System.EventArgs e)
            {
                FolderView group = Owner as FolderView;
                _TreeView.Remove(group);
            }
        }
    }
}
