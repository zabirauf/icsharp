

namespace iCSharp.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class Status
    {
        [JsonProperty("execution_state")]
        public string ExecutionState { get; set; }
    }
}
