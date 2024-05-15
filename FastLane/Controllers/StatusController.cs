using FastLane.Dtos.Status;
using FastLane.Service.Status;
using Microsoft.AspNetCore.Mvc;

namespace FastLane.Controllers
{
    [Route("status")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStatuses()
        {
            var status = await _statusService.GetAllStatusesAsync();
            return Ok(status);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatusById(int? id)
        {
            var status = await _statusService.GetStatusByIdAsync(id);
            return Ok(status);
        }


        [HttpGet("/status/role/{id}")]
        public async Task<IActionResult> GetStatusByRole_Id(int? id)
        {
            var status = await _statusService.GetStatusByRole_IdAsync(id);
            return Ok(status);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateStatus(Entities.Status status)
        {
            var result = await _statusService.CreateStatusAsync(status);
            if (result)
            {
                return Ok(new { Message = "Status created successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to create status" });
            }
        }


        [HttpPost("update/{StatusId}")]
        public async Task<IActionResult> UpdateStatus(int? StatusId, EditStatus_DTO status)
        {
            var result = await _statusService.UpdateStatusAsync(StatusId, status);
            if (result)
            {
                return Ok(new { Message = "Status updated successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to create Service" });
            }
        }

        [HttpDelete("delete/{StatusId}")]
        public async Task<IActionResult> DeleteStatus(int? StatusId)
        {
            var result = await _statusService.DeleteStatusAsync(StatusId);
            if (result)
            {
                return Ok(new { Message = "Status deleted successfully" });
            }
            else
            {
                return NotFound(new { Message = "Status not found" });
            }
        }
    }
}
