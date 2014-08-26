

namespace iCSharp.Kernel.Shell
{
    using iCSharp.Kernel.IOPub;
    using NetMQ;
    using NetMQ.Sockets;
    using System.Threading;
    using Common.Logging;

    public class Shell : IServer
    {
        private ILog logger;
        private string address;
        private IOPub ioPub;

        private NetMQContext context;
        private RouterSocket server;

        private ManualResetEventSlim stopEvent;

        private bool disposed;

        public Shell(ILog logger,string address, IOPub ioPub, NetMQContext context)
        {
            this.logger = logger;
            this.address = address;
            this.ioPub = ioPub;

            this.context = context;
            this.server = this.context.CreateRouterSocket();
            this.stopEvent = new ManualResetEventSlim();
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartServerLoop));
        }

        private void StartServerLoop(object state)
        {
            this.server.Bind(this.address);

            while (this.stopEvent.Wait(0))
            {
                string data = this.server.ReceiveString();

                this.logger.Info(data);
            }
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

                    this.disposed = true;
                }
            }
        }
    }
}
