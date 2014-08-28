

namespace iCSharp.Messages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class DisplayData
    {
        public DisplayData()
        {
            this.Source = string.Empty;
            this.Data = new Dictionary<string, string>();
            this.MetaData = new Dictionary<string, string>();
        }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("data")]
        public Dictionary<string,string> Data { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string,string> MetaData { get; set; }
    }
}
