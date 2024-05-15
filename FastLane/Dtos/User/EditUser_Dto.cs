using System.ComponentModel.DataAnnotations;

namespace FastLane.Dtos.User
{
    public class EditUser_Dto
    {
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Role_ID { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
