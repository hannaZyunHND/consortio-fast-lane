namespace FastLane.Dtos.Customer
{
    public class CustomerCreate_DTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Created_at { get; set; }
    }
}
