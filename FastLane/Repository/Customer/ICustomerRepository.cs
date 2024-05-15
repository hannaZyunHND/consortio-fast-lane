using FastLane.Dtos.Customer;

namespace FastLane.Repository.Customer
{
    public interface ICustomerRepository
    {
        Task<List<Entities.Customer>> GetAllCustomersAsync();
        Task<Entities.Customer> GetCustomerByIdAsync(int? id);
        Task<Entities.Customer> GetCustomerByEmailAsync(string email);
        Task<int> CreateCustomerAsync(Entities.Customer customer);
        Task<bool> UpdateCustomerAsync(Entities.Customer customer);
        Task<bool> DeleteCustomerAsync(int? customerId);
    }
}
