using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework
{
    #region TextKey
    [System.Serializable]
    public class TextKey
    {
        public string Key;
        public string Value;
        public string Comment;
    }
    #endregion

    [System.Serializable]
    public class Dictionary : ScriptableObject
    {
        public Font Font;
        public int FontSize = 10;
        public Color FontColor = Color.white;
        public FontStyle FontStyle = FontStyle.Normal;
        public TextAlignment Alignment = TextAlignment.Center;

        [HideInInspector]
        public TextKey[] Keys;
        [HideInInspector]
        public AudioClipSubtitle[] Subtitles;               


        private System.Collections.Generic.Dictionary<int, AudioClipSubtitle> _AudioMap;
        private System.Collections.Generic.Dictionary<string, TextKey> _TextsMap;

        public bool IsLoaded { get; private set; }
        public void Reload()
        {
            if (_TextsMap != null)
                _TextsMap.Clear();
            _TextsMap = null;
            if (Keys != null && Keys.Length > 0)
            {
                _TextsMap = new Dictionary<string, TextKey>(Keys.Length);
                foreach (var key in Keys)
                {
#if DEBUG
                        if (_TextsMap.ContainsKey(key.Key))
                            Debug.LogWarning(string.Format("An element with the same key '{0}' already exists in the Dictionary", key.Key));
                        else
#endif
                    _TextsMap.Add(key.Key, key);
                }
            }
            else
            {
                _TextsMap = new Dictionary<string, TextKey>();
            }

            _AudioMap = new System.Collections.Generic.Dictionary<int, AudioClipSubtitle>();
            if (Subtitles != null)
            {
                foreach (var item in Subtitles)
                {
                    if (item.Clip != null && item.Titles != null)
                        _AudioMap.Add(item.Clip.GetInstanceID(), item);
                }
            }
            IsLoaded = true;
        }

        public AudioClipSubtitle this[AudioClip clip] { get { return GetSubtitle(clip); } }
        public string this[string key] { get { return GetValue(key); } }
        public string GetValue(string key)
        {
            if (IsLoaded)
            {
                TextKey value;
                if (_TextsMap.TryGetValue(key, out value))
                    return value.Value;
            }
            return string.Empty;
        }
        public string GetComment(string key)
        {
            if (IsLoaded)
            {
                TextKey value;
                if (_TextsMap.TryGetValue(key, out value))
                    return value.Comment;
            }
            return string.Empty;
        }
        public AudioClipSubtitle GetSubtitle(AudioClip clip)
        {
            return GetSubtitle(clip.GetInstanceID());
        }
        public AudioClipSubtitle GetSubtitle(int instanceId)
        {
            if (IsLoaded)
            {
                AudioClipSubtitle at;
                if (_AudioMap.TryGetValue(instanceId, out at))
                    return at;
            }
            return null;
        }
    }

}


