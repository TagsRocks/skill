using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public class Vector3TrackBar : PropertyTrackBar<Vector3>
    {
        protected override PropertyTrackBar<Vector3>.PropertyTimeLineEvent CreateNewEvent(IPropertyKey<Vector3> key) { return new Vector3KeyView(this, key); }
        protected override IPropertyKey<Vector3> CreateNewKey() { return new Vector3Key(); }
        protected override IPropertyKey<Vector3>[] CreateKeyArray(int arraySize) { return new Vector3Key[arraySize]; }

        public Vector3TrackBar(Vector3Track track)
            : base(track)
        {
        }

        protected override void EvaluateNewKey(IPropertyKey<Vector3> newKey, IPropertyKey<Vector3> previousKey)
        {
            if (previousKey != null)
                newKey.ValueKey = previousKey.ValueKey;
            else
                newKey.ValueKey = ((PropertyTrack<Vector3>)Track).DefaultValue;

            Vector3Key newfKey = (Vector3Key)newKey;

            if (previousKey != null)
            {

                Vector3Key prefKey = (Vector3Key)previousKey;

                newfKey.CurveX = CreateNextCurve(prefKey.CurveX, ((Vector3Track)Track).DefaultValue.x);
                newfKey.CurveY = CreateNextCurve(prefKey.CurveY, ((Vector3Track)Track).DefaultValue.y);
                newfKey.CurveZ = CreateNextCurve(prefKey.CurveZ, ((Vector3Track)Track).DefaultValue.z);
            }
            else
            {
                newfKey.CurveX = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.x });
                newfKey.CurveY = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.y });
                newfKey.CurveZ = CreateNextCurve(new Keyframe() { value = newKey.ValueKey.z });
            }
        }

        class Vector3KeyView : PropertyTimeLineEvent
        {
            public override double Duration
            {
                get
                {
                    if (_Vector3Key.CurveX != null && _Vector3Key.CurveY != null && _Vector3Key.CurveZ != null)
                    {
                        float max = 0;
                        if (_Vector3Key.CurveX.length > 0)
                            max = Mathf.Max(max, _Vector3Key.CurveX.keys[_Vector3Key.CurveX.length - 1].time);
                        if (_Vector3Key.CurveY.length > 0)
                            max = Mathf.Max(max, _Vector3Key.CurveY.keys[_Vector3Key.CurveY.length - 1].time);
                        if (_Vector3Key.CurveZ.length > 0)
                            max = Mathf.Max(max, _Vector3Key.CurveZ.keys[_Vector3Key.CurveZ.length - 1].time);
                        return max;
                    }
                    else
                        return 0.05f;
                }
                set { }
            }
            public override string Title { get { return "Vector3 Event"; } }

            private float _MinWidth;
            private Vector3 _PreValue;
            public override float MinWidth
            {
                get
                {
                    if (_Vector3Key.CurveX != null && _Vector3Key.CurveY != null && _Vector3Key.CurveZ != null)
                        return 22;
                    else
                    {
                        if (_MinWidth < 1f || _PreValue != _Vector3Key.Value)
                        {
                            _PreValue = _Vector3Key.Value;
                            GUIStyle labelStyle = "Label";
                            GUIContent content = new GUIContent() { text = _Vector3Key.Value.ToString() };
                            _MinWidth = labelStyle.CalcSize(content).x;
                        }
                        return _MinWidth;
                    }
                }
            }

            protected override Properties CreateProperties() { return new Vector3KeyViewProperties(this); }

            private Vector3Key _Vector3Key;
            public Vector3KeyView(Vector3TrackBar trackbar, IPropertyKey<Vector3> key)
                : base(trackbar, key)
            {
                _Vector3Key = (Vector3Key)key;
            }



            protected override void Render()
            {
                if (_Vector3Key.CurveX != null && _Vector3Key.CurveY != null && _Vector3Key.CurveZ != null)
                {
                    UnityEditor.EditorGUIUtility.DrawCurveSwatch(RenderArea, _Vector3Key.CurveX, null, Color.red, Color.clear);
                    UnityEditor.EditorGUIUtility.DrawCurveSwatch(RenderArea, _Vector3Key.CurveY, null, Color.green, Color.clear);
                    UnityEditor.EditorGUIUtility.DrawCurveSwatch(RenderArea, _Vector3Key.CurveZ, null, Color.blue, Color.clear);
                }
                else
                {
                    GUI.Label(RenderArea, PropertyKey.ValueKey.ToString());
                }
                base.Render();
            }

            class Vector3KeyViewProperties : EventProperties
            {
                private Skill.Editor.UI.ToggleButton _TbConstant;
                private Skill.Editor.UI.Vector3Field _Value;
                private Skill.Editor.UI.CurveField _CurveFieldX;
                private Skill.Editor.UI.CurveField _CurveFieldY;
                private Skill.Editor.UI.CurveField _CurveFieldZ;
                private Skill.Editor.UI.ChangeCheck _ChangeCheck;

                public Vector3KeyViewProperties(Vector3KeyView e)
                    : base(e)
                {
                    _TbConstant = new Skill.Editor.UI.ToggleButton() { Margin = ControlMargin };
                    _TbConstant.Label.text = "Constant?";
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
                    ((Vector3KeyView)_View).PropertyKey.ValueKey = _Value.Value;
                    SetDirty();
                }

                protected override void RefreshData()
                {
                    base.RefreshData();
                    _Value.Value = ((Vector3KeyView)_View).PropertyKey.ValueKey;

                    Vector3Key k = (Vector3Key)((Vector3KeyView)_View).Key;
                    _TbConstant.IsChecked = k.CurveX == null || k.CurveY == null || k.CurveZ == null;
                    ValidateCurve();
                }

                void ValidateCurve()
                {
                    if (_TbConstant.IsChecked)
                    {
                        _Value.Visibility = Skill.Framework.UI.Visibility.Visible;
                        _ChangeCheck.Visibility = Skill.Framework.UI.Visibility.Collapsed;
                        Vector3Key k = (Vector3Key)((Vector3KeyView)_View).Key;
                        k.CurveX = null;
                        k.CurveY = null;
                        k.CurveZ = null;
                    }
                    else
                    {
                        _Value.Visibility = Skill.Framework.UI.Visibility.Collapsed;
                        _ChangeCheck.Visibility = Skill.Framework.UI.Visibility.Visible;

                        Vector3Key k = (Vector3Key)((Vector3KeyView)_View).Key;
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
}