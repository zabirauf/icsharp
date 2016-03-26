
namespace iCSharp.Kernel.Helpers
{
	using iCSharp.Messages;
	using NetMQ;

	public interface IMessageSender
	{
		bool Send(Message message, NetMQSocket socket);
	}
}

