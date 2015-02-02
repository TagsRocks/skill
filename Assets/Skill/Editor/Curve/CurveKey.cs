using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.Curve
{
    public class CurveKey : CanvasPanel, Skill.Editor.UI.Extended.IProperties, Skill.Editor.UI.ISelectable
    {
        class CurveKeyDragThumb : Skill.Editor.UI.Extended.DragThumb
        {
            private CurveKey _Key;

            public CurveKeyDragThumb(CurveKey key)
            {
                this._Key = key;
            }

            protected override void OnDrag(Vector2 delta)
            {
                this._Key.Track.View.Editor.KeyDrag(_Key, delta, Event.current.control);
            }
        }

        private CurveKeyDragThumb _DragThumb;
        private TangentHandle _HandleLeft;
        private TangentHandle _HandleRight;

        internal Vector2 StartDrag;

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (value != _IsSelected)
                {
                    _IsSelected = value;
                }
            }
        }


        public int Index { get; private set; }
        public CurveTrack Track { get; private set; }
        public Keyframe Keyframe
        {
            get { return Track.Curve[Mathf.Clamp(Index, 0, Track.Curve.length - 1)]; }
            set
            {
                Track.Curve.MoveKey(Index, value);
                OnLayoutChanged();
            }
        }
        public AnimationCurve Curve { get { return Track.Curve; } }

        public CurveKey(CurveTrack track, int index)
        {
            this.Track = track;
            this.Index = index;

            _DragThumb = new CurveKeyDragThumb(this) { Style = new GUIStyle(), Position = new Rect(0, 0, Size, Size) };
            this.Controls.Add(_DragThumb);
            _DragThumb.MouseDown += _DragThumb_MouseDown;

            if (Index > 0)
            {
                _HandleLeft = new TangentHandleLeft(this) { Visibility = Skill.Framework.UI.Visibility.Hidden };
                this.Controls.Add(_HandleLeft);
            }

            if (Index < Curve.length - 1)
            {
                _HandleRight = new TangentHandleRight(this) { Visibility = Skill.Framework.UI.Visibility.Hidden };
                this.Controls.Add(_HandleRight);
            }
        }

        void _DragThumb_MouseDown(object sender, MouseClickEventArgs args)
        {
            if (args.Button == MouseButton.Left || args.Button == MouseButton.Right)
            {
                if (args.Shift)
                    Track.View.Editor.Selection.Add(this);
                else if (args.Ctrl)
                    Track.View.Editor.Selection.Remove(this);
                else
                    Track.View.Editor.Selection.Select(this);
            }
        }

        public override Rect Position
        {
            get
            {
                Keyframe key = this.Keyframe;
                Vector2 v = Track.GetPoint(key.time, key.value, true);
                return new Rect(v.x - (Width * 0.5f), v.y - (Height * 0.5f), Width, Height);
            }
            set
            {
                Keyframe key = this.Keyframe;
                key.time = Track.GetTime(value.x + (Width * 0.5f), true);
                key.value = Track.GetValue(value.y + (Height * 0.5f), true);
                Track.MoveKey(Index, key);
                base.Position = value;
            }
        }
        private const float Size = 9;
        public override float Width { get { return Size; } set { base.Width = Size; } }
        public override float Height { get { return Size; } set { base.Height = Size; } }
        public override float X
        {
            get { return Track.GetX(this.Keyframe.time, true) - (Width * 0.5f); }
            set
            {
                Keyframe key = this.Keyframe;
                key.time = Track.GetTime(value + (Width * 0.5f), true);
                key.time = ClampTime(key.time);
                Track.MoveKey(Index, key);
                base.X = value;
            }
        }
        public override float Y
        {
            get
            {
                return Track.GetY(this.Keyframe.value, true) - (Height * 0.5f);
            }
            set
            {
                Keyframe key = this.Keyframe;
                key.value = Track.GetValue(value + (Height * 0.5f), true);
                Track.MoveKey(Index, key);
                base.Y = value;
            }
        }

        protected override void Render()
        {
            Rect ra = RenderArea;
            Color savedColor = GUI.color;
            GUI.color = Track.Color;
            GUI.DrawTexture(ra, Skill.Editor.Resources.UITextures.Keyframe);

            if (IsSelected)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(ra, Skill.Editor.Resources.UITextures.KeyframeSelected);
            }
            GUI.color = savedColor;

            base.Render();
        }

        protected override void BeginRender()
        {
            if (IsSelected)
            {
                if (_HandleLeft != null)
                {
                    if (Skill.Editor.CurveUtility.GetKeyTangentMode(Keyframe, 0) == Skill.Editor.CurveUtility.TangentMode.Editable)
                    {
                        _HandleLeft.Visibility = Skill.Framework.UI.Visibility.Visible;
                        _HandleLeft.UpdatePosition();
                    }
                    else
                    {
                        _HandleLeft.Visibility = Skill.Framework.UI.Visibility.Hidden;
                    }
                }
                if (_HandleRight != null)
                {
                    if (Skill.Editor.CurveUtility.GetKeyTangentMode(Keyframe, 1) == Skill.Editor.CurveUtility.TangentMode.Editable)
                    {
                        _HandleRight.Visibility = Skill.Framework.UI.Visibility.Visible;
                        _HandleRight.UpdatePosition();
                    }
                    else
                    {
                        _HandleRight.Visibility = Skill.Framework.UI.Visibility.Hidden;
                    }
                }
            }
            else
            {
                if (_HandleLeft != null)
                    _HandleLeft.Visibility = Skill.Framework.UI.Visibility.Hidden;
                if (_HandleRight != null)
                    _HandleRight.Visibility = Skill.Framework.UI.Visibility.Hidden;
            }
            base.BeginRender();
        }

        private float ClampTime(float time)
        {
            if (Index > 0)
            {
                if (Index == 1 || Index == Curve.length - 1)
                    time = Mathf.Max(time, Curve[Index - 1].time + 0.01f);
                else
                    time = Mathf.Max(time, Curve[Index - 1].time);
            }
            if (Index < Curve.length - 1)
            {
                if (Index == 0 || Index == Curve.length - 2)
                    time = Mathf.Min(time, Curve[Index + 1].time - 0.01f);
                else
                    time = Mathf.Min(time, Curve[Index + 1].time);
            }
            return time;
        }

        #region IProperties members
        public bool IsSelectedProperties { get; set; }

        private Skill.Editor.UI.Extended.PropertiesPanel _Properties;
        public Skill.Editor.UI.Extended.PropertiesPanel Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = new CurveKeyProperties(this);
                return _Properties;
            }
        }
        public string Title { get { return string.Format("Keyframe {0}", Index); } }


        [Skill.Framework.ExposeProperty(1, "Time")]
        public float Time
        {
            get { return Keyframe.time; }
            set
            {
                Keyframe k = this.Keyframe;
                k.time = ClampTime(value);
                this.Keyframe = k;
            }
        }

        [Skill.Framework.ExposeProperty(2, "Value")]
        public float Value
        {
            get { return Keyframe.value; }
            set
            {
                Keyframe k = this.Keyframe;
                k.value = value;
                this.Keyframe = k;
            }
        }

        //[Skill.Framework.ExposeProperty(3, "In Tangent")]
        //public float InTangent
        //{
        //    get { return Keyframe.inTangent; }
        //    set
        //    {
        //        Keyframe k = this.Keyframe;
        //        k.inTangent = value;
        //        this.Keyframe = k;
        //    }
        //}

        //[Skill.Framework.ExposeProperty(4, "Out Tangent")]
        //public float OutTangent
        //{
        //    get { return Keyframe.outTangent; }
        //    set
        //    {
        //        Keyframe k = this.Keyframe;
        //        k.outTangent = value;
        //        this.Keyframe = k;
        //    }
        //}

        #endregion
    }

    class CurveKeyComparer : System.Collections.Generic.IComparer<CurveKey>
    {
        private static CurveKeyComparer _Instance;

        public static CurveKeyComparer Instance
        {
            get
            {
                if (_Instance == null) _Instance = new CurveKeyComparer();
                return _Instance;
            }
        }

        public int Compare(CurveKey x, CurveKey y) { return x.Time.CompareTo(y.Time); }

        private CurveKeyComparer()
        {

        }
    }
}
