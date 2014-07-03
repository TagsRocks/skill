using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Sequence
{
    public class Vector2TrackBar : PropertyTrackBar<Vector2>
    {
        protected override PropertyTrackBar<Vector2>.PropertyTimeLineEvent CreateNewEvent(IPropertyKey<Vector2> key) { return new Vector2KeyView(this, key); }
        protected override IPropertyKey<Vector2> CreateNewKey() { return new Vector2Key(); }
        protected override IPropertyKey<Vector2>[] CreateKeyArray(int arraySize) { return new Vector2Key[arraySize]; }

        protected override bool IsCurve { get { return true; } }
        public Vector2TrackBar(Vector2Track track)
            : base(track)
        {
            this.Height = 40;
        }

        protected override void EvaluateNewKey(IPropertyKey<Vector2> newKey, IPropertyKey<Vector2> previousKey)
        {
            if (previousKey != null)
                newKey.ValueKey = previousKey.ValueKey;
            else
                newKey.ValueKey = ((PropertyTrack<Vector2>)Track).DefaultValue;

            Vector2Key newfKey = (Vector2Key)newKey;
            if (previousKey != null)
            {

                Vector2Key prefKey = (Vector2Key)previousKey;

                newfKey.CurveX = CreateNextCurve(prefKey.CurveX, ((Vector2Track)Track).DefaultValue.x);
                newfKey.CurveY = CreateNextCurve(prefKey.CurveY, ((Vector2Track)Track).DefaultValue.y);
            }
            else
            {
                newfKey.CurveX = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.x });
                newfKey.CurveY = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.y });
            }
        }

        internal override void Evaluate(float time)
        {
            Seek(time);
        }

        internal override void Seek(float time)
        {
            if ((this.RecordState & RecordState.XY) != RecordState.XY)
            {
                Vector2Track vt = (Vector2Track)Track;
                Vector2Key vk = (Vector2Key)vt.PropertyKeys[0];


                Vector2 curveValue = new Vector2(vk.CurveX.Evaluate(time), vk.CurveY.Evaluate(time));
                Vector2 sceneValue = curveValue;
                object v = vt.GetValue();
                if (v != null)
                    sceneValue = (Vector2)v;

                Vector2 value = curveValue;
                if ((this.RecordState & RecordState.X) != 0) value.x = sceneValue.x;
                if ((this.RecordState & RecordState.Y) != 0) value.y = sceneValue.y;

                vt.SetValue(value);
            }
        }

        class Vector2KeyView : PropertyTimeLineEvent
        {
            public override double Duration
            {
                get
                {
                    if (_Vector2Key.CurveX != null && _Vector2Key.CurveY != null)
                    {
                        float max = 0;
                        if (_Vector2Key.CurveX.length > 0)
                            max = Mathf.Max(max, _Vector2Key.CurveX.keys[_Vector2Key.CurveX.length - 1].time);
                        if (_Vector2Key.CurveY.length > 0)
                            max = Mathf.Max(max, _Vector2Key.CurveY.keys[_Vector2Key.CurveY.length - 1].time);
                        return max;
                    }
                    else
                        return 0.05f;
                }
                set { }
            }
            public override string Title { get { return "Vector2 Event"; } }

            protected override bool CanDrag { get { return false; } }

            private float _MinWidth;
            private Vector2 _PreValue;
            public override float MinWidth
            {
                get
                {
                    if (_Vector2Key.CurveX != null && _Vector2Key.CurveY != null)
                        return 22;
                    else
                    {
                        if (_MinWidth < 1f || _PreValue != _Vector2Key.Value)
                        {
                            _PreValue = _Vector2Key.Value;
                            GUIStyle labelStyle = "Label";
                            GUIContent content = new GUIContent() { text = _Vector2Key.Value.ToString() };
                            _MinWidth = labelStyle.CalcSize(content).x;
                        }
                        return _MinWidth;
                    }
                }
            }

            protected override PropertiesPanel CreateProperties() { return new Vector2KeyViewProperties(this); }


            private Vector2Key _Vector2Key;
            public Vector2KeyView(Vector2TrackBar trackbar, IPropertyKey<Vector2> key)
                : base(trackbar, key)
            {
                _Vector2Key = (Vector2Key)key;
            }


            private Rect[] _CurevRenderAreas = new Rect[2];
            private Rect[] _CurevRanges = new Rect[2];
            protected override void Render()
            {
                base.Render();
                if (_Vector2Key.CurveX != null && _Vector2Key.CurveY != null)
                {
                    CalcCurveRenderArea(ref _CurevRenderAreas, ref _CurevRanges, _Vector2Key.CurveX, _Vector2Key.CurveY);
                    if (_CurevRenderAreas[0].width > 0.1f)
                        DrawCurve(_CurevRenderAreas[0], _CurevRanges[0], _Vector2Key.CurveX, CurveXColor);
                    if (_CurevRenderAreas[1].width > 0.1f)
                        DrawCurve(_CurevRenderAreas[1], _CurevRanges[1], _Vector2Key.CurveY, CurveYColor);
                }
                //else
                //{
                //    GUI.Label(RenderArea, PropertyKey.ValueKey.ToString());
                //}                
            }

            class Vector2KeyViewProperties : EventProperties
            {
                private Skill.Editor.UI.Vector2Field _Value;
                private Skill.Editor.UI.CurveField _CurveFieldX;
                private Skill.Editor.UI.CurveField _CurveFieldY;
                private Skill.Editor.UI.ChangeCheck _ChangeCheck;

                public Vector2KeyViewProperties(Vector2KeyView e)
                    : base(e)
                {
                    _Value = new Skill.Editor.UI.Vector2Field() { Margin = ControlMargin };
                    _Value.Label = "Value";

                    _CurveFieldX = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 0, Color = Color.red, UseColor = true }; _CurveFieldX.Label.text = "Curve X";
                    _CurveFieldY = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 1, Color = Color.green, UseColor = true }; _CurveFieldY.Label.text = "Curve Y";

                    _ChangeCheck = new Skill.Editor.UI.ChangeCheck() { Height = 44 };
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.Controls.Add(_CurveFieldX);
                    _ChangeCheck.Controls.Add(_CurveFieldY);

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
                    ((Vector2KeyView)_View).PropertyKey.ValueKey = _Value.Value;
                    SetDirty();
                }

                protected override void RefreshData()
                {
                    base.RefreshData();
                    _Value.Value = ((Vector2KeyView)_View).PropertyKey.ValueKey;
                    ValidateCurves();
                }

                void ValidateCurves()
                {
                    _Value.Visibility = Skill.Framework.UI.Visibility.Collapsed;
                    _ChangeCheck.Visibility = Skill.Framework.UI.Visibility.Visible;

                    Vector2Key k = (Vector2Key)((Vector2KeyView)_View).Key;
                    if (k.CurveX != null) _CurveFieldX.Curve = k.CurveX;
                    if (k.CurveY != null) _CurveFieldY.Curve = k.CurveY;

                    if (_CurveFieldX.Curve == null) _CurveFieldX.Curve = new AnimationCurve();
                    if (_CurveFieldY.Curve == null) _CurveFieldY.Curve = new AnimationCurve();

                    k.CurveX = _CurveFieldX.Curve;
                    k.CurveY = _CurveFieldY.Curve;
                }
            }
        }


    }

}