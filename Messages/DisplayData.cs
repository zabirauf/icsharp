

namespace iCSharp.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
