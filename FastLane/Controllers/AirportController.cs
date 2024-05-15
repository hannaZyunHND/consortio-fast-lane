using FastLane.Context;
using FastLane.Dtos.Email;
using FastLane.Service.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Controllers
{

    [Route("/api/airport")]
    //[ApiController]
    public class AirportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AirportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAirportAsync()
        {
            var airport = await _context.Airports.ToListAsync();
            return Ok(airport);
        }
    }
}
