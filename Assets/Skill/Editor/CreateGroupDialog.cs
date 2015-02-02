using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor
{
    class CreateGroupDialog : UnityEditor.EditorWindow
    {
        private static CreateGroupDialog _Instance;
        public static CreateGroupDialog Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = ScriptableObject.CreateInstance<CreateGroupDialog>();
                }
                return _Instance;
            }
        }


        private static Vector2 Size = new Vector2(300, 48);

        private Skill.Editor.UI.EditorFrame _Frame;
        private Skill.Editor.UI.TextField _TxtGroupName;
        private Skill.Framework.UI.Button _BtnCreate;
        private Skill.Framework.UI.Button _BtnCancel;


        public void OnGUI()
        {
            Event e = Event.current;
            if (e != null)
            {
                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.Return)
                        _BtnCreate_Click(_BtnCreate, EventArgs.Empty);
                }
            }
            _Frame.OnGUI();
        }

        public CreateGroupDialog()
        {
            hideFlags = HideFlags.DontSave;

            title = "Create Group";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = new Vector2(Size.x, Size.y);
            base.maxSize = new Vector2(Size.x + 200, Size.y);

            CreateUI();
        }

        private void CreateUI()
        {
            _Frame = new UI.EditorFrame("Frame", this);

            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(1, Skill.Framework.UI.GridUnitType.Star) });
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(24, Skill.Framework.UI.GridUnitType.Pixel) });

            float offset = 5;
            _Frame.Grid.ColumnDefinitions.Add(offset, Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);
            _Frame.Grid.ColumnDefinitions.Add(offset, Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);
            _Frame.Grid.ColumnDefinitions.Add(offset, Framework.UI.GridUnitType.Pixel);

            _TxtGroupName = new UI.TextField() { Row = 0, Column = 1, ColumnSpan = 3, Margin = new Framework.UI.Thickness(4) }; _TxtGroupName.Label.text = "Group Name";
            _TxtGroupName.Text = "Group";

            _BtnCreate = new Framework.UI.Button() { Row = 1, Column = 1, Margin = new Framework.UI.Thickness(4, 0, 4, 4) };
            _BtnCreate.Content.text = "Create";
            _BtnCreate.Click += _BtnCreate_Click;

            _BtnCancel = new Framework.UI.Button() { Row = 1, Column = 3, Margin = new Framework.UI.Thickness(4, 0, 4, 4) };
            _BtnCancel.Content.text = "Cancel";
            _BtnCancel.Click += _BtnCancel_Click;

            _Frame.Grid.Controls.Add(_TxtGroupName);
            _Frame.Grid.Controls.Add(_BtnCreate);
            _Frame.Grid.Controls.Add(_BtnCancel);
        }

        void _BtnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        void _BtnCreate_Click(object sender, EventArgs e)
        {
            string groupName = _TxtGroupName.Text;
            if (string.IsNullOrEmpty(groupName))
                groupName = "Group";

            Skill.Editor.Commands.GroupObjects(groupName);
            base.Close();
        }

        void OnDestroy()
        {
            _Instance = null;
        }

        void OnEnable()
        {
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
        }
    }
}
