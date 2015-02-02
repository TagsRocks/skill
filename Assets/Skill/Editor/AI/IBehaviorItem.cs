using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    public interface IBehaviorItem
    {
        BehaviorTreeEditorWindow Editor { get; }
        BehaviorData Data { get; }        

        /// <summary> Check whether this node can move up inside parent children </summary>  
        bool CanMoveUp { get; }

        /// <summary> Check whether this node can move down inside parent children </summary>  
        bool CanMoveDown { get; }

        void RefreshContent();
    }
}