using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using System.Collections.Generic;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public class TrackTreeView : Skill.Framework.UI.Grid
    {
        private TreeView _TreeView;
        private MatineeEditor _Editor;
        private TrackGroupContextMenu _TrackGroupContextMenu;
        private TrackItemContextMenu _TrackItemContextMenu;
        private Skill.Framework.UI.Label _Title;
        internal TrackTreeView(MatineeEditor editor)
        {
            this._Editor = editor;
            RowDefinitions.Add(18, Skill.Framework.UI.GridUnitType.Pixel);
            RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            // create header
            _Title = new Skill.Framework.UI.Label() { Row = 0 };
            _Title.Text = "Tracks";

            Controls.Add(_Title);

            _TreeView = new TreeView() { Row = 1, UserData = this };
            Controls.Add(_TreeView);

            // create context menu
            this._TreeView.ContextMenu = new TrackTreeViewContextMenu(this);
            this._TrackGroupContextMenu = new TrackGroupContextMenu(this);
            this._TrackItemContextMenu = new TrackItemContextMenu(this);

            _TreeView.SelectedItemChanged += _TreeView_SelectedItemChanged;
        }

        void _TreeView_SelectedItemChanged(object sender, System.EventArgs e)
        {
            if (_TreeView.SelectedItem is IProperties)
                _Editor.PropertyGrid.SelectedObject = (IProperties)_TreeView.SelectedItem;
        }
        protected override void Render()
        {
            if (_Title.Style == null)
                _Title.Style = Skill.Editor.Resources.Styles.Header;
            if (_TreeView.Background.Style == null)
                _TreeView.Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;

            base.Render();
        }

        // Remove all items
        public void Clear()
        {
            _TreeView.Controls.Clear();
        }

        internal void Refresh()
        {
            if (_Editor.Matinee != null)
                Refresh(_Editor.Matinee.transform, _TreeView);
        }

        private void Refresh(Transform transform, Skill.Framework.UI.Panel panel)
        {
            List<TrackGroup> groupList = new List<TrackGroup>();
            List<Track> trackList = new List<Track>();

            int childCount = transform.childCount;
            // first check for any new group or track
            for (int childIndex = 0; childIndex < childCount; childIndex++)
            {
                Transform childT = transform.GetChild(childIndex);
                TrackGroup group = childT.GetComponent<TrackGroup>();
                if (group != null)
                {
                    groupList.Add(group);

                    TrackTreeViewGroup groupItem = null;
                    foreach (var item in panel.Controls)
                    {
                        if (item is TrackTreeViewGroup)
                        {
                            if (((TrackTreeViewGroup)item).Group == group)
                            {
                                groupItem = (TrackTreeViewGroup)item;
                                break;
                            }
                        }
                    }

                    if (groupItem == null)
                    {
                        groupItem = new TrackTreeViewGroup(group);
                        groupItem.Foldout.Content.text = groupItem.Group.gameObject.name;
                        groupItem.ContextMenu = _TrackGroupContextMenu;
                        panel.Controls.Add(groupItem);
                    }
                    else
                        groupItem.Refresh();
                    Refresh(groupItem.Group.transform, groupItem);

                    continue;
                }

                // if group not exist look for track
                Track track = childT.GetComponent<Track>();
                if (track != null)
                {
                    trackList.Add(track);
                    TrackTreeViewItem trackItem = null;
                    foreach (var item in panel.Controls)
                    {
                        if (item is TrackTreeViewItem)
                        {
                            if (((TrackTreeViewItem)item).Track == track)
                            {
                                trackItem = (TrackTreeViewItem)item;
                                break;
                            }
                        }
                    }

                    if (trackItem == null)
                    {
                        BaseTrackBar bar = CreateNewTrackBar(track);
                        _Editor.TimeLine.TrackView.Controls.Add(bar);
                        trackItem = new TrackTreeViewItem(track, bar);
                        trackItem.ContextMenu = _TrackItemContextMenu;
                        panel.Controls.Add(trackItem);
                    }
                }
            }

            // now check for any deleted group or track
            int index = 0;
            while (index < panel.Controls.Count)
            {
                var item = panel.Controls[index];
                if (item is TrackTreeViewGroup)
                {
                    TrackGroup group = ((TrackTreeViewGroup)item).Group;
                    if (!groupList.Contains(group))
                    {
                        RemoveTracks((TrackTreeViewGroup)item);
                        panel.Controls.Remove(item);
                        continue;
                    }
                }
                else if (item is TrackTreeViewItem)
                {
                    Track track = ((TrackTreeViewItem)item).Track;
                    if (!trackList.Contains(track))
                    {
                        panel.Controls.Remove(item);
                        _Editor.TimeLine.TrackView.Controls.Remove(((TrackTreeViewItem)item).TrackBar);
                        continue;
                    }
                }

                index++;
            }
        }

        private void CreateGroup(TrackTreeViewGroup parent)
        {
            if (_Editor.Matinee != null)
            {
                GameObject newObj = new GameObject("NewGroup");
                TrackTreeViewGroup newG = new TrackTreeViewGroup(newObj.AddComponent<TrackGroup>());
                newG.Foldout.Content.text = newG.Group.gameObject.name;
                newG.Group.Visible = true;
                newG.ContextMenu = _TrackGroupContextMenu;

                if (parent != null)
                {
                    newObj.transform.parent = parent.Group.transform;
                    parent.Controls.Add(newG);
                    parent.Foldout.IsOpen = true;
                }
                else
                {
                    newObj.transform.parent = _Editor.Matinee.transform;
                    _TreeView.Controls.Add(newG);
                }
                newObj.transform.localPosition = Vector3.zero;
                newObj.transform.localRotation = Quaternion.identity;
            }
        }
        private void DeleteGroup(TrackTreeViewGroup group)
        {
            if (_Editor.Matinee != null)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Delete TrackGroup", "Are you sure you want to delete group : " + group.Group.gameObject.name, "Yes", "No"))
                {
                    ((Skill.Framework.UI.Panel)group.Parent).Controls.Remove(group);
                    RemoveTracks(group);
                    Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(group);
                    GameObject.DestroyImmediate(group.Group.gameObject);
                    _Editor.TrackChanged();
                }
            }
        }

        private void RemoveTracks(TrackTreeViewGroup group)
        {
            foreach (var item in group.Controls)
            {
                if (item is TrackTreeViewItem)
                    _Editor.TimeLine.TrackView.Controls.Remove(((TrackTreeViewItem)item).TrackBar);
                else if (item is TrackTreeViewGroup)
                    RemoveTracks((TrackTreeViewGroup)item);
            }
        }
        private void CreateTrack(TrackTreeViewGroup parent, TrackType type)
        {
            if (_Editor.Matinee != null)
            {
                GameObject newObj = new GameObject(string.Format("New{0}Track", type.ToString()));
                Track newTrack = CreateNewTrack(newObj, type);
                BaseTrackBar bar = CreateNewTrackBar(newTrack);

                _Editor.TimeLine.TrackView.Controls.Add(bar);
                TrackTreeViewItem newItem = new TrackTreeViewItem(newTrack, bar);
                newItem.ContextMenu = _TrackItemContextMenu;

                newObj.transform.parent = parent.Group.transform;
                parent.Controls.Add(newItem);
                parent.Foldout.IsOpen = true;

                newObj.transform.localPosition = Vector3.zero;
                newObj.transform.localRotation = Quaternion.identity;

                _Editor.TrackChanged();
            }
        }
        private Track CreateNewTrack(GameObject obj, TrackType type)
        {
            switch (type)
            {
                case TrackType.Event:
                    return obj.AddComponent<EventTrack>();
                case TrackType.Bool:
                    return obj.AddComponent<BooleanTrack>();
                case TrackType.Float:
                    return obj.AddComponent<FloatTrack>();
                case TrackType.Integer:
                    return obj.AddComponent<IntegerTrack>();
                case TrackType.Color:
                    return obj.AddComponent<ColorTrack>();
                case TrackType.Vector2:
                    return obj.AddComponent<Vector2Track>();
                case TrackType.Vector3:
                    return obj.AddComponent<Vector3Track>();
                case TrackType.Vector4:
                    return obj.AddComponent<Vector4Track>();
                case TrackType.Quaternion:
                    return obj.AddComponent<QuaternionTrack>();
                case TrackType.Sound:
                    return obj.AddComponent<SoundTrack>();
                default:
                    return null;
            }
        }
        private BaseTrackBar CreateNewTrackBar(Track track)
        {
            switch (track.Type)
            {
                case TrackType.Event:
                    return new EventTrackBar((EventTrack)track);
                case TrackType.Bool:
                    return new BooleanTrackBar((BooleanTrack)track);
                case TrackType.Float:
                    return new FloatTrackBar((FloatTrack)track);
                case TrackType.Integer:
                    return new IntegerTrackBar((IntegerTrack)track);
                case TrackType.Color:
                    return new ColorTrackBar((ColorTrack)track);
                case TrackType.Vector2:
                    return new Vector2TrackBar((Vector2Track)track);
                case TrackType.Vector3:
                    return new Vector3TrackBar((Vector3Track)track);
                case TrackType.Vector4:
                    return new Vector4TrackBar((Vector4Track)track);
                case TrackType.Quaternion:
                    return new QuaternionTrackBar((QuaternionTrack)track);
                case TrackType.Sound:
                    return new SoundTrackBar((SoundTrack)track);
                default:
                    return null;
            }
        }
        private void DeleteTrack(TrackTreeViewItem track)
        {
            if (_Editor.Matinee != null)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Delete Track", "Are you sure you want to delete track : " + track.Track.gameObject.name, "Yes", "No"))
                {
                    ((Skill.Framework.UI.Panel)track.Parent).Controls.Remove(track);
                    Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(track);
                    if (track.TrackBar != null) _Editor.TimeLine.TrackView.Controls.Remove(track.TrackBar);
                    GameObject.DestroyImmediate(track.Track.gameObject);
                    _Editor.TrackChanged();
                }
            }
        }



        #region ContextMenu class
        class TrackTreeViewContextMenu : Skill.Editor.UI.ContextMenu
        {
            private TrackTreeView _View;
            public TrackTreeViewContextMenu(TrackTreeView view)
            {
                _View = view;
                Skill.Editor.UI.MenuItem addItem = new Skill.Editor.UI.MenuItem("Add New Group");
                addItem.Click += AddItem_Click;
                Add(addItem);
            }
            private void AddItem_Click(object sender, System.EventArgs e)
            {
                _View.CreateGroup(null);
            }
        }

        class TrackGroupContextMenu : Skill.Editor.UI.ContextMenu
        {
            private TrackTreeView _View;

            public TrackGroupContextMenu(TrackTreeView view)
            {
                _View = view;

                Skill.Editor.UI.MenuItem newTrack = new Skill.Editor.UI.MenuItem("New Track");
                var names = System.Enum.GetNames(typeof(TrackType));
                var values = System.Enum.GetValues(typeof(TrackType));
                int index = 0;
                foreach (var value in values)
                {
                    Skill.Editor.UI.MenuItem item = new Skill.Editor.UI.MenuItem(names[index]) { UserData = value };
                    item.Click += AddTrack_Click;
                    newTrack.Add(item);
                    index++;
                }


                Skill.Editor.UI.MenuItem addGroup = new Skill.Editor.UI.MenuItem("Add New Group");
                addGroup.Click += TrackGroupAdd_Click;

                Skill.Editor.UI.MenuItem deleteItem = new Skill.Editor.UI.MenuItem("Delete");
                deleteItem.Click += TrackGroupDelete_Click;

                Skill.Editor.UI.MenuItem propertiesItem = new Skill.Editor.UI.MenuItem("Properties");
                propertiesItem.Click += TrackGroupProperties_Click;

                Add(newTrack);
                AddSeparator();
                Add(addGroup);
                Add(deleteItem);
                AddSeparator();
                Add(propertiesItem);
            }

            void AddTrack_Click(object sender, System.EventArgs e)
            {
                TrackTreeViewGroup group = Owner as TrackTreeViewGroup;
                _View.CreateTrack(group, (TrackType)((Skill.Editor.UI.MenuItem)sender).UserData);
            }

            private void TrackGroupAdd_Click(object sender, System.EventArgs e)
            {
                TrackTreeViewGroup group = Owner as TrackTreeViewGroup;
                _View.CreateGroup(group);
            }
            private void TrackGroupDelete_Click(object sender, System.EventArgs e)
            {
                TrackTreeViewGroup group = Owner as TrackTreeViewGroup;
                _View.DeleteGroup(group);
            }
            private void TrackGroupProperties_Click(object sender, System.EventArgs e)
            {
                TrackTreeViewGroup group = Owner as TrackTreeViewGroup;
                _View._Editor.PropertyGrid.SelectedObject = group;
            }
        }

        class TrackItemContextMenu : Skill.Editor.UI.ContextMenu
        {
            private TrackTreeView _View;

            public TrackItemContextMenu(TrackTreeView view)
            {
                _View = view;

                Skill.Editor.UI.MenuItem deleteItem = new Skill.Editor.UI.MenuItem("Delete");
                deleteItem.Click += TrackGroupDelete_Click;

                Skill.Editor.UI.MenuItem propertiesItem = new Skill.Editor.UI.MenuItem("Properties");
                propertiesItem.Click += TrackGroupProperties_Click;

                Add(deleteItem);
                AddSeparator();
                Add(propertiesItem);
            }

            private void TrackGroupDelete_Click(object sender, System.EventArgs e)
            {
                TrackTreeViewItem track = Owner as TrackTreeViewItem;
                _View.DeleteTrack(track);
            }
            private void TrackGroupProperties_Click(object sender, System.EventArgs e)
            {
                TrackTreeViewItem track = Owner as TrackTreeViewItem;
                _View._Editor.PropertyGrid.SelectedObject = track;
            }
        }
        #endregion
    }
}