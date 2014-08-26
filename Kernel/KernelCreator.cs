


namespace iCSharp.Kernel
{
    using Common.Logging;
    using Common.Logging.Simple;
    using iCSharp.Messages;
    using NetMQ;


    public class KernelCreator
    {
        private ILog _logger;

        private ConnectionInformation _connectionInformation;
        private NetMQContext _context;

        private IServer _shellServer;
        //private IOPub _ioPub;
        private IServer _heartBeatServer;


        public KernelCreator(ConnectionInformation connectionInformation)
        {
            this._logger = new ConsoleOutLogger("kernel", LogLevel.All, true, true, false, "yyyy/MM/dd HH:mm:ss:fff");
            this._connectionInformation = connectionInformation;
            this._context = NetMQContext.Create();
        }

        public IServer ShellServer
        {
            get
            {
                if (this._shellServer == null)
                {
                    
                    this._shellServer = new Shell.Shell(this._logger, this.GetAddress(this._connectionInformation.ShellPort), null, this._context);
                }

                return this._shellServer;
            }
        }

        public IServer HeartBeatServer
        {
            get
            {
                if (this._heartBeatServer == null)
                {
                    this._heartBeatServer = new Heartbeat.Heartbeat(this._logger,
                        this.GetAddress(this._connectionInformation.HBPort), this._context);
                }

                return this._heartBeatServer;
            }
        }

        private string GetAddress(int port)
        {
            string address = string.Format("{0}://{1}:{2}", this._connectionInformation.Transport,
                        this._connectionInformation.IP, port);

            return address;
        }
    }
}
