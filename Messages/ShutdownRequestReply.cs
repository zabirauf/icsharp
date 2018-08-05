using Newtonsoft.Json;

namespace iCSharp.Messages
{
    public class ShutdownRequestReply
    {
        [JsonProperty("restart")]
        public bool Restart { get; set; }
    }
}
