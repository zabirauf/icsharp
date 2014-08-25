

namespace iCSharp.Kernel.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using iCSharp.Kernel.IOPub;
    using NetMQ;
    using NetMQ.Sockets;
    using System.Threading;

    public class Shell : IServer
    {
        private int port;
        private IOPub ioPub;

        private NetMQContext context;
        private RouterSocket server;

        private ManualResetEventSlim stopEvent;

        private bool disposed;

        public Shell(int port, IOPub ioPub, NetMQContext context)
        {
            this.port = port;
            this.ioPub = ioPub;

            this.context = context;
            this.server = this.context.CreateRouterSocket();
            this.stopEvent = new ManualResetEventSlim();
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
