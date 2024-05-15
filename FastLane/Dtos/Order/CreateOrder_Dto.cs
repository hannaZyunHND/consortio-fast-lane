using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace FastLane.Dtos.Order
{
    public class CreateOrderRequest
    {
        public int Is_group_order { get; set; }
        public List<OrderItemDto> Orders { get; set; }
    }
    public class OrderItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Passport_Number { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Flight_Number { get; set; } = string.Empty;
        public string Operator_Note {  get; set; } = string.Empty;
        public string AirPort { get; set; } = string.Empty;
        public string Service_ID { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string? Passport_File { get; set; } = string.Empty;
        public string? Visa_File { get; set; }
        public string? Portrait_File { get; set; } = string.Empty;
        public int? CreateBy { get; set; }
        public Boolean Is_group_order { get; set; } 
        public DateTime Service_Time { get; set; }
        public DateTime Created_at { get; set; }
    }
}
