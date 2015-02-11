using UnityEngine;
using System.Collections;
using Skill.Editor.UI;
using System.Collections.Generic;
using Skill.Framework.Sequence;
using System;

namespace Skill.Editor.Sequence
{
    /// <summary>
    /// TrackBar to edit PropertyTrack
    /// </summary>
    public abstract class EventOrientedTrackBar : BaseTrackBar
    {

        private List<EventOrientedKeyView> _Events;
        private EventOrientedTrack _EventOrientedTrack;

        public override bool IsContinuous { get { return false; } }

        /// <summary>
        /// Create a SoundTrackBar
        /// </summary>
        /// <param name="track"> SoundTrack to edit</param>
        public EventOrientedTrackBar(EventOrientedTrack track)
            : base(track)
        {
            _EventOrientedTrack = track;
            _Events = new List<EventOrientedKeyView>();
            CreateEvents();            
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
            if (_EventOrientedTrack.Keys != null)
            {
                // search for new events in SoundTrack that we didn't create SoundEvent for them 
                foreach (var key in _EventOrientedTrack.Keys)
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
                    if (!IsKeyExistInEventTrack(e.Key))
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
        protected EventOrientedKeyView CreateEvent(EventOrientedKey key)
        {
            EventOrientedKeyView view = CreateNewEvent(key);
            view.ContextMenu = OrientedKeyViewContextMenu.Instance;
            this.Controls.Add(view);
            this._Events.Add(view);
            return view;
        }

        protected abstract EventOrientedKeyView CreateNewEvent(EventOrientedKey key);

        private bool IsEventExistWithKey(EventOrientedKey key)
        {
            foreach (var e in _Events)
                if (e.Key == key) return true;
            return false;
        }
        private bool IsKeyExistInEventTrack(ITrackKey key)
        {
            foreach (var k in _EventOrientedTrack.Keys)
            {
                if ((ITrackKey)k == key)
                    return true;
            }
            return false;
        }

        public override void AddKey() { }

        /// <summary>
        /// Delete SoundEvent 
        /// </summary>
        /// <param name="soundEvent">SoundEvent to delete</param>
        public override void Delete(KeyView keyView)
        {
            EventOrientedKeyView ev = keyView as EventOrientedKeyView;
            if (ev != null)
            {
                if (_Events.Remove(ev))
                {
                    if (InspectorProperties.IsSelected(ev))
                        InspectorProperties.Select(null);
                    this.Controls.Remove(keyView);
                    RebuildTrackKeys();
                }
            }
        }

        /// <summary>
        /// Rebuild Keys pf SoundTrack
        /// </summary>
        protected void RebuildTrackKeys()
        {
            var keys = new EventOrientedKey[_Events.Count];
            for (int i = 0; i < _Events.Count; i++)
                keys[i] = _Events[i].Key;
            _EventOrientedTrack.Keys = keys;
            _EventOrientedTrack.SortKeys();
            SetDirty();
        }

        #region OrientedKeyView

        /// <summary>
        /// Visual representation of EventKey
        /// </summary>
        protected abstract class EventOrientedKeyView : KeyView
        {
            public override double Duration { get { return Key.Length; } set { } }
            public override float MinWidth { get { return 10; } }
            public EventOrientedKey Key { get; private set; }

            public EventOrientedKeyView(EventOrientedTrackBar trackBar, EventOrientedKey key)
                : base(trackBar)
            {
                this.Key = key;
            }

            private OrientedKeyViewProperties _Properties;
            /// <summary> Properties </summary>
            public override PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new OrientedKeyViewProperties(this);
                    return _Properties;
                }
            }
            public override double FireTime { get { return Key.FireTime; } set { Key.FireTime = (float)value; Properties.Refresh(); } }
            public class OrientedKeyViewProperties : ExposeProperties
            {
                protected EventOrientedKeyView _View;
                public OrientedKeyViewProperties(EventOrientedKeyView view)
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

        #region OrientedKeyViewContextMenu
        protected class OrientedKeyViewContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static OrientedKeyViewContextMenu _Instance;
            public static OrientedKeyViewContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new OrientedKeyViewContextMenu();
                    return _Instance;
                }
            }

            public OrientedKeyViewContextMenu()
            {
                Skill.Editor.UI.MenuItem deleteItem = new Skill.Editor.UI.MenuItem("Delete");
                Add(deleteItem);

                deleteItem.Click += deleteItem_Click;

            }

            void deleteItem_Click(object sender, System.EventArgs e)
            {
                EventOrientedKeyView view = (EventOrientedKeyView)Owner;
                ((EventOrientedTrackBar)view.TrackBar).Delete(view);
            }
        }
        #endregion

    }
}
