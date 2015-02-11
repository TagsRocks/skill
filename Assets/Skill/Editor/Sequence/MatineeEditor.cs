using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Editor.UI;
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

        // rollback all changes do to preview of editor
        void Rollback()
        {
            if (Matinee != null)
                Matinee.Rollback();
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

                if (_InvalidateTimeLineViewCounter-- > 0)
                    TimeLine.View.Invalidate();

                HandleEvents();

                _Frame.OnGUI();
            }
        }

        private void HandleEvents()
        {
            Event e = Event.current;
            if (e != null && e.type != EventType.Used)
            {
                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.Delete)
                    {
                        IProperties p = InspectorProperties.GetSelected();
                        if (p != null)
                        {
                            if (p is KeyView)
                            {
                                ((BaseTrackBar)(((KeyView)p).TrackBar)).Delete((KeyView)p);
                                e.Use();
                            }
                            else if (p is TrackTreeViewItem)
                            {
                                e.Use();
                                _TracksTreeView.DeleteTrack((TrackTreeViewItem)p);
                            }
                            else if (p is TrackTreeViewGroup)
                            {
                                e.Use();
                                _TracksTreeView.DeleteGroup((TrackTreeViewGroup)p);
                            }
                        }
                    }
                    else if (e.keyCode == KeyCode.S)
                    {
                        _TracksTreeView.AddKey();
                        e.Use();
                    }
                }
            }
        }

        private int _InvalidateTimeLineViewCounter;
        internal void InvalidateTimeLineView()
        {
            _InvalidateTimeLineViewCounter = 2;
        }


        private int _AutoKeyCounter;
        void Update()
        {
            if (_Frame != null)
            {
                _Frame.Update();
                UpdatePlayback();

                if (IsAutoKey)
                {
                    if (EditorWindow.focusedWindow != this)
                    {
                        _AutoKeyCounter++;
                        if (_AutoKeyCounter > 10)
                        {
                            _AutoKeyCounter = 0;
                            foreach (var tb in TrackBars())
                                tb.AutoKey();
                        }
                    }
                }
            }

        }

        private IEnumerable<Track> Tracks()
        {
            foreach (var c in _TimeLine.View.Controls)
            {
                if (c is BaseTrackBar)
                {
                    BaseTrackBar tb = (BaseTrackBar)c;
                    if (!tb.Track.IsDestroyed)
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
                    BaseTrackBar tb = (BaseTrackBar)c;
                    if (!tb.Track.IsDestroyed)
                        yield return tb;
                }
            }
        }


        void OnFocus()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Undo.undoRedoPerformed += Refresh;
                FindMatinee();
                FindSceneMatinees();
                Refresh();
            }
        }

        void OnLostFocus()
        {
            Undo.undoRedoPerformed -= Refresh;
            if (IsAutoKey)
            {
                foreach (var tb in TrackBars())
                    tb.SaveRecordState();
            }
            SaveEditorData();
        }

        void OnDisable()
        {
            if (_IsPlaying)
                Stop();
        }

        void OnEnable()
        {
            _Instance = this;
            _RefreshStyles = true;
            if (_Frame == null)
                this.CreateUI();
        }

        void OnDestroy()
        {
            if (_IsPlaying) Stop();
            Rollback();
            Clear();

            _Frame = null;
            if (_Instance == this)
                _Instance = null;
            else if (!this.Equals(_Instance))
                throw new System.ApplicationException("_Instance does not equal this");
        }

        #endregion

        #region Variables

        private EditorFrame _Frame;
        private TimeLine _TimeLine;
        private Curve.CurveEditor _CurveEditor;
        private TrackTreeView _TracksTreeView;
        private CurveTrackTreeView _CurveTrackTreeView;
        private Grid _MainGrid;
        private Skill.Editor.UI.GridSplitter _VSplitter;

        private bool _RefreshStyles;
        #endregion

        #region Properties
        internal TimeLine TimeLine { get { return _TimeLine; } }
        internal CurveTrackTreeView CurveTracks { get { return _CurveTrackTreeView; } }


        private bool _IsAutoKey;
        internal bool IsAutoKey
        {
            get { return _IsAutoKey; }
            set
            {
                _IsAutoKey = value;
                if (_BtnAutoKey != null)
                    _BtnAutoKey.Content.image = IsAutoKey ? Skill.Editor.Resources.UITextures.RecordOn : Skill.Editor.Resources.UITextures.RecordOff;
            }
        }


        [SerializeField]
        private Matinee _EdittingMatinee;

        [SerializeField]
        private int _EdittingMatineeId;

        private void FindMatinee()
        {
            if (_EdittingMatineeId != 0)
            {
                var obj = EditorUtility.InstanceIDToObject(_EdittingMatineeId);
                if (obj != null)
                {
                    if (obj is GameObject)
                        _EdittingMatinee = ((GameObject)obj).GetComponent<Matinee>();
                }
            }
        }

        public Matinee Matinee
        {
            get
            {
                return _EdittingMatinee;
            }
            set
            {
                if (_EdittingMatinee != value)
                {
                    Rollback();
                    Clear();
                    _EdittingMatinee = value;
                    if (_EdittingMatinee == null)
                        _EdittingMatineeId = 0;
                    else
                        _EdittingMatineeId = _EdittingMatinee.gameObject.GetInstanceID();
                    UnityEditor.EditorUtility.SetDirty(this);
                }
                Refresh();
            }
        }

        private void Clear()
        {
            _TracksTreeView.Clear();
            _CurveTrackTreeView.RemoveAll(true);
            InspectorProperties.Select(null);
            _TimeLine.Clear();
            _TimeLine.StartVisible = 0.0f;
            _TimeLine.EndVisible = 1.0f;
            _TimeLine.MinTime = 0.0f;
            _TimeLine.MaxTime = 1.0f;

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
            dockPanel.Controls.Add(_ToolbarPanel); // add toolbar at top of window under menu            

            // grid for CurveEditor and Timeline
            _MainGrid = new Skill.Framework.UI.Grid();
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star), MinHeight = 30 }); // _TimeLine

            _MainGrid.ColumnDefinitions.Add(1, GridUnitType.Star); // _CurveTreeView, _TracksGrid
            _MainGrid.ColumnDefinitions.Add(4, GridUnitType.Pixel); // splitter
            _MainGrid.ColumnDefinitions.Add(2, GridUnitType.Star); // _TimeLine  and _CurveEditor

            _CurveEditor = new Curve.CurveEditor() { Row = 0, Column = 2 };
            _CurveEditor.TimeLine.TimeBar.ShowTimePosition = true;
            _CurveEditor.TimeLine.View.ShowTimePosition = true;
            //_CurveEditor.TimeLine.MinTime = -0.1f;
            _MainGrid.Controls.Add(_CurveEditor);


            _CurveTrackTreeView = new CurveTrackTreeView(_CurveEditor) { Row = 0, Column = 0, Margin = new Thickness(0, 0, 0, 0) };
            _MainGrid.Controls.Add(_CurveTrackTreeView);

            _TimeLine = new TimeLine(new TrackBarView() { SideView = true }) { Row = 0, Column = 2 };
            _TimeLine.TimePositionChanged += _TimeLine_TimePositionChanged;

            _TracksTreeView = new TrackTreeView(this) { Row = 0, Column = 0, Margin = new Thickness(0, 0, 0, 0) };
            _MainGrid.Controls.Add(_TracksTreeView);

            // add splitter between (_TimeLine and _CurveEditor) and (_CurveTreeView, _TracksTreeView and _PropertyGrid)
            _VSplitter = new Skill.Editor.UI.GridSplitter() { Orientation = Orientation.Vertical, Row = 0, Column = 1, RowSpan = 3 };
            _MainGrid.Controls.Add(_VSplitter);

            _MainGrid.Controls.Add(_TimeLine);

            // fill remaining area with mainGrid
            dockPanel.Controls.Add(_MainGrid);

            _CurveEditor.TimeLine.TimePositionChanged += CurveEditorTimeLine_PositionChanged;

            _CurveEditor.TimeLine.TimeBar.SnapTime = 0.001;
            _TimeLine.TimeBar.SnapTime = 0.001;

            _TimeLine.View.VerticalScrollChanged += View_VerticalScrollChanged;
            _TracksTreeView.TreeView.ScrollPositionChanged += TreeView_ScrollPositionChanged;

            SetLayout(0);
        }


        #region Scroll handling

        private int _IgnoreScroll;
        void TreeView_ScrollPositionChanged(object sender, System.EventArgs e)
        {
            if (_IgnoreScroll == 1) return;
            _IgnoreScroll = 1;
            _TimeLine.View.VerticalScroll = _TracksTreeView.TreeView.ScrollPosition.y;
            _IgnoreScroll = 0;
        }
        void View_VerticalScrollChanged(object sender, System.EventArgs e)
        {
            if (_IgnoreScroll == 2) return;
            _IgnoreScroll = 2;
            Vector2 scrollPos = _TracksTreeView.TreeView.ScrollPosition;
            scrollPos.y = _TimeLine.View.VerticalScroll;
            _TracksTreeView.TreeView.ScrollPosition = scrollPos;
            _IgnoreScroll = 0;
        }

        #endregion

        #region Edit Curve

        private int _IgnorePosition;
        internal void EditCurve(BaseTrackBar trackBar)
        {
            _CurveTrackTreeView.Add(trackBar);
        }

        private void _TimeLine_TimePositionChanged(object sender, System.EventArgs e)
        {
            if (_IgnorePosition == 1) return;
            if (_IsPlaying) return;

            _SeekUpdate = true;
            _IgnorePosition = 1;
            _CurveEditor.TimeLine.TimePosition = System.Math.Max(0, _TimeLine.TimePosition);
            _IgnorePosition = 0;

        }

        void CurveEditorTimeLine_PositionChanged(object sender, System.EventArgs e)
        {
            if (_IgnorePosition == 2) return;
            if (_IsPlaying) return;
            _IgnorePosition = 2;
            _TimeLine.TimePosition = System.Math.Max(0, _CurveEditor.TimeLine.TimePosition);
            _IgnorePosition = 0;
        }
        #endregion

        #region Toolbar

        private Grid _ToolbarPanel;
        private Box _ToolbarBg;
        private Grid _PlaybackButtonsPanel;
        private Skill.Framework.UI.Button _BtnAutoKey;
        private MediaButton _BtnPlay;
        private MediaButton _BtnPause;
        private MediaButton _BtnStepForward;
        private MediaButton _BtnStepBackward;
        private MediaButton _BtnLoop;
        private MediaButton[] _PlayButtons;

        private Skill.Editor.UI.IntPopup _SnapTime;
        private Skill.Editor.UI.IntPopup _Fps;
        private Skill.Editor.UI.IntPopup _PlaybackSpeed;

        private Toolbar _LayoutButtonsPanel;
        private ToolbarButton _TBtnDopeSheet;
        private ToolbarButton _TBtnCurves;

        private void CreateToolbar()
        {
            _ToolbarBg = new Box() { Column = 0, ColumnSpan = 20 };
            _ToolbarPanel = new Grid() { Dock = Skill.Framework.UI.Dock.Top, Height = 20, Margin = new Thickness(0, 0, 0, 0) };
            _ToolbarPanel.ColumnDefinitions.Add(180, GridUnitType.Pixel); // _PlaybackButtonsPanel
            _ToolbarPanel.ColumnDefinitions.Add(70, GridUnitType.Pixel); // _SnapTime
            _ToolbarPanel.ColumnDefinitions.Add(60, GridUnitType.Pixel); // _Fps
            _ToolbarPanel.ColumnDefinitions.Add(60, GridUnitType.Pixel); // _PlaybackSpeed
            _ToolbarPanel.ColumnDefinitions.Add(1, GridUnitType.Star);
            _ToolbarPanel.ColumnDefinitions.Add(140, GridUnitType.Pixel); // _LayoutButtonsPanel
            _ToolbarPanel.Controls.Add(_ToolbarBg);

            #region Playback buttons
            _PlaybackButtonsPanel = new Grid() { Column = 0 };
            _PlayButtons = new MediaButton[5];
            for (int i = 0; i < 6; i++)
                _PlaybackButtonsPanel.ColumnDefinitions.Add(1, GridUnitType.Star);

            _BtnAutoKey = new Skill.Framework.UI.Button() { Column = 0 }; _BtnAutoKey.Content.tooltip = "Auto key";
            _BtnPlay = new MediaButton(Skill.Editor.Resources.UITextures.Play, Skill.Editor.Resources.UITextures.PlayOn) { Column = 1, TogglePressed = true }; _BtnPlay.Content.tooltip = "Start preview playback from current position";
            _BtnPause = new MediaButton(Skill.Editor.Resources.UITextures.Pause, Skill.Editor.Resources.UITextures.PauseOn) { Column = 2, TogglePressed = true }; _BtnPause.Content.tooltip = "Pause preview playback ";
            _BtnStepForward = new MediaButton(Skill.Editor.Resources.UITextures.StepForward, Skill.Editor.Resources.UITextures.StepForwardOn) { Column = 3, TogglePressed = false, IsEnabled = false }; _BtnStepForward.Content.tooltip = "Step forward";
            _BtnStepBackward = new MediaButton(Skill.Editor.Resources.UITextures.StepBackward, Skill.Editor.Resources.UITextures.StepBackwardOn) { Column = 4, TogglePressed = false, IsEnabled = false }; _BtnStepBackward.Content.tooltip = "Step backward";
            _BtnLoop = new MediaButton(Skill.Editor.Resources.UITextures.Loop, Skill.Editor.Resources.UITextures.LoopOn) { Column = 5, TogglePressed = true }; _BtnLoop.Content.tooltip = "Loop preview playback of loop section";

            _PlayButtons[0] = _BtnPlay;
            _PlayButtons[1] = _BtnPause;
            _PlayButtons[2] = _BtnStepBackward;
            _PlayButtons[3] = _BtnStepForward;
            _PlayButtons[4] = _BtnLoop;

            _PlaybackButtonsPanel.Controls.Add(_BtnAutoKey);
            for (int i = 0; i < _PlayButtons.Length; i++)
                _PlaybackButtonsPanel.Controls.Add(_PlayButtons[i]);

            _ToolbarPanel.Controls.Add(_PlaybackButtonsPanel);

            _BtnAutoKey.Click += _BtnAutoKey_Click;
            _BtnPlay.Click += _BtnPlay_Click;
            _BtnPause.Click += _BtnPause_Click;
            _BtnLoop.Click += _BtnLoop_Click;
            _BtnStepBackward.Click += _BtnStepBackward_Click;
            _BtnStepForward.Click += _BtnStepForward_Click;

            IsAutoKey = false;
            #endregion

            #region SnapTime
            _SnapTime = new IntPopup() { Column = 1 };
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
            _SnapTime.SelectedValue = 0;
            _ToolbarPanel.Controls.Add(_SnapTime);
            _SnapTime.OptionChanged += _SnapTime_OptionChanged;
            #endregion

            #region FPS
            _Fps = new IntPopup() { Column = 2 };
            _Fps.Label.tooltip = "Sets the timeline granularity for visualiization purposes.";
            _Fps.Options.Add(new PopupOption(30, "30 fps"));
            _Fps.Options.Add(new PopupOption(24, "24 fps"));
            _Fps.Options.Add(new PopupOption(20, "20 fps"));
            _Fps.Options.Add(new PopupOption(60, "60 fps"));

            _Fps.SelectedValue = 30;
            _ToolbarPanel.Controls.Add(_Fps);
            #endregion

            #region PlaybackSpeed

            _PlaybackSpeed = new IntPopup() { Column = 3 };
            _PlaybackSpeed.Options.Add(new PopupOption(100, "100%"));
            _PlaybackSpeed.Options.Add(new PopupOption(50, "50%"));
            _PlaybackSpeed.Options.Add(new PopupOption(25, "25%"));
            _PlaybackSpeed.Options.Add(new PopupOption(10, "10%"));
            _PlaybackSpeed.Options.Add(new PopupOption(1, "1%"));

            _PlaybackSpeed.SelectedValue = 100;
            _ToolbarPanel.Controls.Add(_PlaybackSpeed);
            #endregion

            #region LayoutButtons

            _LayoutButtonsPanel = new Toolbar() { Column = 5 };
            _TBtnDopeSheet = new ToolbarButton();
            _TBtnCurves = new ToolbarButton();

            _TBtnDopeSheet.Content.text = "Dope Sheet"; _TBtnDopeSheet.Content.tooltip = "Dope Sheet";
            _TBtnCurves.Content.text = "Curves"; _TBtnCurves.Content.tooltip = "Curves";

            _LayoutButtonsPanel.Items.Add(_TBtnDopeSheet);
            _LayoutButtonsPanel.Items.Add(_TBtnCurves);
            _LayoutButtonsPanel.SelectedIndex = 0;

            _ToolbarPanel.Controls.Add(_LayoutButtonsPanel);

            _TBtnDopeSheet.Selected += LayoutButtons_Selected;
            _TBtnCurves.Selected += LayoutButtons_Selected;

            #endregion
        }

        void LayoutButtons_Selected(object sender, System.EventArgs e)
        {
            SetLayout(_LayoutButtonsPanel.SelectedIndex);
        }

        private bool _IgnoreLayout;
        private void SetLayout(int layoutType)
        {
            if (_IgnoreLayout) return;
            if (_ED != null)
                _ED.LayoutType = layoutType;

            _IgnoreLayout = true;
            switch (layoutType)
            {
                case 0: // dope sheets

                    _CurveTrackTreeView.Visibility = Visibility.Hidden;
                    _CurveEditor.Visibility = Visibility.Hidden;
                    _TimeLine.Visibility = Visibility.Visible;
                    _TracksTreeView.Visibility = Visibility.Visible;


                    break;
                default: // curves

                    _CurveTrackTreeView.Visibility = Visibility.Visible;
                    _CurveEditor.Visibility = Visibility.Visible;
                    _TimeLine.Visibility = Visibility.Hidden;
                    _TracksTreeView.Visibility = Visibility.Hidden;

                    if (_CurveEditor != null)
                        _CurveEditor.Invalidate();
                    break;
            }
            _LayoutButtonsPanel.SelectedIndex = layoutType;
            _IgnoreLayout = false;
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
        void _BtnAutoKey_Click(object sender, System.EventArgs e)
        {
            IsAutoKey = !IsAutoKey;
        }


        void _SnapTime_OptionChanged(object sender, System.EventArgs e)
        {
            if (_SnapTime.SelectedOption != null)
                SetSnap((double)_SnapTime.SelectedOption.UserData);
        }



        #endregion

        #region MenuBar

        private void FindSceneMatinees()
        {
            if (_Frame == null) return;
            _ItmSceneMatinee.Clear();
            Matinee[] matinees = GameObject.FindObjectsOfType<Matinee>();
            if (matinees != null && matinees.Length > 0)
            {
                for (int i = 0; i < matinees.Length; i++)
                {
                    Skill.Editor.UI.MenuItem item = new UI.MenuItem(matinees[i].gameObject.name);
                    item.UserData = matinees[i];
                    item.Click += MatineeItem_Click;
                    _ItmSceneMatinee.Add(item);
                }
            }
        }

        void MatineeItem_Click(object sender, System.EventArgs e)
        {
            Skill.Editor.UI.MenuItem item = (Skill.Editor.UI.MenuItem)sender;
            if (item != null)
                Matinee = (Matinee)item.UserData;
        }

        private MenuBar _MenuBar;
        private Skill.Editor.UI.MenuItem _ItmSceneMatinee;
        private Skill.Editor.UI.MenuItem _ItmRollBack;

        private void CreateMenuBar()
        {
            _MenuBar = new Skill.Editor.UI.MenuBar() { Dock = Skill.Framework.UI.Dock.Top };

            MenuBarItem file = new MenuBarItem() { Title = "File" };
            MenuBarItem edit = new MenuBarItem() { Title = "Edit" };
            MenuBarItem view = new MenuBarItem() { Title = "View" };
            MenuBarItem window = new MenuBarItem() { Title = "Window", Width = 54 };

            _ItmSceneMatinee = new UI.MenuItem("Matinees");
            file.Add(_ItmSceneMatinee);

            _MenuBar.Controls.Add(file);
            _MenuBar.Controls.Add(edit);
            _MenuBar.Controls.Add(view);
            _MenuBar.Controls.Add(window);


            Skill.Editor.UI.MenuItem resetLayout = new Skill.Editor.UI.MenuItem("Reset Layout");
            window.Add(resetLayout);
            resetLayout.Click += resetLayout_Click;

            _ItmRollBack = new Skill.Editor.UI.MenuItem("Rollback");
            edit.Add(_ItmRollBack);
            _ItmRollBack.Click += mnuRollback_Click;

        }

        void mnuRollback_Click(object sender, System.EventArgs e)
        {
            Rollback();
        }

        void resetLayout_Click(object sender, System.EventArgs e)
        {
            if (_ED != null)
            {
                _ED.SetDefaultLayout();
                LoadLayout();
            }
            else
            {
                _MainGrid.ColumnDefinitions[0].Width = new GridLength(1.0f, GridUnitType.Star); // _CurveTreeView, _TracksTreeView and _PropertyGrid                
                _MainGrid.ColumnDefinitions[2].Width = new GridLength(2.0f, GridUnitType.Star); // _TimeLine  and _CurveEditor
            }
        }
        #endregion

        #region Load / Save

        // refresh styles when state changed between editor mode and play mode
        private void RefreshStyles()
        {
            _LayoutButtonsPanel.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _ToolbarBg.Style = Skill.Editor.Resources.Styles.Toolbar;
            _BtnAutoKey.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnPlay.SetStyle(Skill.Editor.Resources.Styles.ToolbarButton);
            _BtnPause.SetStyle(Skill.Editor.Resources.Styles.ToolbarButton);
            _BtnStepBackward.SetStyle(Skill.Editor.Resources.Styles.ToolbarButton);
            _BtnStepForward.SetStyle(Skill.Editor.Resources.Styles.ToolbarButton);
            _BtnLoop.SetStyle(Skill.Editor.Resources.Styles.ToolbarButton);


            _SnapTime.Style = Skill.Editor.Resources.Styles.ToolbarPopup;
            _Fps.Style = Skill.Editor.Resources.Styles.ToolbarPopup;
            _PlaybackSpeed.Style = Skill.Editor.Resources.Styles.ToolbarPopup;
            _VSplitter.Style = Skill.Editor.Resources.Styles.VerticalSplitter;
        }



        private void CheckSeleced()
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
                            Matinee = newMatinee;
                        else
                            Matinee = null;
                    }
                }
            }
        }

        private void SetEnable(bool enable)
        {
            _MainGrid.IsEnabled = enable;
            _ToolbarPanel.IsEnabled = enable;
            _ItmRollBack.IsEnabled = enable;
        }

        // refresh editor data to to changes happened to matinee outside of MatineeEditor
        private void Refresh()
        {
            if (_Frame != null)
            {
                if (Matinee == null)
                {
                    Clear();
                    SetEnable(false);
                }
                else
                {
                    LoadEditorData();
                    _TracksTreeView.Refresh();
                    _CurveTrackTreeView.RemoveDestroyed();
                    foreach (BaseTrackBar bt in TrackBars())
                    {
                        if (bt.IsEditingCurves)
                            EditCurve(bt);
                    }
                    SetEnable(true);
                }
            }
        }

        // load timeline state from matinee
        private void LoadEditorData()
        {
            if (Matinee != null)
            {
                _TimeLine.MaxTime = EditorData.MaxTime;
                _TimeLine.StartVisible = EditorData.StartVisible;
                _TimeLine.EndVisible = EditorData.EndVisible;
                _TimeLine.TimePosition = EditorData.TimePosition;
                _TimeLine.SelectTime(EditorData.StartSelection, EditorData.EndSelection);

                _SnapTime.SelectedValue = (int)EditorData.Snap;
                _SnapTimeBeforePlay = _SnapTime.SelectedValue;
                _Fps.SelectedValue = (int)EditorData.Fps;
                _PlaybackSpeed.SelectedValue = (int)EditorData.PlaybackSpeed;

                _TimeLine.TimeBar.TimeStyle = EditorData.DropSheetTimeStyle > 1;
                _CurveEditor.TimeLine.TimeBar.TimeStyle = EditorData.CurveEditorTimeStyle > 1;

                LoadLayout();
            }
        }

        private void LoadLayout()
        {
            _MainGrid.ColumnDefinitions[0].Width = new GridLength(EditorData.MainGridColumn0, GridUnitType.Star); // _CurveTreeView, _TracksTreeView and _PropertyGrid                
            _MainGrid.ColumnDefinitions[2].Width = new GridLength(EditorData.MainGridColumn2, GridUnitType.Star); // _TimeLine  and _CurveEditor

            SetLayout((int)EditorData.LayoutType);
        }

        // save timeline state to matinee
        private void SaveEditorData()
        {
            if (Matinee != null)
            {
                EditorData.MaxTime = (float)_TimeLine.MaxTime;
                EditorData.StartVisible = (float)_TimeLine.StartVisible;
                EditorData.EndVisible = (float)_TimeLine.EndVisible;
                EditorData.StartSelection = (float)_TimeLine.StartSelection;
                EditorData.EndSelection = (float)_TimeLine.EndSelection;
                EditorData.TimePosition = (float)_TimeLine.TimePosition;

                EditorData.Snap = _SnapTime.SelectedValue;
                EditorData.Fps = _Fps.SelectedValue;
                EditorData.PlaybackSpeed = _PlaybackSpeed.SelectedValue;

                EditorData.DropSheetTimeStyle = _TimeLine.TimeBar.TimeStyle ? 2 : 0;
                EditorData.CurveEditorTimeStyle = _CurveEditor.TimeLine.TimeBar.TimeStyle ? 2 : 0;

                SaveLayout();

                EditorUtility.SetDirty(Matinee);
            }
        }

        private void SaveLayout()
        {
            EditorData.MainGridColumn0 = _MainGrid.ColumnDefinitions[0].Width.Value;
            EditorData.MainGridColumn2 = _MainGrid.ColumnDefinitions[2].Width.Value;
            EditorData.LayoutType = _LayoutButtonsPanel.SelectedIndex;
        }


        private MatineeEditorData _ED;
        private MatineeEditorData EditorData
        {
            get
            {
                if (_ED == null)
                    _ED = new MatineeEditorData();

                _ED.Matinee = Matinee;
                return _ED;
            }
        }

        class MatineeEditorData
        {
            private Matinee _Matinee;
            public Matinee Matinee
            {
                get { return _Matinee; }
                set
                {
                    _Matinee = value;
                    if (_Matinee != null)
                    {
                        if (_Matinee.EditorData == null || _Matinee.EditorData.Length != 20)
                        {
                            _Matinee.EditorData = new float[20];
                            SetDefault();
                        }

                        if (Fps == 0) Fps = 30;
                        if (PlaybackSpeed == 0) PlaybackSpeed = 100;

                    }
                }
            }

            private float GetFloat(int index, float defaultValue)
            {
                if (Matinee != null)
                {
                    if (Matinee.EditorData != null && Matinee.EditorData.Length > index)
                        return Matinee.EditorData[index];
                }
                return defaultValue;
            }
            private void SetFloat(int index, float value)
            {
                if (Matinee != null)
                {
                    if (Matinee.EditorData != null && Matinee.EditorData.Length > index)
                        Matinee.EditorData[index] = value;
                }
            }



            public float MaxTime { get { return GetFloat(0, 1.0f); } set { SetFloat(0, value); } }
            public float StartVisible { get { return GetFloat(1, 0.0f); } set { SetFloat(1, value); } }
            public float EndVisible { get { return GetFloat(2, 1.0f); } set { SetFloat(2, value); } }
            public float StartSelection { get { return GetFloat(3, 0.0f); } set { SetFloat(3, value); } }
            public float EndSelection { get { return GetFloat(4, 0.0f); } set { SetFloat(4, value); } }
            public float TimePosition { get { return GetFloat(5, 0.0f); } set { SetFloat(5, value); } }

            public float MainGridColumn0 { get { return GetFloat(6, 1.0f); } set { SetFloat(6, value); } }
            public float MainGridColumn2 { get { return GetFloat(7, 2.0f); } set { SetFloat(7, value); } }
            public float LayoutType { get { return GetFloat(8, 0); } set { SetFloat(8, value); } }


            public float Snap { get { return GetFloat(9, 0); } set { SetFloat(9, value); } }
            public float Fps { get { return GetFloat(10, 0); } set { SetFloat(10, value); } }
            public float PlaybackSpeed { get { return GetFloat(11, 0); } set { SetFloat(11, value); } }

            public float DropSheetTimeStyle { get { return GetFloat(12, 0); } set { SetFloat(12, value); } }
            public float CurveEditorTimeStyle { get { return GetFloat(13, 0); } set { SetFloat(13, value); } }

            public void SetDefault()
            {
                MaxTime = 1.0f;
                StartVisible = 0.0f;
                EndVisible = 1.0f;
                StartSelection = 0.0f;
                EndSelection = 0.0f;
                TimePosition = 0.0f;
                Snap = 0;
                Fps = 30;
                PlaybackSpeed = 100;
                DropSheetTimeStyle = 0;
                CurveEditorTimeStyle = 0;
                SetDefaultLayout();
            }

            public void SetDefaultLayout()
            {
                MainGridColumn0 = 1.0f;
                MainGridColumn2 = 2.0f;
                LayoutType = 0.0f;
            }
        }
        #endregion

        #region Playback

        private float _PlayStartOffset;
        private float _PlayStartTime;
        private float _DeltaTime;// delta time since last playback update
        private float _ContinuousPlayTime; // playback time
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
            float speed = (float)_PlaybackSpeed.SelectedValue / 100.0f;
            _DeltaTime += stepTime;
            _ContinuousPlayTime = _PlayStartOffset + (Time.realtimeSinceStartup - _PlayStartTime) * speed;
            float fpsTime = 1.0f / _Fps.SelectedValue;
            if (_DeltaTime >= fpsTime) // if we reach fpsTime after last step
            {
                _DeltaTime -= fpsTime;
                _PlayTime = _ContinuousPlayTime;

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

                        _ContinuousPlayTime = _PlayTime;
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

                    _ContinuousPlayTime = _PlayTime;
                }
            }
        }

        // step playback in negative stepTime
        private void StepPlaybackBackward(float stepTime)
        {
            float speed = (float)_PlaybackSpeed.SelectedValue / 100.0f;
            _DeltaTime -= stepTime;
            _ContinuousPlayTime -= stepTime * speed;
            if (_DeltaTime < 0) // if we reach fpsTime after last step
            {
                float fpsTime = 1.0f / _Fps.SelectedValue;
                _DeltaTime += fpsTime;
                _PlayTime = _ContinuousPlayTime;

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

                        _ContinuousPlayTime = _PlayTime;
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

                    _ContinuousPlayTime = _PlayTime;
                }
            }
        }

        private void UpdatePlayback()
        {
            if (Matinee != null)
            {
                if (_IsPlaying)
                {
                    float prePlayTime = _PlayTime;
                    if (!_IsPause)
                        StepPlaybackForward(1.0f / 100.0f); // update called 100 time per second
                    else if (_StepForward == 1)
                        StepPlaybackForward(1.0f / _Fps.SelectedValue); // step forward with fpsTime
                    else if (_StepForward == -1)
                        StepPlaybackBackward(1.0f / _Fps.SelectedValue); // step backward with fpsTime
                    _StepForward = 0;

                    if (_ContinuousPlayTime > _TimeLine.MinTime && _ContinuousPlayTime < _TimeLine.MaxTime)
                    {
                        _TimeLine.TimePosition = _ContinuousPlayTime;
                        _CurveEditor.TimeLine.TimePosition = _ContinuousPlayTime;
                    }
                    if (prePlayTime != _PlayTime)
                        Evaluate();
                }
                else
                {
                    _ContinuousPlayTime = _PlayTime = (float)_TimeLine.TimePosition;
                    if (_SeekUpdate) // user changed timeline position and we must force seek to that position
                    {
                        foreach (var tb in TrackBars())
                        {
                            tb.Track.SortKeys();
                            tb.Seek(_PlayTime); // and seek to TimeLine position
                        }
                        _SeekUpdate = false;
                        SceneView.RepaintAll();
                    }
                }
            }
        }

        private void Evaluate()
        {
            foreach (var t in TrackBars())
                t.Evaluate(_PlayTime);
            SceneView.RepaintAll();
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
            _PlayStartOffset = (float)_TimeLine.TimePosition;
            _PlayStartTime = Time.realtimeSinceStartup;
            foreach (var t in Tracks())
            {
                t.SortKeys();
                t.Seek(_PlayTime);
            }
            Repaint();// repaint to force update scene
            _ContinuousPlayTime = _PlayTime;
        }

        // play or resume playback
        private void Play()
        {
            if (Matinee == null) return;
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
            if (Matinee == null) return;
            _IsPlaying = false;
            _IsPause = false;
            UpdatePlayButtonsState(false); // update state of playback buttons
            _BtnPlay.IsPressed = false;
            _BtnPause.IsPressed = false;

            _TimeLine.TimeBar.SnapTime = _SnapTimeBeforePlay;// rollback snaptime
            _CurveEditor.TimeLine.TimeBar.SnapTime = _SnapTimeBeforePlay;// rollback snaptime
            _TimeLine.TimePosition = _TimePositionBeforePlay; // rollback time position

            foreach (var t in Tracks()) t.Stop();
        }

        // pause playback
        private void Pause()
        {
            if (Matinee == null) return;
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
            {
                _TimeLine.TimeBar.SnapTime = snapTime;
                _CurveEditor.TimeLine.TimeBar.SnapTime = snapTime;
            }
        }
        #endregion

    }



    #region MatineeEditor
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
    #endregion
}