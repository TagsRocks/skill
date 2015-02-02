using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.IO
{
    /// <summary>
    /// Handles touches and allow gesture detectors to access touches and use them
    /// </summary>
    public interface ITouchStateProvider
    {
        /// <summary> go throw free and ready for use touches </summary>
        /// <param name="phase">Phase of free touch</param>
        /// <param name="boundaryFrame">Boundary frame that touch must be within</param>
        /// <returns>Free touches</returns>
        IEnumerable<TouchState> GetFreeTouches(TouchPhase phase, Rect? boundaryFrame);
    }

}