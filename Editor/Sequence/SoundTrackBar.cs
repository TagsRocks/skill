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
            this.Height = 40;
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
            private Skill.Framework.UI.Label _LblClipName;
            private Skill.Editor.UI.Extended.AudioPreviewCurve _PreviewImage;

            public override double Duration { get { return PropertyKey.ValueKey != null ? PropertyKey.ValueKey.length : 0.5f; } set { } }
            public override string Title { get { return "Sound Event"; } }

            public SoundKeyView(SoundTrackBar trackBar, SoundKey key)
                : base(trackBar, key)
            {
                _PreviewImage = new AudioPreviewCurve() { Margin = new Skill.Framework.UI.Thickness(2), BackgroundColor = Color.clear };
                _LblClipName = new Skill.Framework.UI.Label() { Style = Resources.Styles.EventLabel };

                Controls.Add(_PreviewImage);
                Controls.Add(_LblClipName);

                base.WantsMouseEvents = true;
            }


            protected override void BeginRender()
            {
                _PreviewImage.Clip = PropertyKey.ValueKey;

                if (PropertyKey.ValueKey != null)
                    _LblClipName.Text = PropertyKey.ValueKey.name;
                else
                    _LblClipName.Text = string.Empty;

                TimeLine timeLine = FindInParents<TimeLine>();
                Rect trackRa = TrackBar.RenderArea;
                double deltaTime = (timeLine.MaxTime - timeLine.MinTime);

                float minVisibleX = trackRa.x + trackRa.width * (float)((timeLine.StartVisible - timeLine.MinTime) / deltaTime);
                float maxVisibleX = trackRa.x + trackRa.width * (float)((timeLine.EndVisible - timeLine.MinTime) / deltaTime);

                Rect ra = RenderArea;

                float xMin = Mathf.Max(ra.xMin, minVisibleX);
                float xMax = Mathf.Min(ra.xMax, maxVisibleX);
                float delta = xMax - xMin;
                if (delta >= 1)
                {
                    _PreviewImage.Visibility = Framework.UI.Visibility.Visible;
                    Rect ranges = new Rect(((xMin - ra.xMin) / ra.width) * _PreviewImage.Resolution, 0, ((xMax - ra.xMin) / ra.width) * _PreviewImage.Resolution, 1);
                    ra.xMin = xMin;
                    ra.xMax = xMax;
                    _PreviewImage.RenderArea = ra;
                    _PreviewImage.Ranges = ranges;
                }
                else
                    _PreviewImage.Visibility = Framework.UI.Visibility.Hidden;

                base.BeginRender();
            }

        }
        #endregion
    }
}