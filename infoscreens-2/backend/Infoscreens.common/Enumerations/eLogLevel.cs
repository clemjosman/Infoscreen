using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum eLogLevel
    {
        [EnumMember(Value = "trace")]
        Trace,

        [EnumMember(Value = "debug")]
        Debug,

        [EnumMember(Value = "info")]
        Info,

        [EnumMember(Value = "warn")]
        Warn,

        [EnumMember(Value = "error")]
        Error,

        [EnumMember(Value = "fatal")]
        Fatal,

        [EnumMember(Value = "none")]
        None
    }
}
