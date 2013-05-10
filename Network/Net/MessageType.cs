using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Net
{
    public enum MessageType
    {
        /// <summary> Disconnect </summary>
        Disconnect = 0,
        /// <summary> Text Message </summary>
        Text = 1,
        /// <summary> Request list of BehaviorTrees </summary>
        RequestBTList = 2,
        /// <summary> list of BehaviorTrees </summary>
        BTList = 3,
        /// <summary> Request BehaviorTree </summary>
        RequestBT = 4,
        /// <summary> Definition of BehaviorTree </summary>
        BT = 5,
        /// <summary> Update information of BehaviorTree </summary>
        BTUpdate = 6,
    }
}
