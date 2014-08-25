

namespace iCSharp.Kernel.Stdin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NetMQ;
    using NetMQ.Sockets;

    public class Stdin : IDisposable
    {
        private int port;

        private NetMQContext context;
        private RouterSocket server;

        private bool disposed;

        public Stdin(int port, NetMQContext context)
        {
            this.port = port;
            this.context = context;

            this.server = this.context.CreateRouterSocket();
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
