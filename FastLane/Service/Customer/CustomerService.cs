using FastLane.Context;
using FastLane.Dtos.Customer;
using FastLane.Repository.Customer;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Service.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ApplicationDbContext _context;

        public CustomerService(ICustomerRepository customerRepository, ApplicationDbContext context)
        {
            _context = context;
            _customerRepository = customerRepository;
        }

        public async Task<bool> EditCustomerAsync(int? CustomerId, Entities.Customer customer)
        {
            var customerDB = await _customerRepository.GetCustomerByIdAsync(CustomerId);
            if (customerDB == null)
            {
                return false;
            }

            customerDB.Name = customer.Name;
            customerDB.Email = customer.Email;
            customerDB.Updated_at = customer.Updated_at;

            await _customerRepository.UpdateCustomerAsync(customerDB);

            return true;
        }

        public async Task<int> CreateCustomerAsync(Entities.Customer customer)
        {
            return await _customerRepository.CreateCustomerAsync(customer);
        }


        public async Task<List<Entities.Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllCustomersAsync();
        }

        public async Task<Entities.Customer> GetCustomerByIdAsync(int? id)
        {
            return await _customerRepository.GetCustomerByIdAsync(id);
        }

        public async Task<bool> DeleteCustomerAsync(int? customerId)
        {
            return await _customerRepository.DeleteCustomerAsync(customerId);
        }

        public async Task<Entities.Customer> GetCustomerByEmailAsync(string email)
        {
            return await _customerRepository.GetCustomerByEmailAsync(email);
        }
    }
}
