using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;

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


        class FloatKeyView : PropertyTimeLineEvent
        {
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

            protected override Properties CreateProperties() { return new FloatKeyViewProperties(this); }

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

            public override float MaxWidth { get { return MinWidth; } }


            private FloatKey _FloatKey;
            public FloatKeyView(FloatTrackBar trackbar, IPropertyKey<float> key)
                : base(trackbar, key)
            {
                _FloatKey = (FloatKey)key;
            }

            protected override void Render()
            {
                if (_FloatKey.Curve != null)
                {
                    UnityEditor.EditorGUIUtility.DrawCurveSwatch(RenderArea, _FloatKey.Curve, null, Color.green, Color.clear);
                }
                else
                {
                    GUI.Label(RenderArea, PropertyKey.ValueKey.ToString());
                }
                base.Render();
            }

            class FloatKeyViewProperties : EventProperties
            {
                private Skill.Editor.UI.ToggleButton _TbConstant;
                private Skill.Editor.UI.FloatField _FFValue;
                private Skill.Editor.UI.CurveField _CurveField;

                public FloatKeyViewProperties(FloatKeyView e)
                    : base(e)
                {
                    _TbConstant = new Skill.Editor.UI.ToggleButton() { Margin = ControlMargin };
                    _TbConstant.Label.text = "Constant?";
                    _FFValue = new Skill.Editor.UI.FloatField() { Margin = ControlMargin };
                    _FFValue.Label.text = "Value";

                    _CurveField = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin };
                    _CurveField.Label.text = "Curve";
                    Skill.Editor.UI.ChangeCheck changeCheck = new Skill.Editor.UI.ChangeCheck() { Height = 20 };
                    changeCheck.Controls.Add(_CurveField);

                    Controls.Add(_TbConstant);
                    Controls.Add(_FFValue);
                    Controls.Add(changeCheck);

                    _TbConstant.Changed += _TbConstant_Changed;
                    _FFValue.ValueChanged += _FFValue_ValueChanged;
                    changeCheck.Changed += changeCheck_Changed;
                }

                void changeCheck_Changed(object sender, System.EventArgs e)
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
                    ((FloatKeyView)_View).PropertyKey.ValueKey = _FFValue.Value;
                    SetDirty();
                }

                protected override void RefreshData()
                {
                    base.RefreshData();
                    _FFValue.Value = ((FloatKeyView)_View).PropertyKey.ValueKey;
                    FloatKey k = (FloatKey)((FloatKeyView)_View).Key;
                    _TbConstant.IsChecked = k.Curve == null;
                    ValidateCurve();
                }

                void ValidateCurve()
                {
                    if (_TbConstant.IsChecked)
                    {
                        _FFValue.Visibility = Skill.Framework.UI.Visibility.Visible;
                        _CurveField.Visibility = Skill.Framework.UI.Visibility.Collapsed;
                        FloatKey k = (FloatKey)((FloatKeyView)_View).Key;
                        k.Curve = null;
                    }
                    else
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

}