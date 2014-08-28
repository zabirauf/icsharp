using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace iCSharp.Messages
{
    public class CompleteReply
    {
        [JsonProperty("matches")]
        public List<string> Matches { get; set; }

        [JsonProperty("cursor_start")]
        public int CursorStart { get; set; }

        [JsonProperty("cursor_end")]
        public int CursorEnd { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> MetaData { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
