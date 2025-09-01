using System;

namespace Infoscreens.Common.Enumerations.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class OAuth2ClientCredentialsTokenAttribute : Attribute
    {
        public OAuth2ClientCredentialsTokenAttribute(string url, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            Url = url;
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            AccessToken = accessToken;
            AccessTokenSecret = accessTokenSecret;
        }

        public string Url { get; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; }
        public string AccessTokenSecret { get; }
    }
}
