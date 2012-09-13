using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.Managers
{
    //namespace Skill.Managers
    //{
    [AddComponentMenu("Skill/Managers/CacheBehavior")]
    public sealed class CacheBehavior : MonoBehaviour
    {
        public int CacheId { get; set; }
        public bool IsCollected { get; set; }
        public CacheGroup Group { get; set; }

        void Awake()
        {
            enabled = false;
        }

        void Update()
        {
            enabled = false;
        }
    }
    //}

}