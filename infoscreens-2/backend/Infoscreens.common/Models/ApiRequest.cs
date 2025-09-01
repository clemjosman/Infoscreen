using System.Collections.Generic;

namespace Infoscreens.Common.Repositories
{
    public class ApiRequest
    {
        public string CachedFileName { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Params { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Postprocessing { get; set; }

        public string UrlExtension { get; set; }

        public object Config { get; set; }
    }
}
