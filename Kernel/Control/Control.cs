

namespace iCSharp.Kernel.Control
{
    using System;
    using NetMQ;
    using NetMQ.Sockets;

    public class Control : IServer
    {
        private int port;

        private NetMQContext context;
        private RouterSocket server;

        private bool disposed;

        public Control(int port, NetMQContext context)
        {
            this.port = port;
            this.context = context;

            this.server = this.context.CreateRouterSocket();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {

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
