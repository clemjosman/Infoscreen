using Newtonsoft.Json;

namespace Infoscreens.Common.Models.ApiResponse
{
    public class UptownUser
    {
        public int Id { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        // public string Settlement { get; set; } // Commented our as always null for now so not sure if int or string

        [JsonProperty("sign_in_count")]
        public int SignInCount { get; set; }

        public string Language { get; set; }

        [JsonProperty("about_me")]
        public string AboutMe { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string Avatar { get; set; }

    }
}
