using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using System.Collections.Generic;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    /// <summary>
    /// TrackBar to edit SoundTrack
    /// </summary>
    public class SoundTrackBar : PropertyTrackBar<AudioClip>
    {
        protected override PropertyTrackBar<AudioClip>.PropertyTimeLineEvent CreateNewEvent(IPropertyKey<AudioClip> key) { return new SoundKeyView(this, (SoundKey)key); }
        protected override IPropertyKey<AudioClip> CreateNewKey() { return new SoundKey(); }
        protected override IPropertyKey<AudioClip>[] CreateKeyArray(int arraySize) { return new SoundKey[arraySize]; }


        public SoundTrackBar(SoundTrack track)
            : base(track)
        {
        }

        protected override void EvaluateNewKey(IPropertyKey<AudioClip> newKey, IPropertyKey<AudioClip> previousKey)
        {
            newKey.ValueKey = null;
        }

        #region SoundKeyView

        /// <summary>
        /// Visual representation of SoundKey
        /// </summary>
        class SoundKeyView : PropertyTimeLineEvent
        {
            private AudioClip _Clip;
            private Skill.Framework.UI.Label _LblClipName;
            private Skill.Framework.UI.Image _PreviewImage;
            public override double Duration { get { return PropertyKey.ValueKey != null ? PropertyKey.ValueKey.length : 0.5f; } set { } }
            public override string Title { get { return "Sound Event"; } }

            public SoundKeyView(SoundTrackBar trackBar, SoundKey key)
                : base(trackBar, key)
            {
                RowDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel);
                RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

                _PreviewImage = new Skill.Framework.UI.Image() { Row = 1, Column = 0, Visibility = Skill.Framework.UI.Visibility.Hidden, Scale = ScaleMode.StretchToFill, Margin = new Skill.Framework.UI.Thickness(2) };
                _LblClipName = new Skill.Framework.UI.Label() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10, Style = MatineeResources.Styles.EventLabel };

                Controls.Add(_PreviewImage);
                Controls.Add(_LblClipName);

                if (key.Clip == null) _LblClipName.Text = string.Empty;
                else _LblClipName.Text = key.Clip.name;

                base.WantsMouseEvents = true;
            }

            //protected override Properties CreateProperties() { return new SoundKeyViewProperties(this); }

            //class SoundKeyViewProperties : BaseTimeLineEventProperties
            //{

            //    private SoundKeyView _Event;
            //    private Skill.Editor.UI.ObjectField<AudioClip> _OFClip;
            //    private Skill.Editor.UI.Slider _SliVolumeFactor;

            //    public SoundKeyViewProperties(SoundKeyView tle)
            //        : base(tle)
            //    {
            //        _Event = tle;

            //        Skill.Framework.UI.Thickness margin = new Skill.Framework.UI.Thickness(2);

            //        _OFClip = new Skill.Editor.UI.ObjectField<AudioClip>() { Margin = margin }; _OFClip.Label.text = "Clip";
            //        _SliVolumeFactor = new Skill.Editor.UI.Slider() { Margin = margin, MinValue = 0, MaxValue = 1.0f }; _SliVolumeFactor.Label.text = "Volume Factor";

            //        Controls.Add(_OFClip);
            //        Controls.Add(_SliVolumeFactor);


            //        _OFClip.ObjectChanged += _OFClip_ObjectChanged;
            //        _SliVolumeFactor.ValueChanged += _SliVolumeFactor_ValueChanged;
            //    }

            //    protected override void RefreshData()
            //    {
            //        base.RefreshData();
            //        _OFClip.Object = _Event.PropertyKey.ValueKey;
            //        _SliVolumeFactor.Value = ((SoundKey)_Event.Key).VolumeFactor;
            //    }

            //    void _SliVolumeFactor_ValueChanged(object sender, System.EventArgs e)
            //    {
            //        if (IgnoreChanges) return;
            //        ((SoundKey)_Event.Key).VolumeFactor = _SliVolumeFactor.Value;
            //        SetDirty();
            //    }

            //    void _OFClip_ObjectChanged(object sender, System.EventArgs e)
            //    {
            //        if (IgnoreChanges) return;
            //        _Event.PropertyKey.ValueKey = _OFClip.Object;
            //        if (_Event.PropertyKey.ValueKey != null)
            //        {
            //            _Event._LblClipName.Text = _Event.PropertyKey.ValueKey.name;
            //        }
            //        else
            //        {
            //            _Event._LblClipName.Text = string.Empty;
            //        }
            //        SetDirty();
            //    }
            //}

            protected override void Render()
            {
                if (_Clip != PropertyKey.ValueKey)
                {
                    _Clip = PropertyKey.ValueKey;
                    if (_Clip != null)
                    {
                        _PreviewImage.Texture = UnityEditor.AssetPreview.GetAssetPreview(_Clip);
                        _PreviewImage.Visibility = Skill.Framework.UI.Visibility.Visible;
                    }
                    else
                    {
                        _PreviewImage.Texture = null;
                        _PreviewImage.Visibility = Skill.Framework.UI.Visibility.Hidden;
                    }
                }
                base.Render();
            }

        }
        #endregion
    }
}