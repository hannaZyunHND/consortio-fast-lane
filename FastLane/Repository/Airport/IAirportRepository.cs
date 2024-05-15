namespace FastLane.Repository.Airport
{
    public interface IAirportRepository
    {
        Task<List<Entities.Airport>> GetAllAirportsAsync();
        Task<Entities.Airport> GetAirportByIdAsync(int? id);
        Task<bool> CreateAirportAsync(Entities.Airport airport);
        Task<bool> UpdateAirportAsync(Entities.Airport airport);
        Task<bool> DeleteAirportAsync(int? airportId);
    }
}
