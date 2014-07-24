using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using System.Collections.Generic;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public abstract class DiscreteTrackBar<V> : PropertyTrackBar<V>
    {
        private List<DiscreteKeyView> _Events;
        private DiscreteTrack<V> _DiscreteTrack;

        public override bool IsContinuous { get { return false; } }

        /// <summary>
        /// Create a DiscreteTrackBar
        /// </summary>
        /// <param name="track"> DiscreteTrack to edit</param>
        public DiscreteTrackBar(DiscreteTrack<V> track)
            : base(track)
        {
            _DiscreteTrack = track;
            _Events = new List<DiscreteKeyView>();
            CreateEvents();
            this.ContextMenu = DiscreteTrackBarContextMenu.Instance;
        }


        /// <summary>
        /// Refresh data do to changes outside of MatineeEditor
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            CreateEvents();
        }
        private void CreateEvents()
        {
            if (_DiscreteTrack.PropertyKeys != null)
            {
                // search for new events in DiscreteTrack that we didn't create DiscreteEvent for them 
                foreach (var key in _DiscreteTrack.PropertyKeys)
                {
                    if (key != null)
                    {
                        if (!IsEventExistWithKey(key))
                            CreateEvent(key);
                    }
                }

                // search for removed keys in SoundTrack that we did create SoundEvent for them 
                int index = 0;
                while (index < _Events.Count)
                {
                    var e = _Events[index];
                    if (!IsKeyExistInDiscreteTrack(e.Key))
                    {
                        _Events.Remove(e);
                        Controls.Remove(e);
                        continue;
                    }
                    index++;
                }
            }
            else
            {
                _Events.Clear();
                Controls.Clear();
            }
        }

        // create a SoundEvent and initialize it
        private DiscreteKeyView CreateEvent(IPropertyKey<V> key)
        {
            DiscreteKeyView se = CreateNewEvent(key);
            se.ContextMenu = DiscreteKeyContextMenu.Instance;
            this.Controls.Add(se);
            this._Events.Add(se);
            return se;
        }

        protected abstract DiscreteKeyView CreateNewEvent(IPropertyKey<V> key);
        protected abstract IPropertyKey<V> CreateNewKey();
        protected abstract IPropertyKey<V>[] CreateKeyArray(int arraySize);

        private void EvaluateNewKey(IPropertyKey<V> newKey, IPropertyKey<V> previousKey)
        {
            if (previousKey != null)
                newKey.ValueKey = previousKey.ValueKey;
            else
                newKey.ValueKey = ((PropertyTrack<V>)Track).DefaultValue;
        }

        private bool IsEventExistWithKey(IPropertyKey<V> key)
        {
            foreach (var e in _Events)
                if (e.Key == key) return true;
            return false;
        }
        private bool IsKeyExistInDiscreteTrack(IPropertyKey<V> key)
        {
            foreach (var k in _DiscreteTrack.PropertyKeys)
            {
                if (k == key)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// add key at position
        /// </summary>
        /// <param name="x">position inside track</param>
        private void AddKeyAtPosition(float x)
        {
            TimeLine timeLine = FindInParents<TimeLine>();
            if (timeLine != null)
            {
                // convert to local position of TimeBar - because of zooming
                x -= timeLine.View.ScrollPosition.x;
                var e = CreateKey((float)timeLine.TimeBar.GetTime(x), _DiscreteTrack.DefaultValue);
                InspectorProperties.Select(e);
            }
        }

        private DiscreteKeyView CreateKey(float time, V value)
        {
            IPropertyKey<V> newKey = CreateNewKey();
            newKey.FireTime = time;
            newKey.ValueKey = value;

            DiscreteKeyView e = CreateEvent(newKey);
            RebuildTrackKeys();

            for (int i = 0; i < _DiscreteTrack.PropertyKeys.Length; i++)
            {
                if (_DiscreteTrack.PropertyKeys[i] == newKey)
                {
                    if (i > 0)
                        EvaluateNewKey(newKey, _DiscreteTrack.PropertyKeys[i - 1]);
                    else
                        EvaluateNewKey(newKey, null);
                    break;
                }
            }
            return e;
        }

        private void AddKey(float time)
        {
            object v = _DiscreteTrack.GetValue();
            V sceneValue;
            if (v != null)
                sceneValue = (V)v;
            else
                sceneValue = _DiscreteTrack.DefaultValue;

            double closestTime = 0.005f;
            int closestIndex = -1;
            for (int i = 0; i < _Events.Count; i++)
            {
                double deltaTime = System.Math.Abs(_Events[i].FireTime - time);
                if (deltaTime < closestTime)
                {
                    closestTime = deltaTime;
                    closestIndex = i;
                }
                else if (closestIndex >= 0) // getting far from time
                    break;
            }

            if (closestIndex >= 0)
                _Events[closestIndex].Key.ValueKey = sceneValue;
            else
                CreateKey(time, sceneValue);

            SetDirty();
        }

        public override void AddKey()
        {
            TimeLine timeLine = FindInParents<TimeLine>();
            if (timeLine != null)
            {
                float time = (float)timeLine.TimePosition;
                AddKey(time);
            }
        }

        private bool _Recording;
        private V _SaveState;
        public override void SaveRecordState()
        {
            object value = _DiscreteTrack.GetValue();
            if (value != null)
            {
                _SaveState = (V)value;
                _Recording = true;
            }
            else
                _Recording = false;
        }
        public override void AutoKey()
        {
            if (_Recording)
            {
                object value = _DiscreteTrack.GetValue();
                if (value != null)
                {
                    V newState = (V)value;
                    if (!IsEqual(newState, _SaveState))
                        AddKey();
                }
            }
        }

        protected abstract bool IsEqual(V v1, V v2);


        /// <summary>
        /// Delete SoundEvent 
        /// </summary>
        /// <param name="soundEvent">SoundEvent to delete</param>
        public override void Delete(KeyView keyView)
        {
            DiscreteKeyView dv = keyView as DiscreteKeyView;
            if (dv != null)
            {
                if (_Events.Remove(dv))
                {
                    this.Controls.Remove(dv);
                    if (InspectorProperties.IsSelected(dv))
                        InspectorProperties.Select(null);
                    RebuildTrackKeys();
                }
            }
        }
        private void Update(DiscreteKeyView keyView)
        {
            AddKey((float)keyView.FireTime);
        }

        /// <summary>
        /// Rebuild Keys pf SoundTrack
        /// </summary>
        private void RebuildTrackKeys()
        {
            var keys = CreateKeyArray(_Events.Count);
            for (int i = 0; i < _Events.Count; i++)
                keys[i] = _Events[i].Key;
            _DiscreteTrack.PropertyKeys = keys;
            _DiscreteTrack.SortKeys();
            SetDirty();
        }

        #region DiscreteKeyView

        /// <summary>
        /// Visual representation of SoundKey
        /// </summary>
        protected abstract class DiscreteKeyView : KeyView
        {
            public override double Duration { get { return 0.1f; } set { } }
            public override float MinWidth { get { return 10; } }
            public override float MaxWidth { get { return 10; } }

            public override float CenterOffset { get { return 5; } }

            private DiscreteKeyViewProperties _Properties;
            /// <summary> Properties </summary>
            public override PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new DiscreteKeyViewProperties(this);
                    return _Properties;
                }
            }

            public IPropertyKey<V> Key { get; private set; }
            public override double FireTime { get { return Key.FireTime; } set { Key.FireTime = (float)value; Properties.Refresh(); } }

            private static Color _IconColor;

            static DiscreteKeyView()
            {
                if (UnityEditor.EditorGUIUtility.isProSkin)
                    _IconColor = Color.white;
                else
                    _IconColor = Color.black;
            }

            public DiscreteKeyView(DiscreteTrackBar<V> trackBar, IPropertyKey<V> key)
                : base(trackBar)
            {
                this.Key = key;
                _ImgIcon = new Skill.Framework.UI.Image() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10, HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Center, VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center, Width = 10, Height = 10 };
                Controls.Add(_ImgIcon);
            }
            protected override void BeginRender()
            {
                base.BeginRender();
                _ImgIcon.TintColor = GetIconColor();
                _ImgIcon.Texture = GetIcon();
            }


            private Skill.Framework.UI.Image _ImgIcon;
            protected virtual Texture GetIcon() { return Resources.UITextures.Keyframe; }
            protected virtual Color GetIconColor() { return _IconColor; }

            public class DiscreteKeyViewProperties : ExposeProperties
            {
                protected DiscreteKeyView _View;
                public DiscreteKeyViewProperties(DiscreteKeyView view)
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
        #endregion

        #region DiscreteKeyContextMenu
        class DiscreteKeyContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static DiscreteKeyContextMenu _Instance;
            public static DiscreteKeyContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new DiscreteKeyContextMenu();
                    return _Instance;
                }
            }

            private Skill.Editor.UI.MenuItem _DeleteItem;
            private Skill.Editor.UI.MenuItem _Update;

            public DiscreteKeyContextMenu()
            {
                _DeleteItem = new Skill.Editor.UI.MenuItem("Delete");
                _Update = new Skill.Editor.UI.MenuItem("Update");
                Add(_DeleteItem);
                AddSeparator();
                Add(_Update);
                _DeleteItem.Click += _DeleteItem_Click;
                _Update.Click += _Update_Click;
            }

            void _Update_Click(object sender, System.EventArgs e)
            {
                DiscreteKeyView se = (DiscreteKeyView)Owner;
                ((DiscreteTrackBar<V>)se.TrackBar).Update(se);
            }

            void _DeleteItem_Click(object sender, System.EventArgs e)
            {
                DiscreteKeyView se = (DiscreteKeyView)Owner;
                ((DiscreteTrackBar<V>)se.TrackBar).Delete(se);
            }
        }
        #endregion

        #region DiscreteTrackBarContextMenu
        class DiscreteTrackBarContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static DiscreteTrackBarContextMenu _Instance;
            public static DiscreteTrackBarContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new DiscreteTrackBarContextMenu();
                    return _Instance;
                }
            }

            private Skill.Editor.UI.MenuItem _AddKeyItem;
            private DiscreteTrackBarContextMenu()
            {
                _AddKeyItem = new Skill.Editor.UI.MenuItem("Add key");

                Add(_AddKeyItem);
                _AddKeyItem.Click += _AddKeyItem_Click;
            }

            void _AddKeyItem_Click(object sender, System.EventArgs e)
            {
                DiscreteTrackBar<V> trackBar = (DiscreteTrackBar<V>)Owner;
                trackBar.AddKeyAtPosition(Position.x);
            }
        }
        #endregion
    }
}
