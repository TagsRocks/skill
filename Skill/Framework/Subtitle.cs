using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
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
}
