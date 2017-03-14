

using Common.Logging;

namespace iCSharp.Kernel.Heartbeat
{
    using NetMQ;
    using NetMQ.Sockets;
    using System.Threading;

    public class Heartbeat : IServer
    {
        private ILog logger;
        private string address;

        private ResponseSocket server;

        private ManualResetEventSlim stopEvent;

        private Thread thread;

        private bool disposed;

        public Heartbeat(ILog logger,  string address)
        {
            this.logger = logger;
            this.address = address;

            this.server = new ResponseSocket();
            this.stopEvent = new ManualResetEventSlim();
        }

        public void Start()
        {
            this.thread = new Thread(this.StartServerLoop);
            this.thread.Start();
            //ThreadPool.QueueUserWorkItem(new WaitCallback(StartServerLoop));
        }

        public void Stop()
        {
            this.stopEvent.Set();
        }

        public ManualResetEventSlim GetWaitEvent()
        {
            return this.stopEvent;
        }

        private void StartServerLoop(object state)
        {
            this.server.Bind(this.address);

            while (!this.stopEvent.Wait(0))
            {
                byte[] data = this.server.ReceiveFrameBytes();

                this.logger.Info(System.Text.Encoding.Default.GetString(data));
                // Echoing back whatever was received
                this.server.TrySendFrame(data);
            }

        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected void Dispose(bool dispose)
        {
            if (!this.disposed)
            {
                if (dispose)
                {
                    if (this.server != null)
                    {
                        this.server.Dispose();
                    }

                    this.disposed = true;
                }
            }
        }
    }
}
