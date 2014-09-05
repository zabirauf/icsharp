


using System.Collections.Generic;
using System.Text;
using System.Threading;
using iCSharp.Kernel.ScriptEngine;

namespace iCSharp.Kernel.Shell
{
    using Common.Logging;
    using Common.Serializer;
    using iCSharp.Messages;
    using NetMQ.Sockets;

    public class ExecuteRequestHandler : IShellMessageHandler
    {
        private ILog logger;

        private int executionCount = 1;

        private IReplEngine replEngine; 

        public ExecuteRequestHandler(ILog logger, IReplEngine replEngine)
        {
            this.logger = logger;
            this.replEngine = replEngine;
        }

        public void HandleMessage(Message message, RouterSocket serverSocket, PublisherSocket ioPub)
        {
            this.logger.Debug(string.Format("Message Content {0}", message.Content));
            ExecuteRequest executeRequest = JsonSerializer.Deserialize<ExecuteRequest>(message.Content);

            this.logger.Info(string.Format("Execute Request received with code {0}", executeRequest.Code));

            // 1: Send Busy status on IOPub
            this.SendMessageToIOPub(message, ioPub, StatusValues.Busy);

            // 2: Send execute input on IOPub
            this.SendInputMessageToIOPub(message, ioPub, executeRequest.Code);

            // 3: Evaluate the C# code
            string code = executeRequest.Code;
            ExecutionResult results = this.replEngine.Execute(code);
            string codeOutput = this.GetCodeOutput(results);
            
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"text/plain", codeOutput},
                {"text/html", codeOutput}
            };

            DisplayData displayData = new DisplayData()
            {
                Data = data,
            };

            // 4: Send execute reply to shell socket
            this.SendExecuteReplyMessage(message, serverSocket);

            // 5: Send execute result message to IOPub
            this.SendOutputMessageToIOPub(message, ioPub, displayData);

            // 6: Send IDLE status message to IOPub
            this.SendMessageToIOPub(message, ioPub, StatusValues.Idle);

            this.executionCount += 1;

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

        public void SendMessageToIOPub(Message message, PublisherSocket ioPub, string statusValue)
        {
            Dictionary<string,string> content = new Dictionary<string, string>();
            content.Add("execution_state", statusValue);
            Message ioPubMessage = MessageBuilder.CreateMessage(MessageTypeValues.Status,
                JsonSerializer.Serialize(content), message.Header);

            this.logger.Info(string.Format("Sending message to IOPub {0}", JsonSerializer.Serialize(ioPubMessage)));
            MessageSender.Send(ioPubMessage, ioPub);
            this.logger.Info("Message Sent");
        }

        public void SendOutputMessageToIOPub(Message message, PublisherSocket ioPub, DisplayData data)
        {
            Dictionary<string,object> content = new Dictionary<string, object>();
            content.Add("execution_count", this.executionCount);
            content.Add("data", data.Data);
            content.Add("metadata", data.MetaData);

            Message outputMessage = MessageBuilder.CreateMessage(MessageTypeValues.Output,
                JsonSerializer.Serialize(content), message.Header);

            this.logger.Info(string.Format("Sending message to IOPub {0}", JsonSerializer.Serialize(outputMessage)));
            MessageSender.Send(outputMessage, ioPub);
        }

        public void SendInputMessageToIOPub(Message message, PublisherSocket ioPub, string code)
        {
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("execution_count", 1);
            content.Add("code", code);

            Message executeInputMessage = MessageBuilder.CreateMessage(MessageTypeValues.Input, JsonSerializer.Serialize(content),
                message.Header);

            this.logger.Info(string.Format("Sending message to IOPub {0}", JsonSerializer.Serialize(executeInputMessage)));
            MessageSender.Send(executeInputMessage, ioPub);
        }

        public void SendExecuteReplyMessage(Message message, RouterSocket shellSocket)
        {
            ExecuteReplyOk executeReply = new ExecuteReplyOk()
            {
                ExecutionCount = this.executionCount,
                Payload = new List<Dictionary<string, string>>(),
                UserExpressions = new Dictionary<string, string>()
            };

            Message executeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.ExecuteReply,
                JsonSerializer.Serialize(executeReply), message.Header);

            this.logger.Info(string.Format("Sending message to Shell {0}", JsonSerializer.Serialize(executeReplyMessage)));
            MessageSender.Send(executeReplyMessage, shellSocket);
        }
    }
}
