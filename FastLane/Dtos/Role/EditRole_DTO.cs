using System.ComponentModel.DataAnnotations;

namespace FastLane.Dtos.Role
{
    public class EditRole_DTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public DateTime Updated_at { get; set; }
    }
}
