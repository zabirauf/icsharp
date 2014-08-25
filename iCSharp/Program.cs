

namespace iCSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using iCSharp.Messages;

    public class Program
    {
        public static void Main(string[] args)
        {
            string connInfo = @"{
              ""control_port"": 50160,
              ""shell_port"": 57503,
              ""transport"": ""tcp"",
              ""signature_scheme"": ""hmac-sha256"",
              ""stdin_port"": 52597,
              ""hb_port"": 42540,
              ""ip"": ""127.0.0.1"",
              ""iopub_port"": 40885,
              ""key"": ""a0436f6c-1916-498b-8eb9-e81ab9368e84""
            }";

            ConnectionInformation connectionInformation = JsonConvert.DeserializeObject<ConnectionInformation>(connInfo);


            Dictionary<string, string> a = new Dictionary<string, string>();
            a.Add("abc", "def");
            string s = JsonConvert.SerializeObject(a);

            Console.WriteLine(s);
        }
    }
}
