

namespace iCSharp.Messages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class DisplayData
    {
        public DisplayData()
        {
            this.Source = string.Empty;
            this.Data = new JObject();
            this.MetaData = new JObject();
        }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("data")]
        public JObject Data { get; set; }

        [JsonProperty("metadata")]
        public JObject MetaData { get; set; }
    }
}
