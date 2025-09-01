using Infoscreens.Common.Enumerations.Attributes;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Enumerations
{
    public enum eApi
    {

        // Files are searched locally in the blob storage, no need to define request parameters
        [EnumMember(Value = "customJobOffer")]
        CustomJobOffer,

        [RootUrl("https://cluster.actemium.ch/ideabox/api/Ideas/v1/Latest")]
        [QueryParam("Number","10")]
        [HeaderParam("authorization", "apikey Ay5A7CaFcHeMhPkSpUrWuZw3y6B8DaGdJfNjQmSq")]
        [EnumMember(Value = "ideabox")]
        Ideabox,
        
        [RootUrl("https://iot-msb-actemium.azure-devices.net")]
        [QueryParam("api-version", "2020-05-31-preview")]
        [SasToken("iot-msb-actemium.azure-devices.net", "iothubowner", "+fMjwkyQJ5r9JZbmOhWDCV/xv1oSliRzRAIoTOg4mBU=", 30)]
        [EnumMember(Value = "iotHub")]
        IotHub,

        [RootUrl("https://api.softgarden.io/api/rest/v3/frontend/jobslist")]
        [EnumMember(Value = "jobOffers")]
        JobOffers,
        
        [RootUrl("https://msb.actemium.ch/api")]
        [HeaderParam("authorization", "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjE5NDE4YTE4LWE1ZmQtNDg4NC1hNTYxLWMxMWU5YmNmNDI5MyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJqb25hcy5oYWJlcmtvcm5AdmluY2ktZW5lcmdpZXMubmV0Iiwic3ViIjoiSldUU2VydmljZUFjY2Vzc1Rva2VuIiwianRpIjoiNzk4MWU2OWMtNWM0OC00ZDlhLWE1MDMtNTA0Zjc3YWRjMzRlIiwiaWF0IjoiMTc1MTU0MjMyMyIsIlVzZXJJZCI6IjE5NDE4YTE4LWE1ZmQtNDg4NC1hNTYxLWMxMWU5YmNmNDI5MyIsIkRpc3BsYXlOYW1lIjoiSm9uYXMgSGFiZXJrb3JuIiwiVXNlck5hbWUiOiJqb25hcy5oYWJlcmtvcm5AdmluY2ktZW5lcmdpZXMubmV0IiwiRW1haWwiOiJqb25hcy5oYWJlcmtvcm5AYWN0ZW1pdW0uY2giLCJleHAiOjIxNDU4MzA0MDAsImlzcyI6Im1pY3Jvc2VydmljZWJ1cy5jb20iLCJhdWQiOiJtaWNyb3NlcnZpY2VidXMuYXBpIn0.gd13Uep34zWUj86R_Ltzx0ENrRq8ZyoRH06lfzhButo")]
        [EnumMember(Value = "microservicebus")]
        Microservicebus,

        // News are searched locally in the db, no need to define request parameters
        [EnumMember(Value = "newsInternal")]
        NewsInternal,

        [RootUrl("https://actemium.ch/wp-json/wp/v2/newspost")]
        [QueryParam("per_page", "10")]
        [EnumMember(Value = "newsPublic")]
        NewsPublic,

        [RootUrl("https://api.openweathermap.org/data")]
        [QueryParam("appid", "a62bb31add1644675a8e5a753cef554f")]
        [EnumMember(Value = "openWeather")]
        OpenWeather,

        [RootUrl("http://transport.opendata.ch/v1/stationboard")]
        [QueryParam("limit","50")]
        [EnumMember(Value = "publicTransport")]
        PublicTransport,

        [RootUrl("https://apigateway.sociabble.com/streams")]
        [HeaderParam("X-Sociabble-SubscriptionKey", "")]
        [EnumMember(Value = "sociabble")]
        Sociabble,

        [RootUrl("https://partner-feeds.20min.ch/rss")]
        [EnumMember(Value = "twentyMin")]
        TwentyMin,
        
        [RootUrl("https://api.twitter.com/1.1")]
        [OAuth2ClientCredentialsToken("https://api.twitter.com/oauth2/token", "4e1e7xyTptlskxWBC1bNgLSCR", "fNIk8p6zJo84zYf8ODqk7h8u2CjB5iFnoqX1ePVwAgFILQID4O", "1187647575454834689-zXbpRSonDtUDXffF07217EPnBluc9a", "EimnrAItIRt4r9J1C1Fa8WxDV2ExGebokSDaqYCCTO8E6")]
        [EnumMember(Value = "twitter")]
        Twitter,

        [RootUrl("https://app1.edoobox.com")]
        [QueryParam("edref", "vesact")]
        [EnumMember(Value = "university")]
        University,

        [RootUrl("https://community-admin.uptownbasel.ch/api/v1")]
        [OAuth2PasswordToken("https://community-admin.uptownbasel.ch/oauth/token", "BaXiNlFcTN7tIhcjRtGLt_cpi0CajuKuw4k1SDr1kjg", "E7K83ghx2TEBvt8DbGfJa1ZAPdzJqNKdvZi_yibu7Sg", "uptown+actemium@panter.ch", "M#Sa7m^HoVqFsh*&fj")]
        [EnumMember(Value = "uptownArticle")]
        UptownArticle,

        [RootUrl("https://community-admin.uptownbasel.ch/api/v1")]
        [OAuth2PasswordToken("https://community-admin.uptownbasel.ch/oauth/token", "BaXiNlFcTN7tIhcjRtGLt_cpi0CajuKuw4k1SDr1kjg", "E7K83ghx2TEBvt8DbGfJa1ZAPdzJqNKdvZi_yibu7Sg", "uptown+actemium@panter.ch", "M#Sa7m^HoVqFsh*&fj")]
        [EnumMember(Value = "uptownEvent")]
        UptownEvent,

        // Menu is searched over a single well defined url, no need to define request parameters or authentication
        [EnumMember(Value = "uptownMenu")]
        UptownMenu,
    }
}
