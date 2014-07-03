using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Sequence
{

    public class Vector4TrackBar : PropertyTrackBar<Vector4>
    {
        protected override PropertyTrackBar<Vector4>.PropertyTimeLineEvent CreateNewEvent(IPropertyKey<Vector4> key) { return new Vector4KeyView(this, key); }
        protected override IPropertyKey<Vector4> CreateNewKey() { return new Vector4Key(); }
        protected override IPropertyKey<Vector4>[] CreateKeyArray(int arraySize) { return new Vector4Key[arraySize]; }
        protected override bool IsCurve { get { return true; } }
        public Vector4TrackBar(Vector4Track track)
            : base(track)
        {
            this.Height = 40;
        }

        protected override void EvaluateNewKey(IPropertyKey<Vector4> newKey, IPropertyKey<Vector4> previousKey)
        {
            if (previousKey != null)
                newKey.ValueKey = previousKey.ValueKey;
            else
                newKey.ValueKey = ((PropertyTrack<Vector4>)Track).DefaultValue;

            Vector4Key newfKey = (Vector4Key)newKey;
            if (previousKey != null)
            {

                Vector4Key prefKey = (Vector4Key)previousKey;

                newfKey.CurveX = CreateNextCurve(prefKey.CurveX, ((Vector4Track)Track).DefaultValue.x);
                newfKey.CurveY = CreateNextCurve(prefKey.CurveY, ((Vector4Track)Track).DefaultValue.y);
                newfKey.CurveZ = CreateNextCurve(prefKey.CurveZ, ((Vector4Track)Track).DefaultValue.z);
                newfKey.CurveW = CreateNextCurve(prefKey.CurveW, ((Vector4Track)Track).DefaultValue.w);
            }
            else
            {
                newfKey.CurveX = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.x });
                newfKey.CurveY = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.y });
                newfKey.CurveZ = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.z });
                newfKey.CurveW = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.w });
            }
        }


        internal override void Evaluate(float time)
        {
            Seek(time);
        }

        internal override void Seek(float time)
        {
            if ((this.RecordState & RecordState.XYZW) != RecordState.XYZW)
            {
                Vector4Track vt = (Vector4Track)Track;
                Vector4Key vk = (Vector4Key)vt.PropertyKeys[0];


                Vector4 curveValue = new Vector4(vk.CurveX.Evaluate(time), vk.CurveY.Evaluate(time), vk.CurveZ.Evaluate(time), vk.CurveW.Evaluate(time));
                Vector4 sceneValue = curveValue;
                object v = vt.GetValue();
                if (v != null)
                    sceneValue = (Vector4)v;

                Vector4 value = curveValue;
                if ((this.RecordState & RecordState.X) != 0) value.x = sceneValue.x;
                if ((this.RecordState & RecordState.Y) != 0) value.y = sceneValue.y;
                if ((this.RecordState & RecordState.Z) != 0) value.z = sceneValue.z;
                if ((this.RecordState & RecordState.W) != 0) value.w = sceneValue.w;

                vt.SetValue(value);
            }
        }
        class Vector4KeyView : PropertyTimeLineEvent
        {
            public override double Duration
            {
                get
                {
                    if (_Vector4Key.CurveX != null && _Vector4Key.CurveY != null && _Vector4Key.CurveZ != null && _Vector4Key.CurveW != null)
                    {
                        float max = 0;
                        if (_Vector4Key.CurveX.length > 0)
                            max = Mathf.Max(max, _Vector4Key.CurveX.keys[_Vector4Key.CurveX.length - 1].time);
                        if (_Vector4Key.CurveY.length > 0)
                            max = Mathf.Max(max, _Vector4Key.CurveY.keys[_Vector4Key.CurveY.length - 1].time);
                        if (_Vector4Key.CurveZ.length > 0)
                            max = Mathf.Max(max, _Vector4Key.CurveZ.keys[_Vector4Key.CurveZ.length - 1].time);
                        if (_Vector4Key.CurveW.length > 0)
                            max = Mathf.Max(max, _Vector4Key.CurveW.keys[_Vector4Key.CurveW.length - 1].time);
                        return max;
                    }
                    else
                        return 0.05f;
                }
                set { }
            }

            public override string Title { get { return "Vector4 Event"; } }

            protected override bool CanDrag { get { return false; } }

            private float _MinWidth;
            private Vector4 _PreValue;
            public override float MinWidth
            {
                get
                {
                    if (_Vector4Key.CurveX != null && _Vector4Key.CurveY != null && _Vector4Key.CurveZ != null && _Vector4Key.CurveW != null)
                        return 22;
                    else
                    {
                        if (_MinWidth < 1f || _PreValue != _Vector4Key.Value)
                        {
                            _PreValue = _Vector4Key.Value;
                            GUIStyle labelStyle = "Label";
                            GUIContent content = new GUIContent() { text = _Vector4Key.Value.ToString() };
                            _MinWidth = labelStyle.CalcSize(content).x;
                        }
                        return _MinWidth;
                    }
                }
            }
            protected override PropertiesPanel CreateProperties() { return new Vector4KeyViewProperties(this); }

            private Vector4Key _Vector4Key;
            public Vector4KeyView(Vector4TrackBar trackbar, IPropertyKey<Vector4> key)
                : base(trackbar, key)
            {
                _Vector4Key = (Vector4Key)key;
            }


            private Rect[] _CurevRenderAreas = new Rect[4];
            private Rect[] _CurevRanges = new Rect[4];
            protected override void Render()
            {
                base.Render();
                if (_Vector4Key.CurveX != null && _Vector4Key.CurveY != null && _Vector4Key.CurveZ != null)
                {
                    CalcCurveRenderArea(ref _CurevRenderAreas, ref _CurevRanges, _Vector4Key.CurveX, _Vector4Key.CurveY, _Vector4Key.CurveZ, _Vector4Key.CurveW);
                    if (_CurevRenderAreas[0].width > 0.1f)
                        DrawCurve(_CurevRenderAreas[0], _CurevRanges[0], _Vector4Key.CurveX, CurveXColor);
                    if (_CurevRenderAreas[1].width > 0.1f)
                        DrawCurve(_CurevRenderAreas[1], _CurevRanges[1], _Vector4Key.CurveY, CurveYColor);
                    if (_CurevRenderAreas[2].width > 0.1f)
                        DrawCurve(_CurevRenderAreas[2], _CurevRanges[2], _Vector4Key.CurveZ, CurveZColor);
                    if (_CurevRenderAreas[3].width > 0.1f)
                        DrawCurve(_CurevRenderAreas[3], _CurevRanges[3], _Vector4Key.CurveW, CurveWColor);
                }
                //else
                //{
                //    GUI.Label(RenderArea, PropertyKey.ValueKey.ToString());
                //}
                
            }

            class Vector4KeyViewProperties : EventProperties
            {
                private Skill.Editor.UI.Vector4Field _Value;
                private Skill.Editor.UI.CurveField _CurveFieldX;
                private Skill.Editor.UI.CurveField _CurveFieldY;
                private Skill.Editor.UI.CurveField _CurveFieldZ;
                private Skill.Editor.UI.CurveField _CurveFieldW;
                private Skill.Editor.UI.ChangeCheck _ChangeCheck;

                public Vector4KeyViewProperties(Vector4KeyView e)
                    : base(e)
                {
                    _Value = new Skill.Editor.UI.Vector4Field() { Margin = ControlMargin };
                    _Value.Label = "Value";

                    _CurveFieldX = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 0, Color = Color.red, UseColor = true }; _CurveFieldX.Label.text = "Curve X";
                    _CurveFieldY = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 1, Color = Color.green, UseColor = true }; _CurveFieldY.Label.text = "Curve Y";
                    _CurveFieldZ = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 2, Color = Color.blue, UseColor = true }; _CurveFieldZ.Label.text = "Curve Z";
                    _CurveFieldW = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 3, Color = Color.yellow, UseColor = true }; _CurveFieldW.Label.text = "Curve W";

                    _ChangeCheck = new Skill.Editor.UI.ChangeCheck() { Height = 88 };
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.Controls.Add(_CurveFieldX);
                    _ChangeCheck.Controls.Add(_CurveFieldY);
                    _ChangeCheck.Controls.Add(_CurveFieldZ);
                    _ChangeCheck.Controls.Add(_CurveFieldW);

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
                    ((Vector4KeyView)_View).PropertyKey.ValueKey = _Value.Value;
                    SetDirty();
                }

                protected override void RefreshData()
                {
                    base.RefreshData();
                    _Value.Value = ((Vector4KeyView)_View).PropertyKey.ValueKey;

                    Vector4Key k = (Vector4Key)((Vector4KeyView)_View).Key;
                    ValidateCurves();
                }

                void ValidateCurves()
                {
                    _Value.Visibility = Skill.Framework.UI.Visibility.Collapsed;
                    _ChangeCheck.Visibility = Skill.Framework.UI.Visibility.Visible;

                    Vector4Key k = (Vector4Key)((Vector4KeyView)_View).Key;
                    if (k.CurveX != null) _CurveFieldX.Curve = k.CurveX;
                    if (k.CurveY != null) _CurveFieldY.Curve = k.CurveY;
                    if (k.CurveZ != null) _CurveFieldZ.Curve = k.CurveZ;
                    if (k.CurveW != null) _CurveFieldW.Curve = k.CurveW;

                    if (_CurveFieldX.Curve == null) _CurveFieldX.Curve = new AnimationCurve();
                    if (_CurveFieldY.Curve == null) _CurveFieldY.Curve = new AnimationCurve();
                    if (_CurveFieldZ.Curve == null) _CurveFieldZ.Curve = new AnimationCurve();
                    if (_CurveFieldW.Curve == null) _CurveFieldW.Curve = new AnimationCurve();

                    k.CurveX = _CurveFieldX.Curve;
                    k.CurveY = _CurveFieldY.Curve;
                    k.CurveZ = _CurveFieldZ.Curve;
                    k.CurveW = _CurveFieldW.Curve;

                }
            }
        }


    }
}