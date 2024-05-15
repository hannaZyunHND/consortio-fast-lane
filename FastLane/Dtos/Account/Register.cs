using FastLane.Entities;
using System.ComponentModel.DataAnnotations;

namespace FastLane.Dtos.Account
{
    public class Register : Activity
    {
        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Passwword { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
