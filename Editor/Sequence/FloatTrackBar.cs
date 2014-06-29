using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Sequence
{
    public class FloatTrackBar : PropertyTrackBar<float>
    {
        protected override PropertyTrackBar<float>.PropertyTimeLineEvent CreateNewEvent(IPropertyKey<float> key) { return new FloatKeyView(this, key); }
        protected override IPropertyKey<float> CreateNewKey() { return new FloatKey(); }
        protected override IPropertyKey<float>[] CreateKeyArray(int arraySize) { return new FloatKey[arraySize]; }

        public FloatTrackBar(FloatTrack track)
            : base(track)
        {
            this.Height = 40;
        }

        protected override void EvaluateNewKey(IPropertyKey<float> newKey, IPropertyKey<float> previousKey)
        {
            if (previousKey != null)
                newKey.ValueKey = previousKey.ValueKey;
            else
                newKey.ValueKey = ((PropertyTrack<float>)Track).DefaultValue;

            FloatKey newfKey = (FloatKey)newKey;
            if (previousKey != null)
            {

                FloatKey prefKey = (FloatKey)previousKey;
                newfKey.Curve = CreateNextCurve(prefKey.Curve, ((FloatTrack)Track).DefaultValue);
            }
            else
            {
                newfKey.Curve = CreateNextCurve(new Keyframe() { value = newKey.ValueKey });
            }

        }

        protected override bool IsCurve { get { return true; } }

        internal override void AddKey(RecordState record)
        {
            if (record == RecordState.X)
            {
                TimeLine timeLine = FindInParents<TimeLine>();
                if (timeLine != null)
                {
                    float time = (float)timeLine.TimePosition;
                    float newValue;
                    FloatTrack ft = (FloatTrack)Track;
                    FloatKey fk = (FloatKey)ft.PropertyKeys[0];

                    if ((this.RecordState & RecordState.X) == RecordState.X)
                    {
                        object v = ft.GetValue();
                        if (v == null)
                            v = fk.Curve.Evaluate(time);
                        newValue = (float)v;
                    }
                    else
                    {
                        newValue = fk.Curve.Evaluate(time);
                    }

                    for (int i = 0; i < fk.Curve.length; i++)
                    {
                        if (Mathf.Approximately(fk.Curve[i].time, time))
                        {
                            Keyframe keyframe = fk.Curve[i];
                            keyframe.value = newValue;
                            fk.Curve.MoveKey(i, keyframe);
                            SetDirty();
                            return;
                        }
                    }

                    int keyIndex = fk.Curve.AddKey(time, newValue);
                    Skill.Editor.CurveUtility.SetKeyModeFromContext(fk.Curve, keyIndex);
                    Skill.Editor.CurveUtility.UpdateTangentsFromModeSurrounding(fk.Curve, keyIndex);
                    SetDirty();
                }
            }
        }

        internal override void Evaluate(float time)
        {
            Seek(time);
        }

        internal override void Seek(float time)
        {
            if ((this.RecordState & RecordState.X) == 0)
            {
                FloatTrack ft = (FloatTrack)Track;
                FloatKey fk = (FloatKey)ft.PropertyKeys[0];
                float value = fk.Curve.Evaluate(time);
                ft.SetValue(value);
            }
        }


        class FloatKeyView : PropertyTimeLineEvent
        {

            protected override bool CanDrag { get { return false; } }
            public override double Duration
            {
                get
                {
                    if (_FloatKey.Curve != null && _FloatKey.Curve.length > 0)
                        return _FloatKey.Curve.keys[_FloatKey.Curve.length - 1].time;
                    else
                        return 0.05f;
                }
                set { }
            }
            public override string Title { get { return "Float Event"; } }

            protected override PropertiesPanel CreateProperties() { return new FloatKeyViewProperties(this); }

            private float _MinWidth;
            private float _PreValue;
            public override float MinWidth
            {
                get
                {
                    if (_FloatKey.Curve != null && _FloatKey.Curve.length > 0)
                        return 22;
                    else
                    {
                        if (_MinWidth < 1f || _PreValue != _FloatKey.Value)
                        {
                            _PreValue = _FloatKey.Value;
                            GUIStyle labelStyle = "Label";
                            GUIContent content = new GUIContent() { text = _FloatKey.Value.ToString() };
                            _MinWidth = labelStyle.CalcSize(content).x;
                        }
                        return _MinWidth;
                    }
                }
            }


            private FloatKey _FloatKey;
            public FloatKeyView(FloatTrackBar trackbar, IPropertyKey<float> key)
                : base(trackbar, key)
            {
                _FloatKey = (FloatKey)key;
            }

            private Rect[] _CurevRenderAreas = new Rect[1];
            private Rect[] _CurevRanges = new Rect[1];
            protected override void Render()
            {
                GUI.Box(RenderArea, string.Empty, Skill.Editor.Resources.Styles.BackgroundShadow);
                if (_FloatKey.Curve != null)
                {
                    CalcCurveRenderArea(ref _CurevRenderAreas, ref _CurevRanges, _FloatKey.Curve);
                    if (_CurevRenderAreas[0].width > 0.1f)
                        DrawCurve(_CurevRenderAreas[0], _CurevRanges[0], _FloatKey.Curve, CurveColor);
                }
                //else
                //{
                //    GUI.Label(RenderArea, PropertyKey.ValueKey.ToString());
                //}
                base.Render();
            }

            class FloatKeyViewProperties : EventProperties
            {
                private Skill.Editor.UI.FloatField _FFValue;
                private Skill.Editor.UI.CurveField _CurveField;

                public FloatKeyViewProperties(FloatKeyView e)
                    : base(e)
                {
                    _FFValue = new Skill.Editor.UI.FloatField() { Margin = ControlMargin };
                    _FFValue.Label.text = "Value";

                    _CurveField = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin };
                    _CurveField.Label.text = "Curve";
                    Skill.Editor.UI.ChangeCheck changeCheck = new Skill.Editor.UI.ChangeCheck() { Height = 20 };
                    changeCheck.Controls.Add(_CurveField);

                    Controls.Add(_FFValue);
                    Controls.Add(changeCheck);

                    _FFValue.ValueChanged += _FFValue_ValueChanged;
                    changeCheck.Changed += changeCheck_Changed;
                }

                void changeCheck_Changed(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    SetDirty();
                }

                void _FFValue_ValueChanged(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    ((FloatKeyView)_View).PropertyKey.ValueKey = _FFValue.Value;
                    SetDirty();
                }

                protected override void RefreshData()
                {
                    base.RefreshData();
                    _FFValue.Value = ((FloatKeyView)_View).PropertyKey.ValueKey;
                    FloatKey k = (FloatKey)((FloatKeyView)_View).Key;
                    ValidateCurves();
                }

                void ValidateCurves()
                {
                    _FFValue.Visibility = Skill.Framework.UI.Visibility.Collapsed;
                    _CurveField.Visibility = Skill.Framework.UI.Visibility.Visible;

                    FloatKey k = (FloatKey)((FloatKeyView)_View).Key;
                    if (k.Curve != null)
                    {
                        _CurveField.Curve = k.Curve;
                    }
                    else if (_CurveField.Curve == null)
                    {
                        _CurveField.Curve = new AnimationCurve();
                    }
                    k.Curve = _CurveField.Curve;

                }
            }
        }


    }

}