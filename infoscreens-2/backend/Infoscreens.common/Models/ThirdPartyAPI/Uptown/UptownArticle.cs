using Newtonsoft.Json;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.ApiResponse
{
    public class UptownArticle_Wrapper
    {
        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }  // Page 1 being the one with the latest articles

        public List<UptownArticle> Articles { get; set; }
    }


    public class UptownArticle
    {
        public int Id { get; set; }

        public string Content { get; set; }

        [JsonProperty("all_settlements")]
        public bool AllSettlements { get; set; }

        [JsonProperty("article_type")]
        public string ArticleType { get; set; } // Either "article" or "poll"

        public UptownUser User { get; set; }

        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        [JsonProperty("like_count")]
        public int LikeCount { get; set; }

        public List<UptownArticle_Picture> Pictures { get; set; }

        public bool Pinned { get; set; }

        public bool Liked { get; set; }

        public bool Bookmarked { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        // public string Group { get; set; } // Commented our as always null for now so not sure if int or string

        public List<UptownArticle_Comment> Comments { get; set; }

        [JsonProperty("video_id")]
        public string VideoId { get; set; } // YouTube Video ID, can be used like this -> https://www.youtube.com/embed/{VideoId}
    }


    public class UptownArticle_Picture
    {
        public int Id { get; set; }

        public string Url { get; set; }
    }

    public class UptownArticle_Comment
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public UptownUser User { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        public bool Liked { get; set; }

        [JsonProperty("like_count")]
        public int LikeCount { get; set; }

    }
}
