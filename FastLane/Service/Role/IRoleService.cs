using FastLane.Dtos.Role;

namespace FastLane.Service.Role
{
    public interface IRoleService
    {
        Task<List<Entities.Role>> GetAllRolesAsync();
        Task<Entities.Role> GetRoleByIdAsync(int? id);
        Task<bool> CreateRoleAsync(CreateRole_DTO roleCreate_DTO);
        Task<bool> UpdateRoleAsync(int? id, EditRole_DTO roleEdit_DTO);
        Task<bool> DeleteRoleAsync(int? roleId);
    }

}
