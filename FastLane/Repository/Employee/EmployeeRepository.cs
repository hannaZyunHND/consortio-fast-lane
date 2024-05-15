using FastLane.Context;
using FastLane.Dtos.Employee;
using FastLane.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Repository.Employee
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Entities.Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employee.ToListAsync();
        }

        public async Task<bool> CreateEmployeeAsync(Entities.Employee employee)
        {
            try
            {
                _context.Employee.Add(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(Entities.Employee employee)
        {
            _context.Employee.Update(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int? Id)
        {
            var employee = await _context.Employee.FirstOrDefaultAsync(r => r.Id == Id);
            if (employee == null)
            {
                return false;
            }

            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Entities.Employee> GetEmployeeByIdAsync(int? id)
        {
            var employee = await _context.Employee.FirstOrDefaultAsync(r => r.Id == id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {id} not found");
            }

            return employee;
        }

        public async Task<List<Entities.Employee>> GetEmployeesByAirportAsync(string airportName)
        {
            return await _context.Employee
                .Include(e => e.Airport)
                .Where(e => e.Airport.Name == airportName)
                .ToListAsync();
        }
    }
}
