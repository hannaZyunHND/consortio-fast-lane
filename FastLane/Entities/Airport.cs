using System.ComponentModel.DataAnnotations;

namespace FastLane.Entities
{
    public class Airport : Activity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
