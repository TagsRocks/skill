using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Net
{
    public interface IMessageTranslator
    {
        Message Translate(int messageType);
    }

    public class DefaultMessageTranslator : IMessageTranslator
    {
        public Message Translate(int messageType)
        {
            switch ((MessageType)messageType)
            {
                case MessageType.Disconnect:
                    return new DisconnectMessage();
                case MessageType.Text:
                    return new TextMessage();
                case MessageType.RequestBTList:
                    break;
                case MessageType.BTList:
                    break;
                case MessageType.RequestBT:
                    break;
                case MessageType.BT:
                    break;
                case MessageType.BTUpdate:
                    break;
                default:
                    return null;
            }

            return null;
        }
    }
}
