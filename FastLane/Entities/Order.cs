using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastLane.Entities
{
    public class Order : Activity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public int? Customer_ID { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<Order_Detail>? OrderDetails { get; set; }

    }
}
