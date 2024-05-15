﻿namespace FastLane.Dtos.Agency
{
    public class Agency_Order_Data_Year
    {
        public int Year { get; set; }
        public  int Total { get; set; }
        public int PendingCount { get; set; }
        public int CancelCount { get; set; }
        public int ConfirmCount { get; set; }
        public int CompletedCount { get; set; }
        public int UncompletedCount { get; set; }
    }
}
