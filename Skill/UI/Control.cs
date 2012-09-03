using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.UI
{
    #region Control
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
        public GUIStyle Style { get; set; }       

        /// <summary>
        /// Indicates whether the element can receive focus.
        /// </summary>
        public virtual bool Focusable { get { return false; } }


    } 
    #endregion

    #region FocusableControl
    /// <summary>
    /// Base class for focusables Controls
    /// </summary>
    public abstract class FocusableControl : Control
    {
        /// <summary>
        /// Indicates whether the element can receive focus.
        /// </summary>
        public override bool Focusable { get { return true; } }

        private bool _IsFocused;
        /// <summary>
        /// Gets a value that determines whether this element has logical focus.
        /// </summary>
        /// <returns>
        /// true if this element has logical focus; otherwise, false.
        /// </returns>
        public bool IsFocused
        {
            get { return _IsFocused; }
            internal set
            {
                if (_IsFocused != value)
                {
                    _IsFocused = value;
                    if (_IsFocused)
                        OnGotFocus();
                    else
                        OnLostFocus();
                }
            }
        }

        /// <summary>
        /// Occurs when this element gets logical focus.
        /// </summary>
        public event EventHandler GotFocus;
        protected virtual void OnGotFocus()
        {
            if (GotFocus != null) GotFocus(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when this element loses logical focus.
        /// </summary>
        public event EventHandler LostFocus;
        protected virtual void OnLostFocus()
        {
            if (LostFocus != null) LostFocus(this, EventArgs.Empty);
        }
    } 
    #endregion
}