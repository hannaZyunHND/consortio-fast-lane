using FastLane.Dtos.User;

namespace FastLane.Repository.User
{
    public interface IUserRepository
    {
        Task<List<Models.User>> GetAllUsersAsync(string role, string email, string keyword, DateTime? createdAt, string sortBy, bool isAscending);
        Task<Models.UserFinal> GetAllUsers(int pageNumber, int pageSize);
        Task<Models.User_Detail> GetUserByIdAsync(int? id);
        Task<bool> CreateUserAsync(Entities.User role);
        Task<bool> UpdateUserAsync(Entities.User role);
        Task<bool> DeleteUserAsync(int? roleId);
        Task<Models.UserFinal> GetAllSales();
        Task<Models.UserFinal> GetAllOperators(string airport);
    }
}
