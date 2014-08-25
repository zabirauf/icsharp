

namespace iCSharp.Kernel.IOPub
{
    using System;
    using NetMQ;
    using NetMQ.Sockets;

    public class IOPub : IDisposable
    {
        private int port;

        private NetMQContext context;

        private PublisherSocket server;

        private bool disposed;

        public IOPub(int port, NetMQContext context)
        {
            this.port = port;
            this.context = context;

            this.server = context.CreatePublisherSocket();
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
