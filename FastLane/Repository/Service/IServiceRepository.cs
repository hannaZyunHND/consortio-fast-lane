namespace FastLane.Repository.Service
{
    public interface IServiceRepository
    {
        Task<List<Entities.Service>> GetAllServices();
        Task<Entities.Service> GetServiceById(int? id);
        Task<bool> CreateService(Entities.Service service);
        Task<bool> UpdateService(Entities.Service service);
        Task<bool> DeleteService(int? id);
    }
}
