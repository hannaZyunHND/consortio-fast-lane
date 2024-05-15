using FastLane.Context;
using FastLane.Dtos.Service;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Repository.Service
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;
        public ServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateService(Entities.Service order)
        {
            try
            {
                _context.Services.Add(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteService(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            var service = await _context.Services.FirstOrDefaultAsync(r => r.Id == id);
            if (service == null)
            {
                return false;
            }

            _context.Services.Remove(service);
            _context.SaveChanges();
            return true;
        }

        public async Task<List<Entities.Service>> GetAllServices()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<Entities.Service> GetServiceById(int? id)
        {

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var order = await _context.Services.FirstOrDefaultAsync(r => r.Id == id);
            if (order == null)
            {
                throw new ArgumentException($"Service with Id {id} is not found");
            }

            return order;
        }

        public async Task<bool> UpdateService(Entities.Service order)
        {
            _context.Services.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
