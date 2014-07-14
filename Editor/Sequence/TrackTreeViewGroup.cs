using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Framework.UI;
using Skill.Editor.UI;
using Skill.Framework.Sequence;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Sequence
{
    public class TrackTreeViewGroup : Skill.Editor.UI.Extended.FolderView, IProperties
    {
        public PropertiesPanel Properties { get; private set; }
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
            Foldout.Content.image = Resources.UITextures.Matinee.Folder;
            Foldout.IsOpen = group.IsOpen;
            Properties = new TrackTreeViewGroupProperties(this);

            Foldout.StateChanged += Foldout_StateChanged;
        }

        void Foldout_StateChanged(object sender, System.EventArgs e)
        {
            Group.IsOpen = Foldout.IsOpen;
            MatineeEditorWindow.Instance.InvalidateTimeLineView();
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


    class TrackTreeViewGroupProperties : PropertiesPanel
    {
        private TrackTreeViewGroup _Group;
        private Skill.Editor.UI.TextField _TxtName;

        public TrackTreeViewGroupProperties(TrackTreeViewGroup group)
            : base(group)
        {
            _Group = group;

            _TxtName = new Skill.Editor.UI.TextField() { Margin = ControlMargin };
            _TxtName.Label.text = "Name";
            _TxtName.TextChanged += _TxtName_TextChanged;

            Controls.Add(_TxtName);
            _TxtName.TextChanged += _TxtName_TextChanged;
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