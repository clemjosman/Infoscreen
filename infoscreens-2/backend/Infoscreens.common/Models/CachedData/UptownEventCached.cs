using Infoscreens.Common.Models.ApiResponse;

namespace Infoscreens.Common.Models.CachedData
{
    public class UptownEventCached
    {
        public string Starting { get; set; }

        public string Ending { get; set; }

        public string Title { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public float? Price { get; set; } // Almost never filled out (null)

        public int? Capacity { get; set; } // Almost never filled out (null)

        public int Id { get; set; }

        public string EventStatus { get; set; } // "cancelled" or "confirmed"

        public string Picture { get; set; } // Never null for now

        public UptownUserCached Author { get; set; }

        public int UsersCount { get; set; }

        public int LikeCount { get; set; }

        public int CommentCount { get; set; }

        public bool CanUserJoins { get; set; }

        public UptownEventCached() { }
        public UptownEventCached(UptownEvent uptownEvent)
        {
            Starting = uptownEvent.Starting;
            Ending = uptownEvent.Ending;
            Title = uptownEvent.Title;
            Location = uptownEvent.Location;
            Description = uptownEvent.Description;
            Price = uptownEvent.Price;
            Capacity = uptownEvent.Capacity;
            Id = uptownEvent.Id;
            EventStatus = uptownEvent.EventStatus;
            Picture = uptownEvent.Picture;
            Author = new UptownUserCached(uptownEvent.Author);
            UsersCount = uptownEvent.UsersCount;
            LikeCount = uptownEvent.LikeCount;
            CommentCount = uptownEvent.CommentCount;
            CanUserJoins = uptownEvent.CanUserJoins;
        }
    }
}
