using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Sequence
{
    public class QuaternionTrackBar : ContinuousTrackBar<Quaternion>
    {
        private QuaternionTrack _QuaternionTrack;

        public QuaternionTrackBar(QuaternionTrack track)
            : base(track)
        {
            _QuaternionTrack = track;
            CheckCurves();
            Refresh();
        }

        private void CheckCurves()
        {
            if (_QuaternionTrack.CurveX == null) _QuaternionTrack.CurveX = new AnimationCurve();
            if (_QuaternionTrack.CurveY == null) _QuaternionTrack.CurveY = new AnimationCurve();
            if (_QuaternionTrack.CurveZ == null) _QuaternionTrack.CurveZ = new AnimationCurve();
        }

        //public override void UpdateDefaultValue(Quaternion defaultValue)
        //{
        //    Vector3 eulers = defaultValue.eulerAngles;
        //    base.UpdateDefaultValue(defaultValue);
        //    CheckCurves();
        //    ValidateCurve(_QuaternionTrack.CurveX, eulers.x);
        //    ValidateCurve(_QuaternionTrack.CurveY, eulers.y);
        //    ValidateCurve(_QuaternionTrack.CurveZ, eulers.z);
        //}


        private float Closify(AnimationCurve curve, float angle, float time)
        {
            if (curve != null && curve.length > 0)
            {
                int index = -1;
                for (int i = 0; i < curve.length; i++)
                {
                    if (curve[i].time >= time - 0.001f)
                    {
                        if (index == -1)
                            index = i;
                        break;
                    }
                    else
                        index = i;
                }                

                if (index >= 0)
                {
                    float neighbor = curve[index].value;                    
                    if (angle > neighbor)
                    {
                        float lowerAngle = angle - 360;
                        while (lowerAngle > neighbor) lowerAngle -= 360;

                        if (neighbor - lowerAngle < angle - neighbor)
                        {
                            angle = lowerAngle;                            
                        }
                    }
                    else if (angle < neighbor)
                    {
                        float upperAngle = angle + 360;
                        while (upperAngle < neighbor) upperAngle += 360;

                        if (upperAngle - neighbor < neighbor - angle)
                        {
                            angle = upperAngle;                            
                        }
                    }
                }
            }
            return angle;
        }

        protected override void AddCurveKey(KeyType keyType, float time)
        {
            Vector3 sceneValue = new Vector3();
            object v = _QuaternionTrack.GetValue();
            if (v != null)
            {
                sceneValue = ((Quaternion)v).eulerAngles;
            }
            else
            {
                sceneValue.x = _QuaternionTrack.CurveX.Evaluate(time);
                sceneValue.y = _QuaternionTrack.CurveY.Evaluate(time);
                sceneValue.z = _QuaternionTrack.CurveZ.Evaluate(time);
            }

            if ((keyType & Sequence.KeyType.X) == KeyType.X)
            {
                sceneValue.x = Closify(_QuaternionTrack.CurveX, sceneValue.x, time);
                AddKeyToCurve(_QuaternionTrack.CurveX, time, sceneValue.x);
            }
            if ((keyType & Sequence.KeyType.Y) == KeyType.Y)
            {
                sceneValue.y = Closify(_QuaternionTrack.CurveY, sceneValue.y, time);
                AddKeyToCurve(_QuaternionTrack.CurveY, time, sceneValue.y);
            }
            if ((keyType & Sequence.KeyType.Z) == KeyType.Z)
            {
                sceneValue.z = Closify(_QuaternionTrack.CurveZ, sceneValue.z, time);
                AddKeyToCurve(_QuaternionTrack.CurveZ, time, sceneValue.z);
            }
        }

        protected override KeyType GetEquality(Quaternion v1, Quaternion v2)
        {
            KeyType equality = KeyType.None;

            Vector3 eulerAngles1 = v1.eulerAngles;
            Vector3 eulerAngles2 = v2.eulerAngles;

            if (Mathf.Approximately(eulerAngles1.x, eulerAngles2.x)) equality |= KeyType.X;
            if (Mathf.Approximately(eulerAngles1.y, eulerAngles2.y)) equality |= KeyType.Y;
            if (Mathf.Approximately(eulerAngles1.z, eulerAngles2.z)) equality |= KeyType.Z;

            return equality;
        }


        // Old Code
        //internal override void Evaluate(float time)
        //{
        //    Seek(time);
        //}

        //internal override void Seek(float time)
        //{
        //    if ((this.RecordState & RecordState.XYZ) != RecordState.XYZ)
        //    {
        //        QuaternionTrack qt = (QuaternionTrack)Track;
        //        QuaternionKey vk = (QuaternionKey)qt.PropertyKeys[0];


        //        Vector3 curveValue = new Vector3(vk.CurveX.Evaluate(time), vk.CurveY.Evaluate(time), vk.CurveZ.Evaluate(time));
        //        Vector3 sceneValue = curveValue;
        //        object v = qt.GetValue();
        //        if (v != null)
        //            sceneValue = ((Quaternion)v).eulerAngles;

        //        Vector3 value = curveValue;
        //        if ((this.RecordState & RecordState.X) != 0) value.x = sceneValue.x;
        //        if ((this.RecordState & RecordState.Y) != 0) value.y = sceneValue.y;
        //        if ((this.RecordState & RecordState.Z) != 0) value.z = sceneValue.z;

        //        qt.SetValue(Quaternion.Euler(value));
        //    }
        //}


        //class QuaternionKeyView : PropertyTimeLineEvent
        //{
        //    public override double Duration
        //    {
        //        get
        //        {
        //            if (_QuaternionKey.CurveX != null && _QuaternionKey.CurveY != null && _QuaternionKey.CurveZ != null)
        //            {
        //                float max = 0;
        //                if (_QuaternionKey.CurveX.length > 0)
        //                    max = Mathf.Max(max, _QuaternionKey.CurveX.keys[_QuaternionKey.CurveX.length - 1].time);
        //                if (_QuaternionKey.CurveY.length > 0)
        //                    max = Mathf.Max(max, _QuaternionKey.CurveY.keys[_QuaternionKey.CurveY.length - 1].time);
        //                if (_QuaternionKey.CurveZ.length > 0)
        //                    max = Mathf.Max(max, _QuaternionKey.CurveZ.keys[_QuaternionKey.CurveZ.length - 1].time);
        //                return max;
        //            }
        //            else
        //                return 0.05f;
        //        }
        //        set { }
        //    }
        //    public override string Title { get { return "Quaternion Event"; } }

        //    protected override bool CanDrag { get { return false; } }

        //    private float _MinWidth;
        //    private Quaternion _PreValue;
        //    public override float MinWidth
        //    {
        //        get
        //        {
        //            if (_QuaternionKey.CurveX != null && _QuaternionKey.CurveY != null && _QuaternionKey.CurveZ != null)
        //                return 22;
        //            else
        //            {
        //                if (_MinWidth < 1f || _PreValue != _QuaternionKey.Value)
        //                {
        //                    _PreValue = _QuaternionKey.Value;
        //                    GUIStyle labelStyle = "Label";
        //                    GUIContent content = new GUIContent() { text = _QuaternionKey.Value.ToString() };
        //                    _MinWidth = labelStyle.CalcSize(content).x;
        //                }
        //                return _MinWidth;
        //            }
        //        }
        //    }
        //    protected override PropertiesPanel CreateProperties() { return new QuaternionKeyViewProperties(this); }

        //    private QuaternionKey _QuaternionKey;
        //    public QuaternionKeyView(QuaternionTrackBar trackbar, IPropertyKey<Quaternion> key)
        //        : base(trackbar, key)
        //    {
        //        _QuaternionKey = (QuaternionKey)key;
        //    }


        //    private Rect[] _CurevRenderAreas = new Rect[3];
        //    private Rect[] _CurevRanges = new Rect[3];
        //    protected override void Render()
        //    {
        //        base.Render();
        //        if (_QuaternionKey.CurveX != null && _QuaternionKey.CurveY != null && _QuaternionKey.CurveZ != null)
        //        {
        //            CalcCurveRenderArea(ref _CurevRenderAreas, ref _CurevRanges, _QuaternionKey.CurveX, _QuaternionKey.CurveY, _QuaternionKey.CurveZ);
        //            if (_CurevRenderAreas[0].width > 0.1f)
        //                DrawCurve(_CurevRenderAreas[0], _CurevRanges[0], _QuaternionKey.CurveX, CurveXColor);
        //            if (_CurevRenderAreas[1].width > 0.1f)
        //                DrawCurve(_CurevRenderAreas[1], _CurevRanges[1], _QuaternionKey.CurveY, CurveYColor);
        //            if (_CurevRenderAreas[2].width > 0.1f)
        //                DrawCurve(_CurevRenderAreas[2], _CurevRanges[2], _QuaternionKey.CurveZ, CurveZColor);
        //        }
        //        //else
        //        //{
        //        //    GUI.Label(RenderArea, PropertyKey.ValueKey.ToString());
        //        //}

        //    }

        //    class QuaternionKeyViewProperties : EventProperties
        //    {
        //        private Skill.Editor.UI.Vector3Field _Value;
        //        private Skill.Editor.UI.CurveField _CurveFieldX;
        //        private Skill.Editor.UI.CurveField _CurveFieldY;
        //        private Skill.Editor.UI.CurveField _CurveFieldZ;
        //        private Skill.Editor.UI.ChangeCheck _ChangeCheck;

        //        public QuaternionKeyViewProperties(QuaternionKeyView e)
        //            : base(e)
        //        {
        //            _Value = new Skill.Editor.UI.Vector3Field() { Margin = ControlMargin };
        //            _Value.Label = "Value";

        //            _CurveFieldX = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 0, Color = Color.red, UseColor = true }; _CurveFieldX.Label.text = "Curve X";
        //            _CurveFieldY = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 1, Color = Color.green, UseColor = true }; _CurveFieldY.Label.text = "Curve Y";
        //            _CurveFieldZ = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 2, Color = Color.blue, UseColor = true }; _CurveFieldZ.Label.text = "Curve Z";

        //            _ChangeCheck = new Skill.Editor.UI.ChangeCheck() { Height = 66 };
        //            _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
        //            _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
        //            _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
        //            _ChangeCheck.Controls.Add(_CurveFieldX);
        //            _ChangeCheck.Controls.Add(_CurveFieldY);
        //            _ChangeCheck.Controls.Add(_CurveFieldZ);

        //            Controls.Add(_Value);
        //            Controls.Add(_ChangeCheck);

        //            _Value.ValueChanged += _FFValue_ValueChanged;
        //            _ChangeCheck.Changed += ChangeCheck_Changed;
        //        }

        //        void ChangeCheck_Changed(object sender, System.EventArgs e)
        //        {
        //            if (IgnoreChanges) return;
        //            SetDirty();
        //        }

        //        void _FFValue_ValueChanged(object sender, System.EventArgs e)
        //        {
        //            if (IgnoreChanges) return;
        //            ((QuaternionKeyView)_View).PropertyKey.ValueKey = Quaternion.Euler(_Value.Value);
        //            SetDirty();
        //        }

        //        protected override void RefreshData()
        //        {
        //            base.RefreshData();
        //            _Value.Value = ((QuaternionKeyView)_View).PropertyKey.ValueKey.eulerAngles;
        //            ValidateCurves();
        //        }

        //        void ValidateCurves()
        //        {
        //            _Value.Visibility = Skill.Framework.UI.Visibility.Collapsed;
        //            _ChangeCheck.Visibility = Skill.Framework.UI.Visibility.Visible;

        //            QuaternionKey k = (QuaternionKey)((QuaternionKeyView)_View).Key;
        //            if (k.CurveX != null) _CurveFieldX.Curve = k.CurveX;
        //            if (k.CurveY != null) _CurveFieldY.Curve = k.CurveY;
        //            if (k.CurveZ != null) _CurveFieldZ.Curve = k.CurveZ;

        //            if (_CurveFieldX.Curve == null) _CurveFieldX.Curve = new AnimationCurve();
        //            if (_CurveFieldY.Curve == null) _CurveFieldY.Curve = new AnimationCurve();
        //            if (_CurveFieldZ.Curve == null) _CurveFieldZ.Curve = new AnimationCurve();

        //            k.CurveX = _CurveFieldX.Curve;
        //            k.CurveY = _CurveFieldY.Curve;
        //            k.CurveZ = _CurveFieldZ.Curve;
        //        }
        //    }
        //}


    }
}