

namespace iCSharp.Messages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Message
    {
        [JsonProperty("uuid")]
        public string UUID { get; set; }

        [JsonProperty("hmac")]
        public string HMac { get; set; }

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("parent_header")]
        public Header ParentHeader { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> MetaData { get; set; }

        [JsonProperty("content")]
        public Dictionary<string, object> Content { get; set; }
    }
}
