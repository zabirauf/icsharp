

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
        /*
         * TWO CASES 
         * - USE CODEMIRRROR AUTOCOMPLETION (shouldn't be hard to integrate, lacks some features, switches calculations to client side)
         * - MAKE OWN COMPLETION ENGINE (Allows greater control and can add many features, multiplies complexity)
         * 
         Completer
            Two major parts;
                1. Find completions for code
                2. Find completions for declarations

            Therefore we will need a completion engine with multiple completers.
            The completers all have a specific category in which they will provide matches
            so a completer for directories, a completer for strings, etc
            The completer will return matches even if another competer returns a similar match
            It is not the completer's job to give you the right match but all possible matches
            It is the completion's engine job to provide the right matches

            Therefore the completion engine (a separate class) will have several functions (completers)
            which it will call and then try to figure out matches. The completion engine will be fed the tokenized
            code from the complete_request message

            Issue 0 - Both:
                0.1 - Tokenise all the words by filtering through word break characters
                - word break characters (not full list): \t\n\"\\\'><=;:
                0.0 - Separate whether it is a local case (code) or global (declarations)
                0.2 - Figure out whether its a declaration case or local case through
                      triggers (./\ etc) and keywords (using)

            Issue 1 - Code:
                - Run the different completers on the code to provide matches
                - Figure out the right matches to return
                - Rebuild the match (e.g. fruits.a -> matches to fruits.apples, fruits.apricot, need to return
                correct changed text using cursor pos
                - Send the results back
                */
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
