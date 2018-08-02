using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Common.Logging;
using Common.Serializer;
using iCSharp.Kernel.Helpers;
using iCSharp.Messages;
using NetMQ.Sockets;

namespace iCSharp.Kernel.ScriptEngine
{
    public class IOPubConsole : TextWriter 
    {
        private readonly Message _message;
        private readonly PublisherSocket _ioPub;
        private readonly IMessageSender _messageSender;
        private readonly int _executionCount;
        private readonly ILog _logger;
        private TextWriter _originalConsoleOut;
        private TextWriter _originalConsoleError;

        public IOPubConsole(Message message, PublisherSocket ioPub, IMessageSender messageSender, int executionCount, ILog logger)
        {
            _message = message;
            _ioPub = ioPub;
            _messageSender = messageSender;
            _executionCount = executionCount;
            _logger = logger;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void WriteLine(string value)
        {
            this.SendOutputMessageToIOPub(value);
#if DEBUG
            _originalConsoleOut.WriteLine(value);
#endif
        }

        public override void WriteLine(object value)
        {
            this.SendOutputMessageToIOPub(value.ToString());
#if DEBUG
            _originalConsoleOut.WriteLine(value);
#endif
        }

        public override void Write(string value)
        {
            this.SendOutputMessageToIOPub(value);
#if DEBUG
            _originalConsoleOut.Write(value);
#endif
        }

        public override void Write(object value)
        {
            this.SendOutputMessageToIOPub(value.ToString());
#if DEBUG
            _originalConsoleOut.Write(value);
#endif
        }

        public void RedirectConsole()
        {
            _originalConsoleOut = Console.Out;
            _originalConsoleError = Console.Error;

            Console.SetOut(this);
            Console.SetError(this);
        }

        public void CancelRedirect()
        {
            Console.SetOut(_originalConsoleOut);
            Console.SetError(_originalConsoleError);
        }

        private void SendOutputMessageToIOPub(string value)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"text/plain", value},
                {"text/html",  HttpUtility.HtmlEncode(value)}
            };

            DisplayData displayData = new DisplayData()
            {
                Data = data,
            };

            Dictionary<string, object> content = new Dictionary<string, object>
            {
                {"execution_count", this._executionCount},
                {"data", displayData.Data},
                {"metadata", displayData.MetaData}
            };

            Message outputMessage = MessageBuilder.CreateMessage(MessageTypeValues.Output,
                JsonSerializer.Serialize(content), _message.Header);

            this._logger.Info(string.Format("Sending message to IOPub {0}", JsonSerializer.Serialize(outputMessage)));
            this._messageSender.Send(outputMessage, _ioPub);
        }
    }
}