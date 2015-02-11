using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework;


namespace Skill.Editor
{
    public interface ITextPickTarget
    {
        void Pick(TextKey pickedKey);
    }

    public class PickTextWindow : UnityEditor.EditorWindow
    {
        private static Vector2 Size = new Vector2(300, 500);
        private static PickTextWindow _Instance;
        public static PickTextWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = ScriptableObject.CreateInstance<PickTextWindow>();
                }
                return _Instance;
            }
        }

        void OnDisable()
        {
            _Browser.SelectionChanged -= _Browser_SelectionChanged;
            _Browser.SelectedDoubleClick -= _Browser_SelectedDoubleClick;
            _Frame.Controls.Remove(_Browser);
            _Instance = null;
        }
        public void OnGUI()
        {
            _Frame.OnGUI();
        }

        private static TextKeyBrowser _Browser;
        private Skill.Editor.UI.EditorFrame _Frame;
        private Skill.Editor.UI.TextField _TxtFilter;

        private Skill.Framework.UI.StackPanel _ButtonsPanel;
        private Skill.Framework.UI.Button _BtnSelect;
        private Skill.Framework.UI.Button _BtnCancel;


        public PickTextWindow()
        {
            hideFlags = HideFlags.DontSave;
            title = "Pick Text";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = new Vector2(Size.x, Size.y);

            CreateUI();
        }

        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
            _Frame.Grid.RowDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(18, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(4, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _Frame.Grid.RowDefinitions.Add(32, Skill.Framework.UI.GridUnitType.Pixel);

            _TxtFilter = new Skill.Editor.UI.TextField() { Row = 1 };
            _Frame.Controls.Add(_TxtFilter);

            _ButtonsPanel = new Skill.Framework.UI.StackPanel() { Row = 4, Orientation = Skill.Framework.UI.Orientation.Horizontal };
            _Frame.Controls.Add(_ButtonsPanel);

            if (_Browser == null)
                _Browser = new TextKeyBrowser() { Row = 3 };            
            _Frame.Controls.Add(_Browser);

            _BtnSelect = new Skill.Framework.UI.Button() { Width = 100, Margin = new Skill.Framework.UI.Thickness(2), HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Left, IsEnabled = _Browser.SelectedItem != null };
            _BtnSelect.Content.text = "Select";
            _ButtonsPanel.Controls.Add(_BtnSelect);

            _BtnCancel = new Skill.Framework.UI.Button() { Width = 100, Margin = new Skill.Framework.UI.Thickness(2), HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Left };
            _BtnCancel.Content.text = "Cancel";
            _ButtonsPanel.Controls.Add(_BtnCancel);



            _Browser.SelectedDoubleClick += _Browser_SelectedDoubleClick;
            _TxtFilter.TextChanged += _TxtFilter_TextChanged;
            _BtnSelect.Click += _BtnSelect_Click;
            _BtnCancel.Click += _BtnCancel_Click;
            _Browser.SelectionChanged += _Browser_SelectionChanged;
        }

        void _Browser_SelectedDoubleClick(object sender, System.EventArgs e)
        {
            OnSelect();
        }

        void _TxtFilter_TextChanged(object sender, System.EventArgs e)
        {
            _Browser.Filter(_TxtFilter.Text);
            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(_Frame.Grid);
        }

        void _Browser_SelectionChanged(object sender, System.EventArgs e)
        {
            _BtnSelect.IsEnabled = _Browser.SelectedIndex >= 0;
            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(_Frame.Grid);
        }
        void _BtnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        void _BtnSelect_Click(object sender, System.EventArgs e)
        {
            OnSelect();
        }

        private void OnSelect()
        {
            if (_Browser.SelectedItem != null && _TextPickTarget != null)
            {
                _TextPickTarget.Pick(((TextKeyView)_Browser.SelectedItem).Key);
                this.Close();
            }
        }

        private ITextPickTarget _TextPickTarget;
        public void Show(Dictionary dictionary, ITextPickTarget textPickTarget)
        {
            _TextPickTarget = textPickTarget;
            _Browser.Dictionary = dictionary;
            this.ShowAuxWindow();
        }
    }

    class TextKeyBrowser : Skill.Framework.UI.ListBox
    {
        private List<TextKeyView> _List;
        private Dictionary _Dictionary;
        public Dictionary Dictionary
        {
            get { return _Dictionary; }
            set
            {
                if (Dictionary != value)
                {
                    this.Controls.Clear();
                    this._List.Clear();
                }
                _Dictionary = value;
                Refresh();
            }
        }

        public TextKeyBrowser()
        {
            _List = new List<TextKeyView>();
            DisableFocusable();
        }
        private void Refresh()
        {
            if (_Dictionary == null)
            {
                this.Controls.Clear();
                this._List.Clear();
            }
            else
            {
                if (Dictionary.Keys != null)
                {
                    foreach (var k in Dictionary.Keys)
                    {
                        if (k != null)
                        {
                            if (!ViewExist(k))
                            {
                                TextKeyView view = new TextKeyView(k);
                                this._List.Add(view);
                                this.Controls.Add(view);
                            }
                        }
                    }

                    int index = 0;
                    while (index < _List.Count)
                    {
                        var view = _List[index];
                        if (!KeyExist(view, Dictionary.Keys))
                        {
                            this._List.Remove(view);
                            this.Controls.Remove(view);
                            continue;
                        }
                        index++;
                    }
                }
            }
        }
        private bool ViewExist(TextKey k)
        {
            foreach (var item in _List)
                if (k == item.Key) return true;
            return false;
        }
        private bool KeyExist(TextKeyView view, IEnumerable<TextKey> keys)
        {
            foreach (var k in keys)
                if (view.Key == k) return true;
            return false;
        }
        public void Filter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                foreach (var view in _List)
                    view.Visibility = Skill.Framework.UI.Visibility.Visible;
            }
            else
            {
                foreach (var view in _List)
                {
                    if (view.Key.Key.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        view.Visibility = Skill.Framework.UI.Visibility.Visible;
                    }
                    else
                    {
                        view.Visibility = Skill.Framework.UI.Visibility.Collapsed;
                        if (SelectedItem == view)
                            SelectedItem = null;
                    }

                }
            }
        }

        protected override void Render()
        {
            SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
            base.Render();
        }
    }
}

