using System;

namespace Infoscreens.Common.Models.Tokens
{
    public class OAuthToken : IToken
    {
        // Based on the OAuth flow used contains different fields
        public object Attributes { get; set; }


        // IToken
        public DateTime? ExpiryDate { get; set; }
        public string Token { get; set; }
    }
}
