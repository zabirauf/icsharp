
namespace iCSharp.Messages
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class KernelInfoReply
    {
        [JsonProperty("protocol_version")]
        public string ProtocolVersion { get; set; }

        [JsonProperty("implementation")]
        public string Implementation { get; set; }

        [JsonProperty("implementation_version")]
        public string ImplementationVersion { get; set; }

        [JsonProperty("language_info")]
        public JObject LanguageInfo { get; set; }

        [JsonProperty("banner")]
        public string Banner { get; set; }
    }
}
