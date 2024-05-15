using FastLane.Entities;
using System.ComponentModel.DataAnnotations;

namespace FastLane.Entities
{
    public class Customer : Activity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public ICollection<Order>? Orders { get; set; }
        public ICollection<User> User { get; set;}
    }
}
