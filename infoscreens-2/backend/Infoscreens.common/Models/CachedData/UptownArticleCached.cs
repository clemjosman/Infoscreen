using Infoscreens.Common.Models.ApiResponse;
using System.Collections.Generic;
using System.Linq;

namespace Infoscreens.Common.Models.CachedData
{

    public class UptownArticleCached
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string ArticleType { get; set; } // Either "article" or "poll"

        public UptownUserCached User { get; set; }

        public int CommentCount { get; set; }

        public int LikeCount { get; set; }

        public List<UptownArticleCached_Picture> Pictures { get; set; }

        public string CreatedAt { get; set; }

        public string VideoId { get; set; } // YouTube Video ID, can be used like this -> https://www.youtube.com/embed/{VideoId}


        public UptownArticleCached() { }
        public UptownArticleCached(UptownArticle article)
        {
            Id = article.Id;
            Content = article.Content;
            ArticleType = article.ArticleType;
            User = new UptownUserCached(article.User);
            CommentCount = article.CommentCount;
            LikeCount = article.LikeCount;
            Pictures = article.Pictures.Select(p => new UptownArticleCached_Picture(p)).ToList();
            CreatedAt = article.CreatedAt;
            VideoId = article.VideoId;
        }
    }


    public class UptownArticleCached_Picture
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public UptownArticleCached_Picture() { }
        public UptownArticleCached_Picture(UptownArticle_Picture picture)
        {
            Id = picture.Id;
            Url = picture.Url;
        }
    }
}
