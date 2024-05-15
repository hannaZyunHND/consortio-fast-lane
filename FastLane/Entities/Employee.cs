using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastLane.Entities
{
    public class Employee : Activity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("User")]
        public int User_Id { get; set; }
        public User? User { get; set; }

        [Required]
        [ForeignKey("Airport")]
        public int Airport_Id { get; set; }
        public Airport? Airport { get; set; }
    }
}
