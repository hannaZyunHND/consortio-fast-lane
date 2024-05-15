namespace FastLane.Dtos.Status
{
    public class CreateStatus_DTO
    {
        public string Name { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public DateTime Created_at { get; set; }
    }
}
