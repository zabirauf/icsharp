using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace iCSharp.Messages
{
    public class VariableMatch
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public Type VarType { get; set; }  
    }
}
