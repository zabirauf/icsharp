

namespace iCSharp.Messages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Message
    {
        public Message()
        {
            this.identifiers = new List<byte[]>();
            this.UUID = string.Empty;
            this.HMac = string.Empty;
            this.MetaData = new Dictionary<string, object>();
            this.Content = string.Empty;
        }

        [JsonProperty("identifiers")]
        public List<byte[]> identifiers { get; set; }

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
        public string Content { get; set; }
    }
}
