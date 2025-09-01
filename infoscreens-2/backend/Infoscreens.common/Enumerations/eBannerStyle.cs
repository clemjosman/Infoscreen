using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum eBannerStyle
    {
        [EnumMember(Value = "left")]
        Left,
        
        [EnumMember(Value = "top")]
        Top,

        [EnumMember(Value = "none")]
        None
    }
}
