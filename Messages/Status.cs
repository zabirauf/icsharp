

namespace iCSharp.Messages
{
    using Newtonsoft.Json;

    public class Status
    {
        [JsonProperty("execution_state")]
        public string ExecutionState { get; set; }
    }
}
