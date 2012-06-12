using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio
{
    public interface IDataViewModel
    {
        /// <summary>
        /// Allow data to know about movement of entities to correct internal references
        /// </summary>
        /// <param name="type">type of entity</param>
        /// <param name="previousPath">previous local path</param>
        /// <param name="newPath">new local path</param>
        void NotifyEntityChange(EntityType type, string previousPath, string newPath);

        /// <summary> Take care of undo and redo </summary>
        UnDoRedo History { get; set; }

        /// <summary> Model data </summary>
        object GetDataModel();

        /// <summary> Allow ViewModel commite changes to Model before save </summary>
        void CommiteChanges();
    }
}
