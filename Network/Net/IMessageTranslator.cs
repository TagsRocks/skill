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
        public Skill.Net.Message Translate(int messageType)
        {
            switch ((MessageType)messageType)
            {
                case MessageType.Disconnect:
                    return new Skill.Net.DisconnectMessage();
                case MessageType.Text:
                    return new Skill.Net.TextMessage();
                default:
                    return null;
            }
        }
    }
}
