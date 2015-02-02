using System;
using System.Collections.Generic;
using System.Text;


namespace Skill.Framework.UI
{

    /// <summary>
    /// Describes how a child element is vertically positioned or stretched within a parent's layout slot.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// The child element is aligned to the top of the parent's layout slot.
        /// </summary>
        Top = 0,
        /// <summary>
        /// The child element is aligned to the center of the parent's layout slot.
        /// </summary>
        Center = 1,
        /// <summary>
        /// The child element is aligned to the bottom of the parent's layout slot.
        /// </summary>
        Bottom = 2,
        /// <summary>
        /// The child element stretches to fill the parent's layout slot.
        /// </summary>
        Stretch = 3,
    }

}