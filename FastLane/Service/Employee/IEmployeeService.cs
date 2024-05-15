using FastLane.Dtos.Employee;

namespace FastLane.Service.Employee
{
    public interface IEmployeeService
    {
        Task<Models.EmployeeFinal>GetAllEmployeesAsync(int? index, int? pageSize, int user_id, bool isOperator);
        Task<Entities.Employee> GetEmployeeByIdAsync(int? id);
        Task<bool> CreateEmployeeAsync(CreateEmployee_Dto employeeCreate_DTO);
        Task<bool> UpdateEmployeeAsync(int? id, EditEmployee_Dto employeeEdit_DTO);
        Task<bool> DeleteEmployeeAsync(int? id);
    }

}
