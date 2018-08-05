

namespace iCSharp.Kernel.Helpers
{
    using System;
    using iCSharp.Messages;
    using Newtonsoft.Json.Linq;

    public class MessageBuilder
    {
        public static Header CreateHeader(string messageType, string session)
        {
            Header newHeader = new Header()
            {
                Username = Constants.USERNAME,
                Session = session,
                MessageId = Guid.NewGuid().ToString(),
                MessageType = messageType,
                Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Version = "5.3"
            };

            return newHeader;
        }

        public static Message CreateMessage(string messageType, JObject content, Header parentHeader)
        {
            string session = parentHeader.Session;

            Message message = new Message()
            {
                ParentHeader = parentHeader,
                Header = MessageBuilder.CreateHeader(messageType, session),
                Content = content
            };

            return message;
        }
    }
}
