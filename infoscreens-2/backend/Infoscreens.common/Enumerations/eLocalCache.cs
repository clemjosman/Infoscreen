using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum eLocalCache
    {
        [EnumMember(Value = "config")]
        Config,

        [EnumMember(Value = "customjoboffer")]
        CustomJobOffer,

        [EnumMember(Value = "ideabox")]
        Ideabox,

        [EnumMember(Value = "infoscreenmonitoring")]
        InfoscreenMonitoring,

        [EnumMember(Value = "joboffers")]
        JobOffers,

        [EnumMember(Value = "newsinternal")]
        NewsInternal,

        [EnumMember(Value = "newspublic")]
        NewsPublic,

        [EnumMember(Value = "openweather")]
        OpenWeather,

        [EnumMember(Value = "sociabble")]
        Sociabble,

        [EnumMember(Value = "publictransport")]
        PublicTransport,

        [EnumMember(Value = "twentymin")]
        TwentyMin,

        [EnumMember(Value = "twitter")]
        Twitter,

        [EnumMember(Value = "university")]
        University,

        [EnumMember(Value = "uptownarticle")]
        UptownArticle,

        [EnumMember(Value = "uptownevent")]
        UptownEvent,

        [EnumMember(Value = "uptownmenu")]
        UptownMenu,
    }
}
