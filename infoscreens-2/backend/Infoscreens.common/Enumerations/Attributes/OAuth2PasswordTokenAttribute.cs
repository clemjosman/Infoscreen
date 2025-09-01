using System;

namespace Infoscreens.Common.Enumerations.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class OAuth2PasswordTokenAttribute : Attribute
    {
        public OAuth2PasswordTokenAttribute(string url, string clientId, string clientSecret, string username, string password)
        {
            Url = url;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Username = username;
            Password = password;
        }

        public string Url { get; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; }
        public string Password{ get; }
    }
}
