using Infoscreens.Common.Enumerations.Attributes;
using System;

namespace Infoscreens.Common.Models.Tokens
{
    public class SasToken : IToken
    {
        public SasTokenAttribute Attributes { get; set; }

        // IToken
        public DateTime? ExpiryDate { get; set; }
        public string Token { get; set; }
    }
}
