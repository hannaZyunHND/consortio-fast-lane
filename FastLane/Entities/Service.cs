using System.ComponentModel.DataAnnotations;

namespace FastLane.Entities
{
    public class Service : Activity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

    }
}
