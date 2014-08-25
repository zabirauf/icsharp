

namespace iCSharp.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class ExecuteInput
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("execution_count")]
        public int ExecutionCount { get; set; }
    }
}
