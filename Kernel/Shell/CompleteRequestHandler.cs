

namespace iCSharp.Kernel.Shell
{
    using Common.Logging;
    using System.Collections.Generic;
    using Common.Serializer;
    using iCSharp.Messages;
    using NetMQ.Sockets;
    using iCSharp.Kernel.Helpers;

    public class CompleteRequestHandler : IShellMessageHandler
    {
        private ILog logger;
        private readonly IMessageSender messageSender;

        public CompleteRequestHandler(ILog logger, IMessageSender messageSender)
        {
            this.logger = logger;
            this.messageSender = messageSender;
        }

        public void HandleMessage(Message message, RouterSocket serverSocket, PublisherSocket ioPub)
        {
            CompleteRequest completeRequest = JsonSerializer.Deserialize<CompleteRequest>(message.Content);

            List<string> matches_ = new List<string>();
             matches_.Add("first_one");
             matches_.Add("second_one");

            /* CompleteReply completeReply = new CompleteReply()
             {
                 CursorEnd = 10,
                 Matches = matches_,
                 Status = "ok",
                 CursorStart = 5
             };*/

            CompleteReply completeReply = new CompleteReply()
            {
                MatchedText = "Matched_Text",
                Matches = matches_,
                Status = "ok",
                FilterStartIndex = 0,
            };

            Message completeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.CompleteReply, JsonSerializer.Serialize(completeReply), message.Header);
            this.logger.Info("Sending complete_reply");
            this.messageSender.Send(completeReplyMessage, serverSocket);

        }
    }
}
