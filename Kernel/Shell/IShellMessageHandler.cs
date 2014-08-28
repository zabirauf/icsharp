



namespace iCSharp.Kernel.Shell
{
    using iCSharp.Messages;
    using NetMQ.Sockets;
    using iCSharp.Kernel.IOPub;

    public interface IShellMessageHandler
    {
        void HandleMessage(Message message, RouterSocket serverSocket, IOPub ioPub);
    }
}
