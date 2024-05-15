namespace FastLane.Dtos.Order
{
    public class OrderParams
    {
        public int Index { get; set; } 
        public int PageSize { get; set; }
        public int? Status { get; set; }
        public DateTime? toDate { get; set; }
        public DateTime? fromDate {  get; set; }
        public int? Agency_Id { get; set; }
        //public bool IsAgency { get; set; }
        public string? Keyword { get; set; } = string.Empty;
        public string? AirPort { get; set; } = string.Empty;
    }
}
