using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public enum RecordState
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        W = 8,
        XY = X | Y,
        XYZ = X | Y | Z,
        XYZW = X | Y | Z | W
    }

    /// <summary>
    ///  Base class for TrackBar in Matinee editor
    /// </summary>
    public abstract class BaseTrackBar : Skill.Editor.UI.Extended.TrackBar
    {

        private static GUIStyle _NameStyle;
        private static GUIStyle NameStyle
        {
            get
            {
                if (_NameStyle == null)
                {
                    GUIStyle labelStyle = "Label";
                    _NameStyle = new GUIStyle(labelStyle);
                    _NameStyle.alignment = TextAnchor.LowerLeft;
                    _NameStyle.fontStyle = FontStyle.Bold;
                    _NameStyle.fontSize = 11;
                    _NameStyle.padding = new RectOffset(2, 0, 0, 0);
                    _NameStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f, 0.5f);
                }
                return _NameStyle;
            }
        }
        /// <summary> Track to edit by this TrackBar </summary>
        public Track Track { get; private set; }

        public BaseTrackBar(Track track)
        {
            this.Track = track;
            this.Height = 22;
            this.Margin = new Skill.Framework.UI.Thickness(0, 1);
        }


        protected override void Render()
        {
            Rect ra = RenderArea;
            Color savedColor = GUI.color;

            // draw background box at entire RenderArea            
            Color c = Track.Color;
            c.a *= 0.4f;
            GUI.color = c;
            GUI.DrawTexture(ra, UnityEditor.EditorGUIUtility.whiteTexture);

            GUI.color = savedColor;

            base.Render();

            // draw name of track at bottom left corner of trackbar            
            Rect rect = ((TrackBarView)Parent).RenderArea;
            rect.x += ((TrackBarView)Parent).ScrollPosition.x;
            rect.y = ra.y - 4;
            rect.height = ra.height;
            rect.width = ((TimeLine)((TrackBarView)Parent).TimeLine).RenderArea.width;
            UnityEditor.EditorGUI.DropShadowLabel(rect, Track.name, NameStyle);
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

        public override double GetValidTime() { return Track.Length; }

        public RecordState RecordState
        {
            get { return (RecordState)Track.RecordState; }
            set
            {
                if ((RecordState)Track.RecordState != value)
                {
                    Track.RecordState = (int)value;
                    SetDirty();
                }
            }

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

        public abstract ITrackKey FirstKey { get; }

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
        internal virtual void AddKey(RecordState record)
        {

        }


    }


    /// <summary>
    /// base class for all TimeLineEvent used in Matinee Editor
    /// </summary>
    public abstract class EventView : TimeLineEvent, IProperties
    {
        public static Color CurveBgColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        public static Color CurveColor = new Color(1.0f, 0.0f, 1.0f);
        public static Color CurveXColor = new Color(1.0f, 0.0f, 0.0f);
        public static Color CurveYColor = new Color(0.0f, 1.0f, 0.0f);
        public static Color CurveZColor = new Color(0.0f, 0.0f, 1.0f);
        public static Color CurveWColor = new Color(1.0f, 1.0f, 0.0f);

        private PropertiesPanel _Properties;
        /// <summary> Properties </summary>
        public PropertiesPanel Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = CreateProperties();
                return _Properties;
            }
        }

        protected virtual PropertiesPanel CreateProperties()
        {
            return new EventProperties(this);
        }
        /// <summary> Type of TimeLineEvent to show in PropertyGrid </summary>
        public virtual string Title { get { return "Event"; } }

        private bool _SelectedProperties;
        /// <summary> Is selected by PropertyGrid </summary>
        public virtual bool IsSelectedProperties
        {
            get { return _SelectedProperties; }
            set
            {
                if (_SelectedProperties != value)
                {
                    _SelectedProperties = value;
                    if (_SelectedProperties) BringToFront();
                    OnLayoutChanged();
                }
            }
        }

        public ITrackKey Key { get; private set; }

        protected virtual bool CanDrag { get { return true; } }

        private TimeLineEventDragDumb _Drag;

        public GUIStyle DragStyle { get { return _Drag.Style; } set { _Drag.Style = value; } }

        public override double FireTime { get { return Key.FireTime; } set { Key.FireTime = (float)value; Properties.Refresh(); } }

        /// <summary>
        /// Create apropriate properties based on type of TimeLineEvent
        /// </summary>
        /// <returns></returns>
        //protected abstract Properties CreateProperties();
        public EventView(BaseTrackBar trackBar, ITrackKey key)
            : base(trackBar)
        {
            this.Margin = new Skill.Framework.UI.Thickness(0, 2);
            this.Key = key;
            if (CanDrag)
            {
                _Drag = new TimeLineEventDragDumb(this) { Row = 0, Column = 0, RowSpan = 100, ColumnSpan = 100 };
                Controls.Add(_Drag);
            }
            base.WantsMouseEvents = true;
        }


        private TrackBarView _TrackView;
        /// <summary>
        /// Owner TrackView in upper hierarchy
        /// </summary>
        public TrackBarView TrackView
        {
            get
            {
                if (_TrackView == null)
                    _TrackView = FindInParents<TrackBarView>();
                return _TrackView;
            }
        }


        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="args">MouseClickEventArgs</param>
        protected override void OnMouseDown(Skill.Framework.UI.MouseClickEventArgs args)
        {
            // show properties when used select this event 
            if (args.Button == Skill.Framework.UI.MouseButton.Left)
            {
                if (MatineeEditorWindow.Instance != null)
                {
                    MatineeEditorWindow.Instance.PropertyGrid.SelectedObject = this;
                }
            }
            base.OnMouseDown(args);
        }

        /// <summary>
        /// Render
        /// </summary>
        protected override void Render()
        {
            GUI.Box(RenderArea, string.Empty, Skill.Editor.Resources.Styles.BackgroundShadow);
            base.Render();
            if (IsSelectedProperties)
                // draw a border around event to show selected
                GUI.Box(RenderArea, string.Empty, Resources.Styles.SelectedEventBorder);
        }

    }

    public class EventProperties : ExposeProperties
    {
        protected EventView _View;
        public EventProperties(EventView view)
            : base(view.Key)
        {
            _View = view;
        }
        protected override void SetDirty()
        {
            ((BaseTrackBar)_View.TrackBar).SetDirty();
        }
    }
}