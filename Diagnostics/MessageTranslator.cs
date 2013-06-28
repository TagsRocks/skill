using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Diagnostics
{
    public class MessageTranslator : Skill.Net.IMessageTranslator
    {
        public Skill.Net.Message Translate(int messageType)
        {
            switch ((MessageType)messageType)
            {
                case MessageType.Disconnect:
                    return new Skill.Net.DisconnectMessage();
                case MessageType.Text:
                    return new Skill.Net.TextMessage();
                case MessageType.RequestControllersList:
                    return new RequestControllersListMessage();
                case MessageType.ControllersList:
                    return new ControllersListMessage();
                case MessageType.RegisterControllerBT:
                    return new RegisterControllerBTMessage();
                case MessageType.BehaviorTree:
                    return new BehaviorTreeMessage();
                case MessageType.BehaviorTreeUpdate:
                    return new BehaviorTreeUpdateMessage();
                case MessageType.ControllerNotFound:
                    return new ControllerNotFoundMessage();
                default:
                    return null;
            }
        }
    }
}
