
using FastLane.Context;
using FastLane.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Repository.Airport
{
    public class AirportRepository : IAirportRepository
    {
        private readonly ApplicationDbContext _context;
        public AirportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAirportAsync(Entities.Airport airport)
        {
            _context.Airports.Add(airport);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAirportAsync(int? airportId)
        {
            if(airportId == null)
            {
                return false;
            }

            var airport = await _context.Airports.FirstOrDefaultAsync(r => r.Id == airportId);

            if(airport == null)
            {
                return false;
            }

            _context.Airports.Remove(airport);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Entities.Airport> GetAirportByIdAsync(int? id)
        {
            if(id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var airport = await _context.Airports.FindAsync(id);
            if(airport == null)
            {
                throw new InvalidOperationException(nameof(airport));
            }
            return airport;
        }

        public async Task<List<Entities.Airport>> GetAllAirportsAsync()
        {
            return  await _context.Airports.ToListAsync();
        }

        public async Task<bool> UpdateAirportAsync(Entities.Airport airport)
        {
            _context.Airports.Update(airport);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
