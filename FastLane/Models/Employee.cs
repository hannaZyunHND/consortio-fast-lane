namespace FastLane.Models
{
    public class EmployeeFinal
    {
        public int TotalCount { get; set; }
        public List<Models.Employee> Employees { get; set; }
    }
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }= string.Empty;
        public string Airport {  get; set; }= string.Empty;
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set;}
    }
}
