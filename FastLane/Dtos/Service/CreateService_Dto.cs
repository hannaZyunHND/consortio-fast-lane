using System.ComponentModel.DataAnnotations;

namespace FastLane.Dtos.Service
{
    public class CreateService_Dto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public DateTime Created_at { get; set; }
    }
}
