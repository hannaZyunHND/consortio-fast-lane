namespace FastLane.Dtos.Agency
{
    public class AgencyOrderData
    {
        public int Year { get; set; }
        public List<MonthOrderData> MonthlyData { get; set; }
    }

    public class MonthOrderData
    {
        public int Month { get; set; }
        public int Total { get; set; }
        public int PendingCount { get; set; }
        public int CancelCount { get; set; }
        public int ConfirmCount { get; set; }
        public int CompletedCount { get; set; }
        public int UncompletedCount { get; set; }
    }
}
