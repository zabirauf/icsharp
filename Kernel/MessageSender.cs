using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Serializer;
using iCSharp.Messages;
using NetMQ;

namespace iCSharp.Kernel
{
    public class MessageSender
    {
        private const string Delimeter = "<IDS|MSG>";
        public static bool Send(Message message, NetMQSocket socket)
        {
            Send(message.UUID, socket);
            Send(Delimeter, socket);
            Send(string.Empty, socket);
            Send(JsonSerializer.Serialize(message.Header), socket);
            Send(JsonSerializer.Serialize(message.ParentHeader), socket);
            Send(JsonSerializer.Serialize(message.MetaData), socket);
            Send(message.Content, socket, false);

            return true;
        }

        private static void Send(string message, NetMQSocket socket, bool sendMore = true)
        {
            socket.Send(message, false, sendMore);
        }
    }
}
