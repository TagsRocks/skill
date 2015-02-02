using Skill.Editor.UI;
using Skill.Framework.UI;
using System;

namespace Skill.Editor
{
    public interface IEditor
    {
        /// <summary>Add new item after this item </summary>
        /// <param name="item"> item to add after</param>
        void NewAfter(EditorListItem item);

        /// <summary>remove this item </summary>
        /// <param name="item"> item to remove</param>
        void Remove(EditorListItem item);
    }

    
}
