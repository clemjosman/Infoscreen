using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum eBranding
    {
        [EnumMember(Value = "ActemiumCH")]
        ActemiumCH,
        
        [EnumMember(Value = "AxiansCH")]
        AxiansCH,

        [EnumMember(Value = "EtavisCH")]
        EtavisCH,

        [EnumMember(Value = "SitecCH")]
        SitecCH,
    }
}
