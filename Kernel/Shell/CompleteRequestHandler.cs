

namespace iCSharp.Kernel.Shell
{
    using Common.Logging;
    using Common.Serializer;
    using iCSharp.Messages;
    using NetMQ.Sockets;

    public class CompleteRequestHandler : IShellMessageHandler
    {
        private ILog logger;

        public CompleteRequestHandler(ILog logger)
        {
            this.logger = logger;
        }

        public void HandleMessage(Message message, RouterSocket serverSocket, PublisherSocket ioPub)
        {
            CompleteRequest completeRequest = JsonSerializer.Deserialize<CompleteRequest>(message.Content);

            // TODO: Send reply
        }
    }
}
