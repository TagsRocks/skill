using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.UI;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Contains some EditorControl and allow user to select on of them.
    /// </summary>
    public class SelectionField : EditorControl
    {
        /// <summary>
        /// Hold information about fields
        /// </summary>
        private class FieldTag
        {
            /// <summary> Field </summary>
            public EditorControl Field { get; set; }
            /// <summary> Label for Field </summary>
            public string Label { get; set; }
            /// <summary> Option in Popup </summary>
            public PopupOption Option { get; set; }
        }

        // Variables    
        private Box _Background;
        private Popup _Popup;
        private Label _Label;
        private Grid _Panel;
        private List<FieldTag> _Fields;

        /// <summary> Popup options at top right corner </summary>
        public Popup Popup { get { return _Popup; } }
        /// <summary> Label that show info about selected field  </summary>
        public Label Label { get { return _Label; } }
        /// <summary> Background </summary>
        public Box Background { get { return _Background; } }

        /// <summary>
        /// Retrieves fields by index
        /// </summary>
        /// <param name="index">Index of field</param>
        /// <returns>Field atspecified index</returns>
        public EditorControl this[int index]
        {
            get { return _Fields[index].Field; }
        }

        private EditorControl _SelectedField;
        /// <summary>
        /// Selected Field by user
        /// </summary>
        public EditorControl SelectedField
        {
            get { return _SelectedField; }
            private set
            {
                if (_SelectedField != value)
                {
                    if (_SelectedField != null)
                        _SelectedField.Visibility = Skill.UI.Visibility.Hidden;
                    _SelectedField = value;
                    if (_SelectedField != null)
                    {
                        _SelectedField.Row = 0;
                        _SelectedField.RowSpan = 2;
                        _SelectedField.Column = 1;
                        SelectedField.Margin = new Thickness(0, _Label.Margin.Top, 0, _Label.Margin.Bottom);
                        SelectedField.VerticalAlignment = Skill.UI.VerticalAlignment.Top;
                        _SelectedField.Visibility = Skill.UI.Visibility.Visible;
                    }
                    OnSelectedFieldChanged();
                }
            }
        }

        /// <summary>
        /// Retrieves number of fields
        /// </summary>
        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Height to use by layout
        /// </summary>
        public override float LayoutHeight
        {
            get
            {
                if (Visibility != Skill.UI.Visibility.Collapsed)
                {
                    float h = Mathf.Max(_Label.Height + _Label.Margin.Vertical, _Popup.Height + _Popup.Margin.Vertical);
                    if (SelectedField != null)
                        return Mathf.Max(h, SelectedField.Height + SelectedField.Margin.Vertical);
                    else
                        return h;
                }
                return base.Height;
            }
        }

        /// <summary>
        /// Occurs when SelectedField of SelectionFoild changed
        /// </summary>
        public event EventHandler SelectedFieldChanged;
        protected virtual void OnSelectedFieldChanged()
        {
            if (SelectedFieldChanged != null) SelectedFieldChanged(this, EventArgs.Empty);
            OnLayoutChanged();
        }

        /// <summary>
        /// Notify that PaintArea changed
        /// </summary>
        protected override void OnPaintAreaChanged()
        {
            base.OnPaintAreaChanged();
            this._Panel.PaintArea = PaintArea;
        }

        /// <summary>
        /// Create an instance of SelectionField
        /// </summary>
        public SelectionField()
        {
            this._Background = new Box() { RowSpan = 2, ColumnSpan = 3 };
            this._Popup = new Popup() { Position = new Rect(0, 0, 16, 16), Column = 2, Row = 0, VerticalAlignment = Skill.UI.VerticalAlignment.Center, HorizontalAlignment = Skill.UI.HorizontalAlignment.Right, Margin = new Thickness(2) };
            this._Label = new Label() { Position = new Rect(0, 0, 100, 16), Column = 0, Row = 0, VerticalAlignment = Skill.UI.VerticalAlignment.Center, HorizontalAlignment = Skill.UI.HorizontalAlignment.Left, Margin = new Thickness(2) };
            this._Panel = new Grid();

            this._Panel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            this._Panel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            this._Panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            this._Panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            this._Panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(22, GridUnitType.Pixel) });
            this._Fields = new List<FieldTag>();

            this._Panel.Controls.Add(_Background);
            this._Panel.Controls.Add(_Label);
            this._Panel.Controls.Add(_Popup);

            this._Popup.OptionChanged += Popup_OptionChanged;
            this._Panel.LayoutChanged += Panel_LayoutChanged;


            ShowField(0);
        }

        private void Panel_LayoutChanged(object sender, EventArgs e)
        {
            OnLayoutChanged();
        }

        private void Popup_OptionChanged(object sender, EventArgs e)
        {
            ShowField(_Popup.SelectedIndex);
        }

        private int FindField(EditorControl field)
        {
            for (int i = 0; i < _Fields.Count; i++)
            {
                if (_Fields[i].Field == field)
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// Select field
        /// </summary>
        /// <param name="field">field to select</param>
        public void SelectField(EditorControl field)
        {
            ShowField(FindField(field));
        }

        /// <summary>
        /// Select field by index
        /// </summary>
        /// <param name="fieldIndex">Index of field to select</param>
        public void ShowField(int fieldIndex)
        {
            if (fieldIndex >= 0 && fieldIndex < _Fields.Count)
            {
                FieldTag ft = _Fields[fieldIndex];
                if (ft.Field != SelectedField)
                {
                    SelectedField = ft.Field;
                    if (SelectedField != null)
                    {
                        _Label.Content.text = _Fields[fieldIndex].Label;
                        _Popup.SelectedOption = ft.Option;
                    }
                    else
                    {
                        _Label.Content.text = string.Empty;
                        _Popup.SelectedOption = null;
                    }
                }
            }
        }

        /// <summary>
        /// Paint contents
        /// </summary>
        protected override void Paint()
        {
            this._Panel.OnGUI();
        }



        /// <summary>
        /// Add a field
        /// </summary>
        /// <param name="field">Field to add</param>
        /// <param name="label">Label for field</param>
        public void AddField(EditorControl field, string label)
        {
            if (field == null)
                throw new ArgumentNullException("Invalid field");
            if (string.IsNullOrEmpty(label))
                throw new ArgumentException("Invalid label for field");

            FieldTag ft = new FieldTag() { Field = field, Label = label, Option = new PopupOption() };
            ft.Option.Content.text = label;
            _Fields.Add(ft);
            _Popup.Options.Add(ft.Option);
            field.Row = 0;
            field.RowSpan = 2;
            field.Column = 1;
            _Panel.Controls.Add(field);
            if (SelectedField == null)
                SelectField(field);
            else
                field.Visibility = Skill.UI.Visibility.Hidden;
        }

        /// <summary>
        /// Remove specified field
        /// </summary>
        /// <param name="field">field to remove</param>
        /// <returns>true if success, otherwise false</returns>
        public bool RemoveField(EditorControl field)
        {
            return RemoveField(FindField(field));
        }
        /// <summary>
        /// Remove field by index
        /// </summary>
        /// <param name="fieldIndex">index of field to remove</param>
        /// <returns>true if success, otherwise false</returns>
        public bool RemoveField(int fieldIndex)
        {
            if (fieldIndex >= 0 && fieldIndex < _Fields.Count)
            {
                FieldTag ft = _Fields[fieldIndex];
                _Fields.RemoveAt(fieldIndex);
                _Popup.Options.Remove(ft.Option);
                _Panel.Controls.Remove(ft.Field);

                if (SelectedField == ft.Field)
                    ShowField(fieldIndex > 0 ? fieldIndex - 1 : fieldIndex);

                ft.Field.Visibility = Skill.UI.Visibility.Visible;
                return true;
            }
            return false;
        }
    }
}
