using FastLane.Dtos.Status;
using FastLane.Repository.Role;
using FastLane.Repository.Status;

namespace FastLane.Service.Status
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IRoleRepository _roleRepository;
        public StatusService(IStatusRepository statusRepository, IRoleRepository roleRepository)
        {
            _statusRepository = statusRepository;
            _roleRepository = roleRepository;
        }

        public async Task<bool> EditStatusAsync(int? Id, Entities.Status status)
        {
            var statusDB = await _statusRepository.GetStatusByIdAsync(Id);
            if (statusDB == null)
            {
                return false;
            }

            statusDB.Name = status.Name;
            statusDB.Updated_at = status.Updated_at;

            await _statusRepository.UpdateStatusAsync(statusDB);

            return true;
        }

        public async Task<bool> CreateStatusAsync(Entities.Status status)
        {
            var newStatus = new Entities.Status
            {
                Name = status.Name,
                RoleId = status.RoleId,
                Created_at = status.Created_at
            };

            return await _statusRepository.CreateStatusAsync(newStatus);
        }


        public async Task<List<StatusDTO>> GetAllStatusesAsync()
        {
            return await _statusRepository.GetAllStatusesAsync();
        }

        public async Task<Entities.Status> GetStatusByIdAsync(int? id)
        {
            return await _statusRepository.GetStatusByIdAsync(id);
        }

        public async Task<List<Entities.Status>> GetStatusByRole_IdAsync(int? id)
        {
            return await _statusRepository.GetStatusByRole_IdAsync(id);
        }
        public async Task<bool> UpdateStatusAsync(int? id, EditStatus_DTO status)
        {
            var statusDB = await _statusRepository.GetStatusByIdAsync(id);
            if (statusDB == null)
            {
                return false;
            }

            statusDB.Name = status.Name;
            statusDB.Updated_at = status.Updated_at;

            return await _statusRepository.UpdateStatusAsync(statusDB);
        }

        public async Task<bool> DeleteStatusAsync(int? statusId)
        {
            return await _statusRepository.DeleteStatusAsync(statusId);
        }
    }
}
