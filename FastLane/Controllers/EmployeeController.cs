using FastLane.Dtos.Employee;
using FastLane.Service.Employee;
using Microsoft.AspNetCore.Mvc;

namespace FastLane.Controllers
{
    [Route("employee")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(int? index, int? pageSize, int user_id, bool isOperator)
        {
            var employees = await _employeeService.GetAllEmployeesAsync(index, pageSize, user_id, isOperator);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int? id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            return Ok(employee);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee(CreateEmployee_Dto employee)
        {
            var result = await _employeeService.CreateEmployeeAsync(employee);
            if (result)
            {
                return Ok(new { Message = "Employee created successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to create Employee" });
            }
        }


        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateEmployee(int? id, EditEmployee_Dto employee)
        {
            var result = await _employeeService.UpdateEmployeeAsync(id, employee);
            if (result)
            {
                return Ok(new { Message = "Employee updated successfully" });
            }
            else
            {
                return NotFound(new { Message = "Employee not found" });
            }
        }

        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> DeleteEmployee(int? Id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(Id);
            if (result)
            {
                return Ok(new { Message = "Employee deleted successfully" });
            }
            else
            {
                return NotFound(new { Message = "Employee not found" });
            }
        }
    }
}
