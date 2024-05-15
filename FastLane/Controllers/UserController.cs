using FastLane.Dtos.User;
using FastLane.Entities;
using FastLane.Models;
using FastLane.Service.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Globalization;

namespace FastLane.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(string user, string email, string keyword, DateTime? createdAt, string sortBy, bool isAscending)
        {
            var users = await _userService.GetAllUsersAsync(user, email, keyword, createdAt, sortBy, isAscending);
            return Ok(users);
        }

        [HttpGet("index")]
        public async Task<IActionResult> GetAllUserDB(int pageNumber, int pageSize)
        {
            var users = await _userService.GetAllUsers(pageNumber, pageSize);
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int? id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUser_Dto userCreate_DTO)
        {
            var result = await _userService.CreateUserAsync(userCreate_DTO);
            if (result)
            {
                return Ok(new { Message = "User created successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to create user" });
            }
        }

        [HttpPut("update/{UserId}")]
        public async Task<IActionResult> UpdateUser(int? UserId, EditUser_Dto userEdit_DTO)
        {
            var result = await _userService.UpdateUserAsync(UserId, userEdit_DTO);
            if (result)
            {
                return Ok(new { Message = "User updated successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to create User" });
            }
        }

        [HttpDelete("delete/{UserId}")]
        public async Task<IActionResult> DeleteUser(int? UserId)
        {
            var result = await _userService.DeleteUserAsync(UserId);
            if (result)
            {
                return Ok(new { Message = "User deleted successfully" });
            }
            else
            {
                return NotFound(new { Message = "User not found" });
            }
        }
    }
}
