

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
            string code = completeRequest.Code;
            int cur_pos = completeRequest.CursorPosition;

           // string txt = completeRequest.Text;
           // string line = completeRequest.Line;
           // int cur_pos = completeRequest.CursorPosition;
           // BlockType block = JsonSerializer.Deserialize<BlockType>(completeRequest.Block);


            // Need to know whats inside completeRequest
            // Temporary message to the shell for debugging purposes


             List<string> matches_ = new List<string>();
             matches_.Add("first_one");
             matches_.Add("second_one");
             matches_.Add("code " + code + " cur_pos " + cur_pos);

            // matches_.Add("ch: " + block.ch + " line: " + block.line + " selected: " + block.selectedIndex);


            /* CompleteReply completeReply = new CompleteReply()
             {
                 CursorEnd = 10,
                 Matches = matches_,
                 Status = "ok",
                 CursorStart = 5
             };*/

            CompleteReply completeReply = new CompleteReply()
            {
                //  MatchedText = txt,
                Matches = matches_,
                Status = "ok",
                CursorStart = 0,
                CursorEnd = 0,
               // FilterStartIndex = 0,
            };

            Message completeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.CompleteReply, JsonSerializer.Serialize(completeReply), message.Header);
            this.logger.Info("Sending complete_reply");
            this.messageSender.Send(completeReplyMessage, serverSocket);

        }
    }
}
