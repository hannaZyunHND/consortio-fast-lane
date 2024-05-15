using FastLane.Dtos.User;
using FastLane.Models;

namespace FastLane.Service.User
{
    public interface IUserService
    {
        Task<List<Models.User>> GetAllUsersAsync(string role, string email,
                                                 string keyword, DateTime? createdAt,
                                                 string sortBy, bool isAscending);
        Task<Models.UserFinal> GetAllUsers(int pageNumber, int pageSize);
        Task<Models.User_Detail> GetUserByIdAsync(int? id);
        Task<bool> CreateUserAsync(CreateUser_Dto roleCreate_DTO);
        Task<bool> UpdateUserAsync(int? id, EditUser_Dto roleEdit_DTO);
        Task<bool> DeleteUserAsync(int? roleId);
        Task<Models.UserFinal> GetAllSales();
        Task<Models.UserFinal> GetAllOperators(string airport);
    }

}
