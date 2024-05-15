using FastLane.Dtos.Role;

namespace FastLane.Repository.Role
{
    public interface IRoleRepository
    {
        Task<List<Entities.Role>> GetAllRolesAsync();
        Task<Entities.Role> GetRoleByIdAsync(int? id);
        Task<bool> CreateRoleAsync(Entities.Role role);
        Task<bool> UpdateRoleAsync(Entities.Role role);
        Task<bool> DeleteRoleAsync(int? roleId);
    }
}
