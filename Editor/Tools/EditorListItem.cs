using Skill.Editor.UI;
using Skill.Framework.UI;
using System;

namespace Skill.Editor.Tools
{
    public class EditorListItem : Grid
    {
        private Label _NameField;
        private Button _BtnRemove;
        private Button _BtnAdd;

        public IEditor Editor { get; private set; }
        public string ObjectName { get { return _NameField.Text; } set { _NameField.Text = value; } }

        public EditorListItem(IEditor editor)
        {
            this.Editor = editor;

            this._NameField = new Label() { Row = 0, Column = 0, Margin = new Thickness(2, 0) };
            this.ObjectName = "Null";

            _BtnAdd = new Button() { Row = 0, Column = 1, Margin = new Thickness(2, 4, 0, 3), Style = Resources.Styles.SmallButton };
            _BtnAdd.Content.image = Resources.UITextures.PlusNext;
            _BtnAdd.Content.tooltip = "Add next";

            _BtnRemove = new Framework.UI.Button() { Row = 0, Column = 2, Margin = new Thickness(1, 4, 2, 3), Style = Resources.Styles.SmallButton };
            _BtnRemove.Content.image = Resources.UITextures.Minus;
            _BtnRemove.Content.tooltip = "Remove this";

            this.RowDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(20, GridUnitType.Pixel);
            this.ColumnDefinitions.Add(20, GridUnitType.Pixel);

            this.Controls.Add(_NameField);
            this.Controls.Add(_BtnRemove);
            this.Controls.Add(_BtnAdd);

            this._BtnRemove.Click += _BtnRemove_Click;
            this._BtnAdd.Click += _BtnAdd_Click;
            this.Height = _NameField.LayoutHeight + _NameField.Margin.Vertical + 2;
        }

        void _BtnAdd_Click(object sender, EventArgs e)
        {
            Editor.NewAfter(this);
        }
        void _BtnRemove_Click(object sender, EventArgs e)
        {
            Editor.Remove(this);
        }
    }
}
