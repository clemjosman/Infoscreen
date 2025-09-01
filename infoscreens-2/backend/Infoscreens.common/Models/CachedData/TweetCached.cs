using Infoscreens.Common.Helpers;
using System;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.CachedData
{
    public class TweetCached
    {
        public ulong Id { get; set; }
        public string Content { get; set; }
        public string PublishDate { get; set; }
        public List<string> Hashtags { get; set; }
        public List<string> ImgSrcs { get; set; }
        public string UserName { get; set; }
        public string UserScreenName { get; set; }
        public string UserImgSrc { get; set; }
        public int RetweetCount { get; set; }
        public int FavoriteCount { get; set; }
        public string Url { get; set; }

        public TweetCached(
            ulong id,
            string content,
            DateTimeOffset publishDate,
            List<string> hashtags,
            List<string> imgSrcs,
            string userName,
            string userScreenName,
            string userImgSrc,
            int retweetCount,
            int favoriteCount)
        {
            Id = id;
            Content = content;
            PublishDate = DateHelper.ToIsoDateTimeString(publishDate);
            Hashtags = hashtags;
            ImgSrcs = imgSrcs;
            UserName = userName;
            UserScreenName = userScreenName;
            UserImgSrc = userImgSrc;
            RetweetCount = retweetCount;
            FavoriteCount = favoriteCount;
            Url = $"https://twitter.com/user/status/{id}";
        }
    }
}
