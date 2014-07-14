﻿using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using System.Collections.Generic;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public enum KeyType
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        W = 8,
        All = X | Y | Z | W
    }
    public abstract class ContinuousTrackBar<V> : PropertyTrackBar<V>
    {
        private static Color[] CurveColors = new Color[] { new Color(1.0f, 0.0f, 0.0f), new Color(0.0f, 1.0f, 0.0f), new Color(0.0f, 0.0f, 1.0f), new Color(1.0f, 1.0f, 0.0f) };

        public abstract int CurveCount { get; }
        public abstract AnimationCurve GetCurve(int index);

        public override bool IsContinuous { get { return true; } }

        private CurveKeyView[] _CurveKeyViews;
        private ContinuousTrack<V> _ContinuousTrack;

        /// <summary>
        /// Create a DiscreteTrackBar
        /// </summary>
        /// <param name="track"> DiscreteTrack to edit</param>
        public ContinuousTrackBar(ContinuousTrack<V> track)
            : base(track)
        {
            this.Height = CurveCount * BaseHeight;
            _ContinuousTrack = track;
            _CurveKeyViews = new CurveKeyView[CurveCount];
            for (int i = 0; i < CurveCount; i++)
                _CurveKeyViews[i] = new CurveKeyView(this, i);

            this.ContextMenu = ContinuousTrackBarContextMenu.Instance;
        }
        public override void Refresh()
        {
            base.Refresh();
            for (int i = 0; i < _CurveKeyViews.Length; i++)
                _CurveKeyViews[i].Refresh(GetCurve(i));
        }

        protected static void ValidateCurve(AnimationCurve curve, float value)
        {
            if (curve.length < 2)
            {
                if (curve.length < 1)
                    curve.AddKey(0.0f, value);
                else
                    curve.MoveKey(0, new Keyframe() { time = 0.0f, value = value });

                if (curve.length < 2)
                    curve.AddKey(0.1f, value);
                else
                    curve.MoveKey(1, new Keyframe() { time = 0.1f, value = value });
            }
        }

        private void Delete(ContinuousKeyView keyView)
        {
            if (_CurveKeyViews[keyView.CurveIndex].Remove(keyView))
            {
                keyView.Curve.RemoveKey(keyView.KeyIndex);
                SetDirty();
                Invalidate();
            }
        }

        internal override void AddCurveKey(KeyType keyType)
        {
            if (CurveCount > 0)
            {
                TimeLine timeLine = FindInParents<TimeLine>();
                if (timeLine != null)
                {
                    float time = (float)timeLine.TimePosition;
                    AddCurveKey(keyType, time);
                    RefreshCurveKeyView(keyType);
                }
            }
        }
        internal void AddCurveKeyAtPosition(KeyType keyType, float x)
        {
            TimeLine timeLine = FindInParents<TimeLine>();
            if (timeLine != null)
            {
                x -= timeLine.View.ScrollPosition.x;
                float time = (float)timeLine.TimeBar.GetTime(x);
                AddCurveKey(keyType, time);
                RefreshCurveKeyView(keyType);
            }
        }

        private void RefreshCurveKeyView(KeyType keyType)
        {
            if ((CurveCount > 0) && (keyType & KeyType.X) == KeyType.X) _CurveKeyViews[0].Refresh(GetCurve(0));
            if ((CurveCount > 1) && (keyType & KeyType.Y) == KeyType.Y) _CurveKeyViews[1].Refresh(GetCurve(1));
            if ((CurveCount > 2) && (keyType & KeyType.Z) == KeyType.Z) _CurveKeyViews[2].Refresh(GetCurve(2));
            if ((CurveCount > 3) && (keyType & KeyType.W) == KeyType.W) _CurveKeyViews[3].Refresh(GetCurve(3));
        }
        protected abstract void AddCurveKey(KeyType keyType, float time);
        protected void AddKeyToCurve(AnimationCurve curve, float time, float value)
        {
            bool found = false;
            for (int i = 0; i < curve.length; i++)
            {
                if (Mathf.Abs(curve[i].time - time) < 0.005f)
                {
                    Keyframe keyframe = curve[i];
                    keyframe.value = value;
                    curve.MoveKey(i, keyframe);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                int keyIndex = curve.AddKey(time, value);
                Skill.Editor.CurveUtility.SetKeyModeFromContext(curve, keyIndex);
                Skill.Editor.CurveUtility.UpdateTangentsFromModeSurrounding(curve, keyIndex);
            }
            SetDirty();
        }


        private bool _Recording;
        private V _SaveState;
        public override void SaveRecordState()
        {
            object value = _ContinuousTrack.GetValue();
            if (value != null)
            {
                _SaveState = (V)value;
                _Recording = true;
            }
            else
                _Recording = false;
        }
        public override void AutoKey()
        {
            if (_Recording)
            {
                object value = _ContinuousTrack.GetValue();
                if (value != null)
                {
                    V newState = (V)value;
                    KeyType notEquality = ~GetEquality(newState, _SaveState);

                    if (notEquality != KeyType.None)
                        AddCurveKey(notEquality);

                }
            }
        }

        protected abstract KeyType GetEquality(V v1, V v2);


        #region CurveKeyView
        class CurveKeyView
        {
            private List<ContinuousKeyView> _Keys;
            private AnimationCurve _Curve;
            private ContinuousTrackBar<V> _TrackBar;
            private int _CurveIndex;

            public CurveKeyView(ContinuousTrackBar<V> trackBar, int curveIndex)
            {
                this._Keys = new List<ContinuousKeyView>();
                this._TrackBar = trackBar;
                this._CurveIndex = curveIndex;
            }

            public void Refresh(AnimationCurve curve)
            {
                _Curve = curve;
                Refresh();
            }
            private void Refresh()
            {
                if (_Curve != null && _Curve.length > 0)
                {
                    if (_Curve.length > _Keys.Count)
                    {
                        int index = _Keys.Count;
                        while (index < _Curve.length)
                        {
                            CreateKeyView(index);
                            index++;
                        }
                    }
                    else if (_Keys.Count > _Curve.length)
                    {
                        int index = _Keys.Count - 1;
                        while (index >= _Curve.length)
                        {
                            _TrackBar.Controls.Remove(_Keys[index]);
                            _Keys.RemoveAt(index);
                            index--;
                        }
                    }

                    for (int i = 0; i < _Keys.Count; i++)
                        _Keys[i].KeyIndex = i;
                }
                else
                {
                    foreach (var k in _Keys)
                        _TrackBar.Controls.Remove(k);
                    _Keys.Clear();

                }
            }

            private void CreateKeyView(int keyIndex)
            {
                ContinuousKeyView key = new ContinuousKeyView(_TrackBar, _Curve, _CurveIndex, keyIndex);
                key.ContextMenu = ContinuousKeyViewContextMenu.Instance;
                _TrackBar.Controls.Add(key);
                _Keys.Add(key);
            }

            public bool Remove(ContinuousKeyView keyView)
            {
                if (_Keys.Remove(keyView))
                {
                    _TrackBar.Controls.Remove(keyView);
                    Refresh();
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region ContinuousKeyView

        /// <summary>
        /// Visual representation of ContinuousKey
        /// </summary>
        protected class ContinuousKeyView : KeyView
        {
            public override double Duration { get { return 0.1f; } set { } }
            public override float MinWidth { get { return 10; } }
            public override float MaxWidth { get { return 10; } }
            public override float Height { get { return BaseHeight; } set { base.Height = value; } }
            public override string Title { get { return "Keyframe"; } }
            public AnimationCurve Curve { get; private set; }
            public int KeyIndex { get; set; }

            public override float CenterOffset { get { return 5; } }

            public int CurveIndex { get; private set; }
            public Keyframe Keyframe
            {
                get { return Curve[Mathf.Clamp(KeyIndex, 0, Curve.length - 1)]; }
                set
                {
                    Curve.MoveKey(KeyIndex, value);
                    OnLayoutChanged();
                }
            }
            private double ClampTime(double time)
            {
                if (KeyIndex > 0)
                {
                    if (KeyIndex == 1 || KeyIndex == Curve.length - 1)
                        time = System.Math.Max(time, Curve[KeyIndex - 1].time + 0.01);
                    else
                        time = System.Math.Max(time, Curve[KeyIndex - 1].time);
                }
                if (KeyIndex < Curve.length - 1)
                {
                    if (KeyIndex == 0 || KeyIndex == Curve.length - 2)
                        time = System.Math.Min(time, Curve[KeyIndex + 1].time - 0.01);
                    else
                        time = System.Math.Min(time, Curve[KeyIndex + 1].time);
                }
                return time;
            }
            public override double FireTime
            {
                get { return Keyframe.time; }
                set
                {
                    Keyframe k = this.Keyframe;
                    k.time = (float)ClampTime(value);
                    this.Keyframe = k;
                    Properties.Refresh();
                }
            }

            public IPropertyKey<V> PropertyKey { get; private set; }
            private Skill.Framework.UI.Image _ImgIcon;

            public ContinuousKeyView(ContinuousTrackBar<V> trackBar, AnimationCurve curve, int curveIndex, int keyIndex)
                : base(trackBar)
            {
                this.Curve = curve;
                this.CurveIndex = curveIndex;
                this.KeyIndex = keyIndex;
                this._ImgIcon = new Skill.Framework.UI.Image() { TintColor = CurveColors[curveIndex], Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10, HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Center, VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center, Width = 10, Height = 10 };
                this.Controls.Add(_ImgIcon);
            }

            protected override void BeginRender()
            {
                _ImgIcon.Texture = Resources.UITextures.Keyframe;
                base.BeginRender();
            }

            private bool _Ignore;
            protected override void OnRenderAreaChanged()
            {
                if (_Ignore) return;
                Rect ra = RenderArea;
                ra.y += CurveIndex * BaseHeight;
                ra.height = BaseHeight;
                _Ignore = true;
                RenderArea = ra;
                _Ignore = false;
                base.OnRenderAreaChanged();
            }


            private ContinuousKeyViewProperties _Properties;
            /// <summary> Properties </summary>
            public override PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new ContinuousKeyViewProperties(this);
                    return _Properties;
                }
            }


            [Skill.Framework.ExposeProperty(0, "Time")]
            public float Time { get { return (float)FireTime; } set { FireTime = value; } }

            [Skill.Framework.ExposeProperty(1, "Value")]
            public float Value
            {
                get { return Keyframe.value; }
                set
                {
                    Keyframe k = this.Keyframe;
                    k.value = value;
                    this.Keyframe = k;
                }
            }

            public class ContinuousKeyViewProperties : ExposeProperties
            {
                protected ContinuousKeyView _View;
                public ContinuousKeyViewProperties(ContinuousKeyView view)
                    : base(view)
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


        #region ContinuousKeyViewContextMenu
        class ContinuousKeyViewContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static ContinuousKeyViewContextMenu _Instance;
            public static ContinuousKeyViewContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new ContinuousKeyViewContextMenu();
                    return _Instance;
                }
            }

            private Skill.Editor.UI.MenuItem _DeleteItem;

            public ContinuousKeyViewContextMenu()
            {
                _DeleteItem = new Skill.Editor.UI.MenuItem("Delete");
                Add(_DeleteItem);
                _DeleteItem.Click += _DeleteItem_Click;
            }

            void _DeleteItem_Click(object sender, System.EventArgs e)
            {
                ContinuousKeyView se = (ContinuousKeyView)Owner;
                ((ContinuousTrackBar<V>)se.TrackBar).Delete(se);
            }
        }
        #endregion

        #region ContinuousTrackBarContextMenu
        class ContinuousTrackBarContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static ContinuousTrackBarContextMenu _Instance;
            public static ContinuousTrackBarContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new ContinuousTrackBarContextMenu();
                    return _Instance;
                }
            }

            private Skill.Editor.UI.MenuItem _AddKeyXItem;
            private Skill.Editor.UI.MenuItem _AddKeyYItem;
            private Skill.Editor.UI.MenuItem _AddKeyZItem;
            private Skill.Editor.UI.MenuItem _AddKeyWItem;
            private Skill.Editor.UI.MenuItem _AddKeyAllItem;

            private ContinuousTrackBarContextMenu()
            {
                _AddKeyXItem = new Skill.Editor.UI.MenuItem("Add key X");
                _AddKeyYItem = new Skill.Editor.UI.MenuItem("Add key Y");
                _AddKeyZItem = new Skill.Editor.UI.MenuItem("Add key Z");
                _AddKeyWItem = new Skill.Editor.UI.MenuItem("Add key W");
                _AddKeyAllItem = new Skill.Editor.UI.MenuItem("Add key All");

                Add(_AddKeyXItem);
                Add(_AddKeyYItem);
                Add(_AddKeyZItem);
                Add(_AddKeyWItem);
                Add(_AddKeyAllItem);

                _AddKeyXItem.Click += _AddKeyXItem_Click;
                _AddKeyYItem.Click += _AddKeyYItem_Click;
                _AddKeyZItem.Click += _AddKeyZItem_Click;
                _AddKeyWItem.Click += _AddKeyWItem_Click;
                _AddKeyAllItem.Click += _AddKeyAllItem_Click;
            }

            void _AddKeyAllItem_Click(object sender, System.EventArgs e)
            {
                ContinuousTrackBar<V> trackBar = (ContinuousTrackBar<V>)Owner;
                trackBar.AddCurveKeyAtPosition(KeyType.All, Position.x);
            }

            void _AddKeyWItem_Click(object sender, System.EventArgs e)
            {
                ContinuousTrackBar<V> trackBar = (ContinuousTrackBar<V>)Owner;
                trackBar.AddCurveKeyAtPosition(KeyType.W, Position.x);
            }

            void _AddKeyZItem_Click(object sender, System.EventArgs e)
            {
                ContinuousTrackBar<V> trackBar = (ContinuousTrackBar<V>)Owner;
                trackBar.AddCurveKeyAtPosition(KeyType.Z, Position.x);
            }

            void _AddKeyYItem_Click(object sender, System.EventArgs e)
            {
                ContinuousTrackBar<V> trackBar = (ContinuousTrackBar<V>)Owner;
                trackBar.AddCurveKeyAtPosition(KeyType.Y, Position.x);
            }

            void _AddKeyXItem_Click(object sender, System.EventArgs e)
            {
                ContinuousTrackBar<V> trackBar = (ContinuousTrackBar<V>)Owner;
                trackBar.AddCurveKeyAtPosition(KeyType.X, Position.x);
            }

            protected override void BeginShow()
            {
                ContinuousTrackBar<V> trackBar = (ContinuousTrackBar<V>)Owner;
                _AddKeyXItem.IsVisible = trackBar.CurveCount > 0;
                _AddKeyYItem.IsVisible = trackBar.CurveCount > 1;
                _AddKeyZItem.IsVisible = trackBar.CurveCount > 2;
                _AddKeyWItem.IsVisible = trackBar.CurveCount > 3;
                _AddKeyAllItem.IsVisible = trackBar.CurveCount > 1;
                base.BeginShow();
            }
        }
        #endregion


    }
}
