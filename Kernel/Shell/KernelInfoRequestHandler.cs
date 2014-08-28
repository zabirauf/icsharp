
namespace iCSharp.Kernel.Shell
{
    using Common.Logging;
    using Common.Serializer;
    using iCSharp.Messages;
    using NetMQ.Sockets;

    public class KernelInfoRequestHandler : IShellMessageHandler
    {
        private ILog logger;

        public KernelInfoRequestHandler(ILog logger)
        {
            this.logger = logger;
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
            MessageSender.Send(replyMessage, serverSocket);
        }

        private KernelInfoReply CreateKernelInfoReply()
        {
            KernelInfoReply kernelInfoReply = new KernelInfoReply()
            {
                ProtocolVersion = "5.0",
                LanguageVersion = "0.0.1",
                Language = "C#",
                Implementation = "iCsharp",
            };

            return kernelInfoReply;
        }
    }
}
