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
    public class SoundTrackBar : EventOrientedTrackBar
    {
        //private SoundTrack _SoundTrack;

        public SoundTrackBar(SoundTrack track)
            : base(track)
        {
            this.Height = BaseHeight * 2;
            //_SoundTrack = track;
            this.ContextMenu = SoundTrackBarContextMenu.Instance;
        }

        protected override EventOrientedKeyView CreateNewEvent(EventOrientedKey key)
        {
            return new SoundKeyView(this, (SoundKey)key);
        }

        public override void AddKey()
        {
            TimeLine timeLine = FindInParents<TimeLine>();
            if (timeLine != null)
            {
                float time = (float)timeLine.TimePosition;
                AddKey(time);
            }
        }

        private void AddKey(float time)
        {
            SoundKey newKey = ScriptableObject.CreateInstance<SoundKey>();
            newKey.Clip = null;
            newKey.FireTime = time;

            EventOrientedKeyView e = CreateEvent(newKey);
            RebuildTrackKeys();
            InspectorProperties.Select(e);

        }

        /// <summary>
        /// add key at position
        /// </summary>
        /// <param name="x">position inside track</param>
        private void AddKeyAtPosition(float x)
        {
            TimeLine timeLine = FindInParents<TimeLine>();
            if (timeLine != null)
            {
                // convert to local position of TimeBar - because of zooming
                x -= timeLine.View.ScrollPosition.x;
                AddKey((float)timeLine.TimeBar.GetTime(x));
            }
        }

        #region SoundKeyView

        /// <summary>
        /// Visual representation of SoundKey
        /// </summary>
        class SoundKeyView : EventOrientedKeyView
        {
            private Skill.Framework.UI.Label _LblClipName;
            private Skill.Editor.UI.Extended.AudioPreviewCurve _PreviewImage;
            public override string Title { get { return "Sound Event"; } }
            public override float MinWidth { get { return 20; } }

            private SoundKey _SoundKey;

            public SoundKeyView(SoundTrackBar trackBar, SoundKey key)
                : base(trackBar, key)
            {
                this._SoundKey = key;
                _PreviewImage = new AudioPreviewCurve() { Margin = new Skill.Framework.UI.Thickness(2), BackgroundColor = Color.clear };
                _LblClipName = new Skill.Framework.UI.Label() { Style = Resources.Styles.EventLabel };

                Controls.Add(_PreviewImage);
                Controls.Add(_LblClipName);
            }

            private Rect[] _CurevRenderAreas = new Rect[1];
            private Rect[] _CurevRanges = new Rect[1];
            private AnimationCurve[] _Curves = new AnimationCurve[1];
            protected override void BeginRender()
            {
                if (_SoundKey.Clip != null)
                {
                    _LblClipName.Text = _SoundKey.Clip.name;

                    if (_SoundKey.Visualize)
                    {
                        _PreviewImage.Clip = _SoundKey.Clip;
                        CalcCurveRenderArea(ref _CurevRenderAreas, ref _CurevRanges, _Curves);

                        Rect ra = _CurevRenderAreas[0];
                        Rect ranges = _CurevRanges[0];

                        ranges.x *= _PreviewImage.Resolution;
                        ranges.width *= _PreviewImage.Resolution;

                        _PreviewImage.RenderArea = ra;
                        _PreviewImage.Ranges = ranges;

                        _PreviewImage.Visibility = ra.width > 0 ? Framework.UI.Visibility.Visible : Framework.UI.Visibility.Hidden;
                    }
                    else
                    {
                        _PreviewImage.Clip = null;
                        _PreviewImage.Visibility = Framework.UI.Visibility.Visible;
                    }
                }
                else
                {
                    _PreviewImage.Clip = null;
                    _PreviewImage.Visibility = Framework.UI.Visibility.Visible;
                    _PreviewImage.RenderArea = RenderArea;
                    _LblClipName.Text = "Null";
                }
                base.BeginRender();
            }
        }
        #endregion


        #region SoundTrackBarContextMenu
        class SoundTrackBarContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static SoundTrackBarContextMenu _Instance;
            public static SoundTrackBarContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new SoundTrackBarContextMenu();
                    return _Instance;
                }
            }

            private Skill.Editor.UI.MenuItem _AddKeyItem;
            private SoundTrackBarContextMenu()
            {
                _AddKeyItem = new Skill.Editor.UI.MenuItem("Add Audio");

                Add(_AddKeyItem);
                _AddKeyItem.Click += _AddKeyItem_Click;
            }

            void _AddKeyItem_Click(object sender, System.EventArgs e)
            {
                SoundTrackBar trackBar = (SoundTrackBar)Owner;
                trackBar.AddKeyAtPosition(Position.x);
            }
        }
        #endregion


    }
}