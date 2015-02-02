using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework.UI;
using Skill.Framework.UI.Extended;
using Skill.Editor.Animation;

namespace Skill.Editor.Animation
{
    public class AnimationSelectorWindow : EditorWindow
    {
        #region AnimationSelectorWindow
        private static AnimationSelectorWindow _Instance;
        public static AnimationSelectorWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = ScriptableObject.CreateInstance<AnimationSelectorWindow>();
                return _Instance;
            }
        }

        private static Vector2 Size = new Vector2(200, 400);
        private static Vector2 MinSize = new Vector2(200, 200);

        public AnimationSelectorWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.title = "Animation Selector";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = MinSize;
            CreateUI();
        }

        void OnFocus()
        {
            if (_Frame != null)
            {
                _RefreshStyles = true;
            }
        }

        void OnLostFocus()
        {
        }

        void OnDestroy()
        {
        }

        #endregion

        #region UI
        private bool _FocuseText;
        private bool _RefreshStyles = true;
        private Skill.Editor.UI.EditorFrame _Frame;

        private Skill.Framework.UI.TextField _TxtSearch;
        private Skill.Framework.UI.Extended.ListBox _AnimationList;
        private Skill.Framework.UI.Button _BtnSelect;
        private Skill.Framework.UI.Button _BtnCancel;

        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
            _Frame.Grid.Padding = new Thickness(2);
            _Frame.Grid.RowDefinitions.Add(20, GridUnitType.Pixel);
            _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star);
            _Frame.Grid.RowDefinitions.Add(24, GridUnitType.Pixel);

            _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star);

            _TxtSearch = new TextField() { Name = "Search", Row = 0, Column = 0, ColumnSpan = 2, VerticalAlignment = VerticalAlignment.Center };
            _Frame.Controls.Add(_TxtSearch);

            _AnimationList = new ListBox() { Row = 1, Column = 0, ColumnSpan = 2 };
            _AnimationList.DisableFocusable();
            _AnimationList.BackgroundVisible = true;
            _Frame.Controls.Add(_AnimationList);

            Thickness margin = new Thickness(2);
            _BtnCancel = new Button() { Row = 2, Column = 0, Margin = margin };
            _Frame.Controls.Add(_BtnCancel);

            _BtnSelect = new Button() { Row = 2, Column = 1, Margin = margin, IsEnabled = false };
            _Frame.Controls.Add(_BtnSelect);

            _BtnCancel.Content.text = "Cancel";
            _BtnSelect.Content.text = "Select";

            _BtnCancel.Click += _BtnCancel_Click;
            _BtnSelect.Click += _BtnSelect_Click;
            _AnimationList.SelectionChanged += _AnimationList_SelectionChanged;
            _TxtSearch.TextChanged += _TxtSearch_TextChanged;

            _FocuseText = true;
        }

        void _TxtSearch_TextChanged(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(_TxtSearch.Text))
            {
                foreach (var lbl in _AnimationList.Items)
                    lbl.Visibility = Visibility.Visible;
                return;
            }

            string filter = _TxtSearch.Text.ToLower();
            foreach (Label lbl in _AnimationList.Items)
            {
                if (lbl.Text.ToLower().Contains(filter))
                    lbl.Visibility = Visibility.Visible;
                else
                    lbl.Visibility = Visibility.Collapsed;
            }

            if (_AnimationList.SelectedItem != null)
            {
                if (_AnimationList.SelectedItem.Visibility != Visibility.Visible)
                    _AnimationList.SelectedItem = null;
            }
        }

        void _AnimationList_SelectionChanged(object sender, System.EventArgs e)
        {
            _BtnSelect.IsEnabled = _AnimationList.SelectedItem != null;
            Repaint();
        }

        void _BtnSelect_Click(object sender, System.EventArgs e)
        {
            if (_AnimationList.SelectedItem != null)
            {
                if (SequenceNode != null)
                {
                    _SequenceNode.AnimationName = ((Label)_AnimationList.SelectedItem).Text;
                    Close();
                }
            }
        }

        void _BtnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }
        void OnGUI()
        {
            if (_Frame != null)
            {
                RefreshStyles();

                if(_FocuseText)
                {
                    _FocuseText = false;
                    _Frame.FocusControl(_TxtSearch);
                }
                _Frame.Update();
                _Frame.OnGUI();
            }
        }
        private void RefreshStyles()
        {
            if (_RefreshStyles)
            {
                _RefreshStyles = false;
                _AnimationList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;
                _AnimationList.Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;
            }
        }

        #endregion


        private AnimNodeSequenceItem _SequenceNode;
        public AnimNodeSequenceItem SequenceNode
        {
            get { return _SequenceNode; }
            set
            {
                _SequenceNode = value;
                _AnimationList.SelectedItem = null;
                if (_SequenceNode != null)
                {
                    string animationName = _SequenceNode.AnimationName;
                    if (!string.IsNullOrEmpty(animationName))
                    {
                        foreach (Label item in _AnimationList.Items)
                        {
                            if (animationName.Equals(item.Text, System.StringComparison.OrdinalIgnoreCase))
                            {
                                _AnimationList.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Reload(SkinMeshData data)
        {
            _AnimationList.Items.Clear();
            if (data.Animations != null)
            {
                for (int i = 0; i < data.Animations.Length; i++)
                {
                    Label item = new Label() { Text = data.Animations[i].Name };
                    _AnimationList.Items.Add(item);
                }
            }
            _AnimationList.SelectedIndex = -1;
        }
    }
}