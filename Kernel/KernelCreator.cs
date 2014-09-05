


using System.Collections.Generic;
using iCSharp.Kernel.ScriptEngine;
using iCSharp.Kernel.Shell;

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
        private ReplEngineFactory _replEngineFactory;

        private IServer _shellServer;
        //private IOPub _ioPub;
        private IServer _heartBeatServer;
        private IShellMessageHandler _kernelInfoRequestHandler;
        private IShellMessageHandler _completeRequestHandler;
        private IShellMessageHandler _executeRequestHandler;
        private IReplEngine _replEngine;

        private Dictionary<string, IShellMessageHandler> _messageHandlerMap; 


        public KernelCreator(ConnectionInformation connectionInformation)
        {
            this._logger = new ConsoleOutLogger("kernel", LogLevel.All, true, true, false, "yyyy/MM/dd HH:mm:ss:fff");
            this._connectionInformation = connectionInformation;
            this._context = NetMQContext.Create();
            this._replEngineFactory = new ReplEngineFactory(this._logger, new string[] {});
        }

        public IServer ShellServer
        {
            get
            {
                if (this._shellServer == null)
                {

                    this._shellServer = new Shell.Shell(this._logger,
                        this.GetAddress(this._connectionInformation.ShellPort),
                        this.GetAddress(this._connectionInformation.IOPubPort), 
                        this._context, 
                        this.MessageHandler);
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

        private IReplEngine ReplEngine
        {
            get
            {
                if (this._replEngine == null)
                {
                    this._replEngine = this._replEngineFactory.ReplEngine;
                }

                return this._replEngine;
            }
        }

        private IShellMessageHandler KernelInfoRequestHandler
        {
            get
            {
                if (this._kernelInfoRequestHandler == null)
                {
                    this._kernelInfoRequestHandler = new KernelInfoRequestHandler(this._logger);
                }

                return this._kernelInfoRequestHandler;
            }
        }

        private IShellMessageHandler CompleteRequestHandler
        {
            get
            {
                if (this._completeRequestHandler == null)
                {
                    this._completeRequestHandler = new CompleteRequestHandler(this._logger);
                }

                return _completeRequestHandler;
            }
        }

        private IShellMessageHandler ExecuteRequestHandler
        {
            get
            {
                if (this._executeRequestHandler == null)
                {
                    this._executeRequestHandler = new ExecuteRequestHandler(this._logger, this.ReplEngine);
                }

                return this._executeRequestHandler;
            }
        }

        private Dictionary<string, IShellMessageHandler> MessageHandler
        {
            get
            {
                if (this._messageHandlerMap == null)
                {
                    this._messageHandlerMap = new Dictionary<string, IShellMessageHandler>();

                    this._messageHandlerMap.Add(MessageTypeValues.KernelInfoRequest, this.KernelInfoRequestHandler);
                    this._messageHandlerMap.Add(MessageTypeValues.CompleteRequest, this.CompleteRequestHandler);
                    this._messageHandlerMap.Add(MessageTypeValues.ExecuteRequest, this.ExecuteRequestHandler);
                }

                return this._messageHandlerMap;
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
