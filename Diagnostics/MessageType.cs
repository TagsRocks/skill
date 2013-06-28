using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Diagnostics
{
    public enum MessageType
    {
        /// <summary> Disconnect </summary>
        Disconnect = Skill.Net.MessageType.Disconnect,
        /// <summary> Text Message </summary>
        Text = Skill.Net.MessageType.Text,
        /// <summary> Request list of Controllers with valid Identifiers </summary>
        RequestControllersList = Text + 1,
        /// <summary> list of Controllers with valid Identifiers</summary>
        ControllersList = Text + 2,
        /// <summary> Register Controller to get BehaviorTree update every frame</summary>
        RegisterControllerBT = Text + 3,
        /// <summary> Definition of BehaviorTree </summary>
        BehaviorTree = Text + 4,
        /// <summary> Update information of BehaviorTree </summary>
        BehaviorTreeUpdate = Text + 5,
        /// <summary> Controller is disposed or not available anymore </summary>
        ControllerNotFound = Text + 6
    }
}
