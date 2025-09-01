using Infoscreens.Common.Enumerations.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Infoscreens.Common.Helpers.Enumerations
{
    public static class EnumHeaderParamHelper
    {
        public static IEnumerable<KeyValuePair<string, string>> GetHeaders(this object enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo == null || memInfo.Length == 0)
                return new List<KeyValuePair<string, string>>();

            var attributes = memInfo[0].GetCustomAttributes(typeof(HeaderParamAttribute), false);
            return (attributes.Length > 0) ?
                   ((HeaderParamAttribute[])attributes).Select(a => new KeyValuePair<string, string>(a.Key, a.Value)).ToList()
                   : new List<KeyValuePair<string, string>>();
        }
    }
}