using FastLane.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; 

namespace FastLane.Entities
{
    public class User : Activity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Password { get; set; } = string.Empty;

        //Map to customer table
        [Required]
        [ForeignKey("Customer")]
        public int Customer_ID { get; set; }
        public Customer? Customer { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; } = new List<UserRole>();
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
