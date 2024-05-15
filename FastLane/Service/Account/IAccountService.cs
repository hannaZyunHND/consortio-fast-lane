using FastLane.Dtos.Account;

namespace FastLane.Service.Account
{
    public interface IAccountService
    {
        Task<AuthenticationResult> Authenticate(Login_Dto userLogin_DTO);
    }

    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string Token { get; set; } = null!;
    }
}
