using DocumentFormat.OpenXml.Wordprocessing;
using FastLane.Context;
using FastLane.Dtos.User;
using FastLane.Entities;
using FastLane.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.UserFinal> GetAllUsers(int pageNumber, int pageSize)
        {
            IQueryable<Entities.User> query = _context.Users
                .Include(u => u.Customer)
                .Include(u => u.UserRole)
                    .ThenInclude(ur => ur.Role)
                .OrderByDescending(u => u.Updated_at);

            //if (includeAgency)
            //{
            //    query = query.Where(u => u.UserRole.Any(ur => ur.Role.Name == "Agency"));
            //}
            //else
            //{
            //    query = query.Where(u => !u.UserRole.Any(ur => ur.Role.Name == "Agency"));
            //}

            var totalUsers = await query.CountAsync();

            var usersWithRoles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var usersDto = usersWithRoles.Select(u => new Models.User
            {
                Id = u.Id,
                Name = u.Customer?.Name ?? "Unknown",
                Email = u.Customer?.Email ?? "Unknown",
                Role = u.UserRole.FirstOrDefault()?.Role.Name ?? "Unknown",
                Created_at = u.Created_at,
                Updated_at = u.Updated_at,
            }).ToList();

            return new Models.UserFinal
            {
                TotalCount = totalUsers,
                Users = usersDto,
            };
        }

        public async Task<bool> CreateUserAsync(Entities.User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(Entities.User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int? userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId), "UserId is required");
            }

            var role = await _context.Users.FirstOrDefaultAsync(r => r.Id == userId);
            if (role == null)
            {
                return false;
            }

            _context.Users.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Models.User_Detail> GetUserByIdAsync(int? id)
        {
            var userWithRoles = await _context.Users
                .Include(u => u.Customer)
                .Include(u => u.UserRole)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userWithRoles == null)
            {
                return null;
            }

            var userDto = new Models.User_Detail
            {
                Id = userWithRoles.Id,
                Name = userWithRoles.Customer?.Name ?? "Unknown",
                Email = userWithRoles.Customer?.Email ?? "Unknown",
                Role_ID = userWithRoles.UserRole.FirstOrDefault()?.Role.Id ?? 0,
                Created_at = userWithRoles.Created_at,
                Updated_at = userWithRoles.Updated_at,
            };

            return userDto;
        }

        Task<List<Models.User>> IUserRepository.GetAllUsersAsync(string user, string email, string keyword, DateTime? createdAt, string sortBy, bool isAscending)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.UserFinal> GetAllSales()
        {
            var query = _context.Users
                .Include(u => u.Customer)
                .Include(u => u.UserRole)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRole.Any(ur => ur.Role.Name == "Sales"));

            var totalSales = await query.CountAsync();
            var salesWithRoles = await query.ToListAsync();

            var salesDto = salesWithRoles.Select(u => new Models.User
            {
                Id = u.Id,
                Name = u.Customer?.Name ?? "Unknown",
                Email = u.Customer?.Email ?? "Unknown",
                Role = u.UserRole.FirstOrDefault()?.Role.Name ?? "Unknown",
                Created_at = u.Created_at,
                Updated_at = u.Updated_at,
            }).ToList();

            return new Models.UserFinal
            {
                TotalCount = totalSales,
                Users = salesDto,
            };
        }


        public async Task<UserFinal> GetAllOperators(string airport)
        {
            // Lấy danh sách user có vai trò là "Operator" và sân bay trùng khớp
            var query = _context.Users
                    .Include(u => u.Customer)
                    .Include(u => u.Employees)
                    .ThenInclude(e => e.Airport)
                    .Include(u => u.UserRole)
                        .ThenInclude(ur => ur.Role)
                    .Where(u => u.UserRole.Any(ur => ur.Role.Name == "Operator") && u.Employees.Any(e => e.Airport.Name == airport));

            // Đếm tổng số người dùng
            var totalOperators = await query.CountAsync();

            // Lấy danh sách người dùng
            var operatorsWithRoles = await query.ToListAsync();

            // Chuyển đổi danh sách người dùng sang DTO
            var operatorsDto = operatorsWithRoles.Select(u => new Models.User
            {
                Id = u.Id,
                Name = u.Customer?.Name ?? "Unknown",
                Email = u.Customer?.Email ?? "Unknown",
                Role = u.UserRole.FirstOrDefault()?.Role.Name ?? "Unknown",
                Created_at = u.Created_at,
                Updated_at = u.Updated_at,
            }).ToList();

            // Trả về kết quả
            return new UserFinal
            {
                TotalCount = totalOperators,
                Users = operatorsDto,
            };
        }

    }
}
