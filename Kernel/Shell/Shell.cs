
namespace iCSharp.Kernel.Shell
{
    using System.Collections.Generic;
    using System.Threading;
    using Common.Logging;
    using Common.Serializer;
	using iCSharp.Kernel.Helpers;
	using iCSharp.Messages;
	using NetMQ;
	using NetMQ.Sockets;

    public class Shell : IServer
    {
        private ILog logger;
        private string addressShell;
        private string addressIOPub;

        private NetMQContext context;
		private ISignatureValidator signatureValidator;
        private RouterSocket server;
        private PublisherSocket ioPubSocket;

        private ManualResetEventSlim stopEvent;

        private Thread thread;
        private bool disposed;

        private Dictionary<string, IShellMessageHandler> messageHandlers; 

        public Shell(
			ILog logger,
			string addressShell, 
			string addressIOPub, 
			NetMQContext context, 
			ISignatureValidator signatureValidator,
			Dictionary<string, IShellMessageHandler> messageHandlers)
        {
            this.logger = logger;
            this.addressShell = addressShell;
            this.addressIOPub = addressIOPub;
			this.signatureValidator = signatureValidator;
            this.context = context;
            this.messageHandlers = messageHandlers;

            this.server = this.context.CreateRouterSocket();
            this.ioPubSocket = this.context.CreatePublisherSocket();
            this.stopEvent = new ManualResetEventSlim();
        }

        public void Start()
        {
            this.thread = new Thread(this.StartServerLoop);
            this.thread.Start();

            this.logger.Info("Shell Started");
            //ThreadPool.QueueUserWorkItem(new WaitCallback(StartServerLoop));
        }

        private void StartServerLoop(object state)
        {
            this.server.Bind(this.addressShell);
            this.logger.Info(string.Format("Binded the Shell server to address {0}", this.addressShell));

            this.ioPubSocket.Bind(this.addressIOPub);
            this.logger.Info(string.Format("Binded the  IOPub to address {0}", this.addressIOPub));

            while (!this.stopEvent.Wait(0))
            {
                Message message = this.GetMessage();

                this.logger.Info(JsonSerializer.Serialize(message));

                IShellMessageHandler handler;
                if (this.messageHandlers.TryGetValue(message.Header.MessageType, out handler))
                {
                    this.logger.Info(string.Format("Sending message to handler {0}", message.Header.MessageType));
                    handler.HandleMessage(message, this.server, this.ioPubSocket);
                    this.logger.Info("Message handling complete");
                }
                else
                {
                    this.logger.Error(string.Format("No message handler found for message type {0}",
                        message.Header.MessageType));
                }
            }
        }

        private Message GetMessage()
        {
            Message message = new Message();

            // Getting UUID
            message.UUID = this.server.ReceiveString();
            this.logger.Info(message.UUID);

            // Getting Delimeter "<IDS|MSG>"
            this.server.ReceiveString();

            // Getting Hmac
            message.HMac = this.server.ReceiveString();
            this.logger.Info(message.HMac);

            // Getting Header
            string header = this.server.ReceiveString();
            this.logger.Info(header);

            message.Header = JsonSerializer.Deserialize<Header>(header);

            // Getting parent header
            string parentHeader = this.server.ReceiveString();
            this.logger.Info(parentHeader);

            message.ParentHeader = JsonSerializer.Deserialize<Header>(parentHeader);

            // Getting metadata
            string metadata = this.server.ReceiveString();
            this.logger.Info(metadata);

            message.MetaData = JsonSerializer.Deserialize<Dictionary<string, object>>(metadata);

            // Getting content
            string content = this.server.ReceiveString();
            this.logger.Info(content);

            message.Content = content;

            return message;
        }


        public void Stop()
        {
            this.stopEvent.Set();
        }

        public ManualResetEventSlim GetWaitEvent()
        {
            return this.stopEvent;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected void Dispose(bool dispose)
        {
            if(!this.disposed)
            {
                if(dispose)
                {
                    if(this.server != null)
                    {
                        this.server.Dispose();
                    }

                    if (this.ioPubSocket != null)
                    {
                        this.ioPubSocket.Dispose();
                    }

                    this.disposed = true;
                }
            }
        }
    }
}
