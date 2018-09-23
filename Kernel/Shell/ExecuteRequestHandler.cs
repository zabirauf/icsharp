


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using iCSharp.Kernel.ScriptEngine;
using System.Web;
using iCSharp.Kernel.Helpers;

namespace iCSharp.Kernel.Shell
{
    using Common.Logging;
    using Common.Serializer;
    using iCSharp.Messages;
    using NetMQ.Sockets;
    using Newtonsoft.Json.Linq;

    public class ExecuteRequestHandler : IShellMessageHandler
    {
        private readonly ILog logger;

        private readonly IReplEngine replEngine;

		private readonly IMessageSender messageSender;

		private int executionCount = 0;

        public ExecuteRequestHandler(ILog logger, IReplEngine replEngine, IMessageSender messageSender)
        {
            this.logger = logger;
            this.replEngine = replEngine;
			this.messageSender = messageSender;
        }

        public void HandleMessage(Message message, RouterSocket serverSocket, PublisherSocket ioPub)
        {
            this.logger.Debug(string.Format("Message Content {0}", message.Content));
            ExecuteRequest executeRequest = message.Content.ToObject<ExecuteRequest>();

            this.logger.Info(string.Format("Execute Request received with code \n{0}", executeRequest.Code));

            // 1: Send Busy status on IOPub
            this.messageSender.SendStatus(message, ioPub, StatusValues.Busy);

            // 2: Send execute input on IOPub
            if (!executeRequest.Silent)
            {
                this.executionCount += 1;
                this.SendExecuteInputMessageToIOPub(message, ioPub, executeRequest.Code);
            }

            // 3: Evaluate the C# code
            IOPubConsole ioPubConsole = new IOPubConsole(message, ioPub, this.messageSender, this.executionCount, this.logger);
            ioPubConsole.RedirectConsole();
            string code = executeRequest.Code;
            ExecutionResult results = this.replEngine.Execute(code);
            ioPubConsole.CancelRedirect();

            if (!results.IsError)
            {
                // 4: Send execute result message to IOPub
                if (results.OutputResultWithColorInformation.Any())
                {
                    string codeOutput = this.GetCodeOutput(results);
                    string codeHtmlOutput = this.GetCodeHtmlOutput(results);

                    JObject data = new JObject()
                    {
                        {"text/plain", codeOutput},
                        {"text/html", codeHtmlOutput}
                    };

                    DisplayData displayData = new DisplayData()
                    {
                        Data = data,
                    };
                    this.SendDisplayDataMessageToIOPub(message, ioPub, displayData);
                }

                // 5: Send execute reply to shell socket
                this.SendExecuteReplyOkMessage(message, serverSocket);
            }
            else
            {
                var ex = results.CompileError != null ? results.CompileError : results.ExecuteError;
                dynamic errorContent = new JObject();
                errorContent.execution_count = this.executionCount;
                errorContent.ename = ex.GetType().ToString();
                errorContent.evalue = ex.Message;
                var trace = new JArray(ex.StackTrace.Split('\n'));
                trace.AddFirst(ex.Message);
                errorContent.traceback = trace;

                // 6: Send error message to IOPub
                this.SendErrorMessageToIOPub(message, ioPub, errorContent);

                // 7: Send execute reply message to shell socket
                this.SendExecuteReplyErrorMessage(message, serverSocket, errorContent);
            }

            // 8: Send IDLE status message to IOPub
            this.messageSender.SendStatus(message, ioPub, StatusValues.Idle);
        }

        private string GetCodeOutput(ExecutionResult executionResult)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string result in executionResult.OutputResults)
            {
                sb.Append(result);
            }

            return sb.ToString();
        }

        private string GetCodeHtmlOutput(ExecutionResult executionResult)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Tuple<string, ConsoleColor> tuple in executionResult.OutputResultWithColorInformation)
            {
                string encoded = HttpUtility.HtmlEncode(tuple.Item1);
                sb.Append(string.Format("<font style=\"color:{0}\">{1}</font>", tuple.Item2.ToString(), encoded));
            }

            return sb.ToString();
        }

        public void SendDisplayDataMessageToIOPub(Message message, PublisherSocket ioPub, DisplayData data)
        {
            JObject content = new JObject()
            {
                { "data",  data.Data},
                { "metadata",  data.MetaData},
                { "transient", new JObject() },
            };

            Message outputMessage = MessageBuilder.CreateMessage(MessageTypeValues.DisplayData, content, message.Header);

            this.logger.Info(string.Format("Sending message to IOPub {0}", JsonSerializer.Serialize(outputMessage)));
			this.messageSender.Send(outputMessage, ioPub);
        }

        public void SendErrorMessageToIOPub(Message message, PublisherSocket ioPub, JObject errorContent)
        {
            Message executeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.Error, errorContent, message.Header);

            this.logger.Info(string.Format("Sending message to IOPub {0}", JsonSerializer.Serialize(executeReplyMessage)));
            this.messageSender.Send(executeReplyMessage, ioPub);
        }

        public void SendExecuteInputMessageToIOPub(Message message, PublisherSocket ioPub, string code)
        {
            JObject content = new JObject()
            {
                { "code", code },
                { "execution_count", this.executionCount },
            };

            Message executeInputMessage = MessageBuilder.CreateMessage(MessageTypeValues.ExecuteInput, content, message.Header);

            this.logger.Info(string.Format("Sending message to IOPub {0}", JsonSerializer.Serialize(executeInputMessage)));
			this.messageSender.Send(executeInputMessage, ioPub);
        }

        public void SendExecuteReplyOkMessage(Message message, RouterSocket shellSocket)
        {
            ExecuteReplyOk executeReply = new ExecuteReplyOk()
            {
                ExecutionCount = this.executionCount,
                Payload = new List<Dictionary<string, string>>(),
                UserExpressions = new Dictionary<string, string>()
            };

            Message executeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.ExecuteReply, JObject.FromObject(executeReply), message.Header);

            // Stick the original identifiers on the message so they'll be sent first
            // Necessary since the shell socket is a ROUTER socket
            executeReplyMessage.Identifiers = message.Identifiers;

            this.logger.Info(string.Format("Sending message to Shell {0}", JsonSerializer.Serialize(executeReplyMessage)));
            this.messageSender.Send(executeReplyMessage, shellSocket);
        }

        public void SendExecuteReplyErrorMessage(Message message, RouterSocket shellSocket, JObject errorContent)
        {
            errorContent["status"] = StatusValues.Error;

            Message executeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.ExecuteReply, errorContent, message.Header);

            // Stick the original identifiers on the message so they'll be sent first
            // Necessary since the shell socket is a ROUTER socket
            executeReplyMessage.Identifiers = message.Identifiers;

            this.logger.Info(string.Format("Sending message to Shell {0}", JsonSerializer.Serialize(executeReplyMessage)));
            this.messageSender.Send(executeReplyMessage, shellSocket);
        }
    }
}
