using System;

namespace Infoscreens.Common.Models.Tokens
{
    interface IToken
    {
        DateTime? ExpiryDate { get; set; }

        string Token { get; set; }
    }
}
