using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FastLane.Dtos.Order
{
    public class EditOrder_Dto
    {
        public int Status_ID { get; set; }
        public int? Employee_Id { get; set; }
        public int Status_Sales_Id { get; set; }
        public int? Status_Operator_ID { get; set; }
        public int? CreateBy { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AirPort { get; set; } = string.Empty;
        public string Service_ID { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Operator_Note { get; set; } = string.Empty;
        public string Flight_Number {  get; set; } = string.Empty;
        public string GroupReference { get; set; } = string.Empty;
        public string Passport_Number { get; set; } = string.Empty;
        public DateTime Service_Time { get; set; }
        public DateTime Updated_at { get; set; }
        public string? Passport_File { get; set; }
        public string? Visa_File { get; set; }
        public string? Portrait_File {  get; set; }
        public int? UpdatedBy { get; set; }
    }
}
