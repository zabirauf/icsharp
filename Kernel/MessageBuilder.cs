

namespace iCSharp.Kernel
{
    using System;
    using iCSharp.Messages;

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
                Version = Constants.VERSION
            };

            return newHeader;
        }

        public static Message CreateMessage(string messageType, string content, Header parentHeader)
        {
            string session = parentHeader.Session;

            Message message = new Message()
            {
                UUID = session,
                ParentHeader = parentHeader,
                Header = MessageBuilder.CreateHeader(MessageTypeValues.Status, session),
                Content = content
            };

            return message;
        }
    }
}
