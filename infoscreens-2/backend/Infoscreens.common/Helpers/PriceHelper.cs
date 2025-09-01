namespace Infoscreens.Common.Helpers
{
    class PriceHelper
    {
        public static string ToCurrencyPriceString(string currency, string price)
        {
            return currency switch
            {
                "EUR" => $"{price} {currency}",
                _ => $"{currency} {price}",
            };
        }
    }
}
