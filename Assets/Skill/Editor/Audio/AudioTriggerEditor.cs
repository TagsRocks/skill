using UnityEngine;
using System.Collections;
using System.Linq;
using Skill.Framework.UI;
using Skill.Framework.Audio;

namespace Skill.Editor.Audio
{
    public class AudioTriggerEditor : Grid
    {

        private Label _TriggerLabel;
        private ListBox _TriggerList;
        private UniformGrid _TriggerToolbar;
        private Button _BtnAddTrigger;
        private Button _BtnRemoveTrigger;
        private AudioControllerEditorWindow _Editor;

        public event System.EventHandler SelectedTriggerChanged;

        private void OnSelectedTriggerChanged()
        {
            if (SelectedTriggerChanged != null)
                SelectedTriggerChanged(this, System.EventArgs.Empty);
        }
        public int SelectedTriggerIndex { get { return _TriggerList.SelectedIndex; } }
        public AudioTriggerEditor(AudioControllerEditorWindow editor)
        {
            this._Editor = editor;

            this.RowDefinitions.Add(20, GridUnitType.Pixel);
            this.RowDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(60, GridUnitType.Pixel);

            _TriggerLabel = new Label() { Row = 0, Column = 0, Text = "Triggers" };
            this.Controls.Add(_TriggerLabel);

            _TriggerList = new ListBox() { Row = 1, Column = 0, ColumnSpan = 2 };
            _TriggerList.DisableFocusable();
            this.Controls.Add(_TriggerList);

            _TriggerToolbar = new UniformGrid() { Row = 0, Column = 1, Rows = 1, Columns = 2 };
            this.Controls.Add(_TriggerToolbar);

            _BtnAddTrigger = new Button() { Column = 0 };
            _BtnAddTrigger.Content.tooltip = "add new trigger";
            _TriggerToolbar.Controls.Add(_BtnAddTrigger);

            _BtnRemoveTrigger = new Button() { Column = 1 };
            _BtnRemoveTrigger.Content.tooltip = "remove selected trigger";
            _TriggerToolbar.Controls.Add(_BtnRemoveTrigger);

            SetButtonsEnable();

            _TriggerList.SelectionChanged += _TriggerList_SelectionChanged;
            _BtnAddTrigger.Click += _BtnAddTrigger_Click;
            _BtnRemoveTrigger.Click += _BtnRemoveTrigger_Click;
        }

        void _BtnRemoveTrigger_Click(object sender, System.EventArgs e)
        {
            RemoveSelectedTrigger();
        }
        void _BtnAddTrigger_Click(object sender, System.EventArgs e)
        {
            AddNewTrigger();
        }
        void _TriggerList_SelectionChanged(object sender, System.EventArgs e)
        {
            OnSelectedTriggerChanged();
            SetButtonsEnable();
            Skill.Editor.UI.InspectorProperties.Select((TriggerItem)_TriggerList.SelectedItem);
            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
        }
        private void SetButtonsEnable()
        {
            _BtnAddTrigger.IsEnabled = true;
            _BtnRemoveTrigger.IsEnabled = _TriggerList.SelectedItem != null;
        }
        internal void Clear()
        {
            _TriggerList.Items.Clear();
        }
        public void RefreshStyles()
        {
            _TriggerLabel.Style = Skill.Editor.Resources.Styles.Header;

            _BtnAddTrigger.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnRemoveTrigger.Style = Skill.Editor.Resources.Styles.ToolbarButton;

            _BtnAddTrigger.Content.image = Skill.Editor.Resources.UITextures.Add;
            _BtnRemoveTrigger.Content.image = Skill.Editor.Resources.UITextures.Remove;
            _TriggerList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
        }


        public void Rebuild()
        {

            if (_Editor.Controller.Triggers == null)
                _Editor.Controller.Triggers = new string[0];

            _TriggerList.Items.Clear();
            for (int i = 0; i < _Editor.Controller.Triggers.Length; i++)
                Add(_Editor.Controller.Triggers[i], i);


        }
        private void Add(string triggerName, int index)
        {
            TriggerItem item = new TriggerItem(this, index);
            _TriggerList.Items.Add(item);
        }

        private bool IsTriggerExists(string name)
        {
            foreach (TriggerItem item in _TriggerList.Items)
            {
                if (item.Name2 == name)
                    return true;
            }
            return false;
        }

        private void RemoveSelectedTrigger()
        {
            if (_TriggerList.SelectedItem != null)
            {
                string selectedTriggerName = ((TriggerItem)_TriggerList.SelectedItem).Name2;
                string[] preTriggers = _Editor.Controller.Triggers;
                string[] newTriggers = new string[preTriggers.Length - 1];

                int preIndex = 0;
                int newIndex = 0;
                while (newIndex < newTriggers.Length && preIndex < preTriggers.Length)
                {
                    if (preTriggers[preIndex] == selectedTriggerName)
                    {
                        preIndex++;
                        continue;
                    }
                    newTriggers[newIndex] = preTriggers[preIndex];
                    newIndex++;
                    preIndex++;
                }
                _Editor.Controller.Triggers = newTriggers;
                _TriggerList.Items.Remove(_TriggerList.SelectedItem);
                _TriggerList.SelectedIndex = 0;
                SetButtonsEnable();
            }
            else
            {
                Debug.LogError("there is no selected trigger to remove");
            }
        }
        private void AddNewTrigger()
        {
            string newName = GetUniqueTriggerName("NewTrigger");
            string[] preTriggers = _Editor.Controller.Triggers;
            string[] newTriggers = new string[preTriggers.Length + 1];
            preTriggers.CopyTo(newTriggers, 0);
            newTriggers[newTriggers.Length - 1] = newName;
            _Editor.Controller.Triggers = newTriggers;
            Add(newName, newTriggers.Length - 1);
            SetButtonsEnable();
        }

        private string GetUniqueTriggerName(string name)
        {
            int i = 1;
            string newName = name;
            while (_Editor.Controller.Triggers.Where(b => b == newName).Count() > 0)
                newName = name + i++;
            return newName;
        }

        public void DeselectInspector()
        {
            if (Skill.Editor.UI.InspectorProperties.GetSelected() is TriggerItem)
                Skill.Editor.UI.InspectorProperties.Select(null);
        }

        public void RefreshContents()
        {
            foreach (TriggerItem item in _TriggerList.Items)
            {
                item.RefreshContent();
            }
        }

        private class TriggerItem : Label, Skill.Editor.UI.IProperties
        {
            public int TriggerIndex { get; private set; }

            private AudioTriggerEditor _Editor;

            public TriggerItem(AudioTriggerEditor editor, int index)
            {
                this._Editor = editor;
                this.TriggerIndex = index;
                this.Text = Name2;
            }

            [Skill.Framework.ExposeProperty(1, "Name", "name of trigger")]
            public string Name2
            {
                get
                {
                    return _Editor._Editor.Controller.Triggers[TriggerIndex];
                }
                set
                {
                    _Editor._Editor.Controller.Triggers[TriggerIndex] = value;
                    this.Text = value;
                }
            }

            public bool IsSelectedProperties { get; set; }
            private TriggerItemProperties _Properties;
            public Skill.Editor.UI.PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new TriggerItemProperties(this);
                    return _Properties;
                }
            }
            public string Title { get { return "Audio Tigger"; } }

            class TriggerItemProperties : Skill.Editor.UI.ExposeProperties
            {
                private TriggerItem _Item;
                public TriggerItemProperties(TriggerItem item)
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
                this.Text = Name2;
            }
        }
    }


}