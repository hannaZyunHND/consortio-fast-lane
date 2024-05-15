using FastLane.Dtos.Customer;
using FastLane.Service.Customer;
using Microsoft.AspNetCore.Mvc;

namespace FastLane.Controllers
{
    [Route("customer")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int? id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            return Ok(customer);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer(Entities.Customer customer)
        {
            var result = await _customerService.CreateCustomerAsync(customer);
            if (result != null)
            {
                return Ok(new { Message = "Customer created successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to create customer" });
            }
        }


        [HttpPost("update/{CustomerId}")]
        public async Task<IActionResult> UpdateCustomer(int? CustomerId, Entities.Customer customer)
        {
            var result = await _customerService.EditCustomerAsync(CustomerId, customer);
            if (result)
            {
                return Ok(new { Message = "Customer updated successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to create Service" });
            }
        }

        [HttpDelete("delete/{CustomerId}")]
        public async Task<IActionResult> DeleteCustomer(int? CustomerId)
        {
            var result = await _customerService.DeleteCustomerAsync(CustomerId);
            if (result)
            {
                return Ok(new { Message = "Customer deleted successfully" });
            }
            else
            {
                return NotFound(new { Message = "Customer not found" });
            }
        }
    }
}
