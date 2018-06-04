

namespace iCSharp.Kernel.Shell
{
    using Common.Logging;
    using System.Collections.Generic;
    using Common.Serializer;
    using iCSharp.Messages;
    using NetMQ.Sockets;
    using iCSharp.Kernel.Helpers;
    using System.Text.RegularExpressions;
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

            string code = completeRequest.CodeCells[0];
			string line = completeRequest.Line;

            code = Regex.Replace(code.Substring(1, code.Length - 2), @"\\n", "*");
            line = line.Substring(1, line.Length - 2);

            int cur_pos = completeRequest.CursorPosition;

            this.logger.Info("cur_pos " + cur_pos);

            line = line.Substring(0, cur_pos); //get substring of code from start to cursor position

            List<CompleteReplyMatch> matches_ = new List<CompleteReplyMatch>();

            string cursorWord = FindWordToAutoComplete(line);

            CompleteReply completeReply = new CompleteReply()
            {
                //CursorEnd = 10,
                Matches = matches_,
                Status = "ok",
                //CursorStart = 5,
                // MetaData = null
            };



            Message completeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.CompleteReply, JsonSerializer.Serialize(completeReply), message.Header);
            this.logger.Info("Sending complete_reply");
            this.messageSender.Send(completeReplyMessage, serverSocket);

        }

        public string FindWordToAutoComplete(string line)
        {
            line = Regex.Replace(line, @"[^\w&^\.]", "*");

            string cursorWord, cursorLine;

            Regex p = new Regex(@".*\*"); //regex to match up to last '*'
            Match mat = p.Match(line);

            if (mat.Success)
            {

                cursorLine = line.Substring(mat.Index + mat.Length);

            }
            else
            {
                cursorLine = line;
            }


            p = new Regex(@".*\.");
            mat = p.Match(cursorLine);

            if (mat.Success)
            {
                cursorWord = cursorLine.Substring(mat.Index + mat.Length);


            }
            else
            {
                cursorWord = cursorLine;
                cursorLine = "";
            }

            return cursorWord;

        }

    }
}
