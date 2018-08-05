
namespace iCSharp.Kernel.Shell
{
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Threading;
    using Common.Logging;
    using Common.Serializer;
    using iCSharp.Kernel.Helpers;
    using iCSharp.Messages;
    using NetMQ;
	using NetMQ.Sockets;
    using Newtonsoft.Json.Linq;

    public class Shell : IServer
    {
        private ILog logger;
        private string addressShell;
        private string addressIOPub;

        private ISignatureValidator signatureValidator;
        private RouterSocket server;
        private PublisherSocket ioPubSocket;

        private ManualResetEventSlim stopEvent;

        private Thread thread;
        private bool disposed;

        private Dictionary<string, IShellMessageHandler> messageHandlers;

        public Shell(ILog logger,
                     string addressShell,
                     string addressIOPub,
                     ISignatureValidator signatureValidator,
                     Dictionary<string, IShellMessageHandler> messageHandlers)
        {
            this.logger = logger;
            this.addressShell = addressShell;
            this.addressIOPub = addressIOPub;
            this.signatureValidator = signatureValidator;
            this.messageHandlers = messageHandlers;

            this.server = new RouterSocket();
            this.ioPubSocket = new PublisherSocket();
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

            // There may be additional ZMQ identities attached; read until the delimiter <IDS|MSG>"
            // and store them in message.identifiers
            // http://ipython.org/ipython-doc/dev/development/messaging.html#the-wire-protocol
            byte[] delimAsBytes = Encoding.ASCII.GetBytes(Constants.DELIMITER);
            byte[] delim;
            while (true) {
                delim = this.server.ReceiveFrameBytes();
                if (delim.SequenceEqual(delimAsBytes)) break;

                message.Identifiers.Add(delim);
            }

            // Getting Hmac
            message.Signature = this.server.ReceiveFrameString();
            this.logger.Info(message.Signature);

            // Getting Header
            string header = this.server.ReceiveFrameString();
            this.logger.Info(header);

            message.Header = JsonSerializer.Deserialize<Header>(header);

            // Getting parent header
            string parentHeader = this.server.ReceiveFrameString();
            this.logger.Info(parentHeader);

            message.ParentHeader = JsonSerializer.Deserialize<Header>(parentHeader);

            // Getting metadata
            string metadata = this.server.ReceiveFrameString();
            this.logger.Info(metadata);

            message.MetaData = JObject.Parse(metadata);

            // Getting content
            string content = this.server.ReceiveFrameString();
            this.logger.Info(content);

            message.Content = JObject.Parse(content);

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
