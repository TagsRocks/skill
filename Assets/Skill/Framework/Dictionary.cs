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
    [CreateAssetMenu(fileName = "NewDictionary", menuName = "Skill/Dictionary", order = 46)]
    public class Dictionary : ScriptableObject
    {
        public Font Font;
        public int FontSize = 10;
        public Color FontColor = Color.white;
        public FontStyle FontStyle = FontStyle.Normal;
        public TextAlignment Alignment = TextAlignment.Center;

        [HideInInspector]
        public int LayoutType; // used by editor window

        [HideInInspector]
        [SerializeField]
        private TextKey[] _Keys;
        [HideInInspector]
        [SerializeField]
        private AudioClipSubtitle[] _Subtitles;

        public IEnumerable<TextKey> Keys { get { if (_Keys == null) _Keys = new TextKey[0]; return _Keys; } }
        public IEnumerable<AudioClipSubtitle> Subtitles { get { if (_Subtitles == null) _Subtitles = new AudioClipSubtitle[0]; return _Subtitles; } }

        public void SetKeys(TextKey[] keys)
        {
            if (keys == null) keys = new TextKey[0];
            this._Keys = keys;
            ReloadKeys();
        }

        public void SetSubtitles(AudioClipSubtitle[] subtitles)
        {
            if (subtitles == null) subtitles = new AudioClipSubtitle[0];
            this._Subtitles = subtitles;
            ReloadSubtitles();
        }


        private System.Collections.Generic.Dictionary<int, AudioClipSubtitle> _AudioMap;
        private System.Collections.Generic.Dictionary<string, TextKey> _TextsMap;

        void OnEnable()
        {
            ReloadKeys();
            ReloadSubtitles();
        }

        void OnDestroy()
        {
            if (_TextsMap != null)
                _TextsMap.Clear();
            if (_AudioMap != null)
                _AudioMap.Clear();
        }

        private void ReloadKeys()
        {
            if (_Keys == null) _Keys = new TextKey[0];

            if (_TextsMap == null)
                _TextsMap = new Dictionary<string, TextKey>(_Keys.Length);
            _TextsMap.Clear();

            foreach (var key in _Keys)
            {
#if DEBUG
                if (_TextsMap.ContainsKey(key.Key))
                    Debug.LogWarning(string.Format("An element with the same key '{0}' already exists in the Dictionary", key.Key));
                else
#endif
                    _TextsMap.Add(key.Key, key);
            }
        }
        private void ReloadSubtitles()
        {
            if (_Subtitles == null) _Subtitles = new AudioClipSubtitle[0];

            if (_AudioMap == null)
                _AudioMap = new System.Collections.Generic.Dictionary<int, AudioClipSubtitle>();
            _AudioMap.Clear();
            foreach (var item in _Subtitles)
            {
                if (item.Clip != null && item.Titles != null)
                    _AudioMap.Add(item.Clip.GetInstanceID(), item);
            }
        }

        public AudioClipSubtitle this[AudioClip clip] { get { return GetSubtitle(clip); } }
        public string this[string key] { get { return GetValue(key); } }
        public string GetValue(string key)
        {
            TextKey value;
            if (_TextsMap.TryGetValue(key, out value))
                return value.Value;
            return string.Empty;
        }
        public string GetComment(string key)
        {
            TextKey value;
            if (_TextsMap.TryGetValue(key, out value))
                return value.Comment;
            return string.Empty;
        }
        public AudioClipSubtitle GetSubtitle(AudioClip clip)
        {
            return GetSubtitle(clip.GetInstanceID());
        }
        public AudioClipSubtitle GetSubtitle(int instanceId)
        {
            AudioClipSubtitle at;
            if (_AudioMap == null)
                Debug.Log("_AudioMap is null");
            if (_AudioMap.TryGetValue(instanceId, out at))
                return at;

            return null;
        }
    }

}


