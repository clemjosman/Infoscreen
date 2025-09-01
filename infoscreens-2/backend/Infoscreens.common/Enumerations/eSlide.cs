using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum eSlide
    {
        [EnumMember(Value = "coffee")]
        Coffee,

        [EnumMember(Value = "customjoboffer")]
        CustomJobOffer,

        [EnumMember(Value = "ideabox")]
        Ideabox,

        [EnumMember(Value = "iframe")]
        Iframe,

        [EnumMember(Value = "infoscreenmonitoring")]
        InfoscreenMonitoring,

        [EnumMember(Value = "joboffers")]
        JobOffers,

        [EnumMember(Value = "localvideo")]
        LocalVideo,

        // Added maintenance here for consistancy with front but should not be used in the node config file!
        [EnumMember(Value = "maintenance")]
        Maintenance,

        [EnumMember(Value = "monitoringiframe")]
        MonitoringIframe,

        [EnumMember(Value ="newsinternal")]
        NewsInternal,

        [EnumMember(Value = "newspublic")]
        NewsPublic,

        [EnumMember(Value = "publictransport")]
        PublicTransport,

        [EnumMember(Value = "sociabble")]
        Sociabble,

        [EnumMember(Value = "spotlight")]
        Spotlight,

        [EnumMember(Value = "stock")]
        Stock,

        [EnumMember(Value = "traffic")]
        Traffic,

        [EnumMember(Value = "twentymin")]
        TwentyMin,

        [EnumMember(Value = "twitter")]
        Twitter,

        [EnumMember(Value = "university")]
        University,

        [EnumMember(Value = "universityoverview")]
        UniversityOverview,

        [EnumMember(Value = "uptownevent")]
        UptownEvent,

        [EnumMember(Value = "uptownarticle")]
        UptownArticle,

        [EnumMember(Value = "uptownmenu")]
        UptownMenu,

        [EnumMember(Value = "weatherweekly")]
        WeatherWeekly,

        [EnumMember(Value = "weatherdaily")]
        WeatherDaily,

        [EnumMember(Value = "youtube")]
        Youtube,
    }
}
