

namespace iCSharp.Messages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class Message
    {
        public Message()
        {
            this.Identifiers = new List<byte[]>();
            this.Signature = string.Empty;
            this.MetaData = new JObject();
            this.Content = new JObject();
            this.Buffers = new List<byte[]>();
        }

        [JsonIgnoreAttribute]
        public List<byte[]> Identifiers { get; set; }

        [JsonIgnoreAttribute]
        public string Signature { get; set; }

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("parent_header")]
        public Header ParentHeader { get; set; }

        [JsonProperty("metadata")]
        public JObject MetaData { get; set; }

        [JsonProperty("content")]
        public JObject Content { get; set; }

        [JsonProperty("buffers")]
        public List<byte[]> Buffers { get; set; }
    }
}
