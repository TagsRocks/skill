﻿using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Sequence
{
    public class QuaternionTrackBar : PropertyTrackBar<Quaternion>
    {
        protected override PropertyTrackBar<Quaternion>.PropertyTimeLineEvent CreateNewEvent(IPropertyKey<Quaternion> key) { return new QuaternionKeyView(this, key); }
        protected override IPropertyKey<Quaternion> CreateNewKey() { return new QuaternionKey(); }
        protected override IPropertyKey<Quaternion>[] CreateKeyArray(int arraySize) { return new QuaternionKey[arraySize]; }

        protected override bool IsCurve { get { return true; } }

        public QuaternionTrackBar(QuaternionTrack track)
            : base(track)
        {
            this.Height = 40;
        }

        internal override void Evaluate(float time)
        {
            Seek(time);
        }

        internal override void Seek(float time)
        {
            if ((this.RecordState & RecordState.XYZ) != RecordState.XYZ)
            {
                QuaternionTrack qt = (QuaternionTrack)Track;
                QuaternionKey vk = (QuaternionKey)qt.PropertyKeys[0];


                Vector3 curveValue = new Vector3(vk.CurveX.Evaluate(time), vk.CurveY.Evaluate(time), vk.CurveZ.Evaluate(time));
                Vector3 sceneValue = curveValue;
                object v = qt.GetValue();
                if (v != null)
                    sceneValue = ((Quaternion)v).eulerAngles;

                Vector3 value = curveValue;
                if ((this.RecordState & RecordState.X) != 0) value.x = sceneValue.x;
                if ((this.RecordState & RecordState.Y) != 0) value.y = sceneValue.y;
                if ((this.RecordState & RecordState.Z) != 0) value.z = sceneValue.z;

                qt.SetValue(Quaternion.Euler(value));
            }
        }

        protected override void EvaluateNewKey(IPropertyKey<Quaternion> newKey, IPropertyKey<Quaternion> previousKey)
        {
            if (previousKey != null)
                newKey.ValueKey = previousKey.ValueKey;
            else
                newKey.ValueKey = ((PropertyTrack<Quaternion>)Track).DefaultValue;

            QuaternionKey newfKey = (QuaternionKey)newKey;
            if (previousKey != null)
            {
                QuaternionKey prefKey = (QuaternionKey)previousKey;

                newfKey.CurveX = CreateNextCurve(prefKey.CurveX, ((QuaternionTrack)Track).DefaultValue.x);
                newfKey.CurveY = CreateNextCurve(prefKey.CurveY, ((QuaternionTrack)Track).DefaultValue.y);
                newfKey.CurveZ = CreateNextCurve(prefKey.CurveZ, ((QuaternionTrack)Track).DefaultValue.z);
            }
            else
            {
                newfKey.CurveX = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.x });
                newfKey.CurveY = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.y });
                newfKey.CurveZ = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.z });
            }
        }

        class QuaternionKeyView : PropertyTimeLineEvent
        {
            public override double Duration
            {
                get
                {
                    if (_QuaternionKey.CurveX != null && _QuaternionKey.CurveY != null && _QuaternionKey.CurveZ != null)
                    {
                        float max = 0;
                        if (_QuaternionKey.CurveX.length > 0)
                            max = Mathf.Max(max, _QuaternionKey.CurveX.keys[_QuaternionKey.CurveX.length - 1].time);
                        if (_QuaternionKey.CurveY.length > 0)
                            max = Mathf.Max(max, _QuaternionKey.CurveY.keys[_QuaternionKey.CurveY.length - 1].time);
                        if (_QuaternionKey.CurveZ.length > 0)
                            max = Mathf.Max(max, _QuaternionKey.CurveZ.keys[_QuaternionKey.CurveZ.length - 1].time);
                        return max;
                    }
                    else
                        return 0.05f;
                }
                set { }
            }
            public override string Title { get { return "Quaternion Event"; } }

            protected override bool CanDrag { get { return false; } }

            private float _MinWidth;
            private Quaternion _PreValue;
            public override float MinWidth
            {
                get
                {
                    if (_QuaternionKey.CurveX != null && _QuaternionKey.CurveY != null && _QuaternionKey.CurveZ != null)
                        return 22;
                    else
                    {
                        if (_MinWidth < 1f || _PreValue != _QuaternionKey.Value)
                        {
                            _PreValue = _QuaternionKey.Value;
                            GUIStyle labelStyle = "Label";
                            GUIContent content = new GUIContent() { text = _QuaternionKey.Value.ToString() };
                            _MinWidth = labelStyle.CalcSize(content).x;
                        }
                        return _MinWidth;
                    }
                }
            }
            protected override PropertiesPanel CreateProperties() { return new QuaternionKeyViewProperties(this); }

            private QuaternionKey _QuaternionKey;
            public QuaternionKeyView(QuaternionTrackBar trackbar, IPropertyKey<Quaternion> key)
                : base(trackbar, key)
            {
                _QuaternionKey = (QuaternionKey)key;
            }


            private Rect[] _CurevRenderAreas = new Rect[3];
            private Rect[] _CurevRanges = new Rect[3];
            protected override void Render()
            {
                base.Render();
                if (_QuaternionKey.CurveX != null && _QuaternionKey.CurveY != null && _QuaternionKey.CurveZ != null)
                {
                    CalcCurveRenderArea(ref _CurevRenderAreas, ref _CurevRanges, _QuaternionKey.CurveX, _QuaternionKey.CurveY, _QuaternionKey.CurveZ);
                    if (_CurevRenderAreas[0].width > 0.1f)
                        DrawCurve(_CurevRenderAreas[0], _CurevRanges[0], _QuaternionKey.CurveX, CurveXColor);
                    if (_CurevRenderAreas[1].width > 0.1f)
                        DrawCurve(_CurevRenderAreas[1], _CurevRanges[1], _QuaternionKey.CurveY, CurveYColor);
                    if (_CurevRenderAreas[2].width > 0.1f)
                        DrawCurve(_CurevRenderAreas[2], _CurevRanges[2], _QuaternionKey.CurveZ, CurveZColor);
                }
                //else
                //{
                //    GUI.Label(RenderArea, PropertyKey.ValueKey.ToString());
                //}
                
            }

            class QuaternionKeyViewProperties : EventProperties
            {
                private Skill.Editor.UI.Vector3Field _Value;
                private Skill.Editor.UI.CurveField _CurveFieldX;
                private Skill.Editor.UI.CurveField _CurveFieldY;
                private Skill.Editor.UI.CurveField _CurveFieldZ;
                private Skill.Editor.UI.ChangeCheck _ChangeCheck;

                public QuaternionKeyViewProperties(QuaternionKeyView e)
                    : base(e)
                {
                    _Value = new Skill.Editor.UI.Vector3Field() { Margin = ControlMargin };
                    _Value.Label = "Value";

                    _CurveFieldX = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 0, Color = Color.red, UseColor = true }; _CurveFieldX.Label.text = "Curve X";
                    _CurveFieldY = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 1, Color = Color.green, UseColor = true }; _CurveFieldY.Label.text = "Curve Y";
                    _CurveFieldZ = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 2, Color = Color.blue, UseColor = true }; _CurveFieldZ.Label.text = "Curve Z";

                    _ChangeCheck = new Skill.Editor.UI.ChangeCheck() { Height = 66 };
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.Controls.Add(_CurveFieldX);
                    _ChangeCheck.Controls.Add(_CurveFieldY);
                    _ChangeCheck.Controls.Add(_CurveFieldZ);

                    Controls.Add(_Value);
                    Controls.Add(_ChangeCheck);

                    _Value.ValueChanged += _FFValue_ValueChanged;
                    _ChangeCheck.Changed += ChangeCheck_Changed;
                }

                void ChangeCheck_Changed(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    SetDirty();
                }

                void _FFValue_ValueChanged(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    ((QuaternionKeyView)_View).PropertyKey.ValueKey = Quaternion.Euler(_Value.Value);
                    SetDirty();
                }

                protected override void RefreshData()
                {
                    base.RefreshData();
                    _Value.Value = ((QuaternionKeyView)_View).PropertyKey.ValueKey.eulerAngles;
                    ValidateCurves();
                }

                void ValidateCurves()
                {
                    _Value.Visibility = Skill.Framework.UI.Visibility.Collapsed;
                    _ChangeCheck.Visibility = Skill.Framework.UI.Visibility.Visible;

                    QuaternionKey k = (QuaternionKey)((QuaternionKeyView)_View).Key;
                    if (k.CurveX != null) _CurveFieldX.Curve = k.CurveX;
                    if (k.CurveY != null) _CurveFieldY.Curve = k.CurveY;
                    if (k.CurveZ != null) _CurveFieldZ.Curve = k.CurveZ;

                    if (_CurveFieldX.Curve == null) _CurveFieldX.Curve = new AnimationCurve();
                    if (_CurveFieldY.Curve == null) _CurveFieldY.Curve = new AnimationCurve();
                    if (_CurveFieldZ.Curve == null) _CurveFieldZ.Curve = new AnimationCurve();

                    k.CurveX = _CurveFieldX.Curve;
                    k.CurveY = _CurveFieldY.Curve;
                    k.CurveZ = _CurveFieldZ.Curve;
                }
            }
        }


    }
}