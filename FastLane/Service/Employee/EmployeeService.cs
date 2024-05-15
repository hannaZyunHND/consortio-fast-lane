using FastLane.Context;
using FastLane.Controllers;
using FastLane.Dtos.Employee;
using FastLane.Entities;
using FastLane.Models;
using FastLane.Repository.Customer;
using FastLane.Repository.Employee;
using FastLane.Repository.Role;
using FastLane.Repository.User;
using FastLane.Service.Customer;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Service.Employee
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ApplicationDbContext _context;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHelper _passwordHelper;
        private readonly ICustomerService _customerService;

        public EmployeeService(IEmployeeRepository employeeRepository, 
                                ApplicationDbContext context, 
                                IUserRepository userRepository,
                                ICustomerRepository customerRepository,
                                PasswordHelper passwordHelper, 
                                ICustomerService customerService, 
                                IRoleRepository roleRepository)
        {
            _context = context;
            _passwordHelper = passwordHelper;
            _userRepository = userRepository;
            _customerService = customerService;
            _roleRepository = roleRepository;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<bool> UpdateEmployeeAsync(int? id, EditEmployee_Dto employeeEdit_DTO)
        {
            var employeeDB = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employeeDB == null)
            {
                return false;
            }

            employeeDB.User_Id = employeeEdit_DTO.User_Id;
            employeeDB.Airport_Id = employeeEdit_DTO.Airport_Id;
            employeeDB.Updated_at = DateTime.Now;

            await _employeeRepository.UpdateEmployeeAsync(employeeDB);

            return true;
        }

        public async Task<bool> CreateEmployeeAsync(CreateEmployee_Dto employeeCreate_DTO)
        {
            var customer = new Entities.Customer
            {
                Name = employeeCreate_DTO.Name,
                Email = employeeCreate_DTO.Email,
                Created_at = employeeCreate_DTO.Created_at,
            };

            int customerId = await _customerService.CreateCustomerAsync(customer);

            var user = new Entities.User
            {
                Customer_ID = customerId,
                Password = _passwordHelper.HashPassword(employeeCreate_DTO.Password),
                Created_at = DateTime.Now,
            };

            await _userRepository.CreateUserAsync(user);

            int user_id = user.Id;
            int role_id = _context.Roles.Where(r => r.Name == "Employee").Select(r => r.Id).FirstOrDefault();

            var userRole = new UserRole()
            {
                User_Id = user_id,
                Role_Id = role_id
            };
            _context.UserRole.Add(userRole);
            await _context.SaveChangesAsync();

            var employee = new Entities.Employee
            {
                User_Id = user_id,
                Airport_Id = employeeCreate_DTO.Airport_Id,
                Created_at = employeeCreate_DTO.Created_at,
            };

            return await _employeeRepository.CreateEmployeeAsync(employee);
        }
        
        public async Task<Models.EmployeeFinal> GetAllEmployeesAsync(int? index, int? pageSize, int customer_id, bool isOperator) // bien operator truyen vao day la vo nghia vi co the lay ra tu customer_id
        {
            int pageIndex = index ?? 1;
            int size = pageSize ?? 20;
            int skip = (pageIndex - 1) * size;

            IQueryable<Entities.Employee> query;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Customer_ID == customer_id);

            // Truy vấn để lấy airport_id từ user_id đầu vào
            int? airportId = await _context.Employee
                .Where(e => e.User_Id == user.Id)
                .Select(e => e.Airport_Id)
                .FirstOrDefaultAsync();
            if (airportId > 0)
            {
                isOperator = true;
            }
            if (isOperator)
            {
                // Lấy danh sách các user có cùng airport_id
                query = _context.Employee
                    .Include(e => e.Airport)
                    .Include(e => e.User)
                    .ThenInclude(r => r.Customer)
                    .Where(e => e.Airport_Id == airportId);
            }
            else
            {
                query = _context.Employee
                    .Include(e => e.Airport)
                    .Include(e => e.User)
                    .ThenInclude(r => r.Customer);
            }

            var totalCount = await query.CountAsync();

            var employees = await query
                .OrderByDescending(e => e.Updated_at)
                .Skip(skip)
                .Take(size)
                .ToListAsync();

            var modelEmployees = employees.Select(e => new Models.Employee
            {
                Id = e.Id,
                Created_at = e.Created_at,
                Updated_at = e.Updated_at,
                Name = e.User.Customer.Name ?? "Unknown",
                Airport = e.Airport.Name ?? "Unknown"
            }).ToList();

            return new EmployeeFinal
            {
                TotalCount = totalCount,
                Employees = modelEmployees
            };
        }


        public async Task<Entities.Employee> GetEmployeeByIdAsync(int? id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {id} not found");
            }

            return employee;
        }

        public async Task<bool> DeleteEmployeeAsync(int? id)
        {
            return await _employeeRepository.DeleteEmployeeAsync(id);
        }
    }
}
