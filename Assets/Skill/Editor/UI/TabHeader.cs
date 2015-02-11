using Skill.Framework.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Editor.UI
{
    public class TabHeader : Grid
    {
        public int TabCount { get; private set; }

        private int _SelectedTab;
        public int SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                value = Mathf.Clamp(value, 0, TabCount - 1);
                if (value != _SelectedTab)
                {
                    _SelectedTab = value;
                    _Toggles[_SelectedTab].IsChecked = true;
                    OnSelectedTabChanged();
                    OnTabChanged();
                }
            }
        }

        public event EventHandler SelectedTabChanged;
        protected virtual void OnSelectedTabChanged()
        {
            if (SelectedTabChanged != null) SelectedTabChanged(this, EventArgs.Empty);
        }

        public event EventHandler TabChanged;
        protected virtual void OnTabChanged()
        {
            if (TabChanged != null) TabChanged(this, EventArgs.Empty);
        }

        public bool MultiSelect { get; private set; }
        public GUIStyle LabelStyle { get; private set; }

        private ToggleButtonGroup _Group;
        private Skill.Editor.UI.ToggleButton[] _Toggles;
        private Skill.Framework.UI.Label[] _Labels;

        public GUIContent this[int index] { get { return _Labels[index].Content; } }

        public TabHeader(int tabCount = 2, bool multiSelect = false)
        {
            this.MultiSelect = multiSelect;
            this.Height = 16;
            this.TabCount = Math.Max(2, tabCount);
            for (int i = 0; i < this.TabCount; i++)
                this.ColumnDefinitions.Add(1, GridUnitType.Star);

            this.LabelStyle = new GUIStyle() { alignment = TextAnchor.MiddleCenter }; this.LabelStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
            if (!multiSelect)
                _Group = new ToggleButtonGroup();
            this._Toggles = new Skill.Editor.UI.ToggleButton[this.TabCount];
            this._Labels = new Label[this.TabCount];
            for (int i = 0; i < this.TabCount; i++)
            {
                _Toggles[i] = new Skill.Editor.UI.ToggleButton() { Row = 0, Column = i, Group = _Group };
                _Labels[i] = new Skill.Framework.UI.Label() { Row = 0, Column = i, Style = LabelStyle };

                this.Controls.Add(_Toggles[i]);
                this.Controls.Add(_Labels[i]);

                if (multiSelect)
                    _Toggles[i].Changed += TabHeader_Changed;
                else
                    _Toggles[i].Checked += TabHeader_Checked;

            }
            this._SelectedTab = -1;
            this.SelectedTab = 0;
            this._FirstTryLoadEditorStyle = false;
        }

        void TabHeader_Changed(object sender, EventArgs e)
        {
            OnTabChanged();
        }

        void TabHeader_Checked(object sender, EventArgs e)
        {
            for (int i = 0; i < this.TabCount; i++)
            {
                if (sender == _Toggles[i])
                {
                    SelectedTab = i;
                    break;
                }
            }
        }

        private bool _FirstTryLoadEditorStyle;

        protected override void Render()
        {
            if (!_FirstTryLoadEditorStyle)
            {
                _Toggles[0].Style = (GUIStyle)"TL tab left";
                for (int i = 1; i < this.TabCount - 1; i++)
                    _Toggles[i].Style = (GUIStyle)"TL tab mid";
                _Toggles[this.TabCount - 1].Style = (GUIStyle)"TL tab right";
                _FirstTryLoadEditorStyle = true;
            }
            base.Render();
        }

        public bool IsTabSelected(int tabIndex)
        {
            return _Toggles[tabIndex].IsChecked;
        }
        public void SetTabSelected(int tabIndex, bool selected)
        {
            if (MultiSelect)
            {
                _Toggles[tabIndex].IsChecked = selected;
            }
            else
            {
                if (selected)
                    SelectedTab = tabIndex;
                else
                    throw new InvalidOperationException("Can not deselect tab while TabHeader is not MultiSelect");
            }
        }

    }
}
