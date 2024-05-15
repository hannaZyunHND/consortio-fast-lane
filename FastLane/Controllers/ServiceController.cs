using FastLane.Dtos.Service;
using FastLane.Service.Service;
using FastLane.Service.Passport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Controllers
{
    [Route("service")]
    [ApiController]
    public class ServiceController : Controller
    {

        private readonly IServiceService _orderService;

        public ServiceController(IServiceService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var orders = await _orderService.GetAllServicesAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int? id)
        {
            var order = await _orderService.GetServiceByIdAsync(id);
            return Ok(order);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateService(CreateService_Dto order)
        {
            var result = await _orderService.CreateServiceAsync(order);
            if (result)
            {
                return Ok(new { Message = "Service created successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to create Service" });
            }
        }


        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateService(int? id, EditService_Dto order)
        {
            var result = await _orderService.UpdateServiceAsync(id, order);
            if (result)
            {
                return Ok(new { Message = "Service updated successfully" });
            }
            else
            {
                return NotFound(new { Message = "Service not found" });
            }
        }

        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> DeleteService(int? Id)
        {
            var result = await _orderService.DeleteServiceAsync(Id);
            if (result)
            {
                return Ok(new { Message = "Service deleted successfully" });
            }
            else
            {
                return NotFound(new { Message = "Service not found" });
            }
        }


    }
}
