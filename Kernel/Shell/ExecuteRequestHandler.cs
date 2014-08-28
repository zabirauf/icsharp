


using System.Collections.Generic;
using System.Threading;

namespace iCSharp.Kernel.Shell
{
    using Common.Logging;
    using Common.Serializer;
    using iCSharp.Messages;
    using NetMQ.Sockets;

    public class ExecuteRequestHandler : IShellMessageHandler
    {
        private ILog logger;

        private int executionCount = 0;

        public ExecuteRequestHandler(ILog logger)
        {
            this.logger = logger;
        }

        public void HandleMessage(Message message, RouterSocket serverSocket, PublisherSocket ioPub)
        {
            this.logger.Debug(string.Format("Message Content {0}", message.Content));
            ExecuteRequest executeRequest = JsonSerializer.Deserialize<ExecuteRequest>(message.Content);

            this.logger.Info(string.Format("Execute Request received with code {0}", executeRequest.Code));

            // 1: Send Busy status on IOPub
            this.SendMessageToIOPub(message, ioPub, StatusValues.Busy);

            // 2: Send execute input on IOPub
            this.SendExecuteInputMessageToIOPub(message, ioPub, executeRequest.Code);

            // 3: Evaluate the C# code
            // TODO: Compile message
            string codeOutput = "Hello World";
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"text/plain", codeOutput}
            };

            DisplayData displayData = new DisplayData()
            {
                Data = data,
            };

            // 4: Send execute reply to shell socket
            this.SendExecuteReplyMessage(message, serverSocket);

            // 5: Send execute result message to IOPub
            this.SendExecuteResultMessageToIOPub(message, ioPub, displayData);

            // 6: Send IDLE status message to IOPub
            this.SendMessageToIOPub(message, ioPub, StatusValues.Idle);

            this.executionCount += 1;

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

        public void SendExecuteResultMessageToIOPub(Message message, PublisherSocket ioPub, DisplayData data)
        {
            Dictionary<string,object> content = new Dictionary<string, object>();
            content.Add("execution_count", this.executionCount);
            content.Add("data", JsonSerializer.Serialize(data.Data));
            content.Add("metadata", JsonSerializer.Serialize(data.MetaData));

            Message outputMessage = MessageBuilder.CreateMessage(MessageTypeValues.ExecuteResult,
                JsonSerializer.Serialize(content), message.Header);

            this.logger.Info(string.Format("Sending message to IOPub {0}", JsonSerializer.Serialize(outputMessage)));
            MessageSender.Send(outputMessage, ioPub);
        }

        public void SendExecuteInputMessageToIOPub(Message message, PublisherSocket ioPub, string code)
        {
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("execution_count", 1);
            content.Add("code", code);

            Message executeInputMessage = MessageBuilder.CreateMessage(MessageTypeValues.ExecuteInput, JsonSerializer.Serialize(content),
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
