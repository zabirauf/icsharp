

namespace iCSharp.Kernel.Heartbeat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NetMQ;
    using NetMQ.Sockets;
    using System.Threading;

    public class Heartbeat : IServer
    {
        private string ip;
        private int port;

        private NetMQContext context;
        private ResponseSocket server;

        private ManualResetEventSlim stopEvent;

        private bool disposed;

        public Heartbeat(string ip, int port, NetMQContext context)
        {
            this.ip = ip;
            this.port = port;
            this.context = context;

            this.server = context.CreateResponseSocket();
            this.stopEvent = new ManualResetEventSlim();
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartServerLoop));
        }

        public void Stop()
        {
            this.stopEvent.Set();
        }

        private void StartServerLoop(object state)
        {
            this.server.Bind(this.GetAddress());

            while (!this.stopEvent.Wait(0))
            {
                byte[] data = this.server.Receive();

                // Echoing back whatever was received
                this.server.Send(data);
            }

        }

        private string GetAddress()
        {
            return string.Format("{0}:{1}", this.ip, this.port);
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
