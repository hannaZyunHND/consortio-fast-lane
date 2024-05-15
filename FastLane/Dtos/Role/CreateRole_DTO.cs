using System.ComponentModel.DataAnnotations;

namespace FastLane.Dtos.Role
{
    public class CreateRole_DTO
    {
        [Required(ErrorMessage = "RoleName is required.")]
        public string RoleName { get; set; } = string.Empty;
        public DateTime Create_at { get; set; }
    }
}
