
namespace iCSharp.Kernel.Helpers
{
	using iCSharp.Messages;
	using NetMQ;

	public interface IMessageSender
	{
		bool Send(Message message, NetMQSocket socket);

        bool SendStatus(Message message, NetMQ.Sockets.PublisherSocket ioPub, string status);
    }
}

