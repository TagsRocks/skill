﻿using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using System.Collections.Generic;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    /// <summary>
    /// TrackBar to edit PropertyTrack
    /// </summary>
    public abstract class PropertyTrackBar<V> : BaseTrackBar
    {
        private List<PropertyTimeLineEvent> _Events;
        private PropertyTrack<V> _PropertyTrack;


        public override ITrackKey FirstKey { get { return _PropertyTrack.PropertyKeys[0]; } }
        protected virtual bool IsCurve { get { return false; } }

        /// <summary>
        /// Create a SoundTrackBar
        /// </summary>
        /// <param name="track"> SoundTrack to edit</param>
        public PropertyTrackBar(PropertyTrack<V> track)
            : base(track)
        {
            _PropertyTrack = track;
            _Events = new List<PropertyTimeLineEvent>();
            CreateEvents();
            this.ContextMenu = PropertyTrackBarContextMenu.Instance;
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
            if (_PropertyTrack.PropertyKeys != null)
            {
                // search for new events in SoundTrack that we didn't create SoundEvent for them 
                foreach (var key in _PropertyTrack.PropertyKeys)
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
                    if (!IsKeyExistInPropertyTrack(e.PropertyKey))
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
        private PropertyTimeLineEvent CreateEvent(IPropertyKey<V> key)
        {
            PropertyTimeLineEvent se = CreateNewEvent(key);
            se.ContextMenu = PropertyEventContextMenu.Instance;
            this.Controls.Add(se);
            this._Events.Add(se);
            return se;
        }

        protected abstract PropertyTimeLineEvent CreateNewEvent(IPropertyKey<V> key);
        protected abstract IPropertyKey<V> CreateNewKey();
        protected abstract IPropertyKey<V>[] CreateKeyArray(int arraySize);

        private bool IsEventExistWithKey(IPropertyKey<V> key)
        {
            foreach (var e in _Events)
                if (e.Key == key) return true;
            return false;
        }
        private bool IsKeyExistInPropertyTrack(IPropertyKey<V> key)
        {
            foreach (var k in _PropertyTrack.PropertyKeys)
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

                IPropertyKey<V> newKey = CreateNewKey();
                newKey.FireTime = (float)timeLine.TimeBar.GetTime(x);

                PropertyTimeLineEvent e = CreateEvent(newKey);
                RebuildTrackKeys();

                for (int i = 0; i < _PropertyTrack.PropertyKeys.Length; i++)
                {
                    if (_PropertyTrack.PropertyKeys[i] == newKey)
                    {
                        if (i > 0)
                            EvaluateNewKey(newKey, _PropertyTrack.PropertyKeys[i - 1]);
                        else
                            EvaluateNewKey(newKey, null);
                        break;
                    }
                }

                if (MatineeEditorWindow.Instance != null)
                    MatineeEditorWindow.Instance.PropertyGrid.SelectedObject = e;
            }
        }

        protected abstract void EvaluateNewKey(IPropertyKey<V> newKey, IPropertyKey<V> previousKey);
        protected void EditCurves()
        {
            MatineeEditorWindow.Instance.EditCurve(this);
        }
        protected static AnimationCurve CreateNextCurve(AnimationCurve preCurve, float defaultValue)
        {
            Keyframe k1;
            if (preCurve == null || preCurve.length == 0)
                k1 = new Keyframe() { value = defaultValue };
            else
                k1 = preCurve.keys[preCurve.length - 1];
            k1.time = 0;
            return CreateNextCurve(k1);
        }
        protected static AnimationCurve CreateNextCurve(Keyframe k1)
        {
            Keyframe k2 = k1;
            k2.time = 0.1f;
            return new AnimationCurve(new Keyframe[] { k1, k2 });
        }

        /// <summary>
        /// Delete SoundEvent 
        /// </summary>
        /// <param name="soundEvent">SoundEvent to delete</param>
        private void Delete(PropertyTimeLineEvent e)
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
            var keys = CreateKeyArray(_Events.Count);
            for (int i = 0; i < _Events.Count; i++)
                keys[i] = _Events[i].PropertyKey;
            _PropertyTrack.PropertyKeys = keys;
            _PropertyTrack.SortKeys();
            SetDirty();
        }

        #region PropertyEvent

        /// <summary>
        /// Visual representation of SoundKey
        /// </summary>
        protected abstract class PropertyTimeLineEvent : EventView
        {


            public override float MinWidth { get { return 6; } }

            public IPropertyKey<V> PropertyKey { get; private set; }

            public PropertyTimeLineEvent(PropertyTrackBar<V> trackBar, IPropertyKey<V> key)
                : base(trackBar, key)
            {
                PropertyKey = key;
            }


            protected void CalcCurveRenderArea(ref Rect[] renderAreas, ref Rect[] ranges, params AnimationCurve[] curves)
            {
                TimeLine timeLine = FindInParents<TimeLine>();
                Rect trackRa = TrackBar.RenderArea;
                double deltaTime = (timeLine.MaxTime - timeLine.MinTime);

                float minVisibleX = trackRa.x + trackRa.width * (float)((timeLine.StartVisible - timeLine.MinTime) / deltaTime);
                float maxVisibleX = trackRa.x + trackRa.width * (float)((timeLine.EndVisible - timeLine.MinTime) / deltaTime);


                float maxTime = 0;
                foreach (AnimationCurve curve in curves)
                {
                    if (curve != null && curve.length > 1)
                        maxTime = Mathf.Max(maxTime, curve[curve.length - 1].time);
                }

                for (int i = 0; i < curves.Length; i++)
                {
                    AnimationCurve curve = curves[i];
                    Rect ra = RenderArea;
                    if (curve != null && curve.length > 1 && maxTime > 0.01f)
                    {
                        ra.x += curve[0].time / maxTime * ra.width;
                        ra.width = curve[curve.length - 1].time / maxTime * ra.width;
                    }

                    float xMin = Mathf.Max(ra.xMin, minVisibleX);
                    float xMax = Mathf.Min(ra.xMax, maxVisibleX);
                    float delta = xMax - xMin;
                    Rect range = new Rect(0, 0, 1, 1);
                    range.xMin = (xMin - ra.xMin) / ra.width;
                    range.xMax = (xMax - ra.xMin) / ra.width;
                    ra.xMin = xMin;
                    ra.xMax = xMax;

                    if (curve != null && curve.length > 1)
                    {
                        for (int j = 0; j < curve.length; j++)
                        {
                            float value = curve[j].value;
                            range.yMin = Mathf.Min(range.yMin, value);
                            range.yMax = Mathf.Max(range.yMax, value);
                        }
                    }

                    renderAreas[i] = ra;                    
                    ranges[i] = range;
                }
            }

            protected static void DrawCurve(Rect ra, Rect ranges, AnimationCurve curve, Color color)
            {
                if (curve != null && curve.length > 1 && ra.xMax - ra.xMin >= 1)
                {
                    float time = curve[curve.length - 1].time;
                    ranges.x *= time;
                    ranges.width *= time;
                    UnityEditor.EditorGUIUtility.DrawCurveSwatch(ra, curve, null, color, CurveBgColor, ranges);
                }
            }
        }
        #endregion

        #region PropertyEventContextMenu
        class PropertyEventContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static PropertyEventContextMenu _Instance;
            public static PropertyEventContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new PropertyEventContextMenu();
                    return _Instance;
                }
            }

            private Skill.Editor.UI.MenuItem _DeleteItem;
            private Skill.Editor.UI.MenuItem _EditCurves;

            public PropertyEventContextMenu()
            {
                _DeleteItem = new Skill.Editor.UI.MenuItem("Delete");
                _EditCurves = new Skill.Editor.UI.MenuItem("Edit Curves");

                _DeleteItem.Click += _DeleteItem_Click;
                _EditCurves.Click += _EditCurves_Click;
            }

            void _EditCurves_Click(object sender, System.EventArgs e)
            {
                PropertyTimeLineEvent se = (PropertyTimeLineEvent)Owner;
                ((PropertyTrackBar<V>)se.TrackBar).EditCurves();
            }

            void _DeleteItem_Click(object sender, System.EventArgs e)
            {
                PropertyTimeLineEvent se = (PropertyTimeLineEvent)Owner;
                ((PropertyTrackBar<V>)se.TrackBar).Delete(se);
            }
            protected override void BeginShow()
            {
                Clear();
                PropertyTimeLineEvent se = (PropertyTimeLineEvent)Owner;
                PropertyTrackBar<V> p = (PropertyTrackBar<V>)se.TrackBar;
                if (p != null)
                {
                    if (p.IsCurve)
                        Add(_EditCurves);
                    else
                        Add(_DeleteItem);
                }
                base.BeginShow();
            }
        }
        #endregion

        #region PropertyTrackBarContextMenu
        class PropertyTrackBarContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static PropertyTrackBarContextMenu _Instance;
            public static PropertyTrackBarContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new PropertyTrackBarContextMenu();
                    return _Instance;
                }
            }

            private Skill.Editor.UI.MenuItem _AddItem;
            private Skill.Editor.UI.MenuItem _EditCurves;

            private PropertyTrackBarContextMenu()
            {
                _AddItem = new Skill.Editor.UI.MenuItem("New");
                _EditCurves = new Skill.Editor.UI.MenuItem("Edit Curves");


                _AddItem.Click += _AddItem_Click;
                _EditCurves.Click += _EditCurves_Click;
            }

            void _EditCurves_Click(object sender, System.EventArgs e)
            {
                PropertyTrackBar<V> trackBar = (PropertyTrackBar<V>)Owner;
                trackBar.EditCurves();
            }

            void _AddItem_Click(object sender, System.EventArgs e)
            {
                PropertyTrackBar<V> trackBar = (PropertyTrackBar<V>)Owner;
                trackBar.AddKeyAtPosition(Position.x);
            }

            protected override void BeginShow()
            {
                Clear();
                PropertyTrackBar<V> p = Owner as PropertyTrackBar<V>;
                if (p != null)
                {
                    if (p.IsCurve)
                        Add(_EditCurves);
                    else
                        Add(_AddItem);
                }
                base.BeginShow();
            }
        }
        #endregion




    }
}