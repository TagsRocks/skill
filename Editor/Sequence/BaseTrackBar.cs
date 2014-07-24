using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{


    /// <summary>
    ///  Base class for TrackBar in Matinee editor
    /// </summary>
    public abstract class BaseTrackBar : Skill.Editor.UI.Extended.TrackBar
    {
        protected static float BaseHeight = 18;

        /// <summary> Track to edit by this TrackBar </summary>
        public Track Track { get; private set; }

        public abstract bool IsContinuous { get; }

        public BaseTrackBar(Track track)
        {
            this.Track = track;
            this.Height = BaseHeight;
        }

        protected override void Render()
        {
            Rect ra = RenderArea;
            // draw background box at entire RenderArea   
            if (Track.Color.a > 0)
                UnityEditor.EditorGUI.DrawRect(ra, Track.Color);
            base.Render();
        }

        /// <summary>
        /// Refresh data do to changes outside of MatineeEditor
        /// </summary>
        public virtual void Refresh() { }

        /// <summary>
        /// Set track as dirty
        /// </summary>
        public virtual void SetDirty()
        {
            UnityEditor.EditorUtility.SetDirty(Track);
            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
            this.OnLayoutChanged();
        }

        public bool IsEditingCurves
        {
            get { return Track.IsEditingCurves; }
            set
            {
                if (Track.IsEditingCurves != value)
                {
                    Track.IsEditingCurves = value;
                    SetDirty();
                }
            }

        }

        internal virtual void Evaluate(float time)
        {
            if (Track != null)
                Track.Evaluate(time);
        }
        internal virtual void Seek(float time)
        {
            if (Track != null)
                Track.Seek(time);
        }


        internal virtual void AddCurveKey(KeyType keyType) { }

        public abstract void AddKey();

        public virtual void SaveRecordState() { }
        public virtual void AutoKey() { }
        public abstract void Delete(KeyView keyView);

        public override void GetTimeBounds(out double minTime, out double maxTime)
        {
            if (Track != null)
            {
                float fminTime, fmaxTime;
                Track.GetTimeBounds(out fminTime, out fmaxTime);
                minTime = fminTime;
                maxTime = fmaxTime;
            }
            else
            {
                minTime = 0;
                maxTime = 0.1f;
            }
        }
    }
}