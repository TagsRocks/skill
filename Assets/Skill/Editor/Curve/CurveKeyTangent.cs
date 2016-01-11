using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
namespace Skill.Editor.Curve
{
    abstract class TangentHandle : Skill.Framework.UI.Image
    {
        public static Color TangentColor = new Color(1.0f, 1.0f, 1.0f, 0.6f);
        public const float TangentLenght = 50;
        protected static Vector2 TangentScale = new Vector2(1f, -1f);

        public CurveKey Key { get; private set; }

        public TangentHandle(CurveKey key)
        {
            Key = key;
            Scale = ScaleMode.StretchToFill;
            TintColor = TangentColor;
            WantsMouseEvents = true;
        }

        protected abstract void UpdateTangent(Vector2 mousePosition);
        public abstract void UpdatePosition();

        protected override void Render()
        {
            if (Texture == null)
                Texture = Skill.Editor.Resources.UITextures.KeyframeSelected;

            Vector2 keyCenter = Key.RenderArea.center;
            Vector2 center = RenderArea.center;
            Skill.Editor.LineDrawer.DrawLine(center, keyCenter, TangentColor);
            base.Render();
        }

        private Vector2 _StartPosition;
        private Vector2 _EndPosition;
        private Vector2 _DeltaDrag;
        private bool _IsMouseDown;
        protected override void MouseDownEvent(MouseClickEventArgs args)
        {
            if (Parent != null && args.Button == MouseButton.Left)
            {
                Frame of = OwnerFrame;
                if (of != null)
                {
                    _IsMouseDown = OwnerFrame.RegisterPrecedenceEvent(this);
                    _StartPosition = _EndPosition = args.MousePosition;
                    _DeltaDrag = Vector2.zero;
                    args.Handled = true;
                }
            }
            base.MouseDownEvent(args);
        }

        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="e">event to handle</param>
        public override void HandleEvent(Event e)
        {
            if (_IsMouseDown && Parent != null && e != null)
            {
                if (e.type == EventType.MouseDrag)
                {
                    _DeltaDrag += e.delta;
                    _EndPosition = _StartPosition + _DeltaDrag;

                    UpdateTangent(new Vector2(Key.Track.GetTime(_EndPosition.x, false), Key.Track.GetValue(_EndPosition.y, false)));
                    e.Use();
                }
                else if (e.type == EventType.MouseUp && e.button == 0)
                {
                    Frame of = OwnerFrame;
                    if (of != null)
                    {
                        of.UnregisterPrecedenceEvent(this);
                        _IsMouseDown = false;
                        e.Use();
                    }
                }
            }
            else
                base.HandleEvent(e);
        }
    }

    class TangentHandleLeft : TangentHandle
    {

        public TangentHandleLeft(CurveKey key)
            : base(key)
        {

        }

        public override void UpdatePosition()
        {
            Keyframe keyframe = Key.Keyframe;
            Vector2 vector1 = new Vector2(keyframe.time, keyframe.value);
            Vector2 vector2 = new Vector2(1f, keyframe.inTangent);
            if (keyframe.inTangent == float.PositiveInfinity)
            {
                vector2 = new Vector2(0f, -1f);
            }
            vector2.Normalize();

            vector2 = vector1 - vector2;
            Vector2 point = Key.Track.GetPoint(vector2.x, vector2.y, false);

            Vector2 dir = (point - Key.RenderArea.center).normalized;
            point = dir * TangentLenght;

            Position = new Rect(point.x, point.y, 8, 8);

        }





        protected override void UpdateTangent(Vector2 mousePosition)
        {
            Keyframe keyframe = Key.Keyframe;
            Vector2 center = new Vector2(keyframe.time, keyframe.value);
            Vector2 delta = mousePosition - center;

            if (delta.x >= -0.0001f)
                keyframe.inTangent = float.PositiveInfinity;
            else
                keyframe.inTangent = delta.y / delta.x;

            Skill.Editor.CurveUtility.SetKeyTangentMode(ref keyframe, 0, Skill.Editor.CurveUtility.TangentMode.Editable);
            if (!Skill.Editor.CurveUtility.GetKeyBroken(keyframe))
            {
                keyframe.outTangent = keyframe.inTangent;
                Skill.Editor.CurveUtility.SetKeyTangentMode(ref keyframe, 1, Skill.Editor.CurveUtility.TangentMode.Editable);
            }


            Key.Curve.MoveKey(Key.Index, keyframe);
            Skill.Editor.CurveUtility.UpdateTangentsFromModeSurrounding(Key.Curve, Key.Index);




        }
    }

    class TangentHandleRight : TangentHandle
    {
        public TangentHandleRight(CurveKey key)
            : base(key)
        {

        }

        public override void UpdatePosition()
        {
            Keyframe keyframe = Key.Keyframe;
            Vector2 vector1 = new Vector2(keyframe.time, keyframe.value);
            Vector2 vector2 = new Vector2(1f, keyframe.outTangent);
            if (keyframe.outTangent == float.PositiveInfinity)
                vector2 = new Vector2(0f, -1f);
            else
                vector2.Normalize();

            vector2 = vector1 + vector2;
            Vector2 point = Key.Track.GetPoint(vector2.x, vector2.y, false);

            Vector2 dir = (point - Key.RenderArea.center).normalized;
            point = dir * TangentLenght;

            Position = new Rect(point.x, point.y, 8, 8);
        }

        protected override void UpdateTangent(Vector2 mousePosition)
        {
            Keyframe keyframe = Key.Keyframe;
            Vector2 center = new Vector2(keyframe.time, keyframe.value);
            Vector2 delta = mousePosition - center;
            delta.Normalize();

            if (delta.x > 0.0001f)
                keyframe.outTangent = delta.y / delta.x;
            else
                keyframe.outTangent = float.PositiveInfinity;


            Skill.Editor.CurveUtility.SetKeyTangentMode(ref keyframe, 1, Skill.Editor.CurveUtility.TangentMode.Editable);
            if (!Skill.Editor.CurveUtility.GetKeyBroken(keyframe))
            {
                keyframe.inTangent = keyframe.outTangent;
                Skill.Editor.CurveUtility.SetKeyTangentMode(ref keyframe, 0, Skill.Editor.CurveUtility.TangentMode.Editable);
            }



            Key.Curve.MoveKey(Key.Index, keyframe);
            Skill.Editor.CurveUtility.UpdateTangentsFromModeSurrounding(Key.Curve, Key.Index);
        }
    }


}