using FastLane.Dtos.Status;

namespace FastLane.Repository.Status
{
    public interface IStatusRepository
    {
        Task<List<StatusDTO>> GetAllStatusesAsync();
        Task<Entities.Status> GetStatusByIdAsync(int? id);
        Task<int> GetStatusByName(string name);
        Task<List<Entities.Status>> GetStatusByRole_IdAsync(int? roleId);
        Task<bool> CreateStatusAsync(Entities.Status status);
        Task<bool> UpdateStatusAsync(Entities.Status status);
        Task<bool> DeleteStatusAsync(int? id);
    }
}