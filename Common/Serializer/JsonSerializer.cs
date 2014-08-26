
using System.Net.Mail;

namespace Common.Serializer
{
    using Newtonsoft.Json;

    public static class JsonSerializer
    {

        public static T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string Serizlize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

    }
}
