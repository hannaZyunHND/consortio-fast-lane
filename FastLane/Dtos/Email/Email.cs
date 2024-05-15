namespace FastLane.Dtos.Email
{
    public class Email
    {
        public int Order_Id { get; set; }
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
