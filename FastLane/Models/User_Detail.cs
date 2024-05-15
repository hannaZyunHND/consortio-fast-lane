using Newtonsoft.Json;

namespace FastLane.Models
{
    public class User_Detail
    {

        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("role")]
        public int Role_ID { get; set; }

        [JsonProperty("created_at")]
        public DateTime Created_at { get; set; }

        [JsonProperty("updated_at")]
        public DateTime Updated_at { get; set; }
    }
}
