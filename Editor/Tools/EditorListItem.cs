using Skill.Editor.UI;
using Skill.Framework.UI;
using System;

namespace Skill.Editor.Tools
{
    public class EditorListItem : EditorControl
    {
        private Grid _Panel;
        private Label _NameField;
        private Skill.Editor.UI.Button _BtnRemove;
        private Skill.Editor.UI.Button _BtnAdd;

        public IEditor Editor { get; private set; }
        public string ObjectName { get { return _NameField.Text; } set { _NameField.Text = value; } }

        public override float LayoutHeight
        {
            get
            {
                if (Visibility != Skill.Framework.UI.Visibility.Collapsed)
                    return _NameField.LayoutHeight + _NameField.Margin.Vertical + 2;
                return base.LayoutHeight;
            }
        }

        public EditorListItem(IEditor editor)
        {
            this.Editor = editor;

            this._NameField = new Label() { Row = 0, Column = 0, Margin = new Thickness(2, 0) };
            this.ObjectName = "Null";

            _BtnAdd = new UI.Button() { Row = 0, Column = 1, Margin = new Thickness(2, 4, 0, 3), Style = Resources.Styles.SmallButton };
            _BtnAdd.Content.image = Resources.Textures.PlusNext;
            _BtnAdd.Content.tooltip = "Add next";

            _BtnRemove = new UI.Button() { Row = 0, Column = 2, Margin = new Thickness(1, 4, 2, 3), Style = Resources.Styles.SmallButton };
            _BtnRemove.Content.image = Resources.Textures.Minus;
            _BtnRemove.Content.tooltip = "Remove this";

            this._Panel = new Grid() { Parent = this };
            this._Panel.RowDefinitions.Add(1, GridUnitType.Star);
            this._Panel.ColumnDefinitions.Add(1, GridUnitType.Star);
            this._Panel.ColumnDefinitions.Add(20, GridUnitType.Pixel);
            this._Panel.ColumnDefinitions.Add(20, GridUnitType.Pixel);

            this._Panel.Controls.Add(_NameField);
            this._Panel.Controls.Add(_BtnRemove);
            this._Panel.Controls.Add(_BtnAdd);

            this._Panel.LayoutChanged += Panel_LayoutChanged;

            this._BtnRemove.Click += _BtnRemove_Click;
            this._BtnAdd.Click += _BtnAdd_Click;
        }

        void _BtnAdd_Click(object sender, EventArgs e)
        {
            Editor.NewAfter(this);
        }
        void _BtnRemove_Click(object sender, EventArgs e)
        {
            Editor.Remove(this);
        }

        protected override void OnRenderAreaChanged()
        {
            this._Panel.RenderArea = RenderArea;
        }

        private void Panel_LayoutChanged(object sender, EventArgs e)
        {
            OnLayoutChanged();
        }

        protected override void Render()
        {
            this._Panel.OnGUI();
        }
    }
}
