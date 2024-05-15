using DocumentFormat.OpenXml.Spreadsheet;
using FastLane.Context;
using FastLane.Controllers;
using FastLane.Dtos.User;
using FastLane.Entities;
using FastLane.Models;
using FastLane.Repository.Customer;
using FastLane.Repository.Role;
using FastLane.Repository.User;
using FastLane.Service.Customer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Service.User
{
    public class UserService : IUserService
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly PasswordHelper _passwordHelper;
        public UserService(IUserRepository userRepository, ApplicationDbContext context, ICustomerRepository customerRepository,
                           PasswordHelper passwordHelper, ICustomerService customerService, IRoleRepository roleRepository)
        {
            _context = context;
            _passwordHelper = passwordHelper;
            _userRepository = userRepository;
            _customerService = customerService;
            _roleRepository = roleRepository;
            _customerRepository = customerRepository;
        }

        public async Task<bool> EditUserAsync(int? Userid, EditUser_Dto userEdit_DTO)
        {
            var userDB = await _userRepository.GetUserByIdAsync(Userid);
            if (userDB == null)
            {
                return false;
            }

            userDB.Updated_at = userEdit_DTO.Updated_at;

            //await _userRepository.UpdateUserAsync(userDB);

            return true;
        }

        public async Task<bool> CreateUserAsync(CreateUser_Dto userCreate_DTO)
        {
            var customer = new Entities.Customer
            {
                Name = userCreate_DTO.Name,
                Email = userCreate_DTO.Email,
                Created_at = userCreate_DTO.Created_at,
            };

            int customerId = await _customerService.CreateCustomerAsync(customer);

            var user = new Entities.User
            {
                Customer_ID = customerId,
                Password = _passwordHelper.HashPassword(userCreate_DTO.Password),
                Created_at = DateTime.Now,
            };

            await _userRepository.CreateUserAsync(user);

            int user_id = user.Id;
            int role_id = _context.Roles.Where(r => r.Name == "Agency").Select(r => r.Id).FirstOrDefault();

            var userRole = new UserRole()
            {
                User_Id = user_id,
                Role_Id = role_id
            };
            _context.UserRole.Add(userRole);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Models.User>> GetAllUsersAsync(string role, string email, string keyword, DateTime? createdAt, string sortBy, bool isAscending)
        {
            return await _userRepository.GetAllUsersAsync(role, email, keyword, createdAt, sortBy, isAscending);
        }
        public async Task<Models.UserFinal> GetAllUsers(int pageNumber, int pageSize)
        {
            return await _userRepository.GetAllUsers(pageNumber, pageSize);
        }

        public async Task<Models.User_Detail> GetUserByIdAsync(int? id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<bool> UpdateUserAsync(int? id, EditUser_Dto userEdit_DTO)
        {
            var userDB = await _context.Users.FindAsync(id);
            if (userDB == null)
            {
                return false;
            }

            var customer = _context.Customers.FirstOrDefault(r => r.Id == userDB.Customer_ID);
            if (customer == null)
            {
                return false;
            }

            customer.Name = userEdit_DTO.Name;
            customer.Email = userEdit_DTO.Email;
            await _customerRepository.UpdateCustomerAsync(customer);

            if (!string.IsNullOrEmpty(userEdit_DTO.Password))
            {
                userDB.Password = _passwordHelper.HashPassword(userEdit_DTO.Password);
            }

            userDB.Updated_at = DateTime.Now;
            await _userRepository.UpdateUserAsync(userDB);

            // Retrieve UserRole
            var userRoleDB = _context.UserRole.FirstOrDefault(r => r.User_Id == id);

            if (userRoleDB != null)
            {
                _context.UserRole.Remove(userRoleDB);
                await _context.SaveChangesAsync();

                var newUserRole = new UserRole
                {
                    User_Id = userDB.Id,
                    Role_Id = userEdit_DTO.Role_ID
                };

                _context.UserRole.Add(newUserRole);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(int? userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }

        public async Task<UserFinal> GetAllSales()
        {
            return await _userRepository.GetAllSales();
        }

        public async Task<UserFinal> GetAllOperators(string airport)
        {
            return await _userRepository.GetAllOperators(airport);
        }
    }
}
