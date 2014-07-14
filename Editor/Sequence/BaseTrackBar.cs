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
        protected static float BaseHeight = 20;

        /// <summary> Track to edit by this TrackBar </summary>
        public Track Track { get; private set; }

        public abstract bool IsContinuous { get; }

        public BaseTrackBar(Track track)
        {
            this.Track = track;
            this.Height = BaseHeight;
            this.Margin = new Skill.Framework.UI.Thickness(0, 1);
        }

        protected override void Render()
        {
            Rect ra = RenderArea;
            // draw background box at entire RenderArea                        
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

        public override double Length { get { return Track.Length; } }

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


        public virtual void SaveRecordState() { }
        public virtual void AutoKey() { }
    }
}