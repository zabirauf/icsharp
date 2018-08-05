
namespace iCSharp.Kernel.Shell
{
    using Common.Logging;
    using iCSharp.Messages;
	using iCSharp.Kernel.Helpers;
    using NetMQ.Sockets;
    using Newtonsoft.Json.Linq;

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
            this.logger.Debug(string.Format("Message Content {0}", message.Content));
            KernelInfoRequest kernelInfoRequest = message.Content.ToObject<KernelInfoRequest>();

            // 1: Send Busy status on IOPub
            this.messageSender.SendStatus(message, ioPub, StatusValues.Busy);

            Message replyMessage = new Message()
            {
                Identifiers = message.Identifiers,
                Signature = message.Signature,
                ParentHeader = message.Header,
                Header = MessageBuilder.CreateHeader(MessageTypeValues.KernelInfoReply, message.Header.Session),
                Content = JObject.FromObject(this.CreateKernelInfoReply())
            };
            this.logger.Info("Sending kernel_info_reply");
            this.messageSender.Send(replyMessage, serverSocket);

            // 3: Send IDLE status message to IOPub
            this.messageSender.SendStatus(message, ioPub, StatusValues.Idle);
        }

        private KernelInfoReply CreateKernelInfoReply()
        {
            KernelInfoReply kernelInfoReply = new KernelInfoReply()
            {
                ProtocolVersion = "5.3",
                Implementation = "iCsharp",
                ImplementationVersion = "0.0.3",
                LanguageInfo = new JObject()
                {
                    { "name",  "C#" },
                    { "version", typeof(string).Assembly.ImageRuntimeVersion.Substring(1) },
                    { "mimetype", "text/x-csharp" },
                    { "file_extension", ".cs"},
                    { "pygments_lexer", "c#" }
                }
            };

            return kernelInfoReply;
        }
    }
}
