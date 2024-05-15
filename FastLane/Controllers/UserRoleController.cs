using FastLane.Context;
using FastLane.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastLane.Controllers
{
    [Route("userRole")]
    [ApiController]
    public class UserRoleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserRoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userRoles = await _context.UserRole
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Select(ur => new
                {
                    user_Id = ur.User_Id,
                    user = ur.User.Id,
                    role_Id = ur.Role_Id,
                    role = ur.Role.Name
                })
                .ToListAsync();

            return Ok(userRoles);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.UserRole.Add(userRole);
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = "User role created successfully." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
                }
            }
            return BadRequest(ModelState);
        }

        // PUT: api/authorize/userrole/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, UserRole userRole)
        {
            if (id != userRole.User_Id && id != userRole.Role_Id)
            {
                return BadRequest();
            }

            _context.Entry(userRole).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/authorize/userrole/delete/{id}
        [HttpDelete("userrole/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userRole = await _context.UserRole.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }

            _context.UserRole.Remove(userRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserRoleExists(int id)
        {
            return _context.UserRole.Any(e => e.User_Id == id);
        }
    }
}