using UnityEngine;
using System.Collections;
using Skill.Framework.UI;


using Skill.Framework.Sequence;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Sequence
{
    public class TrackTreeViewItem : Grid, IProperties, ITrackBarTreeViewItem
    {
        public Track Track { get; private set; }
        public BaseTrackBar TrackBar { get; private set; }

        public PropertiesPanel Properties { get; private set; }
        public bool IsSelectedProperties { get; set; }
        public string Title { get { return string.Format("{0} Track", Track.Type.ToString()); } }

        private Box _ContentBox;
        private Skill.Editor.UI.Rectangle _Background;
        public void SetColor(Color color) { Track.Color = _Background.Color = color; }
        public void SetName(string name) { Track.gameObject.name = _ContentBox.Content.text = name; }

        public bool IsVisible
        {
            get
            {
                FolderView fv = Parent as FolderView;
                while (fv != null)
                {
                    if (!fv.Foldout.IsOpen)
                        return false;
                    fv = fv.Parent as FolderView;
                }

                return true;
            }
        }

        internal TrackTreeViewItem(Track track, BaseTrackBar trackBar)
        {

            this.Track = track;
            this.TrackBar = trackBar;
            this.TrackBar.TreeViewItem = this;
            this.Height = TrackBar.Height;

            this.ColumnDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(14, GridUnitType.Pixel);


            this._Background = new UI.Rectangle() { Column = 0, ColumnSpan = 20, Margin = new Thickness(0, 2) };
            this._ContentBox = new Box() { Column = 0 };

            this.Controls.Add(_Background);
            this.Controls.Add(_ContentBox);


            this.Properties = CreateProperties();
            this._ContentBox.Content.image = GetIcon();
            this._Background.Color = track.Color;
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
                    return Resources.UITextures.Matinee.Event;
                case TrackType.Bool:
                    return Resources.UITextures.Matinee.Boolean;
                case TrackType.Float:
                    return Resources.UITextures.Matinee.Float;
                case TrackType.Integer:
                    return Resources.UITextures.Matinee.Integer;
                case TrackType.Color:
                    return Resources.UITextures.Matinee.Color;
                case TrackType.Vector2:
                    return Resources.UITextures.Matinee.Vector2;
                case TrackType.Vector3:
                    return Resources.UITextures.Matinee.Vector3;
                case TrackType.Vector4:
                    return Resources.UITextures.Matinee.Vector4;
                case TrackType.Quaternion:
                    return Resources.UITextures.Matinee.Quaternion;
                case TrackType.Sound:
                    return Resources.UITextures.Matinee.Sound;
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

        protected override void BeginRender()
        {
            if (this._ContentBox.Style == null)
                this._ContentBox.Style = Skill.Editor.Resources.Styles.TreeViewItem;
            base.BeginRender();
        }
    }

    public class TrackTreeViewItemPropertiesBase : ExposeProperties
    {
        public TrackTreeViewItem Item { get; private set; }

        private Skill.Editor.UI.TextField _TxtName;
        private Skill.Editor.UI.ColorField _CFColor;

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

            Controls.Add(_TxtName);
            Controls.Add(_CFColor);

            _TxtName.TextChanged += _TxtName_TextChanged;
            _CFColor.ColorChanged += _CFColor_ColorChanged;
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

        protected override void RefreshData()
        {
            base.RefreshData();
            _TxtName.Text = Item.Track.gameObject.name;
            _CFColor.Color = Item.Track.Color;
        }

        protected override void SetDirty()
        {
            if (!Item.Track.IsDestroyed)
                UnityEditor.EditorUtility.SetDirty(Item.Track);
        }


    }

}