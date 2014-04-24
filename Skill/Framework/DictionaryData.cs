using UnityEngine;
using System.Collections;

namespace Skill.Framework
{
    [System.Serializable]
    public class DictionaryData : ScriptableObject
    {
        [HideInInspector]
        public TextKey[] Keys;
    }


    [System.Serializable]
    public class TextKey
    {
        public string Key;
        public string Value;
        public string Comment;
    }
}