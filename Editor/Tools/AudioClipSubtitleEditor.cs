using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using Skill.Editor.UI.Extended;
using System.Collections.Generic;
using Skill.Editor;
using Skill.Framework.Audio;
using Skill.Framework;

namespace Skill.Editor.Tools
{

    public class AudioClipSubtitleEditor : Grid
    {
        #region SubtitleEvent
        class SubtitleEvent : TimeLineEvent, IProperties, ITextPickTarget
        {
            public override double Duration { get { return Subtitle.Duration; } set { Subtitle.Duration = (float)value; ((SubtitleTrackBar)TrackBar).SetDirty(); } }
            public override double FireTime { get { return Subtitle.Time; } set { Subtitle.Time = (float)value; ((SubtitleTrackBar)TrackBar).SetDirty(); } }

            public Subtitle Subtitle { get; private set; }
            public AudioClipSubtitleEditor Editor { get; private set; }

            private Label _LblTitle;
            private TimeLineEventDragDumb _DragDumb;

            public SubtitleEvent(AudioClipSubtitleEditor editor, TrackBar trackBar, Subtitle subtitle, GUIStyle style)
                : base(trackBar)
            {
                this.Editor = editor;
                Subtitle = subtitle;

                _DragDumb = new TimeLineEventDragDumb(this);

                _LblTitle = new Label() { Margin = new Thickness(2, 0) };
                _LblTitle.Style = new GUIStyle(style);

                Controls.Add(_DragDumb);
                Controls.Add(_LblTitle);

                base.WantsMouseEvents = true;
                UpdateTitle();
            }

            protected override void Render()
            {
                UpdateStyle();
                base.Render();
                if (IsSelectedProperties)
                    // draw a border around event to show selected
                    GUI.Box(RenderArea, string.Empty, Skill.Editor.Resources.Styles.SelectedEventBorder);
            }
            public void UpdateTitle()
            {
                if (!string.IsNullOrEmpty(Subtitle.TitleKey))
                    _LblTitle.Text = Editor.Editor.GetTitle(Subtitle.TitleKey);
                else
                    _LblTitle.Text = "None";
            }
            private void UpdateStyle()
            {

                _LblTitle.Style.font = Editor.Editor.Dictionary.Font;
                _LblTitle.Style.fontSize = Editor.Editor.Dictionary.FontSize;
                if (Subtitle.OverrideStyle)
                {
                    _LblTitle.Style.fontStyle = Subtitle.FontStyle;
                    _LblTitle.Style.normal.textColor = Subtitle.FontColor;
                    _LblTitle.Style.alignment = AlignmentToAnchor(Subtitle.Alignment);
                }
                else
                {
                    _LblTitle.Style.fontStyle = Editor.Editor.Dictionary.FontStyle;
                    _LblTitle.Style.normal.textColor = Editor.Editor.Dictionary.FontColor;
                    _LblTitle.Style.alignment = AlignmentToAnchor(Editor.Editor.Dictionary.Alignment);
                }

            }

            private TextAnchor AlignmentToAnchor(TextAlignment alignment)
            {
                if (alignment == TextAlignment.Center) return TextAnchor.UpperCenter;
                else if (alignment == TextAlignment.Left) return TextAnchor.UpperLeft;
                else return TextAnchor.UpperRight;
            }

            public void Pick(TextKey pickedKey)
            {
                Subtitle.TitleKey = pickedKey.Key;
                _LblTitle.Text = pickedKey.Value;
                Editor.Editor.SetDirty2();
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

            private bool _SelectedProperties;
            /// <summary> Is selected by PropertyGrid </summary>
            public bool IsSelectedProperties
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

            private SubtitleEventProperties _Properties;
            public PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null) _Properties = new SubtitleEventProperties(this);
                    return _Properties;
                }
            }
            public string Title { get { return "Subtitle"; } }
            class SubtitleEventProperties : PropertiesPanel
            {
                private Skill.Editor.UI.FloatField _TimeField;
                private Skill.Editor.UI.FloatField _DurationField;
                private Skill.Framework.UI.Button _BtnPickTitle;
                private Skill.Editor.UI.ToggleButton _TbOverrideStyle;
                private Skill.Editor.UI.ColorField _ColorField;
                private Skill.Editor.UI.EnumPopup _FontStyleField;
                private Skill.Editor.UI.EnumPopup _AlignmentField;

                private SubtitleEvent _Event;

                public SubtitleEventProperties(SubtitleEvent se)
                    : base(se)
                {
                    _Event = se;


                    _BtnPickTitle = new Skill.Framework.UI.Button() { Margin = ControlMargin };
                    _BtnPickTitle.Content.text = "Select Subtitle";
                    this.Controls.Add(_BtnPickTitle);

                    _TimeField = new Skill.Editor.UI.FloatField() { Margin = ControlMargin };
                    _TimeField.Label.text = "Time";
                    this.Controls.Add(_TimeField);

                    _DurationField = new Skill.Editor.UI.FloatField() { Margin = ControlMargin };
                    _DurationField.Label.text = "Duration";
                    this.Controls.Add(_DurationField);


                    _TbOverrideStyle = new Skill.Editor.UI.ToggleButton() { Margin = ControlMargin };
                    _TbOverrideStyle.Label.text = "Override Style";
                    this.Controls.Add(_TbOverrideStyle);

                    _ColorField = new Skill.Editor.UI.ColorField() { Margin = ControlMargin };
                    _ColorField.Label.text = "Font Color";
                    this.Controls.Add(_ColorField);

                    _FontStyleField = new Skill.Editor.UI.EnumPopup() { Margin = ControlMargin };
                    _FontStyleField.Label.text = "Font Style";
                    this.Controls.Add(_FontStyleField);

                    _AlignmentField = new Skill.Editor.UI.EnumPopup() { Margin = ControlMargin };
                    _AlignmentField.Label.text = "Alignment";
                    this.Controls.Add(_AlignmentField);

                    _BtnPickTitle.Click += _BtnPickTitle_Click;
                    _TimeField.ValueChanged += _TimeField_ValueChanged;
                    _DurationField.ValueChanged += _DurationField_ValueChanged;
                    _TbOverrideStyle.Changed += _TbOverrideStyle_Changed;
                    _ColorField.ColorChanged += _ColorField_ColorChanged;
                    _FontStyleField.ValueChanged += _FontStyleField_ValueChanged;
                    _AlignmentField.ValueChanged += _AlignmentField_ValueChanged;
                }

                protected override void RefreshData()
                {
                    _TimeField.Value = _Event.Subtitle.Time;
                    _DurationField.Value = _Event.Subtitle.Duration;
                    _TbOverrideStyle.IsChecked = _Event.Subtitle.OverrideStyle;
                    _ColorField.Color = _Event.Subtitle.FontColor;
                    _FontStyleField.Value = _Event.Subtitle.FontStyle;
                    _AlignmentField.Value = _Event.Subtitle.Alignment;
                    UpdateControlVisibilities();
                }

                private void UpdateControlVisibilities()
                {
                    Skill.Framework.UI.Visibility v = _Event.Subtitle.OverrideStyle ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;
                    _ColorField.Visibility = v;
                    _FontStyleField.Visibility = v;
                    _AlignmentField.Visibility = v;
                }

                protected override void SetDirty()
                {
                    _Event.Editor.Editor.SetDirty2();
                }

                void _AlignmentField_ValueChanged(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    _Event.Subtitle.Alignment = (TextAlignment)_AlignmentField.Value;
                    SetDirty();
                }

                void _FontStyleField_ValueChanged(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    _Event.Subtitle.FontStyle = (FontStyle)_FontStyleField.Value;
                    SetDirty();
                }

                void _ColorField_ColorChanged(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    _Event.Subtitle.FontColor = _ColorField.Color;
                    SetDirty();
                }

                void _TbOverrideStyle_Changed(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    _Event.Subtitle.OverrideStyle = _TbOverrideStyle.IsChecked;
                    UpdateControlVisibilities();
                    SetDirty();
                }

                void _DurationField_ValueChanged(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    if (_DurationField.Value < 0.001f) _DurationField.Value = 0.05f;
                    if (_Event.Editor.Subtitle.Clip != null)
                    {
                        if (_DurationField.Value > _Event.Editor.Subtitle.Clip.length - _Event.Subtitle.Time)
                            _DurationField.Value = _Event.Editor.Subtitle.Clip.length - _Event.Subtitle.Time;
                    }
                    _Event.Duration = _DurationField.Value;
                    SetDirty();
                }

                void _TimeField_ValueChanged(object sender, System.EventArgs e)
                {
                    if (IgnoreChanges) return;
                    if (_TimeField.Value < 0) _TimeField.Value = 0;
                    if (_Event.Editor.Subtitle.Clip != null)
                    {
                        if (_TimeField.Value > _Event.Editor.Subtitle.Clip.length) _TimeField.Value = _Event.Editor.Subtitle.Clip.length;
                    }
                    _Event.FireTime = _TimeField.Value;
                    SetDirty();
                }

                void _BtnPickTitle_Click(object sender, System.EventArgs e)
                {
                    _Event.Editor.Editor.ShowTextPicker(_Event);
                }
            }
        }
        #endregion

        #region SubtitleTrackBar

        class SubtitleTrackBar : TrackBar
        {
            private AudioClipSubtitleEditor _Editor;

            public SubtitleTrackBar(AudioClipSubtitleEditor editor)
            {
                _Editor = editor;
            }

            protected override void Render()
            {
                GUI.Box(RenderArea, string.Empty);
                base.Render();
            }

            public void SetDirty()
            {
                Invalidate();
            }

            public override void GetTimeBounds(out double minTime, out double maxTime)
            {
                minTime = 0;
                if (_Editor._AudioPreview.Clip != null)
                    maxTime = _Editor._AudioPreview.Clip.length;
                else
                    maxTime = 1.0f;
            }
        }

        #endregion

        private TimeLine _TimeLine;
        private TrackBar _AudioTrack;
        private SubtitleTrackBar _SubtitleTrack;
        private Skill.Editor.UI.ContextMenu _SubtitleTrackContextMenu;
        private Skill.Editor.UI.ContextMenu _SubtitleEventContextMenu;

        private AudioPreviewCurve _AudioPreview;
        private GUIStyle _TitleStyle;
        private List<SubtitleEvent> _Events;
        private Box _ToolbarBg;
        private MediaButton _BtnPlay;
        private AudioSource _Audio;

        private bool _Refresh;
        private bool _IsPlaying;
        private double _PlayStartTime;

        private AudioClipSubtitle _Subtitle;
        public AudioClipSubtitle Subtitle
        {
            get { return _Subtitle; }
            set
            {
                if (_Subtitle != value)
                {
                    _Subtitle = value;
                    _Refresh = true;
                }
                IsEnabled = _Subtitle != null;
            }
        }

        public DictionaryEditorWindow Editor { get; private set; }

        public AudioClipSubtitleEditor(DictionaryEditorWindow editor)
        {
            this.Editor = editor;
            GameObject obj = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("AudioPreview", HideFlags.HideAndDontSave | HideFlags.HideInHierarchy,
               new System.Type[] { typeof(AudioSource) });
            _Audio = obj.audio;

            _Events = new List<SubtitleEvent>();

            RowDefinitions.Add(16, GridUnitType.Pixel);
            RowDefinitions.Add(26, GridUnitType.Pixel);
            RowDefinitions.Add(140, GridUnitType.Pixel);
            RowDefinitions.Add(80, GridUnitType.Pixel);
            RowDefinitions.Add(1, GridUnitType.Star);

            _TimeLine = new TimeLine(new TrackBarView()) { Row = 1, Column = 0, RowSpan = 3, SelectionEnable = false, ExtendTime = false };
            _TimeLine.MaxTime = 1;

            _ToolbarBg = new Box() { Row = 0, Column = 0, Margin = new Thickness(0, 0, 16, 0) };
            _BtnPlay = new MediaButton() { Row = 0, Column = 0, Width = 28, HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Left, TogglePressed = false, Margin = new Thickness(4, 0, 0, 0) };
            _BtnPlay.Content.tooltip = "Start preview playback from current position";

            _AudioPreview = new AudioPreviewCurve() { Row = 2, Margin = new Thickness(0, 0, 16, 0) };
            _AudioTrack = new TrackBar() { Height = 138 };
            _SubtitleTrack = new SubtitleTrackBar(this) { Height = 60 };
            _TimeLine.View.Controls.Add(_AudioTrack);
            _TimeLine.View.Controls.Add(_SubtitleTrack);

            this.Controls.Add(_ToolbarBg);
            this.Controls.Add(_BtnPlay);
            this.Controls.Add(_AudioPreview);
            this.Controls.Add(_TimeLine);


            // ************** ContextMenu **************
            _SubtitleTrackContextMenu = new Skill.Editor.UI.ContextMenu();
            Skill.Editor.UI.MenuItem addSubtitle = new Skill.Editor.UI.MenuItem("Add");

            _SubtitleTrackContextMenu.Add(addSubtitle);
            _SubtitleTrack.ContextMenu = _SubtitleTrackContextMenu;

            _SubtitleEventContextMenu = new Skill.Editor.UI.ContextMenu();
            Skill.Editor.UI.MenuItem deleteItem = new Skill.Editor.UI.MenuItem("Delete");
            _SubtitleEventContextMenu.Add(deleteItem);

            Subtitle = null;

            addSubtitle.Click += AddSubtitle_Click;
            deleteItem.Click += DeleteSubtitle_Click;
            _BtnPlay.Click += _BtnPlay_Click;
            _TimeLine.TimeBar.MouseDown += Timebar_MouseDown;
        }

        void Timebar_MouseDown(object sender, MouseClickEventArgs args)
        {
            Stop();
        }

        void DeleteSubtitle_Click(object sender, System.EventArgs e)
        {
            SubtitleEvent se = (SubtitleEvent)_SubtitleEventContextMenu.Owner;
            if (_Events.Remove(se))
            {
                _SubtitleTrack.Controls.Remove(se);
                RebuildSubtitles();
            }
        }

        void AddSubtitle_Click(object sender, System.EventArgs e)
        {
            float x = _SubtitleTrackContextMenu.Position.x;
            // convert to local position of TimeBar - because of zooming
            x -= _TimeLine.View.ScrollPosition.x;

            Subtitle newTitle = new Subtitle();
            newTitle.Time = (float)_TimeLine.TimeBar.GetTime(x);
            newTitle.FontColor = Color.white;
            newTitle.Duration = 0.1f;
            newTitle.FontStyle = FontStyle.Normal;
            newTitle.TitleKey = "New Subtitle";

            SubtitleEvent se = CreateEvent(newTitle);
            InspectorProperties.Select(se);
            RebuildSubtitles();
        }

        protected override void Render()
        {
            if (_TitleStyle == null)
                _TitleStyle = new GUIStyle(UnityEditor.EditorStyles.label);

            if (!_Refresh)
            {
                if (_Subtitle != null)
                    _Refresh = _AudioPreview.Clip != _Subtitle.Clip;
                else
                    _Refresh = _AudioPreview.Clip != null;
            }
            if (_Refresh) Refresh();
            _AudioPreview.SetTime((float)_TimeLine.StartVisible, (float)_TimeLine.EndVisible);
            base.Render();
        }

        private void Refresh()
        {
            Stop();
            if (_Subtitle != null)
                _AudioPreview.Clip = _Subtitle.Clip;
            else
                _AudioPreview.Clip = null;

            _TimeLine.StartVisible = 0;
            if (_AudioPreview.Clip != null)
            {
                _TimeLine.MaxTime = _AudioPreview.Clip.length;
                _TimeLine.EndVisible = _AudioPreview.Clip.length;
            }
            else
            {
                _TimeLine.MaxTime = 1.0f;
                _TimeLine.EndVisible = 1.0f;
            }

            if (_Subtitle != null && _Subtitle.Titles != null)
            {
                // search for new subtitle in AudioClipSubtitle that we didn't create SubtitleEvent for them 
                foreach (var t in _Subtitle.Titles)
                {
                    if (t != null)
                    {
                        if (!IsEventExistWithTitle(t))
                            CreateEvent(t);
                    }
                }

                // search for removed subtitle in AudioClipSubtitle that we did create SubtitleEvent for them 
                int index = 0;
                while (index < _Events.Count)
                {
                    var e = _Events[index];
                    if (!IsTitleExistInSubtitle(e.Subtitle))
                    {
                        _Events.Remove(e);
                        _SubtitleTrack.Controls.Remove(e);
                        continue;
                    }
                    index++;
                }
            }
            else
            {
                _Events.Clear();
                _SubtitleTrack.Controls.Clear();
            }

            _TimeLine.View.FrameAll();
            _Refresh = false;
        }

        public void RefreshStyles()
        {
            _ToolbarBg.Style = Skill.Editor.Resources.Styles.Toolbar;

            _BtnPlay.NormalTexture = UnityEditor.EditorGUIUtility.FindTexture("d_preAudioPlayOff");
            _BtnPlay.OnTexture = UnityEditor.EditorGUIUtility.FindTexture("d_preAudioPlayOn");
            _BtnPlay.SetStyle(Skill.Editor.Resources.Styles.ToolbarButton);
        }

        private SubtitleEvent CreateEvent(Subtitle title)
        {
            SubtitleEvent se = new SubtitleEvent(this, _SubtitleTrack, title, _TitleStyle);
            se.ContextMenu = _SubtitleEventContextMenu;
            _Events.Add(se);
            _SubtitleTrack.Controls.Add(se);
            return se;
        }
        private bool IsEventExistWithTitle(Subtitle t)
        {
            foreach (var e in _Events)
                if (e.Subtitle == t) return true;
            return false;
        }
        private bool IsTitleExistInSubtitle(Subtitle t)
        {
            foreach (var k in _Subtitle.Titles)
            {
                if (k == t)
                    return true;
            }
            return false;
        }
        private void RebuildSubtitles()
        {
            if (_Subtitle != null)
            {
                _Subtitle.Titles = new Subtitle[_Events.Count];
                for (int i = 0; i < _Events.Count; i++)
                    _Subtitle.Titles[i] = _Events[i].Subtitle;
            }
        }


        #region Playback
        void _BtnPlay_Click(object sender, System.EventArgs e)
        {
            if (!_IsPlaying)
                Play();
            else
                Stop();
        }
        private void Play()
        {
            if (!_IsPlaying)
            {
                _PlayStartTime = _TimeLine.TimePosition;
                _BtnPlay.IsPressed = true;
                _BtnPlay.IsPlayMode = true;
                _IsPlaying = true;

                if (Camera.main != null) _Audio.transform.position = Camera.main.transform.position;
                _Audio.clip = _Subtitle.Clip;
                _Audio.time = (float)_PlayStartTime;
                _Audio.Play();
            }
        }
        public void Stop()
        {
            if (_IsPlaying)
            {
                _TimeLine.TimePosition = _PlayStartTime;
                _BtnPlay.IsPressed = false;
                _BtnPlay.IsPlayMode = false;
                _IsPlaying = false;

                if (_Audio.isPlaying)
                    _Audio.Stop();
            }
        }
        public void UpdatePlayback()
        {
            if (_IsPlaying)
            {
                if (!_Audio.isPlaying)
                    Stop();
                else
                    _TimeLine.TimePosition = _Audio.time;
            }
        }
        #endregion

        #region Destroy
        public void Destroy()
        {
            if (_Audio != null)
            {
                GameObject.DestroyImmediate(_Audio.gameObject);
                _Audio = null;
            }
        }
        #endregion
    }
}
