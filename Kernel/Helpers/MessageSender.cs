
namespace iCSharp.Kernel.Helpers
{
	using Common.Serializer;
	using iCSharp.Messages;
	using NetMQ;

	public class MessageSender : IMessageSender
    {
        private const string Delimeter = "<IDS|MSG>";

		private readonly ISignatureValidator _signatureValidator;

		public MessageSender(ISignatureValidator signatureValidator)
		{
			this._signatureValidator = signatureValidator;
		}

        public bool Send(Message message, NetMQSocket socket)
        {
			string hmac = this._signatureValidator.CreateSignature (message);

            Send(message.UUID, socket);
            Send(Delimeter, socket);
			Send(hmac, socket);
            Send(JsonSerializer.Serialize(message.Header), socket);
            Send(JsonSerializer.Serialize(message.ParentHeader), socket);
            Send(JsonSerializer.Serialize(message.MetaData), socket);
            Send(message.Content, socket, false);

            return true;
        }

        private void Send(string message, NetMQSocket socket, bool sendMore = true)
        {
            socket.Send(message, false, sendMore);
        }
    }
}
