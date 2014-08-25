

namespace iCSharp.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class Header
    {
        [JsonProperty("msg_id")]
        public string MessageId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }

        [JsonProperty("msg_type")]
        public string MessageType { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
