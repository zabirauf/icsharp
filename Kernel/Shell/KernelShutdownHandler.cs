namespace iCSharp.Kernel.Shell
{
    using System;
    using System.Collections.Generic;
    using Common.Logging;
    using iCSharp.Kernel.Helpers;
    using iCSharp.Messages;
    using NetMQ.Sockets;

    public class KernelShutdownHandler : IShellMessageHandler
    {
        private ILog logger;

        private readonly IMessageSender messageSender;

        private readonly List<IServer> servers;

        public KernelShutdownHandler(ILog logger, IMessageSender messageSender, params IServer[] servers)
        {
            this.logger = logger;
            this.messageSender = messageSender;

            this.servers = new List<IServer>();
            foreach (var s in servers)
            {
                this.servers.Add(s);
            }
        }

        public void HandleMessage(Message message, RouterSocket serverSocket, PublisherSocket ioPub)
        {
            ShutdownRequestReply shutdownRequest = message.Content.ToObject<ShutdownRequestReply>();

            // shutdown servers
            foreach (IServer server in this.servers)
            {                
                this.logger.Info("Shutdown "+ server.ToString());
                server.Stop();
                if (shutdownRequest.Restart)
                {
                    this.logger.Info("Restart " + server.ToString());
                    server.Start();
                }
            }
        }
    }
}
