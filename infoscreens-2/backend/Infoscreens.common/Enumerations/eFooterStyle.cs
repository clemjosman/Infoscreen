using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum eFooterStyle
    {
        [EnumMember(Value = "rollingText")]
        RollingText,
        
        [EnumMember(Value = "duoBranding")]
        DuoBranding
    }
}
