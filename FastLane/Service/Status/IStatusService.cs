using FastLane.Dtos.Status;

namespace FastLane.Service.Status
{
    public interface IStatusService
    {
        Task<List<StatusDTO>> GetAllStatusesAsync();
        Task<Entities.Status> GetStatusByIdAsync(int? id);
        Task<List<Entities.Status>> GetStatusByRole_IdAsync(int? roleId);
        Task<bool> CreateStatusAsync(Entities.Status status);
        Task<bool> UpdateStatusAsync(int? id, EditStatus_DTO status);
        Task<bool> DeleteStatusAsync(int? statusId);
    }
}
