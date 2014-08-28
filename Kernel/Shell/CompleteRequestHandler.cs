using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Common.Serializer;
using iCSharp.Messages;
using NetMQ.Sockets;

namespace iCSharp.Kernel.Shell
{
    public class CompleteRequestHandler : IShellMessageHandler
    {
        private ILog logger;

        public CompleteRequestHandler(ILog logger)
        {
            this.logger = logger;
        }

        public void HandleMessage(Message message, RouterSocket serverSocket, IOPub.IOPub ioPub)
        {
            CompleteRequest completeRequest = JsonSerializer.Deserialize<CompleteRequest>(message.Content);

            // TODO: Send reply
        }
    }
}
