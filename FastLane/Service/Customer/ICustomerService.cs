using FastLane.Dtos.Customer;

namespace FastLane.Service.Customer
{
    public interface ICustomerService
    {
        Task<bool> EditCustomerAsync(int? CustomerId, Entities.Customer customer);
        Task<int> CreateCustomerAsync(Entities.Customer customer);
        Task<List<Entities.Customer>> GetAllCustomersAsync();
        Task<Entities.Customer> GetCustomerByIdAsync(int? id);
        Task<bool> DeleteCustomerAsync(int? customerId);
        Task<Entities.Customer> GetCustomerByEmailAsync(string email);
    }
}
