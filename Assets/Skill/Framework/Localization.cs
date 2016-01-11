using UnityEngine;
using System.Collections;

namespace Skill.Framework
{

    public class Localization : MonoBehaviour
    {
        [System.Serializable]
        public class Language
        {
            public string Name;
            public string[] Dictionaries; // address of Dictionaries in resources
            public Font[] Fonts; // Font
            public bool RightToLeft; // right to left
        }

        /// <summary>
        /// Set to valid index to load at startup
        /// </summary>
        public int DefaultLanguage = -1;
        public Language[] Languages;


        private Language _SelectedLanguage;
        private Dictionary[] _Dictionaries;


        public Language SelectedLanguage { get { return _SelectedLanguage; } }

        /// <summary> The only instance of Localization object </summary>
        public static Localization Instance { get; private set; }

        /// <summary> Awake </summary>
        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);

                if (DefaultLanguage >= 0 && DefaultLanguage < Languages.Length)
                    SelectLanguage(Languages[DefaultLanguage].Name);
            }

        }

        public bool IsRightToLeft
        {
            get
            {
                if (_SelectedLanguage != null)
                    return _SelectedLanguage.RightToLeft;
                return false;
            }
        }

        public void SelectLanguage(string languageName)
        {
            if (_SelectedLanguage != null)
            {
                if (languageName == _SelectedLanguage.Name)
                    return;
                if (_Dictionaries != null)
                {
                    for (int i = 0; i < _Dictionaries.Length; i++)
                        Resources.UnloadAsset(_Dictionaries[i]);
                    _Dictionaries = null;
                }
                _SelectedLanguage = null;
            }

            for (int i = 0; i < Languages.Length; i++)
            {
                if (Languages[i].Name == languageName)
                {
                    _SelectedLanguage = Languages[i];
                    break;
                }
            }

            if (_SelectedLanguage != null)
            {
                _Dictionaries = new Dictionary[_SelectedLanguage.Dictionaries.Length];
                for (int i = 0; i < _SelectedLanguage.Dictionaries.Length; i++)
                    _Dictionaries[i] = Resources.Load<Dictionary>(_SelectedLanguage.Dictionaries[i]);
            }
            else
            {
                Debug.LogWarning(string.Format("Language '{0}' not found", languageName));
            }
        }

        private Dictionary GetDictionary(int dictionaryIndex)
        {
            if (_SelectedLanguage != null && (_Dictionaries != null && _Dictionaries.Length > 0))
                return _Dictionaries[Mathf.Clamp(dictionaryIndex, 0, _Dictionaries.Length)];
            return null;
        }

        public string GetText(string key, int dictionaryIndex = 0)
        {
            Dictionary d = GetDictionary(dictionaryIndex);
            if (d != null)
                return d.GetValue(key);
            return key;
        }

        public Font GetFont(int fontIndex)
        {
            if (_SelectedLanguage != null && (_SelectedLanguage.Fonts != null && _SelectedLanguage.Fonts.Length > 0))
                return _SelectedLanguage.Fonts[Mathf.Clamp(fontIndex, 0, _SelectedLanguage.Fonts.Length)];
            return null;
        }

        public AudioClipSubtitle GetSubtitle(AudioClip clip, int dictionaryIndex = 0)
        {
            Dictionary d = GetDictionary(dictionaryIndex);
            if (d != null)
                return d.GetSubtitle(clip);
            return new AudioClipSubtitle() { Clip = clip };
        }

    }
}