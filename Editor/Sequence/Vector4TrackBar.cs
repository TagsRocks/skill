using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Sequence
{

    public class Vector4TrackBar : ContinuousTrackBar<Vector4>
    {
        public override int CurveCount { get { return 4; } }
        public override AnimationCurve GetCurve(int index)
        {
            if (index == 0) return _Vector4Track.CurveX;
            if (index == 1) return _Vector4Track.CurveY;
            if (index == 2) return _Vector4Track.CurveZ;
            return _Vector4Track.CurveW;
        }

        private Vector4Track _Vector4Track;


        public Vector4TrackBar(Vector4Track track)
            : base(track)
        {
            _Vector4Track = track;
            CheckCurves();
            Refresh();
        }

        private void CheckCurves()
        {
            if (_Vector4Track.CurveX == null) _Vector4Track.CurveX = new AnimationCurve();
            if (_Vector4Track.CurveY == null) _Vector4Track.CurveY = new AnimationCurve();
            if (_Vector4Track.CurveZ == null) _Vector4Track.CurveZ = new AnimationCurve();
            if (_Vector4Track.CurveW == null) _Vector4Track.CurveW = new AnimationCurve();
        }


        public override void UpdateDefaultValue(Vector4 defaultValue)
        {
            base.UpdateDefaultValue(defaultValue);
            CheckCurves();
            ValidateCurve(_Vector4Track.CurveX, defaultValue.x);
            ValidateCurve(_Vector4Track.CurveY, defaultValue.y);
            ValidateCurve(_Vector4Track.CurveZ, defaultValue.z);
            ValidateCurve(_Vector4Track.CurveW, defaultValue.w);
        }

        protected override void AddCurveKey(KeyType keyType, float time)
        {
            Vector4 sceneValue = new Vector4();
            object v = _Vector4Track.GetValue();
            if (v != null)
            {
                sceneValue = (Vector4)v;
            }
            else
            {
                sceneValue.x = _Vector4Track.CurveX.Evaluate(time);
                sceneValue.y = _Vector4Track.CurveY.Evaluate(time);
                sceneValue.z = _Vector4Track.CurveZ.Evaluate(time);
                sceneValue.w = _Vector4Track.CurveW.Evaluate(time);
            }

            if ((keyType & Sequence.KeyType.X) == KeyType.X)
                AddKeyToCurve(_Vector4Track.CurveX, time, sceneValue.x);
            if ((keyType & Sequence.KeyType.Y) == KeyType.Y)
                AddKeyToCurve(_Vector4Track.CurveY, time, sceneValue.y);
            if ((keyType & Sequence.KeyType.Z) == KeyType.Z)
                AddKeyToCurve(_Vector4Track.CurveZ, time, sceneValue.z);
            if ((keyType & Sequence.KeyType.W) == KeyType.W)
                AddKeyToCurve(_Vector4Track.CurveW, time, sceneValue.w);
        }

        protected override KeyType GetEquality(Vector4 v1, Vector4 v2)
        {
            KeyType equality = KeyType.None;

            if (Mathf.Approximately(v1.x, v2.x)) equality |= KeyType.X;
            if (Mathf.Approximately(v1.y, v2.y)) equality |= KeyType.Y;
            if (Mathf.Approximately(v1.z, v2.z)) equality |= KeyType.Z;
            if (Mathf.Approximately(v1.w, v2.w)) equality |= KeyType.W;
            return equality;
        }

        // Old Code
        //internal override void Evaluate(float time)
        //{
        //    Seek(time);
        //}

        //internal override void Seek(float time)
        //{
        //    if ((this.RecordState & RecordState.XYZW) != RecordState.XYZW)
        //    {
        //        Vector4Track vt = (Vector4Track)Track;
        //        Vector4Key vk = (Vector4Key)vt.PropertyKeys[0];


        //        Vector4 curveValue = new Vector4(vk.CurveX.Evaluate(time), vk.CurveY.Evaluate(time), vk.CurveZ.Evaluate(time), vk.CurveW.Evaluate(time));
        //        Vector4 sceneValue = curveValue;
        //        object v = vt.GetValue();
        //        if (v != null)
        //            sceneValue = (Vector4)v;

        //        Vector4 value = curveValue;
        //        if ((this.RecordState & RecordState.X) != 0) value.x = sceneValue.x;
        //        if ((this.RecordState & RecordState.Y) != 0) value.y = sceneValue.y;
        //        if ((this.RecordState & RecordState.Z) != 0) value.z = sceneValue.z;
        //        if ((this.RecordState & RecordState.W) != 0) value.w = sceneValue.w;

        //        vt.SetValue(value);
        //    }
        //}



        //class Vector4KeyView : PropertyTimeLineEvent
        //{
        //    public override double Duration
        //    {
        //        get
        //        {
        //            if (_Vector4Key.CurveX != null && _Vector4Key.CurveY != null && _Vector4Key.CurveZ != null && _Vector4Key.CurveW != null)
        //            {
        //                float max = 0;
        //                if (_Vector4Key.CurveX.length > 0)
        //                    max = Mathf.Max(max, _Vector4Key.CurveX.keys[_Vector4Key.CurveX.length - 1].time);
        //                if (_Vector4Key.CurveY.length > 0)
        //                    max = Mathf.Max(max, _Vector4Key.CurveY.keys[_Vector4Key.CurveY.length - 1].time);
        //                if (_Vector4Key.CurveZ.length > 0)
        //                    max = Mathf.Max(max, _Vector4Key.CurveZ.keys[_Vector4Key.CurveZ.length - 1].time);
        //                if (_Vector4Key.CurveW.length > 0)
        //                    max = Mathf.Max(max, _Vector4Key.CurveW.keys[_Vector4Key.CurveW.length - 1].time);
        //                return max;
        //            }
        //            else
        //                return 0.05f;
        //        }
        //        set { }
        //    }

        //    public override string Title { get { return "Vector4 Event"; } }

        //    protected override bool CanDrag { get { return false; } }

        //    private float _MinWidth;
        //    private Vector4 _PreValue;
        //    public override float MinWidth
        //    {
        //        get
        //        {
        //            if (_Vector4Key.CurveX != null && _Vector4Key.CurveY != null && _Vector4Key.CurveZ != null && _Vector4Key.CurveW != null)
        //                return 22;
        //            else
        //            {
        //                if (_MinWidth < 1f || _PreValue != _Vector4Key.Value)
        //                {
        //                    _PreValue = _Vector4Key.Value;
        //                    GUIStyle labelStyle = "Label";
        //                    GUIContent content = new GUIContent() { text = _Vector4Key.Value.ToString() };
        //                    _MinWidth = labelStyle.CalcSize(content).x;
        //                }
        //                return _MinWidth;
        //            }
        //        }
        //    }
        //    protected override PropertiesPanel CreateProperties() { return new Vector4KeyViewProperties(this); }

        //    private Vector4Key _Vector4Key;
        //    public Vector4KeyView(Vector4TrackBar trackbar, IPropertyKey<Vector4> key)
        //        : base(trackbar, key)
        //    {
        //        _Vector4Key = (Vector4Key)key;
        //    }


        //    private Rect[] _CurevRenderAreas = new Rect[4];
        //    private Rect[] _CurevRanges = new Rect[4];
        //    protected override void Render()
        //    {
        //        base.Render();
        //        if (_Vector4Key.CurveX != null && _Vector4Key.CurveY != null && _Vector4Key.CurveZ != null)
        //        {
        //            CalcCurveRenderArea(ref _CurevRenderAreas, ref _CurevRanges, _Vector4Key.CurveX, _Vector4Key.CurveY, _Vector4Key.CurveZ, _Vector4Key.CurveW);
        //            if (_CurevRenderAreas[0].width > 0.1f)
        //                DrawCurve(_CurevRenderAreas[0], _CurevRanges[0], _Vector4Key.CurveX, CurveXColor);
        //            if (_CurevRenderAreas[1].width > 0.1f)
        //                DrawCurve(_CurevRenderAreas[1], _CurevRanges[1], _Vector4Key.CurveY, CurveYColor);
        //            if (_CurevRenderAreas[2].width > 0.1f)
        //                DrawCurve(_CurevRenderAreas[2], _CurevRanges[2], _Vector4Key.CurveZ, CurveZColor);
        //            if (_CurevRenderAreas[3].width > 0.1f)
        //                DrawCurve(_CurevRenderAreas[3], _CurevRanges[3], _Vector4Key.CurveW, CurveWColor);
        //        }
        //        //else
        //        //{
        //        //    GUI.Label(RenderArea, PropertyKey.ValueKey.ToString());
        //        //}

        //    }

        //    class Vector4KeyViewProperties : EventProperties
        //    {
        //        private Skill.Editor.UI.Vector4Field _Value;
        //        private Skill.Editor.UI.CurveField _CurveFieldX;
        //        private Skill.Editor.UI.CurveField _CurveFieldY;
        //        private Skill.Editor.UI.CurveField _CurveFieldZ;
        //        private Skill.Editor.UI.CurveField _CurveFieldW;
        //        private Skill.Editor.UI.ChangeCheck _ChangeCheck;

        //        public Vector4KeyViewProperties(Vector4KeyView e)
        //            : base(e)
        //        {
        //            _Value = new Skill.Editor.UI.Vector4Field() { Margin = ControlMargin };
        //            _Value.Label = "Value";

        //            _CurveFieldX = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 0, Color = Color.red, UseColor = true }; _CurveFieldX.Label.text = "Curve X";
        //            _CurveFieldY = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 1, Color = Color.green, UseColor = true }; _CurveFieldY.Label.text = "Curve Y";
        //            _CurveFieldZ = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 2, Color = Color.blue, UseColor = true }; _CurveFieldZ.Label.text = "Curve Z";
        //            _CurveFieldW = new Skill.Editor.UI.CurveField(null) { Margin = ControlMargin, Row = 3, Color = Color.yellow, UseColor = true }; _CurveFieldW.Label.text = "Curve W";

        //            _ChangeCheck = new Skill.Editor.UI.ChangeCheck() { Height = 88 };
        //            _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
        //            _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
        //            _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
        //            _ChangeCheck.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
        //            _ChangeCheck.Controls.Add(_CurveFieldX);
        //            _ChangeCheck.Controls.Add(_CurveFieldY);
        //            _ChangeCheck.Controls.Add(_CurveFieldZ);
        //            _ChangeCheck.Controls.Add(_CurveFieldW);

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
        //            ((Vector4KeyView)_View).PropertyKey.ValueKey = _Value.Value;
        //            SetDirty();
        //        }

        //        protected override void RefreshData()
        //        {
        //            base.RefreshData();
        //            _Value.Value = ((Vector4KeyView)_View).PropertyKey.ValueKey;

        //            Vector4Key k = (Vector4Key)((Vector4KeyView)_View).Key;
        //            ValidateCurves();
        //        }

        //        void ValidateCurves()
        //        {
        //            _Value.Visibility = Skill.Framework.UI.Visibility.Collapsed;
        //            _ChangeCheck.Visibility = Skill.Framework.UI.Visibility.Visible;

        //            Vector4Key k = (Vector4Key)((Vector4KeyView)_View).Key;
        //            if (k.CurveX != null) _CurveFieldX.Curve = k.CurveX;
        //            if (k.CurveY != null) _CurveFieldY.Curve = k.CurveY;
        //            if (k.CurveZ != null) _CurveFieldZ.Curve = k.CurveZ;
        //            if (k.CurveW != null) _CurveFieldW.Curve = k.CurveW;

        //            if (_CurveFieldX.Curve == null) _CurveFieldX.Curve = new AnimationCurve();
        //            if (_CurveFieldY.Curve == null) _CurveFieldY.Curve = new AnimationCurve();
        //            if (_CurveFieldZ.Curve == null) _CurveFieldZ.Curve = new AnimationCurve();
        //            if (_CurveFieldW.Curve == null) _CurveFieldW.Curve = new AnimationCurve();

        //            k.CurveX = _CurveFieldX.Curve;
        //            k.CurveY = _CurveFieldY.Curve;
        //            k.CurveZ = _CurveFieldZ.Curve;
        //            k.CurveW = _CurveFieldW.Curve;

        //        }
        //    }
        //}


    }
}