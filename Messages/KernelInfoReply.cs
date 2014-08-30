
namespace iCSharp.Messages
{
    using Newtonsoft.Json;

    public class KernelInfoReply
    {
        [JsonProperty("protocol_version")]
        public string ProtocolVersion { get; set; }

        [JsonProperty("implementation")]
        public string Implementation { get; set; }

        [JsonProperty("implementation_version")]
        public string ImplementationVersion { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("language_version")]
        public string LanguageVersion { get; set; }

        [JsonProperty("banner")]
        public string Banner { get; set; }

        [JsonProperty("ipython_version")]
        public string IPythonVersion { get; set; }
    }
}
