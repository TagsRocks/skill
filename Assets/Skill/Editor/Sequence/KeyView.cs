using UnityEngine;
using System.Collections;
using Skill.Editor.UI;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    /// <summary>
    /// base class for all TimeLineEvent used in Matinee Editor
    /// </summary>
    public abstract class KeyView : TimeLineEvent, IProperties
    {

        /// <summary> Properties </summary>
        public abstract PropertiesPanel Properties { get; }
        /// <summary> Type of TimeLineEvent to show in PropertyGrid </summary>
        public abstract string Title { get; }

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

        private static GUIStyle _DragStyle;
        private TimeLineEventDragDumb _Drag;

        public KeyView(BaseTrackBar trackBar)
            : base(trackBar)
        {
            this.Margin = new Skill.Framework.UI.Thickness(0, 2);
            _Drag = new TimeLineEventDragDumb(this) { Row = 0, Column = 0, RowSpan = 100, ColumnSpan = 100 };
            if (_DragStyle == null) _DragStyle = new GUIStyle();
            _Drag.Style = _DragStyle;
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
                InspectorProperties.Select(this);
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
}
