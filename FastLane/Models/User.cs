using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FastLane.Models
{
    public class UserFinal
    {
        public int TotalCount { get; set; }
        public List<Models.User> Users { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;

        [JsonProperty("created_at")]
        public DateTime Created_at { get; set; }

        [JsonProperty("updated_at")]
        public DateTime Updated_at { get; set; }
    }
}
