using FastLane.Dtos.Employee;

namespace FastLane.Repository.Employee
{
    public interface IEmployeeRepository
    {
        Task<List<Entities.Employee>> GetAllEmployeesAsync();
        Task<Entities.Employee> GetEmployeeByIdAsync(int? id);
        Task<bool> CreateEmployeeAsync(Entities.Employee employee);
        Task<bool> UpdateEmployeeAsync(Entities.Employee employee);
        Task<bool> DeleteEmployeeAsync(int? Id);
        Task<List<Entities.Employee>> GetEmployeesByAirportAsync(string airportName);
    }
}
