using Skill.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor.UI
{
    class PasteTextField : Grid
    {
        private static UnityEngine.GUIStyle _ButtonStyle;
        private Skill.Editor.UI.TextField _TextField;
        private Skill.Framework.UI.Button _BtnPaste;
        private Skill.Framework.UI.Button _BtnConvertToPersian;

        public Skill.Editor.UI.TextField TextField { get { return _TextField; } }

        public PasteTextField(bool persian = false)
        {
            if (_ButtonStyle == null)
                _ButtonStyle = new UnityEngine.GUIStyle();

            this.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(16, GridUnitType.Pixel);

            this.RowDefinitions.Add(16, GridUnitType.Pixel);
            this.RowDefinitions.Add(16, GridUnitType.Pixel);
            this.RowDefinitions.Add(1, GridUnitType.Star);

            _TextField = new Editor.UI.TextField() { Row = 0, RowSpan = 3, Column = 0, Margin = new Thickness(0, 0, 2, 0) };
            _BtnPaste = new Framework.UI.Button() { Row = 0, Column = 1, Style = _ButtonStyle, Margin = new Thickness(1) };
            _BtnPaste.Content.tooltip = "Paste from Clipboard";
            _BtnPaste.Click += _BtnPaste_Click;

            this.Controls.Add(_TextField);
            this.Controls.Add(_BtnPaste);


            if (persian)
            {
                _BtnConvertToPersian = new Framework.UI.Button() { Row = 1, Column = 1, Style = _ButtonStyle, Margin = new Thickness(1) };
                _BtnConvertToPersian.Content.tooltip = "Convert to Persian";
                this.Controls.Add(_BtnConvertToPersian);
                _BtnConvertToPersian.Click += _BtnConvertToPersian_Click;
                this.Height += 16;
            }



        }

        void _BtnConvertToPersian_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_TextField.Text))
                _TextField.Text = Skill.Framework.Text.Persian.Convert(_TextField.Text);
        }

        void _BtnPaste_Click(object sender, EventArgs e)
        {
            string st = UnityEditor.EditorGUIUtility.systemCopyBuffer;
            if (!string.IsNullOrEmpty(st))
            {
                _TextField.Text = st;
                Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
            }

        }

        protected override void Render()
        {
            if (_BtnPaste.Content.image == null)
            {
                _BtnPaste.Content.image = Skill.Editor.Resources.UITextures.Paste;
                if (_BtnConvertToPersian != null)
                    _BtnConvertToPersian.Content.image = Skill.Editor.Resources.UITextures.Convert;
            }
            base.Render();
        }
    }
}
