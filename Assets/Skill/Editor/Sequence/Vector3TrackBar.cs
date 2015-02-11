using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;
using Skill.Editor.UI;

namespace Skill.Editor.Sequence
{
    public class Vector3TrackBar : ContinuousTrackBar<Vector3>
    {
        
        private Vector3Track _Vector3Track;

        public Vector3TrackBar(Vector3Track track)
            : base(track)
        {
            _Vector3Track = track;
            CheckCurves();
            Refresh();
        }

        //public override void UpdateDefaultValue(Vector3 defaultValue)
        //{
        //    base.UpdateDefaultValue(defaultValue);
        //    CheckCurves();
        //    ValidateCurve(_Vector3Track.CurveX, defaultValue.x);
        //    ValidateCurve(_Vector3Track.CurveY, defaultValue.y);
        //    ValidateCurve(_Vector3Track.CurveZ, defaultValue.z);
        //}

        private void CheckCurves()
        {
            if (_Vector3Track.CurveX == null) _Vector3Track.CurveX = new AnimationCurve();
            if (_Vector3Track.CurveY == null) _Vector3Track.CurveY = new AnimationCurve();
            if (_Vector3Track.CurveZ == null) _Vector3Track.CurveZ = new AnimationCurve();
        }


        protected override void AddCurveKey(KeyType keyType, float time)
        {
            Vector3 sceneValue = new Vector3();
            object v = _Vector3Track.GetValue();
            if (v != null)
            {
                sceneValue = (Vector3)v;
            }
            else
            {
                sceneValue.x = _Vector3Track.CurveX.Evaluate(time);
                sceneValue.y = _Vector3Track.CurveY.Evaluate(time);
                sceneValue.z = _Vector3Track.CurveZ.Evaluate(time);
            }

            if ((keyType & KeyType.X) == KeyType.X)
                AddKeyToCurve(_Vector3Track.CurveX, time, sceneValue.x);
            if ((keyType & KeyType.Y) == KeyType.Y)
                AddKeyToCurve(_Vector3Track.CurveY, time, sceneValue.y);
            if ((keyType & KeyType.Z) == KeyType.Z)
                AddKeyToCurve(_Vector3Track.CurveZ, time, sceneValue.z);
        }

        protected override KeyType GetEquality(Vector3 v1, Vector3 v2)
        {
            KeyType equality = KeyType.None;

            if (Mathf.Approximately(v1.x, v2.x)) equality |= KeyType.X;
            if (Mathf.Approximately(v1.y, v2.y)) equality |= KeyType.Y;
            if (Mathf.Approximately(v1.z, v2.z)) equality |= KeyType.Z;            
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
        //        Vector3Track vt = (Vector3Track)Track;
        //        Vector3Key vk = (Vector3Key)vt.PropertyKeys[0];


        //        Vector3 curveValue = new Vector3(vk.CurveX.Evaluate(time), vk.CurveY.Evaluate(time), vk.CurveZ.Evaluate(time));
        //        Vector3 sceneValue = curveValue;
        //        object v = vt.GetValue();
        //        if (v != null)
        //            sceneValue = (Vector3)v;

        //        Vector3 value = curveValue;
        //        if ((this.RecordState & RecordState.X) != 0) value.x = sceneValue.x;
        //        if ((this.RecordState & RecordState.Y) != 0) value.y = sceneValue.y;
        //        if ((this.RecordState & RecordState.Z) != 0) value.z = sceneValue.z;

        //        vt.SetValue(value);
        //    }
        //}

        //class Vector3KeyView : PropertyTimeLineEvent
        //{
        //    public override double Duration
        //    {
        //        get
        //        {
        //            if (_Vector3Key.CurveX != null && _Vector3Key.CurveY != null && _Vector3Key.CurveZ != null)
        //            {
        //                float max = 0;
        //                if (_Vector3Key.CurveX.length > 0)
        //                    max = Mathf.Max(max, _Vector3Key.CurveX.keys[_Vector3Key.CurveX.length - 1].time);
        //                if (_Vector3Key.CurveY.length > 0)
        //                    max = Mathf.Max(max, _Vector3Key.CurveY.keys[_Vector3Key.CurveY.length - 1].time);
        //                if (_Vector3Key.CurveZ.length > 0)
        //                    max = Mathf.Max(max, _Vector3Key.CurveZ.keys[_Vector3Key.CurveZ.length - 1].time);
        //                return max;
        //            }
        //            else
        //                return 0.05f;
        //        }
        //        set { }
        //    }
        //    public override string Title { get { return "Vector3 Event"; } }

        //    private float _MinWidth;
        //    private Vector3 _PreValue;
        //    public override float MinWidth
        //    {
        //        get
        //        {
        //            if (_Vector3Key.CurveX != null && _Vector3Key.CurveY != null && _Vector3Key.CurveZ != null)
        //                return 22;
        //            else
        //            {
        //                if (_MinWidth < 1f || _PreValue != _Vector3Key.Value)
        //                {
        //                    _PreValue = _Vector3Key.Value;
        //                    GUIStyle labelStyle = "Label";
        //                    GUIContent content = new GUIContent() { text = _Vector3Key.Value.ToString() };
        //                    _MinWidth = labelStyle.CalcSize(content).x;
        //                }
        //                return _MinWidth;
        //            }
        //        }
        //    }
        //    protected override bool CanDrag { get { return false; } }
        //    protected override PropertiesPanel CreateProperties() { return new Vector3KeyViewProperties(this); }

        //    private Vector3Key _Vector3Key;
        //    public Vector3KeyView(Vector3TrackBar trackbar, IPropertyKey<Vector3> key)
        //        : base(trackbar, key)
        //    {
        //        _Vector3Key = (Vector3Key)key;
        //    }


        //    private Rect[] _CurevRenderAreas = new Rect[3];
        //    private Rect[] _CurevRanges = new Rect[3];
        //    protected override void Render()
        //    {
        //        base.Render();
        //        if (_Vector3Key.CurveX != null && _Vector3Key.CurveY != null && _Vector3Key.CurveZ != null)
        //        {
        //            CalcCurveRenderArea(ref _CurevRenderAreas, ref _CurevRanges, _Vector3Key.CurveX, _Vector3Key.CurveY, _Vector3Key.CurveZ);
        //            if (_CurevRenderAreas[0].width > 0.1f)
        //                DrawCurve(_CurevRenderAreas[0], _CurevRanges[0], _Vector3Key.CurveX, CurveXColor);
        //            if (_CurevRenderAreas[1].width > 0.1f)
        //                DrawCurve(_CurevRenderAreas[1], _CurevRanges[1], _Vector3Key.CurveY, CurveYColor);
        //            if (_CurevRenderAreas[2].width > 0.1f)
        //                DrawCurve(_CurevRenderAreas[2], _CurevRanges[2], _Vector3Key.CurveZ, CurveZColor);
        //        }
        //        //else
        //        //{
        //        //    GUI.Label(RenderArea, PropertyKey.ValueKey.ToString());
        //        //}

        //    }



        //    class Vector3KeyViewProperties : EventProperties
        //    {
        //        private Skill.Editor.UI.Vector3Field _Value;
        //        private Skill.Editor.UI.CurveField _CurveFieldX;
        //        private Skill.Editor.UI.CurveField _CurveFieldY;
        //        private Skill.Editor.UI.CurveField _CurveFieldZ;
        //        private Skill.Editor.UI.ChangeCheck _ChangeCheck;

        //        public Vector3KeyViewProperties(Vector3KeyView e)
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
        //            ((Vector3KeyView)_View).PropertyKey.ValueKey = _Value.Value;
        //            SetDirty();
        //        }

        //        protected override void RefreshData()
        //        {
        //            base.RefreshData();
        //            _Value.Value = ((Vector3KeyView)_View).PropertyKey.ValueKey;
        //            ValidateCurves();
        //        }

        //        void ValidateCurves()
        //        {
        //            _Value.Visibility = Skill.Framework.UI.Visibility.Collapsed;
        //            _ChangeCheck.Visibility = Skill.Framework.UI.Visibility.Visible;

        //            Vector3Key k = (Vector3Key)((Vector3KeyView)_View).Key;
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