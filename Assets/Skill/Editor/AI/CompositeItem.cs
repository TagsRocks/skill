using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    public class CompositeItem : TreeViewFolder
    {
        public CompositeItem(CompositeData data)
            : base(data)
        {

        }

        public override string Title { get { return ((CompositeData)Data).CompositeType.ToString(); } }
    }
}