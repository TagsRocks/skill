using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework
{
    public class Dictionary : MonoBehaviour
    {
        public DictionaryData[] Data;

        private Dictionary<string, TextKey> _TextsMap;        

        public bool IsLoaded { get; private set; }
        public void Reload()
        {
            if (_TextsMap != null)
                _TextsMap.Clear();
            _TextsMap = null;
            if (Data != null && Data.Length > 0)
            {

                int capacity = 0;
                for (int i = 0; i < Data.Length; i++)
                {
                    if (Data[i] != null)
                        capacity += Data[i].Keys.Length;
                }
                _TextsMap = new Dictionary<string, TextKey>(capacity);

                for (int i = 0; i < Data.Length; i++)
                {
                    var data = Data[i];
                    if (data != null)
                    {


                        foreach (var key in data.Keys)
                        {
#if DEBUG
                        if (_TextsMap.ContainsKey(key.Key))
                            Debug.LogWarning(string.Format("An element with the same key '{0}' already exists in the Dictionary", key.Key));
                        else
#endif
                            _TextsMap.Add(key.Key, key);
                        }
                    }
                }
            }
            else
            {
                _TextsMap = new Dictionary<string, TextKey>();
            }
            IsLoaded = true;
        }


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
    }

}


