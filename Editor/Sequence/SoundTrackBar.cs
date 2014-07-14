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
    public class SoundTrackBar : BaseTrackBar
    {
        private List<SoundKeyView> _Events;
        private SoundTrack _SoundTrack;

        public override bool IsContinuous { get { return false; } }

        public SoundTrackBar(SoundTrack track)
            : base(track)
        {
            this.Height = BaseHeight * 2;
            _SoundTrack = track;
            _Events = new List<SoundKeyView>();
            CreateEvents();
            this.ContextMenu = SoundTrackBarContextMenu.Instance;
        }

        /// <summary>
        /// Refresh data do to changes outside of MatineeEditor
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            CreateEvents();
        }
        private void CreateEvents()
        {
            if (_SoundTrack.Keys != null)
            {
                // search for new events in DiscreteTrack that we didn't create DiscreteEvent for them 
                foreach (var key in _SoundTrack.Keys)
                {
                    if (key != null)
                    {
                        if (!IsEventExistWithKey(key))
                            CreateEvent(key);
                    }
                }

                // search for removed keys in SoundTrack that we did create SoundEvent for them 
                int index = 0;
                while (index < _Events.Count)
                {
                    var e = _Events[index];
                    if (!IsKeyExistInDiscreteTrack(e.Key))
                    {
                        _Events.Remove(e);
                        Controls.Remove(e);
                        continue;
                    }
                    index++;
                }
            }
            else
            {
                _Events.Clear();
                Controls.Clear();
            }
        }

        // create a SoundEvent and initialize it
        private SoundKeyView CreateEvent(SoundKey key)
        {
            SoundKeyView se = new SoundKeyView(this, key);
            se.ContextMenu = SoundEventContextMenu.Instance;
            this.Controls.Add(se);
            this._Events.Add(se);
            return se;
        }

        private bool IsEventExistWithKey(SoundKey key)
        {
            foreach (var e in _Events)
                if (e.Key == key) return true;
            return false;
        }
        private bool IsKeyExistInDiscreteTrack(SoundKey key)
        {
            foreach (var k in _SoundTrack.Keys)
            {
                if (k == key)
                    return true;
            }
            return false;
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

                SoundKey newKey = new SoundKey();
                newKey.Clip = null;
                newKey.FireTime = (float)timeLine.TimeBar.GetTime(x);

                SoundKeyView e = CreateEvent(newKey);
                RebuildTrackKeys();

                InspectorProperties.Select(e);
            }
        }



        /// <summary>
        /// Delete SoundEvent 
        /// </summary>
        /// <param name="soundEvent">SoundEvent to delete</param>
        private void Delete(SoundKeyView e)
        {
            if (_Events.Remove(e))
            {
                this.Controls.Remove(e);
                RebuildTrackKeys();
            }
        }

        /// <summary>
        /// Rebuild Keys pf SoundTrack
        /// </summary>
        private void RebuildTrackKeys()
        {
            SoundKey[] keys = new SoundKey[_Events.Count];
            for (int i = 0; i < _Events.Count; i++)
                keys[i] = _Events[i].Key;
            _SoundTrack.Keys = keys;
            _SoundTrack.SortKeys();
            SetDirty();
        }

        #region SoundKeyView

        /// <summary>
        /// Visual representation of SoundKey
        /// </summary>
        class SoundKeyView : KeyView
        {
            private Skill.Framework.UI.Label _LblClipName;
            private Skill.Editor.UI.Extended.AudioPreviewCurve _PreviewImage;

            public SoundKey Key { get; private set; }
            public override double Duration { get { return Key.ValueKey != null ? Key.ValueKey.length : 0.01; } set { } }
            public override string Title { get { return "Sound Event"; } }
            public override float MinWidth { get { return 20; } }

            public SoundKeyView(SoundTrackBar trackBar, SoundKey key)
                : base(trackBar)
            {
                this.Key = key;
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
                _PreviewImage.Clip = Key.ValueKey;

                if (Key.ValueKey != null)
                {
                    _LblClipName.Text = Key.ValueKey.name;
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
                    _PreviewImage.Visibility = Framework.UI.Visibility.Visible;
                    _PreviewImage.RenderArea = RenderArea;
                    _LblClipName.Text = string.Empty;
                }
                base.BeginRender();
            }            


            private SoundKeyViewProperties _Properties;
            /// <summary> Properties </summary>
            public override PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new SoundKeyViewProperties(this);
                    return _Properties;
                }
            }
            public override double FireTime { get { return Key.FireTime; } set { Key.FireTime = (float)value; Properties.Refresh(); } }
            public class SoundKeyViewProperties : ExposeProperties
            {
                protected SoundKeyView _View;
                public SoundKeyViewProperties(SoundKeyView view)
                    : base(view.Key)
                {
                    _View = view;
                }
                protected override void SetDirty()
                {
                    ((BaseTrackBar)_View.TrackBar).SetDirty();
                }
            }
        }
        #endregion

        #region SoundEventContextMenu
        class SoundEventContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static SoundEventContextMenu _Instance;
            public static SoundEventContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new SoundEventContextMenu();
                    return _Instance;
                }
            }

            private Skill.Editor.UI.MenuItem _DeleteItem;

            public SoundEventContextMenu()
            {
                _DeleteItem = new Skill.Editor.UI.MenuItem("Delete");
                Add(_DeleteItem);
                _DeleteItem.Click += _DeleteItem_Click;
            }

            void _DeleteItem_Click(object sender, System.EventArgs e)
            {
                SoundKeyView se = (SoundKeyView)Owner;
                ((SoundTrackBar)se.TrackBar).Delete(se);
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