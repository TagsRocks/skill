using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Net
{
    /// <summary>
    /// A message containing text
    /// </summary>
    public class TextMessage : Message
    {
        /// <summary> Type of message </summary>
        public override int Type { get { return (int)MessageType.Text; } }
        /// <summary> Text message </summary>
        public string Text { get; set; }

        public override void WriteData(MessageStream stream)
        {
            if (string.IsNullOrEmpty(Text))
            {
                stream.Write(false);
            }
            else
            {
                stream.Write(true);
                stream.Write(Text);
            }
        }

        public override void ReadData(MessageStream stream)
        {
            if (stream.ReadBoolean())
            {
                Text = stream.ReadString();
            }
        }
    }
}
