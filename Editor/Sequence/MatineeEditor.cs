using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Editor.UI;
using Skill.Editor.UI.Extended;
using Skill.Framework.UI;
using System.Collections.Generic;
using Skill.Framework.Sequence;
using Skill.Editor.Curve;

namespace Skill.Editor.Sequence
{
    // editor of matinee
    public class MatineeEditorWindow : UnityEditor.EditorWindow
    {
        #region Basic
        private static MatineeEditorWindow _Instance;
        public static MatineeEditorWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = EditorWindow.CreateInstance<MatineeEditorWindow>();
                return _Instance;
            }
        }

        public MatineeEditorWindow()
        {
            base.wantsMouseMove = true;
            base.title = "Matinee Editor";
            base.position = new Rect((Screen.width - 1280) / 2.0f, (Screen.height - 720) / 2.0f, 1280, 720);
            base.minSize = new Vector2(1280 * 0.3f, 720 * 0.3f);
        }

        void OnDestroy()
        {
            if (_IsPlaying) Stop();
            SaveTimeLine();
            Rollback();
            _CurveTrackTreeView.RemoveAll(false);
            _Frame = null;
        }

        // rollback all changes do to preview of editor
        void Rollback()
        {
            if (_Matinee != null)
            {
                Track[] tracks = _Matinee.GetComponentsInChildren<Track>();
                if (tracks != null)
                {
                    foreach (var t in tracks)
                    {
                        t.Rollback();
                    }
                }
            }
        }

        void OnGUI()
        {
            if (_Frame != null)
            {
                if (_RefreshStyles)
                {
                    RefreshStyles();
                    _RefreshStyles = false;
                }

                _Frame.OnGUI();
            }
        }



        void Update()
        {
            if (_Frame != null)
            {
                _Frame.Update();
                UpdatePlayback();
            }

        }

        private IEnumerable<Track> Tracks()
        {
            foreach (var c in _TimeLine.View.Controls)
            {
                if (c is BaseTrackBar)
                {
                    BaseTrackBar tb = (BaseTrackBar)c;
                    yield return tb.Track;
                }
            }
        }

        private IEnumerable<BaseTrackBar> TrackBars()
        {
            foreach (var c in _TimeLine.View.Controls)
            {
                if (c is BaseTrackBar)
                {
                    yield return (BaseTrackBar)c;
                }
            }
        }


        void OnFocus()
        {
            if (_Frame != null)
                Refresh();
        }

        void OnLostFocus()
        {
            SaveTimeLine();
        }

        void OnDisable()
        {
            if (_IsPlaying) Stop();
            SaveTimeLine();
            _Frame = null;
            if (Undo.undoRedoPerformed == Refresh)
                Undo.undoRedoPerformed = null;

            if (_Instance == this)
            {
                _Instance = null;
            }
            else if (!this.Equals(_Instance))
            {
                throw new System.ApplicationException("_Instance does not equal this");
            }
        }

        void OnEnable()
        {
            Undo.undoRedoPerformed = Refresh;
            _Instance = this;
            _RefreshStyles = true;
            this.CreateUI();
        }

        private void ChangePlayMode()
        {
            _Frame = null;
        }

        #endregion

        #region Variables
        [SerializeField]
        private Matinee _Matinee;
        private EditorFrame _Frame;
        private TimeLine _TimeLine;
        private Curve.CurveEditor _CurveEditor;
        private TrackTreeView _TracksTreeView;
        private PropertyGrid _PropertyGrid;
        private CurveTrackTreeView _CurveTrackTreeView;
        private bool _RefreshStyles;
        #endregion

        #region Properties
        internal PropertyGrid PropertyGrid { get { return _PropertyGrid; } }
        internal TimeLine TimeLine { get { return _TimeLine; } }
        internal CurveTrackTreeView CurveTracks { get { return _CurveTrackTreeView; } }

        public Matinee Matinee
        {
            get { return _Matinee; }
            set
            {
                if (_Matinee != value)
                {
                    _Matinee = value;
                    Refresh();
                }
            }
        }
        #endregion

        private void CreateUI()
        {
            _RefreshStyles = true;
            _Frame = new EditorFrame("Frame", this) { Location = Skill.Framework.UI.FrameLocation.Fill };

            Skill.Framework.UI.DockPanel dockPanel = new Skill.Framework.UI.DockPanel() { LastChildFill = true };
            _Frame.Controls.Add(dockPanel);

            CreateMenuBar();
            CreateToolbar();

            dockPanel.Controls.Add(_MenuBar); // add menu at top of window
            dockPanel.Controls.Add(_Toolbar); // add toolbar at top of window under menu


            // grid for CurveEditor and Timeline
            Skill.Framework.UI.Grid mainGrid = new Skill.Framework.UI.Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(300, Skill.Framework.UI.GridUnitType.Pixel), MinHeight = 100 }); // _CurveEditor
            mainGrid.RowDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Pixel); // splitter
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, Skill.Framework.UI.GridUnitType.Star), MinHeight = 30 }); // _TimeLine

            mainGrid.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // _CurveTreeView, _TracksTreeView and _PropertyGrid
            mainGrid.ColumnDefinitions.Add(4, Skill.Framework.UI.GridUnitType.Pixel); // splitter
            mainGrid.ColumnDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Star); // _TimeLine  and _CurveEditor

            _CurveEditor = new Curve.CurveEditor() { Row = 0, Column = 2 };
            _CurveEditor.TimeLine.TimeBar.ShowTimePosition = true;
            _CurveEditor.TimeLine.View.ShowTimePosition = true;
            mainGrid.Controls.Add(_CurveEditor);


            _CurveTrackTreeView = new CurveTrackTreeView(_CurveEditor) { Row = 0, Column = 0, Margin = new Thickness(0, 2, 0, 0) };
            mainGrid.Controls.Add(_CurveTrackTreeView);

            Skill.Editor.UI.GridSplitter hSplitter = new Skill.Editor.UI.GridSplitter() { Orientation = Orientation.Horizontal, Row = 1, Column = 0, ColumnSpan = 3, Style = Skill.Editor.Resources.Styles.HorizontalSplitter };
            mainGrid.Controls.Add(hSplitter);

            CreateTimeLine();
            _TimeLine.Row = 2;
            _TimeLine.Column = 2;


            #region TracksTreeView and PropertyGrid

            // grid to split TracksTreeView and PropertyGrid
            Grid grid = new Grid() { Row = 2, Column = 0 };
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, Skill.Framework.UI.GridUnitType.Star), MinHeight = 80 });
            grid.RowDefinitions.Add(2, GridUnitType.Pixel);
            grid.RowDefinitions.Add(1, GridUnitType.Star);

            // **************************** _TracksTreeView *****************************

            _TracksTreeView = new TrackTreeView(this) { Row = 0 };
            grid.Controls.Add(_TracksTreeView);

            Skill.Editor.UI.GridSplitter splitter = new Skill.Editor.UI.GridSplitter() { Orientation = Orientation.Horizontal, Row = 1, Column = 0, Style = Skill.Editor.Resources.Styles.HorizontalSplitter, OverFlow = 3 };
            grid.Controls.Add(splitter);

            _PropertyGrid = new PropertyGrid() { Row = 2 };
            grid.Controls.Add(_PropertyGrid);

            mainGrid.Controls.Add(grid);
            #endregion

            // add splitter between (_TimeLine and _CurveEditor) and (_CurveTreeView, _TracksTreeView and _PropertyGrid)
            Skill.Editor.UI.GridSplitter vSplitter = new Skill.Editor.UI.GridSplitter() { Orientation = Orientation.Vertical, Row = 0, Column = 1, RowSpan = 3, Style = Skill.Editor.Resources.Styles.VerticalSplitter };
            mainGrid.Controls.Add(vSplitter);

            mainGrid.Controls.Add(_TimeLine);

            // fill remaining area with mainGrid
            dockPanel.Controls.Add(mainGrid);

            _CurveEditor.PropertyGrid = _PropertyGrid;
            _CurveEditor.TimeLine.TimePositionChanged += CurveEditorTimeLine_PositionChanged;

            _CurveEditor.TimeLine.TimeBar.SnapTime = 0.001;
            _TimeLine.TimeBar.SnapTime = 0.001;
        }

        #region Edit Curve

        private int _IgnorePosition;
        internal void EditCurve(BaseTrackBar trackBar)
        {
            _CurveTrackTreeView.Add(trackBar, trackBar.FirstKey);
        }

        private void _TimeLine_TimePositionChanged(object sender, System.EventArgs e)
        {
            if (_IgnorePosition == 1) return;
            if (!_IsPlaying)
                _SeekUpdate = true;

            _IgnorePosition = 1;
            _CurveEditor.TimeLine.TimePosition = System.Math.Max(0, _TimeLine.TimePosition);
            _IgnorePosition = 0;

        }

        void CurveEditorTimeLine_PositionChanged(object sender, System.EventArgs e)
        {
            if (_IgnorePosition == 2) return;
            _IgnorePosition = 2;
            _TimeLine.TimePosition = System.Math.Max(0, _CurveEditor.TimeLine.TimePosition);
            _IgnorePosition = 0;
        }
        #endregion

        #region TimeLine
        private void CreateTimeLine()
        {
            _TimeLine = new TimeLine(new TrackBarView());
            _TimeLine.TimePositionChanged += _TimeLine_TimePositionChanged;
        }
        #endregion

        #region Toolbar

        private Grid _Toolbar;
        private MediaButton _BtnPlay;
        private MediaButton _BtnPause;
        private MediaButton _BtnStepForward;
        private MediaButton _BtnStepBackward;
        private MediaButton _BtnLoop;
        private MediaButton[] _PlayButtons;

        private Skill.Editor.UI.IntPopup _SnapTime;
        private Skill.Editor.UI.IntPopup _Fps;
        private Skill.Editor.UI.IntPopup _PlaybackSpeed;


        private void CreateToolbar()
        {
            Box bg = new Box();
            _Toolbar = new Grid() { Dock = Skill.Framework.UI.Dock.Top, Height = 24 };
            _Toolbar.Controls.Add(bg);

            StackPanel toolbarPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            _Toolbar.Controls.Add(toolbarPanel);

            #region Playback buttons
            Grid playButtonsPanel = new Grid() { Width = 150, Margin = new Thickness(1, 2, 1, 3) };
            _PlayButtons = new MediaButton[5];
            for (int i = 0; i < _PlayButtons.Length; i++)
                playButtonsPanel.ColumnDefinitions.Add(1, GridUnitType.Star);

            _BtnStepBackward = new MediaButton(Skill.Editor.Resources.Textures.StepBackward, Skill.Editor.Resources.Textures.StepBackwardOn) { Column = 0, TogglePressed = false, IsEnabled = false }; _BtnStepBackward.Content.tooltip = "Step backward";
            _BtnPlay = new MediaButton(Skill.Editor.Resources.Textures.Play, Skill.Editor.Resources.Textures.PlayOn) { Column = 1, TogglePressed = true }; _BtnPlay.Content.tooltip = "Start preview playback from current position";
            _BtnPause = new MediaButton(Skill.Editor.Resources.Textures.Pause, Skill.Editor.Resources.Textures.PauseOn) { Column = 2, TogglePressed = true }; _BtnPause.Content.tooltip = "Pause preview playback ";
            _BtnStepForward = new MediaButton(Skill.Editor.Resources.Textures.StepForward, Skill.Editor.Resources.Textures.StepForwardOn) { Column = 3, TogglePressed = false, IsEnabled = false }; _BtnStepForward.Content.tooltip = "Step forward";
            _BtnLoop = new MediaButton(Skill.Editor.Resources.Textures.Loop, Skill.Editor.Resources.Textures.LoopOn) { Column = 4, TogglePressed = true }; _BtnLoop.Content.tooltip = "Loop preview playback of loop section";



            _PlayButtons[0] = _BtnStepBackward;
            _PlayButtons[1] = _BtnPlay;
            _PlayButtons[2] = _BtnPause;
            _PlayButtons[3] = _BtnStepForward;
            _PlayButtons[4] = _BtnLoop;

            for (int i = 0; i < _PlayButtons.Length; i++)
                playButtonsPanel.Controls.Add(_PlayButtons[i]);

            toolbarPanel.Controls.Add(playButtonsPanel);

            _BtnPlay.Click += _BtnPlay_Click;
            _BtnPause.Click += _BtnPause_Click;
            _BtnLoop.Click += _BtnLoop_Click;
            _BtnStepBackward.Click += _BtnStepBackward_Click;
            _BtnStepForward.Click += _BtnStepForward_Click;
            #endregion

            #region SnapTime
            _SnapTime = new IntPopup() { Margin = new Thickness(1, 4), Width = 70 };
            _SnapTime.Label.tooltip = "Snap time";
            _SnapTime.Options.Add(new PopupOption(00, "None") { UserData = 0.0 });
            _SnapTime.Options.Add(new PopupOption(01, "1.0") { UserData = 1.0 });
            _SnapTime.Options.Add(new PopupOption(02, "0.50") { UserData = 0.5 });
            _SnapTime.Options.Add(new PopupOption(03, "0.25") { UserData = 0.25 });
            _SnapTime.Options.Add(new PopupOption(04, "0.20") { UserData = 0.2 });
            _SnapTime.Options.Add(new PopupOption(05, "0.10") { UserData = 0.1 });
            _SnapTime.Options.Add(new PopupOption(06, "0.050") { UserData = 0.05 });
            _SnapTime.Options.Add(new PopupOption(07, "0.025") { UserData = 0.025 });
            _SnapTime.Options.Add(new PopupOption(08, "0.020") { UserData = 0.02 });
            _SnapTime.Options.Add(new PopupOption(09, "0.010") { UserData = 0.01 });
            _SnapTime.Options.Add(new PopupOption(10, "0.0050") { UserData = 0.005 });
            _SnapTime.Options.Add(new PopupOption(11, "0.0025") { UserData = 0.0025 });
            _SnapTime.Options.Add(new PopupOption(12, "0.0020") { UserData = 0.002 });
            _SnapTime.Options.Add(new PopupOption(13, "0.0010") { UserData = 0.001 });
            _SnapTime.Options.Add(new PopupOption(14, "0.00050") { UserData = 0.0005 });
            _SnapTime.Options.Add(new PopupOption(15, "0.00025") { UserData = 0.00025 });
            _SnapTime.Options.Add(new PopupOption(16, "0.00020") { UserData = 0.0002 });
            _SnapTime.Options.Add(new PopupOption(17, "0.00010") { UserData = 0.0001 });

            _SnapTime.SelectedValue = 0;
            toolbarPanel.Controls.Add(_SnapTime);
            _SnapTime.OptionChanged += _SnapTime_OptionChanged;
            #endregion

            #region FPS
            _Fps = new IntPopup() { Margin = new Thickness(1, 4), Width = 60 };
            _Fps.Label.tooltip = "Sets the timeline granularity for visualiization purposes.";
            _Fps.Options.Add(new PopupOption(30, "30 fps"));
            _Fps.Options.Add(new PopupOption(24, "24 fps"));
            _Fps.Options.Add(new PopupOption(20, "20 fps"));
            _Fps.Options.Add(new PopupOption(60, "60 fps"));

            _Fps.SelectedValue = 30;
            toolbarPanel.Controls.Add(_Fps);
            #endregion

            #region PlaybackSpeed

            _PlaybackSpeed = new IntPopup() { Margin = new Thickness(1, 4), Width = 60 };
            _PlaybackSpeed.Options.Add(new PopupOption(100, "100%"));
            _PlaybackSpeed.Options.Add(new PopupOption(50, "50%"));
            _PlaybackSpeed.Options.Add(new PopupOption(25, "25%"));
            _PlaybackSpeed.Options.Add(new PopupOption(10, "10%"));
            _PlaybackSpeed.Options.Add(new PopupOption(1, "1%"));

            _PlaybackSpeed.SelectedValue = 100;
            toolbarPanel.Controls.Add(_PlaybackSpeed);
            #endregion

        }

        void _BtnStepForward_Click(object sender, System.EventArgs e)
        {
            if (_IsPlaying && _IsPause)
                _StepForward = 1;
        }

        void _BtnStepBackward_Click(object sender, System.EventArgs e)
        {
            if (_IsPlaying && _IsPause)
                _StepForward = -1;
        }

        void _BtnLoop_Click(object sender, System.EventArgs e)
        {
            _IsLoop = _BtnLoop.IsPressed;
        }


        void _BtnPause_Click(object sender, System.EventArgs e)
        {
            if (!_IsPause)
                Pause();
            else
                Play();
        }

        void _BtnPlay_Click(object sender, System.EventArgs e)
        {
            if (!_IsPlaying)
                Play();
            else
                Stop();
        }

        void _SnapTime_OptionChanged(object sender, System.EventArgs e)
        {
            SetSnap((double)_SnapTime.SelectedOption.UserData);
        }



        #endregion

        #region MenuBar

        private MenuBar _MenuBar;

        private void CreateMenuBar()
        {
            _MenuBar = new Skill.Editor.UI.Extended.MenuBar() { Dock = Skill.Framework.UI.Dock.Top };

            MenuBarItem file = new MenuBarItem() { Title = "File" };
            MenuBarItem edit = new MenuBarItem() { Title = "Edit" };
            MenuBarItem view = new MenuBarItem() { Title = "View" };
            MenuBarItem window = new MenuBarItem() { Title = "Window", Width = 54 };

            _MenuBar.Controls.Add(file);
            _MenuBar.Controls.Add(edit);
            _MenuBar.Controls.Add(view);
            _MenuBar.Controls.Add(window);


            //Skill.Editor.UI.MenuItem file_1 = new Skill.Editor.UI.MenuItem("File_1 dsasd dsasd sasdsa"); file_1.Click += item_Click;
            //Skill.Editor.UI.MenuItem file_2 = new Skill.Editor.UI.MenuItem("file_2"); file_2.Click += item_Click;
            //Skill.Editor.UI.MenuItem file_3 = new Skill.Editor.UI.MenuItem("file_3"); file_3.Click += item_Click;
            //file.Add(file_1);
            //file.Add(file_2);
            //file.AddSeparator();
            //file.Add(file_3);


            //Skill.Editor.UI.MenuItem edit_1 = new Skill.Editor.UI.MenuItem("edit_1"); edit_1.Click += item_Click;
            //Skill.Editor.UI.MenuItem edit_2 = new Skill.Editor.UI.MenuItem("edit_2"); edit_2.Click += item_Click;
            //edit.Add(edit_1);
            //edit.Add(edit_2);            
        }
        #endregion

        #region Load / Save

        // refresh styles when state changed between editor mode and play mode
        private void RefreshStyles()
        {
            _BtnStepBackward.SetStyle(Skill.Editor.Resources.Styles.LargeLeftButton);
            _BtnPlay.SetStyle(Skill.Editor.Resources.Styles.LargeMiddleButton);
            _BtnPause.SetStyle(Skill.Editor.Resources.Styles.LargeMiddleButton);
            _BtnStepForward.SetStyle(Skill.Editor.Resources.Styles.LargeMiddleButton);
            _BtnLoop.SetStyle(Skill.Editor.Resources.Styles.RightButton);
        }

        // refresh editor data to to changes happened to matinee outside of MatineeEditor
        private void Refresh()
        {
            if (Selection.activeObject != null)
            {
                GameObject selectedObject = null;
                if (Selection.activeObject is GameObject)
                    selectedObject = (GameObject)Selection.activeObject;
                else if (Selection.activeObject is Component)
                    selectedObject = ((Component)Selection.activeObject).gameObject;

                if (selectedObject != null)
                {
                    Matinee newMatinee = selectedObject.GetComponent<Matinee>();
                    if (newMatinee != null)
                    {
                        PrefabType pt = PrefabUtility.GetPrefabType(newMatinee.gameObject);
                        if (pt == PrefabType.None || pt == PrefabType.PrefabInstance || pt == PrefabType.MissingPrefabInstance || pt == PrefabType.DisconnectedPrefabInstance)
                            _Matinee = newMatinee;
                        else
                            _Matinee = null;
                    }
                }
            }
            if (_Frame != null)
            {
                if (_Matinee == null)
                {
                    _Frame.Grid.IsEnabled = false;
                    _TracksTreeView.Clear();
                    _CurveTrackTreeView.RemoveAll(true);
                    _TimeLine.Clear();
                    _TimeLine.MaxTime = 1;
                    _PropertyGrid.SelectedObject = null;
                }
                else
                {
                    LoadTimeLine();
                    _TracksTreeView.Refresh();
                    foreach (BaseTrackBar bt in TrackBars())
                    {
                        if (bt.IsEditingCurves)
                            EditCurve(bt);
                    }
                    _Frame.Grid.IsEnabled = true;
                }
            }
        }

        // load timeline state from matinee
        private void LoadTimeLine()
        {
            if (_Matinee != null)
            {
                _TimeLine.MaxTime = _Matinee.MaxTime;
                _TimeLine.StartVisible = Mathf.Max(0, Mathf.Min(_Matinee.StartVisible, _Matinee.Length - 1));
                _TimeLine.EndVisible = _Matinee.EndVisible;
                _TimeLine.TimePosition = _Matinee.TimePosition;
                _TimeLine.SelectTime(_Matinee.StartSelection, _Matinee.EndSelection);
            }
        }

        // save timeline state to matinee
        private void SaveTimeLine()
        {
            if (_Matinee != null)
            {
                _Matinee.MaxTime = (float)_TimeLine.MaxTime;
                _Matinee.StartVisible = (float)_TimeLine.StartVisible;
                _Matinee.EndVisible = (float)_TimeLine.EndVisible;
                _Matinee.StartSelection = (float)_TimeLine.StartSelection;
                _Matinee.EndSelection = (float)_TimeLine.EndSelection;
                _Matinee.TimePosition = (float)_TimeLine.TimePosition;
                EditorUtility.SetDirty(_Matinee);
            }
        }
        #endregion

        #region Playback

        private float _DeltaTime;// delta time since last playback update
        private float _PlayTime; // playback time
        private bool _IsPause; // is playback paused
        private bool _IsPlaying; // is preview playback
        private bool _IsLoop;// is loop playback looped

        // we can't do snapping when preview playback is active
        // so we save Snap Time right before starting playback
        // then rollback snap time when playback stopped
        private double _SnapTimeBeforePlay; // snap time before enter preview playback

        // position of timeline before enter playback preview
        private double _TimePositionBeforePlay;
        private bool _SeekUpdate; // seek preview to TimeLine position in next update
        private int _StepForward; // step forward(1) or backward(-1) in next update when playback is active and paused    


        // step playback in positive stepTime
        private void StepPlaybackForward(float stepTime)
        {
            _DeltaTime += stepTime;
            float fpsTime = 1.0f / _Fps.SelectedValue;
            if (_DeltaTime >= fpsTime) // if we reach fpsTime after last step
            {
                _DeltaTime -= fpsTime;
                _PlayTime += fpsTime * (float)_PlaybackSpeed.SelectedValue / 100.0f;

                if (_TimeLine.SelectionLenght > 0)
                {
                    if (_PlayTime >= _TimeLine.EndSelection)
                    {
                        if (_IsLoop)
                        {
                            _PlayTime -= (float)_TimeLine.SelectionLenght;
                            foreach (var t in Tracks()) t.Seek(_PlayTime);
                        }
                        else if (_IsPause)
                            _PlayTime = (float)_TimeLine.EndSelection;
                        else
                            Stop();
                    }
                }
                else if (_PlayTime >= _TimeLine.MaxTime)
                {
                    if (_IsLoop)
                    {
                        _PlayTime -= (float)(_TimeLine.MaxTime - _TimeLine.MinTime);
                        foreach (var t in Tracks()) t.Seek(_PlayTime);
                    }
                    else if (_IsPause)
                        _PlayTime = (float)_TimeLine.MaxTime;
                    else
                        Stop();
                }
            }
        }

        // step playback in negative stepTime
        private void StepPlaybackBackward(float stepTime)
        {
            _DeltaTime -= stepTime;
            if (_DeltaTime < 0) // if we reach fpsTime after last step
            {
                float fpsTime = 1.0f / _Fps.SelectedValue;
                _DeltaTime += fpsTime;
                _PlayTime -= fpsTime * (float)_PlaybackSpeed.SelectedValue / 100.0f;

                if (_TimeLine.SelectionLenght > 0)
                {
                    if (_PlayTime < _TimeLine.StartSelection)
                    {
                        if (_IsLoop)
                        {
                            _PlayTime += (float)_TimeLine.SelectionLenght;
                            foreach (var t in Tracks()) t.Seek(_PlayTime);
                        }
                        else if (_IsPause)
                            _PlayTime = (float)_TimeLine.StartSelection;
                        else
                            Stop();
                    }
                }
                else if (_PlayTime <= _TimeLine.MinTime)
                {
                    if (_IsLoop)
                    {
                        _PlayTime += (float)(_TimeLine.MaxTime - _TimeLine.MinTime);
                        foreach (var t in Tracks()) t.Seek(_PlayTime);
                    }
                    else if (_IsPause)
                        _PlayTime = (float)_TimeLine.MinTime;
                    else
                        Stop();
                }
            }
        }

        private void UpdatePlayback()
        {
            if (_Matinee != null)
            {
                if (_IsPlaying)
                {
                    if (!_IsPause)
                        StepPlaybackForward(1.0f / 100.0f); // update called 100 time per second
                    else if (_StepForward == 1)
                        StepPlaybackForward(1.0f / _Fps.SelectedValue); // step forward with fpsTime
                    else if (_StepForward == -1)
                        StepPlaybackBackward(1.0f / _Fps.SelectedValue); // step backward with fpsTime
                    _StepForward = 0;
                    Evaluate();
                }
                else
                {
                    _PlayTime = (float)_TimeLine.TimePosition;
                    if (_SeekUpdate) // user changed timeline position and we must force seek to that position
                    {
                        foreach (var tb in TrackBars())
                        {
                            tb.Track.SortKeys();
                            tb.Seek(_PlayTime); // and seek to TimeLine position
                        }
                        _SeekUpdate = false;
                        Evaluate();
                    }
                }
            }
        }

        private void Evaluate()
        {
            _TimeLine.TimePosition = _PlayTime;
            foreach (var t in TrackBars())
                t.Evaluate(_PlayTime);
            Repaint();
        }

        // initilize variables to start playback
        private void StartPlayback()
        {
            _TimePositionBeforePlay = _TimeLine.TimePosition; // save position of timeline
            _SnapTimeBeforePlay = _TimeLine.TimeBar.SnapTime; // save snaptime        

            if (_TimeLine.SelectionLenght > 0)
                _PlayTime = (float)_TimeLine.StartSelection; // we must start at start selected area
            else
                _PlayTime = (float)_TimeLine.TimePosition; // we must start at position of timeline selected by user
            _DeltaTime = 0;
            foreach (var t in Tracks())
            {
                t.SortKeys();
                t.Seek(_PlayTime);
            }
            Repaint();// repaint to force update scene

        }

        // play or resume playback
        private void Play()
        {
            if (_Matinee == null) return;
            if (!_IsPlaying)
            {
                StartPlayback();
            }
            else
            {
                _IsPause = false;
                _BtnPause.IsPressed = false;
            }
            _IsPlaying = true;
            _SeekUpdate = false;
            UpdatePlayButtonsState(true);// update state of playback buttons
        }
        // stop playback and restore data
        private void Stop()
        {
            if (_Matinee == null) return;
            _IsPlaying = false;
            _IsPause = false;
            UpdatePlayButtonsState(false); // update state of playback buttons
            _BtnPlay.IsPressed = false;
            _BtnPause.IsPressed = false;

            _TimeLine.TimeBar.SnapTime = _SnapTimeBeforePlay;// rollback snaptime
            _TimeLine.TimePosition = _TimePositionBeforePlay; // rollback time position

            foreach (var t in Tracks()) t.Stop();
        }

        // pause playback
        private void Pause()
        {
            if (_Matinee == null) return;
            _IsPause = true;
            SetStepEnable();// enable step buttons
            _StepForward = 0;
        }

        // update state of playback buttons based on is playmode or not
        private void UpdatePlayButtonsState(bool playmode)
        {
            for (int i = 0; i < _PlayButtons.Length; i++)
                _PlayButtons[i].IsPlayMode = playmode;
            SetStepEnable();
        }

        // enable or disable step buttons based on is playback on or not and is paused or not
        private void SetStepEnable()
        {
            bool enable = _IsPlaying && _IsPause;
            _BtnStepBackward.IsEnabled = enable;
            _BtnStepForward.IsEnabled = enable;
        }

        // set snap time of timeline
        private void SetSnap(double snapTime)
        {
            if (_BtnPlay.IsPlayMode)
                _SnapTimeBeforePlay = snapTime;
            else
                _TimeLine.TimeBar.SnapTime = snapTime;
        }
        #endregion



    }



    [CustomEditor(typeof(Matinee))]

    public class MatineeEditor : UnityEditor.Editor
    {
        private Matinee _Matinee;
        void OnEnable()
        {
            _Matinee = (Matinee)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Edit"))
            {
                MatineeEditorWindow.Instance.Show();
                MatineeEditorWindow.Instance.Matinee = _Matinee;
            }
            EditorGUILayout.EndVertical();
        }
    }
}