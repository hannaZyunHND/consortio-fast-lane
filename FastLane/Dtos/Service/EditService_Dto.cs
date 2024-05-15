using System.ComponentModel.DataAnnotations;

namespace FastLane.Dtos.Service
{
    public class EditService_Dto
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Updated_at { get; set; }
    }
}
