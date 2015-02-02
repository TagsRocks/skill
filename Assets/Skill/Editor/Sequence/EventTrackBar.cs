using UnityEngine;
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
    public class EventTrackBar : EventOrientedTrackBar
    {
        class EventInfo
        {
            public System.Reflection.Assembly Assembly;
            public string DisplayName;
            public Type Type;
            public string Path;
        }

        //private EventTrack _EventTrack;

        /// <summary>
        /// Create a SoundTrackBar
        /// </summary>
        /// <param name="track"> SoundTrack to edit</param>
        public EventTrackBar(EventTrack track)
            : base(track)
        {
            //_EventTrack = track;
            this.ContextMenu = EventTrackBarContextMenu.Instance;
        }


        protected override EventOrientedKeyView CreateNewEvent(EventOrientedKey key)
        {
            return new EventKeyView(this, (EventKey)key);
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

                EventOrientedKeyView e = CreateEvent(newKey);
                InspectorProperties.Select(e);
                RebuildTrackKeys();
            }
        }

        #region EventKeyView

        /// <summary>
        /// Visual representation of EventKey
        /// </summary>
        protected class EventKeyView : EventOrientedKeyView
        {
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
            private Skill.Framework.UI.Box _Bg;

            //private EventKey _EventKey;

            public EventKeyView(EventTrackBar trackBar, EventKey key)
                : base(trackBar, key)
            {
                //_EventKey = key;

                this.ColumnDefinitions.Add(10, Framework.UI.GridUnitType.Pixel);
                this.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);


                _Bg = new Framework.UI.Box();
                _ImgState = new Skill.Framework.UI.Image() { Row = 0, Column = 0 };

                Controls.Add(_Bg);
                Controls.Add(_ImgState);
            }
            protected override void BeginRender()
            {
                _ImgState.Texture = Resources.UITextures.Event;
                base.BeginRender();
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


