using UnityEngine;
using System.Collections;
namespace Skill.Framework.Audio
{



    /// <summary>
    /// A Singleton Audio subtitle manager in scene
    /// </summary>
    public class AudioSubtitle : Skill.Framework.DynamicBehaviour
    {
        public static AudioSubtitle Instance { get; private set; }

        public Dictionary[] Dictionaries;
        public SubtitleRenderer SubRenderer;

        private Color _FontColor = Color.white;
        private FontStyle _FontStyle = FontStyle.Normal;
        private TextAlignment _Alignment = TextAlignment.Center;
        private System.Collections.Generic.Queue<SubTime> _Queue;


        /// <summary>
        /// Use Translations if not null - (this is a temporary solution for translation)
        /// </summary>
        public Dictionary[] Translations { get; set; }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            Instance = this;
            base.Awake();
            _Queue = new System.Collections.Generic.Queue<SubTime>(100);
            if (Dictionaries != null && Dictionaries.Length > 0)
            {
                foreach (var dictionary in Dictionaries)
                {
                    if (dictionary != null)
                    {
                        if (!dictionary.IsLoaded)
                            dictionary.Reload();
                    }
                }
            }
        }
        /// <summary>
        /// GetReferences
        /// </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            if (SubRenderer == null)
                SubRenderer = GetComponent<SubtitleRenderer>();
        }
        
        private AudioClipSubtitle GetSubtitle(AudioClip clip, out  Dictionary dictionary)
        {
            if (Dictionaries != null && Dictionaries.Length > 0)
            {
                int instanceID = clip.GetInstanceID();
                foreach (var d in Dictionaries)
                {
                    if (d != null)
                    {
                        if (!d.IsLoaded)
                            d.Reload();

                        AudioClipSubtitle subtitle = d.GetSubtitle(instanceID);
                        if (subtitle != null)
                        {
                            dictionary = d;
                            return subtitle;
                        }
                    }
                }
            }
            dictionary = null;
            return null;
        }

        private string GetText(string key)
        {
            if (Translations != null && Translations.Length > 0)
                return GetText(Translations, key);
            else
                return GetText(Dictionaries, key);
        }
        private string GetText(Dictionary[] dictionaries, string key)
        {
            if (dictionaries != null && dictionaries.Length > 0)
            {
                foreach (var dictionary in dictionaries)
                {
                    if (dictionary != null)
                    {
                        if (!dictionary.IsLoaded)
                            dictionary.Reload();

                        string text = dictionary.GetValue(key);
                        if (text != null) return text;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Start show subtitles of specified AudioClip
        /// </summary>
        /// <param name="clip">AudioClip to show it's subtitles (must be exists in Dictionaries)</param>
        public void Show(AudioClip clip)
        {
            if (clip != null)
            {
                _Queue.Clear();
                Dictionary dictionary = null;
                AudioClipSubtitle at = GetSubtitle(clip, out dictionary);
                if (at != null)
                {
                    this._FontColor = dictionary.FontColor;
                    this._FontStyle = dictionary.FontStyle;
                    this._Alignment = dictionary.Alignment;

                    if (at.Titles.Length > 0)
                    {
                        if (at.Titles.Length > 1)
                            Skill.Framework.Utility.QuickSort(at.Titles, SubtitleComparer.Instance);
                        for (int i = 0; i < at.Titles.Length; i++)
                        {
                            SubTime st = new SubTime();
                            st.Time = Time.time + at.Titles[i].Time;
                            st.Title = at.Titles[i];
                            _Queue.Enqueue(st);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Skill.Framework.Global.IsGamePaused) return;
            while (_Queue.Count > 0 && _Queue.Peek().Time <= Time.time)
            {
                SubTime st = _Queue.Dequeue();
                string valueText = GetText(st.Title.TitleKey);
                if (SubRenderer != null && valueText != null)
                {
                    if (!string.IsNullOrEmpty(valueText))
                    {
                        if (st.Title.OverrideStyle)
                            SubRenderer.ShowTitle(valueText, st.Title.Duration, st.Title.FontColor, st.Title.FontStyle, st.Title.Alignment);
                        else
                            SubRenderer.ShowTitle(valueText, st.Title.Duration, this._FontColor, this._FontStyle, this._Alignment);
                    }
                }
            }
            base.Update();
        }


        class SubTime
        {
            public float Time;
            public Subtitle Title;
        }
    }
}