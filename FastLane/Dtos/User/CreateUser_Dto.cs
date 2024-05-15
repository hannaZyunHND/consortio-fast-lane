using System.ComponentModel.DataAnnotations;

namespace FastLane.Dtos.User
{
    public class CreateUser_Dto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime Created_at { get; set; }
    }
}
