using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework.UI;
using System.Linq;
using Skill.Framework.Audio;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Audio
{
    public class AudioStateGraphEditor : Grid, IEnumerable<Skill.Editor.UI.ISelectable>
    {
        private AudioControllerEditorWindow _Editor;
        private Skill.Framework.UI.Box _Background;
        private AudioConnectionHost _ConnectionHost;
        private Skill.Editor.UI.Extended.ZoomPanel _Panel;
        private Skill.Editor.UI.MultiSelector<Skill.Editor.UI.ISelectable> _MultiSelector;

        public Skill.Editor.UI.SelectableCollection<Skill.Editor.UI.ISelectable> Selection { get; private set; }
        public AudioStateGraphEditor(AudioControllerEditorWindow editor)
        {
            this._Editor = editor;

            this.Selection = new Skill.Editor.UI.SelectableCollection<Skill.Editor.UI.ISelectable>();
            this.Selection.SelectionChanged += Selection_SelectionChanged;

            _Background = new Box() { Row = 0 };
            this.Controls.Add(_Background);

            _ConnectionHost = new AudioConnectionHost(this) { Row = 0 };
            this.Controls.Add(_ConnectionHost);

            _Panel = new Skill.Editor.UI.Extended.ZoomPanel() { Row = 0, MinZoomFactor = 0.6f, MaxZoomFactor = 1.0f };
            this.Controls.Add(_Panel);

            this._MultiSelector = new Skill.Editor.UI.MultiSelector<Skill.Editor.UI.ISelectable>(this, Selection) { Row = 1 };
            this.Controls.Add(_MultiSelector);

            this.WantsMouseEvents = true;
            this.ContextMenu = new GraphContextMenu(this);
        }
        void Selection_SelectionChanged(object sender, System.EventArgs e)
        {
            _DragStarted = false;
            Skill.Editor.UI.ISelectable selected = this.Selection.SelectedItem;
            if (selected != null)
            {
                Skill.Editor.UI.Extended.InspectorProperties.Select((Skill.Editor.UI.Extended.IProperties)selected);
            }
            else
            {
                var s = Skill.Editor.UI.Extended.InspectorProperties.GetSelected();
                if (s != null)
                {
                    if (s is AudioStateNode)
                        Skill.Editor.UI.Extended.InspectorProperties.Select(null);
                    if (s is AudioConnection)
                        Skill.Editor.UI.Extended.InspectorProperties.Select(null);
                }
            }
            StopPreview();
        }

        protected override void BeginRender()
        {
            Skill.Editor.LineDrawer.ClipArea = this._Panel.RenderArea;
            base.BeginRender();
        }

        protected override void EndRender()
        {
            Skill.Editor.LineDrawer.ClipArea = null;
            base.EndRender();
        }

        public void DeselectInspector()
        {
            if (Skill.Editor.UI.Extended.InspectorProperties.GetSelected() is AudioStateNode)
                Skill.Editor.UI.Extended.InspectorProperties.Select(null);
            else if (Skill.Editor.UI.Extended.InspectorProperties.GetSelected() is AudioConnection)
                Skill.Editor.UI.Extended.InspectorProperties.Select(null);
        }

        private AudioStateNode Find(AudioState state)
        {
            foreach (var item in this._Panel.Controls)
            {
                if (item is AudioStateNode)
                {
                    if (((AudioStateNode)item).State == state)
                        return (AudioStateNode)item;
                }
            }
            return null;
        }
        public void Rebuild()
        {
            Clear();
            if (_Editor.Controller != null)
            {
                List<AudioStateNode> stateList = new List<AudioStateNode>();
                if (_Editor.Controller.States != null)
                {
                    foreach (var state in _Editor.Controller.States)
                        stateList.Add(CreateNode(state));

                    foreach (var s in _Editor.Controller.States)
                    {
                        AudioStateNode start = FindState(s.Name);
                        if (start != null && s.Transitions != null)
                        {
                            start.Transitions.Clear();
                            foreach (var t in s.Transitions)
                            {
                                AudioStateNode end = FindState(t.Destination);
                                if (end != null)
                                {
                                    start.Transitions.Add(t);
                                    AudioConnection ac = new AudioConnection(start, end, t);
                                    _ConnectionHost.Controls.Add(ac);
                                }
                            }
                        }
                    }

                    foreach (var state in stateList)
                        state.NextState = FindState(state.State.NextState);

                    if (_Editor.Controller.DefaultState != null)
                    {
                        AudioStateNode defaultState = FindState(_Editor.Controller.DefaultState);
                        if (defaultState != null)
                            defaultState.IsDefault = true;
                        else
                        {
                            if (_Panel.Controls.Count > 0)
                            {
                                ((AudioStateNode)_Panel.Controls[0]).IsDefault = true;
                                _Editor.Controller.DefaultState = ((AudioStateNode)_Panel.Controls[0]).StateName;
                            }
                            else
                            {
                                _Editor.Controller.DefaultState = string.Empty;
                            }
                        }
                    }
                }

                _Panel.ZoomFactor = _Editor.Controller.Zoom;
                _Panel.PanPosition = new Vector2(_Editor.Controller.PanX, _Editor.Controller.PanY);
            }
        }

        private AudioStateNode FindState(string stateName)
        {
            foreach (var c in _Panel.Controls)
            {
                if (c is AudioStateNode)
                {
                    if (((AudioStateNode)c).State.Name == stateName)
                        return (AudioStateNode)c;
                }
            }
            return null;
        }

        public AudioStateNode[] GetStates()
        {
            List<AudioStateNode> list = new List<AudioStateNode>();
            foreach (var item in this._Panel.Controls)
            {
                if (item is AudioStateNode)
                    list.Add((AudioStateNode)item);
            }
            return list.ToArray();
        }

        private AudioStateNode CreateNode(AudioState state)
        {
            AudioStateNode node = new AudioStateNode(state);
            node.X = state.X;
            node.Y = state.Y;
            _Panel.Controls.Add(node);
            return node;
        }

        private void New(Vector2 position)
        {
            position -= _Panel.PanPosition;
            position /= _Panel.ZoomFactor;
            position.x -= this.RenderArea.x;
            position.y -= this.RenderArea.y;

            AudioState state = new AudioState();
            state.Name = "new state";
            state.X = position.x;
            state.Y = position.y;
            AudioStateNode node = CreateNode(state);
            if (_Panel.Controls.Count == 1)
                node.IsDefault = true;
        }

        public void MakeAsDefaultState(AudioStateNode node)
        {
            foreach (var c in _Panel.Controls)
            {
                if (c is AudioStateNode)
                {
                    ((AudioStateNode)c).IsDefault = false;
                }
            }
            node.IsDefault = true;
        }

        public void RemoveNode(AudioStateNode node)
        {
            _Panel.Controls.Remove(node);
            List<AudioConnection> connectionList = new List<AudioConnection>();
            foreach (var c in _ConnectionHost.Controls)
            {
                if (c is AudioConnection)
                {
                    AudioConnection ac = (AudioConnection)c;
                    if (ac.Start == node || ac.End == node)
                        connectionList.Add(ac);
                }
            }

            foreach (var item in connectionList)
                _ConnectionHost.Controls.Remove(item);
            connectionList.Clear();

            node.NextState = null;
            foreach (var c in _Panel.Controls)
            {
                if (c is AudioStateNode)
                {
                    AudioStateNode asn = (AudioStateNode)c;
                    if (asn.NextState == node)
                        asn.NextState = null;
                }
            }
        }

        private void Break(AudioConnection connection)
        {
            _ConnectionHost.Controls.Remove(connection);
        }

        public void Save()
        {
            AudioStateNode defaultState = null;
            List<AudioStateNode> stateList = new List<AudioStateNode>();
            foreach (var item in this._Panel.Controls)
            {
                if (item is AudioStateNode)
                {
                    ((AudioStateNode)item).Transitions.Clear();
                    stateList.Add((AudioStateNode)item);

                    if (defaultState == null)
                    {
                        if (((AudioStateNode)item).IsDefault)
                            defaultState = (AudioStateNode)item;
                    }
                    else
                    {
                        ((AudioStateNode)item).IsDefault = false;
                    }
                }
            }

            if (defaultState == null && stateList.Count > 0)
                defaultState = stateList[0];

            foreach (var state in stateList)
                state.StateName = GetUniqueStateName(state.StateName, stateList);

            foreach (var item in this._ConnectionHost.Controls)
            {
                if (item is AudioConnection)
                {
                    AudioConnection ac = (AudioConnection)item;
                    if (ac.Start != null && ac.End != null)
                    {
                        ac.Transition.Destination = ((AudioStateNode)ac.End).State.Name;
                        ((AudioStateNode)ac.Start).Transitions.Add(ac.Transition);
                    }
                }
            }

            foreach (var state in stateList)
                state.Save();

            _Editor.Controller.States = new AudioState[stateList.Count];
            for (int i = 0; i < _Editor.Controller.States.Length; i++)
                _Editor.Controller.States[i] = stateList[i].State;

            if (defaultState != null)
                _Editor.Controller.DefaultState = (defaultState != null) ? defaultState.StateName : string.Empty;

            _Editor.Controller.Zoom = this._Panel.ZoomFactor;
            _Editor.Controller.PanX = this._Panel.PanPosition.x;
            _Editor.Controller.PanY = this._Panel.PanPosition.y;
        }

        private string GetUniqueStateName(string name, List<AudioStateNode> states)
        {
            int i = 1;
            string newName = name;
            while (states.Where(s => s.StateName == newName).Count() > 1)
                newName = name + i++;
            return newName;
        }
        public void Clear()
        {
            this._Panel.Controls.Clear();
            this._ConnectionHost.Controls.Clear();
        }
        public void RefreshStyles()
        {
            _Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;
        }

        public void BeginConnection(AudioStateNode node)
        {
            _ConnectionHost.BeginConnectionDetect(node);
        }

        private Vector2 _DeltaDrag;
        private bool _DragStarted;
        internal void ItemDrag(AudioStateNode node, Vector2 delta)
        {
            if (Selection.Contains(node))
            {
                if (!_DragStarted)
                {
                    foreach (var item in Selection)
                    {
                        if (item is AudioStateNode)
                        {
                            ((AudioStateNode)item).StartDrag.x = ((AudioStateNode)item).X;
                            ((AudioStateNode)item).StartDrag.y = ((AudioStateNode)item).Y;
                        }
                    }
                    _DeltaDrag = Vector3.zero;
                    _DragStarted = true;
                }
                _DeltaDrag += delta;
                delta = _DeltaDrag;

                foreach (var item in Selection)
                {
                    if (item is AudioStateNode)
                    {
                        ((AudioStateNode)item).X = ((AudioStateNode)item).StartDrag.x + delta.x;
                        ((AudioStateNode)item).Y = ((AudioStateNode)item).StartDrag.y + delta.y;
                    }
                }
            }
        }

        #region ContextMenu

        class GraphContextMenu : Skill.Editor.UI.ContextMenu
        {
            private AudioStateGraphEditor _Editor;
            public GraphContextMenu(AudioStateGraphEditor editor)
            {
                this._Editor = editor;

                Skill.Editor.UI.MenuItem munNew = new Skill.Editor.UI.MenuItem("New State");
                munNew.Click += munNew_Click;
                this.Add(munNew);
            }

            void munNew_Click(object sender, System.EventArgs e)
            {
                _Editor.New(this.Position);
            }
        }

        #endregion

        #region Enumerator
        public IEnumerator<Skill.Editor.UI.ISelectable> GetEnumerator()
        {
            foreach (var item in _Panel.Controls)
            {
                if (item is AudioStateNode)
                {
                    yield return (AudioStateNode)item;
                }
            }
            foreach (var item in _ConnectionHost.Controls)
            {
                if (item is DirectConnection)
                {
                    yield return (DirectConnection)item;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in _Panel.Controls)
            {
                if (item is AudioStateNode)
                {
                    yield return item;
                }
            }

            foreach (var item in _ConnectionHost.Controls)
            {
                if (item is DirectConnection)
                {
                    yield return item;
                }
            }
        }
        #endregion

        #region Preview

        private GameObject _PreviewObject = null;
        private float _EndPreviewTime;
        private AudioPreviewHandler _PreviewHandler;
        private void ValidatePreviewObject()
        {
            if (_PreviewObject == null)
            {
                System.Type[] types = new System.Type[] { typeof(AudioSource) };
                _PreviewObject = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("Preview", HideFlags.HideAndDontSave | HideFlags.HideInHierarchy, types);
            }
            _PreviewObject.transform.position = _Editor.Controller.transform.position;
            _PreviewObject.transform.rotation = _Editor.Controller.transform.rotation;
        }
        public void DestroyPreviewObject()
        {
            if (_PreviewObject != null)
            {
                StopPreview();
                GameObject.DestroyImmediate(_PreviewObject);
            }
            _PreviewObject = null;
        }
        public void StopPreview()
        {
            if (_PreviewHandler != null)
                _PreviewHandler.PreviewStopped();
            if (_PreviewObject != null)
                _PreviewObject.audio.Stop();

        }

        public void StartPreview(AudioPreviewHandler handler, AudioClip clip, float startTime, float endTime)
        {

            _PreviewHandler = handler;
            ValidatePreviewObject();
            StopPreview();
            if (_PreviewHandler != null)
            {
                if (clip != null)
                {
                    startTime = Mathf.Clamp(startTime, 0, clip.length);
                    endTime = Mathf.Clamp(endTime, 0, clip.length);
                    if (endTime > startTime)
                    {
                        _PreviewObject.audio.clip = clip;
                        _PreviewObject.audio.time = startTime;
                        _PreviewObject.audio.Play();
                        _EndPreviewTime = Time.realtimeSinceStartup + endTime - startTime;
                    }
                    _PreviewHandler.PreviewStarted();
                }
            }
        }

        public void UpdatePreview()
        {
            if (_EndPreviewTime > 0)
            {
                if (Time.realtimeSinceStartup > _EndPreviewTime)
                {
                    StopPreview();
                    _EndPreviewTime = -1;
                }
                else
                {
                    if (_PreviewHandler != null)
                        _PreviewHandler.UpdatePreview(_PreviewObject.audio.time);
                }
            }
        }

        #endregion


        private class AudioConnectionHost : DirectConnectionPanel
        {
            private AudioStateGraphEditor _Editor;

            public AudioConnectionHost(AudioStateGraphEditor editor)
            {
                _Editor = editor;
            }
            protected override IEnumerable<BaseControl> GetConnectableControls()
            {
                foreach (var item in _Editor._Panel.Controls)
                {
                    if (item is AudioStateNode)
                        yield return item;
                }
            }

            protected override bool CanConnect(BaseControl control1, BaseControl control2)
            {
                foreach (var c in Controls)
                {
                    if (c is DirectConnection)
                    {
                        DirectConnection dc = (DirectConnection)c;
                        if (dc.Start == control1 && dc.End == control2)
                            return false;
                    }
                }
                return true;
            }

            protected override DirectConnection CreateNewConnection(BaseControl start, BaseControl end)
            {
                return new AudioConnection(start, end);
            }

            private static Color _NextConnectionColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

            protected override void Render()
            {
                base.Render();

                foreach (var c in _Editor._Panel.Controls)
                {
                    if (c is AudioStateNode)
                    {
                        AudioStateNode node = (AudioStateNode)c;
                        if (node.NextState != null)
                        {
                            DirectConnection.DrawConnection(node.NextPoint, node.NextState.PrePoint, _NextConnectionColor, 16 * _Editor._Panel.ZoomFactor, _NextConnectionColor, Skill.Editor.Resources.UITextures.ArrowHead, 0);
                        }
                    }
                }
            }
        }

        private class AudioConnection : DirectConnection, Skill.Editor.UI.Extended.IProperties
        {
            public AudioTransition Transition { get; private set; }


            #region ExposeProperties

            [Skill.Framework.ExposeProperty(11, "FadeIn", "fade in time")]
            public float FadeIn { get { return Transition.FadeIn; } set { Transition.FadeIn = Mathf.Max(0, value); } }

            [Skill.Framework.ExposeProperty(12, "FadeOut", "fade out time")]
            public float FadeOut { get { return Transition.FadeOut; } set { Transition.FadeOut = Mathf.Max(0, value); } }

            [Skill.Framework.ExposeProperty(13, "CrossFade", "CrossFade?")]
            public bool CrossFade { get { return Transition.CrossFade; } set { Transition.CrossFade = value; } }

            [Skill.Framework.ExposeProperty(14, "Wait for BreakPoint", "wait for BreakPoint or immidiate break")]
            public bool WaitForBreakPoint { get { return Transition.WaitForBreakPoint; } set { Transition.WaitForBreakPoint = value; } }

            #endregion


            public AudioConnection(BaseControl start, BaseControl end)
                : this(start, end, new AudioTransition() { Destination = end.Name, Trigger = string.Empty })
            {
            }

            public AudioConnection(BaseControl start, BaseControl end, AudioTransition transition)
                : base(start, end)
            {
                this.Transition = transition;
                WantsMouseEvents = true;
                this.ContextMenu = AudioConnectionContextMenu.Instance;
            }

            protected override void OnMouseDown(MouseClickEventArgs args)
            {
                if (Start != null && args.Button == MouseButton.Left)
                {
                    AudioStateGraphEditor editor = Start.FindInParents<AudioStateGraphEditor>();
                    if (editor != null)
                    {
                        if (args.Ctrl)
                        {
                            if (this.IsSelected)
                                editor.Selection.Remove(this);
                            else
                                editor.Selection.Add(this);
                        }
                        else if (args.Shift)
                        {
                            if (!this.IsSelected)
                                editor.Selection.Add(this);
                        }
                        else
                        {
                            editor.Selection.Select(this);
                        }
                        args.Handled = true;
                    }
                }
                base.OnMouseDown(args);
            }

            public string Title { get { return "AudioConnection"; } }
            public bool IsSelectedProperties { get; set; }

            private AudioConnectionProperties _Properties;
            public Skill.Editor.UI.Extended.PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null) _Properties = new AudioConnectionProperties(this);
                    return _Properties;
                }
            }

            private AudioStateGraphEditor _Editor;
            protected override void Render()
            {
                if (_Editor == null)
                    _Editor = FindInParents<AudioStateGraphEditor>();
                ScaleFactor = _Editor._Panel.ZoomFactor;
                base.Render();
            }

            class AudioConnectionProperties : Skill.Editor.UI.Extended.ExposeProperties
            {
                private Skill.Editor.UI.IntPopup _Triggers;

                public AudioConnectionProperties(AudioConnection connection)
                    : base(connection)
                {
                }

                protected override void CreateCustomFileds()
                {
                    base.CreateCustomFileds();
                    _Triggers = new Skill.Editor.UI.IntPopup() { Margin = ControlMargin };
                    _Triggers.Label.text = "Trigger";
                    this.Controls.Add(_Triggers);
                    _Triggers.OptionChanged += _Triggers_OptionChanged;
                }


                void _Triggers_OptionChanged(object sender, System.EventArgs e)
                {
                    if (_Triggers.SelectedOption != null)
                        ((AudioConnection)Object).Transition.Trigger = _Triggers.SelectedOption.Content.text;
                    else
                        ((AudioConnection)Object).Transition.Trigger = string.Empty;
                }

                protected override void RefreshData()
                {
                    base.RefreshData();
                    string triggerName = ((AudioConnection)Object).Transition.Trigger;
                    _Triggers.Options.Clear();

                    AudioStateGraphEditor editor = ((AudioConnection)Object).FindInParents<AudioStateGraphEditor>();
                    if (editor._Editor.Controller.Triggers != null)
                    {

                        Skill.Editor.UI.PopupOption selectedOp = null;
                        for (int i = 0; i < editor._Editor.Controller.Triggers.Length; i++)
                        {
                            Skill.Editor.UI.PopupOption op = new Skill.Editor.UI.PopupOption(i, editor._Editor.Controller.Triggers[i]);
                            _Triggers.Options.Add(op);

                            if (editor._Editor.Controller.Triggers[i] == triggerName)
                                selectedOp = op;
                        }

                        _Triggers.SelectedOption = selectedOp;
                        if (selectedOp != null)
                            ((AudioConnection)Object).Transition.Trigger = selectedOp.Content.text;
                        else
                            ((AudioConnection)Object).Transition.Trigger = string.Empty;
                    }
                    else
                    {
                        ((AudioConnection)Object).Transition.Trigger = string.Empty;
                    }
                }
                protected override void SetDirty()
                {
                    UnityEditor.EditorUtility.SetDirty(((AudioConnection)Object).FindInParents<AudioStateGraphEditor>()._Editor.Controller);
                }
            }



            #region ContextMenu

            class AudioConnectionContextMenu : Skill.Editor.UI.ContextMenu
            {
                private static AudioConnectionContextMenu _Instance;
                Skill.Editor.UI.MenuItem _MnuBreak;
                public static AudioConnectionContextMenu Instance
                {
                    get
                    {
                        if (_Instance == null)
                            _Instance = new AudioConnectionContextMenu();
                        return _Instance;
                    }
                }

                private AudioConnectionContextMenu()
                {
                    _MnuBreak = new Skill.Editor.UI.MenuItem("Break");
                    _MnuBreak.Click += _MnuBreak_Click;
                    this.Add(_MnuBreak);
                }

                void _MnuBreak_Click(object sender, System.EventArgs e)
                {
                    AudioConnection connection = (AudioConnection)Owner;
                    AudioStateGraphEditor editor = Owner.FindInParents<AudioStateGraphEditor>();
                    editor.Break(connection);
                }
            }

            #endregion
        }
    }


    public interface AudioPreviewHandler
    {
        void PreviewStarted();
        void PreviewStopped();
        void UpdatePreview(float previewTime);
    }
}