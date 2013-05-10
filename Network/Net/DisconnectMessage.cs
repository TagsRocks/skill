using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Net
{
    public class DisconnectMessage : Message
    {
        public override int Type { get { return (int)MessageType.Disconnect; } }

        public override void WriteData(MessageStream stream)
        {
            stream.Write(true);
        }

        public override void ReadData(MessageStream stream)
        {
            stream.ReadBoolean();
        }
    }
}
