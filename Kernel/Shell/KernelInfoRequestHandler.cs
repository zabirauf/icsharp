
namespace iCSharp.Kernel.Shell
{
    using Common.Logging;
    using Common.Serializer;
    using iCSharp.Messages;
	using iCSharp.Kernel.Helpers;
    using NetMQ.Sockets;

    public class KernelInfoRequestHandler : IShellMessageHandler
    {
        private readonly ILog logger;

		private readonly IMessageSender messageSender;

        public KernelInfoRequestHandler(ILog logger, IMessageSender messageSender)
        {
            this.logger = logger;
			this.messageSender = messageSender;
        }

        public void HandleMessage(Message message, RouterSocket serverSocket, PublisherSocket ioPub)
        {
            KernelInfoRequest kernelInfoRequest = JsonSerializer.Deserialize<KernelInfoRequest>(message.Content);

            Message replyMessage = new Message()
            {
                UUID = message.Header.Session,
                ParentHeader = message.Header,
                Header = MessageBuilder.CreateHeader(MessageTypeValues.KernelInfoReply, message.Header.Session),
                Content = JsonSerializer.Serialize(this.CreateKernelInfoReply())
            };

            this.logger.Info("Sending kernel_info_reply");
			this.messageSender.Send(replyMessage, serverSocket);
        }

        private KernelInfoReply CreateKernelInfoReply()
        {
            KernelInfoReply kernelInfoReply = new KernelInfoReply()
            {
                ProtocolVersion = "4.1",
                LanguageVersion = "0.0.1",
                IPythonVersion = "4.0.0",
                Language = "csharp",
                Implementation = "iCsharp",
                ImplementationVersion = "0.0.2"
            };

            return kernelInfoReply;
        }
    }
}
