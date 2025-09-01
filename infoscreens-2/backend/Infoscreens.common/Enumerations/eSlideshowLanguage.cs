using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    /// <summary>
    /// Languages (cultures) supported by the slideshow. 
    /// Its EnumMember returns the culture code based on ISO 369-1 and ISO 3166-a alpha-2 (coutry codes) like "de-CH"
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum eSlideshowLanguage
    {
        [EnumMember(Value = "de-CH")]
        DE_CH,

        [EnumMember(Value = "fr-CH")]
        FR_CH,

        [EnumMember(Value = "it-CH")]
        IT_CH,
        
        [EnumMember(Value = "en-GB")]
        EN_GB
    }
}
