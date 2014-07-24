using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using System.Collections.Generic;
using Skill.Framework.Sequence;
using System;
using Skill.Editor.UI;

namespace Skill.Editor.Sequence
{
    /// <summary>
    /// TrackBar to edit PropertyTrack
    /// </summary>
    public class AnimationTrackBar : EventOrientedTrackBar
    {
        private AnimationTrack _AnimationTrack;

        /// <summary>
        /// Create a AnimatorTrackBar
        /// </summary>
        /// <param name="track"> AnimatorTrackBar to edit</param>
        public AnimationTrackBar(AnimationTrack track)
            : base(track)
        {
            _AnimationTrack = track;
            this.ContextMenu = AnimationTrackBarContextMenu.Instance;
        }


        protected override EventOrientedKeyView CreateNewEvent(EventOrientedKey key)
        {
            return new AnimationKeyView(this, (AnimationKey)key);
        }

        /// <summary>
        /// add key at position
        /// </summary>
        /// <param name="x">position inside track</param>
        private void AddKeyAt(float x, AnimationKey newKey)
        {
            TimeLine timeLine = FindInParents<TimeLine>();
            if (timeLine != null)
            {
                // convert to local position of TimeBar - because of zooming
                x -= timeLine.View.ScrollPosition.x;
                newKey.FireTime = (float)timeLine.TimeBar.GetTime(x);

                EventOrientedKeyView e = CreateEvent(newKey);
                InspectorProperties.Select(e);
                RebuildTrackKeys();
            }
        }

        #region AnimationKeyView

        /// <summary>
        /// Visual representation of EventKey
        /// </summary>
        protected class AnimationKeyView : EventOrientedKeyView
        {
            private string _Title;
            public override string Title
            {
                get
                {
                    if (_Title == null)
                        _Title = Key.GetType().Name;
                    return _Title;
                }
            }

            private Skill.Framework.UI.Image _ImgState;
            private Skill.Framework.UI.Box _Bg;

            private AnimationKey _AnimationKey;

            public AnimationKeyView(AnimationTrackBar trackBar, AnimationKey key)
                : base(trackBar, key)
            {
                _AnimationKey = key;

                this.ColumnDefinitions.Add(10, Framework.UI.GridUnitType.Pixel);
                this.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);


                _Bg = new Framework.UI.Box();
                _ImgState = new Skill.Framework.UI.Image() { Row = 0, Column = 0 };

                Controls.Add(_Bg);
                Controls.Add(_ImgState);
            }
            protected override void BeginRender()
            {
                _ImgState.Texture = Resources.UITextures.Event;
                base.BeginRender();
            }
        }
        #endregion

        #region AnimationTrackBarContextMenu
        class AnimationTrackBarContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static AnimationTrackBarContextMenu _Instance;
            public static AnimationTrackBarContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new AnimationTrackBarContextMenu();
                    return _Instance;
                }
            }

            private AnimationTrackBarContextMenu()
            {                
                MenuItem mnuCrossFade = new MenuItem("CrossFade") { UserData = typeof(CrossFadeAnimationKey) };
                this.Add(mnuCrossFade);
                
                mnuCrossFade.Click += menuItem_Click;
            }

            void menuItem_Click(object sender, System.EventArgs e)
            {
                AnimationTrackBar trackBar = (AnimationTrackBar)Owner;
                System.Type type = (System.Type)((Skill.Editor.UI.MenuItem)sender).UserData;

                AnimationKey newKey = ScriptableObject.CreateInstance(type) as AnimationKey;
                trackBar.AddKeyAt(Position.x, newKey);
            }
        }
        #endregion


    }
}


