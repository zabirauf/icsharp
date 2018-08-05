
namespace iCSharp.Kernel.Helpers
{
	using Common.Serializer;
	using iCSharp.Messages;
	using NetMQ;
    using NetMQ.Sockets;
    using Newtonsoft.Json.Linq;

    public class MessageSender : IMessageSender
    {
		private readonly ISignatureValidator _signatureValidator;

		public MessageSender(ISignatureValidator signatureValidator)
		{
			this._signatureValidator = signatureValidator;
		}

        public bool Send(Message message, NetMQSocket socket)
        {
			string hmac = this._signatureValidator.CreateSignature (message);

            foreach (var ident in message.Identifiers)
            {
                socket.TrySendFrame(ident, true);
            }

            Send(Constants.DELIMITER, socket);
			Send(hmac, socket);
            Send(JsonSerializer.Serialize(message.Header), socket);
            Send(JsonSerializer.Serialize(message.ParentHeader), socket);
            Send(JsonSerializer.Serialize(message.MetaData), socket);
            Send(JsonSerializer.Serialize(message.Content), socket, false);

            return true;
        }

        private void Send(string message, NetMQSocket socket, bool sendMore = true)
        {
            socket.SendFrame(message, sendMore);
        }

        public bool SendStatus(Message message, PublisherSocket ioPub, string status)
        {
            Status content = new Status
            {
                ExecutionState = status
            };
            Message ioPubMessage = MessageBuilder.CreateMessage(MessageTypeValues.Status, JObject.FromObject(content), message.Header);

            return Send(ioPubMessage, ioPub);
        }
    }
}
