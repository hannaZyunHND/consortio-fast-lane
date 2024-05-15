using System.ComponentModel.DataAnnotations;

namespace FastLane.Dtos.Account
{
    public class Login_Dto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
