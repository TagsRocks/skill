using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Base class for ContextMenu
    /// </summary>
    public interface IContextMenu
    {
        /// <summary> Owner ui object that showthis contenxt menu </summary>
        BaseControl Owner { get; }

        /// <summary>
        /// Show ContextMenu
        /// </summary>
        /// <param name="owner">Owner ui object that showthis contenxt menu</param>
        /// <param name="position">Mouse position</param>
        void Show(BaseControl owner, UnityEngine.Vector2 position);

    }
}
