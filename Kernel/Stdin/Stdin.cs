

namespace iCSharp.Kernel.Stdin
{
    using System;
    using NetMQ;
    using NetMQ.Sockets;

    public class Stdin : IDisposable
    {
        private int port;

        private RouterSocket server;

        private bool disposed;

		public Stdin(int port)
        {
            this.port = port;

            this.server = new RouterSocket();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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
