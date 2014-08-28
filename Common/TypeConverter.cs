using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Serializer;

namespace Common
{
    public class TypeConverter
    {

        public static T Convert<T>(Dictionary<string, object> obj)
        {
            string serialized = JsonSerializer.Serialize(obj);
            T convertedObj = JsonSerializer.Deserialize<T>(serialized);

            return convertedObj;
        }
    
    }
}
