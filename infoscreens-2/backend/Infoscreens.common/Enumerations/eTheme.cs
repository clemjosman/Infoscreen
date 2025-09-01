using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum eTheme
    {
        [EnumMember(Value = "light-default")]
        LightDefault,
        
        [EnumMember(Value = "light-and-dark-blue-footer")]
        LightAndDarkBlueFooter,

        [EnumMember(Value = "uptown-blue-footer")]
        UptownBlue,

        [EnumMember(Value = "robomat-orange")]
        RobomatOrange
    }
}
