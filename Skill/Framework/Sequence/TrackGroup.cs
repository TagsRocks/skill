using UnityEngine;
using System.Collections;
namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Group of tracks in Matinee
    /// </summary>
    public class TrackGroup : Skill.Framework.StaticBehaviour
    {
        #region Editor variables
        [HideInInspector]
        public bool IsOpen = true; // state of FoldOut        
        #endregion
    }
}