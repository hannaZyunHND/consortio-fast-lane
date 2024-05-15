using System.ComponentModel.DataAnnotations;

namespace FastLane.Models
{
    public class Order_Detail
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Customer_Id { get; set; }
        public string Phone { get; set; } = string.Empty;
        public int Employee {  get; set; }
        public string Passport_Number { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Flight_Number { get; set; } = string.Empty;
        public string? Passport_File { get; set; }
        public string? Visa_File { get; set; }
        public string? Portrait_File { get; set; }
        public string AirPort { get; set; } = string.Empty;
        public string GroupReference { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Status_Sales {  get; set; } = string.Empty;
        public string Status_Opeartor { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Operator_Note {  get; set; } = string.Empty;
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public DateTime Service_Time { get; set; }
    }
}
