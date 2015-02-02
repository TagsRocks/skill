using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using Skill.Editor.Animation;
using System.Collections.Generic;
using System.Linq;
using Skill.Framework.UI;
using System;

namespace Skill.Editor.AI
{
    [UnityEditor.CustomEditor(typeof(SharedAccessKeysAsset))]
    class SharedAccessKeysEditor : UnityEditor.Editor, IEditor
    {

        class AccessKeyListItem : EditorListItem
        {
            public AccessKeyData Data { get; private set; }

            public AccessKeyListItem(SharedAccessKeysEditor editor, AccessKeyData data)
                : base(editor, false)
            {
                this.Data = data;
            }

            public override void UpdateName()
            {
                ObjectName = string.Format("{0} ({1})", Data.Key, Data.Type);
            }
        }

        #region CreateUI
        private const float FrameHeight = 350;
        private const float ButtonRowHeight = 26;

        private Skill.Framework.UI.Frame _Frame;
        private Skill.Framework.UI.Extended.ListBox _ItemsList;
        private Skill.Framework.UI.Button _BtnAddCounterLimit;
        private Skill.Framework.UI.Button _BtnAddTimeLimit;
        private Skill.Editor.UI.TextField _BuildPathField;
        private Skill.Framework.UI.Button _BtnBuild;

        private CounterLimitAccessKeyField _CounterLimitField;
        private TimeLimitAccessKeyField _TimeLimitField;


        private void CreateUI()
        {
            _CounterLimitField = new CounterLimitAccessKeyField(this) { Row = 2, Column = 0, ColumnSpan = 2, Visibility = Visibility.Hidden };
            _TimeLimitField = new TimeLimitAccessKeyField(this) { Row = 2, Column = 0, ColumnSpan = 2, Visibility = Visibility.Hidden };

            _Frame = new Frame("MainFrame");
            _Frame.Grid.RowDefinitions.Add(ButtonRowHeight, GridUnitType.Pixel); // _BtnAdd
            _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star); // _ItemsList
            _Frame.Grid.RowDefinitions.Add(_CounterLimitField.LayoutHeight + 2, GridUnitType.Pixel); // _ObjectField
            _Frame.Grid.RowDefinitions.Add(30, GridUnitType.Pixel); // Build button
            _Frame.Grid.RowDefinitions.Add(20, GridUnitType.Pixel); // Build path

            _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star); // TimeLimit
            _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star); // Counter Limit

            _BtnAddTimeLimit = new Skill.Framework.UI.Button() { Row = 0, Column = 0, Margin = new Thickness(2) };
            _BtnAddTimeLimit.Content.text = "Time";
            _BtnAddTimeLimit.Content.tooltip = "add timelimit accessy key";
            _BtnAddTimeLimit.Click += _BtnAddTimeLimit_Click;

            _BtnAddCounterLimit = new Skill.Framework.UI.Button() { Row = 0, Column = 1, Margin = new Thickness(2) };
            _BtnAddCounterLimit.Content.text = "Counter";
            _BtnAddCounterLimit.Content.tooltip = "add counterlimit accessy key";
            _BtnAddCounterLimit.Click += _BtnAddCounterLimit_Click;

            _ItemsList = new Skill.Framework.UI.Extended.ListBox() { Row = 1, Column = 0, ColumnSpan = 2, Margin = new Thickness(2) };
            _ItemsList.BackgroundVisible = true;
            _ItemsList.DisableFocusable();

            _Frame.Controls.Add(new Box() { Row = 2, Column = 0, ColumnSpan = 2 });
            _Frame.Controls.Add(_BtnAddTimeLimit);
            _Frame.Controls.Add(_BtnAddCounterLimit);
            _Frame.Controls.Add(_ItemsList);
            _Frame.Controls.Add(_CounterLimitField);
            _Frame.Controls.Add(_TimeLimitField);


            _BtnBuild = new Skill.Framework.UI.Button() { Row = 3, Column = 0, ColumnSpan = 2, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _BtnBuild.Content.text = "Build";
            _Frame.Controls.Add(_BtnBuild);

            _BuildPathField = new UI.TextField() { Row = 4, Column = 0, ColumnSpan = 2, Margin = new Skill.Framework.UI.Thickness(0, 2) };
            _BuildPathField.Label.text = "Path";
            _BuildPathField.Text = Asset.BuildPath;
            _Frame.Controls.Add(_BuildPathField);


            _BtnBuild.Click += _BtnBuild_Click;
            _BuildPathField.TextChanged += _BuildPathField_TextChanged;

            _ItemsList.SelectionChanged += _ItemsPanel_SelectionChanged;
        }

        void _BuildPathField_TextChanged(object sender, System.EventArgs e)
        {
            Asset.BuildPath = _BuildPathField.Text;
            EditorUtility.SetDirty(target);
        }

        void _BtnBuild_Click(object sender, System.EventArgs e)
        {
            Build();
        }

        void _BtnAddCounterLimit_Click(object sender, EventArgs e)
        {
            Add(new CounterLimitAccessKeyData() { Key = "NewCounter" });
        }

        void _BtnAddTimeLimit_Click(object sender, EventArgs e)
        {
            Add(new TimeLimitAccessKeyData() { Key = "NewTime" });
        }

        private void Add(AccessKeyData data)
        {
            AccessKeyListItem newField = new AccessKeyListItem(this, data);
            newField.UpdateName();
            _ItemsList.Controls.Add(newField);
            _ItemsList.SelectedItem = newField;
        }

        void _ItemsPanel_SelectionChanged(object sender, EventArgs e)
        {
            if (_ItemsList.SelectedItem != null)
            {
                AccessKeyData data = ((AccessKeyListItem)_ItemsList.SelectedItem).Data;
                if (data.Type == Framework.AI.AccessKeyType.CounterLimit)
                {
                    _CounterLimitField.Data = data;
                    _CounterLimitField.Visibility = Visibility.Visible;

                    _TimeLimitField.Data = null;
                    _TimeLimitField.Visibility = Visibility.Hidden;
                }
                else
                {
                    _TimeLimitField.Data = data;
                    _TimeLimitField.Visibility = Visibility.Visible;

                    _CounterLimitField.Data = null;
                    _CounterLimitField.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                _CounterLimitField.Visibility = Visibility.Hidden;
                _TimeLimitField.Visibility = Visibility.Hidden;
            }
            Repaint();
        }

        public void Remove(EditorListItem item)
        {
            _ItemsList.Controls.Remove(item);
        }
        public void NewAfter(EditorListItem item)
        {
        }
        #endregion

        private void Save()
        {
            if (_Data != null)
            {
                _Data.Keys = new AccessKeyData[_ItemsList.Controls.Count];
                for (int i = 0; i < _ItemsList.Controls.Count; i++)
                {
                    AccessKeyListItem field = (AccessKeyListItem)_ItemsList.Controls[i];
                    _Data.Keys[i] = field.Data;
                }
                Asset.Save(_Data);
            }
        }

        private SharedAccessKeysAsset Asset { get { return target as SharedAccessKeysAsset; } }
        private SharedAccessKeysData _Data;
        void OnEnable()
        {
            _Data = Asset.Load();
            CreateUI();
            if (_Data != null)
            {
                if (_Data.Keys != null)
                {
                    foreach (var item in _Data.Keys)
                    {
                        if (item != null)
                        {
                            AccessKeyListItem field = new AccessKeyListItem(this, item);
                            field.UpdateName();
                            _ItemsList.Controls.Add(field);
                        }
                    }
                }
            }
            else
            {
                _ItemsList.Controls.Clear();
            }
            if (_ItemsList.Controls.Count > 0)
                _ItemsList.SelectedIndex = 0;
        }

        void OnDisable()
        {
            Save();
        }

        public override void OnInspectorGUI()
        {
            if (_ItemsList.SelectedStyle == null)
                _ItemsList.SelectedStyle = Resources.Styles.SelectedItem;
            _Frame.Update();
            _Frame.OnInspectorGUI(FrameHeight);
        }

        public void UpdateNames()
        {
            foreach (EditorListItem item in _ItemsList.Controls)
                item.UpdateName();
        }

        private void Build()
        {
            if (_Data != null)
            {
                Save();
                bool compiled = SharedAccessKeysCompiler.Compile(_Data);
                if (compiled)
                    Builder.Build(_Data, Asset.BuildPath, _Data.Name);
            }
            else
            {
                Debug.LogError("Invalid SharedAccessKeysData");
            }
        }

        #region Fields
        private class AccessKeyField : StackPanel
        {
            private Skill.Editor.UI.TextField _NameField;

            private AccessKeyData _Data;
            public virtual AccessKeyData Data
            {
                get { return _Data; }
                set
                {
                    _Data = value;
                    if (_Data != null)
                    {
                        this._NameField.Text = _Data.Key;
                        IsEnabled = true;
                    }
                    else
                    {
                        this._NameField.Text = string.Empty;
                        IsEnabled = false;
                    }
                }
            }
            public SharedAccessKeysEditor Editor { get; private set; }

            public AccessKeyField(SharedAccessKeysEditor editor)
            {
                this.Editor = editor;
                this.Orientation = Framework.UI.Orientation.Vertical;

                this._NameField = new Skill.Editor.UI.TextField() { Margin = new Thickness(4, 2) };
                this._NameField.Label.text = "Name";
                this.Padding = new Thickness(2);
                this.AutoHeight = true;
                this.Controls.Add(_NameField);
                this._NameField.TextChanged += _NameField_TextChanged;
            }

            void _NameField_TextChanged(object sender, System.EventArgs e)
            {
                if (this.Data != null)
                {
                    this.Data.Key = _NameField.Text;
                    Editor.UpdateNames();
                }
            }
        }
        private class CounterLimitAccessKeyField : AccessKeyField
        {
            private Skill.Editor.UI.IntField _MaxAccessCountField;

            public override AccessKeyData Data
            {
                get
                {
                    return base.Data;
                }
                set
                {
                    base.Data = value;
                    if (Data != null)
                    {
                        _MaxAccessCountField.Value = ((CounterLimitAccessKeyData)Data).MaxAccessCount;
                    }
                    else
                    {
                        _MaxAccessCountField.Value = 0;
                    }
                }
            }

            public CounterLimitAccessKeyField(SharedAccessKeysEditor editor)
                : base(editor)
            {
                this._MaxAccessCountField = new Skill.Editor.UI.IntField() { Margin = new Thickness(4, 2) };
                this._MaxAccessCountField.Label.text = "Max Access Count";
                this.Controls.Add(_MaxAccessCountField);
                this._MaxAccessCountField.ValueChanged += _MaxAccessCountField_ValueChanged;
            }

            void _MaxAccessCountField_ValueChanged(object sender, System.EventArgs e)
            {
                if (Data != null)
                    ((CounterLimitAccessKeyData)this.Data).MaxAccessCount = _MaxAccessCountField.Value;
            }
        }
        private class TimeLimitAccessKeyField : AccessKeyField
        {
            private Skill.Editor.UI.FloatField _TimeIntervalField;

            public override AccessKeyData Data
            {
                get
                {
                    return base.Data;
                }
                set
                {
                    base.Data = value;
                    if (Data != null)
                    {
                        _TimeIntervalField.Value = ((TimeLimitAccessKeyData)Data).TimeInterval;
                    }
                    else
                    {
                        _TimeIntervalField.Value = 0;
                    }
                }
            }

            public TimeLimitAccessKeyField(SharedAccessKeysEditor editor)
                : base(editor)
            {
                this._TimeIntervalField = new Skill.Editor.UI.FloatField() { Margin = new Thickness(4, 2) };
                this._TimeIntervalField.Label.text = "Time Interval";
                this.Controls.Add(_TimeIntervalField);
                this._TimeIntervalField.ValueChanged += _TimeIntervalField_ValueChanged;
            }

            void _TimeIntervalField_ValueChanged(object sender, System.EventArgs e)
            {
                if (Data != null)
                    ((TimeLimitAccessKeyData)this.Data).TimeInterval = _TimeIntervalField.Value;
            }
        }
        #endregion
    }




}
