﻿using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using System.Collections.Generic;
using Skill.Framework.Sequence;
using System;

namespace Skill.Editor.Sequence
{
    /// <summary>
    /// TrackBar to edit PropertyTrack
    /// </summary>
    public class EventTrackBar : BaseTrackBar
    {
        class EventInfo
        {
            public System.Reflection.Assembly Assembly;
            public string DisplayName;
            public Type Type;
            public string Path;
        }

        private List<EventKeyView> _Events;
        private EventTrack _EventTrack;

        public override bool IsContinuous { get { return false; } }

        /// <summary>
        /// Create a SoundTrackBar
        /// </summary>
        /// <param name="track"> SoundTrack to edit</param>
        public EventTrackBar(EventTrack track)
            : base(track)
        {
            _EventTrack = track;
            _Events = new List<EventKeyView>();
            CreateEvents();
            this.ContextMenu = EventTrackBarContextMenu.Instance;
            this.Height = 22;
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
            if (_EventTrack.EventKeys != null)
            {
                // search for new events in SoundTrack that we didn't create SoundEvent for them 
                foreach (var key in _EventTrack.EventKeys)
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
        private EventKeyView CreateEvent(EventKey key)
        {
            EventKeyView view = CreateNewEvent(key);
            view.ContextMenu = EventKeyViewContextMenu.Instance;
            this.Controls.Add(view);
            this._Events.Add(view);
            return view;
        }

        protected EventKeyView CreateNewEvent(EventKey key)
        {
            return new EventKeyView(this, key);
        }

        private bool IsEventExistWithKey(EventKey key)
        {
            foreach (var e in _Events)
                if (e.Key == key) return true;
            return false;
        }
        private bool IsKeyExistInEventTrack(ITrackKey key)
        {
            foreach (var k in _EventTrack.EventKeys)
            {
                if ((ITrackKey)k == key)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// add key at position
        /// </summary>
        /// <param name="x">position inside track</param>
        private void AddKeyAt(float x, EventKey newKey)
        {
            TimeLine timeLine = FindInParents<TimeLine>();
            if (timeLine != null)
            {
                // convert to local position of TimeBar - because of zooming
                x -= timeLine.View.ScrollPosition.x;
                newKey.FireTime = (float)timeLine.TimeBar.GetTime(x);

                EventKeyView e = CreateEvent(newKey);
                InspectorProperties.Select(e);
                RebuildTrackKeys();
            }
        }

        /// <summary>
        /// Delete SoundEvent 
        /// </summary>
        /// <param name="soundEvent">SoundEvent to delete</param>
        private void Delete(EventKeyView e)
        {
            if (_Events.Remove(e))
            {
                this.Controls.Remove(e);
                RebuildTrackKeys();
            }
        }

        /// <summary>
        /// Rebuild Keys pf SoundTrack
        /// </summary>
        private void RebuildTrackKeys()
        {
            var keys = new EventKey[_Events.Count];
            for (int i = 0; i < _Events.Count; i++)
                keys[i] = _Events[i].Key;
            _EventTrack.EventKeys = keys;
            _EventTrack.SortKeys();
            SetDirty();
        }

        #region EventKeyView

        /// <summary>
        /// Visual representation of EventKey
        /// </summary>
        public class EventKeyView : KeyView
        {
            public override double Duration { get { return 0.01f; } set { } }
            public override float MinWidth { get { return 10; } }
            public override float MaxWidth { get { return MinWidth; } }

            public EventKey Key { get; private set; }

            private string _Title;
            public override string Title
            {
                get
                {
                    if (_Title == null)
                        _Title = Key.GetType().Name;
                    return _Title;
                }
            }

            private Skill.Framework.UI.Image _ImgState;
            public EventKeyView(EventTrackBar trackBar, EventKey key)
                : base(trackBar)
            {
                this.Key = key;
                _ImgState = new Skill.Framework.UI.Image() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10, HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Center, VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center, Width = 10, Height = 10 };
                Controls.Add(_ImgState);
            }
            protected override void BeginRender()
            {
                _ImgState.Texture = Resources.UITextures.Event;
                base.BeginRender();
            }

            private EventKeyViewProperties _Properties;
            /// <summary> Properties </summary>
            public override PropertiesPanel Properties
            {
                get
                {
                    if (_Properties == null)
                        _Properties = new EventKeyViewProperties(this);
                    return _Properties;
                }
            }
            public override double FireTime { get { return Key.FireTime; } set { Key.FireTime = (float)value; Properties.Refresh(); } }
            public class EventKeyViewProperties : ExposeProperties
            {
                protected EventKeyView _View;
                public EventKeyViewProperties(EventKeyView view)
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

        #region EventKeyViewContextMenu
        class EventKeyViewContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static EventKeyViewContextMenu _Instance;
            public static EventKeyViewContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new EventKeyViewContextMenu();
                    return _Instance;
                }
            }

            public EventKeyViewContextMenu()
            {
                Skill.Editor.UI.MenuItem deleteItem = new Skill.Editor.UI.MenuItem("Delete");
                Add(deleteItem);

                deleteItem.Click += deleteItem_Click;

            }

            void deleteItem_Click(object sender, System.EventArgs e)
            {
                EventKeyView view = (EventKeyView)Owner;
                ((EventTrackBar)view.TrackBar).Delete(view);
            }
        }
        #endregion

        #region EventTrackBarContextMenu
        class EventTrackBarContextMenu : Skill.Editor.UI.ContextMenu
        {
            private static EventTrackBarContextMenu _Instance;
            public static EventTrackBarContextMenu Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new EventTrackBarContextMenu();
                    return _Instance;
                }
            }


            class MenuItemData
            {
                public Skill.Editor.UI.MenuItem Item;
                public string Path;
            }


            private EventTrackBarContextMenu()
            {
                // find all event class                
                List<EventInfo> events = new List<EventInfo>();

                Type baseType = typeof(EventKey);
                foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type objType in assembly.GetTypes())
                    {
                        if ((objType.IsPublic && ((objType.Attributes & System.Reflection.TypeAttributes.Abstract) != System.Reflection.TypeAttributes.Abstract))
                            && objType.IsSubclassOf(baseType))
                        {
                            object[] customEventAttributes = objType.GetCustomAttributes(typeof(CustomEventAttribute), true);
                            if (customEventAttributes != null && customEventAttributes.Length > 0)
                            {
                                CustomEventAttribute att = (CustomEventAttribute)customEventAttributes[0];

                                EventInfo info = new EventInfo();
                                info.DisplayName = att.DisplayName;
                                info.Path = string.IsNullOrEmpty(att.Path) ? "Custom" : att.Path.Trim(' ', '/', '\\', ',', '.').Replace('\\', '/');
                                info.Type = objType;
                                info.Assembly = assembly;

                                events.Add(info);
                            }
                        }
                    }
                }

                List<MenuItemData> itemList = new List<MenuItemData>();

                foreach (var e in events)
                {
                    Skill.Editor.UI.MenuItem pathItem = FindItem(itemList, e.Path);
                    if (pathItem == null)
                    {
                        string[] pathParts = e.Path.Split('/');
                        if (pathParts == null || pathParts.Length == 0) pathParts = new string[] { "Custom" };

                        string path = string.Empty;
                        Skill.Editor.UI.MenuItemBase parentItem = this;
                        for (int i = 0; i < pathParts.Length; i++)
                        {
                            path += pathParts[i];
                            pathItem = FindItem(itemList, path);

                            if (pathItem == null)
                            {
                                pathItem = new UI.MenuItem(pathParts[i]);
                                itemList.Add(new MenuItemData() { Path = path, Item = pathItem });
                                parentItem.Add(pathItem);
                            }

                            parentItem = pathItem;
                            path += "/";
                        }
                    }

                    if (pathItem != null)
                    {
                        Skill.Editor.UI.MenuItem addItem = new UI.MenuItem(e.DisplayName) { UserData = e };
                        addItem.Click += addItem_Click;
                        pathItem.Add(addItem);
                    }
                }

            }

            private UI.MenuItem FindItem(List<MenuItemData> items, string path)
            {
                foreach (var item in items)
                {
                    if (item.Path.Equals(path, StringComparison.OrdinalIgnoreCase))
                        return item.Item;
                }
                return null;
            }

            void addItem_Click(object sender, System.EventArgs e)
            {
                EventTrackBar trackBar = (EventTrackBar)Owner;
                EventInfo info = (EventInfo)((Skill.Editor.UI.MenuItem)sender).UserData;

                EventKey newKey = ScriptableObject.CreateInstance(info.Type) as EventKey;
                trackBar.AddKeyAt(Position.x, newKey);
            }
        }
        #endregion


    }
}


