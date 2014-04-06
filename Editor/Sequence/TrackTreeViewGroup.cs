using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Framework.UI;
using Skill.Editor.UI;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public class TrackTreeViewGroup : Skill.Editor.UI.Extended.FolderView, IProperties
    {
        public Properties Properties { get; private set; }
        public bool IsSelectedProperties { get; set; }
        public string Title { get { return "Track Group"; } }
        public TrackGroup Group { get; private set; }

        public Matinee Matinee
        {
            get
            {
                Transform parent = Group.transform;
                while (parent != null)
                {
                    Matinee m = parent.GetComponent<Matinee>();
                    if (m != null) return m;
                    parent = parent.parent;
                }
                return null;
            }
        }

        public TrackTreeViewGroup(TrackGroup group)
        {
            if (group == null)
                throw new System.ArgumentNullException("Invalid TrackGroup");
            this.Height = 20;
            this.Group = group;
            Foldout.Content.image = MatineeResources.Textures.Folder;
            Foldout.IsOpen = group.IsOpen;
            Properties = new TrackTreeViewGroupProperties(this);

            Foldout.StateChanged += Foldout_StateChanged;
        }

        void Foldout_StateChanged(object sender, System.EventArgs e)
        {
            Group.IsOpen = Foldout.IsOpen;
            EditorUtility.SetDirty(Group);
        }

        public void Refresh()
        {
            foreach (var item in Controls)
            {
                if (item is TrackTreeViewGroup) ((TrackTreeViewGroup)item).Refresh();
                else if (item is TrackTreeViewItem) ((TrackTreeViewItem)item).Refresh();
            }
        }



    }


    class TrackTreeViewGroupProperties : Properties
    {
        private TrackTreeViewGroup _Group;
        private Skill.Editor.UI.TextField _TxtName;
        private Skill.Editor.UI.ToggleButton _TBVisible;

        public TrackTreeViewGroupProperties(TrackTreeViewGroup group)
            : base(group)
        {
            _Group = group;

            _TxtName = new Skill.Editor.UI.TextField() { Margin = new Thickness(2) };
            _TxtName.Label.text = "Name";
            _TxtName.TextChanged += _TxtName_TextChanged;

            _TBVisible = new Skill.Editor.UI.ToggleButton() { Margin = new Thickness(2, 2) };
            _TBVisible.Label.text = "Visible";

            Controls.Add(_TxtName);
            Controls.Add(_TBVisible);

            _TxtName.TextChanged += _TxtName_TextChanged;
            _TBVisible.Changed += _TBVisible_Changed;

            Refresh();
        }

        void _TxtName_TextChanged(object sender, System.EventArgs e)
        {
            if (IgnoreChanges) return;
            if (string.IsNullOrEmpty(_TxtName.Text))
                _TxtName.Text = _Group.Foldout.Content.text;
            _Group.Foldout.Content.text = _TxtName.Text;
            _Group.Group.gameObject.name = _TxtName.Text;
        }

        void _TBVisible_Changed(object sender, System.EventArgs e)
        {
            if (IgnoreChanges) return;
            _Group.Group.Visible = _TBVisible.IsChecked;
            _TBVisible.IsChecked = _Group.Group.Visible;
            SetDirty();
        }

        protected override void RefreshData()
        {
            _Group.Foldout.Content.text = _TxtName.Text = _Group.Group.gameObject.name;
        }

        protected override void SetDirty()
        {
            UnityEditor.EditorUtility.SetDirty(_Group.Group);
        }
    }

}