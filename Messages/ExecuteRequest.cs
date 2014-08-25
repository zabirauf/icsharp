

namespace iCSharp.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class ExecuteRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("silent")]
        public bool Silent { get; set; }

        [JsonProperty("store_history")]
        public bool StoreHistory { get; set; }

        [JsonProperty("user_expressions")]
        public Dictionary<string,string> UserExpressions { get; set; }

        [JsonProperty("allow_stdin")]
        public bool AllowStdin { get; set; }
    }
}
