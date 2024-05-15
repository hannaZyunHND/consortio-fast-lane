using FastLane.Dtos.Service;

namespace FastLane.Service.Service
{
    public interface IServiceService
    {
        Task<List<Entities.Service>> GetAllServicesAsync();
        Task<Entities.Service> GetServiceByIdAsync(int? id);
        Task<bool> CreateServiceAsync(CreateService_Dto order);
        Task<bool> UpdateServiceAsync(int? id, EditService_Dto order);
        Task<bool> DeleteServiceAsync(int? id);
    }
}
