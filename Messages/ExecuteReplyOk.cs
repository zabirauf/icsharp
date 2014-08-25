

namespace iCSharp.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class ExecuteReplyOk : ExecuteReply
    {
        public ExecuteReplyOk()
        {
            this.Status = StatusValues.Ok;
        }

        [JsonProperty("payload")]
        public List<Dictionary<string,string>> Payload { get; set; }

        [JsonProperty("user_expressions")]
        public Dictionary<string,string> UserExpressions { get; set; }
    }
}
