

namespace iCSharp.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class ExecuteReplyError : ExecuteReply
    {
        public ExecuteReplyError()
        {
            this.Status = StatusValues.Error;
        }

        [JsonProperty("ename")]
        public string EName { get; set; }

        [JsonProperty("evalue")]
        public string EValue { get; set; }

        [JsonProperty("traceback")]
        public List<string> Traceback { get; set; }
    }
}
