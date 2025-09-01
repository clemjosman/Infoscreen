using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Infoscreens.Common.Helpers
{
    public class UrlHelper
    {
        public static readonly string YOUTUBE_WEBSITE_URL_PATTERN = @"^(https?\:\/\/)?(www\.)?(youtube\.com\/watch|youtu\.?be)\/?(.+)$";
        public static readonly string YOUTUBE_WEBSITE_QUERY_PARAMS_PATTERN = @"^(.+)?[\?\&](v=.[^&]+)(.+)?$";
        public static readonly string YOUTUBE_EMBED_URL_PATTERN = @"^(https?\:\/\/)?(www\.)?(youtube\.com\/embed)\/(.[^\/\?\&]+){1}.*$";
        public static readonly string YOUTUBE_EMBED_URL_FORMAT = "https://www.youtube.com/embed/{0}";

        #region Youtube website

        public static string GetVideoIdFromYoutubeWebsiteUrl(string url) {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            // INFO: '()' are optional, 'xxxxxxxx' are the video id, '[]' are for container that can have multiple forms
            // Format allowed: 
            // (https://)(www.)youtube.com/watch[Query containing 'v=xxxxxxxx']
            // (https://)(www.)youtu.be/xxxxxxxx

            string videoId = null;

            // Match the regular expression pattern against the given url
            MatchCollection matches = Regex.Matches(url, YOUTUBE_WEBSITE_URL_PATTERN);


            // Only one match should be found
            if (matches.Count != 1)
            {
                return videoId;
            }

            if (matches[0].Groups.Values.Any(g => g.Value == "youtube.com/watch"))
            {
                // On youtube.com/watch, the video id is in the query parameter as value 'v'
                // The query parameters are in the last group of our regex pattern
                MatchCollection matchesQuery = Regex.Matches(matches[0].Groups.Values.Last().Value, YOUTUBE_WEBSITE_QUERY_PARAMS_PATTERN);

                // Only one match should be found
                if (matchesQuery.Count != 1)
                {
                    return videoId;
                }

                // Get the value from the group starting with "v=" 
                videoId = matchesQuery[0].Groups?
                                         .Values?
                                         .FirstOrDefault(g => g.Value.StartsWith("v="))?
                                         .Value
                                         .Replace("v=", "");

            }
            else if (matches[0].Groups.Values.Any(g => g.Value == "youtu.be"))
            {
                // On youtu.be, the video id is in the path, right after the domain name
                // The path 
                videoId = matches[0].Groups.Values.Last().Value.Split("?")[0];
            }

            return videoId;
        }

        public static bool IsYoutubeWebsiteUrl(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            return !string.IsNullOrWhiteSpace(GetVideoIdFromYoutubeWebsiteUrl(url));
        }

        #endregion Youtube website

        #region Youtube embed

        public static string GetVideoIdFromYoutubeEmbedUrl(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            // INFO: '()' are optional, 'xxxxxxxx' are the video id, '[]' are for container that can have multiple forms
            // Format allowed: 
            // (https://)(www.)youtube.com/embed/xxxxxxxx['/' or '?' then anything]

            string videoId = null;

            // Match the regular expression pattern against the given url
            MatchCollection matches = Regex.Matches(url, YOUTUBE_EMBED_URL_PATTERN);

            // Only one match should be found
            if (matches.Count != 1)
            {
                return videoId;
            }

  
            int groupIndex = 0;
            while(groupIndex < matches[0].Groups.Count -1 && videoId == null)
            {
                if(matches[0].Groups[groupIndex].Value == "youtube.com/embed" && groupIndex + 1 <= matches[0].Groups.Count - 1)
                {
                    videoId = matches[0].Groups[groupIndex + 1].Value;
                }
                groupIndex++;
            }

            return videoId;
        }

        public static bool IsYoutubeEmbedUrl(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            return !string.IsNullOrWhiteSpace(GetVideoIdFromYoutubeEmbedUrl(url));
        }

        public static string GenerateYoutubeEmbedUrlFromYoutubeWebsiteUrl(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            var videoId = GetVideoIdFromYoutubeWebsiteUrl(url);

            if(string.IsNullOrWhiteSpace(videoId))
            {
                throw new Exception($"Could not generate embed url from following url: {url}.");
            }

            return string.Format(YOUTUBE_EMBED_URL_FORMAT, videoId);
        }

        #endregion Youtube embed

        #region Query parameters

        /// <summary>
        /// Parses an array of ids. Returns an empty array if none could be parsed of if null has been provided.
        /// </summary>
        /// <param name="param">A string like "21,57,184"</param>
        /// <returns>A list of IDs</returns>
        public static List<int> ParseIdArray(string param)
        {
            return param?
                   .Split(",")
                   .Select(id => {
                       try {
                           return int.Parse(id);
                       }
                       catch (Exception) { 
                           return 0;
                       }
                   })
                   .Where(id => id > 0)
                   .ToList()
                   ?? new List<int>();
        }

        #endregion
    }
}
