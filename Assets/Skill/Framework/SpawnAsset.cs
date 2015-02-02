﻿using System;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Defines serializable data asset required for Spawner
    /// </summary>
    [System.Serializable]
    public class SpawnAsset : ScriptableObject
    {
        /// <summary>
        /// Array of SpawnObjects
        /// </summary>
        public SpawnObject[] Objects;
    }
}
