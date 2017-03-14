
namespace iCSharp.Kernel.Helpers
{
	using Common.Serializer;
	using iCSharp.Messages;
	using NetMQ;

	public class MessageSender : IMessageSender
    {
        private const string Delimiter = "<IDS|MSG>";

		private readonly ISignatureValidator _signatureValidator;

		public MessageSender(ISignatureValidator signatureValidator)
		{
			this._signatureValidator = signatureValidator;
		}

        public bool Send(Message message, NetMQSocket socket)
        {
			string hmac = this._signatureValidator.CreateSignature (message);

            if (message.identifiers.Count > 0) {
                // Send ZMQ identifiers from the message we're responding to.
                // This is important when we're dealing with ROUTER sockets, like the shell socket,
                // because the message won't be sent unless we manually include these.
                foreach (var ident in message.identifiers) {
                    socket.TrySendFrame(ident, true);
                }
            } else {
                // This is just a normal message so send the UUID
                Send(message.UUID, socket);
            }

            Send(Delimiter, socket);
			Send(hmac, socket);
            Send(JsonSerializer.Serialize(message.Header), socket);
            Send(JsonSerializer.Serialize(message.ParentHeader), socket);
            Send(JsonSerializer.Serialize(message.MetaData), socket);
            Send(message.Content, socket, false);

            return true;
        }

        private void Send(string message, NetMQSocket socket, bool sendMore = true)
        {
			socket.SendFrame(message, sendMore);
        }
    }
}
