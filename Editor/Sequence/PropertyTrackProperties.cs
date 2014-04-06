using UnityEngine;
using System.Collections;
using System.Reflection;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    #region PropertyTrackProperties
    public class PropertyTrackProperties<V> : TrackTreeViewItemPropertiesBase
    {

        private Skill.Editor.UI.ObjectField<GameObject> _Object;
        private Skill.Editor.UI.Popup _Component;
        private Skill.Editor.UI.Popup _Property;
        private Skill.Editor.UI.Button _BtnRefresh;

        public PropertyTrackProperties(TrackTreeViewItem item)
            : base(item)
        {

        }

        protected override void CreateCustomFileds()
        {
            base.CreateCustomFileds();

            Skill.Framework.UI.Thickness margin = new Skill.Framework.UI.Thickness(2);

            _Object = new Skill.Editor.UI.ObjectField<GameObject>() { Margin = margin }; _Object.Label.text = "Game Object";
            _Component = new Skill.Editor.UI.Popup() { Margin = margin }; _Component.Label.text = "Component";
            _Property = new Skill.Editor.UI.Popup() { Margin = margin }; _Property.Label.text = "Property";
            _BtnRefresh = new Skill.Editor.UI.Button() { Margin = margin, Width = 80, HorizontalAlignment = Framework.UI.HorizontalAlignment.Right }; _BtnRefresh.Content.text = "Refresh";

            Controls.Add(_Object);
            Controls.Add(_BtnRefresh);
            Controls.Add(_Component);
            Controls.Add(_Property);

            _Object.ObjectChanged += _Object_ObjectChanged;
            _Component.OptionChanged += _Component_OptionChanged;
            _Property.OptionChanged += _Property_OptionChanged;
            _BtnRefresh.Click += _BtnRefresh_Click;
        }

        void _BtnRefresh_Click(object sender, System.EventArgs e)
        {
            RebuildComponent();
        }

        private PropertyTrack<V> GetTrack() { return (PropertyTrack<V>)base.Item.Track; }

        void _Object_ObjectChanged(object sender, System.EventArgs e)
        {
            if (IgnoreChanges) return;
            PropertyTrack<V> track = GetTrack();
            track.Object = _Object.Object;
            track.Invalidate();
            RebuildComponent();
            SetDirty();
        }
        void _Component_OptionChanged(object sender, System.EventArgs e)
        {
            if (IgnoreChanges) return;
            PropertyTrack<V> track = GetTrack();
            if (_Component.SelectedOption != null)
                track.Component = (Component)_Component.SelectedOption.UserData;
            else
                track.Component = null;
            track.Invalidate();
            RebuildProperties();
            SetDirty();
        }
        void _Property_OptionChanged(object sender, System.EventArgs e)
        {
            if (IgnoreChanges) return;
            PropertyTrack<V> track = GetTrack();
            if (_Property.SelectedOption != null)
            {
                if (track.PropertyName != (string)_Property.SelectedOption.UserData)
                {
                    track.PropertyName = (string)_Property.SelectedOption.UserData;
                    track.Invalidate();
                    RefreshDefaultValue();
                }
            }
            else
                track.PropertyName = null;
            SetDirty();
        }

        private void RefreshDefaultValue()
        {            
            PropertyTrack<V> track = GetTrack();
            if (!string.IsNullOrEmpty(track.PropertyName) && track.Component != null)
            {
                PropertyInfo[] infos = track.Component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var info in infos)
                {
                    if (info.Name == track.PropertyName)
                    {
                        if (info.PropertyType == track.PropertyType && info.CanWrite)
                        {                            
                            track.DefaultValue = (V)info.GetGetMethod().Invoke(track.Component, null);
                            base.Refresh();
                        }
                        break;
                    }
                }
            }
        }



        private void RebuildComponent()
        {
            _Component.Options.Clear();

            int selectedIndex = -1;
            if (_Object.Object != null)
            {
                Component[] components = _Object.Object.GetComponents<Component>();
                if (components != null)
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        Component c = components[i];
                        Skill.Editor.UI.PopupOption option = new Skill.Editor.UI.PopupOption(i, GetComponentName(c)) { UserData = c };
                        _Component.Options.Add(option);
                        if (((PropertyTrack<V>)base.Item.Track).Component == c)
                            selectedIndex = i;
                    }
                }
            }
            else
            {
                ((PropertyTrack<V>)base.Item.Track).Component = null;
            }

            _Component.SelectedIndex = selectedIndex;
            if (_Component.SelectedOption != null)
                ((PropertyTrack<V>)base.Item.Track).Component = (Component)_Component.SelectedOption.UserData;
            else
                ((PropertyTrack<V>)base.Item.Track).Component = null;
            RebuildProperties();
        }

        private string GetComponentName(Component c)
        {
            string fullName = c.GetType().Name;
            return fullName;
        }

        private void RebuildProperties()
        {
            _Property.Options.Clear();
            int selectedIndex = -1;

            if (_Component.SelectedOption != null)
            {
                Component c = (Component)_Component.SelectedOption.UserData;
                PropertyInfo[] infos = c.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                int index = 0;
                if (infos != null)
                {
                    for (int i = 0; i < infos.Length; i++)
                    {
                        PropertyInfo info = infos[i];
                        if (info.PropertyType == ((PropertyTrack<V>)base.Item.Track).PropertyType && info.CanWrite)
                        {
                            Skill.Editor.UI.PopupOption option = new Skill.Editor.UI.PopupOption(index, info.Name) { UserData = info.Name };
                            _Property.Options.Add(option);

                            if (((PropertyTrack<V>)base.Item.Track).PropertyName == info.Name)
                                selectedIndex = index;
                            index++;
                        }
                    }
                }
            }
            else
            {
                ((PropertyTrack<V>)base.Item.Track).PropertyName = null;
            }

            _Property.SelectedIndex = selectedIndex;
            if (_Property.SelectedOption != null)
                ((PropertyTrack<V>)base.Item.Track).PropertyName = (string)_Property.SelectedOption.UserData;
            else
                ((PropertyTrack<V>)base.Item.Track).PropertyName = null;
        }



        protected override void RefreshData()
        {
            base.RefreshData();
            _Object.Object = ((PropertyTrack<V>)base.Item.Track).Object;
            RebuildComponent();
        }
    }
    #endregion

}