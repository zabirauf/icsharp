

using System.Threading;

namespace iCSharp.Kernel.Control
{
    using System;
    using NetMQ;
    using NetMQ.Sockets;

	public class Control : IServer
    {
        private int port;

        private RouterSocket server;

        private bool disposed;

        public Control(int port)
        {
            this.port = port;
            this.server = new RouterSocket();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {

        }

        public ManualResetEventSlim GetWaitEvent()
        {
            throw new NotImplementedException();
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
