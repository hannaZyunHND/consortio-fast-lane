using FastLane.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastLane.Entities
{
    public class Order_Detail : Activity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Passport_Number { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string? Operator_Note { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
        [Required]
        public string Flight_Number { get; set; } = string.Empty;
        [Required]
        public string AirPort { get; set; } = string.Empty;
        [Required]
        public string Service_ID { get; set; } = string.Empty;
        public string GroupReference { get; set; } = string.Empty;
        public string? Passport_Path { get; set; } = string.Empty;
        public string? Visa_Path { get; set; } = string.Empty;
        public string? Portrait_Path { get; set; } = string.Empty;

        //Map to user table
        [ForeignKey("User")]
        public int? CreateBy { get; set; }
        public int? UpdatedBy { get; set; }
        public User? User { get; set; }

        //Time
        public DateTime Service_Time { get; set; }

        //Map to employee table
        [ForeignKey("Employee")]
        public int? Employee_Id { get; set; }
        public Employee? Employee { get; set; }

        //Map to status table
        [ForeignKey("Status")]
        public int Status_ID { get; set; }
        public int Status_Operator_ID { get; set; }
        public int Status_Sales_Id { get; set; }
        public Status? Status { get; set; }

        [ForeignKey("Order")]
        [Column("OrdersId")] 
        public int OrdersId { get; set; }
        public Order? Orders { get; set; }

    }
}
