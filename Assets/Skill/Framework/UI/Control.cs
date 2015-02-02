using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.Framework.UI
{

    /// <summary>
    /// Defines base class for all Controls in Skill.UI
    /// </summary>
    public abstract class Control : BaseControl
    {
        /// <summary>
        /// Type of Control : Control
        /// </summary>
        public override ControlType ControlType { get { return ControlType.Control; } }

        /// <summary>
        /// The style to use. If null, the style from the current GUISkin is used.
        /// </summary>
        public virtual GUIStyle Style { get; set; }       

    }



}