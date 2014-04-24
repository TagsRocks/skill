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

        public Vector2TrackBar(Vector2Track track)
            : base(track)
        {
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



            protected override void Render()
            {
                if (_Vector2Key.CurveX != null && _Vector2Key.CurveY != null)
                {
                    UnityEditor.EditorGUIUtility.DrawCurveSwatch(RenderArea, _Vector2Key.CurveX, null, Color.red, Color.clear);
                    UnityEditor.EditorGUIUtility.DrawCurveSwatch(RenderArea, _Vector2Key.CurveY, null, Color.green, Color.clear);
                }
                else
                {
                    GUI.Label(RenderArea, PropertyKey.ValueKey.ToString());
                }
                base.Render();
            }

            class Vector2KeyViewProperties : EventProperties
            {
                private Skill.Editor.UI.ToggleButton _TbConstant;
                private Skill.Editor.UI.Vector2Field _Value;
                private Skill.Editor.UI.CurveField _CurveFieldX;
                private Skill.Editor.UI.CurveField _CurveFieldY;
                private Skill.Editor.UI.ChangeCheck _ChangeCheck;

                public Vector2KeyViewProperties(Vector2KeyView e)
                    : base(e)
                {
                    _TbConstant = new Skill.Editor.UI.ToggleButton() { Margin = ControlMargin };
                    _TbConstant.Label.text = "Constant?";
                    _Value = new Skill.Editor.UI.Vector2Field() { Margin = ControlMargin };
                    _Value.Label = "Value";

                    _CurveFieldX = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 0, Color = Color.red, UseColor = true }; _CurveFieldX.Label.text = "Curve X";
                    _CurveFieldY = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 1, Color = Color.green, UseColor = true }; _CurveFieldY.Label.text = "Curve Y";

                    _ChangeCheck = new Skill.Editor.UI.ChangeCheck() { Height = 44 };
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
                    _ChangeCheck.Controls.Add(_CurveFieldX);
                    _ChangeCheck.Controls.Add(_CurveFieldY);

                    Controls.Add(_TbConstant);
                    Controls.Add(_Value);
                    Controls.Add(_ChangeCheck);

                    _TbConstant.Changed += _TbConstant_Changed;
                    _Value.ValueChanged += _FFValue_ValueChanged;
                    _ChangeCheck.Changed += ChangeCheck_Changed;
                }

                void ChangeCheck_Changed(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    SetDirty();
                }

                void _TbConstant_Changed(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    ValidateCurve();
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

                    Vector2Key k = (Vector2Key)((Vector2KeyView)_View).Key;
                    _TbConstant.IsChecked = k.CurveX == null || k.CurveY == null;
                    ValidateCurve();
                }

                void ValidateCurve()
                {
                    if (_TbConstant.IsChecked)
                    {
                        _Value.Visibility = Skill.Framework.UI.Visibility.Visible;
                        _ChangeCheck.Visibility = Skill.Framework.UI.Visibility.Collapsed;
                        Vector2Key k = (Vector2Key)((Vector2KeyView)_View).Key;
                        k.CurveX = null;
                        k.CurveY = null;
                    }
                    else
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

}