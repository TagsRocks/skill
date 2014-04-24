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
                _LblClipName = new Skill.Framework.UI.Label() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10, Style = Resources.Styles.EventLabel };

                Controls.Add(_PreviewImage);
                Controls.Add(_LblClipName);

                base.WantsMouseEvents = true;
            }

            protected override void Render()
            {
                if (_Clip != PropertyKey.ValueKey)
                {
                    _Clip = PropertyKey.ValueKey;
                    if (_Clip != null && _PreviewImage.Texture == null)
                        _PreviewImage.Texture = UnityEditor.AssetPreview.GetAssetPreview(_Clip);
                    if (_Clip != null)
                    {
                        _PreviewImage.Visibility = Skill.Framework.UI.Visibility.Visible;
                        _LblClipName.Text = _Clip.name;
                    }
                    else
                    {
                        _LblClipName.Text = "None";
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