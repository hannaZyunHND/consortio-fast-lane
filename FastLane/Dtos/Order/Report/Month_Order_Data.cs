namespace FastLane.Dtos.Order.Report
{
    public class Month_Order_Data
    {
        public int Year { get; set; }
        public List<Each_Month_Order_Data> MonthlyData { get; set; }
    }

    public class Each_Month_Order_Data
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