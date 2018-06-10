using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace iCSharp.Messages
{
    public class MethodMatch
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("documentation")]
        public string Documentation { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

		[JsonProperty("paramlist")]
		public List<string> ParamList { get; set; }

		[JsonProperty("access")]
        public string Access { get; set; }





    }
}
