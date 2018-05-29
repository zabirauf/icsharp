

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
            
            this.logger.Info("oringal code:" + code);

			Regex returnType = new Regex(@"[string|int|void]");

			Regex methodName = new Regex(@"(\w)");



            

            int cur_pos = completeRequest.CursorPosition;

            this.logger.Info("cur_pos " + cur_pos);

            code = code.Substring(2, code.Length - 2);

            this.logger.Info("fixed code " + code);

            string newCode = code.Substring(0, cur_pos); //get substring of code from start to cursor position

            this.logger.Info("newcode " + newCode);

            string[] arrayOfKeywords = {"team", "tech", "te", "term", "tame", "tata"};

            List<string> listOfKeywords = new List<string>();

            listOfKeywords.AddRange(arrayOfKeywords); 

            List<CompleteReplyMatch> matches_ = new List<CompleteReplyMatch>();

			List<MethodMatch> methodMatches = new List<MethodMatch>();

            

            string catchPattern = @"(\w+)";

            Regex p = new Regex(catchPattern);

            foreach(Match m in p.Matches(newCode)){
                
              CompleteReplyMatch crm = new CompleteReplyMatch(){

                Name = m.ToString(),
                Documentation = "",
                Value = "",

              };
                matches_.Add(crm);    
            }

			/////////////////////////////////////////(\([.*]\))   .Groups["methodname"].  (\((.*)\))

			p = new Regex(@"(?<returntype>string|int|void)([\s]+)(?<methodname>\w+)(?<paramlist>\([^\)]*\))");
            
			Regex r = new Regex(@"(?<returntype>string|int|void)([\s]+)(?<parnames>\w+)");

			List<CompleteReplyMatch> methodMatchNames = new List<CompleteReplyMatch>();





			foreach (Match m in p.Matches(code))
            {

				List<string> parameterTypes = new List<string>();

				this.logger.Info("found method match");
				this.logger.Info(m.Groups["methodname"] + " name");
				this.logger.Info(m.Groups["returntype"] + " type");
				this.logger.Info(m.Groups["paramlist"] + " paramList");

				foreach (Match parType in r.Matches(m.Groups["paramlist"].ToString())){

					this.logger.Info("we have a parameter");
					this.logger.Info(parType.Groups["returntype"]);
					this.logger.Info(parType.Groups["parnames"]);

					parameterTypes.Add(parType.Groups["returntype"].ToString());

				}

				MethodMatch mm = new MethodMatch()
				{
					Name = m.Groups["methodname"].ToString(),
				    Documentation = "",
				    Value = "",
					ParamList = parameterTypes,



				};
				methodMatches.Add(mm);





                

				CompleteReplyMatch crm = new CompleteReplyMatch(){
                    
					Name = m.Groups["methodname"].ToString(),
                    Documentation = "",
                    Value = "",

                };
				methodMatchNames.Add(crm);
            }

            ////////////////////////////////////////////////



            foreach(string i in listOfKeywords){

              CompleteReplyMatch crm = new CompleteReplyMatch(){

                Name = i,
                Documentation = "",
                Value = "",

              };
                matches_.Add(crm);

            }

			foreach(MethodMatch m in methodMatches){
				this.logger.Info(m.Name);
				this.logger.Info("number of params = " + m.ParamList.Count );
				this.logger.Info("param types");
				for (int i = 0; i < m.ParamList.Count; i++){
					this.logger.Info(m.ParamList[i]);
				}

			}

            //matches_.AddRange(listOfKeywords);

            newCode = Regex.Replace(newCode, @"[^\w&^\.]", "*"); //replace all non word and dot characters with '*'

            string cursorWord, cursorLine; 

            p = new Regex(@".*\*"); //regex to match up to last '*'
            Match mat = p.Match(newCode);

            if(mat.Success){

              cursorLine = newCode.Substring(mat.Index+mat.Length);              

            }
            else{
              cursorLine = newCode;
            }


            p = new Regex(@".*\.");
            mat = p.Match(cursorLine);

            if(mat.Success){
              cursorWord = cursorLine.Substring(mat.Index+mat.Length);

               
            }
            else{
              cursorWord = cursorLine;
              cursorLine = "";
            }

            this.logger.Info("cursor word " + cursorWord);

            this.logger.Info("Before Removal");

            for (int j = matches_.Count - 1; j > -1; j--)
            {
                this.logger.Info(matches_[j].Name);
            }

            //Console.WriteLine(cursorWord);

            for (int j = matches_.Count-1; j > -1; j--){
              if(!(matches_[j].Name.StartsWith(cursorWord))){
                matches_.RemoveAt(j);
              }
            }

            this.logger.Info("After Removal");

            for (int j = matches_.Count - 1; j > -1; j--)
            {
                this.logger.Info(matches_[j].Name);
            }



            matches_.Add(new CompleteReplyMatch() { Name = "newcode" + newCode });
            matches_.Add(new CompleteReplyMatch() { Name = "code" + code });

            // string txt = completeRequest.Text;
            // string line = completeRequest.Line;
            // int cur_pos = completeRequest.CursorPosition;
            // BlockType block = JsonSerializer.Deserialize<BlockType>(completeRequest.Block);


            // Need to know whats inside completeRequest
            // Temporary message to the shell for debugging purposes

            //   matches_.Add("first_one");
            //   matches_.Add("second_one");
            //   matches_.Add("code " + code + " cur_pos " + cur_pos);

            // matches_.Add("ch: " + block.ch + " line: " + block.line + " selected: " + block.selectedIndex);

            // New PROTOCOL

			if(mat.Success){
				matches_ = methodMatchNames;
			}

			for (int j = methodMatchNames.Count - 1; j > -1; j--)
            {
				this.logger.Info("methodmatch");
				this.logger.Info(methodMatchNames[j].Name);
            }

                CompleteReply completeReply = new CompleteReply()
                {
                    //CursorEnd = 10,
                    Matches = matches_,
                    Status = "ok",
                    //CursorStart = 5,
                   // MetaData = null
                };

            // OLD PROCOTOL
            /*
            CompleteReply completeReply = new CompleteReply()
            {
                MatchedText = "something_matched",
                Matches = matches_,
                Status = "ok",
                FilterStartIndex = 0,
            };*/

            Message completeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.CompleteReply, JsonSerializer.Serialize(completeReply), message.Header);
            this.logger.Info("Sending complete_reply");
            this.messageSender.Send(completeReplyMessage, serverSocket);

        }
    }
}
