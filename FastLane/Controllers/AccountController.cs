using FastLane.Context;
using FastLane.Dtos.Account;
using FastLane.Service.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FastLane.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("auth")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public AccountController(ApplicationDbContext context, IAccountService authService)
        {
            _context = context;
            _accountService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(Login_Dto userLogin_DTO)
        {
            try
            {
                if (userLogin_DTO == null)
                {
                    return BadRequest(new { message = "Invalid user information" });
                }
                var result = await _accountService.Authenticate(userLogin_DTO);

                if (result.Success)
                {
                    return Ok(new { token = result.Token });
                }

                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");

            return Ok(new { message = "Logged out successfully" });
        }
    }
}
