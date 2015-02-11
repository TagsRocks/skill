using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System.Collections.Generic;
using Skill.Framework.Audio;

namespace Skill.Editor.Audio
{

    public class AudioStateNode : Grid, Skill.Editor.UI.IProperties, Skill.Editor.UI.ISelectable
    {

        private List<AudioTransition> _Transitions = new List<AudioTransition>();
        internal List<AudioTransition> Transitions { get { return _Transitions; } }
        internal AudioStateNode NextState { get; set; }

        private static GUIStyle _DefaultStyle;
        private static GUIStyle _DefaultSelectedStyle;
        private static GUIStyle _Style;
        private static GUIStyle _SelectedStyle;

        private bool _IsDefault;
        public bool IsDefault
        {
            get { return _IsDefault; }
            set
            {
                if (_IsDefault != value)
                {
                    _IsDefault = value;
                    _RefreshSyles = true;
                }
            }
        }

        private Box _Background;
        //private Image _NextSlot;
        //private Image _PreSlot;
        private AudioStateNodeDragThumb _Drag;
        private bool _RefreshSize;
        public AudioState State { get; private set; }
        public string Header
        {
            get { return _Background.Content.text; }
            set
            {
                _Background.Content.text = value;
                _RefreshSize = true;
            }
        }

        public Vector2 NextPoint
        {
            get
            {
                Vector2 p = RenderArea.center;
                p.x = RenderArea.xMax;
                return p;
            }
        }
        public Vector2 PrePoint
        {
            get
            {
                Vector2 p = RenderArea.center;
                p.x = RenderArea.xMin;
                return p;
            }
        }

        public AudioStateNode(AudioState state)
        {
            this.Height = 45;
            this.State = state;
            this.RowDefinitions.Add(1, GridUnitType.Star);

            _Background = new Box() { Row = 0, Column = 0 };
            this.Controls.Add(_Background);

            //_NextSlot = new Image() { Width = 16, Height = 16, VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Top, HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Right, Margin = new Thickness(0, 0, -16, 0) };
            //this.Controls.Add(_NextSlot);

            //_PreSlot = new Image() { Width = 16, Height = 16, VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Bottom, HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Left, Margin = new Thickness(-16, 0, 0, 0) };
            //this.Controls.Add(_PreSlot);


            _Drag = new AudioStateNodeDragThumb(this) { Row = 0, Column = 0 };
            this.Controls.Add(_Drag);


            _RefreshSyles = true;

            this.Header = state.Name;
            this.X = state.X;
            this.Y = state.Y;

            this.ContextMenu = AudioStateNodeContextMenu.Instance;
        }
        private bool _RefreshSyles;
        protected override void BeginRender()
        {
            if (_RefreshSize)
            {
                float size = 0;
                if (!string.IsNullOrEmpty(_Background.Content.text))
                    size = UnityEditor.EditorStyles.label.CalcSize(_Background.Content).x + 20;
                this.Width = Mathf.Max(120, size);
            }
            if (_RefreshSyles)
            {
                _RefreshSyles = false;
                //_NextSlot.Texture = Skill.Editor.Resources.UITextures.ArrowRight;
                //_PreSlot.Texture = Skill.Editor.Resources.UITextures.ArrowRight;
                SetBackgroundStyle();
            }
            base.BeginRender();
        }


        public void Save()
        {
            State.X = this.X;
            State.Y = this.Y;
            State.Transitions = _Transitions.ToArray();
            if (this.NextState != null)
                State.NextState = this.NextState.StateName;
            else
                State.NextState = string.Empty;
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (value != _IsSelected)
                {
                    _IsSelected = value;
                    SetBackgroundStyle();

                }
            }
        }
        private void SetBackgroundStyle()
        {
            ValidateStyles();
            if (_IsSelected)
            {
                if (IsDefault)
                    _Background.Style = _DefaultSelectedStyle;
                else
                    _Background.Style = _SelectedStyle;
            }
            else
            {
                if (IsDefault)
                    _Background.Style = _DefaultStyle;
                else
                    _Background.Style = _Style;
            }
        }
        private void ValidateStyles()
        {
            if (_DefaultStyle == null)
            {
                _DefaultStyle = new GUIStyle((GUIStyle)"flow node 5");
                _DefaultSelectedStyle = new GUIStyle((GUIStyle)"flow node 5 on");
                _Style = new GUIStyle((GUIStyle)"flow node 0");
                _SelectedStyle = new GUIStyle((GUIStyle)"flow node 0 on");
            }
        }

        #region IProperties members

        protected class ItemProperties : Skill.Editor.UI.ExposeProperties, AudioPreviewHandler
        {
            private AudioStateNode _Node;
            private Skill.Editor.UI.IntPopup _NextStateOptions;
            private BreakPointsEditor _BreakPointsEditor;
            private Skill.Editor.UI.UntypedObjectField _ClipField;
            private Skill.Editor.UI.MediaButton _BtnPreview;

            public ItemProperties(AudioStateNode node)
                : base(node)
            {
                _Node = node;
            }

            protected override void SetDirty()
            {
                Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(_Node);
            }

            protected override void CreateCustomFileds()
            {
                base.CreateCustomFileds();
                _NextStateOptions = new Skill.Editor.UI.IntPopup() { Margin = ControlMargin };
                _NextStateOptions.Label.text = "Next State";
                _NextStateOptions.OptionChanged += _NextStateOptions_OptionChanged;
                Controls.Add(_NextStateOptions);

                _BreakPointsEditor = new BreakPointsEditor(this, ((AudioStateNode)Object).State);
                Controls.Add(_BreakPointsEditor);

                _BtnPreview = new Skill.Editor.UI.MediaButton() { TogglePressed = false, Height = 20 };
                _BtnPreview.Content.text = "Preview";
                _BtnPreview.Content.image = UnityEditor.EditorGUIUtility.FindTexture("d_PlayButton");
                _BtnPreview.SetStyle(UnityEditor.EditorStyles.miniButton);
                Controls.Add(_BtnPreview);

                _BtnPreview.Click += _BtnPreview_Changed;
            }

            void _BtnPreview_Changed(object sender, System.EventArgs e)
            {
                AudioStateGraphEditor editor = _Node.FindInParents<AudioStateGraphEditor>();
                if (editor != null)
                {
                    if (!_BtnPreview.IsPressed)
                        editor.StartPreview(this, _Node.State.Clip, _Node.State.Begin, _Node.State.End);
                    else
                        editor.StopPreview();

                }
            }

            void _NextStateOptions_OptionChanged(object sender, System.EventArgs e)
            {
                if (IgnoreChanges) return;
                if (_NextStateOptions.SelectedOption != null)
                    _Node.NextState = _NextStateOptions.SelectedOption.UserData as AudioStateNode;
                else
                    _Node.NextState = null;
            }

            protected override void RefreshData()
            {
                base.RefreshData();

                if (_ClipField == null)
                {
                    foreach (var c in Controls)
                    {
                        if (_ClipField == null && c is Skill.Editor.UI.UntypedObjectField)
                        {
                            if (((Skill.Editor.UI.UntypedObjectField)c).ObjectType == typeof(AudioClip))
                            {
                                _ClipField = (Skill.Editor.UI.UntypedObjectField)c;
                                _ClipField.ObjectChanged += _ClipField_ObjectChanged;
                            }
                        }
                        else if (c is Skill.Editor.UI.FloatField)
                        {
                            Skill.Editor.UI.FloatField ff = (Skill.Editor.UI.FloatField)c;

                            if (ff.Label.text == "Start" || ff.Label.text == "End")
                            {
                                ff.ValueChanged += StartEnd_ValueChanged;
                            }
                        }
                    }
                }

                _BtnPreview.IsPressed = false;
                _BreakPointsEditor.RefreshData();
                _NextStateOptions.Options.Clear();
                AudioStateGraphEditor editor = _Node.FindInParents<AudioStateGraphEditor>();
                AudioStateNode[] states = editor.GetStates();

                Skill.Editor.UI.PopupOption noneOp = new Skill.Editor.UI.PopupOption(-1, " ");
                _NextStateOptions.Options.Add(noneOp);

                Skill.Editor.UI.PopupOption selectedOp = noneOp;
                for (int i = 0; i < states.Length; i++)
                {
                    if (states[i] == _Node) continue;
                    Skill.Editor.UI.PopupOption op = new Skill.Editor.UI.PopupOption(i, states[i].StateName);
                    op.UserData = states[i];
                    if (states[i] == _Node.NextState)
                        selectedOp = op;
                    _NextStateOptions.Options.Add(op);
                }
                _NextStateOptions.SelectedOption = selectedOp;
            }

            void StartEnd_ValueChanged(object sender, System.EventArgs e)
            {
                if (IgnoreChanges) return;
                _BreakPointsEditor.RefreshAudio();
            }

            void _ClipField_ObjectChanged(object sender, System.EventArgs e)
            {
                if (IgnoreChanges) return;
                _BreakPointsEditor.RefreshAudio();
            }

            public void PreviewStarted()
            {
                _BtnPreview.IsPressed = true;
            }

            public void PreviewStopped()
            {
                _BtnPreview.IsPressed = false;
            }

            public void UpdatePreview(float previewTime)
            {

            }

            class BreakPointsEditor : Grid
            {
                private bool _RefreshStyle;
                private Skill.Framework.UI.ListBox _BreakPointsList;
                private Skill.Editor.UI.AudioPreviewCurve _AudioPreview;

                private Skill.Framework.UI.Label _Title;
                private Skill.Framework.UI.Grid _ButtonsPanel;
                private Skill.Framework.UI.Button _BtnAdd;
                private Skill.Framework.UI.Button _BtnRemove;
                private GUIStyle _TimePosStyle;

                private ItemProperties _OwnerProperties;
                public AudioState State { get; private set; }

                public void RefreshAudio()
                {
                    _AudioPreview.Clip = State.Clip;
                    _AudioPreview.SetTime(State.Begin, State.End);
                }

                public void RefreshData()
                {
                    RefreshAudio();

                    if (State.BreakPoints == null)
                        State.BreakPoints = new float[0];
                    _BreakPointsList.SelectedIndex = -1;
                    _BreakPointsList.Items.Clear();
                    foreach (var bp in State.BreakPoints)
                        Add(bp);
                }

                private void Add(float value)
                {
                    Skill.Editor.UI.FloatField field = new Skill.Editor.UI.FloatField() { Value = value, Margin = new Thickness(2) };
                    _BreakPointsList.Items.Add(field);
                    SetLable(field);
                    field.ValueChanged += field_ValueChanged;
                }

                private void SetLable(Skill.Editor.UI.FloatField field)
                {
                    field.Label.text = string.Format("Break Point {0}", _BreakPointsList.Items.IndexOf(field));
                }

                private bool _IgnoreField;
                void field_ValueChanged(object sender, System.EventArgs e)
                {
                    if (_IgnoreField) return;
                    Skill.Editor.UI.FloatField field = (Skill.Editor.UI.FloatField)sender;
                    _IgnoreField = true;
                    int index = _BreakPointsList.Items.IndexOf(field);
                    State.BreakPoints[index] = Mathf.Clamp(field.Value, State.Begin, State.End);
                    field.Value = State.BreakPoints[index];
                    _IgnoreField = false;
                }

                public override float LayoutHeight
                {
                    get
                    {
                        return Mathf.Max(40, _BreakPointsList.Items.Count * 20) + 16 + 16 + 60 + 4;
                    }
                }

                public BreakPointsEditor(ItemProperties owner, AudioState state)
                {
                    _OwnerProperties = owner;
                    this.State = state;
                    this._RefreshStyle = true;

                    this.RowDefinitions.Add(16, GridUnitType.Pixel); // title
                    this.RowDefinitions.Add(1, GridUnitType.Star); // list
                    this.RowDefinitions.Add(16, GridUnitType.Pixel); // buttons
                    this.RowDefinitions.Add(60, GridUnitType.Pixel); // Audio Preview

                    _Title = new Label { Row = 0, Text = "Break Points" };
                    this.Controls.Add(_Title);

                    _BreakPointsList = new Skill.Framework.UI.ListBox() { Row = 1 };
                    _BreakPointsList.DisableFocusable();
                    _BreakPointsList.BackgroundVisible = true;
                    this.Controls.Add(_BreakPointsList);

                    _ButtonsPanel = new Grid() { Row = 2 };
                    _ButtonsPanel.ColumnDefinitions.Add(1, GridUnitType.Star);
                    _ButtonsPanel.ColumnDefinitions.Add(20, GridUnitType.Pixel);
                    _ButtonsPanel.ColumnDefinitions.Add(20, GridUnitType.Pixel);
                    this.Controls.Add(_ButtonsPanel);

                    _BtnAdd = new Button() { Column = 1 };
                    _ButtonsPanel.Controls.Add(_BtnAdd);

                    _BtnRemove = new Button() { Column = 2, IsEnabled = false };
                    _ButtonsPanel.Controls.Add(_BtnRemove);

                    _AudioPreview = new Skill.Editor.UI.AudioPreviewCurve() { Row = 3 };
                    this.Controls.Add(_AudioPreview);

                    _BreakPointsList.SelectionChanged += _BreakPointsList_SelectionChanged;
                    _BtnAdd.Click += _BtnAdd_Click;
                    _BtnRemove.Click += _BtnRemove_Click;
                }

                void _BtnRemove_Click(object sender, System.EventArgs e)
                {
                    RemoveSelectedBreakPoint();
                    _OwnerProperties.SetDirty();
                    foreach (Skill.Editor.UI.FloatField ff in _BreakPointsList.Items)
                        SetLable(ff);
                }

                void _BtnAdd_Click(object sender, System.EventArgs e)
                {
                    AddNewBreakPoint();
                    _OwnerProperties.SetDirty();
                }

                void _BreakPointsList_SelectionChanged(object sender, System.EventArgs e)
                {
                    _BtnRemove.IsEnabled = _BreakPointsList.SelectedItem != null;
                }

                protected override void BeginRender()
                {
                    if (_RefreshStyle)
                    {
                        _RefreshStyle = false;

                        if (_TimePosStyle == null)
                        {
                            _TimePosStyle = new GUIStyle((GUIStyle)"MeBlendPosition");
                            _TimePosStyle.fixedHeight = 0;
                        }

                        if (_Title.Style == null)
                        {
                            _Title.Style = new GUIStyle((GUIStyle)"RL Header");
                            _Title.Style.alignment = TextAnchor.MiddleCenter;
                        }

                        _BreakPointsList.SelectedStyle = Skill.Editor.Resources.Styles.SelectedItem;

                        _BtnAdd.Content.image = Skill.Editor.Resources.UITextures.Plus;
                        _BtnRemove.Content.image = Skill.Editor.Resources.UITextures.Minus;

                        _BtnAdd.Style = Skill.Editor.Resources.Styles.SmallButton;
                        _BtnRemove.Style = Skill.Editor.Resources.Styles.SmallButton;
                    }
                    base.BeginRender();
                }

                protected override void Render()
                {
                    base.Render();
                    Rect ra = _AudioPreview.RenderArea;
                    float width = ra.width;
                    float x = ra.x - 4;
                    ra.width = 1;
                    if (State.BreakPoints != null)
                    {
                        float length = State.End - State.Begin;
                        if (length > 0)
                        {
                            for (int i = 0; i < State.BreakPoints.Length; i++)
                            {
                                ra.x = x + width * ((State.BreakPoints[i] - State.Begin) / length);
                                GUI.Box(ra, string.Empty, _TimePosStyle);
                            }
                        }
                    }
                }

                private void RemoveSelectedBreakPoint()
                {
                    if (_BreakPointsList.SelectedItem != null)
                    {
                        int selectedIndex = _BreakPointsList.SelectedIndex;
                        float[] preBreakPoints = State.BreakPoints;
                        float[] newBreakPoints = new float[preBreakPoints.Length - 1];

                        int preIndex = 0;
                        int newIndex = 0;
                        while (newIndex < newBreakPoints.Length && preIndex < preBreakPoints.Length)
                        {
                            if (preIndex == selectedIndex)
                            {
                                preIndex++;
                                continue;
                            }
                            newBreakPoints[newIndex] = preBreakPoints[preIndex];
                            newIndex++;
                            preIndex++;
                        }
                        State.BreakPoints = newBreakPoints;
                        _BreakPointsList.Items.Remove(_BreakPointsList.SelectedItem);
                        _BreakPointsList.SelectedIndex = -1;
                    }
                    else
                    {
                        Debug.LogError("there is no selected BreakPoint to remove");
                    }
                }
                private void AddNewBreakPoint()
                {
                    float[] preBreakPoints = State.BreakPoints;
                    float[] newBreakPoints = new float[preBreakPoints.Length + 1];
                    preBreakPoints.CopyTo(newBreakPoints, 0);

                    float value = State.Begin;
                    if (preBreakPoints.Length > 0)
                        value = preBreakPoints[preBreakPoints.Length - 1];
                    newBreakPoints[newBreakPoints.Length - 1] = value;
                    State.BreakPoints = newBreakPoints;
                    Add(value);
                }
            }



        }

        private ItemProperties _Properties;
        public Skill.Editor.UI.PropertiesPanel Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = new ItemProperties(this);
                return _Properties;
            }
        }
        public bool IsSelectedProperties { get; set; }
        public string Title { get { return "AudioState"; } }

        #endregion

        #region DragThumb
        internal Vector2 StartDrag;
        class AudioStateNodeDragThumb : Skill.Editor.UI.DragThumb
        {
            private AudioStateNode _Node;

            public AudioStateNodeDragThumb(AudioStateNode node)
            {
                this._Node = node;
            }

            protected override void OnDrag(Vector2 delta)
            {
                AudioStateGraphEditor editor = _Node.FindInParents<AudioStateGraphEditor>();
                if (editor != null)
                    editor.ItemDrag(_Node, delta);
            }

            protected override void OnMouseDown(MouseClickEventArgs args)
            {
                if (args.Button == MouseButton.Left)
                {
                    AudioStateGraphEditor editor = _Node.FindInParents<AudioStateGraphEditor>();
                    if (editor != null)
                    {
                        if (args.Ctrl)
                        {
                            if (_Node.IsSelected)
                                editor.Selection.Remove(_Node);
                            else
                                editor.Selection.Add(_Node);
                        }
                        else if (args.Shift)
                        {
                            if (!_Node.IsSelected)
                                editor.Selection.Add(_Node);
                        }
                        else
                        {
                            editor.Selection.Select(_Node);
                        }
                    }
                }
                base.OnMouseDown(args);
            }
        }
        #endregion

        #region Expose Properties

        [Skill.Framework.ExposeProperty(10, "Name", "Name of state")]
        public string StateName
        {
            get { return State.Name; }
            set
            {
                State.Name = value;
                Header = value;
            }
        }

        [Skill.Framework.ExposeProperty(11, "Clip", "AudioClip")]
        public AudioClip Clip
        {
            get { return State.Clip; }
            set
            {
                State.Clip = value;
                if (State.Begin < 0) State.Begin = 0;
                Begin = State.Begin;
                End = State.End;
                if (State.Clip != null)
                {
                    if (State.BreakPoints != null)
                    {
                        for (int i = 0; i < State.BreakPoints.Length; i++)
                            State.BreakPoints[i] = Mathf.Clamp(State.BreakPoints[i], 0, State.Clip.length);
                    }
                }
            }
        }


        [Skill.Framework.ExposeProperty(12, "Category ", "category of audio")]
        public Skill.Framework.Audio.SoundCategory Category
        {
            get { return State.Category; }
            set { State.Category = value; }
        }

        [Skill.Framework.ExposeProperty(13, "Volume Factor", "volume factor of audio (0.0 - 1.0)")]
        public float VolumeFactor
        {
            get { return State.VolumeFactor; }
            set { State.VolumeFactor = Mathf.Clamp01(value); }
        }

        [Skill.Framework.ExposeProperty(14, "Start", "start time")]
        public float Begin
        {
            get { return State.Begin; }
            set
            {
                State.Begin = Mathf.Max(0, value);
                if (State.Clip != null)
                    if (State.Begin > State.Clip.length)
                        State.Begin = State.Clip.length;

            }
        }


        [Skill.Framework.ExposeProperty(15, "End", "end time")]
        public float End
        {
            get { return State.End; }
            set
            {
                State.End = Mathf.Max(0, value);
                if (State.Clip != null)
                {
                    if (State.End > State.Clip.length)
                        State.End = State.Clip.length;
                }
            }
        }

        [Skill.Framework.ExposeProperty(16, "SaveTime", "save stopped time")]
        public bool SaveTime { get { return State.SaveTime; } set { State.SaveTime = value; } }

        #endregion

        #region ContextMenu

        class AudioStateNodeContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static AudioStateNodeContextMenu _Instance;
            Skill.Editor.UI.MenuItem _MnuMakeTransition;
            Skill.Editor.UI.MenuItem _MnuSetAsDefault;
            Skill.Editor.UI.MenuItem _MnuDelete;
            public static AudioStateNodeContextMenu Instance
            {
                get
                {
                    if (_Instance == null)
                        _Instance = new AudioStateNodeContextMenu();
                    return _Instance;
                }
            }

            protected override void BeginShow()
            {
                base.BeginShow();
                AudioStateNode node = (AudioStateNode)Owner;
                _MnuSetAsDefault.IsEnabled = !node.IsDefault;
            }

            private AudioStateNodeContextMenu()
            {
                _MnuMakeTransition = new Skill.Editor.UI.MenuItem("Make Transition");
                _MnuMakeTransition.Click += mnuCreateTransition_Click;
                this.Add(_MnuMakeTransition);

                _MnuSetAsDefault = new Skill.Editor.UI.MenuItem("Set As Default");
                _MnuSetAsDefault.Click += mnuSetAsDefault_Click;
                this.Add(_MnuSetAsDefault);

                this.AddSeparator();

                _MnuDelete = new Skill.Editor.UI.MenuItem("Delete");
                _MnuDelete.Click += mnuDelete_Click;
                this.Add(_MnuDelete);
            }

            void mnuSetAsDefault_Click(object sender, System.EventArgs e)
            {
                AudioStateNode node = (AudioStateNode)Owner;
                AudioStateGraphEditor editor = Owner.FindInParents<AudioStateGraphEditor>();
                editor.MakeAsDefaultState(node);
            }

            void mnuCreateTransition_Click(object sender, System.EventArgs e)
            {
                AudioStateNode node = (AudioStateNode)Owner;
                AudioStateGraphEditor editor = Owner.FindInParents<AudioStateGraphEditor>();
                editor.BeginConnection(node);
            }

            void mnuDelete_Click(object sender, System.EventArgs e)
            {
                AudioStateNode node = (AudioStateNode)Owner;
                AudioStateGraphEditor editor = Owner.FindInParents<AudioStateGraphEditor>();
                editor.RemoveNode(node);
            }
        }

        #endregion
    }
}