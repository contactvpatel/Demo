using Demo.Core.Models;

namespace Demo.Core.Services
{
    public interface ISsoService
    {
        Task<bool> ValidateToken(string token);
        Task<SsoAuthModel> RenewToken(string token, string refreshToken);
        Task<bool> Logout(string token);
    }
}
