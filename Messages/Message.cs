

namespace iCSharp.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class Message
    {
        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("parent_header")]
        public Header ParentHeader { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, string> MetaData { get; set; }

        [JsonProperty("content")]
        public Dictionary<string, string> Content { get; set; }
    }
}
