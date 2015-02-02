using Skill.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor
{
    class PasteTextField : Grid
    {
        private static UnityEngine.GUIStyle _ButtonStyle;
        private Skill.Framework.UI.TextField _TextField;
        private Skill.Framework.UI.Button _BtnPaste;

        public Skill.Framework.UI.TextField TextField { get { return _TextField; } }

        public PasteTextField()
        {
            if (_ButtonStyle == null)
                _ButtonStyle = new UnityEngine.GUIStyle();

            this.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(16, GridUnitType.Pixel);

            this.RowDefinitions.Add(16, GridUnitType.Pixel);
            this.RowDefinitions.Add(1, GridUnitType.Star);

            _TextField = new  TextField() { Row = 0, RowSpan = 2, Column = 0, Margin = new Thickness(0,0,2,0) };
            _BtnPaste = new Framework.UI.Button() { Row = 0, Column = 1, Style = _ButtonStyle, Margin = new Thickness(1) };
            _BtnPaste.Content.tooltip = "Paste from Clipboard";


            this.Controls.Add(_TextField);
            this.Controls.Add(_BtnPaste);

            _BtnPaste.Click += _BtnPaste_Click;

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
                _BtnPaste.Content.image = Skill.Editor.Resources.UITextures.Paste;
            base.Render();
        }
    }
}
