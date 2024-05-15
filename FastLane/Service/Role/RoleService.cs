using FastLane.Dtos.Role;
using FastLane.Repository.Role;

namespace FastLane.Service.Role
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<bool> EditRoleAsync(int? Roleid, EditRole_DTO roleEdit_DTO)
        {
            var roleDB = await _roleRepository.GetRoleByIdAsync(Roleid);
            if (roleDB == null)
            {
                return false;
            }

            roleDB.Name = roleEdit_DTO.Name;
            roleDB.Updated_at = roleEdit_DTO.Updated_at;

            await _roleRepository.UpdateRoleAsync(roleDB);

            return true;
        }

        public async Task<bool> CreateRoleAsync(CreateRole_DTO roleCreate_DTO)
        {
            var role = new Entities.Role
            {
                Name = roleCreate_DTO.RoleName,
                Created_at = roleCreate_DTO.Create_at
            };

            return await _roleRepository.CreateRoleAsync(role);
        }

        public async Task<List<Entities.Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllRolesAsync();
        }

        public async Task<Entities.Role> GetRoleByIdAsync(int? id)
        {
            return await _roleRepository.GetRoleByIdAsync(id);
        }

        public async Task<bool> UpdateRoleAsync(int? id, EditRole_DTO roleEdit_DTO)
        {
            var roleDB = await _roleRepository.GetRoleByIdAsync(id);
            if (roleDB == null)
            {
                return false;
            }

            roleDB.Name = roleEdit_DTO.Name;
            roleDB.Updated_at = roleEdit_DTO.Updated_at;

            return await _roleRepository.UpdateRoleAsync(roleDB);
        }

        public async Task<bool> DeleteRoleAsync(int? roleId)
        {
            return await _roleRepository.DeleteRoleAsync(roleId);
        }
    }
}
