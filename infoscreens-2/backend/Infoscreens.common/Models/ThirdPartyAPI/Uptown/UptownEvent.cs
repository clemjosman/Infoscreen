using Newtonsoft.Json;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.ApiResponse
{
    public class UptownEvent
    {
        [JsonProperty("event_type")]
        public string EventType { get; set; } // "common"

        //[JsonProperty("gingco_booking_id")]
        // public string GingcoBookingId { get; set; } // Commented our as always null for now so not sure if int or string

        //[JsonProperty("gingco_room_id")]
        // public string GingcoRoomId { get; set; } // Commented our as always null for now so not sure if int or string

        public string Starting { get; set; }

        public string Ending { get; set; }

        public string Title { get; set; }

        public string Location { get; set; }

        [JsonProperty("event_url")]
        public string EventUrl { get; set; }

        public string Description { get; set; }

        public float? Price { get; set; } // Almost never filled out (null)

        public int? Capacity { get; set; } // Almost never filled out (null)

        public int Id { get; set; }

        public string Color { get; set; } // Commented our as always null for now so not sure if int or string

        [JsonProperty("event_status")]
        public string EventStatus { get; set; } // "cancelled" or "confirmed"

        public string Picture { get; set; } // Never null for now

        public UptownUser Author { get; set; }

        public List<UptownUser> Users { get; set; }

        [JsonProperty("users_count")]
        public int UsersCount { get; set; }

        public bool Liked { get; set; }

        public bool Bookmarked { get; set; }

        [JsonProperty("like_count")]
        public int LikeCount { get; set; }

        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        // public string Group { get; set; } // Commented our as always null for now so not sure if int or string

        [JsonProperty("can_user_joins")]
        public bool CanUserJoins { get; set; }

        [JsonProperty("current_user_joined")]
        public bool CurrentUserJoined { get; set; }

    }
}
