using Infoscreens.Common.Models.ApiResponse;

namespace Infoscreens.Common.Models.CachedData
{
    public class UptownUserCached
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Avatar { get; set; }

        public UptownUserCached() { }

        public UptownUserCached(UptownUser user)
        {
            Id = user.Id;
            FullName = user.FullName;
            Avatar = user.Avatar;
        }
    }
}
