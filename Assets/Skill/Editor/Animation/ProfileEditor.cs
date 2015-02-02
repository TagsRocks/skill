using UnityEngine;
using System.Collections;
using System.Linq;
using Skill.Framework.UI;
using Skill.Framework.UI.Extended;

namespace Skill.Editor.Animation
{
    public class ProfileEditor : Grid
    {

        private Label _ProfileLabel;
        private ListBox _ProfileList;
        private UniformGrid _ProfileToolbar;
        private Button _BtnAddProfile;
        private Button _BtnRemoveProfile;
        private AnimationTreeEditorWindow _Editor;

        public event System.EventHandler SelectedProfileChanged;

        private void OnSelectedProfileChanged()
        {
            if (SelectedProfileChanged != null)
                SelectedProfileChanged(this, System.EventArgs.Empty);
        }
        public AnimationTreeProfileData SelectedProfile
        {
            get
            {
                if (_ProfileList.SelectedItem != null)
                    return ((ProfileItem)_ProfileList.SelectedItem).Data;
                else
                    return null;
            }
        }
        public ProfileEditor(AnimationTreeEditorWindow editor)
        {
            this._Editor = editor;

            this.RowDefinitions.Add(20, GridUnitType.Pixel);
            this.RowDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(60, GridUnitType.Pixel);

            _ProfileLabel = new Label() { Row = 0, Column = 0, Text = "Profiles" };
            this.Controls.Add(_ProfileLabel);

            _ProfileList = new ListBox() { Row = 1, Column = 0, ColumnSpan = 2 };
            _ProfileList.DisableFocusable();
            this.Controls.Add(_ProfileList);

            _ProfileToolbar = new UniformGrid() { Row = 0, Column = 1, Rows = 1, Columns = 2 };
            this.Controls.Add(_ProfileToolbar);

            _BtnAddProfile = new Button() { Column = 0 };
            _BtnAddProfile.Content.tooltip = "add new profile";
            _ProfileToolbar.Controls.Add(_BtnAddProfile);

            _BtnRemoveProfile = new Button() { Column = 1 };
            _BtnRemoveProfile.Content.tooltip = "remove selected profile";
            _ProfileToolbar.Controls.Add(_BtnRemoveProfile);

            SetButtonsEnable();

            _ProfileList.SelectionChanged += _ProfileList_SelectionChanged;
            _BtnAddProfile.Click += _BtnAddProfile_Click;
            _BtnRemoveProfile.Click += _BtnRemoveProfile_Click;
        }

        void _BtnRemoveProfile_Click(object sender, System.EventArgs e)
        {
            RemoveSelectedProfile();
        }
        void _BtnAddProfile_Click(object sender, System.EventArgs e)
        {
            AddNewProfile();
        }
        void _ProfileList_SelectionChanged(object sender, System.EventArgs e)
        {
            OnSelectedProfileChanged();
            SetButtonsEnable();
            Skill.Editor.UI.Extended.InspectorProperties.Select((ProfileItem)_ProfileList.SelectedItem);
            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
        }
        private void SetButtonsEnable()
        {
            _BtnAddProfile.IsEnabled = true;
            _BtnRemoveProfile.IsEnabled = _ProfileList.SelectedItem != null;
        }
        internal void Clear()
        {
            _ProfileList.Items.Clear();
        }
        public void RefreshStyles()
        {
            _ProfileLabel.Style = Skill.Editor.Resources.Styles.Header;

            _BtnAddProfile.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnRemoveProfile.Style = Skill.Editor.Resources.Styles.ToolbarButton;

            _BtnAddProfile.Content.image = Skill.Editor.Resources.UITextures.Add;
            _BtnRemoveProfile.Content.image = Skill.Editor.Resources.UITextures.Remove;
            _ProfileList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
        }


        public void Rebuild()
        {

            if (_Editor.Tree.Profiles == null)
                _Editor.Tree.Profiles = new AnimationTreeProfileData[0];

            _ProfileList.Items.Clear();
            foreach (var item in _Editor.Tree.Profiles)
                Add(item);

        }
        private void Add(AnimationTreeProfileData data)
        {
            ProfileItem item = new ProfileItem(this, data);
            _ProfileList.Items.Add(item);
        }

        private bool IsProfileExists(AnimationTreeProfileData data)
        {
            foreach (ProfileItem item in _ProfileList.Items)
            {
                if (item.Data == data)
                    return true;
            }
            return false;
        }

        private void RemoveSelectedProfile()
        {
            if (_ProfileList.SelectedItem != null)
            {
                AnimationTreeProfileData data = ((ProfileItem)_ProfileList.SelectedItem).Data;
                AnimationTreeProfileData[] preProfiles = _Editor.Tree.Profiles;
                AnimationTreeProfileData[] newProfiles = new AnimationTreeProfileData[preProfiles.Length - 1];

                int preIndex = 0;
                int newIndex = 0;
                while (newIndex < newProfiles.Length && preIndex < preProfiles.Length)
                {
                    if (preProfiles[preIndex] == data)
                    {
                        preIndex++;
                        continue;
                    }
                    newProfiles[newIndex] = preProfiles[preIndex];
                    newIndex++;
                    preIndex++;
                }
                _Editor.Tree.Profiles = newProfiles;
                _ProfileList.Items.Remove(_ProfileList.SelectedItem);
                _ProfileList.SelectedIndex = 0;
                SetButtonsEnable();
            }
            else
            {
                Debug.LogError("there is no selected profile to remove");
            }
        }
        private void AddNewProfile()
        {
            AnimationTreeProfileData profile = new AnimationTreeProfileData();
            profile.Name = GetUniqueProfileName("NewProfile");
            AnimationTreeProfileData[] preProfiles = _Editor.Tree.Profiles;
            AnimationTreeProfileData[] newProfiles = new AnimationTreeProfileData[preProfiles.Length + 1];
            preProfiles.CopyTo(newProfiles, 0);
            newProfiles[newProfiles.Length - 1] = profile;
            _Editor.Tree.Profiles = newProfiles;
            Add(profile);
            SetButtonsEnable();
        }

        private string GetUniqueProfileName(string name)
        {
            int i = 1;
            string newName = name;
            while (_Editor.Tree.Profiles.Where(b => b.Name == newName).Count() > 0)
                newName = name + i++;
            return newName;
        }

        public void DeselectInspector()
        {
            if (Skill.Editor.UI.Extended.InspectorProperties.GetSelected() is ProfileItem)
                Skill.Editor.UI.Extended.InspectorProperties.Select(null);
        }

        public void RefreshContents()
        {
            foreach (ProfileItem item in _ProfileList.Items)
            {
                item.RefreshContent();
            }
        }

        private class ProfileItem : Label, Skill.Editor.UI.Extended.IProperties
        {
            //private ProfileEditor _Editor;
            public AnimationTreeProfileData Data { get; private set; }
            public ProfileItem(ProfileEditor editor, AnimationTreeProfileData data)
            {
                //this._Editor = editor;
                this.Data = data;
                this.Text = data.Name;
            }

            [Skill.Framework.ExposeProperty(1, "Name", "name of profile")]
            public string Name2
            {
                get
                {
                    return Data.Name;
                }
                set
                {
                    Data.Name = value;
                    this.Text = value;
                }
            }

            [Skill.Framework.ExposeProperty(2, "Format", "format of profile")]
            public string Format
            {
                get
                {
                    return Data.Format;
                }
                set
                {
                    Data.Format = value;
                }
            }

            public bool IsSelectedProperties { get; set; }
            private ProfileItemProperties _Properties;
            public Skill.Editor.UI.Extended.PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new ProfileItemProperties(this);
                    return _Properties;
                }
            }
            public string Title { get { return "AnimationTreeProfile"; } }

            class ProfileItemProperties : Skill.Editor.UI.Extended.ExposeProperties
            {
                private ProfileItem _Item;
                public ProfileItemProperties(ProfileItem item)
                    : base(item)
                {
                    _Item = item;
                }

                protected override void SetDirty()
                {
                    Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(_Item);
                }
            }

            internal void RefreshContent()
            {
                this.Text = Data.Name;
            }
        }
    }
}
