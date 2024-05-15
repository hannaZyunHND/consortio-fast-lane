namespace FastLane.Dtos.Employee
{
    public class CreateEmployee_Dto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
        public int Airport_Id { get; set; }
        public DateTime Created_at { get; set; }
    }
}
