using FastLane.Dtos.Role;
using FastLane.Service.Role;
using Microsoft.AspNetCore.Mvc;

namespace FastLane.Controllers
{
    [Route("roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int? id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            return Ok(role);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(CreateRole_DTO roleCreate_DTO)
        {
            var result = await _roleService.CreateRoleAsync(roleCreate_DTO);
            if (result)
            {
                return Ok(new { Message = "Role created successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to create role" });
            }
        }


        [HttpPost("update/{RoleId}")]
        public async Task<IActionResult> UpdateRole(int? RoleId, EditRole_DTO roleEdit_DTO)
        {
            var result = await _roleService.UpdateRoleAsync(RoleId, roleEdit_DTO);
            if (result)
            {
                return Ok(new { Message = "Role updated successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to create Service" });
            }
        }

        [HttpDelete("delete/{RoleId}")]
        public async Task<IActionResult> DeleteRole(int? RoleId)
        {
            var result = await _roleService.DeleteRoleAsync(RoleId);
            if (result)
            {
                return Ok(new { Message = "Role deleted successfully" });
            }
            else
            {
                return NotFound(new { Message = "Role not found" });
            }
        }
    }
}
