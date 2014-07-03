using UnityEngine;
using System.Collections;
using Skill.Framework.UI;


using Skill.Framework.Sequence;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Sequence
{
    public class TrackTreeViewItem : Grid, IProperties
    {
        public Track Track { get; private set; }
        public BaseTrackBar TrackBar { get; private set; }

        public PropertiesPanel Properties { get; private set; }
        public bool IsSelectedProperties { get; set; }
        public string Title { get { return string.Format("{0} Track", Track.Type.ToString()); } }

        private Box _ContentBox;
        private Image _ImgColor;
        public void SetColor(Color color) { Track.Color = _ImgColor.TintColor = color; }
        public void SetName(string name) { Track.gameObject.name = _ContentBox.Content.text = name; }

        internal void SetVisibleStyle(bool visible)
        {
            this._ContentBox.Style = visible ? Resources.Styles.TreeViewItem : Resources.Styles.HiddenTreeViewItem;
        }

        internal TrackTreeViewItem(Track track, BaseTrackBar trackBar)
        {
            this.Track = track;
            this.TrackBar = trackBar;
            this.Height = 20;

            this.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(14, GridUnitType.Pixel);

            this._ContentBox = new Box() { Column = 0, Style = Resources.Styles.TreeViewItem };
            this._ImgColor = new Image() { Column = 1, Texture = UnityEditor.EditorGUIUtility.whiteTexture, Scale = ScaleMode.StretchToFill, Margin = new Thickness(0, 3) };

            this.Controls.Add(_ContentBox);
            this.Controls.Add(_ImgColor);

            this.Properties = CreateProperties();
            this._ContentBox.Content.image = GetIcon();
            this._ImgColor.TintColor = track.Color;
            this._ContentBox.Content.text = Track.gameObject.name;
        }

        private PropertiesPanel CreateProperties()
        {
            switch (Track.Type)
            {
                case TrackType.Event:
                    return new TrackTreeViewItemPropertiesBase(this);
                case TrackType.Bool:
                    return new PropertyTrackProperties<bool>(this);
                case TrackType.Float:
                    return new PropertyTrackProperties<float>(this);
                case TrackType.Integer:
                    return new PropertyTrackProperties<int>(this);
                case TrackType.Color:
                    return new PropertyTrackProperties<Color>(this);
                case TrackType.Vector2:
                    return new PropertyTrackProperties<Vector2>(this);
                case TrackType.Vector3:
                    return new PropertyTrackProperties<Vector3>(this);
                case TrackType.Vector4:
                    return new PropertyTrackProperties<Vector4>(this);
                case TrackType.Quaternion:
                    return new PropertyTrackProperties<Quaternion>(this);
                case TrackType.Sound:
                    return new SoundTrackProperties(this);
                default:
                    return null;
            }
        }
        private Texture2D GetIcon()
        {
            switch (Track.Type)
            {
                case TrackType.Event:
                    return Resources.Textures.Matinee.Event;
                case TrackType.Bool:
                    return Resources.Textures.Matinee.Boolean;
                case TrackType.Float:
                    return Resources.Textures.Matinee.Float;
                case TrackType.Integer:
                    return Resources.Textures.Matinee.Integer;
                case TrackType.Color:
                    return Resources.Textures.Matinee.Color;
                case TrackType.Vector2:
                    return Resources.Textures.Matinee.Vector2;
                case TrackType.Vector3:
                    return Resources.Textures.Matinee.Vector3;
                case TrackType.Vector4:
                    return Resources.Textures.Matinee.Vector4;
                case TrackType.Quaternion:
                    return Resources.Textures.Matinee.Quaternion;
                case TrackType.Sound:
                    return Resources.Textures.Matinee.Sound;
                default:
                    return null;
            }
        }
        public void Refresh()
        {
            _ContentBox.Content.text = Track.gameObject.name;
            Properties.Refresh();
            TrackBar.Refresh();
        }


    }

    public class TrackTreeViewItemPropertiesBase : ExposeProperties
    {
        public TrackTreeViewItem Item { get; private set; }

        private Skill.Editor.UI.TextField _TxtName;
        private Skill.Editor.UI.ColorField _CFColor;
        private Skill.Editor.UI.ToggleButton _TBVisible;

        public TrackTreeViewItemPropertiesBase(TrackTreeViewItem item)
            : base(item.Track)
        {
            this.Item = item;
        }

        protected override void CreateCustomFileds()
        {
            _TxtName = new Skill.Editor.UI.TextField() { Margin = ControlMargin };
            _TxtName.Label.text = "Name";
            _CFColor = new Skill.Editor.UI.ColorField() { Margin = ControlMargin };
            _CFColor.Label.text = "Color";
            _TBVisible = new Skill.Editor.UI.ToggleButton() { Margin = ControlMargin };
            _TBVisible.Label.text = "Visible";

            Controls.Add(_TxtName);
            Controls.Add(_CFColor);
            Controls.Add(_TBVisible);

            _TxtName.TextChanged += _TxtName_TextChanged;
            _CFColor.ColorChanged += _CFColor_ColorChanged;
            _TBVisible.Changed += _TBVisible_Changed;
        }
        void _TxtName_TextChanged(object sender, System.EventArgs e)
        {
            if (IgnoreChanges) return;
            if (string.IsNullOrEmpty(_TxtName.Text))
                _TxtName.Text = Item.Track.gameObject.name;
            Item.SetName(_TxtName.Text);
            SetDirty();
        }
        void _CFColor_ColorChanged(object sender, System.EventArgs e)
        {
            if (IgnoreChanges) return;
            Item.SetColor(_CFColor.Color);
            SetDirty();
        }
        void _TBVisible_Changed(object sender, System.EventArgs e)
        {
            if (IgnoreChanges) return;
            Item.Track.Visible = _TBVisible.IsChecked;
            Item.TrackBar.Visibility = Item.Track.Visible ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Collapsed;
            Item.SetVisibleStyle(_TBVisible.IsChecked);
            SetDirty();
        }

        protected override void RefreshData()
        {
            base.RefreshData();
            _TxtName.Text = Item.Track.gameObject.name;
            _CFColor.Color = Item.Track.Color;
            _TBVisible.IsChecked = Item.Track.Visible;
            Item.TrackBar.Visibility = Item.Track.Visible ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Collapsed;
            Item.SetVisibleStyle(_TBVisible.IsChecked);
        }

        protected override void SetDirty()
        {
            if (!Item.Track.IsDestroyed)
                UnityEditor.EditorUtility.SetDirty(Item.Track);
        }


    }

}