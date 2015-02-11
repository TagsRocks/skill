using UnityEngine;
using System.Collections;
using Skill.Editor.UI;
using System.Collections.Generic;
using Skill.Framework.Sequence;
using System;

namespace Skill.Editor.Sequence
{
    /// <summary>
    /// TrackBar to edit PropertyTrack
    /// </summary>
    public class AnimatorTrackBar : EventOrientedTrackBar
    {
        private AnimatorTrack _AnimatorTrack;

        /// <summary>
        /// Create a AnimatorTrackBar
        /// </summary>
        /// <param name="track"> AnimatorTrackBar to edit</param>
        public AnimatorTrackBar(AnimatorTrack track)
            : base(track)
        {
            _AnimatorTrack = track;
            this.ContextMenu = AnimatorTrackBarContextMenu.Instance;
        }


        protected override EventOrientedKeyView CreateNewEvent(EventOrientedKey key)
        {
            return new AnimatorKeyView(this, (AnimatorKey)key);
        }

        /// <summary>
        /// add key at position
        /// </summary>
        /// <param name="x">position inside track</param>
        private void AddKeyAt(float x, AnimatorKey newKey)
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

        public override void Refresh()
        {
            base.Refresh();
            if (_AnimatorTrack != null && _AnimatorTrack._Animator != null)
            {
                if (_AnimatorTrack._Animator.runtimeAnimatorController != null)
                {
                    UnityEditorInternal.AnimatorController ac = _AnimatorTrack._Animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
                    if (ac != null)
                    {
                        AnimatorTrack.Parameter[] parameters = new AnimatorTrack.Parameter[ac.parameterCount];
                        for (int i = 0; i < ac.parameterCount; i++)
                        {
                            AnimatorTrack.Parameter ap = new AnimatorTrack.Parameter();
                            var p = ac.GetParameter(i);
                            ap.Name = p.name;
                            ap.ParameterType = (Framework.AnimatorParameterType)p.type;

                            switch (p.type)
                            {
                                case UnityEditorInternal.AnimatorControllerParameterType.Bool:
                                    ap.DefaultBool = p.defaultBool;
                                    break;
                                case UnityEditorInternal.AnimatorControllerParameterType.Float:
                                    ap.DefaultFloat = p.defaultFloat;
                                    break;
                                case UnityEditorInternal.AnimatorControllerParameterType.Int:
                                    ap.DefaultInt = p.defaultInt;
                                    break;
                            }

                            parameters[i] = ap;
                        }

                        _AnimatorTrack._Parameters = parameters;
                    }
                    else
                    {
                        _AnimatorTrack._Parameters = new AnimatorTrack.Parameter[0];
                    }
                }
                else
                {
                    _AnimatorTrack._Parameters = new AnimatorTrack.Parameter[0];
                }
                SetDirty();
            }

        }

        #region AnimatorKeyView

        /// <summary>
        /// Visual representation of EventKey
        /// </summary>
        protected class AnimatorKeyView : EventOrientedKeyView
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

            //private AnimatorKey _AnimatorKey;

            public AnimatorKeyView(AnimatorTrackBar trackBar, AnimatorKey key)
                : base(trackBar, key)
            {
                //_AnimatorKey = key;

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

        #region AnimatorTrackBarContextMenu
        class AnimatorTrackBarContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static AnimatorTrackBarContextMenu _Instance;
            public static AnimatorTrackBarContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new AnimatorTrackBarContextMenu();
                    return _Instance;
                }
            }

            private AnimatorTrackBarContextMenu()
            {
                MenuItem mnuBool = new MenuItem("Bool") { UserData = typeof(BooleanAnimatorKey) };
                MenuItem mnuInt = new MenuItem("Int") { UserData = typeof(IntengerAnimatorKey) };
                MenuItem mnuFloat = new MenuItem("Float") { UserData = typeof(FloatAnimatorKey) };
                MenuItem mnuTrigger = new MenuItem("Trigger") { UserData = typeof(TriggerAnimatorKey) };
                MenuItem mnuParameters = new MenuItem("Parameter");

                this.Add(mnuParameters);
                mnuParameters.Add(mnuBool);
                mnuParameters.Add(mnuInt);
                mnuParameters.Add(mnuFloat);
                mnuParameters.Add(mnuTrigger);

                this.AddSeparator();

                MenuItem mnuIkPosition = new MenuItem("Position") { UserData = typeof(IkPositionAnimatorKey) };
                MenuItem mnuIkRotation = new MenuItem("Rotation") { UserData = typeof(IkRotationAnimatorKey) };
                MenuItem mnuIk = new MenuItem("Ik");

                this.Add(mnuIk);
                mnuIk.Add(mnuIkPosition);
                mnuIk.Add(mnuIkRotation);

                this.AddSeparator();

                MenuItem mnuLookAt = new MenuItem("LookAt") { UserData = typeof(LookAtAnimatorKey) };
                MenuItem mnuLayerWeight = new MenuItem("LayerWeight") { UserData = typeof(LayerWeightAnimatorKey) };

                this.Add(mnuLookAt);
                this.Add(mnuLayerWeight);

                MenuItem mnuCrossFade = new MenuItem("CrossFade") { UserData = typeof(CrossFadeAnimatorKey) };
                this.Add(mnuCrossFade);


                mnuBool.Click += menuItem_Click;
                mnuInt.Click += menuItem_Click;
                mnuFloat.Click += menuItem_Click;
                mnuTrigger.Click += menuItem_Click;
                mnuIkPosition.Click += menuItem_Click;
                mnuIkRotation.Click += menuItem_Click;
                mnuLookAt.Click += menuItem_Click;
                mnuLayerWeight.Click += menuItem_Click;
                mnuCrossFade.Click += menuItem_Click;
            }

            void menuItem_Click(object sender, System.EventArgs e)
            {
                AnimatorTrackBar trackBar = (AnimatorTrackBar)Owner;
                System.Type type = (System.Type)((Skill.Editor.UI.MenuItem)sender).UserData;

                AnimatorKey newKey = ScriptableObject.CreateInstance(type) as AnimatorKey;
                trackBar.AddKeyAt(Position.x, newKey);
            }
        }
        #endregion


    }
}


