

using System;
using Common;
using Common.Logging;
using Common.Serializer;
using iCSharp.Messages;
using NetMQ.Sockets;

namespace iCSharp.Kernel.Shell
{
    public class KernelInfoRequestHandler : IShellMessageHandler
    {
        private ILog logger;

        public KernelInfoRequestHandler(ILog logger)
        {
            this.logger = logger;
        }

        public void HandleMessage(Message message, RouterSocket serverSocket, IOPub.IOPub ioPub)
        {
            KernelInfoRequest kernelInfoRequest = JsonSerializer.Deserialize<KernelInfoRequest>(message.Content);

            Message replyMessage = new Message()
            {
                UUID = message.Header.Session,
                ParentHeader = message.Header,
                Header = this.CreateHeader(message),
                Content = JsonSerializer.Serialize(this.CreateKernelInfoReply())
            };

            this.logger.Info("Sending kernel_info_reply");
            MessageSender.Send(replyMessage, serverSocket);
        }

        private Header CreateHeader(Message message)
        {
            Header newHeader = new Header()
            {
                Username = Constants.USERNAME,
                Session = message.Header.Session,
                MessageId = Guid.NewGuid().ToString(),
                MessageType = MessageTypeValues.KernelInfoReply,
                Version = Constants.VERSION
            };

            return newHeader;
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
