

namespace iCSharp.Messages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class DisplayData
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("data")]
        public Dictionary<string,string> Data { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string,string> MetaData { get; set; }
    }
}
