﻿using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    /// <summary>
    ///  Base class for TrackBar in Matinee editor
    /// </summary>
    public class BaseTrackBar : Skill.Editor.UI.Extended.TrackBar
    {
        private static Color _NameColor = new Color(0.85f, 0.85f, 0.85f, 0.6f);// color used to draw name of track at bottom left corner of trackbar

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

            // draw name of track at bottom left corner of trackbar
            GUI.color = _NameColor;
            Rect rect = ((TrackBarView)Parent).RenderArea;
            rect.x += ((TrackBarView)Parent).ScrollPosition.x;
            rect.y = ra.y - 4;
            rect.height = ra.height;
            rect.width = Track.name.Length * 8;
            UnityEditor.EditorGUI.DropShadowLabel(rect, Track.name);

            GUI.color = savedColor;
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

        public override double GetValidTime() { return Track.Length; }
    }


    /// <summary>
    /// base class for all TimeLineEvent used in Matinee Editor
    /// </summary>
    public abstract class EventView : TimeLineEvent, IProperties
    {
        public static Color CurveBgColor = new Color(0.1f, 0.1f, 0.1f);
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
            _Drag = new TimeLineEventDragDumb(this) { Row = 0, Column = 0, RowSpan = 100, ColumnSpan = 100 };
            Controls.Add(_Drag);
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