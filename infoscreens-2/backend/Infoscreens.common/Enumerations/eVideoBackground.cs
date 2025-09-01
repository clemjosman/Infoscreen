using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum eVideoBackground
    {
        [EnumMember(Value = "blue-dot-mesh")]
        BlueDotMesh
    }
}
