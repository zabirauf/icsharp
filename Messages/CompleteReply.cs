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
       /* [JsonProperty("matches")]
          public List<string> Matches { get; set; }

          [JsonProperty("cursor_start")]
          public int CursorStart { get; set; }

          [JsonProperty("cursor_end")]
          public int CursorEnd { get; set; }

          [JsonProperty("metadata")]
          public Dictionary<string, object> MetaData { get; set; }

          [JsonProperty("status")]
          public string Status { get; set; }*/

       [JsonProperty("matches")]
        public List<CompleteReplyMatch> Matches { get; set; }

        [JsonProperty("matched_text")]
        public string MatchedText { get; set; }

       [JsonProperty("filter_start_index")]
        public int FilterStartIndex { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } 
    }
}
