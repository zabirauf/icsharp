using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace iCSharp.Messages
{
    public class CompleteRequest
    {
        /* [JsonProperty("code")]
         public string Code { get; set; }

         [JsonProperty("cursor_pos")]
         public int CursorPosition { get; set; }*/

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("cursor_pos")]
        public int CursorPosition { get; set; }

        [JsonProperty("line")]
        public string Line { get; set; }

        [JsonProperty("block")]
        public string Block { get; set; }
    }
}
