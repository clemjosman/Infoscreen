using Infoscreens.Common.Models.Configs;
using System;
using System.Globalization;

namespace Infoscreens.Common.Helpers
{
    public class DateHelper
    {
        public static string GetTwitterDateTemplate()
        {
            return "ddd MMM dd HH:mm:ss +ffff yyyy";
        }

        public static string ToIsoDateTimeString(DateTimeOffset? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString("yyyy-MM-ddTHH:mm:sszzz");
            }
            return null;
        }

        public static string ToLocalDateTimeString(NodeConfig node, DateTimeOffset date)
        {
            return $"{ToLocalDateString(node, date)} {ToLocalTimeString(node, date)}";
        }

        public static string ToLocalDateString(NodeConfig node, DateTimeOffset date)
        {
            return node.FrontendConfig.Language switch
            {
                "DE" => date.ToString("dd.MM.yyyy"),
                _ => date.ToString("dd/MM/yyyy"),
            };
        }

        public static string ToLocalTimeString(NodeConfig node, DateTimeOffset date)
        {
            return node.FrontendConfig.Language switch
            {
                _ => date.ToString("HH:mm"),
            };
        }
    }
}
