



namespace iCSharp.Kernel.Shell
{
    using iCSharp.Messages;
    using NetMQ.Sockets;

    public interface IShellMessageHandler
    {
        void HandleMessage(Message message, RouterSocket serverSocket, PublisherSocket ioPub);
    }
}
