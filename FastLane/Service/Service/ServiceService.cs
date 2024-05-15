using FastLane.Dtos.Service;
using FastLane.Repository.Service;
using FastLane.Service.Passport;
using System.Numerics;

namespace FastLane.Service.Service
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }
        public async Task<bool> CreateServiceAsync(CreateService_Dto service)
        {
            if (service == null)
            {
                return false;
            }

            var newService = new Entities.Service
            {
                Name = service.Name,
                Created_at = service.Created_at
            };

            await _serviceRepository.CreateService(newService);
            return true;
        }

        public async Task<bool> DeleteServiceAsync(int? id)
        {
            await _serviceRepository.DeleteService(id);
            return true;
        }

        public async Task<List<Entities.Service>> GetAllServicesAsync()
        {
            return await _serviceRepository.GetAllServices();
        }

        public async Task<Entities.Service> GetServiceByIdAsync(int? id)
        {
            return await _serviceRepository.GetServiceById(id);
        }

        public async Task<bool> UpdateServiceAsync(int? id, EditService_Dto service)
        {
            if (id == null)
            {
                return false;
            }

            var serviceDB = await _serviceRepository.GetServiceById(id);
            if (serviceDB == null)
            {
                return false;
            }

            serviceDB.Name = service.Name;
            serviceDB.Updated_at = DateTime.Now;

            await _serviceRepository.UpdateService(serviceDB);
            return true;
        }
    }
}
