using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Skill.Net
{
    /// <summary>
    /// A Worker handles connection of client from server
    /// </summary>
    public sealed class ServerWorker : Worker
    {
        /// <summary> Server </summary>
        public Server Server { get; private set; }

        internal ServerWorker(IMessageTranslator messageTranslator, int bufferSize, Server server)
            : base(messageTranslator, bufferSize)
        {
            this.Server = server;
        }

        protected override void OnClosed()
        {
            if (this.Server != null)
                this.Server.NotifyWorkerDisconnected(this);
            base.OnClosed();
        }
    }
}
