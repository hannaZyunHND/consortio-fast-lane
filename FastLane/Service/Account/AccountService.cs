using FastLane.Context;
using FastLane.Controllers;
using FastLane.Dtos.Account;
using FastLane.Entities;
using FastLane.Service.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FastLane.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly PasswordHelper _passwordHelper;

        public AccountService(IConfiguration configuration, ApplicationDbContext context, PasswordHelper passwordHelper)
        {
            _context = context;
            _configuration = configuration;
            _passwordHelper = passwordHelper;
        }



        public async Task<AuthenticationResult> Authenticate(Login_Dto userLogin_DTO)
        {

            if (userLogin_DTO == null || string.IsNullOrEmpty(userLogin_DTO.Email) || string.IsNullOrEmpty(userLogin_DTO.Password))
            {
                return new AuthenticationResult { Success = false, Token = null };
            }

            if (await Authentication(userLogin_DTO))
            {
                try
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(u => u.Email == userLogin_DTO.Email);

                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Customer_ID == customer.Id);

                    int role_Id = await _context.UserRole.Where(r => r.User_Id == user.Id)
                                                          .Select(r => r.Role_Id)
                                                          .FirstOrDefaultAsync();

                    var role = await _context.Roles.Where(r => r.Id == role_Id)
                                                      .Select(r => r.Name)
                                                      .FirstOrDefaultAsync();


                    // Payload
                    var claims = new[]
                    {
                        new Claim("UserId", customer.Id.ToString()), 
                        new Claim(ClaimTypes.Name, customer.Name),
                        new Claim(ClaimTypes.Email, customer.Email),
                        new Claim(ClaimTypes.Role, role),
                    };

                    // Signature
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);


                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpirationInMinutes"])),
                        signingCredentials: creds
                    );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    // Create and return AuthenticationResult
                    return new AuthenticationResult
                    {
                        Success = true,
                        Token = tokenString
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    throw;
                }
            }
            else
            {
                return new AuthenticationResult { Success = false, Token = null };
            }
        }

        public async Task<bool> Authentication(Login_Dto userLogin_DTO)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(u => u.Email == userLogin_DTO.Email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Customer_ID == customer.Id);

            if (user != null && _passwordHelper.VerifyPassword(userLogin_DTO.Password, user.Password))
            {
                Console.WriteLine("Authentication succeeded");
                return true;
            }

            Console.WriteLine("Authentication failed");
            return false;
        }
    }
}