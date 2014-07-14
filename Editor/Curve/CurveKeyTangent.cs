using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
namespace Skill.Editor.Curve
{
    abstract class TangentHandle : Skill.Framework.UI.Image
    {
        public static Color TangentColor = new Color(1.0f, 1.0f, 1.0f, 0.6f);
        public const float TangentLenght = 60;

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
            Skill.Editor.LineDrawer.DrawLineGL(center, keyCenter, TangentColor);
            base.Render();
        }

        private bool _IsMouseDown;
        protected override void OnMouseDown(MouseClickEventArgs args)
        {
            if (Parent != null && args.Button == MouseButton.Left)
            {
                Frame of = OwnerFrame;
                if (of != null)
                {
                    _IsMouseDown = OwnerFrame.RegisterPrecedenceEvent(this);
                    args.Handled = true;
                }
            }
            base.OnMouseDown(args);
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
                    UpdateTangent(e.mousePosition + Key.Track.View.ScrollPosition);
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
            Keyframe key = Key.Keyframe;
            float angle = Mathf.Atan(key.inTangent);
            float x = -Mathf.Cos(angle) * TangentLenght;
            float y = Mathf.Sin(angle) * TangentLenght;
            Position = new Rect(x, y, 8, 8);
        }

        protected override void UpdateTangent(Vector2 mousePosition)
        {
            Rect ra = Key.RenderArea;
            Vector2 center = ra.center;
            Keyframe keyframe = Key.Keyframe;

            if (mousePosition.x > center.x) mousePosition.x = center.x;
            Vector2 delta = mousePosition - center;
            delta.Normalize();

            if (delta.x >= -0.0001f)
            {
                if (delta.y < 0)
                    keyframe.outTangent = float.NegativeInfinity;
                else
                    keyframe.outTangent = float.PositiveInfinity;
            }
            else
                keyframe.inTangent = -delta.y / delta.x;

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
            Keyframe key = Key.Keyframe;
            float angle = Mathf.Atan(key.outTangent);
            float x = Mathf.Cos(angle) * TangentLenght;
            float y = -Mathf.Sin(angle) * TangentLenght;
            Position = new Rect(x, y, 8, 8);
        }

        protected override void UpdateTangent(Vector2 mousePosition)
        {
            Rect ra = Key.RenderArea;
            Vector2 center = ra.center;
            Keyframe keyframe = Key.Keyframe;

            if (mousePosition.x < center.x) mousePosition.x = center.x;
            Vector2 delta = mousePosition - center;
            delta.Normalize();

            if (delta.x > 0.0001f)
                keyframe.outTangent = -delta.y / delta.x;
            else
            {
                if (delta.y < 0)
                    keyframe.outTangent = float.NegativeInfinity;
                else
                    keyframe.outTangent = float.PositiveInfinity;
            }


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