using Infoscreens.Common.Enumerations;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.CachedData
{
    public class YoutubeVideoListCached
    {
        public ICollection<YoutubeVideoCached> Videos { get; set; }

    }

    public class YoutubeVideoCached
    {
        public string Url { get; set; }
        public string EmbedUrl { get; set; }
        public int Duration { get; set; }
        public eVideoBackground? Background { get; set; }
        public Dictionary<string, string> Title { get; set; }

        public YoutubeVideoCached(string url, string embedUrl, int duration, eVideoBackground? background, Dictionary<string, string> title)
        {
            Url = url;
            EmbedUrl = embedUrl;
            Duration = duration;
            Background = background;
            Title = title;
        }
    }
}
