using FastLane.Context;
using FastLane.Dtos.Customer;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Repository.Customer
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Entities.Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<int> CreateCustomerAsync(Entities.Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return customer.Id;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task<bool> UpdateCustomerAsync(Entities.Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int? customerId)
        {
            if (customerId == null)
            {
                throw new ArgumentNullException(nameof(customerId), "CustomerId is required");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(r => r.Id == customerId);
            if (customer == null)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Entities.Customer> GetCustomerByIdAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Customer ID is required");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(r => r.Id == id);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found");
            }

            return customer;
        }

        public async Task<Entities.Customer> GetCustomerByEmailAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email), "Customer's email is required");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(r => r.Email == email);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with Email {email} not found");
            }

            return customer;
        }
    }
}
