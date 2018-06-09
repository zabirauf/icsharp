

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

            this.logger.Info("original code:" + code);
            this.logger.Info("original line:" + line);

            code = Regex.Replace(code.Substring(1, code.Length - 2), @"\\n", "*");
            line = line.Substring(1, line.Length - 2);

            this.logger.Info("substring code:" + code);
            this.logger.Info("substring line:" + line);

            Regex returnType = new Regex(@"[string|int|void]");
            Regex methodName = new Regex(@"(\w)");

            int cur_pos = completeRequest.CursorPosition;

            this.logger.Info("cur_pos " + cur_pos);

            
            //string newCode = code.
            
            line = line.Substring(0, cur_pos); //get substring of code from start to cursor position


            string[] arrayOfKeywords = { "team", "tech", "te", "term", "tame", "tata" };

            List<string> listOfKeywords = new List<string>();

            listOfKeywords.AddRange(arrayOfKeywords);

            List<CompleteReplyMatch> matches_ = new List<CompleteReplyMatch>();

            List<MethodMatch> methodMatches = new List<MethodMatch>();
            
            CatchAllWords(ref matches_, code);

            List<CompleteReplyMatch> methodMatchNames = new List<CompleteReplyMatch>();

            CatchMethods(code, ref methodMatchNames, ref methodMatches);

			List<CompleteReplyMatch> classMatchNames = new List<CompleteReplyMatch>();
			CatchClasses(code, ref classMatchNames);


                
            AddKeywordsToMatches(ref matches_);

            foreach (MethodMatch m in methodMatches)
            {
                this.logger.Info(m.Name);
                this.logger.Info("number of params = " + m.ParamList.Count);
                this.logger.Info("param types");
                for (int i = 0; i < m.ParamList.Count; i++)
                {
                    this.logger.Info(m.ParamList[i]);
                }

            }
            
            //code = Regex.Replace(code, @"[^\w&^\.]", "*"); //replace all non word and dot characters with '*'

            string cursorWord = FindWordToAutoComplete(line);

			List<CompleteReplyMatch> matchesSign = new List<CompleteReplyMatch>();
            
			MakeMethodSignMatches(line, methodMatches, ref matchesSign);

            this.logger.Info("cursor word " + cursorWord);

            this.logger.Info("Before Removal");

            for (int j = matches_.Count - 1; j > -1; j--)
            {
                this.logger.Info(matches_[j].Name);
            }

            //Console.WriteLine(cursorWord);

            RemoveNonMatches(ref matches_, cursorWord);

            this.logger.Info("After Removal");

            for (int j = matches_.Count - 1; j > -1; j--)
            {
                this.logger.Info(matches_[j].Name);
            }
            
            if (line.Length > 0)
            {
                if (line[(line.Length - 1)].Equals('.'))
                {
                    matches_ = methodMatchNames;
					matches_.AddRange(classMatchNames);
                }
				else if(line[(line.Length - 1)].Equals('(')){
					matches_ = matchesSign;
				}
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



            Message completeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.CompleteReply, JsonSerializer.Serialize(completeReply), message.Header);
            this.logger.Info("Sending complete_reply");
            this.messageSender.Send(completeReplyMessage, serverSocket);

        }

        public void CatchAllWords(ref List<CompleteReplyMatch> matches_, string newCode)
        {
            //this.logger.Info(s);
            this.logger.Info("weselna elhamdlAllah");

            string catchPattern = @"(\w+)";

            Regex p = new Regex(catchPattern);

            foreach (Match m in p.Matches(newCode))
            {

                CompleteReplyMatch crm = new CompleteReplyMatch()
                {

                    Name = m.ToString(),
                    Documentation = "",
                    Value = "",

                };
                matches_.Add(crm);
            }
            

        }

		public void CatchClasses(string code, ref List<CompleteReplyMatch> classMatchNames){
			Regex p = new Regex(@"(?<documentation>\/\/\/(?:(?!\n\w).)*?)?(\n)*(private|internal|public)?(class)([\s]+)(?<classname>\w+)");

			Regex doc = new Regex(@"(<summary>)(?<docsum>(?:(?!<\/summary>).)*)");
            string editedDoc;




			foreach(Match m in p.Matches(code)){

				this.logger.Info("found class");

				Match i = doc.Match(m.Groups["documentation"].ToString());
				editedDoc = i.Groups["docsum"].ToString();
				editedDoc = editedDoc.Replace("///", "");
                editedDoc = editedDoc.Replace("*", "");
                editedDoc = editedDoc.TrimStart(' ');
                editedDoc = editedDoc.TrimEnd(' ');
                editedDoc = editedDoc.Replace(@"\s+", " ");

				this.logger.Info(editedDoc + " class new docsum here----");
			
				CompleteReplyMatch crm = new CompleteReplyMatch()
                {

                    Name = m.Groups["classname"].ToString(),
                    Documentation = "",
                    Value = "",

                };
				classMatchNames.Add(crm);

			}
		}

        public void CatchMethods(string code, ref List<CompleteReplyMatch> methodMatchNames, ref List<MethodMatch> methodMatches)
        {

            this.logger.Info("code in methods " + code);

            //[^(\n\w)]

            Regex p = new Regex(@"(?<documentation>\/\/\/(?:(?!\n\w).)*?)?(\n)*(?<returntype>string|sbyte|short|int|long|byte|ushort|uint|ulong|char|float|double|decimal|bool|enum|void)([\s]+)(?<methodname>\w+)(?<paramlist>\([^\)]*\))");
            
            Regex r = new Regex(@"(?<returntype>string|sbyte|short|int|long|byte|ushort|uint|ulong|char|float|double|decimal|bool|enum)([\s]+)(?<parnames>\w+)");

            Regex doc = new Regex(@"(<summary>)(?<docsum>(?:(?!<\/summary>).)*)");
            string editedDoc;

            foreach (Match m in p.Matches(code))
            {



                List<string> parameterTypes = new List<string>();

                this.logger.Info("found method match");
                this.logger.Info(m.Groups["documentation"] + " documentation");
                Match i = doc.Match(m.Groups["documentation"].ToString());
                editedDoc = i.Groups["docsum"].ToString();

                this.logger.Info(i.Groups["docsum"].ToString() + " docsum here!!!");

                editedDoc = editedDoc.Replace("///", "");
                editedDoc = editedDoc.Replace("*", "");
                editedDoc = editedDoc.TrimStart(' ');
                editedDoc = editedDoc.TrimEnd(' ');
                editedDoc = editedDoc.Replace(@"\s+", " ");

                this.logger.Info(editedDoc + " method new docsum here----");
                this.logger.Info(m.Groups["methodname"] + " name");
                this.logger.Info(m.Groups["returntype"] + " type");
                this.logger.Info(m.Groups["paramlist"] + " paramList");

                foreach (Match parType in r.Matches(m.Groups["paramlist"].ToString()))
                {

                    this.logger.Info("we have a parameter");
                    this.logger.Info(parType.Groups["returntype"]);
                    this.logger.Info(parType.Groups["parnames"]);

                    parameterTypes.Add(parType.Groups["returntype"].ToString());

                }

                MethodMatch mm = new MethodMatch()
                {
                    Name = m.Groups["methodname"].ToString(),
                    Documentation = editedDoc,
                    Value = "",
                    ParamList = parameterTypes,

                };
                methodMatches.Add(mm);

                CompleteReplyMatch crm = new CompleteReplyMatch()
                {

                    Name = m.Groups["methodname"].ToString(),
                    Documentation = editedDoc,
                    Value = "",

                };
                methodMatchNames.Add(crm);
            }


        }

        public void AddKeywordsToMatches(ref List<CompleteReplyMatch> matches_)
        {

            string[] arrayOfKeywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "using static", "virtual", "void", "volatile", "while" };
            List<string> listOfKeywords = new List<string>();
            listOfKeywords.AddRange(arrayOfKeywords);

            foreach (string i in listOfKeywords)
            {
                CompleteReplyMatch crm = new CompleteReplyMatch()
                {

                    Name = i,
                    Documentation = "",
                    Value = "",

                };
                matches_.Add(crm);

            }
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

        public void RemoveNonMatches(ref List<CompleteReplyMatch> matches_, string cursorWord)
        {
            for (int j = matches_.Count - 1; j > -1; j--)
            {
                if (!(matches_[j].Name.StartsWith(cursorWord)))
                {
                    matches_.RemoveAt(j);
                }
            }
        }

		public void MakeMethodSignMatches(string line,List<MethodMatch> methodMatches, ref List<CompleteReplyMatch> matchesSign){

			Regex regex = new Regex(@"(?<name>\w+)(\()");

			Match i = regex.Match(line);

			string add;
            
			foreach(MethodMatch m in methodMatches){

				add = "";
				if(i.Groups["name"].ToString().Equals(m.Name)){
					add = m.Name + "(";
					foreach(string s in m.ParamList){
						add = add + s+", ";
					}
					add = add + ")";
				}

				CompleteReplyMatch crm = new CompleteReplyMatch
				{
					Name = add,
					Documentation = "",
					Value = "",
				};
				matchesSign.Add(crm);


			}

			
		}


    }
}
