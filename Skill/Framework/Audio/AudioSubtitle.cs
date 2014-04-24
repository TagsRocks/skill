using UnityEngine;
using System.Collections;
namespace Skill.Framework.Audio
{
    /// <summary>
    /// Text subtitle to draw at bottom of screen when playing audios
    /// </summary>
    [System.Serializable]
    public class Subtitle
    {
        /// <summary> Time to show subtitle </summary>
        [SerializeField]
        public float Time;

        /// <summary> Duration of subtitle </summary>
        [SerializeField]
        public float Duration;

        /// <summary> Key in DictionaryData </summary>
        [SerializeField]
        public string TitleKey;

        /// <summary> Override global AudioSubtitle style</summary>
        [SerializeField]
        public bool OverrideStyle;

        /// <summary> Font Color </summary>
        [SerializeField]
        public Color FontColor;

        /// <summary> Font Style </summary>
        [SerializeField]
        public FontStyle FontStyle;

        /// <summary> Text Alignment </summary>
        [SerializeField]
        public TextAlignment Alignment;
    }

    class SubtitleComparer : System.Collections.Generic.IComparer<Subtitle>
    {
        private static SubtitleComparer _Instance;
        public static SubtitleComparer Instance
        {
            get
            {
                if (_Instance == null) _Instance = new SubtitleComparer();
                return _Instance;
            }
        }

        public int Compare(Subtitle x, Subtitle y)
        {
            return x.Time.CompareTo(y.Time);
        }
    }


    /// <summary>
    /// Aollection of subtitles for an AudioClip
    /// </summary>
    [System.Serializable]
    public class AudioClipSubtitle
    {
        [SerializeField]
        public AudioClip Clip;

        [SerializeField]
        public Subtitle[] Titles;
    }


    /// <summary>
    /// A Singleton Audio subtitle manager in scene
    /// </summary>
    public class AudioSubtitle : Skill.Framework.DynamicBehaviour
    {
        public static AudioSubtitle Instance { get; private set; }

        public Dictionary Dictionary;
        public SubtitleRenderer SubRenderer;
        public Color FontColor = Color.white;
        public FontStyle FontStyle = FontStyle.Normal;
        public TextAlignment Alignment = TextAlignment.Center;
        public Font EditorFont;
        public int EditorFontSize = 10;


        [HideInInspector]
        public AudioClipSubtitle[] Subtitles;

        private System.Collections.Generic.Dictionary<int, AudioClipSubtitle> _AudioMap;
        private System.Collections.Generic.Queue<SubTime> _Queue;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
            _AudioMap = new System.Collections.Generic.Dictionary<int, AudioClipSubtitle>();
            _Queue = new System.Collections.Generic.Queue<SubTime>(100);
            if (this.Dictionary != null)
            {
                if (!this.Dictionary.IsLoaded)
                    this.Dictionary.Reload();
            }
            Reload();
        }
        protected override void GetReferences()
        {
            base.GetReferences();
            if (SubRenderer == null)
                SubRenderer = GetComponent<SubtitleRenderer>();
        }

        private void Reload()
        {
            _AudioMap.Clear();
            if (Subtitles != null)
            {
                foreach (var item in Subtitles)
                {
                    if (item.Clip != null && item.Titles != null)
                        _AudioMap.Add(item.Clip.GetInstanceID(), item);
                }
            }
        }
        public void Show(AudioClip clip)
        {
            if (clip != null)
            {
                _Queue.Clear();
                int id = clip.GetInstanceID();
                AudioClipSubtitle at;
                if (_AudioMap.TryGetValue(id, out at))
                {
                    if (at.Titles.Length > 0)
                    {
                        if (at.Titles.Length > 1)
                            Skill.Framework.MathHelper.QuickSort(at.Titles, SubtitleComparer.Instance);
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

        protected override void Update()
        {
            if (Skill.Framework.Global.IsGamePaused) return;
            while (_Queue.Count > 0 && _Queue.Peek().Time <= Time.time)
            {
                SubTime st = _Queue.Dequeue();
                if (SubRenderer != null && Dictionary != null)
                {
                    string valueText = Dictionary[st.Title.TitleKey];
                    if (!string.IsNullOrEmpty(valueText))
                    {
                        if (st.Title.OverrideStyle)
                            SubRenderer.ShowTitle(valueText, st.Title.Duration, st.Title.FontColor, st.Title.FontStyle, st.Title.Alignment);
                        else
                            SubRenderer.ShowTitle(valueText, st.Title.Duration, this.FontColor, this.FontStyle, this.Alignment);
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