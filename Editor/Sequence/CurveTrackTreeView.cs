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
        private Skill.Framework.UI.Label _Title;
        internal CurveTrackTreeView(CurveEditor curveEditor)
        {
            this._CurveEditor = curveEditor;
            RowDefinitions.Add(18, Skill.Framework.UI.GridUnitType.Pixel);
            RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            // create header
            _Title = new Skill.Framework.UI.Label() { Row = 0 };
            _Title.Text = "Curves";

            Controls.Add(_Title);

            _TreeView = new TreeView() { Row = 1, UserData = this, HandleScrollWheel = true };
            Controls.Add(_TreeView);

            // create context menu            
            //this._TrackGroupContextMenu = new TrackGroupContextMenu(this);
            //this._TrackItemContextMenu = new TrackItemContextMenu(this);            

            _CurveEditor.Changed += _CurveEditor_Changed;
        }

        protected override void Render()
        {
            if (_Title.Style == null)
                _Title.Style = Skill.Editor.Resources.Styles.Header;
            if (_TreeView.Background.Style == null)
                _TreeView.Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;

            // update name if changed by user
            foreach (FolderView folder in _TreeView.Controls)
                folder.Foldout.Content.text = ((BaseTrackBar)folder.UserData).Track.gameObject.name;

            base.Render();
        }

        public void Add(BaseTrackBar trackBar, Framework.Sequence.ITrackKey key)
        {
            // avoid edit same trackBar twice
            foreach (FolderView folder in _TreeView.Controls)
            {
                if ((BaseTrackBar)folder.UserData == trackBar)
                    return;
            }

            Curve.CurveEditor.EditCurveInfo[] curves = Curve.CurveEditor.GetCurves(key);
            if (curves != null && curves.Length > 0)
            {
                trackBar.IsEditingCurves = true;
                FolderView folder = new FolderView();
                folder.UserData = trackBar;
                folder.Foldout.Content.text = trackBar.Track.gameObject.name;
                folder.Foldout.IsOpen = true;
                folder.ContextMenu = new CurveGroupContextMenu(this);

                int record = 1;
                foreach (var c in curves)
                {
                    CurveTrack track = _CurveEditor.AddCurve(c.GetCurve(), c.Attribute.Color);
                    CurveTrackTreeViewItem item = new CurveTrackTreeViewItem(c.Attribute.Name, track, trackBar, (RecordState)record);
                    folder.Controls.Add(item);
                    _TreeView.Controls.Add(folder);
                    record *= 2;
                }
            }
        }

        private void Remove(FolderView group)
        {
            BaseTrackBar trackBar = group.UserData as BaseTrackBar;
            if (trackBar != null)
            {
                trackBar.IsEditingCurves = false;
                trackBar.RecordState = RecordState.None;
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

        public void RemoveAll(bool clearIsEditingCurves)
        {
            foreach (FolderView group in _TreeView.Controls)
            {
                BaseTrackBar trackBar = group.UserData as BaseTrackBar;
                if (trackBar != null)
                {
                    if (clearIsEditingCurves)
                        trackBar.IsEditingCurves = false;
                    trackBar.RecordState = RecordState.None;
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

            private static UnityEngine.GUIStyle _RecordStyle;
            private static UnityEngine.GUIStyle RecordStyle
            {
                get
                {
                    if (_RecordStyle == null)
                    {
                        _RecordStyle = new UnityEngine.GUIStyle();
                        _RecordStyle.normal.background = Skill.Editor.Resources.Textures.RecordOff;
                        _RecordStyle.onNormal.background = Skill.Editor.Resources.Textures.RecordOn;
                    }
                    return _RecordStyle;
                }
            }

            private Skill.Editor.UI.ToggleButton _TbVisible;
            private Skill.Editor.UI.ToggleButton _TbRecord;
            private Skill.Editor.UI.Button _BtnKey;
            private Skill.Framework.UI.Label _LblName;

            public CurveTrack CurveTrack { get; private set; }
            public BaseTrackBar TrackBar { get; private set; }

            public RecordState RecordState { get; private set; }

            public CurveTrackTreeViewItem(string name, CurveTrack curveTrack, BaseTrackBar trackBar, RecordState recordState)
            {
                this.CurveTrack = curveTrack;
                this.TrackBar = trackBar;
                this.RecordState = recordState;


                this.ColumnDefinitions.Add(22, GridUnitType.Pixel);
                this.ColumnDefinitions.Add(1, GridUnitType.Star);
                this.ColumnDefinitions.Add(18, GridUnitType.Pixel);
                this.ColumnDefinitions.Add(20, GridUnitType.Pixel);

                _TbVisible = new Skill.Editor.UI.ToggleButton() { Column = 0, IsChecked = TrackBar.Visibility == Skill.Framework.UI.Visibility.Visible, Margin = new Thickness(2, 0, 0, 0) };
                _LblName = new Label() { Column = 1, Text = name };
                _TbRecord = new UI.ToggleButton() { Column = 2, IsChecked = (TrackBar.RecordState & RecordState) == RecordState, Style = RecordStyle, Margin = new Thickness(0, 0,2,0) };
                _BtnKey = new UI.Button() { Column = 3, Margin = new Thickness(0, 0, 2, 0) };
                _BtnKey.Content.tooltip = "Add Key";

                this.Controls.Add(_TbVisible);
                this.Controls.Add(_LblName);
                this.Controls.Add(_TbRecord);
                this.Controls.Add(_BtnKey);

                _TbVisible.Changed += _TbVisible_Changed;
                _TbRecord.Changed += _TbRecord_Changed;
                _BtnKey.Click += _BtnKey_Click;
            }

            protected override void Render()
            {
                if (_BtnKey.Style == null)
                {
                    _BtnKey.Style = Skill.Editor.Resources.Styles.SmallButton;
                    _BtnKey.Content.image = Skill.Editor.Resources.Textures.Key;                    
                }
                base.Render();
            }

            void _BtnKey_Click(object sender, EventArgs e)
            {
                TrackBar.AddKey(this.RecordState);
            }
            void _TbRecord_Changed(object sender, System.EventArgs e)
            {
                if (_TbRecord.IsChecked)
                    TrackBar.RecordState |= this.RecordState;
                else
                    TrackBar.RecordState &= ~this.RecordState;
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
