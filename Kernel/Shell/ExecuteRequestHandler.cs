


namespace iCSharp.Kernel.Shell
{
    using Common.Logging;
    using Common.Serializer;
    using iCSharp.Messages;
    using NetMQ.Sockets;
    using iCSharp.Kernel.IOPub;

    public class ExecuteRequestHandler : IShellMessageHandler
    {
        private ILog logger;

        public ExecuteRequestHandler(ILog logger)
        {
            this.logger = logger;
        }

        public void HandleMessage(Message message, RouterSocket serverSocket, IOPub ioPub)
        {
            this.logger.Debug(string.Format("Message Content {0}", message.Content));
            ExecuteRequest executeRequest = JsonSerializer.Deserialize<ExecuteRequest>(message.Content);

            this.logger.Info(string.Format("Execute Request received with code {0}", executeRequest.Code));
        }
    }
}
