using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum eDateFormat
    {
        [EnumMember(Value = "long")]
        Long,
        
        [EnumMember(Value = "short")]
        Short
    }
}
